using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using ProVLib;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using KCSDK;

namespace ADSW11000
{
    public class LightUIObj
    {
        public OutBit GreenLight;
        public OutBit YellowLight;
        public OutBit RedLight;
        public OutBit Music1;
        public OutBit Music2;
        public OutBit Music3;
        public OutBit Music4;
        public LightUIObj
            (
            OutBit Green,
            OutBit Yellow,
            OutBit Red,
            OutBit M1,
            OutBit M2,
            OutBit M3,
            OutBit M4
            )
        {
            GreenLight = Green;
            YellowLight = Yellow;
            RedLight = Red;
            Music1 = M1;
            Music2 = M2;
            Music3 = M3;
            Music4 = M4;
        }
        public LightUIObj(List<object> mLightIO)
        {
            GreenLight = (OutBit)mLightIO[0];
            YellowLight = (OutBit)mLightIO[1];
            RedLight = (OutBit)mLightIO[2];
            Music1 = (OutBit)mLightIO[3];
            Music2 = (OutBit)mLightIO[4];
            Music3 = (OutBit)mLightIO[5];
            Music4 = (OutBit)mLightIO[6];
        }
        public LightUIObj()
        {
            GreenLight = null;
            YellowLight = null;
            RedLight = null;
            Music1 = null;
            Music2 = null;
            Music3 = null;
            Music4 = null;
        }
    }

    public struct ArmLan
    {
        public String Explain;          //錯誤說明
        public String Cause;            //原因
        public String Solution;         //解決方式

        public ArmLan(String Explain, String Cause, String Solution)
        {
            this.Explain = String.Empty;
            this.Cause = String.Empty;
            this.Solution = String.Empty;
        }
    }

    //燈號設定
    public struct LightSet
    {
        public bool Green;              //綠燈是否亮
        public bool GreenBlink;         //綠燈是否閃爍
        public bool Yellow;             //黃燈是否亮
        public bool YellowBlink;        //黃燈是否閃爍
        public bool Red;                //紅燈是否亮
        public bool RedBlink;           //紅燈是否閃爍
        public byte MusicNo;            //音樂編碼
    }

    //依狀態設定燈號結構
    public class StatusLightSetDT
    {
        public string State;
        public LightSet mLightSet;
    }

    //代表一個Alarm訊息的結構
    public class ArmMtrl : IEquatable<ArmMtrl>
    {
        public String AlarmLevel;       //錯誤類型
        public String Module;           //模組
        public int ErrorCode;           //錯誤碼
        public String ImagePath;        //圖片路徑
        public ArmLan LanENG;           //英文Alarm
        public ArmLan LanCHT;           //中文Alarm
        public String Explain;          //要顯示的文字
        public int ModuleID;            //模組編號
        public double OccurTM;          //發生時間
        public double OccurTMTemp;      //發生時間(DateTime格式)
        public String DT;               //發生日期與時間
        public LightSet mLightSet;      //燈號控制資料
        public bool MTBF;               //是否記入MTBF統計
        public bool MTBA;               //是否記入MTBA統計
        //By Max 20211025, v4.0.94.0
        public bool SplitStatistic;          //是否以Alarm內容做統計

        public ArmMtrl()
        {
            AlarmLevel = string.Empty;
            ErrorCode = 0;
            ModuleID = 0;
            ImagePath = string.Empty;
            LanENG = new ArmLan();
            LanCHT = new ArmLan();
        }

