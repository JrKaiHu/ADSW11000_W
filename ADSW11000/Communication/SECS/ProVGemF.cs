using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVTool;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ADSW11000
{
    //表示資料儲存的位置
    // REAL -> Class中的Member
    // OPTION -> 通用設定表格
    // PACKAGE -> 當下的產品表格
    // PRELOADPACKAGE -> HOST指定的產品表格
    public enum TableCode { REAL = 0, OPTION, PACKAGE, PRELOADPACKAGE};
    // HOST遠端指令
    public enum HOSTCMD { INITIAL = 0, START, PPSELECT, STOP, PAUSE, RESUME, ABORT, LOTEND};
    // ProVGEM透過事件通知Handler端可能的事件類型
    // QUERY -> 詢問資料
    // WRITE -> 寫入資料
    // COMMAND -> 遠端命令
    // STATECHANGE -> 通訊或控制狀態改變
    // REPLAYTIMEOUT -> 某一通訊命令發生T3 Reply Timeout
    // SPOOLING -> Spooling功能啟動或停止
    public enum EventType { QUERY = 0, WRITE, COMMAND, STATECHANGE, REPLYTIMEOUT, SPOOLING };
    // 使用者針對Package操作的動作 
    public enum PPOPState { CREATE = 1, MODIFY, DELETE};

    public partial class ProVGemF : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        //Status Variable List
        private List<SVData> SVList = new List<SVData>();
        //Data Variable List
        private List<DVData> DVList = new List<DVData>();
        //Equipment Constant List
        private List<ECData> ECList = new List<ECData>();
        //Report List
        private List<RPTData> RPTList = new List<RPTData>();
        //Collection Report List
        private List<EventReport> EVTReportList = new List<EventReport>();
        //Alarm Data List
        private List<ALMData> ArmList = new List<ALMData>();
        //Trace Data List
        private List<TraceDataCollection> TRList = new List<TraceDataCollection>();
        private TraceDataCollection tdc = null;
        //SnFnData List
        private List<SnFnData> SnFnList = new List<SnFnData>();
        //PackageSchema
        private PackageSchema PKGSchema = new PackageSchema();
        //Spool Data Collection
        private SpoolDataCollection SpoolList = new SpoolDataCollection();
        //Limit Monitoring Map
        private Dictionary<ushort, ProVGEMLimitFSM.ProVLimitFSM> LimitMap = new Dictionary<ushort, ProVGEMLimitFSM.ProVLimitFSM>();

        //Communication FSM For Host-Initiated Connect
        private ProVGEMCommFSM.ProVHostInitiatedFSM HostCommFSM = null;
        //Communication FSM For Equipment-Initiated Connect
        private ProVGEMCommFSM.ProVEQPInitiatedFSM EQPCommFSM = null;
        //Control FSM
        private ProVGEMControlFSM.ProVControlFSM CtrlFSM = null;
        //Process FSM
        private ProVGEMProcessFSM.ProVProcessFSM ProcFSM = null;
        //Spooling FSM (SpoolLoad)
        private ProVGEMSpoolFSM.SpoolLoadFSM SpoolLoadFSM = null;
        //Spooling FSM (SpoolUnload)
        private ProVGEMSpoolFSM.SpoolUnloadFSM SpoolUnloadFSM = null;

        //Logger Function
        private ProVTool.Logger commLogger = null;

        //超出監控值的項目
        ProVGEMLimitFSM.ProVLimitFSM CurExceedLimitItem = null;

        //Event For ProVGEM to Inform Handler
        //Handler端需註冊此事件以接收ProVGEM的通知 
        public delegate ExchangeData DataExchangeDelegate(EventType evtType, ExchangeData exData);
        public event DataExchangeDelegate OnDataExchangeEvent;

        public PPOPState PPState
        {
            get;
            set;
        }

        public ProVGemF()
        {
            InitializeComponent();
            TopLevel = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            GetControls(this);
        }

        private void GetControls(Control ctr)
        {
            foreach (Control myctr in ctr.Controls)
            {
                //如果傳進來的控制項有子控制項的話
                if (myctr.HasChildren == true)
                {
                    //就遞迴呼叫自己
                    GetControls(myctr);
                }
            }
        }

        /// <summary>
        /// Handler端透過此函式要求ProVGEM回報事件給Host
        /// </summary>
        /// <param name="EventName">定義在proVSECS.xls中的事件名稱</param>
        public void CustomEventReport(String EventName)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {

                EventReport report = EVTReportList.Find(x => x.EventName == EventName); //Custom Define Event Report
                CurEventName = report.EventName;
                if (report != null)
                {
                    List<byte> sb = report.EncodeEventReport();
                    SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                }
            }
        }

        /// <summary>
        /// Handler端透過此函式要求ProVGEM回報事件給Host
        /// </summary>
        /// <param name="EventID">定義在proVSECS.xls中的事件編號</param>
        public void CustomEventReport(ushort EventID)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {

                EventReport report = EVTReportList.Find(x => x.CEID == EventID); //Custom Define Event Report
                CurEventName = report.EventName;
                if (report != null)
                {
                    List<byte> sb = report.EncodeEventReport();
                    SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                }
            }
        }

        //S5F1 Alarm Report Send
        /// <summary>
        /// Handler端發生或是解除Alarm時向Host端發出事件報告
        /// </summary>
        /// <param name="arm">Alarm 結構</param>
        /// <param name="isSet">發生Alarm或是解除Alarm</param>
        //2019/09/02 By Max For Alarm ID & Alarm Text
        ushort CurAlarmID = 0;
        String CurAlarmText = "";
        public void AlarmReport(ArmMtrl arm, bool isSet = true)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                List<byte> sb = new List<byte>();
                SECS_LIST listData = new SECS_LIST(3);
                SECSEngine.EncodeDataItem(ref sb, listData);
                ushort errCode = (ushort)(arm.ModuleID * 100 + arm.ErrorCode);
                ALMData almData = ArmList.Find(x => x.ALID == errCode);
                if (almData != null)
                {
                    EventReport report = null;
                    if (almData.ALED == 0x01) //if enabled
                    {
                        if (isSet)
                        {
                            int data = almData.ALCD;
                            ProVLib.ProVFuncStd.SetBit(ref data, 7);
                            almData.ALCD = (byte)(data);
                            //Alarm Set Report
                            report = EVTReportList.Find(x => x.CEID == (ushort)(7));
                            CurEventName = report.EventName;
                            CurAlarmID = errCode;
                            CurAlarmText = almData.ALTX;
                        }
                        else
                        {
                            int data = almData.ALCD;
                            ProVLib.ProVFuncStd.ClearBit(ref data, 7);
                            almData.ALCD = (byte)(data);
                            //Alarm Clear Report
                            report = EVTReportList.Find(x => x.CEID == (ushort)(8));
                            CurEventName = report.EventName;
                            CurAlarmID = 0;
                            CurAlarmText = "";
                        }
                        SECS_BINARY alCD = new SECS_BINARY(almData.ALCD);
                        SECSEngine.EncodeDataItem(ref sb, alCD);
                        SECS_U2 alID = new SECS_U2(almData.ALID);
                        SECSEngine.EncodeDataItem(ref sb, alID);
                        SECS_ASCII alTX = new SECS_ASCII(almData.ALTX);
                        SECSEngine.EncodeDataItem(ref sb, alTX);
                        SECSEngine.SendHSMSMessage(5, 1, true, sb.ToArray());

                        sb.Clear();
                        
                        if (report != null)
                        {
                            sb = report.EncodeEventReport();
                            SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                        }
                    }
                    ChangeProcessState(isSet ? ProVGEMProcessFSM.EventID.EVT_ALARMSET : ProVGEMProcessFSM.EventID.EVT_ALARMRST);
                }
            }
        }

        //S7F5 PP Request
        /// <summary>
        /// Handler端向Host要求下載指定PPID對應的內容
        /// </summary>
        /// <param name="ppid">Package 名稱</param>
        private bool IsFormatedPPOperation = true;
        public void PPRequest(String PPID, bool IsFormated = true)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                List<byte> sb = new List<byte>();
                SECS_ASCII ppid = new SECS_ASCII(PPID);
                SECSEngine.EncodeDataItem(ref sb, ppid);
                IsFormatedPPOperation = IsFormated;
                if (IsFormated)
                {
                    SECSEngine.SendHSMSMessage(7, 25, true, sb.ToArray());
                }
                else
                {
                    SECSEngine.SendHSMSMessage(7, 5, true, sb.ToArray());
                }
            }
        }

        //S7F1 PP Load Inquire
        /// <summary>
        /// Handler端向Host詢問是否可上傳指定PPID對應的內容
        /// </summary>
        /// <param name="ppid">Package 名稱</param>
        private String LoadInquirePPID = String.Empty;
        public void PPLoadInquire(String PPID, bool IsFormated = true)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                IsFormatedPPOperation = IsFormated;
                LoadInquirePPID = PPID;

                ExchangeData data = new ExchangeData();
                data.Code = TableCode.REAL;
                data.DataName = "PPACTION";
                data.ParamMap.SetParameter("SELPPID", LoadInquirePPID);
                data = OnDataExchangeEvent(EventType.QUERY, data);
                int iRet = data.RetData.ToInt();

                if (IsFormatedPPOperation)
                {
                    List<byte> sb = new List<byte>();
                    sb.Clear();
                    SECS_LIST lstData = new SECS_LIST(4);
                    SECSEngine.EncodeDataItem(ref sb, lstData);
                    SECS_ASCII ascData = new SECS_ASCII(LoadInquirePPID);
                    SECSEngine.EncodeDataItem(ref sb, ascData);
                    SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                    SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                    lstData = new SECS_LIST(1); //Number of process commands
                    SECSEngine.EncodeDataItem(ref sb, lstData);
                    lstData = new SECS_LIST(2);
                    SECSEngine.EncodeDataItem(ref sb, lstData);
                    SECS_U2 CCODE = new SECS_U2(1); //Command Code
                    SECSEngine.EncodeDataItem(ref sb, CCODE);
                    lstData = new SECS_LIST(PKGSchema.Count); //Number of parameters
                    SECSEngine.EncodeDataItem(ref sb, lstData);
                    for (int i = 0; i < PKGSchema.Count; ++i)
                    {
                        PKGItem Item = PKGSchema[i];
                        data = new ExchangeData();
                        data.Code = TableCode.PRELOADPACKAGE;
                        data.DataName = Item.QueryName;
                        data = OnDataExchangeEvent(EventType.QUERY, data);
                        switch (Item.Format)
                        {
                            case "A":
                                {
                                    SECSData = new SECS_ASCII(data.RetData.ToString());
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                }
                                break;
                            case "BOOL":
                                {
                                    SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                }
                                break;
                            case "F4":
                            case "F8":
                                {
                                    SECSData = new SECS_F8(data.RetData.ToDouble());
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                }
                                break;
                            case "I1":
                            case "I2":
                            case "I4":
                            case "I8":
                            case "U1":
                            case "U2":
                            case "U4":
                            case "U8":
                                {
                                    SECSData = new SECS_I4(data.RetData.ToInt());
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                }
                                break;
                        }
                    }

                    List<byte> sbSend = new List<byte>();
                    lstData = new SECS_LIST(2);
                    SECSEngine.EncodeDataItem(ref sbSend, lstData);
                    SECS_ASCII strData = new SECS_ASCII(PPID); //PPID
                    SECSEngine.EncodeDataItem(ref sbSend, strData);
                    SECS_I4 iData = new SECS_I4(sb.Count); //Length
                    SECSEngine.EncodeDataItem(ref sbSend, iData);
                    SECSEngine.SendHSMSMessage(7, 1, true, sb.ToArray());
                }
                else
                {
                    List<byte> bData = new List<byte>();
                    for (int i = 0; i < PKGSchema.Count; ++i)
                    {
                        PKGItem Item = PKGSchema[i];
                        SECS_LIST lst = new SECS_LIST(2);
                        SECSEngine.EncodeDataItem(ref bData, lst);
                        SECS_ASCII ascData = new SECS_ASCII(Item.Name);
                        SECSEngine.EncodeDataItem(ref bData, ascData);
                        data = new ExchangeData();
                        data.Code = TableCode.PRELOADPACKAGE;
                        data.DataName = Item.QueryName;
                        data = OnDataExchangeEvent(EventType.QUERY, data);
                        switch (Item.Format)
                        {
                            case "A":
                                {
                                    SECS_BASE SECSData = new SECS_ASCII(data.RetData.ToString());
                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                }
                                break;
                            case "BOOL":
                                {
                                    SECS_BASE SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                }
                                break;
                            case "F4":
                            case "F8":
                                {
                                    SECS_BASE SECSData = new SECS_F8(data.RetData.ToDouble());
                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                }
                                break;
                            case "I1":
                            case "I2":
                            case "I4":
                            case "I8":
                            case "U1":
                            case "U2":
                            case "U4":
                            case "U8":
                                {
                                    SECS_BASE SECSData = new SECS_I4(data.RetData.ToInt());
                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                }
                                break;
                        }
                    }

                    List<byte> sb = new List<byte>();
                    SECS_LIST lstData = new SECS_LIST(2);
                    SECSEngine.EncodeDataItem(ref sb, lstData);
                    SECS_ASCII strData = new SECS_ASCII(PPID); //PPID
                    SECSEngine.EncodeDataItem(ref sb, strData);
                    SECS_I4 iData = new SECS_I4(bData.Count); //Length
                    SECSEngine.EncodeDataItem(ref sb, iData);
                    SECSEngine.SendHSMSMessage(7, 1, true, sb.ToArray());
                }
            }
        }

        //Handler Process State Change
        /// <summary>
        /// Handler端機台狀態發生變化時向Host端發出事件報告
        /// </summary>
        /// <param name="evtID">事件ID</param>
        public void ChangeProcessState(ProVGEMProcessFSM.EventID evtID)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {

                if (evtID == ProVGEMProcessFSM.EventID.EVT_START && CurProcState == ProVGEMProcessFSM.StateID.PAUSE)
                {
                    ProcFSM.Send(ProVGEMProcessFSM.EventID.EVT_RESUME);
                }
                else
                {
                    ProcFSM.Send(evtID);
                }
                ProcFSM.Execute();
            }
        }

        /// <summary>
        /// 使用者對Package操作時向Host端發出事件報告
        /// </summary>
        /// <param name="PPName">受操作的Package名稱</param>
        /// <param name="State">操作狀態</param>
        public void PackageOperation(String PPName, PPOPState State)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                PPState = State;
                EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(12)); //Package State Change Report
                CurEventName = report.EventName;
                if (report != null)
                {
                    List<byte> sb = report.EncodeEventReport();
                    SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                }
            }
        }

        /// <summary>
        /// 料片放置在Load Port或是移離Load Port時透過此函式向Host發出事件報告
        /// </summary>
        /// <param name="bReceived">放上LoadPort或是移離LoadPort</param>
        public void MaterialMovement(bool bReceived = true)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                            (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                            ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                            )
            {
                if (bReceived)
                {
                    EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(16)); //Material Has Placed On Load Port
                    CurEventName = report.EventName;
                    if (report != null)
                    {
                        List<byte> sb = report.EncodeEventReport();
                        SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                    }
                }
                else
                {
                    EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(17)); //Material Has Removed From Load Port
                    CurEventName = report.EventName;
                    if (report != null)
                    {
                        List<byte> sb = report.EncodeEventReport();
                        SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                    }
                }
            }
        }

        private void ProVGemF_Load(object sender, EventArgs e)
        {
            //通訊Log物件的建立
            commLogger = new ProVTool.Logger(SECSEngine, listBox1);

            //讀取ProVSECS.xls檔案並建立相關的資料
            try
            {
                #region 讀取ProVSECS資料、相關Event與Report設定

                String SECSConfigFile = SYSPara.SysDir + @"\Localdata\ProVSECS.xls";
                string tempfile = @"D:\ProVSECSTemp.xls";


                if (File.Exists(SECSConfigFile))
                {
                    try
                    {
                        File.Copy(SECSConfigFile, tempfile, true);
                    }
                    catch
                    {
                        return;
                    }

                    using (FileStream fs = new FileStream(tempfile, FileMode.Open, FileAccess.Read))
                    {

                        HSSFWorkbook workbook = new HSSFWorkbook(fs);

                        //CEID Table
                        HSSFSheet sheet = (HSSFSheet)workbook.GetSheet("ECID");
                        for (int j = 0; j <= sheet.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "ECID")) //Header Row
                                continue;
                            else
                            {
                                if (!IsCellNullorEmpty(row, 0) ||
                                    !IsCellNullorEmpty(row, 1) ||
                                    !IsCellNullorEmpty(row, 2) ||
                                    !IsCellNullorEmpty(row, 3) ||
                                    !IsCellNullorEmpty(row, 4) ||
                                    !IsCellNullorEmpty(row, 5) ||
                                    !IsCellNullorEmpty(row, 6) ||
                                    !IsCellNullorEmpty(row, 7)
                                    )
                                {

                                    ushort ECID = Convert.ToUInt16(row.GetCell(0).ToString());
                                    string ECName = row.GetCell(1).ToString();
                                    string ECUnit = row.GetCell(2).ToString();
                                    string ECFormat = row.GetCell(3).ToString();
                                    string ECValue = row.GetCell(4).ToString();
                                    string ECMin = row.GetCell(5).ToString();
                                    string ECMax = row.GetCell(6).ToString();
                                    string StrCode = row.GetCell(7).ToString();
                                    TableCode ECCode = TableCode.REAL;
                                    if (StrCode.Substring(0, 1) == "P")
                                    {
                                        ECCode = TableCode.PACKAGE;
                                    }
                                    else if (StrCode.Substring(0, 1) == "O")
                                    {
                                        ECCode = TableCode.OPTION;
                                    }
                                    else
                                    {
                                        ECCode = TableCode.REAL;
                                    }

                                    ECData ecData = null;
                                    switch (ECFormat)
                                    {
                                        case "B":
                                            {
                                                byte value = Convert.ToByte(row.GetCell(4).ToString());
                                                byte min = Convert.ToByte(row.GetCell(5).ToString());
                                                byte max = Convert.ToByte(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_BINARY(value), new SECS_BINARY(min), new SECS_BINARY(max));
                                            }
                                            break;
                                        case "BOOL":
                                            {
                                                bool value = false;
                                                bool min = false;
                                                bool max = false;   
                                                if (ECValue == "TRUE")
                                                {
                                                    value = true;
                                                }
                                                else
                                                {
                                                    value = false;
                                                }
                                                if (ECMin == "TRUE")
                                                {
                                                    min = true;
                                                }
                                                else
                                                {
                                                    min = false;
                                                }
                                                if (ECMax == "TRUE")
                                                {
                                                    max = true;
                                                }
                                                else
                                                {
                                                    max = false;
                                                }

                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_BOOLEAN(value), new SECS_BOOLEAN(min), new SECS_BOOLEAN(max));
                                            }
                                            break;
                                        case "A":
                                            {
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_ASCII(ECValue), new SECS_ASCII(ECMin), new SECS_ASCII(ECMax));
                                            }
                                            break;
                                        case "I2":
                                            {
                                                short value = Convert.ToInt16(row.GetCell(4).ToString());
                                                short min = Convert.ToInt16(row.GetCell(5).ToString());
                                                short max = Convert.ToInt16(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_I2(value), new SECS_I2(min), new SECS_I2(max));
                                            }
                                            break;
                                        case "I4":
                                            {
                                                int value = Convert.ToInt32(row.GetCell(4).ToString());
                                                int min = Convert.ToInt32(row.GetCell(5).ToString());
                                                int max = Convert.ToInt32(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_I4(value), new SECS_I4(min), new SECS_I4(max));
                                            }
                                            break;
                                        case "U2":
                                            {
                                                ushort value = Convert.ToUInt16(row.GetCell(4).ToString());
                                                ushort min = Convert.ToUInt16(row.GetCell(5).ToString());
                                                ushort max = Convert.ToUInt16(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_U2(value), new SECS_U2(min), new SECS_U2(max));
                                            }
                                            break;
                                        case "U4":
                                            {
                                                uint value = Convert.ToUInt32(row.GetCell(4).ToString());
                                                uint min = Convert.ToUInt32(row.GetCell(5).ToString());
                                                uint max = Convert.ToUInt32(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_U4(value), new SECS_U4(min), new SECS_U4(max));
                                            }
                                            break;
                                        case "F4":
                                            {
                                                float value = Convert.ToSingle(row.GetCell(4).ToString());
                                                float min = Convert.ToSingle(row.GetCell(5).ToString());
                                                float max = Convert.ToSingle(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_F4(value), new SECS_F4(min), new SECS_F4(max));
                                            }
                                            break;
                                        case "F8":
                                            {
                                                double value = Convert.ToDouble(row.GetCell(4).ToString());
                                                double min = Convert.ToDouble(row.GetCell(5).ToString());
                                                double max = Convert.ToDouble(row.GetCell(6).ToString());
                                                ecData = new ECData(ECID, ECName, ECFormat, ECCode, ECUnit, new SECS_F8(value), new SECS_F8(min), new SECS_F8(max));
                                            }
                                            break;
                                    }
                                    ECList.Add(ecData);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        //SVID Table
                        sheet = (HSSFSheet)workbook.GetSheet("SVID");

                        for (int j = 0; j <= sheet.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "SVID")) //Header Row
                                continue;
                            else
                            {
                                if (!IsCellNullorEmpty(row, 0) ||
                                    !IsCellNullorEmpty(row, 1) ||
                                    !IsCellNullorEmpty(row, 2) ||
                                    !IsCellNullorEmpty(row, 3) ||
                                    !IsCellNullorEmpty(row, 4) ||
                                    !IsCellNullorEmpty(row, 5) ||
                                    !IsCellNullorEmpty(row, 6) ||
                                    !IsCellNullorEmpty(row, 7)
                                    )
                                {

                                    ushort SVID = Convert.ToUInt16(row.GetCell(0).ToString());
                                    string SVName = row.GetCell(1).ToString();
                                    string SVUnit = row.GetCell(2).ToString();
                                    string SVFormat = row.GetCell(3).ToString();
                                    string SVValue = row.GetCell(4).ToString();
                                    string SVMin = row.GetCell(5).ToString();
                                    string SVMax = row.GetCell(6).ToString();
                                    string StrCode = row.GetCell(7).ToString();
                                    TableCode SVCode = TableCode.REAL;
                                    if (StrCode.Substring(0, 1) == "P")
                                    {
                                        SVCode = TableCode.PACKAGE;
                                    }
                                    else if (StrCode.Substring(0, 1) == "O")
                                    {
                                        SVCode = TableCode.OPTION;
                                    }
                                    else
                                    {
                                        SVCode = TableCode.REAL;
                                    }
                                    SVData svData = null;

                                    switch (SVFormat)
                                    {
                                        case "B":
                                            {
                                                byte min = Convert.ToByte(row.GetCell(5).ToString());
                                                byte max = Convert.ToByte(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_BINARY(min), new SECS_BINARY(max));
                                            }
                                            break;
                                        case "BOOL":
                                            {
                                                bool min = false;
                                                bool max = false;
                                                if (SVMin == "TRUE")
                                                {
                                                    min = true;
                                                }
                                                else
                                                {
                                                    min = false;
                                                }
                                                if (SVMax == "TRUE")
                                                {
                                                    max = true;
                                                }
                                                else
                                                {
                                                    max = false;
                                                }

                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_BOOLEAN(min), new SECS_BOOLEAN(max));
                                            }
                                            break;
                                        case "A":
                                            {
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_ASCII(SVMin), new SECS_ASCII(SVMax));
                                            }
                                            break;
                                        case "I2":
                                            {
                                                short min = Convert.ToInt16(row.GetCell(5).ToString());
                                                short max = Convert.ToInt16(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_I2(min), new SECS_I2(max));
                                            }
                                            break;
                                        case "I4":
                                            {
                                                int min = Convert.ToInt32(row.GetCell(5).ToString());
                                                int max = Convert.ToInt32(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_I4(min), new SECS_I4(max));
                                            }
                                            break;
                                        case "U2":
                                            {
                                                ushort min = Convert.ToUInt16(row.GetCell(5).ToString());
                                                ushort max = Convert.ToUInt16(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_U2(min), new SECS_U2(max));
                                            }
                                            break;
                                        case "U4":
                                            {
                                                uint min = Convert.ToUInt32(row.GetCell(5).ToString());
                                                uint max = Convert.ToUInt32(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_U4(min), new SECS_U4(max));
                                            }
                                            break;
                                        case "F4":
                                            {
                                                float min = Convert.ToSingle(row.GetCell(5).ToString());
                                                float max = Convert.ToSingle(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_F4(min), new SECS_F4(max));
                                            }
                                            break;
                                        case "F8":
                                            {
                                                double min = Convert.ToDouble(row.GetCell(5).ToString());
                                                double max = Convert.ToDouble(row.GetCell(6).ToString());
                                                svData = new SVData(SVID, SVName, SVFormat, SVCode, SVUnit, new SECS_F8(min), new SECS_F8(max));
                                            }
                                            break;
                                    }

                                    SVList.Add(svData);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        //DVID Table
                        sheet = (HSSFSheet)workbook.GetSheet("DVID");

                        for (int j = 0; j <= sheet.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "DVID")) //Header Row
                                continue;
                            else
                            {
                                if (!IsCellNullorEmpty(row, 0) ||
                                    !IsCellNullorEmpty(row, 1) ||
                                    !IsCellNullorEmpty(row, 2) ||
                                    !IsCellNullorEmpty(row, 3) ||
                                    !IsCellNullorEmpty(row, 4) ||
                                    !IsCellNullorEmpty(row, 5) ||
                                    !IsCellNullorEmpty(row, 6) ||
                                    !IsCellNullorEmpty(row, 7)
                                    )
                                {

                                    ushort DVID = Convert.ToUInt16(row.GetCell(0).ToString());
                                    string DVName = row.GetCell(1).ToString();
                                    string DVUnit = row.GetCell(2).ToString();
                                    string DVFormat = row.GetCell(3).ToString();
                                    string DVValue = row.GetCell(4).ToString();
                                    string DVMin = row.GetCell(5).ToString();
                                    string DVMax = row.GetCell(6).ToString();
                                    string StrCode = row.GetCell(7).ToString();
                                    TableCode DVCode = TableCode.REAL;
                                    if (StrCode.Substring(0, 1) == "P")
                                    {
                                        DVCode = TableCode.PACKAGE;
                                    }
                                    else if (StrCode.Substring(0, 1) == "O")
                                    {
                                        DVCode = TableCode.OPTION;
                                    }
                                    else
                                    {
                                        DVCode = TableCode.REAL;
                                    }
                                    DVData dvData = null;

                                    switch (DVFormat)
                                    {
                                        case "B":
                                            {
                                                byte value = Convert.ToByte(row.GetCell(4).ToString());
                                                byte min = Convert.ToByte(row.GetCell(5).ToString());
                                                byte max = Convert.ToByte(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_BINARY(min), new SECS_BINARY(max));
                                            }
                                            break;
                                        case "BOOL":
                                            {
                                                bool min = false;
                                                bool max = false;
                                                if (DVMin == "TRUE")
                                                {
                                                    min = true;
                                                }
                                                else
                                                {
                                                    min = false;
                                                }
                                                if (DVMax == "TRUE")
                                                {
                                                    max = true;
                                                }
                                                else
                                                {
                                                    max = false;
                                                }

                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_BOOLEAN(min), new SECS_BOOLEAN(max));
                                            }
                                            break;
                                        case "A":
                                            {
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_ASCII(DVMin), new SECS_ASCII(DVMax));
                                            }
                                            break;
                                        case "I2":
                                            {
                                                short min = Convert.ToInt16(row.GetCell(5).ToString());
                                                short max = Convert.ToInt16(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_I2(min), new SECS_I2(max));
                                            }
                                            break;
                                        case "I4":
                                            {
                                                int min = Convert.ToInt32(row.GetCell(5).ToString());
                                                int max = Convert.ToInt32(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_I4(min), new SECS_I4(max));
                                            }
                                            break;
                                        case "U2":
                                            {
                                                ushort min = Convert.ToUInt16(row.GetCell(5).ToString());
                                                ushort max = Convert.ToUInt16(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_U2(min), new SECS_U2(max));
                                            }
                                            break;
                                        case "U4":
                                            {
                                                uint min = Convert.ToUInt32(row.GetCell(5).ToString());
                                                uint max = Convert.ToUInt32(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_U4(min), new SECS_U4(max));
                                            }
                                            break;
                                        case "F4":
                                            {
                                                float min = Convert.ToSingle(row.GetCell(5).ToString());
                                                float max = Convert.ToSingle(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_F4(min), new SECS_F4(max));
                                            }
                                            break;
                                        case "F8":
                                            {
                                                double min = Convert.ToDouble(row.GetCell(5).ToString());
                                                double max = Convert.ToDouble(row.GetCell(6).ToString());
                                                dvData = new DVData(DVID, DVName, DVFormat, DVCode, DVUnit, new SECS_F8(min), new SECS_F8(max));
                                            }
                                            break;
                                    }

                                    DVList.Add(dvData);
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        //SNFN Table
                        sheet = (HSSFSheet)workbook.GetSheet("SNFN");

                        for (int j = 0; j <= sheet.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "Sn")) //Header Row
                                continue;
                            else
                            {
                                if (!IsCellNullorEmpty(row, 0) ||
                                    !IsCellNullorEmpty(row, 1) ||
                                    !IsCellNullorEmpty(row, 2) ||
                                    !IsCellNullorEmpty(row, 3)
                                    )
                                {

                                    byte SN = Convert.ToByte(row.GetCell(0).ToString());
                                    byte FN = Convert.ToByte(row.GetCell(1).ToString());
                                    string Status = row.GetCell(2).ToString();

                                    if (Status == "V")
                                    {
                                        SnFnData snfnData = new SnFnData(SN, FN);
                                        SnFnList.Add(snfnData);
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        //SNFN Table
                        sheet = (HSSFSheet)workbook.GetSheet("PPBODY");

                        for (int j = 0; j <= sheet.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheet.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "ItemName")) //Header Row
                                continue;
                            else
                            {
                                if (!IsCellNullorEmpty(row, 0) ||
                                    !IsCellNullorEmpty(row, 1) ||
                                    !IsCellNullorEmpty(row, 2) 
                                    )
                                {

                                    String Name = row.GetCell(0).ToString();
                                    String Format = row.GetCell(1).ToString();
                                    String QueryName = row.GetCell(2).ToString();
                                    PKGSchema.Add(new PKGItem(Name, QueryName, Format));
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        LinkReport(workbook);
                        
                        fs.Close();

                        dgvEC.DataSource = ECList;
                        dgvSV.DataSource = SVList;
                        dgvDV.DataSource = DVList;
                        dgvRPT.DataSource = RPTList;
                        dgvEVT.DataSource = EVTReportList;

                        File.Delete(tempfile);
                    }
                }
                else
                {
                    MessageBox.Show("ProVSECS.xls Not Existed!");
                }
                #endregion

                //通訊模式切換狀態機的建立
                if (HostCommFSM == null)
                {
                    HostCommFSM = new ProVGEMCommFSM.ProVHostInitiatedFSM();
                    HostCommFSM.TransitionCompleted += HostCommFSM_TransitionCompleted;
                    HostCommFSM.OnS1F14Send += SendS1F14;
                    HostCommFSM.OnCommEnable += CommunicationEnable;
                    HostCommFSM.OnCommDisable += CommunicationDisable;
                }
                if (EQPCommFSM == null)
                {
                    EQPCommFSM = new ProVGEMCommFSM.ProVEQPInitiatedFSM();
                    EQPCommFSM.TransitionCompleted += HostCommFSM_TransitionCompleted;
                }
                //控制模式切換狀態機的建立
                if (CtrlFSM == null)
                {
                    ECData ecData = ECList.Find(x => x.ECID == 1007); //2019/09/02 By Max 1000 -> 1007
                    if (ecData != null)
                    {
                        SECS_U2 uData = ecData.Value as SECS_U2;
                        ECData ecInitState = ECList.Find(x => x.ECID == 1005); //2019/09/02 By Max 1000 -> 1007
                        SECS_U2 u2 = ecInitState.Value as SECS_U2;

                        if (u2.Data == 4)
                        {
                            PreCtrlState = ProVGEMControlFSM.StateID.ONLINELOCAL;
                            CurCtrlState = ProVGEMControlFSM.StateID.ONLINELOCAL;
                            CtrlFSM = new ProVGEMControlFSM.ProVControlFSM(uData.Data, ProVGEMControlFSM.StateID.ONLINE);
                            lblEQPOffLine.BackColor = Color.White;
                            lblAttemptOnLine.BackColor = Color.White;
                            lblHostOffLine.BackColor = Color.White;
                            lblOnLineLocal.BackColor = Color.GreenYellow;
                            lblOnLineRemote.BackColor = Color.White;
                        }
                        else if (u2.Data == 5)
                        {
                            PreCtrlState = ProVGEMControlFSM.StateID.ONLINEREMOTE;
                            CurCtrlState = ProVGEMControlFSM.StateID.ONLINEREMOTE;
                            CtrlFSM = new ProVGEMControlFSM.ProVControlFSM(uData.Data, ProVGEMControlFSM.StateID.ONLINE);
                            lblEQPOffLine.BackColor = Color.White;
                            lblAttemptOnLine.BackColor = Color.White;
                            lblHostOffLine.BackColor = Color.White;
                            lblOnLineLocal.BackColor = Color.White;
                            lblOnLineRemote.BackColor = Color.GreenYellow;
                        }
                        else
                        {
                            PreCtrlState = ProVGEMControlFSM.StateID.OFFLINE;
                            CurCtrlState = ProVGEMControlFSM.StateID.OFFLINE;
                            CtrlFSM = new ProVGEMControlFSM.ProVControlFSM(uData.Data, ProVGEMControlFSM.StateID.OFFLINE);
                            lblEQPOffLine.BackColor = Color.GreenYellow;
                            lblAttemptOnLine.BackColor = Color.White;
                            lblHostOffLine.BackColor = Color.White;
                            lblOnLineLocal.BackColor = Color.White;
                            lblOnLineRemote.BackColor = Color.White;
                        }
                        CtrlFSM.TransitionCompleted += CtrlFSM_TransitionCompleted;
                        CtrlFSM.ControlFSMEvent += CtrlFSM_ControlFSMEvent;
                    }
                }
                //機台模式切換狀態機的建立
                if (ProcFSM == null)
                {
                    ProcFSM = new ProVGEMProcessFSM.ProVProcessFSM();
                    ProcFSM.TransitionCompleted += ProcFSM_TransitionCompleted;
                }
                //Spooling功能狀態機的建立
                if (SpoolLoadFSM == null)
                {
                    SpoolLoadFSM = new ProVGEMSpoolFSM.SpoolLoadFSM();
                    SpoolLoadFSM.TransitionCompleted += SpoolFSM_TransitionCompleted;
                }

                if (SpoolUnloadFSM == null)
                {
                    SpoolUnloadFSM = new ProVGEMSpoolFSM.SpoolUnloadFSM();
                    SpoolUnloadFSM.TransitionCompleted += SpoolFSM_TransitionCompleted;
                }

                //Alarm Report目前僅處理E與e Alarm Level
                foreach (ArmMtrl arm in SYSPara.Alarm.AlarmTable)
                {
                    if (arm.AlarmLevel == "E" || arm.AlarmLevel == "e")
                    {
                        //All ALCD is set to Equipment Safty 0x02 and enabled
                        ushort errCode = (ushort)(arm.ModuleID * 100 + arm.ErrorCode);
                        ArmList.Add(new ALMData(0x02, arm.LanENG.Explain, errCode, 0x01));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //Communication預設進入Enable狀態
            rdoEnable.Checked = true;
        }

        private void LinkReport(HSSFWorkbook book)
        {
            EventReport evtReport = null;

            //CEID Table
            HSSFSheet sheetCE = (HSSFSheet)book.GetSheet("CEID");

            for (int j = 0; j <= sheetCE.LastRowNum; j++)
            {
                HSSFRow row = (HSSFRow)sheetCE.GetRow(j);
                if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "CEID")) //Header Row
                    continue;
                else
                {
                    if (!IsCellNullorEmpty(row, 0) ||
                        !IsCellNullorEmpty(row, 1) ||
                        !IsCellNullorEmpty(row, 2) ||
                        !IsCellNullorEmpty(row, 3)
                        )
                    {

                        ushort CEID = Convert.ToUInt16(row.GetCell(0).ToString()); //CEID
                        string CEName = row.GetCell(1).ToString(); //Event Name
                        string CEFormat = row.GetCell(2).ToString(); //Format
                        string CEValue = row.GetCell(3).ToString(); //RPT List

                        evtReport = new EventReport(SECSEngine, CEID, CEName);
                        evtReport.QueryValueEvent += QueryValueEventHandler;
                        if (CEValue.EndsWith(";")) //將結尾的分號去掉
                            CEValue = CEValue.Remove(CEValue.Length - 1);
                        String[] RPTIDList = CEValue.Split(';');
                        foreach (String s in RPTIDList)
                        {
                            ushort id = Convert.ToUInt16(s);
                            RPTData rptData = new RPTData(id);
                            evtReport.AddRPT(rptData);
                            rptData = evtReport[id];
                            LinkVID(book, ref rptData);
                            RPTList.Add(rptData);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //將每一個Link好的Event Report加到List中
                EVTReportList.Add(evtReport);
            }

        }

        private void LinkVID(HSSFWorkbook book, ref RPTData RPTData)
        {
            //RPTID Table
            HSSFSheet sheet = (HSSFSheet)book.GetSheet("RPTID");
            for (int j = 0; j <= sheet.LastRowNum; j++)
            {
                HSSFRow row = (HSSFRow)sheet.GetRow(j);
                if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "RPTID")) //Header Row
                    continue;
                else
                {
                    if (!IsCellNullorEmpty(row, 0) ||
                        !IsCellNullorEmpty(row, 1) ||
                        !IsCellNullorEmpty(row, 2) ||
                        !IsCellNullorEmpty(row, 3)
                        )
                    {

                        ushort RPTID = Convert.ToUInt16(row.GetCell(0).ToString()); //RPTID
                        string RPTName = row.GetCell(1).ToString(); //Report Name
                        string RPTFormat = row.GetCell(2).ToString(); //Format
                        string RPTValue = row.GetCell(3).ToString(); //VID List

                        if (RPTID != RPTData.RPTID)
                            continue;

                        if (RPTValue.EndsWith(";")) //將結尾的分號去掉
                            RPTValue = RPTValue.Remove(RPTValue.Length - 1);
                        String[] VIDList = RPTValue.Split(';');
                        foreach (String s in VIDList)
                        {
                            ushort id = Convert.ToUInt16(s);
                            VIDData vidData = GetVID(id);
                            if(vidData != null)
                                RPTData.AddVID(vidData); 
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private String CurEventName = "NONE";
        //所有EventReport物件會透過此事件處理函式來要資料
        SECS_BASE QueryValueEventHandler(string Name, string Format, TableCode Code)
        {
            SECS_BASE SECSData = null;
            try
            {
                if (Name == "CLOCK")
                {
                    String DTStr = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss");
                    SECSData = new SECS_ASCII(DTStr);
                }
                else if (Name == "EVTNAME")
                    SECSData = new SECS_ASCII(CurEventName);
                else if (Name == "InitCommState")
                    SECSData = new SECS_U2(1);
                else if (Name == "OnlineSubState")
                    SECSData = new SECS_U2(0);
                else if (Name == "OfflineSubState")
                    SECSData = new SECS_U2(0);
                else if (Name == "InitControlState")
                    SECSData = new SECS_U2(1);
                else if (Name == "EstabTimeDelay")
                {
                    ECData ecData = ECList.Find(x => x.Name == "EstabTimeDelay");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "CurrCommState")
                    SECSData = new SECS_U2(Convert.ToUInt16(CurCommState));
                else if (Name == "CurrControlState")
                    SECSData = new SECS_U2(Convert.ToUInt16(CurCtrlState));
                else if (Name == "PreControlState")
                    SECSData = new SECS_U2(Convert.ToUInt16(PreCtrlState));
                else if (Name == "CurrProcessState")
                    SECSData = new SECS_U2(Convert.ToUInt16(CurProcState));
                else if (Name == "PreProcessState")
                    SECSData = new SECS_U2(Convert.ToUInt16(PreProcState));
                else if (Name == "PPCHANGESTATE")
                    SECSData = new SECS_BINARY(Convert.ToByte(PPState));
                else if (Name == "SpoolCountActual")
                    SECSData = new SECS_U4(Convert.ToUInt32(SpoolList.Count));
                else if (Name == "SpoolCountTotal")
                    SECSData = new SECS_U4(Convert.ToUInt32(SpoolList.SpoolCountTotal));
                else if (Name == "SpoolFullTime")
                    SECSData = new SECS_ASCII(SpoolList.SpoolStartTime);
                else if (Name == "SpoolStartTime")
                    SECSData = new SECS_ASCII(SpoolList.SpoolStartTime);
                else if (Name == "LimitVariable")
                {
                    SECSData = new SECS_U2(CurExceedLimitItem.VID);
                }
                else if (Name == "EventLimit")
                {
                    SECSData = new SECS_U2(CurExceedLimitItem.LIMITID);
                }
                else if (Name == "TransitionType")
                {
                    SECSData = new SECS_U2(CurExceedLimitItem.TransitionType);
                }
                else if (Name == "DeviceID")
                {
                    ECData ecData = ECList.Find(x => x.Name == "DeviceID");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "IP")
                {
                    ECData ecData = ECList.Find(x => x.Name == "IP");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "Port")
                {
                    ECData ecData = ECList.Find(x => x.Name == "Port");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "TimeFormat")
                {
                    ECData ecData = ECList.Find(x => x.Name == "TimeFormat");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "MaxSpoolTransmit")
                {
                    ECData ecData = ECList.Find(x => x.Name == "MaxSpoolTransmit");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "OverWriteSpool")
                {
                    ECData ecData = ECList.Find(x => x.Name == "OverWriteSpool");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "EnableSpooling")
                {
                    ECData ecData = ECList.Find(x => x.Name == "EnableSpooling");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                else if (Name == "LimitQueryInterval")
                {
                    ECData ecData = ECList.Find(x => x.Name == "LimitQueryInterval");
                    if (ecData != null)
                    {
                        SECSData = ecData.Value;
                    }
                }
                //2019/09/02 By Max For Alarm Code & Alarm Text
                else if (Name == "AlarmID")
                {
                    SECSData = new SECS_U2(Convert.ToUInt16(CurAlarmID));
                }
                else if (Name == "AlarmText")
                {
                    SECSData = new SECS_ASCII(CurAlarmText);
                }
                else
                {
                    //在此層找不到資料，就向上層找
                    if (OnDataExchangeEvent != null)
                    {
                        ExchangeData data = new ExchangeData();
                        data.Code = Code;
                        data.DataName = Name;
                        if (Name == "PPID")
                        {
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("PPID", "");
                        }
                        data = OnDataExchangeEvent(EventType.QUERY, data);
                        switch (Format)
                        {
                            case "A":
                                {
                                    SECSData = new SECS_ASCII(data.RetData.ToString());
                                }
                                break;
                            case "BOOL":
                                {
                                    SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                }
                                break;
                            case "F4":
                                {
                                    SECSData = new SECS_F4((float)data.RetData.ToDouble());
                                }
                                break;
                            case "F8":
                                {
                                    SECSData = new SECS_F8(data.RetData.ToDouble());
                                }
                                break;
                            case "I1":
                                {
                                    SECSData = new SECS_I1((sbyte)data.RetData.ToInt());
                                }
                                break;
                            case "I2":
                                {
                                    SECSData = new SECS_I2((short)data.RetData.ToInt());
                                }
                                break;
                            case "I4":
                                {
                                    SECSData = new SECS_I4(data.RetData.ToInt());
                                }
                                break;
                            case "I8":
                                {
                                    SECSData = new SECS_I8(data.RetData.ToInt());
                                }
                                break;
                            case "U1":
                                {
                                    SECSData = new SECS_U1((byte)data.RetData.ToInt());
                                }
                                break;
                            case "U2":
                                {
                                    SECSData = new SECS_U2((ushort)data.RetData.ToInt());
                                }
                                break;
                            case "U4":
                                {
                                    SECSData = new SECS_U4((uint)data.RetData.ToInt());
                                }
                                break;
                            case "U8":
                                {
                                    SECSData = new SECS_U8((ulong)data.RetData.ToInt());
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}, {1}", Name, ex.Message));
            }
            return SECSData;
        }

        private VIDData GetVID(ushort ID)
        {
            VIDData vid = null;
            String IDName = String.Empty;
            if (ID >= 1000 && ID < 1999) //ECID
            {
                ECData ecData = ECList.Find(x => x.ECID == (ushort)(ID));
                vid = new VIDData(ID, ecData.Name, ecData.Format, ecData.Code, ecData.Min, ecData.Max, ecData.Unit);
            }
            else if (ID >= 2000 && ID < 2999) //SVID
            {
                SVData svData = SVList.Find(x => x.SVID == (ushort)(ID));
                vid = new VIDData(ID, svData.Name, svData.Format, svData.Code, svData.Min, svData.Max, svData.Unit);
            }
            else if (ID >= 3000 && ID < 3999) //DVID
            {
                DVData dvData = DVList.Find(x => x.DVID == (ushort)(ID));
                vid = new VIDData(ID, dvData.Name, dvData.Format, dvData.Code, dvData.Min, dvData.Max, dvData.Unit);
            }

            return vid;
        }

        private bool IsCellNullorEmpty(HSSFRow row, int ColIndex)
        {
            if ((row.GetCell(ColIndex) != null) && (row.GetCell(ColIndex).ToString() != string.Empty))
                return false;
            else
            {
                String strMsg = String.Format("Row Index{0}, Col Index{1} is Null or Empty!", row.RowNum, ColIndex);
                MessageBox.Show(strMsg);
                return true;
            }
        }

        void ControlStateChange()
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(1));
                CurEventName = report.EventName;
                if (report != null)
                {
                    List<byte> sb = report.EncodeEventReport();
                    SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                }

                if (OnDataExchangeEvent != null)
                {
                    ExchangeData data = new ExchangeData();
                    data.DataName = ControlState.ToString();
                    OnDataExchangeEvent(EventType.STATECHANGE, data); 
                }
            }
        }

        void CtrlFSM_ControlFSMEvent(object sender, ProVGEMControlFSM.ControlFSMEventArgs e)
        {
            if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
            {
                List<byte> sb = new List<byte>();
                switch (e.ActionName)
                {
                    case "SendS1F1":
                        {
                            sb.Clear();
                            SECSEngine.SendHSMSMessage(1, 1, true, sb.ToArray()); //ON-LINE Request
                        }
                        break;
                    case "SendS1F16":
                        {
                            sb.Clear();
                            SECS_BINARY bData = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, bData);
                            SECSEngine.SendHSMSMessage(1, 16, false, sb.ToArray()); //Reply Host Request Off-Line
                        }
                        break;
                    case "SendS1F18":
                        {
                            sb.Clear();
                            SECS_BINARY bData = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, bData);
                            SECSEngine.SendHSMSMessage(1, 18, false, sb.ToArray()); //Reply Host Request On-Line
                        }
                        break;
                }
            }
        }

        #region Spooling FSM Transition Completed Event Handler
        private SpoolData TransmitSpoolData = null;
        void SpoolFSM_TransitionCompleted(object sender, Sanford.StateMachineToolkit.TransitionCompletedEventArgs<ProVGEMSpoolFSM.StateID, ProVGEMSpoolFSM.EventID, EventArgs> e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }


            switch (e.TargetStateID)
            {
                case ProVGEMSpoolFSM.StateID.SPOOLINACTIVE:
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            ExchangeData data = new ExchangeData();
                            data.DataName = "INACTIVE";
                            OnDataExchangeEvent(EventType.SPOOLING, data);
                        }

                        EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(14)); //Spool Deactivated Report
                        CurEventName = report.EventName;
                        if (report != null)
                        {
                            List<byte> sb = report.EncodeEventReport();
                            SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                        }
                    }
                    break;
                case ProVGEMSpoolFSM.StateID.TRANSMITSPOOL:
                    {
                        if (SpoolList.Count > 0)
                        {
                            TransmitSpoolData = SpoolList[0];
                            SECSEngine.SendHSMSMessage(TransmitSpoolData.SType, TransmitSpoolData.FType, TransmitSpoolData.W_Bit, TransmitSpoolData.Data);
                            SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_MAXSPOOLTRANSREACH);
                            SpoolUnloadFSM.Execute();
                            SpoolList.RemoveAt(0);
                        }
                        else
                        {
                            SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_SPOOLEMPTY);
                            SpoolUnloadFSM.Execute();
                            TransmitSpoolData = null;
                        }
                    }
                    break;
                case ProVGEMSpoolFSM.StateID.PURGESPOOL:
                    {
                        SpoolList.Clear();
                    }
                    break;
                case ProVGEMSpoolFSM.StateID.NOSPOOLPUTPUT:
                    {
                    }
                    break;
                case ProVGEMSpoolFSM.StateID.SPOOLNOTFULL:
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            ExchangeData data = new ExchangeData();
                            data.DataName = "ACTIVE";
                            OnDataExchangeEvent(EventType.SPOOLING, data);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Process FSM Transition Completed Event Handler
        ProVGEMProcessFSM.StateID CurProcState = ProVGEMProcessFSM.StateID.IDLE;
        ProVGEMProcessFSM.StateID PreProcState = ProVGEMProcessFSM.StateID.IDLE;
        void ProcFSM_TransitionCompleted(object sender, Sanford.StateMachineToolkit.TransitionCompletedEventArgs<ProVGEMProcessFSM.StateID, ProVGEMProcessFSM.EventID, EventArgs> e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            PreProcState = CurProcState;
            CurProcState = e.TargetStateID;
            ProcessStateChange();

            switch (e.TargetStateID)
            {
                case ProVGEMProcessFSM.StateID.IDLE:
                    {
                        lblIdle.BackColor = Color.GreenYellow;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.SETUP:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.GreenYellow;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.INITIAL:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.GreenYellow;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.EXECUTING:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.GreenYellow;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.PAUSE:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.Yellow;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.ALARM:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.Red;
                        lblHome.BackColor = Color.White;
                    }
                    break;
                case ProVGEMProcessFSM.StateID.HOME:
                    {
                        lblIdle.BackColor = Color.White;
                        lblSetup.BackColor = Color.White;
                        lblInitial.BackColor = Color.White;
                        lblExecuting.BackColor = Color.White;
                        lblPause.BackColor = Color.White;
                        lblAlarm.BackColor = Color.White;
                        lblHome.BackColor = Color.GreenYellow;
                    }
                    break;
            }
        }

        void ProcessStateChange()
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(2));
                CurEventName = report.EventName;
                if (report != null)
                {
                    List<byte> sb = report.EncodeEventReport();
                    SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                }
            }
        }
        #endregion

        #region Control FSM Transition Completed Event Handler
        ProVGEMControlFSM.StateID CurCtrlState = ProVGEMControlFSM.StateID.OFFLINE;
        ProVGEMControlFSM.StateID PreCtrlState = ProVGEMControlFSM.StateID.OFFLINE;
        public ProVGEMControlFSM.StateID ControlState
        {
            get
            {
                return CurCtrlState;
            }
        }
        void CtrlFSM_TransitionCompleted(object sender, Sanford.StateMachineToolkit.TransitionCompletedEventArgs<ProVGEMControlFSM.StateID, ProVGEMControlFSM.EventID, EventArgs> e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            PreCtrlState = e.SourceStateID;
            CurCtrlState = e.TargetStateID;

            switch (e.TargetStateID)
            {
                case ProVGEMControlFSM.StateID.OFFLINE:
                    {

                    }
                    break;
                case ProVGEMControlFSM.StateID.EQPOFFLINE:
                    {
                        lblEQPOffLine.BackColor = Color.GreenYellow;
                        lblAttemptOnLine.BackColor = Color.White;
                        lblHostOffLine.BackColor = Color.White;
                        lblOnLineLocal.BackColor = Color.White;
                        lblOnLineRemote.BackColor = Color.White;
                        ControlStateChange();
                    }
                    break;
                case ProVGEMControlFSM.StateID.ATTEMPTONLINE:
                    {
                        lblEQPOffLine.BackColor = Color.White;
                        lblAttemptOnLine.BackColor = Color.GreenYellow;
                        lblHostOffLine.BackColor = Color.White;
                        lblOnLineLocal.BackColor = Color.White;
                        lblOnLineRemote.BackColor = Color.White;
                    }
                    break;
                case ProVGEMControlFSM.StateID.HOSTOFFLINE:
                    {
                        lblEQPOffLine.BackColor = Color.White;
                        lblAttemptOnLine.BackColor = Color.White;
                        lblHostOffLine.BackColor = Color.GreenYellow;
                        lblOnLineLocal.BackColor = Color.White;
                        lblOnLineRemote.BackColor = Color.White;
                        ControlStateChange();
                    }
                    break;
                case ProVGEMControlFSM.StateID.ONLINELOCAL:
                    {
                        lblEQPOffLine.BackColor = Color.White;
                        lblAttemptOnLine.BackColor = Color.White;
                        lblHostOffLine.BackColor = Color.White;
                        lblOnLineLocal.BackColor = Color.GreenYellow;
                        lblOnLineRemote.BackColor = Color.White;
                        ControlStateChange();
                    }
                    break;
                case ProVGEMControlFSM.StateID.ONLINEREMOTE:
                    {
                        lblEQPOffLine.BackColor = Color.White;
                        lblAttemptOnLine.BackColor = Color.White;
                        lblHostOffLine.BackColor = Color.White;
                        lblOnLineLocal.BackColor = Color.White;
                        lblOnLineRemote.BackColor = Color.GreenYellow;
                        ControlStateChange();
                    }
                    break;
            }
        }
        #endregion

        #region Communication FSM Transition Completed Event Handler
        ProVGEMCommFSM.StateID CurCommState = ProVGEMCommFSM.StateID.DISABLED;
        public ProVGEMCommFSM.StateID CommunicationState
        {
            get
            {
                return CurCommState;
            }
        }

        private void SendS1F14(object sender, EventArgs args)
        {
            List<byte> sb = new List<byte>();
            sb.Clear();
            SECS_LIST List = new SECS_LIST(2);
            SECSEngine.EncodeDataItem(ref sb, List);
            SECS_BINARY commACK = new SECS_BINARY(0);
            SECSEngine.EncodeDataItem(ref sb, commACK);
            List = new SECS_LIST(2);
            SECSEngine.EncodeDataItem(ref sb, List);
            SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
            SECSEngine.EncodeDataItem(ref sb, SECSData);
            SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
            SECSEngine.EncodeDataItem(ref sb, SECSData);
            SECSEngine.SendHSMSMessage(1, 14, false, sb.ToArray());
        }

        private void CommunicationEnable(object sender, EventArgs args)
        {
            ECData ecData = ECList.Find(x => x.ECID == 1000);
            if (ecData != null)
            {
                SECS_U2 uData = ecData.Value as SECS_U2;
                SECSEngine.DeviceID = uData.Data;
            }
            ecData = ECList.Find(x => x.ECID == 1001);
            if (ecData != null)
            {
                SECS_ASCII uData = ecData.Value as SECS_ASCII;
                SECSEngine.IP = uData.Data;
            }
            ecData = ECList.Find(x => x.ECID == 1002);
            if (ecData != null)
            {
                SECS_U2 uData = ecData.Value as SECS_U2;
                SECSEngine.Port = uData.Data;
            }
            SECSEngine.Initialize("");
            SECSEngine.Connect();
        }

        private void CommunicationDisable(object sender, EventArgs args)
        {
            SECSEngine.Disconnect();
        }

        void HostCommFSM_TransitionCompleted(object sender, Sanford.StateMachineToolkit.TransitionCompletedEventArgs<ProVGEMCommFSM.StateID, ProVGEMCommFSM.EventID, EventArgs> e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (CurCommState != e.TargetStateID)
            {
                CurCommState = e.TargetStateID;

                if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                {
                    if (OnDataExchangeEvent != null)
                    {
                        ExchangeData data = new ExchangeData();
                        data.DataName = CurCommState.ToString();
                        OnDataExchangeEvent(EventType.STATECHANGE, data);
                    }
                }
                else if (CurCommState == ProVGEMCommFSM.StateID.DISABLED)
                {
                    if (OnDataExchangeEvent != null)
                    {
                        ExchangeData data = new ExchangeData();
                        data.DataName = CurCommState.ToString();
                        OnDataExchangeEvent(EventType.STATECHANGE, data);
                    }
                }
                else if (CurCommState == ProVGEMCommFSM.StateID.WAIT_CR_FROM_HOST)
                {
                    if (OnDataExchangeEvent != null)
                    {
                        ExchangeData data = new ExchangeData();
                        data.DataName = "NOTCOMMUNICATING";
                        OnDataExchangeEvent(EventType.STATECHANGE, data);
                    }
                }
            }

            switch (e.TargetStateID)
            {
                case ProVGEMCommFSM.StateID.DISABLED:
                    {
                        lblDisabled.BackColor = Color.GreenYellow;
                        lblCommunicating.BackColor = Color.White;
                        lblWaitCR.BackColor = Color.White;
                        lblWaitCRA.BackColor = Color.White;
                        lblWaitDelay.BackColor = Color.White;
                    }
                    break;
                case ProVGEMCommFSM.StateID.ENABLED:
                    {
                    }
                    break;
                case ProVGEMCommFSM.StateID.WAIT_CRA:
                    {
                        lblDisabled.BackColor = Color.White;
                        lblCommunicating.BackColor = Color.White;
                        lblWaitCRA.BackColor = Color.GreenYellow;
                        lblWaitDelay.BackColor = Color.White;

                        if (SECSEngine.IsConnected)
                        {
                            List<byte> sb = new List<byte>();
                            sb.Clear();
                            SECS_LIST List = new SECS_LIST(2);
                            SECSEngine.EncodeDataItem(ref sb, List);
                            SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                            SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                            SECSEngine.SendHSMSMessage(1, 13, true, sb.ToArray());
                        }
                    }
                    break;
                case ProVGEMCommFSM.StateID.WAIT_DELAY:
                    {
                        lblDisabled.BackColor = Color.White;
                        lblCommunicating.BackColor = Color.White;
                        lblWaitCRA.BackColor = Color.White;
                        lblWaitDelay.BackColor = Color.GreenYellow;
                    }
                    break;
                case ProVGEMCommFSM.StateID.NOT_COMMUNICATING:
                    {
                        lblDisabled.BackColor = Color.White;
                        lblCommunicating.BackColor = Color.White;
                        lblWaitCR.BackColor = Color.GreenYellow;
                        lblWaitCRA.BackColor = Color.GreenYellow;
                        lblWaitDelay.BackColor = Color.White;
                    }
                    break;
                case ProVGEMCommFSM.StateID.COMMUNICATING:
                    {
                        lblDisabled.BackColor = Color.White;
                        lblWaitCR.BackColor = Color.White;
                        lblWaitCRA.BackColor = Color.White;
                        lblWaitDelay.BackColor = Color.White;
                        lblCommunicating.BackColor = Color.GreenYellow;

                        EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMMACK_0);
                        EQPCommFSM.Execute();
                        HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMMACK_0);
                        HostCommFSM.Execute();

                        
                    }
                    break;
                case ProVGEMCommFSM.StateID.WAIT_CR_FROM_HOST:
                    {
                        lblDisabled.BackColor = Color.White;
                        lblCommunicating.BackColor = Color.White;
                        lblWaitCR.BackColor = Color.GreenYellow;
                        //取得是否啟用Spool功能
                        ECData ecData = ECList.Find(x => x.ECID == 1012);
                        SECS_BOOLEAN bEnableSpool = (SECS_BOOLEAN)ecData.Value;
                        if (e.SourceStateID == ProVGEMCommFSM.StateID.COMMUNICATING && bEnableSpool.Data)
                        {
                            SpoolLoadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_COMMBROKEN);
                            SpoolLoadFSM.Execute();
                            SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_COMMBROKEN);
                            SpoolUnloadFSM.Execute();

                            
                            SpoolList.Clear();
                            String DTStr = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss");
                            SpoolList.SpoolStartTime = DTStr;

                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(13)); //Spool Activated Report
                            CurEventName = report.EventName;
                            if (report != null)
                            {
                                List<byte> sb = report.EncodeEventReport();
                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region SECS Event Handler
        private long proVSECSEngine1_SECSEvent(ProVTool.EVENTID EvtID, ProVTool.EventData pEventData)
        {
            switch (EvtID)
            {
                #region Event Connected
                case ProVTool.EVENTID.EVENT_CONNECTED:
                    {
                        commLogger.AppendText(String.Format("HSMS Connected Successfully!"));
                        if (CurCommState == ProVGEMCommFSM.StateID.WAIT_CRA)
                        {
                            if (SECSEngine.IsConnected)
                            {
                                List<byte> sb = new List<byte>();
                                sb.Clear();
                                SECS_LIST List = new SECS_LIST(2);
                                SECSEngine.EncodeDataItem(ref sb, List);
                                SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                                SECSEngine.EncodeDataItem(ref sb, SECSData);
                                SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                                SECSEngine.EncodeDataItem(ref sb, SECSData);
                                SECSEngine.SendHSMSMessage(1, 13, true, sb.ToArray());
                            }
                        }
                    }
                    break;
                #endregion

                #region Event Disconnected
                case ProVTool.EVENTID.EVENT_DISCONNECTED:
                    {
                        commLogger.AppendText(String.Format("HSMS Disconnected!"));
                        HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMM_FAILURE);
                        HostCommFSM.Execute();
                        EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMM_FAILURE);
                        EQPCommFSM.Execute();

                        foreach (TraceDataCollection t in TRList)
                        {
                            t.Stop();
                        }
                    }
                    break;
                #endregion

                #region Send Message
                case ProVTool.EVENTID.EVENT_SEND_MSG:
                    {
                        if (pEventData.CtrlMsg == 0)
                        {
                            if (pEventData.W_Bit)
                            {
                                commLogger.AppendText(String.Format("<- S{0}F{1} W", pEventData.S, pEventData.F));
                            }
                            else
                            {
                                commLogger.AppendText(String.Format("<- S{0}F{1}", pEventData.S, pEventData.F));
                            }
                            commLogger.ShowBinaryData(pEventData.Head);
                            commLogger.ShowBinaryData(pEventData.RawData);
                            commLogger.ShowSECSIIMessage(pEventData.RawData);
                        }
                        else
                        {
                            switch (pEventData.CtrlMsg)
                            {
                                case 1:
                                    {
                                        commLogger.AppendText(String.Format("<- Select.req"));
                                    }
                                    break;
                                case 2:
                                    {
                                        commLogger.AppendText(String.Format("<- Select.rsp"));
                                    }
                                    break;
                                case 5:
                                    {
                                        commLogger.AppendText(String.Format("<- Linktest.req"));
                                    }
                                    break;
                                case 6:
                                    {
                                        commLogger.AppendText(String.Format("<- Linktest.rsp"));
                                    }
                                    break;
                                case 9:
                                    {
                                        commLogger.AppendText(String.Format("<- Seperate.req"));
                                    }
                                    break;
                            }
                            commLogger.ShowBinaryData(pEventData.Head);
                        }
                    }
                    break;
                #endregion

                #region Receive Message
                case ProVTool.EVENTID.EVENT_RECV_MSG:
                    {
                        if (pEventData.CtrlMsg == 0)
                        {
                            //Log Received Message
                            if (pEventData.W_Bit)
                            {
                                commLogger.AppendText(String.Format("-> S{0}F{1} W", pEventData.S, pEventData.F));
                            }
                            else
                            {
                                commLogger.AppendText(String.Format("-> S{0}F{1}", pEventData.S, pEventData.F));
                            }
                            commLogger.ShowBinaryData(pEventData.Head);
                            commLogger.ShowBinaryData(pEventData.RawData);
                            commLogger.ShowSECSIIMessage(pEventData.RawData);

                            //Process Received Message
                            //Not In On-Line State, Only Can Process S1F13 and S1F17
                            if (CurCtrlState == ProVGEMControlFSM.StateID.OFFLINE ||
                                CurCtrlState == ProVGEMControlFSM.StateID.EQPOFFLINE || 
                                CurCtrlState == ProVGEMControlFSM.StateID.HOSTOFFLINE ||
                                CurCtrlState == ProVGEMControlFSM.StateID.ATTEMPTONLINE
                                )
                            {
                                switch (pEventData.S)
                                {
                                    case 1:
                                        {
                                            switch (pEventData.F)
                                            {
                                                case 0:
                                                    {
                                                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                                                        {
                                                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F0);
                                                            CtrlFSM.Execute();
                                                        }
                                                    }
                                                    break;
                                                case 2:
                                                    {
                                                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                                                        {
                                                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F2);
                                                            CtrlFSM.Execute();
                                                        }
                                                    }
                                                    break;
                                                case 13:
                                                    {
                                                        //Will Reply S1F14 Establish Communications Request Acknowledge
                                                        HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_RCV_S1F13);
                                                        HostCommFSM.Execute();
                                                    }
                                                    break;
                                                case 17:
                                                    {
                                                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                                                        {
                                                            //Host Request OnLine
                                                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F17);
                                                            CtrlFSM.Execute();
                                                        }
                                                    }
                                                    break;
                                                case 14:
                                                    {
                                                        int lAddress = 0;
                                                        SECS_LIST listData = new SECS_LIST();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                                        SECS_BINARY bData = new SECS_BINARY();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref bData);
                                                        if (bData.Data == 0)
                                                        {
                                                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMMACK_0);
                                                        }
                                                        else
                                                        {
                                                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_CONN_TRANSACTION_FAILURE);
                                                        }
                                                        EQPCommFSM.Execute();
                                                    }
                                                    break;
                                                default:
                                                    {
                                                        if (pEventData.W_Bit)
                                                        {
                                                            List<byte> sb = new List<byte>();
                                                            sb.Clear();
                                                            SECSEngine.SendHSMSMessage(pEventData.S, 0, false, sb.ToArray());
                                                            commLogger.AppendText(String.Format("Abort Transaction With S{0}F{1} Cause Not In ON-LINE State", pEventData.S, pEventData.F));
                                                        }
                                                    }
                                                    break;
                                            }

                                        }
                                        break;
                                    default:
                                        {
                                            if (pEventData.W_Bit)
                                            {
                                                List<byte> sb = new List<byte>();
                                                sb.Clear();
                                                SECSEngine.SendHSMSMessage(pEventData.S, 0, false, sb.ToArray());
                                                commLogger.AppendText(String.Format("Abort Transaction With S{0}F{1} Cause Not In ON-LINE State", pEventData.S, pEventData.F));
                                            }
                                        }
                                        break;
                                }
                            }
                            else //In ON-LINE State
                            {
                                switch (pEventData.S) //Stream
                                {
                                    case 1:
                                        {
                                            ProcessSType_1(pEventData);
                                        }
                                        break;
                                    case 2:
                                        {
                                            ProcessSType_2(pEventData);
                                        }
                                        break;
                                    case 5:
                                        {
                                            ProcessSType_5(pEventData);
                                        }
                                        break;
                                    case 6:
                                        {
                                            ProcessSType_6(pEventData);   
                                        }
                                        break;
                                    case 7:
                                        {
                                            ProcessSType_7(pEventData);
                                        }
                                        break;
                                    case 10:
                                        {
                                            ProcessSType_10(pEventData);   
                                        }
                                        break;
                                    default:
                                        {
                                            //S9,F3 Unrecognized Stream Type (USN)
                                            if (pEventData.W_Bit)
                                            {
                                                List<byte> sb = new List<byte>();
                                                SECSEngine.SendHSMSMessage(9, 3, false, sb.ToArray());
                                            }
                                        }
                                        break;
                                    #endregion
                                }

                                if (TransmitSpoolData != null)
                                {
                                    if ((pEventData.S == TransmitSpoolData.SType && pEventData.F == TransmitSpoolData.FType + 1) || TransmitSpoolData.W_Bit == false)
                                    {
                                        TransmitSpoolData = null;
                                        SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_TRANSMIT);
                                        SpoolUnloadFSM.Execute();
                                    }
                                }
                            }
                        }
                        else
                        {
                            switch (pEventData.CtrlMsg)
                            {
                                case 1:
                                    {
                                        commLogger.AppendText(String.Format("-> Select.req"));
                                    }
                                    break;
                                case 2:
                                    {
                                        commLogger.AppendText(String.Format("-> Select.rsp"));
                                    }
                                    break;
                                case 5:
                                    {
                                        commLogger.AppendText(String.Format("-> Linktest.req"));
                                    }
                                    break;
                                case 6:
                                    {
                                        commLogger.AppendText(String.Format("-> Linktest.rsp"));
                                    }
                                    break;
                                case 9:
                                    {
                                        commLogger.AppendText(String.Format("-> Seperate.req"));
                                    }
                                    break;
                            }

                            commLogger.ShowBinaryData(pEventData.Head);
                        }
                    }
                    break;
                case ProVTool.EVENTID.EVENT_REPLY_TIMEOUT:
                    {
                        if (pEventData.S == 1 && pEventData.F == 13)
                        {
                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_CONN_TRANSACTION_FAILURE);
                            EQPCommFSM.Execute();
                        }
                    }
                    break;
                
            }
            return default(long);
        }
        #endregion

        #region Handler For Stream Type 1
        private void ProcessSType_1(ProVTool.EventData pEventData)
        {
            switch (pEventData.F) //Function
            {
                case 0: //S1F0 Abort Transaction
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F0);
                            CtrlFSM.Execute();
                        }
                    }
                    break;
                case 1: //S1F1 Are You There Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST(2);
                            SECSEngine.EncodeDataItem(ref sb, listData);
                            SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                            SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                            SECSEngine.SendHSMSMessage(1, 2, false, sb.ToArray());
                        }
                    }
                    break;
                case 2: //S1F2 ON Line Data
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F2);
                            CtrlFSM.Execute();
                        }
                    }
                    break;
                case 3: //Selected Equipment Status Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            if (listData.Data == 0)
                            {
                                listData = new SECS_LIST(SVList.Count);
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < SVList.Count; ++i)
                                {
                                    ushort svid = SVList[i].SVID;
                                    SVData svData = SVList.Find(x => x.SVID == (ushort)(svid));
                                    if (svData != null)
                                    {
                                        SECS_BASE SECSData = QueryValueEventHandler(svData.Name, svData.Format, svData.Code);
                                        SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    }
                                }
                            }
                            else
                            {
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    SECS_U2 svid = new SECS_U2();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref svid);
                                    SVData svData = SVList.Find(x => x.SVID == (ushort)(svid.Data));
                                    if (svData != null)
                                    {
                                        SECS_BASE SECSData = QueryValueEventHandler(svData.Name, svData.Format, svData.Code);
                                        SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    }
                                }
                            }
                            SECSEngine.SendHSMSMessage(1, 4, false, sb.ToArray());
                        }
                    }
                    break;
                case 11: //S1F11 Status Variable Namelist Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            if (listData.Data == 0)
                            {
                                listData = new SECS_LIST(SVList.Count);
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < SVList.Count; ++i)
                                {
                                    listData = new SECS_LIST(3);
                                    SECSEngine.EncodeDataItem(ref sb, listData);
                                    ushort svid = SVList[i].SVID;
                                    SVData svData = SVList.Find(x => x.SVID == svid);
                                    if (svData != null)
                                    {
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_U2(svData.SVID));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(svData.Name));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(svData.Unit));
                                    }
                                }
                            }
                            else
                            {
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    listData = new SECS_LIST(3);
                                    SECSEngine.EncodeDataItem(ref sb, listData);
                                    SECS_U2 svid = new SECS_U2();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref svid);
                                    SVData svData = SVList.Find(x => x.SVID == (ushort)(svid.Data));
                                    if (svData != null)
                                    {
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_U2(svData.SVID));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(svData.Name));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(svData.Unit));
                                    }
                                }
                            }
                            SECSEngine.SendHSMSMessage(1, 12, false, sb.ToArray());
                        }
                    }
                    break;
                case 13:
                    {
                        HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_RCV_S1F13);
                        HostCommFSM.Execute();
                    }
                    break;
                case 14:
                    {
                        int lAddress = 0;
                        SECS_LIST listData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                        SECS_BINARY bData = new SECS_BINARY();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref bData);
                        if (bData.Data == 0)
                        {
                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_COMMACK_0);
                        }
                        else
                        {
                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_CONN_TRANSACTION_FAILURE);
                        }
                        EQPCommFSM.Execute();
                    }
                    break;
                case 15:
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F15);
                            CtrlFSM.Execute();
                        }
                    }
                    break;
                case 17:
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F17);
                            CtrlFSM.Execute();
                        }

                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 2
        private void ProcessSType_2(ProVTool.EventData pEventData)
        {
            switch (pEventData.F)
            {
                case 13: //S2F13 Equipment Constant Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            if (listData.Data == 0)
                            {
                                listData = new SECS_LIST(ECList.Count);
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < ECList.Count; ++i)
                                {
                                    ushort ecid = ECList[i].ECID;
                                    ECData ecData = ECList.Find(x => x.ECID == (ushort)(ecid));
                                    if (ecData != null)
                                    {
                                        SECS_BASE SECSData = QueryValueEventHandler(ecData.Name, ecData.Format, ecData.Code);
                                        SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    }
                                }
                            }
                            else
                            {
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    SECS_U2 ecid = new SECS_U2();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecid);
                                    ECData ecData = ECList.Find(x => x.ECID == (ushort)(ecid.Data));
                                    if (ecData != null)
                                    {
                                        SECS_BASE SECSData = QueryValueEventHandler(ecData.Name, ecData.Format, ecData.Code);
                                        SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    }
                                }
                            }
                            SECSEngine.SendHSMSMessage(2, 14, false, sb.ToArray());
                        }
                    }
                    break;
                case 15: //S2F15 New Equipment Constant Send
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            int iRet = 0;
                            String ParamName = String.Empty;
                            ExchangeData data = new ExchangeData();

                            if (OnDataExchangeEvent != null)
                            {
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    SECS_LIST listData2 = new SECS_LIST();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData2);

                                    SECS_U2 ecid = new SECS_U2();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecid);
                                    ECData ecData = ECList.Find(x => x.ECID == (ushort)(ecid.Data));

                                    #region Decode ECID Value
                                    if (ecData != null)
                                    {
                                        data.DataName = ecData.Name;
                                        data.Code = ecData.Code;
                                        switch (ecData.Format)
                                        {
                                            case "BOOL":
                                                {
                                                    SECS_BOOLEAN ecv = new SECS_BOOLEAN();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "A":
                                                {
                                                    SECS_ASCII ecv = new SECS_ASCII();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "B": //Binary
                                                {
                                                    SECS_BINARY ecv = new SECS_BINARY();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "I1":
                                                {
                                                    SECS_I1 ecv = new SECS_I1();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "I2":
                                                {
                                                    SECS_I2 ecv = new SECS_I2();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "I4":
                                                {
                                                    SECS_I4 ecv = new SECS_I4();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "I8":
                                                {
                                                    SECS_I8 ecv = new SECS_I8();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "U1":
                                                {
                                                    SECS_U1 ecv = new SECS_U1();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "U2":
                                                {
                                                    SECS_U2 ecv = new SECS_U2();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "U4":
                                                {
                                                    SECS_U4 ecv = new SECS_U4();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "U8":
                                                {
                                                    SECS_U8 ecv = new SECS_U8();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "F4":
                                                {
                                                    SECS_F4 ecv = new SECS_F4();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;
                                            case "F8":
                                                {
                                                    SECS_F8 ecv = new SECS_F8();
                                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecv);
                                                    ParamName = String.Format("P{0}", i + 1);
                                                    data.ParamMap.SetParameter(ParamName, ecv.Data);
                                                }
                                                break;

                                        }
                                        data = OnDataExchangeEvent(EventType.WRITE, data);
                                        iRet |= data.RetData.ToInt();
                                    }
                                    #endregion
                                }
                            }

                            if (iRet != 0)
                            {
                                SECSEngine.EncodeDataItem(ref sb, new SECS_BINARY(1));
                                SECSEngine.SendHSMSMessage(2, 16, false, sb.ToArray());
                            }
                            else
                            {
                                SECSEngine.EncodeDataItem(ref sb, new SECS_BINARY(0));
                                SECSEngine.SendHSMSMessage(2, 16, false, sb.ToArray());
                            }
                        }
                    }
                    break;
                case 17: //S2F17 Date and Time Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            List<byte> sb = new List<byte>();
                            String DTStr = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmmss");
                            SECS_ASCII SECSData = new SECS_ASCII(DTStr);
                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                            SECSEngine.SendHSMSMessage(2, 18, false, sb.ToArray());
                        }
                    }
                    break;
                case 23: //S2F23 Trace Initialize Send
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);


                            SECS_U2 trid = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref trid);
                            tdc = TRList.Find(x => x.TRID == (ushort)(trid.Data));
                            if (tdc == null)
                            {
                                tdc = new TraceDataCollection();
                                TRList.Add(tdc);
                            }

                            SECS_ASCII dsper = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref dsper);
                            SECS_U2 totsmp = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref totsmp);
                            SECS_U2 repgsz = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref repgsz);

                            tdc.TRID = trid.Data;
                            String strTemp = dsper.Data;
                            int hh = Convert.ToInt32(strTemp.Substring(0, 2));
                            int mm = Convert.ToInt32(strTemp.Substring(2, 2));
                            int ss = Convert.ToInt32(strTemp.Substring(4, 2));
                            tdc.DSPER = (ushort)(ss + mm * 60 + hh * 60);
                            tdc.TOTSMP = totsmp.Data;
                            tdc.REPGSZ = repgsz.Data;
                            tdc.QuerySVEvent += tdc_QuerySVEvent;

                            tdc.SVDataList.Clear();
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            for (int i = 0; i < listData.Data; ++i)
                            {
                                SECS_U2 svid = new SECS_U2();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref svid);
                                tdc.SVDataList.Add(svid.Data);
                            }

                            if (tdc.TOTSMP == 0)
                            {
                                tdc.Stop();
                                tdc.SVDataList.Clear();
                            }
                            else
                            {
                                tdc.Start();
                            }


                            SECS_BINARY TIACK = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, TIACK);
                            SECSEngine.SendHSMSMessage(2, 24, false, sb.ToArray());
                        }
                    }
                    break;
                case 29: //S2F29 Equipment Constane Namelist Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            if (listData.Data == 0)
                            {
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    listData = new SECS_LIST(ECList.Count);
                                    SECS_LIST listData2 = new SECS_LIST(6);
                                    SECSEngine.EncodeDataItem(ref sb, listData2);
                                    ushort ecid = ECList[i].ECID;
                                    ECData ecData = ECList.Find(x => x.ECID == ecid);
                                    if (ecData != null)
                                    {
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_U2(ecData.ECID));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(ecData.Name));
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Min);
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Max);
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Value);
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(ecData.Unit));
                                    }
                                }
                            }
                            else
                            {
                                SECSEngine.EncodeDataItem(ref sb, listData);
                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    SECS_LIST listData2 = new SECS_LIST(6);
                                    SECSEngine.EncodeDataItem(ref sb, listData2);
                                    SECS_U2 ecid = new SECS_U2();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ecid);
                                    ECData ecData = ECList.Find(x => x.ECID == (ushort)(ecid.Data));
                                    if (ecData != null)
                                    {
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_U2(ecData.ECID));
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(ecData.Name));
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Min);
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Max);
                                        SECSEngine.EncodeDataItem(ref sb, ecData.Value);
                                        SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(ecData.Unit));
                                    }
                                }
                            }
                            SECSEngine.SendHSMSMessage(2, 30, false, sb.ToArray());
                        }
                    }
                    break;
                case 31: //S2F31 Date and Time Set Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            SECS_ASCII time = new SECS_ASCII();
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref time);
                            String strTime = time.Data;
                            if (strTime.Length == 12)
                            {
                                SystemTime sysTime = new SystemTime();
                                sysTime.wYear = Convert.ToUInt16(strTime.Substring(0, 4));
                                sysTime.wMonth = Convert.ToUInt16(strTime.Substring(4, 2));
                                sysTime.wDay = Convert.ToUInt16(strTime.Substring(6, 2));
                                sysTime.wHour = Convert.ToUInt16(strTime.Substring(8, 2));
                                sysTime.wMinute = Convert.ToUInt16(strTime.Substring(10, 2));
                                sysTime.wSecond = Convert.ToUInt16(strTime.Substring(12, 2));
                                SetLocalTime(ref sysTime);
                            }
                            SECS_BINARY TACK = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, TACK);
                            SECSEngine.SendHSMSMessage(2, 32, false, sb.ToArray());
                        }
                    }
                    break;
                case 33: //S2F33 Define Report
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_U2 u2Data = new SECS_U2(); //DATAID
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            int La = listData.Data; //Report Number
                            for (int i = 0; i < La; ++i)
                            {
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //L2
                                u2Data = new SECS_U2(); //RPTID
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                int Lb = listData.Data; //Lb
                                RPTData rpt = RPTList.Find(x => x.RPTID == (ushort)(u2Data.Data));
                                if (rpt != null)
                                {
                                    rpt.RemoveVID(null); //Clear All VID with this Report
                                    for (int j = 0; j < Lb; ++j) //VIDs
                                    {
                                        u2Data = new SECS_U2(); //VID
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                                        rpt.AddVID(GetVID(u2Data.Data)); //Re-Create VID List with this Report
                                    }
                                }
                            }

                            SECS_BINARY binaryData = new SECS_BINARY(0); //DRACK
                            List<byte> sb = new List<byte>();
                            SECSEngine.EncodeDataItem(ref sb, binaryData);
                            SECSEngine.SendHSMSMessage(2, 34, false, sb.ToArray());
                        }
                    }
                    break;
                case 35: //S2F35 Link Event Report
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_U2 u2Data = new SECS_U2(); //DATAID
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            int La = listData.Data; //Event Number
                            for (int i = 0; i < La; ++i)
                            {
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //L2
                                u2Data = new SECS_U2(); //CEID
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                int Lb = listData.Data; //Lb
                                EventReport ceReport = EVTReportList.Find(x => x.CEID == (ushort)(u2Data.Data));
                                if (ceReport != null)
                                {
                                    ceReport.RemoveRPT(null); //Clear All RPT with this Report
                                    for (int j = 0; j < Lb; ++j) //VIDs
                                    {
                                        u2Data = new SECS_U2();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref u2Data);
                                        RPTData rpt = RPTList.Find(x => x.RPTID == (ushort)(u2Data.Data));
                                        if (rpt != null)
                                            ceReport.AddRPT(rpt); //Re-Create RPT List with this CEID
                                    }
                                }
                            }

                            SECS_BINARY binaryData = new SECS_BINARY(0); //LRACK
                            List<byte> sb = new List<byte>();
                            SECSEngine.EncodeDataItem(ref sb, binaryData);
                            SECSEngine.SendHSMSMessage(2, 36, false, sb.ToArray());
                        }
                    }
                    break;
                case 37: //S2F37 Enable/Disable Event Report
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_BOOLEAN bData = new SECS_BOOLEAN(); //CEED
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref bData);
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            for (int i = 0; i < listData.Data; ++i)
                            {
                                SECS_U2 ceID = new SECS_U2();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ceID);
                                EventReport rpt = EVTReportList.Find(x => x.CEID == (ceID.Data));
                                rpt.Enabled = bData.Data;
                            }
                            SECS_BINARY ERACK = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, ERACK);
                            SECSEngine.SendHSMSMessage(2, 38, false, sb.ToArray());
                        }
                    }
                    break;
                case 41: //S2F41 Host Command Send
                    {
                        /*
                            0 = Acknowledge, command has been performed
                            1 = Command does not exist
                            2 = Cannot perform now
                            3 = At least one parameter isinvalid
                            4 = Acknowledge, command will be performed with completion signaled later by an event
                            5 = Rejected, Already in Desired Condition
                            6 = No such object exists
                            7-63 Reserved
                        */
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII cmd = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref cmd);
                            switch (cmd.Data)
                            {
                                case "INITIAL":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //START Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }

                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.INITIAL;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "START":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //START Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }

                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.START;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "PP_SELECT":
                                    {
                                        String PPName = String.Empty;
                                        SECS_ASCII CPName = new SECS_ASCII();
                                        SECS_ASCII CPVal = new SECS_ASCII();
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //PP-SELECT Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i) //Will be N Parameters
                                        {
                                            SECS_LIST listData2 = new SECS_LIST();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData2); //Should be 2
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CPName);
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CPVal);
                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.PPSELECT;
                                            data.ParamMap.SetParameter("PPName", CPVal.Data);
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();
                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "STOP":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //STOP Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.STOP;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "PAUSE":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //PAUSE Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.PAUSE;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "RESUME":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //RESUME Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.RESUME;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "ABORT":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //ABORT Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.ABORT;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                                case "LOTEND":
                                    {
                                        listData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                                        //STOP Command Parameter if any
                                        for (int i = 0; i < listData.Data; ++i)
                                        {

                                        }
                                        if (OnDataExchangeEvent != null)
                                        {
                                            //Execute Command
                                            ExchangeData data = new ExchangeData();
                                            data.HostCMD = HOSTCMD.LOTEND;
                                            data = OnDataExchangeEvent(EventType.COMMAND, data);
                                            int RetCode = data.RetData.ToInt();

                                            //Reply S2F42
                                            listData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECS_BINARY HACK = new SECS_BINARY((byte)RetCode);
                                            SECSEngine.EncodeDataItem(ref sb, HACK);
                                            listData = new SECS_LIST(0);
                                            SECSEngine.EncodeDataItem(ref sb, listData);
                                            SECSEngine.SendHSMSMessage(2, 42, false, sb.ToArray());

                                            sb.Clear();
                                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(11));
                                            CurEventName = report.EventName;

                                            if (report != null)
                                            {
                                                sb = report.EncodeEventReport();
                                                SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    break;
                case 43: //S2F43 Reset Spooling Streams and Functions
                    {
                        Dictionary<byte, List<byte>> ErrSnFnMap = new Dictionary<byte, List<byte>>();
                        int lAddress = 0;
                        SECS_LIST lstData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData);
                        int Lm = lstData.Data;
                        if (Lm == 0) //A zero-length list, Lm = 0, turns off spooling for all streams and functions
                        {
                            foreach (SnFnData Item in SnFnList)
                            {
                                Item.EnableSpool = false;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Lm; ++i)
                            {
                                lstData = new SECS_LIST();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData);
                                SECS_BINARY STRID = new SECS_BINARY();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref STRID);
                                if (STRID.Data == 1) //Spooling for Stream 1 is not allowed
                                    continue;
                                lstData = new SECS_LIST();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData);
                                int Ln = lstData.Data;
                                if (Ln == 0) //A zero-length list, Ln = 0, turns on spooling for all functions for the associated stream 
                                {
                                    SECS_BINARY FCNID = new SECS_BINARY();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref FCNID);
                                    IEnumerable<SnFnData> itemList = SnFnList.Where(x => x.SType == STRID.Data);
                                    foreach (SnFnData Item in itemList)
                                    {
                                        Item.EnableSpool = true;
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < Ln; ++j) //Function
                                    {
                                        SECS_BINARY FCNID = new SECS_BINARY();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref FCNID);
                                        SnFnData snfnData = new SnFnData(STRID.Data, FCNID.Data);
                                        SnFnData findit = SnFnList.Find(x => (x.SType == STRID.Data && x.FType == FCNID.Data));
                                        if (findit != null)
                                        {
                                            if(findit.FType % 2 == 1) //Primary Message
                                                findit.EnableSpool = true;
                                        }
                                        else
                                        {
                                            if(ErrSnFnMap.ContainsKey(STRID.Data))
                                            {
                                                List<byte> fcnList = null;
                                                if(ErrSnFnMap.TryGetValue(STRID.Data, out fcnList))
                                                {
                                                    if(!fcnList.Contains(FCNID.Data))
                                                        fcnList.Add(FCNID.Data);
                                                }
                                            }
                                            else
                                            {
                                                List<byte> fcnList = new List<byte>();
                                                fcnList.Add(FCNID.Data);
                                                ErrSnFnMap.Add(STRID.Data, fcnList);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        lAddress = 0;
                        List<byte> sb = new List<byte>();
                        lstData = new SECS_LIST(2);
                        SECSEngine.EncodeDataItem(ref sb, lstData);
                        if (ErrSnFnMap.Count == 0)
                        {
                            SECS_BINARY RSPACK = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, RSPACK);
                            lstData = new SECS_LIST(0); //Lm = 0, is given, indicating no streams or functions in error.
                            SECSEngine.EncodeDataItem(ref sb, lstData);
                        }
                        else
                        {
                            //STRACK
                            //1 = Spooling not allowed for stream (i.e., Stream 1)
                            //2 = Stream unknown 
                            //3 = Unknown function specified for this stream  
                            //4 = Secondary function specified for this stream
                            SECS_BINARY RSPACK = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, RSPACK);
                            lstData = new SECS_LIST(ErrSnFnMap.Count);
                            SECSEngine.EncodeDataItem(ref sb, lstData);
                            foreach (byte k in ErrSnFnMap.Keys)
                            {
                                lstData = new SECS_LIST(3);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                SECS_BINARY STRID = new SECS_BINARY(k);
                                SECSEngine.EncodeDataItem(ref sb, STRID);
                                SECS_BINARY STRACK = new SECS_BINARY(3);
                                SECSEngine.EncodeDataItem(ref sb, STRACK);
                                List<byte> fcnList = null;
                                if (ErrSnFnMap.TryGetValue(k, out fcnList))
                                {
                                    lstData = new SECS_LIST(fcnList.Count);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    foreach (byte f in fcnList)
                                    {
                                        SECS_BINARY FCNID = new SECS_BINARY(f);
                                        SECSEngine.EncodeDataItem(ref sb, FCNID);
                                    }
                                }
                            }
                        }
                        SECSEngine.SendHSMSMessage(2, 44, false, sb.ToArray());
                    }
                    break;
                case 45:
                    {
                        ushort QueryInterval = 1;
                        ECData ecData = ECList.Find(x => x.ECID == (ushort)1013);
                        if (ecData != null)
                        {
                            SECS_U2 uData = ecData.Value as SECS_U2;
                            QueryInterval = uData.Data;
                        }
                        int lAddress = 0;
                        SECS_LIST lstData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //L2
                        SECS_U2 DATAID = new SECS_U2();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref DATAID);
                        lstData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //Lm, (m = # of variables in this definition)
                        int Lm = lstData.Data;
                        if (Lm == 0) //A zero-length list, Lm = 0, sets all limit values for all monitored VIDs to “undefined.”
                        {
                            foreach (KeyValuePair<ushort, ProVGEMLimitFSM.ProVLimitFSM> item in LimitMap)
                            {
                                item.Value.Send(ProVGEMLimitFSM.EventID.EVT_UNDEFINELIMIT);
                                item.Value.Execute();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Lm; ++i)
                            {
                                lstData = new SECS_LIST();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //L2   
                                SECS_U2 VID = new SECS_U2();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref VID); //VID
                                lstData = new SECS_LIST();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //Ln, (n = # of limits being defined/changed for VID1)
                                int Ln = lstData.Data;
                                if (Ln == 0)
                                {
                                    ProVGEMLimitFSM.ProVLimitFSM item = null;
                                    if (LimitMap.TryGetValue(VID.Data, out item))
                                    {
                                        item.Send(ProVGEMLimitFSM.EventID.EVT_UNDEFINELIMIT);
                                        item.Execute();
                                    }
                                }
                                else
                                {
                                    for (int j = 0; j < Ln; ++j)
                                    {
                                        lstData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //L2 
                                        SECS_U2 LIMITID = new SECS_U2();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LIMITID); //LIMITID
                                        lstData = new SECS_LIST();
                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //Lp 
                                        int Lp = lstData.Data;
                                        
                                        VIDData vidData = GetVID(VID.Data);
                                        if (vidData != null)
                                        {

                                            switch (vidData.Format)
                                            {
                                                case "B":
                                                    {
                                                        SECS_BINARY UPPERDB = new SECS_BINARY();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_BINARY LOWERDB = new SECS_BINARY();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "B", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }
                                                    }
                                                    break;
                                                case "I1":
                                                    {
                                                        SECS_I1 UPPERDB = new SECS_I1();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_I1 LOWERDB = new SECS_I1();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "I1", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "I2":
                                                    {
                                                        SECS_I2 UPPERDB = new SECS_I2();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_I2 LOWERDB = new SECS_I2();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "I2", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "I4":
                                                    {
                                                        SECS_I4 UPPERDB = new SECS_I4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_I4 LOWERDB = new SECS_I4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "I4", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "U1":
                                                    {
                                                        SECS_U1 UPPERDB = new SECS_U1();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_U1 LOWERDB = new SECS_U1();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "U1", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "U2":
                                                    {
                                                        SECS_U2 UPPERDB = new SECS_U2();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_U2 LOWERDB = new SECS_U2();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "U2", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "U4":
                                                    {
                                                        SECS_U4 UPPERDB = new SECS_U4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_U4 LOWERDB = new SECS_U4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "U4", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "F4":
                                                    {
                                                        SECS_F4 UPPERDB = new SECS_F4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_F4 LOWERDB = new SECS_F4();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "F4", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                                case "F8":
                                                    {
                                                        SECS_F8 UPPERDB = new SECS_F8();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref UPPERDB); //UPPERDB 
                                                        SECS_F8 LOWERDB = new SECS_F8();
                                                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref LOWERDB); //LOWERDB 
                                                        if (!LimitMap.ContainsKey(VID.Data))
                                                        {
                                                            LimitMap.Add(VID.Data, new ProVGEMLimitFSM.ProVLimitFSM(VID.Data, LIMITID.Data, "F8", UPPERDB, LOWERDB));
                                                            LimitMap[VID.Data].QueryDataValue += OnQueryDataValue;
                                                            LimitMap[VID.Data].TransitionCompleted += LimitFSM_TransitionCompleted;
                                                        }
                                                        else
                                                        {
                                                            LimitMap[VID.Data].UPPERDB = UPPERDB;
                                                            LimitMap[VID.Data].LOWERDB = LOWERDB;
                                                        }

                                                    }
                                                    break;
                                            }

                                            if (LimitMap.ContainsKey(VID.Data))
                                            {
                                                LimitMap[VID.Data].QueryInterval = QueryInterval;
                                                LimitMap[VID.Data].Send(ProVGEMLimitFSM.EventID.EVT_DEFINELIMIT);
                                                LimitMap[VID.Data].Execute();
                                            }
                                        }

                                    }
                                }
                            }

                        }

                        //S2F46, Variable Limit Attribute Acknowledge
                        List<byte> sb = new List<byte>();
                        lstData = new SECS_LIST(2);
                        SECSEngine.EncodeDataItem(ref sb, lstData);
                        SECS_BINARY VLAACK = new SECS_BINARY(0); //Accept Command, will perform
                        SECSEngine.EncodeDataItem(ref sb, VLAACK);
                        lstData = new SECS_LIST(0); //Lm = 0, indicates no invalid variable limit attributes
                        SECSEngine.EncodeDataItem(ref sb, lstData);
                        SECSEngine.SendHSMSMessage(2, 46, false, sb.ToArray());
                    }
                    break;
                case 47: //S2F47, Variable Limit Attribute Request
                    {
                        List<byte> sb = new List<byte>();
                        int lAddress = 0;
                        SECS_LIST lstData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref lstData); //Lm
                        int Lm = lstData.Data;
                        SECSEngine.EncodeDataItem(ref sb, lstData);
                        for (int i = 0; i < Lm; ++i)
                        {
                            lstData = new SECS_LIST(2);
                            SECSEngine.EncodeDataItem(ref sb, lstData);

                            SECS_U2 VID = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref VID); //VID

                            SECSEngine.EncodeDataItem(ref sb, VID);

                            VIDData vidData = GetVID(VID.Data);
                            if (vidData != null)
                            {
                                SECS_BASE data = QueryValueEventHandler(vidData.Name, vidData.Format, vidData.Code);
                                lstData = new SECS_LIST(4);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                SECS_ASCII UNIT = new SECS_ASCII(vidData.Unit);
                                SECSEngine.EncodeDataItem(ref sb, UNIT);
                                SECSEngine.EncodeDataItem(ref sb, vidData.Min);
                                SECSEngine.EncodeDataItem(ref sb, vidData.Max);
                                lstData = new SECS_LIST(1);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                lstData = new SECS_LIST(3);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                SECS_U2 LIMITID = new SECS_U2(1);
                                SECSEngine.EncodeDataItem(ref sb, LIMITID);
                                SECSEngine.EncodeDataItem(ref sb, vidData.Max); //UPPERDB
                                SECSEngine.EncodeDataItem(ref sb, vidData.Min); //LOWERDB
                            }
                        }

                        SECSEngine.SendHSMSMessage(2, 48, false, sb.ToArray());
                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 5
        private void ProcessSType_5(ProVTool.EventData pEventData)
        {
            switch (pEventData.F) //Function
            {
                case 3: //S5F3 Enable/Disable Alarm Send
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_BINARY alED = new SECS_BINARY();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref alED);
                            SECS_U2 alID = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref alID);
                            if (alID.Items == 0) //Zero-Length Indicated All Alarms
                            {
                                foreach (ALMData almData in ArmList)
                                {
                                    almData.ALED = alED.Data;
                                }
                            }
                            else
                            {
                                ALMData almData = ArmList.Find(x => x.ALID == alID.Data);
                                almData.ALED = alED.Data;
                            }

                            SECS_BINARY ACK5 = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, ACK5);
                            SECSEngine.SendHSMSMessage(5, 4, false, sb.ToArray());
                        }
                    }
                    break;
                case 5: //S5F5 List Alarm Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            //S5F5
                            //<ALID1, . . . ,ALIDn>
                            int lAddress = 0;
                            SECS_U2 alID = new SECS_U2();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref alID);

                            //S5F6
                            //L,m
                            //  1. L,3
                            //      <ALCD1>
                            //      <ALID1>
                            //      <ALTX1>
                            //  2. L,3
                            //      .
                            //      .
                            //  m. L,3
                            //      <ALCDm>
                            //      <ALIDm>
                            //      <ALTXm>

                            List<byte> sbsend = new List<byte>();
                            if (alID.Items != 0 && alID[0] != 0)
                            {
                                SECS_LIST lstData = new SECS_LIST(alID.Items);
                                SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                for (int i = 0; i < alID.Items; ++i)
                                {
                                    ALMData almData = ArmList.Find(x => x.ALID == alID[i]);
                                    if (almData != null)
                                    {
                                        lstData = new SECS_LIST(3);
                                        SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                        SECS_BINARY alCD = new SECS_BINARY(almData.ALCD);
                                        SECSEngine.EncodeDataItem(ref sbsend, alCD);
                                        SECS_U2 alIDSend = new SECS_U2(almData.ALID);
                                        SECSEngine.EncodeDataItem(ref sbsend, alIDSend);
                                        SECS_ASCII alTX = new SECS_ASCII(almData.ALTX);
                                        SECSEngine.EncodeDataItem(ref sbsend, alTX);
                                    }
                                    else
                                    {
                                        //A zero-length item returned for ALCDi or ALTXi means that value does not exist
                                        lstData = new SECS_LIST(0);
                                        SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                    }
                                }
                            }
                            else //A zero-length item means send all possible alarms regardless of the state of ALED.
                            {
                                SECS_LIST lstData = new SECS_LIST(ArmList.Count);
                                SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                for (int i = 0; i < ArmList.Count; ++i)
                                {
                                    ALMData almData = ArmList[i];
                                    lstData = new SECS_LIST(3);
                                    SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                    SECS_BINARY alCD = new SECS_BINARY(almData.ALCD);
                                    SECSEngine.EncodeDataItem(ref sbsend, alCD);
                                    SECS_U2 alIDSend = new SECS_U2(almData.ALID);
                                    SECSEngine.EncodeDataItem(ref sbsend, alIDSend);
                                    SECS_ASCII alTX = new SECS_ASCII(almData.ALTX);
                                    SECSEngine.EncodeDataItem(ref sbsend, alTX);
                                }
                            }
                            SECSEngine.SendHSMSMessage(5, 6, false, sbsend.ToArray());
                        }
                    }
                    break;
                case 7: //S5F7 List Enabled Alarm Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            //S5F8
                            //L,m
                            //  1. L,3
                            //      <ALCD1>
                            //      <ALID1>
                            //      <ALTX1>
                            //  2. L,3
                            //      .
                            //      .
                            //  m. L,3
                            //      <ALCDm>
                            //      <ALIDm>
                            //      <ALTXm>

                            List<byte> sbsend = new List<byte>();
                            List<ALMData> lstALMData = ArmList.Where(x => x.ALED == 0x1).ToList();

                            SECS_LIST lstData = new SECS_LIST(lstALMData.Count);
                            SECSEngine.EncodeDataItem(ref sbsend, lstData);
                            foreach (ALMData almData in lstALMData)
                            {
                                lstData = new SECS_LIST(3);
                                SECSEngine.EncodeDataItem(ref sbsend, lstData);
                                SECS_BINARY alCD = new SECS_BINARY(almData.ALCD);
                                SECSEngine.EncodeDataItem(ref sbsend, alCD);
                                SECS_U2 alIDSend = new SECS_U2(almData.ALID);
                                SECSEngine.EncodeDataItem(ref sbsend, alIDSend);
                                SECS_ASCII alTX = new SECS_ASCII(almData.ALTX);
                                SECSEngine.EncodeDataItem(ref sbsend, alTX);
                            }
                            SECSEngine.SendHSMSMessage(5, 8, false, sbsend.ToArray());
                        }
                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 6
        private void ProcessSType_6(ProVTool.EventData pEventData)
        {
            switch (pEventData.F) //Function
            {
                case 11: //S6F11
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_RCVS1F0);
                            CtrlFSM.Execute();
                        }
                    }
                    break;
                case 15:  //S6F15 Host Request an Event Report
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            SECS_U2 CEID = new SECS_U2();
                            int lAddress = 0;
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CEID);

                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(CEID.Data));

                            if (report != null)
                            {
                                List<byte> sb = report.EncodeEventReport();
                                SECSEngine.SendHSMSMessage(6, 16, false, sb.ToArray());
                            }
                        }
                    }
                    break;
                case 17:  //S6F17 Host Request an Event Report with Annotated
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            SECS_U2 CEID = new SECS_U2();
                            int lAddress = 0;
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CEID);

                            EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(CEID.Data));

                            if (report != null)
                            {
                                List<byte> sb = report.EncodeEventReport(true);
                                SECSEngine.SendHSMSMessage(6, 18, false, sb.ToArray());
                            }
                        }
                    }
                    break;
                case 19:  //S6F19 Individual Report Request
                    {
                        if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                        {
                            SECS_U2 RPTID = new SECS_U2();
                            int lAddress = 0;
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref RPTID);

                            RPTData report = RPTList.Find(x => x.RPTID == (ushort)(RPTID.Data));
                            int VIDNum = report.GetVIDList().Count;
                            List<byte> sb = new List<byte>();
                            SECS_LIST lstData = new SECS_LIST(VIDNum);
                            SECSEngine.EncodeDataItem(ref sb, lstData);
                            foreach (VIDData vid in report.GetVIDList())
                            {
                                SECS_BASE SECSData = QueryValueEventHandler(vid.Name, vid.Format, vid.Code);
                                SECSEngine.EncodeDataItem(ref sb, SECSData);
                            }
                            SECSEngine.SendHSMSMessage(6, 20, false, sb.ToArray());
                        }
                    }
                    break;
                case 23: //S6F23 Request Spooled Data
                    {
                        List<byte> sb = new List<byte>();
                        SECS_BINARY RSDA = new SECS_BINARY(0);
                        SECSEngine.EncodeDataItem(ref sb, RSDA);
                        SECSEngine.SendHSMSMessage(6, 24, false, sb.ToArray());

                        SECS_BINARY RSDC = new SECS_BINARY();
                        int lAddress = 0;
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref RSDC);
                        if (RSDC.Data == 0) //Transmit Message
                        {
                            SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_TRANSMIT);
                            SpoolUnloadFSM.Execute();
                        }
                        else // == 1 Purge Message
                        {
                            SpoolUnloadFSM.Send(ProVGEMSpoolFSM.EventID.EVT_PURGE);
                            SpoolUnloadFSM.Execute();
                        }

                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 7
        private void ProcessSType_7(ProVTool.EventData pEventData)
        {
            switch (pEventData.F)
            {
                case 1://S7F1 Process Program Load Inquire
                    {
                        //S7F2 PPGNT
                        //0 = OK
                        //1 = Already have
                        //2 = No space
                        //3 = Invalid PPID
                        //4 = Busy, try later
                        //5 = Will not accept
                        //>5 = Other error
                        //6-63 Reserved
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData);

                            ExchangeData data = new ExchangeData();
                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("EXISTPPID", ascData.Data);
                            data = OnDataExchangeEvent(EventType.QUERY, data);
                            int iRet = data.RetData.ToInt();
                            SECS_BINARY bData = new SECS_BINARY((byte)iRet);
                            sb.Clear();
                            SECSEngine.EncodeDataItem(ref sb, listData);
                            SECSEngine.SendHSMSMessage(7, 2, false, sb.ToArray());
                        }
                    }
                    break;
                case 2://S7F2 Process Program Load Grant
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            SECS_BINARY binData = new SECS_BINARY();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref binData);
                            if (binData.Data == 0) //可以將LoadInquirePPID對應的Package內容往Host傳
                            {
                                lAddress = 0;
                                List<byte> sb = new List<byte>();
                                SECS_ASCII ascData = new SECS_ASCII();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData);
                                String PPID = ascData.Data;

                                ExchangeData data = new ExchangeData();
                                data.Code = TableCode.REAL;
                                data.DataName = "PPACTION";
                                data.ParamMap.SetParameter("EXISTPPID", ascData.Data);
                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                int iRet = data.RetData.ToInt();
                                if (iRet == 0) //Not Existed
                                {
                                    sb.Clear();
                                    SECS_LIST lstData = new SECS_LIST(0);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    SECSEngine.SendHSMSMessage(7, 3, false, sb.ToArray());
                                }
                                else
                                {
                                    //Load selected package to PreloadPackageDS
                                    data = new ExchangeData();
                                    data.Code = TableCode.REAL;
                                    data.DataName = "PPACTION";
                                    data.ParamMap.SetParameter("SELPPID", LoadInquirePPID);
                                    data = OnDataExchangeEvent(EventType.QUERY, data);
                                    iRet = data.RetData.ToInt();
                                    if (iRet == 0)
                                    {
                                        if (IsFormatedPPOperation)
                                        {
                                            /*
                                            L,4
                                                <PPID>
                                                <MDLN>
                                                <SOFTREV>
                                                L,1 (c = Number of Process Commands)
                                                L,2
                                                    <CCODE>
                                                    L,p (p = Number of Parameters)
                                                    <PPARM1>
                                                    .
                                                    .
                                                    p. <PPARMp>
                                            */
                                            sb.Clear();
                                            SECS_LIST lstData = new SECS_LIST(4);
                                            SECSEngine.EncodeDataItem(ref sb, lstData);
                                            ascData = new SECS_ASCII(LoadInquirePPID);
                                            SECSEngine.EncodeDataItem(ref sb, ascData);
                                            SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                            SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                            lstData = new SECS_LIST(1); //Number of process commands
                                            SECSEngine.EncodeDataItem(ref sb, lstData);
                                            lstData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, lstData);
                                            SECS_U2 CCODE = new SECS_U2(1); //Command Code
                                            SECSEngine.EncodeDataItem(ref sb, CCODE);
                                            lstData = new SECS_LIST(PKGSchema.Count); //Number of parameters
                                            SECSEngine.EncodeDataItem(ref sb, lstData);
                                            for (int i = 0; i < PKGSchema.Count; ++i)
                                            {
                                                PKGItem Item = PKGSchema[i];
                                                data = new ExchangeData();
                                                data.Code = TableCode.PRELOADPACKAGE;
                                                data.DataName = Item.QueryName;
                                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                                switch (Item.Format)
                                                {
                                                    case "A":
                                                        {
                                                            SECSData = new SECS_ASCII(data.RetData.ToString());
                                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                        }
                                                        break;
                                                    case "BOOL":
                                                        {
                                                            SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                        }
                                                        break;
                                                    case "F4":
                                                    case "F8":
                                                        {
                                                            SECSData = new SECS_F8(data.RetData.ToDouble());
                                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                        }
                                                        break;
                                                    case "I1":
                                                    case "I2":
                                                    case "I4":
                                                    case "I8":
                                                    case "U1":
                                                    case "U2":
                                                    case "U4":
                                                    case "U8":
                                                        {
                                                            SECSData = new SECS_I4(data.RetData.ToInt());
                                                            SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                        }
                                                        break;
                                                }
                                            }
                                            SECSEngine.SendHSMSMessage(7, 23, true, sb.ToArray());

                                        }
                                        else
                                        {
                                            sb.Clear();
                                            SECS_LIST lstData = new SECS_LIST(2);
                                            SECSEngine.EncodeDataItem(ref sb, lstData);
                                            ascData = new SECS_ASCII(PPID);
                                            SECSEngine.EncodeDataItem(ref sb, ascData);

                                            //先用bData來將所有的欄位編碼成Byte List
                                            List<byte> bData = new List<byte>();

                                            for (int i = 0; i < PKGSchema.Count; ++i)
                                            {
                                                PKGItem Item = PKGSchema[i];
                                                lstData = new SECS_LIST(2);
                                                SECSEngine.EncodeDataItem(ref bData, lstData);
                                                ascData = new SECS_ASCII(Item.Name);
                                                SECSEngine.EncodeDataItem(ref bData, ascData);
                                                data = new ExchangeData();
                                                data.Code = TableCode.PRELOADPACKAGE;
                                                data.DataName = Item.QueryName;
                                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                                switch (Item.Format)
                                                {
                                                    case "A":
                                                        {
                                                            SECS_BASE SECSData = new SECS_ASCII(data.RetData.ToString());
                                                            SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                        }
                                                        break;
                                                    case "BOOL":
                                                        {
                                                            SECS_BASE SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                                            SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                        }
                                                        break;
                                                    case "F4":
                                                    case "F8":
                                                        {
                                                            SECS_BASE SECSData = new SECS_F8(data.RetData.ToDouble());
                                                            SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                        }
                                                        break;
                                                    case "I1":
                                                    case "I2":
                                                    case "I4":
                                                    case "I8":
                                                    case "U1":
                                                    case "U2":
                                                    case "U4":
                                                    case "U8":
                                                        {
                                                            SECS_BASE SECSData = new SECS_I4(data.RetData.ToInt());
                                                            SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                        }
                                                        break;
                                                }
                                            }

                                            //將Byte List用SECS_BINARY裝起來，再整個編碼成PPBODY
                                            SECS_BINARY ppbody = new SECS_BINARY(bData.ToArray());
                                            SECSEngine.EncodeDataItem(ref sb, ppbody);
                                            SECSEngine.SendHSMSMessage(7, 3, true, sb.ToArray());
                                        }
                                    }
                                    else
                                    {
                                        sb.Clear();
                                        SECS_LIST lstData = new SECS_LIST(0);
                                        SECSEngine.EncodeDataItem(ref sb, lstData);
                                        SECSEngine.SendHSMSMessage(7, 3, true, sb.ToArray());
                                    }
                                }
                            }
                            else //Host拒絕將LoadInquirePPID對應的Package內容往Host傳
                            {
                            }
                        }
                    }
                    break;
                case 3: //S7F3 Process Program Send
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //PPID
                            SECS_BINARY bData = new SECS_BINARY();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref bData); //PPBODY

                            int ppAddress = 0;
                            byte[] ppArray = new byte[bData.Items];
                            for (int i = 0; i < ppArray.Length; ++i)
                            {
                                ppArray[i] = bData[i];
                            }

                            List<PKGItem> pkgItemList = new List<PKGItem>();
                            pkgItemList.Clear();

                            ExchangeData data = new ExchangeData();
                            for (int i = 0; i < PKGSchema.Count; ++i)
                            {
                                SECS_ASCII Name = new SECS_ASCII();
                                ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref Name);

                                PKGItem item = PKGSchema[i];
                                switch (item.Format)
                                {
                                    case "A":
                                        {
                                            SECS_ASCII itemData = new SECS_ASCII();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "BOOL":
                                        {
                                            SECS_BOOLEAN itemData = new SECS_BOOLEAN();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "F4":
                                    case "F8":
                                        {
                                            SECS_F8 itemData = new SECS_F8();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "I1":
                                    case "I2":
                                    case "I4":
                                    case "I8":
                                    case "U1":
                                    case "U2":
                                    case "U4":
                                    case "U8":
                                        {
                                            SECS_I4 itemData = new SECS_I4();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                }
                            }

                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("RCVPPID", ascData.Data);
                            data.ParamMap.SetParameter("PKGItemData", pkgItemList);
                            data = OnDataExchangeEvent(EventType.WRITE, data);
                            int iRet = data.RetData.ToInt();

                            sb.Clear();
                            SECS_LIST lstData = new SECS_LIST(0);
                            SECSEngine.EncodeDataItem(ref sb, lstData);
                            SECSEngine.SendHSMSMessage(7, 4, false, sb.ToArray());
                        }
                    }
                    break;
                case 5: //S7F5 Process Program Request  H -> E
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData);
                            String PPID = ascData.Data;

                            ExchangeData data = new ExchangeData();
                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("EXISTPPID", ascData.Data);
                            data = OnDataExchangeEvent(EventType.QUERY, data);
                            int iRet = data.RetData.ToInt();
                            if (iRet == 0) //Not Existed
                            {
                                sb.Clear();
                                SECS_LIST lstData = new SECS_LIST(0);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                SECSEngine.SendHSMSMessage(7, 6, false, sb.ToArray());

                            }
                            else
                            {
                                //Load selected package to PreloadPackageDS
                                data = new ExchangeData();
                                data.Code = TableCode.REAL;
                                data.DataName = "PPACTION";
                                data.ParamMap.SetParameter("SELPPID", ascData.Data);
                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                iRet = data.RetData.ToInt();
                                if (iRet == 0)
                                {
                                    sb.Clear();
                                    SECS_LIST lstData = new SECS_LIST(2);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    ascData = new SECS_ASCII(PPID);
                                    SECSEngine.EncodeDataItem(ref sb, ascData);

                                    //先用bData來將所有的欄位編碼成Byte List
                                    List<byte> bData = new List<byte>();
                                    
                                    for (int i = 0; i < PKGSchema.Count; ++i)
                                    {
                                        PKGItem Item = PKGSchema[i];
                                        lstData = new SECS_LIST(2);
                                        SECSEngine.EncodeDataItem(ref bData, lstData);
                                        ascData = new SECS_ASCII(Item.Name);
                                        SECSEngine.EncodeDataItem(ref bData, ascData);
                                        data = new ExchangeData();
                                        data.Code = TableCode.PRELOADPACKAGE;
                                        data.DataName = Item.QueryName;
                                        data = OnDataExchangeEvent(EventType.QUERY, data);
                                        switch (Item.Format)
                                        {
                                            case "A":
                                                {
                                                    SECS_BASE SECSData = new SECS_ASCII(data.RetData.ToString());
                                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                }
                                                break;
                                            case "BOOL":
                                                {
                                                    SECS_BASE SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                }
                                                break;
                                            case "F4":
                                            case "F8":
                                                {
                                                    SECS_BASE SECSData = new SECS_F8(data.RetData.ToDouble());
                                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                }
                                                break;
                                            case "I1":
                                            case "I2":
                                            case "I4":
                                            case "I8":
                                            case "U1":
                                            case "U2":
                                            case "U4":
                                            case "U8":
                                                {
                                                    SECS_BASE SECSData = new SECS_I4(data.RetData.ToInt());
                                                    SECSEngine.EncodeDataItem(ref bData, SECSData);
                                                }
                                                break;
                                        }
                                    }

                                    //將Byte List用SECS_BINARY裝起來，再整個編碼成PPBODY
                                    SECS_BINARY ppbody = new SECS_BINARY(bData.ToArray());
                                    SECSEngine.EncodeDataItem(ref sb, ppbody);
                                    SECSEngine.SendHSMSMessage(7, 6, false, sb.ToArray());
                                }
                                else
                                {
                                    sb.Clear();
                                    SECS_LIST lstData = new SECS_LIST(0);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    SECSEngine.SendHSMSMessage(7, 6, false, sb.ToArray());
                                }
                            }
                        }
                    }
                    break;
                case 6:
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //PPID
                            SECS_BINARY bData = new SECS_BINARY();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref bData); //PPBODY

                            int ppAddress = 0;
                            byte[] ppArray = new byte[bData.Items];
                            for (int i = 0; i < ppArray.Length; ++i)
                            {
                                ppArray[i] = bData[i];
                            }

                            List<PKGItem> pkgItemList = new List<PKGItem>();
                            pkgItemList.Clear();

                            ExchangeData data = new ExchangeData();
                            for (int i = 0; i < PKGSchema.Count; ++i)
                            {
                                SECS_ASCII Name = new SECS_ASCII();
                                ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref Name);

                                PKGItem item = PKGSchema[i];
                                switch (item.Format)
                                {
                                    case "A":
                                        {
                                            SECS_ASCII itemData = new SECS_ASCII();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "BOOL":
                                        {
                                            SECS_BOOLEAN itemData = new SECS_BOOLEAN();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "F4":
                                    case "F8":
                                        {
                                            SECS_F8 itemData = new SECS_F8();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "I1":
                                    case "I2":
                                    case "I4":
                                    case "I8":
                                    case "U1":
                                    case "U2":
                                    case "U4":
                                    case "U8":
                                        {
                                            SECS_I4 itemData = new SECS_I4();
                                            ppAddress = SECSEngine.DecodeDataItem(ppArray, ppAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                }
                            }

                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("RCVPPID", ascData.Data);
                            data.ParamMap.SetParameter("PKGItemData", pkgItemList);
                            data = OnDataExchangeEvent(EventType.WRITE, data);
                            int iRet = data.RetData.ToInt();
                        }
                    }
                    break;
                case 17://S7F17 Delete Process Program Send
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            bool ret = true;
                            if (listData.Data == 0)
                            {
                                ExchangeData data = new ExchangeData();
                                data.Code = TableCode.REAL;
                                data.DataName = "PPACTION";
                                data.ParamMap.SetParameter("DELPPID", "ALL");
                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                ret = data.RetData.ToBoolean();
                            }
                            else
                            {

                                for (int i = 0; i < listData.Data; ++i)
                                {
                                    SECS_ASCII ppidName = new SECS_ASCII();
                                    lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ppidName);
                                    ExchangeData data = new ExchangeData();
                                    data.Code = TableCode.REAL;
                                    data.DataName = "PPACTION";
                                    data.ParamMap.SetParameter("DELPPID", ppidName.Data);
                                    data = OnDataExchangeEvent(EventType.QUERY, data);
                                    ret |= data.RetData.ToBoolean();
                                }
                            }


                            sb.Clear();
                            if (ret)
                            {
                                SECS_BINARY bData = new SECS_BINARY(0x00);
                                SECSEngine.EncodeDataItem(ref sb, bData);
                                SECSEngine.SendHSMSMessage(7, 18, false, sb.ToArray());
                            }
                            else
                            {
                                SECS_BINARY bData = new SECS_BINARY(0x01);
                                SECSEngine.EncodeDataItem(ref sb, bData);
                                SECSEngine.SendHSMSMessage(7, 18, false, sb.ToArray());
                            }

                        }
                    }
                    break;
                case 19: //S7F19 Current EPPD Request
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            ExchangeData data = new ExchangeData();
                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("LISTPPID", "");
                            data = OnDataExchangeEvent(EventType.QUERY, data);
                            String strPPID = data.RetData.ToString();
                            String[] PPIDArray = strPPID.Split(',');
                            int count = PPIDArray.Count();
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST(count);
                            SECSEngine.EncodeDataItem(ref sb, listData);
                            for (int i = 0; i < count; ++i)
                            {
                                SECS_ASCII ascData = new SECS_ASCII(PPIDArray[i]);
                                SECSEngine.EncodeDataItem(ref sb, ascData);
                            }
                            SECSEngine.SendHSMSMessage(7, 20, false, sb.ToArray());
                        }
                    }
                    break;
                case 23: //Fortmated Process Program Send
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //PPID
                            ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //MDLN
                            ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //SOFTREV
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Number of Process Commands --> 1
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Always 2
                            SECS_U2 CCODE = new SECS_U2(); //Command Code
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CCODE);
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Parameter Count

                            List<PKGItem> pkgItemList = new List<PKGItem>();
                            pkgItemList.Clear();

                            ExchangeData data = new ExchangeData();
                            for (int i = 0; i < listData.Data; ++i)
                            {
                                SECS_ASCII Name = new SECS_ASCII();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref Name);

                                PKGItem item = PKGSchema[i];
                                switch (item.Format)
                                {
                                    case "A":
                                        {
                                            SECS_ASCII itemData = new SECS_ASCII();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "BOOL":
                                        {
                                            SECS_BOOLEAN itemData = new SECS_BOOLEAN();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "F4":
                                    case "F8":
                                        {
                                            SECS_F8 itemData = new SECS_F8();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "I1":
                                    case "I2":
                                    case "I4":
                                    case "I8":
                                    case "U1":
                                    case "U2":
                                    case "U4":
                                    case "U8":
                                        {
                                            SECS_I4 itemData = new SECS_I4();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                }
                            }

                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("RCVPPID", ascData.Data);
                            data.ParamMap.SetParameter("PKGItemData", pkgItemList);
                            data = OnDataExchangeEvent(EventType.WRITE, data);
                            int iRet = data.RetData.ToInt();

                            sb.Clear();
                            SECS_BINARY bData = new SECS_BINARY(0);
                            SECSEngine.EncodeDataItem(ref sb, bData);
                            SECSEngine.SendHSMSMessage(7, 24, false, sb.ToArray());
                        }
                    }
                    break;
                case 25: //Fortmated Process Program Request
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData);
                            String PPID = ascData.Data;

                            ExchangeData data = new ExchangeData();
                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("EXISTPPID", ascData.Data);
                            data = OnDataExchangeEvent(EventType.QUERY, data);
                            int iRet = data.RetData.ToInt();
                            if (iRet == 0) //Not Existed
                            {
                                sb.Clear();
                                //A zero-length list indicates the request was denied.
                                SECS_LIST lstData = new SECS_LIST(0);
                                SECSEngine.EncodeDataItem(ref sb, lstData);
                                SECSEngine.SendHSMSMessage(7, 26, false, sb.ToArray());
                            }
                            else
                            {
                                //Load selected package to PreloadPackageDS
                                data = new ExchangeData();
                                data.Code = TableCode.REAL;
                                data.DataName = "PPACTION";
                                data.ParamMap.SetParameter("SELPPID", ascData.Data);
                                data = OnDataExchangeEvent(EventType.QUERY, data);
                                iRet = data.RetData.ToInt();
                                if (iRet == 0)
                                {
                                    /*
                                    L,4
                                      <PPID>
                                      <MDLN>
                                      <SOFTREV>
                                      L,1 (c = Number of Process Commands)
                                        L,2
                                          <CCODE>
                                          L,p (p = Number of Parameters)
                                            <PPARM1>
                                            .
                                            .
                                            p. <PPARMp>
                                    */
                                    sb.Clear();
                                    SECS_LIST lstData = new SECS_LIST(4);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    ascData = new SECS_ASCII(PPID);
                                    SECSEngine.EncodeDataItem(ref sb, ascData);
                                    SECS_BASE SECSData = QueryValueEventHandler("MDLN", "A", TableCode.REAL);
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    SECSData = QueryValueEventHandler("SOFTREV", "A", TableCode.REAL);
                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                    lstData = new SECS_LIST(1); //Number of process commands
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    lstData = new SECS_LIST(2);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    SECS_U2 CCODE = new SECS_U2(1); //Command Code
                                    SECSEngine.EncodeDataItem(ref sb, CCODE);
                                    lstData = new SECS_LIST(PKGSchema.Count); //Number of parameters
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    for (int i = 0; i < PKGSchema.Count; ++i)
                                    {
                                        PKGItem Item = PKGSchema[i];
                                        data = new ExchangeData();
                                        data.Code = TableCode.PRELOADPACKAGE;
                                        data.DataName = Item.QueryName;
                                        data = OnDataExchangeEvent(EventType.QUERY, data);
                                        switch (Item.Format)
                                        {
                                            case "A":
                                                {
                                                    SECSData = new SECS_ASCII(data.RetData.ToString());
                                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                }
                                                break;
                                            case "BOOL":
                                                {
                                                    SECSData = new SECS_BOOLEAN(data.RetData.ToBoolean());
                                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                }
                                                break;
                                            case "F4":
                                            case "F8":
                                                {
                                                    SECSData = new SECS_F8(data.RetData.ToDouble());
                                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                }
                                                break;
                                            case "I1":
                                            case "I2":
                                            case "I4":
                                            case "I8":
                                            case "U1":
                                            case "U2":
                                            case "U4":
                                            case "U8":
                                                {
                                                    SECSData = new SECS_I4(data.RetData.ToInt());
                                                    SECSEngine.EncodeDataItem(ref sb, SECSData);
                                                }
                                                break;
                                        }
                                    }
                                    SECSEngine.SendHSMSMessage(7, 26, false, sb.ToArray());
                                }
                                else
                                {
                                    sb.Clear();
                                    //A zero-length list indicates the request was denied.
                                    SECS_LIST lstData = new SECS_LIST(0);
                                    SECSEngine.EncodeDataItem(ref sb, lstData);
                                    SECSEngine.SendHSMSMessage(7, 26, false, sb.ToArray());
                                }
                            }
                        }
                    }
                    break;
                case 26: //Formatted Process Program Data
                    {
                        if (OnDataExchangeEvent != null)
                        {
                            int lAddress = 0;
                            List<byte> sb = new List<byte>();
                            SECS_LIST listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                            SECS_ASCII ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //PPID
                            ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //MDLN
                            ascData = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref ascData); //SOFTREV
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Number of Process Commands --> 1
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Always 2
                            SECS_U2 CCODE = new SECS_U2(); //Command Code
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref CCODE);
                            listData = new SECS_LIST();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData); //Parameter Count

                            List<PKGItem> pkgItemList = new List<PKGItem>();
                            pkgItemList.Clear();

                            ExchangeData data = new ExchangeData();
                            for (int i = 0; i < listData.Data; ++i)
                            {
                                SECS_ASCII Name = new SECS_ASCII();
                                lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref Name);

                                PKGItem item = PKGSchema[i];
                                switch (item.Format)
                                {
                                    case "A":
                                        {
                                            SECS_ASCII itemData = new SECS_ASCII();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "BOOL":
                                        {
                                            SECS_BOOLEAN itemData = new SECS_BOOLEAN();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "F4":
                                    case "F8":
                                        {
                                            SECS_F8 itemData = new SECS_F8();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                    case "I1":
                                    case "I2":
                                    case "I4":
                                    case "I8":
                                    case "U1":
                                    case "U2":
                                    case "U4":
                                    case "U8":
                                        {
                                            SECS_I4 itemData = new SECS_I4();
                                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref itemData);
                                            item.Data = itemData;
                                            pkgItemList.Add(item);
                                        }
                                        break;
                                }
                            }

                            data.Code = TableCode.REAL;
                            data.DataName = "PPACTION";
                            data.ParamMap.SetParameter("RCVPPID", ascData.Data);
                            data.ParamMap.SetParameter("PKGItemData", pkgItemList);
                            data = OnDataExchangeEvent(EventType.WRITE, data);
                            int iRet = data.RetData.ToInt();
                        }
                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 10
        private void ProcessSType_10(ProVTool.EventData pEventData)
        {
            switch (pEventData.F)
            {
                case 3: //S10F3 Terminal Display, Single
                    {
                        SECS_LIST listData = new SECS_LIST();
                        int lAddress = 0;
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                        SECS_BINARY TID = new SECS_BINARY();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref TID);
                        SECS_ASCII TEXT = new SECS_ASCII();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref TEXT);
                        TerminalServiceDisplay(TEXT.Data, false);
                    }
                    break;
                case 5: //S10F5 Terminal Display, Multi
                    {
                        SECS_LIST listData = new SECS_LIST();
                        int lAddress = 0;
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                        SECS_BINARY TID = new SECS_BINARY();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref TID);
                        listData = new SECS_LIST();
                        lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref listData);
                        string strDisp = String.Empty;
                        for (int i = 0; i < listData.Data; ++i)
                        {
                            SECS_ASCII TEXT = new SECS_ASCII();
                            lAddress = SECSEngine.DecodeDataItem(pEventData.RawData, lAddress, ref TEXT);

                            strDisp += TEXT.Data;
                            strDisp += " ";
                        }
                        TerminalServiceDisplay(strDisp, true);
                    }
                    break;
                #region S9,F5 Unrecognized Function Type
                default:
                    {
                        if (pEventData.W_Bit)
                        {
                            List<byte> sb = new List<byte>();
                            SECSEngine.SendHSMSMessage(9, 5, false, sb.ToArray());
                        }
                    }
                    break;
                #endregion
            }
        }
        #endregion

        #region Handler For Stream Type 14
        private void ProcessSType_14(ProVTool.EventData pEventData)
        {
            switch (pEventData.F)
            {
                case 1://S14F1 GetAttr Request
                    {
                    }
                    break;
                case 2://S14F2 GetAttr Data
                    {
                    }
                    break;
            }
        }
        #endregion

        void LimitFSM_TransitionCompleted(object sender, Sanford.StateMachineToolkit.TransitionCompletedEventArgs<ProVGEMLimitFSM.StateID, ProVGEMLimitFSM.EventID, EventArgs> e)
        {
            CurExceedLimitItem = sender as ProVGEMLimitFSM.ProVLimitFSM;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            switch (e.TargetStateID)
            {
                case ProVGEMLimitFSM.StateID.DISABLED:
                    {
                        CurExceedLimitItem.StopMonitor();
                    }
                    break;
                case ProVGEMLimitFSM.StateID.ENABLED:
                    {
                    }
                    break;
                case ProVGEMLimitFSM.StateID.NOZONE:
                    {
                        CurExceedLimitItem.StartMonitor();
                    }
                    break;
                case ProVGEMLimitFSM.StateID.ABOVELIMIT:
                    {
                        // Clock, LimitVariable, EventLimit, Transition Type
                        EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(15)); //Package State Change Report
                        CurEventName = report.EventName;
                        if (report != null)
                        {
                            List<byte> sb = report.EncodeEventReport();
                            SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                        }
                    }
                    break;
                case ProVGEMLimitFSM.StateID.BELOWLIMIT:
                    {
                        // Clock, LimitVariable, EventLimit, Transition Type
                        EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(15)); //Package State Change Report
                        CurEventName = report.EventName;
                        if (report != null)
                        {
                            List<byte> sb = report.EncodeEventReport();
                            SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Limit Monitor Event Handler For Query Current VID Value
        /// </summary>
        /// <param name="VID">欲查詢的VID</param>
        /// <returns>查詢結果以SECS_BASE資料型態封裝</returns>
        SECS_BASE OnQueryDataValue(ushort VID)
        {
            SECS_BASE data = null;
            VIDData vidData = GetVID(VID);
            if (vidData != null)
            {
                data = QueryValueEventHandler(vidData.Name, vidData.Format, vidData.Code);
            }
            return data;
        }

        SECS_BASE tdc_QuerySVEvent(object sender, ushort SVID)
        {
            SECS_BASE data = null;
            if (SVID != 0) //Collect SV Data
            {
                SVData svData = SVList.Find(x => x.SVID == (ushort)SVID);
                if (svData != null)
                {
                    data = QueryValueEventHandler(svData.Name, svData.Format, svData.Code);
                }
            }
            else //S6F1 Trace Data Report 
            {
                TraceDataCollection tdc = sender as TraceDataCollection;
                
                List<byte> sb = new List<byte>();
                SECSEngine.EncodeDataItem(ref sb, new SECS_LIST(4));
                SECSEngine.EncodeDataItem(ref sb, new SECS_U2(tdc.TRID));
                SECSEngine.EncodeDataItem(ref sb, new SECS_U2(tdc.SMPLN));
                DateTime dtNow = DateTime.Now;
                String strTime = dtNow.ToString("yyMMddhhmmss");
                SECSEngine.EncodeDataItem(ref sb, new SECS_ASCII(strTime));
                SECSEngine.EncodeDataItem(ref sb, new SECS_LIST(tdc.REPGSZ * tdc.SVDataList.Count));
                foreach (SECS_BASE sv in tdc.TraceReportList)
                {
                    SECSEngine.EncodeDataItem(ref sb, sv);
                }
                SECSEngine.SendHSMSMessage(6, 1, true, sb.ToArray());
            }
            return data;
        }

        private void rdoDisable_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = sender as RadioButton;
            if (rdo != null)
            {
                switch ((String)rdo.Tag)
                {
                    case "0":
                        {
                            HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_DISABLE);
                            HostCommFSM.Execute();
                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_DISABLE);
                            EQPCommFSM.Execute();
                        }
                        break;
                    case "1":
                        {
                            HostCommFSM.Send(ProVGEMCommFSM.EventID.EVT_ENABLE);
                            HostCommFSM.Execute();
                            EQPCommFSM.Send(ProVGEMCommFSM.EventID.EVT_ENABLE);
                            EQPCommFSM.Execute();
                        }
                        break;
                }
            }
        }

        private void ControlMode_OnClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                switch ((String)btn.Tag)
                {
                    case "0":
                        {
                            if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                            {
                                if (CurCtrlState != ProVGEMControlFSM.StateID.EQPOFFLINE)
                                {
                                    CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_TURNOFFLINE);
                                    CtrlFSM.Execute();
                                }
                            }
                        }
                        break;
                    case "1":
                        {
                            if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                            {
                                CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_TURNONLINE);
                                CtrlFSM.Execute();
                            }
                        }
                        break;
                    case "2":
                        {
                            if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                            {
                                CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_TOLOCAL);
                                CtrlFSM.Execute();
                            }
                        }
                        break;
                    case "3":
                        {
                            if (CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING)
                            {
                                CtrlFSM.Send(ProVGEMControlFSM.EventID.EVT_TOREMOTE);
                                CtrlFSM.Execute();
                            }
                        }
                        break;
                }
            }
        }

        #region S10Fn Terminal Service
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            if (CurCommState != ProVGEMCommFSM.StateID.COMMUNICATING)
                return;
            if (dgvSND.Rows.Count <= 0)
                return;

            DataGridViewRow row = dgvSND.Rows[0];
            
            if (row.Cells[0].Value != null)
            {
                String strContent = row.Cells[0].Value.ToString();
                if (String.IsNullOrEmpty(strContent))
                    return;

                List<byte> sb = new List<byte>();
                SECS_LIST listData = new SECS_LIST(2);
                SECSEngine.EncodeDataItem(ref sb, listData);
                SECS_BINARY TID = new SECS_BINARY(0);
                SECSEngine.EncodeDataItem(ref sb, TID);
                SECS_ASCII TEXT = new SECS_ASCII(strContent);
                SECSEngine.EncodeDataItem(ref sb, TEXT);
                SECSEngine.SendHSMSMessage(10, 1, true, sb.ToArray());
            }
            if(dgvSND.Rows.Count > 1)
                dgvSND.Rows.RemoveAt(0);
        }

        private void btnClearSND_Click(object sender, EventArgs e)
        {
            dgvSND.Rows.Clear();
        }

        private void TerminalServiceDisplay(String strContent, bool bIsMultiMsg = false)
        {
            char[] strSplit = new char[] { '\b' };
            String[] strMsg = strContent.Split(strSplit, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strMsg.Count(); ++i)
            {
                DataGridViewRowCollection rows = dgvRCV.Rows;
                String DTStr = DateTime.Now.ToString("yyyy/MM/dd ") + DateTime.Now.ToString("HH:mm:ss");
                rows.Add(new object[] { DTStr, strMsg[i] });
            }

            List<byte> sb = new List<byte>();
            SECS_BINARY ACK10 = new SECS_BINARY(0);
            SECSEngine.EncodeDataItem(ref sb, ACK10);
            if (bIsMultiMsg)
            {
                SECSEngine.SendHSMSMessage(10, 6, false, sb.ToArray());
            }
            else
            {
                SECSEngine.SendHSMSMessage(10, 4, false, sb.ToArray());
            }
        }

        private void btnRecognized_Click(object sender, EventArgs e)
        {
            if ((CurCommState == ProVGEMCommFSM.StateID.COMMUNICATING) &&
                (ControlState == ProVGEMControlFSM.StateID.ONLINELOCAL ||
                ControlState == ProVGEMControlFSM.StateID.ONLINEREMOTE)
                )
            {
                if (dgvRCV.Rows.Count > 0)
                {
                    //Message Recognized Event Report
                    EventReport report = EVTReportList.Find(x => x.CEID == (ushort)(10));
                    CurEventName = report.EventName;
                    if (report != null)
                    {
                        List<byte> sb = report.EncodeEventReport();
                        SECSEngine.SendHSMSMessage(6, 11, true, sb.ToArray());
                    }

                    dgvRCV.Rows.RemoveAt(0);
                }
            }
        }
        #endregion

        #region Save ProVSECS.xls with Dynamic Event Report Configuration
        private void SaveProVSECSXLS()
        {
            try
            {
                String SECSConfigFile = "ProVSECS.xls";

                if (File.Exists(SECSConfigFile))
                {
                    using (FileStream fs = new FileStream(SECSConfigFile, FileMode.Open, FileAccess.ReadWrite))
                    {
                        HSSFWorkbook workbook = new HSSFWorkbook(fs);
                        //CEID Table
                        HSSFSheet sheetCEID = (HSSFSheet)workbook.GetSheet("CEID");
                        //RPTID Table
                        HSSFSheet sheetRPTID = (HSSFSheet)workbook.GetSheet("RPTID");

                        for (int j = 0; j <= sheetCEID.LastRowNum; j++)
                        {
                            HSSFRow row = (HSSFRow)sheetCEID.GetRow(j);
                            if ((row.GetCell(0) != null) && (row.GetCell(0).ToString() == "CEID")) //Header Row
                                continue;

                            ushort CEID = Convert.ToUInt16(row.GetCell(0).ToString()); //取得CEID

                            EventReport report = EVTReportList.Find(x => x.CEID == CEID); //取得此CEID對應的Event Report

                            //將此Event Report的ReportList串成Report Data以便寫回Excel
                            String strRPTData = String.Empty;
                            foreach (RPTData rpt in report.GetRPTList())
                            {
                                strRPTData += rpt.RPTID.ToString();
                                strRPTData += ";";
                            }

                            if (strRPTData.EndsWith(";")) //將結尾的分號去掉
                                strRPTData = strRPTData.Remove(strRPTData.Length - 1);

                            SetCellValue(row.GetCell(3), strRPTData);

                            if (report != null)
                            {
                                List<RPTData> rptList = report.GetRPTList();
                                for (int i = 0; i <= sheetRPTID.LastRowNum; i++)
                                {
                                    HSSFRow rowRPT = (HSSFRow)sheetRPTID.GetRow(i);
                                    if ((rowRPT.GetCell(0) != null) && (rowRPT.GetCell(0).ToString() == "RPTID")) //Header Row
                                        continue;

                                    ushort RPTID = Convert.ToUInt16(rowRPT.GetCell(0).ToString()); //取得RPTID
                                    RPTData rptData = rptList.Find(x => x.RPTID == RPTID);//取得此RPTID隊的Report Data
                                    if (rptData != null)
                                    {
                                        //將此Report Data的VIDList串成VID Data以便寫回Excel
                                        String strVIDData = String.Empty;
                                        foreach (VIDData vid in rptData.GetVIDList())
                                        {
                                            strVIDData += vid.VID.ToString();
                                            strVIDData += ";";
                                        }

                                        if (strVIDData.EndsWith(";")) //將結尾的分號去掉
                                            strVIDData = strVIDData.Remove(strVIDData.Length - 1);

                                        SetCellValue(rowRPT.GetCell(3), strVIDData);
                                    }
                                }
                            }
                        }

                        FileStream fsw = File.OpenWrite(SECSConfigFile);
                        workbook.Write(fsw);
                        fsw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SetCellValue(ICell cell, object obj)
        {
            if (obj.GetType() == typeof(int))
            {
                cell.SetCellValue((int)obj);
            }
            else if (obj.GetType() == typeof(double))
            {
                cell.SetCellValue((double)obj);
            }
            else if (obj.GetType() == typeof(IRichTextString))
            {
                cell.SetCellValue((IRichTextString)obj);
            }
            else if (obj.GetType() == typeof(string))
            {
                cell.SetCellValue(obj.ToString());
            }
            else if (obj.GetType() == typeof(DateTime))
            {
                cell.SetCellValue((DateTime)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                cell.SetCellValue((bool)obj);
            }
            else
            {
                cell.SetCellValue(obj.ToString());
            }
        }
        #endregion
    }

    public class ExchangeData
    {
        public string DataName;
        public string Format;
        public TableCode Code;
        public HOSTCMD HostCMD;
        public ProVLib.ProVMessage ParamMap;
        public KCSDK.DataTransfer RetData;
        public ExchangeData()
        {
            ParamMap = new ProVLib.ProVMessage();
        }
    }
}