        public bool Equals(ArmMtrl other)
        {
            //By Max 20211025, v4.0.94.0
            //this.LanENG.Explain -> this.Explain 
            if (this.Explain == other.Explain)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //+ by Max 20150122 For AfterShowAlarm
    public delegate void AfterShowAlarmDelegate(ArmMtrl armMtrl, bool bSet);


    public class AlarmSystem
    {
        private bool StopAlarmProcessor;
        private Thread AlarmProcessor;
        private object lockObj = new object();
        public Light m_Light = null; //燈號控制
        public Type UIType = null; //顯示元件型別
        private Control m_UIObj; //外部的顯示元件
        private System.Windows.Forms.Timer m_ResponseTmr = null;
        ArmMtrl pArmTemp = null;

        private List<ArmMtrl> ArmTable; //Alarm Table
        public List<ArmMtrl> ArmUIList; //由Timer從List中取出
        private Queue<ArmMtrl> ArmList; //存放由Say函式來的Alarm
        private List<StatusLightSetDT> StatusLightList; //狀態燈號設定列表
        private bool bRefresh; //UI 更新
        private bool bClearOk; 

        private GetTickCountEx tick = new GetTickCountEx();

        private string Language; //語系
        private AfterShowAlarmDelegate armDelegate = null;

        //+ By Max 20210108
        public static DateTime ErrStopDT = DateTime.Now;   //機台錯誤停機時間

        //By Max 20211025, v4.0.94.0
        private List<ArmMtrl> ArmW2WList;  
        private bool bNeedToWrite = false; 


        //建構
        public AlarmSystem()
        {
            ArmTable = new List<ArmMtrl>();
            ArmUIList = new List<ArmMtrl>();
            ArmList = new Queue<ArmMtrl>();
            //By Max 20211025, v4.0.94.0
            ArmW2WList = new List<ArmMtrl>();

            StatusLightList = new List<StatusLightSetDT>();
            Language = "tw";

            StopAlarmProcessor = false;
            AlarmProcessor = new Thread(Execute);
            AlarmProcessor.IsBackground = true;
            AlarmProcessor.Start();

            m_ResponseTmr = new System.Windows.Forms.Timer();
            m_ResponseTmr.Tick += ResponseScan;
            m_ResponseTmr.Interval = 1;
            m_ResponseTmr.Enabled = true;

            bRefresh = false;
            bClearOk = true;
        }

        //+ By Max For SECS
        public List<ArmMtrl> AlarmTable
        {
            get
            {
                return ArmTable;
            }
        }

        #region 私有函數

        //清除UI內容
        private void UIClear()
        {
            switch (UIType.Name)
            {
                case "ListView":
                    ((ListView)m_UIObj).BeginUpdate();
                    ((ListView)m_UIObj).Items.Clear();
                    ((ListView)m_UIObj).EndUpdate();
                    break;
                case "Label":
                    ((Label)m_UIObj).Text = "";
                    break;
                case "TextBox":
                    ((TextBox)m_UIObj).Text = "";
                    break;
            }
        }

        //增加 UI 內容
        private void UIAddItem(ListViewItem pItem)
        {
            string s = "";
            switch (UIType.Name)
            {
                case "ListView":
                    ((ListView)m_UIObj).BeginUpdate();
                    ((ListView)m_UIObj).Items.Insert(0, pItem);
                    ((ListView)m_UIObj).EndUpdate();
                    break;
                case "Label":
                    for (int i = 0; i < pItem.SubItems.Count; i++)
                        s = s + pItem.SubItems[i] + " ";
                    s = s.Substring(1, s.Length - 1);
                    ((Label)m_UIObj).Text = s;
                    break;
                case "TextBox":
                    for (int i = 0; i < pItem.SubItems.Count; i++)
                        s = s + pItem.SubItems[i] + " ";
                    s = s.Substring(1, s.Length - 1);
                    ((TextBox)m_UIObj).Text = s;
                    break;
            }
        }

        private delegate void ClearUIDelegate(); //m_UIObj Clear Invoke 處理
        private delegate void AddItemUIDelegate(ListViewItem pItem); //m_UIObj Add Item Invoke 處理

        private void AlarmStop(string type)
        {
            switch (type)
            {
                case "E":
                case "e":
                case "w":
                case "i":
                    if (!SYSPara.ErrorStop)
                        ErrStopDT = DateTime.Now;
                    SYSPara.ErrorStop = true;
                    SYSPara.SysRun = false;
                    FormSet.mMSS.M_Stop();
                    break;
            }
        }

        //錯誤顯示處理
        private void ResponseScan(Object myObject, EventArgs myEventArgs)
        {
            m_ResponseTmr.Enabled = false;
            ArmMtrl pArm = null;
            
            if (bRefresh)
            {
                bRefresh = false;
                ClearUIDelegate mClearUI = new ClearUIDelegate(UIClear);
                m_UIObj.BeginInvoke(mClearUI);

                lock (lockObj)
                {
                    for (int i = 0; i < ArmUIList.Count; i++)
                    {
                        pArm = ArmUIList[i];
                        ListViewItem pItem = new ListViewItem();
                        pItem.SubItems.Add(pArm.DT);
                        pItem.SubItems.Add(pArm.Module);
                        pItem.SubItems.Add(string.Format("{0:0}{1:0000}", pArm.AlarmLevel.ToUpper(), pArm.ModuleID * 100 + pArm.ErrorCode));
                        pItem.SubItems.Add(pArm.Explain);

                        switch (pArm.AlarmLevel)
                        {
                            case "E":
                            case "e":
                                pItem.SubItems[0].BackColor = Color.Red;
                                break;
                            case "W":
                                pItem.SubItems[0].BackColor = Color.Yellow;
                                break;
                            case "w":
                                pItem.SubItems[0].BackColor = Color.Yellow;
                                break;
                        }

                        //+ By Max For AlarmForm Show
                        AddItemUIDelegate mAddItemUI = new AddItemUIDelegate(UIAddItem);
                        m_UIObj.BeginInvoke(mAddItemUI, pItem);

                        //+ By Max For SECS
                        if (armDelegate != null)
                            armDelegate(pArm, true);
                    }
                }

                bClearOk = true;
            }

            #region 依狀態顯示燈號
            if (SYSPara.LightMode == 0)
            {
                if (m_Light != null)
                {
                    string StateStr = SYSPara.RunMode.ToString();
                    if (SYSPara.SysRun)
                    {
                        if (pArm != null)
                        {
                            switch (pArm.AlarmLevel)
                            {
                                case "I":
                                    pArmTemp = pArm;
                                    StateStr += "_INFO";
                                    break;
                                case "W":
                                    pArmTemp = pArm;
                                    StateStr += "_WARN";
                                    break;
                                default:
                                    StateStr += "_RUN";
                                    break;
                            }
                        }
                        else
                        {
                            if (pArmTemp != null)
                            {
                                switch (pArmTemp.AlarmLevel)
                                {
                                    case "I":
                                        StateStr += "_INFO";
                                        break;
                                    case "W":
                                        StateStr += "_WARN";
                                        break;
                                    default:
                                        StateStr += "_RUN";
                                        break;
                                }
                            }
                            else
                                StateStr += "_RUN";
                        }
                    }
                    else
                    {
                        if (SYSPara.ErrorStop)
                            StateStr += "_STOP";
                        else
                        {
                            if (pArm != null)
                            {
                                switch (pArm.AlarmLevel)
                                {
                                    case "I":
                                        pArmTemp = pArm;
                                        StateStr += "_INFO";
                                        break;
                                    case "W":
                                        pArmTemp = pArm;
                                        StateStr += "_WARN";
                                        break;
                                    default:
                                        StateStr += "_PAUSE";
                                        break;
                                }
                            }
                            else
                            {
                                if (pArmTemp != null)
                                {
                                    switch (pArmTemp.AlarmLevel)
                                    {
                                        case "I":
                                            StateStr += "_INFO";
                                            break;
                                        case "W":
                                            StateStr += "_WARN";
                                            break;
                                        default:
                                            StateStr += "_RUN";
                                            break;
                                    }
                                }
                                else
                                    StateStr += "_PAUSE";
                            }
                        }
                    }
                    m_Light.SetLightByStatus(StatusLightList.Find(x => x.State == StateStr));
                }
            }
            #endregion

            if (m_Light != null)
                m_Light.ShowLight();

            m_ResponseTmr.Enabled = true;
        }

        //處理目前發生的Alarm
        private void Execute()
        {
            while (!StopAlarmProcessor)
            {
                if (SYSPara.HMIReady)
                {
                    lock (lockObj)
                    {
                        if (ArmList.Count > 0)
                        {
                            ArmMtrl almObjTemp = ArmList.Dequeue();
                            if (almObjTemp.Module == "ClearAll")
                            {
                                //依Alarm狀態顯示燈號
                                if (SYSPara.LightMode == 1)
                                {
                                    if (m_Light!=null)
                                        m_Light.SetLightByAlarm(null);
                                }

                                pArmTemp = null;
                                ArmUIList.Clear();
                                ArmList.Clear();

                                if (m_Light != null)
                                    m_Light.MusicOff();

                                bRefresh = true;
                            }
                            else
                            {
                                if (!GetRepeatInShowList(almObjTemp))
                                {
                                    ArmUIList.Add(almObjTemp);

                                    //依Alarm狀態顯示燈號
                                    if (SYSPara.LightMode == 1)
                                    {
                                        if (m_Light != null)
                                            m_Light.SetLightByAlarm(almObjTemp);
                                    }

                                    bRefresh = true;
                                }
                            }
                        }
                        //By Max 20211025, v4.0.94.0
                        if (!SYSPara.SysRun)
                        {
                            sw.Restart();
                        }
                        //需要寫檔案（呼叫ClearAll後）且系統運行超過5秒
                        if (bNeedToWrite && SYSPara.SysRun && sw.ElapsedMilliseconds > 5000)
                        {
                            sw.Restart();
                            WriteArmToFile();
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        //By Max 20211025, v4.0.94.0
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private bool WriteArmToFile()
        {
            bNeedToWrite = false;
            //計算Alarm的持續時間並記錄到Log
            foreach (ArmMtrl pArm in ArmW2WList)
            {
                double ContinueTM = tick.Value - pArm.OccurTM;
                DateTime OccurTM = DateTime.FromOADate(pArm.OccurTMTemp);
                string sMTBA = (!pArm.MTBA && !pArm.MTBF) ? "N" : ((pArm.MTBA && pArm.MTBF) ? "A" : (!pArm.MTBA && pArm.MTBF) ? "F" : "A");
                string sSplitStatistic = (pArm.SplitStatistic) ? "Y" : "N";
                string s = string.Format("{0}\t{1}{2:0000}\t{3}\t{4}\t{5:0}\t{6}", pArm.Explain, pArm.AlarmLevel.ToUpper()
                    , pArm.ModuleID * 100 + pArm.ErrorCode, pArm.Module, OccurTM.ToString(@"yyyy\/MM\/dd") + "\t" + OccurTM.ToString("HH:mm:ss.fff"), ContinueTM, sMTBA, sSplitStatistic);

                SYSPara.LogSay(EnLoggerType.EnLog_ARM, s);
                if (armDelegate != null)
                    armDelegate(pArm, false);
            }

            ArmW2WList.Clear();

            if (!MotorList.Motors.Any(x => x.Busy()))
            {
                TObjForm.ObjFormAlarmReset();
            }

            return true;
        }

        //Alarm是否重覆出現在Alarm Table
        private bool GetRepeatInTable(ArmMtrl pArm)
        {
            string pArmStr = string.Format("{0}{1:0000}", pArm.AlarmLevel, pArm.ModuleID * 100 + pArm.ErrorCode);
            string armstr = "";
            foreach (ArmMtrl arm in ArmTable)
            {
                armstr = string.Format("{0}{1:0000}", arm.AlarmLevel, arm.ModuleID * 100 + arm.ErrorCode);
                if ((armstr == pArmStr))
                    return true;
            }
            return false;
        }

        //取出 Alarm Table 的詳細內容
        private ArmMtrl FindAlarm(ArmMtrl pArm)
        {
            string pArmStr = string.Format("{0}{1:0000}", pArm.AlarmLevel, pArm.ModuleID * 100 + pArm.ErrorCode);
            string armstr = "";
            foreach (ArmMtrl arm in ArmTable)
            {
                armstr = string.Format("{0}{1:0000}", arm.AlarmLevel, arm.ModuleID * 100 + arm.ErrorCode);
                if (armstr == pArmStr)
                    return arm;
            }
            return null;
        }

        //Alarm是否重覆出現在Arm List
        private bool GetRepeatInList(ArmMtrl pArm)
        {
            string pArmStr = string.Format("{0}{1:0000}", pArm.AlarmLevel, pArm.ModuleID * 100 + pArm.ErrorCode);
            string armstr = "";
            foreach (ArmMtrl arm in ArmList)
            {
                armstr = string.Format("{0}{1:0000}", arm.AlarmLevel, arm.ModuleID * 100 + arm.ErrorCode);
                if (armstr == pArmStr)
                {
                    if (arm.Explain == pArm.Explain)
                        return true;
                }
            }
            return false;
        }

        //Alarm是否重覆出現在Arm Show List
        private bool GetRepeatInShowList(ArmMtrl pArm)
        {
            string pArmStr = string.Format("{0}{1:0000}", pArm.AlarmLevel, pArm.ModuleID * 100 + pArm.ErrorCode);
            string armstr = "";
            foreach (ArmMtrl arm in ArmUIList)
            {
                armstr = string.Format("{0}{1:0000}", arm.AlarmLevel, arm.ModuleID * 100 + arm.ErrorCode);
                if (armstr == pArmStr)
                {
                    if (arm.Explain == pArm.Explain)
                        return true;
                }
            }
            return false;
        }

        #endregion

        #region 使用者函數

        public void SetAfterShowAlarDelegate(AfterShowAlarmDelegate armDelegate)
        {
            this.armDelegate = armDelegate;
        }

        public void DisposeTH()
        {
            m_ResponseTmr.Stop();

            StopAlarmProcessor = true;
            AlarmProcessor.Join();
        }

        public void SetLanguage(string lang)
        {
            Language = lang;
        }

        public void SetUI(Type type, Control control,List<object> mLightIO)
        {
            UIType = type;
            m_UIObj = control;
            LightUIObj lightui;
            if (mLightIO != null)
                lightui = new LightUIObj(mLightIO);
            else
                lightui = new LightUIObj();
            m_Light = new Light(ref lightui);
        }

        private bool FieldSet(HSSFRow row, int ColIndex, ref string Field)
        {
            if (row.GetCell(ColIndex) == null)
                Field = "";
            else
            {
                switch (row.GetCell(ColIndex).CellType)
                {
                    case NPOI.SS.UserModel.CellType.String:
                        Field = row.GetCell(ColIndex).StringCellValue;
                        return true;
                    case NPOI.SS.UserModel.CellType.Numeric:
                        Field = string.Format("{0}", row.GetCell(ColIndex).NumericCellValue);
                        return true;
                    case NPOI.SS.UserModel.CellType.Blank:
                        Field = "";
                        return true;
                }
            }
            return false;
        }

        private bool FieldSet(HSSFRow row, int ColIndex, ref int Field)
        {
            if (row.GetCell(ColIndex) == null)
                Field = 0;
            else
            {
                switch (row.GetCell(ColIndex).CellType)
                {
                    case NPOI.SS.UserModel.CellType.String:
                        Field = Convert.ToInt32(row.GetCell(ColIndex).StringCellValue);
                        return true;
                    case NPOI.SS.UserModel.CellType.Numeric:
                        Field = (int)row.GetCell(ColIndex).NumericCellValue;
                        return true;
                    case NPOI.SS.UserModel.CellType.Blank:
                        Field = 0;
                        return true;
                }
            }
            return false;
        }

        private bool FieldSet(HSSFRow row, int ColIndex, ref byte Field)
        {
            if (row.GetCell(ColIndex) == null)
                Field = 0;
            else
            {
                switch (row.GetCell(ColIndex).CellType)
                {
                    case NPOI.SS.UserModel.CellType.String:
                        Field = Convert.ToByte(row.GetCell(ColIndex).StringCellValue);
                        return true;
                    case NPOI.SS.UserModel.CellType.Numeric:
                        Field = (byte)row.GetCell(ColIndex).NumericCellValue;
                        return true;
                    case NPOI.SS.UserModel.CellType.Blank:
                        Field = 0;
                        return true;
                }
            }
            return false;
        }

        private bool FieldSet(HSSFRow row, int ColIndex, ref bool Field)
        {
            if (row.GetCell(ColIndex) == null)
                Field = false;
            else
            {
                switch (row.GetCell(ColIndex).CellType)
                {
                    case NPOI.SS.UserModel.CellType.String:
                        Field = Convert.ToBoolean(row.GetCell(ColIndex).StringCellValue);
                        return true;
                    case NPOI.SS.UserModel.CellType.Numeric:
                        Field = Convert.ToBoolean(row.GetCell(ColIndex).NumericCellValue);
                        return true;
                    case NPOI.SS.UserModel.CellType.Blank:
                        Field = false;
                        return true;
                }
            }
            return false;
        }

        //讀取特定檔案至 AlarmTable
        public bool LoadFromFile(string AlarmTableName)
        {
            //Scofield_v0.5.7.46[修改]新增每次讀取AlarmTable時會修改alarm在DB內MTBF/A的統計型式
            string ChangeDBAlarmCodeType;
            string ChangeDBAlarmCodeMeanTimeType;

            if (!File.Exists(AlarmTableName))
                return false;
            try
            {
                string tempfile = @"D:\alarmtemp.xls";
                try
                {
                    File.Copy(AlarmTableName, tempfile, true);
                }
                catch
                {
                    return false;
                }

                using (FileStream fs = new FileStream(tempfile, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);

                    //取出AlarmTable頁的資料
                    ArmTable.Clear();
                    HSSFSheet sheet = (HSSFSheet)workbook.GetSheet("AlarmTable");
                    for (int j = 2; j <= sheet.LastRowNum; j++)
                    {
                        HSSFRow row = (HSSFRow)sheet.GetRow(j);

                        ArmMtrl pArm = new ArmMtrl();
                        if (!FieldSet(row, 0, ref pArm.AlarmLevel)) continue;
                        if (!FieldSet(row, 1, ref pArm.ErrorCode)) continue;
                        if (!FieldSet(row, 2, ref pArm.LanCHT.Explain)) continue;

                        FieldSet(row, 3, ref pArm.LanCHT.Cause);
                        FieldSet(row, 4, ref pArm.LanCHT.Solution);
                        FieldSet(row, 5, ref pArm.LanENG.Explain);
                        FieldSet(row, 6, ref pArm.LanENG.Cause);
                        FieldSet(row, 7, ref pArm.LanENG.Solution);
                        FieldSet(row, 8, ref pArm.mLightSet.Red);
                        FieldSet(row, 9, ref pArm.mLightSet.RedBlink);
                        FieldSet(row, 10, ref pArm.mLightSet.Yellow);
                        FieldSet(row, 11, ref pArm.mLightSet.YellowBlink);
                        FieldSet(row, 12, ref pArm.mLightSet.Green);
                        FieldSet(row, 13, ref pArm.mLightSet.GreenBlink);
                        FieldSet(row, 14, ref pArm.mLightSet.MusicNo);
                        FieldSet(row, 15, ref pArm.MTBF);
                        FieldSet(row, 16, ref pArm.MTBA);
                        FieldSet(row, 17, ref pArm.SplitStatistic);
                        ArmTable.Add(pArm);

                        //Scofield_v0.5.7.46[修改]新增每次讀取AlarmTable時會修改alarm在DB內MTBF/A的統計型式
                        if (pArm.ErrorCode <= 9)
                        {
                            ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + "0" + pArm.ModuleID.ToString() + "0" + pArm.ErrorCode.ToString();
                        }
                        else if (pArm.ErrorCode <= 99)
                        {
                            ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ModuleID.ToString() + "0" + pArm.ErrorCode.ToString();
                        }
                        else if (pArm.ErrorCode <= 999)
                        {
                            ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ModuleID.ToString() + pArm.ErrorCode.ToString();
                        }
                        else
                        {
                            ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ErrorCode.ToString();
                        }

                        if (pArm.MTBA == true)
                        {
                            ChangeDBAlarmCodeMeanTimeType = "A";
                        }
                        else if (pArm.MTBF == true)
                        {
                            ChangeDBAlarmCodeMeanTimeType = "F";
                        }
                        else
                        {
                            ChangeDBAlarmCodeMeanTimeType = "N";
                        }
                        SYSPara.logDB.ModifyAlarmMeanTimeType(ChangeDBAlarmCodeType, ChangeDBAlarmCodeMeanTimeType);
                    }

                    //取出StatusLightSetting頁的資料
                    StatusLightList.Clear();
                    sheet = (HSSFSheet)workbook.GetSheet("StatusLightSetting");
                    for (int j = 2; j <= sheet.LastRowNum; j++)
                    {
                        HSSFRow row = (HSSFRow)sheet.GetRow(j);

                        StatusLightSetDT light = new StatusLightSetDT();

                        FieldSet(row, 0, ref light.State);
                        FieldSet(row, 1, ref light.mLightSet.Red);
                        FieldSet(row, 2, ref light.mLightSet.RedBlink);
                        FieldSet(row, 3, ref light.mLightSet.Yellow);
                        FieldSet(row, 4, ref light.mLightSet.YellowBlink);
                        FieldSet(row, 5, ref light.mLightSet.Green);
                        FieldSet(row, 6, ref light.mLightSet.GreenBlink);
                        FieldSet(row, 7, ref light.mLightSet.MusicNo);

                        StatusLightList.Add(light);
                    }
                    fs.Close();
                    workbook = null;

                    File.Delete(tempfile);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //新增特定檔案至 AlarmTable
        public bool AppendFromFile(string AlarmTableName, int ModuleID)
        {
            //Scofield_v0.5.7.46[修改]新增每次讀取AlarmTable時會修改alarm在DB內MTBF/A的統計型式
            string ChangeDBAlarmCodeType;
            string ChangeDBAlarmCodeMeanTimeType;

            if (!File.Exists(AlarmTableName))
                return false;
            try
            {
                string tempfile = @"D:\alarmtemp.xls";
                try
                {
                    File.Copy(AlarmTableName, tempfile, true);
                }
                catch
                {
                    return false;
                }

                using (FileStream fs = new FileStream(tempfile, FileMode.Open, FileAccess.Read))
                {
                    HSSFWorkbook workbook = new HSSFWorkbook(fs);
                    HSSFSheet sheet = (HSSFSheet)workbook.GetSheet("AlarmTable");

                    for (int j = 2; j <= sheet.LastRowNum; j++)
                    {
                        HSSFRow row = (HSSFRow)sheet.GetRow(j);

                        ArmMtrl pArm = new ArmMtrl();
                        if (!FieldSet(row, 0, ref pArm.AlarmLevel)) continue;
                        if (!FieldSet(row, 1, ref pArm.ErrorCode)) continue;
                        if (!FieldSet(row, 2, ref pArm.LanCHT.Explain)) continue;

                        FieldSet(row, 3, ref pArm.LanCHT.Cause);
                        FieldSet(row, 4, ref pArm.LanCHT.Solution);
                        FieldSet(row, 5, ref pArm.LanENG.Explain);
                        FieldSet(row, 6, ref pArm.LanENG.Cause);
                        FieldSet(row, 7, ref pArm.LanENG.Solution);
                        FieldSet(row, 8, ref pArm.mLightSet.Red);
                        FieldSet(row, 9, ref pArm.mLightSet.RedBlink);
                        FieldSet(row, 10, ref pArm.mLightSet.Yellow);
                        FieldSet(row, 11, ref pArm.mLightSet.YellowBlink);
                        FieldSet(row, 12, ref pArm.mLightSet.Green);
                        FieldSet(row, 13, ref pArm.mLightSet.GreenBlink);
                        FieldSet(row, 14, ref pArm.mLightSet.MusicNo);
                        FieldSet(row, 15, ref pArm.MTBF);
                        FieldSet(row, 16, ref pArm.MTBA);
                        FieldSet(row, 17, ref pArm.SplitStatistic);
                        pArm.ModuleID = ModuleID;

                        //Scofield_v0.5.7.46[修改]新增每次讀取AlarmTable時會修改alarm在DB內MTBF/A的統計型式
                        if (pArm.ErrorCode <= 9)
                        {
                            switch (pArm.ModuleID)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + "0" + pArm.ModuleID.ToString() + "0" + pArm.ErrorCode.ToString();
                                    break;
                                default:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ModuleID.ToString() + "0" + pArm.ErrorCode.ToString();
                                    break;
                            }
                        }
                        else if (pArm.ErrorCode <= 99)
                        {
                            switch (pArm.ModuleID)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + "0" + pArm.ModuleID.ToString() + pArm.ErrorCode.ToString();
                                    break;
                                default:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ModuleID.ToString() + pArm.ErrorCode.ToString();
                                    break;
                            }
                        }
                        else if (pArm.ErrorCode <= 999)
                        {
                            switch (pArm.ModuleID)
                            {
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                case 9:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ModuleID.ToString() + pArm.ErrorCode.ToString();
                                    break;
                                default:
                                    ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + "0" + pArm.ErrorCode.ToString();
                                    break;
                            }
                        }
                        else
                        {
                            ChangeDBAlarmCodeType = pArm.AlarmLevel.ToString() + pArm.ErrorCode.ToString();
                        }

                        if (pArm.MTBA == true)
                        {
                            ChangeDBAlarmCodeMeanTimeType = "A";
                        }
                        else if (pArm.MTBF == true)
                        {
                            ChangeDBAlarmCodeMeanTimeType = "F";
                        }
                        else
                        {
                            ChangeDBAlarmCodeMeanTimeType = "N";
                        }

                        SYSPara.logDB.ModifyAlarmMeanTimeType(ChangeDBAlarmCodeType, ChangeDBAlarmCodeMeanTimeType);

                        if (!GetRepeatInTable(pArm))
                            ArmTable.Add(pArm);
                    }
                    fs.Close();
                    workbook = null;

                    File.Delete(tempfile);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //發出一個Alarm 
        public void Say(String ErrCode)
        {
            lock (lockObj)
            {
                try
                {
                    if (m_UIObj == null)
                        return;

                    ArmMtrl armMtrlTemp;
                    ArmMtrl aArm = new ArmMtrl();
                    if (String.Compare(ErrCode, "ClearAll") != 0)
                    {
                        aArm.AlarmLevel = ErrCode.Substring(0, 1);
                        aArm.ModuleID = 0;// Convert.ToInt16(ErrCode.Substring(1, 1));
                        aArm.ErrorCode = Convert.ToInt16(ErrCode.Substring(1, 4));

                        AlarmStop(aArm.AlarmLevel);

                        armMtrlTemp = FindAlarm(aArm);
                        if (armMtrlTemp != null)
                        {
                            if (Language == "tw")
                                aArm.Explain = armMtrlTemp.LanCHT.Explain;
                            else if (Language == "en")
                                aArm.Explain = armMtrlTemp.LanENG.Explain;
                            //尋找 Alarm List 內是否有重覆的 Alarm
                            if (!GetRepeatInList(aArm))
                            {
                                aArm.DT = DateTime.Now.ToString("G");
                                aArm.OccurTM = tick.Value;
                                aArm.OccurTMTemp = DateTime.Now.ToOADate();
                                aArm.Module = "SYS";
                                aArm.MTBA = armMtrlTemp.MTBA;
                                aArm.MTBF = armMtrlTemp.MTBF;
                                aArm.SplitStatistic = armMtrlTemp.SplitStatistic;

                                ArmList.Enqueue(aArm);
                            }
                        }
                        else
                        {
                            Say("W0500", ErrCode);
                        }
                    }
                    else
                    {
                        aArm.Module = "ClearAll";
                        ArmList.Enqueue(aArm);
                    }
                }
                catch (Exception e)
                {
                    String ErrStr = e.Message;
                }
            }//lock(obj)
        }
        public void Say(String ErrCode, Control Sender)
        {
            lock (lockObj)
            {
                try
                {
                    if (m_UIObj==null)
                        return;

                    ArmMtrl armMtrlTemp;
                    ArmMtrl aArm = new ArmMtrl();
                    if (String.Compare(ErrCode, "ClearAll") != 0)
                    {
                        aArm.AlarmLevel = ErrCode.Substring(0,1);
                        aArm.ModuleID = Convert.ToInt16(ErrCode.Substring(1, 2));
                        aArm.ErrorCode = Convert.ToInt16(ErrCode.Substring(3, 2));

                        AlarmStop(aArm.AlarmLevel);

                        armMtrlTemp = FindAlarm(aArm);
                        if (armMtrlTemp != null)
                        {
                            if (Language == "tw")
                                aArm.Explain = armMtrlTemp.LanCHT.Explain;
                            else if (Language == "en")
                                aArm.Explain = armMtrlTemp.LanENG.Explain;
                            //尋找 Alarm List 內是否有重覆的 Alarm

                            if (!GetRepeatInList(aArm))
                            {
                                aArm.DT = DateTime.Now.ToString("G");
                                aArm.OccurTM = tick.Value;
                                aArm.OccurTMTemp = DateTime.Now.ToOADate();
                                aArm.Module = Sender.Name;
                                aArm.MTBA = armMtrlTemp.MTBA;
                                aArm.MTBF = armMtrlTemp.MTBF;
                                aArm.SplitStatistic = armMtrlTemp.SplitStatistic;

                                ArmList.Enqueue(aArm);
                            }
                        }
                        else
                        {
                            Say("W0500", ErrCode);
                        }
                    }
                    else
                    {
                        aArm.Module = "ClearAll";
                        ArmList.Enqueue(aArm);
                    }
                }
                catch (Exception e)
                {
                    String ErrStr = e.Message;
                }
            }//lock(obj)
        }
        public void Say(String ErrCode, params Object[] args)
        {
            lock (lockObj)
            {
                try
                {
                    if (m_UIObj == null)
                        return;

                    ArmMtrl armMtrlTemp;
                    ArmMtrl aArm = new ArmMtrl();
                    if (String.Compare(ErrCode, "ClearAll") != 0)
                    {
                        aArm.AlarmLevel = ErrCode.Substring(0, 1);
                        aArm.ModuleID = 0;// Convert.ToInt16(ErrCode.Substring(1, 1));
                        aArm.ErrorCode = Convert.ToInt16(ErrCode.Substring(1, 4));

                        AlarmStop(aArm.AlarmLevel);

                        armMtrlTemp = FindAlarm(aArm);
                        if (armMtrlTemp != null)
                        {
                            String explain = "";
                            if (Language == "tw")
                                explain = armMtrlTemp.LanCHT.Explain;
                            else if (Language == "en")
                                explain = armMtrlTemp.LanENG.Explain;
                            if (args.Count() > 0)
                                explain = string.Format(explain, args);
                            aArm.Explain = explain;

                            //尋找 Alarm List 內是否有重覆的 Alarm
                            if (!GetRepeatInList(aArm))
                            {
                                aArm.DT = DateTime.Now.ToString("G");
                                aArm.OccurTM = tick.Value;
                                aArm.OccurTMTemp = DateTime.Now.ToOADate();
                                aArm.Module = "SYS";
                                aArm.MTBA = armMtrlTemp.MTBA;
                                aArm.MTBF = armMtrlTemp.MTBF;
                                aArm.SplitStatistic = armMtrlTemp.SplitStatistic;

                                ArmList.Enqueue(aArm);
                            }
                        }
                        else
                        {
                            Say("W0500", ErrCode);
                        }
                    }
                    else
                    {
                        aArm.Module = "ClearAll";
                        ArmList.Enqueue(aArm);
                    }
                }
                catch (Exception e)
                {
                    String ErrStr = e.Message;
                }
            }//lock(obj)
        }
        public void Say(String ErrCode, Control Sender, params Object[] args)
        {
            lock (lockObj)
            {
                try
                {
                    if (m_UIObj == null)
                        return;

                    ArmMtrl armMtrlTemp;
                    ArmMtrl aArm = new ArmMtrl();
                    if (String.Compare(ErrCode, "ClearAll") != 0)
                    {
                        aArm.AlarmLevel = ErrCode.Substring(0, 1);
                        aArm.ModuleID = Convert.ToInt16(ErrCode.Substring(1, 2));
                        aArm.ErrorCode = Convert.ToInt16(ErrCode.Substring(3, 2));

                        AlarmStop(aArm.AlarmLevel);

                        armMtrlTemp = FindAlarm(aArm);
                        if (armMtrlTemp != null)
                        {
                            String explain = "";
                            if (Language == "tw")
                                explain = armMtrlTemp.LanCHT.Explain;
                            else if (Language == "en")
                                explain = armMtrlTemp.LanENG.Explain;
                            if (args.Count() > 0)
                                explain = string.Format(explain, args);
                            aArm.Explain = explain;

                            //尋找 Alarm List 內是否有重覆的 Alarm
                            if (!GetRepeatInList(aArm))
                            {
                                aArm.DT = DateTime.Now.ToString("G");
                                aArm.OccurTM = tick.Value;
                                aArm.OccurTMTemp = DateTime.Now.ToOADate();
                                aArm.Module = Sender.Name;
                                aArm.MTBA = armMtrlTemp.MTBA;
                                aArm.MTBF = armMtrlTemp.MTBF;
                                aArm.SplitStatistic = armMtrlTemp.SplitStatistic;

                                ArmList.Enqueue(aArm);
                            }
                        }
                        else
                        {
                            Say("W0500", ErrCode);
                        }
                    }
                    else
                    {
                        aArm.Module = "ClearAll";
                        ArmList.Enqueue(aArm);
                    }
                }
                catch (Exception e)
                {
                    String ErrStr = e.Message;
                }
            }//lock(obj)
        }

        //清除所有Alarm
        //public bool ClearAll()
        //{
        //    if (!bClearOk)
        //        return false;

        //    lock (lockObj)
        //    {
        //        //計算Alarm的持續時間並記錄到Log
        //        foreach (ArmMtrl pArm in ArmUIList)
        //        {
        //            double ContinueTM = tick.Value - pArm.OccurTM;
        //            DateTime OccurTM = DateTime.FromOADate(pArm.OccurTMTemp);
        //            string sMTBA = (!pArm.MTBA && !pArm.MTBF) ? "N" : ((pArm.MTBA && pArm.MTBF) ? "A" : (!pArm.MTBA && pArm.MTBF) ? "F" : "A");
        //            string sSplitStatistic = (pArm.SplitStatistic) ? "Y" : "N";
        //            string s = string.Format("{0}\t{1}{2:0000}\t{3}\t{4}\t{5:0}\t{6}", pArm.Explain, pArm.AlarmLevel.ToUpper()
        //                , pArm.ModuleID * 100 + pArm.ErrorCode, pArm.Module, OccurTM.ToString(@"yyyy\/MM\/dd") + "\t" + OccurTM.ToString("HH:mm:ss.fff"), ContinueTM, sMTBA);

        //            SYSPara.LogSay(EnLoggerType.EnLog_ARM, s);

        //            if (armDelegate != null)
        //                armDelegate(pArm, false);
        //        }
        //        //+ By Max 20210108
        //        if (!SYSPara.ErrorStop && ArmUIList.Count > 0)
        //        {
        //            LogAnalyzer.AlarmLevels MTBWhat = ArmUIList.Exists(pArm => pArm.MTBA) ? LogAnalyzer.AlarmLevels.A : ArmUIList.Exists(pArm => pArm.MTBF) ? LogAnalyzer.AlarmLevels.F : LogAnalyzer.AlarmLevels.N;
        //            TimeSpan ts = DateTime.Now.Subtract(ErrStopDT);
        //            float totalSecond = (float)Math.Round(ts.TotalSeconds, 1);
        //            SYSPara.logDB.MachineStopSayDb(ErrStopDT, SYSPara.PackageName, SYSPara.生產批號, totalSecond, MTBWhat);
        //        }
        //    }
        //    TObjForm.ObjFormAlarmReset();

        //    bClearOk = false;

        //    ArmMtrl aArm = new ArmMtrl();
        //    aArm.Module = "ClearAll";
        //    ArmList.Enqueue(aArm);
        //    return true;
        //}

        //By Max 20211025, v4.0.94.0
        //清除所有Alarm
        public bool ClearAll(bool WriteNow = false)
        {
            lock (lockObj)
            {
                foreach (ArmMtrl pArm in ArmUIList)
                {
                    if (!ArmW2WList.Contains(pArm))
                        ArmW2WList.Add(pArm);
                }

                //+ By Max 20210108
                if (!SYSPara.ErrorStop && ArmUIList.Count > 0)
                {
                    LogAnalyzer.AlarmLevels MTBWhat = ArmUIList.Exists(pArm => pArm.MTBA) ? LogAnalyzer.AlarmLevels.A : ArmUIList.Exists(pArm => pArm.MTBF) ? LogAnalyzer.AlarmLevels.F : LogAnalyzer.AlarmLevels.N;
                    TimeSpan ts = DateTime.Now.Subtract(ErrStopDT);
                    float totalSecond = (float)Math.Round(ts.TotalSeconds, 1);
                    SYSPara.logDB.MachineStopSayDb(ErrStopDT, SYSPara.PackageName, SYSPara.生產批號, totalSecond, MTBWhat);
                }

                if (WriteNow)
                    WriteArmToFile();
            }

            bNeedToWrite = true; 
            bClearOk = false;

            ArmMtrl aArm = new ArmMtrl();
            aArm.Module = "ClearAll";
            ArmList.Enqueue(aArm);
            return true;
        }
        #endregion
    }

    public class Light
    {
        LightUIObj LUIObj;
        ArmMtrl armMtrl = null;
        StatusLightSetDT StatusLight = null;
        object obj = new object();
        bool m_bBlink = false;
        public Light(ref LightUIObj mLUIObj)
        {
            this.LUIObj = mLUIObj;
            BlinkTimer = new MyTimer();
            BlinkTimer.AutoReset = false;
            BlinkTimer.SetOnDelayTime(500);
        }
        public void SetLightByAlarm(ArmMtrl ArmMaterial)
        {
            armMtrl = ArmMaterial;
        }
        public void SetLightByStatus(StatusLightSetDT mStatusLight)
        {
            StatusLight = mStatusLight;
        }

        //實際顯示燈號及音樂
        static MyTimer BlinkTimer;
        public void ShowLight()
        {
            switch (SYSPara.LightMode)
            {
                case 0: //By Status
                    ShowLightByStatus();
                    break;
                case 1: //By Alarm
                    ShowLightByAlarm();
                    break;
            }
        }

        //依設備狀態顯示燈號
        private void ShowLightByStatus()
        {
            if (SYSPara.ManualControlIO) //Handler進入手動控制頁面，釋放燈號音樂控制權
                return;
            if (LUIObj.GreenLight == null)
                return;
            if (StatusLight == null)
                return;

            if (LUIObj.GreenLight != null && LUIObj.GreenLight.IOPort != "")
                LUIObj.GreenLight.Value = StatusLight.mLightSet.GreenBlink ? m_bBlink : (StatusLight.mLightSet.Green ? true : false);
            if (LUIObj.YellowLight != null && LUIObj.YellowLight.IOPort != "")
                LUIObj.YellowLight.Value = StatusLight.mLightSet.YellowBlink ? m_bBlink : (StatusLight.mLightSet.Yellow ? true : false);
            if (LUIObj.RedLight != null && LUIObj.RedLight.IOPort != "")
                LUIObj.RedLight.Value = StatusLight.mLightSet.RedBlink ? m_bBlink : (StatusLight.mLightSet.Red ? true : false);

            if (SYSPara.MusicOn)
            {
                if (LUIObj.Music1 != null && LUIObj.Music1.IOPort != "")
                    LUIObj.Music1.Value = (StatusLight.mLightSet.MusicNo == 1);
                if (LUIObj.Music2 != null && LUIObj.Music2.IOPort != "")
                    LUIObj.Music2.Value = (StatusLight.mLightSet.MusicNo == 2);
                if (LUIObj.Music3 != null && LUIObj.Music3.IOPort != "")
                    LUIObj.Music3.Value = (StatusLight.mLightSet.MusicNo == 3);
                if (LUIObj.Music4 != null && LUIObj.Music4.IOPort != "")
                    LUIObj.Music4.Value = (StatusLight.mLightSet.MusicNo == 4);
            }
            else
            {
                if (LUIObj.Music1 != null && LUIObj.Music1.IOPort != "")
                    LUIObj.Music1.Value = false;
                if (LUIObj.Music2 != null && LUIObj.Music2.IOPort != "")
                    LUIObj.Music2.Value = false;
                if (LUIObj.Music3 != null && LUIObj.Music3.IOPort != "")
                    LUIObj.Music3.Value = false;
                if (LUIObj.Music4 != null && LUIObj.Music4.IOPort != "")
                    LUIObj.Music4.Value = false;
            }

            //控制燈號閃爍的Timer
            if (BlinkTimer.On())
            {
                m_bBlink = !m_bBlink;
                BlinkTimer.Restart();
            }
        }

        public void MusicOff()
        {
            if (LUIObj.Music1 != null && LUIObj.Music1.IOPort != "")
                LUIObj.Music1.Value = false;
            if (LUIObj.Music2 != null && LUIObj.Music2.IOPort != "")
                LUIObj.Music2.Value = false;
            if (LUIObj.Music3 != null && LUIObj.Music3.IOPort != "")
                LUIObj.Music3.Value = false;
            if (LUIObj.Music4 != null && LUIObj.Music4.IOPort != "")
                LUIObj.Music4.Value = false;
        }

        //依Alarm狀態顯示燈號
        private void ShowLightByAlarm()
        {
            lock (obj)
            {
                if (SYSPara.ManualControlIO) //Handler進入手動控制頁面，釋放燈號音樂控制權
                    return;

                if (LUIObj.GreenLight == null)
                    return;

                if (armMtrl == null)
                    return;

                if (LUIObj.GreenLight != null && LUIObj.GreenLight.IOPort != "")
                    LUIObj.GreenLight.Value = armMtrl.mLightSet.GreenBlink ? m_bBlink : (armMtrl.mLightSet.Green ? true : false);
                if (LUIObj.YellowLight != null && LUIObj.YellowLight.IOPort != "")
                    LUIObj.YellowLight.Value = armMtrl.mLightSet.YellowBlink ? m_bBlink : (armMtrl.mLightSet.Yellow ? true : false);
                if (LUIObj.RedLight != null && LUIObj.RedLight.IOPort != "")
                    LUIObj.RedLight.Value = armMtrl.mLightSet.RedBlink ? m_bBlink : (armMtrl.mLightSet.Red ? true : false);

                if (SYSPara.MusicOn)
                {
                    if (LUIObj.Music1 != null && LUIObj.Music1.IOPort != "")
                        LUIObj.Music1.Value = (armMtrl.mLightSet.MusicNo == 1);
                    if (LUIObj.Music2 != null && LUIObj.Music2.IOPort != "")
                        LUIObj.Music2.Value = (armMtrl.mLightSet.MusicNo == 2);
                    if (LUIObj.Music3 != null && LUIObj.Music3.IOPort != "")
                        LUIObj.Music3.Value = (armMtrl.mLightSet.MusicNo == 3);
                    if (LUIObj.Music4 != null && LUIObj.Music4.IOPort != "")
                        LUIObj.Music4.Value = (armMtrl.mLightSet.MusicNo == 4);
                }
                else
                {
                    if (LUIObj.Music1 != null && LUIObj.Music1.IOPort != "")
                        LUIObj.Music1.Value = false;
                    if (LUIObj.Music2 != null && LUIObj.Music2.IOPort != "")
                        LUIObj.Music2.Value = false;
                    if (LUIObj.Music3 != null && LUIObj.Music3.IOPort != "")
                        LUIObj.Music3.Value = false;
                    if (LUIObj.Music4 != null && LUIObj.Music4.IOPort != "")
                        LUIObj.Music4.Value = false;
                }

                //控制燈號閃爍的Timer
                if (BlinkTimer.On()) 
                {
                    m_bBlink = !m_bBlink;
                    BlinkTimer.Restart();
                }
            }
        }
    }
}
