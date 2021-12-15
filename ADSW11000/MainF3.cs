using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVLib;
using System.Threading;
using ProVIFM;
using System.Diagnostics;
using System.Reflection;
using ProVTool;
using KCSDK;
using System.Runtime.InteropServices;
using CommonObj;

using System.Resources;
using System.Globalization;

#region copy用
#endregion copy用
namespace ADSW11000
{
    public partial class MainF3 : Form
    {

        #region 基本

        #region 基本_架構使用 (參數定義)

        private Object mtpMSS = null;
        private Object mtpMainFlow = null;
        private Object mtpGEM = null;

        #endregion 基本_架構使用 (參數定義)

        public MainF3()
        {
            InitializeComponent();

            KeyPreview = true;

            #region 基本_MainF3_架構使用 (參數初始化)

            //初始變數
            Width = 1366;
            Height = 768;

            //使MessageBox有多國語系的功能
            MessageBoxManager.Register();

            //設定警報系統的顯示 UI
            SYSPara.Alarm.SetLanguage("tw");
            List<object> lightio = (List<object>)SYSPara.CallProc("MAA", "GetLightIO");
            SYSPara.Alarm.SetUI(lvArmGrid.GetType(), lvArmGrid, lightio);

            //MSS 嵌入 tpMSS
            mtpMSS = tpMSS;
            tpMSS.Controls.Add(FormSet.mMSS);
            FormSet.mMSS.Dock = DockStyle.Fill;



            //MainFlow 嵌入 tpMainFlow
            mtpMainFlow = tpMainFlow;
            //tpMainFlow.Controls.Add(FormSet.mMainFlow);
            //FormSet.mMainFlow.Dock = DockStyle.Fill;
            //Woody v4.0.1.1 將Saw Flowchart顯示至主畫面中
            TabControl tSaw = (TabControl)CallProc("Saw", "GetFlowView");
            tpMainFlow.Controls.Add(tSaw);
            tSaw.Dock = DockStyle.Fill;


            //+ By Max For SECS
            //ProVGemF 嵌入 tpGEM
            int idx = SYSPara.OReadValue("CommProtocol").ToInt();
            if (idx == 1) //SECS
            {
                mtpGEM = tpCommunication;
                tpCommunication.Controls.Add(FormSet.mGemF);
                FormSet.mGemF.Dock = DockStyle.Fill;
                FormSet.mGemF.OnDataExchangeEvent += OnDataExchangeEvent;
            }
            else
            {
                tabMachineState.TabPages.Remove(tpCommunication);
            }

            //建立 AaferShowAlarm 函數
            SYSPara.Alarm.SetAfterShowAlarDelegate(AfterShowAlarm);

            TB_授權有效性.Value = SYSPara.LicenseDate;
            switch (SYSPara.LicenseMode)
            {
                case 0:
                    TB_授權模式.Value = "試用";
                    break;
                case 1:
                    TB_授權模式.Value = "單機授權";
                    TB_授權有效性.Value = "No Limit";
                    break;
                case 2:
                    TB_授權模式.Value = "天數授權";
                    break;
            }

            #endregion 基本_MainF3_架構使用 (參數初始化)

            tp教導流程.Parent = null;
            tp自動流程.Parent = null;
            tp接觸測高.Parent = null;
            tp非接觸測高.Parent = null;
            tp基準線校正.Parent = null;
            tp切割道學習.Parent = null;
            tp靶點學習.Parent = null;
            tp換刀流程.Parent = null;
            tp下刀點確認.Parent = null;
            tp動態刀痕.Parent = null;
            tp手動定靶.Parent = null;
            tp刀痕資料.Parent = null;
            tp重啟視覺.Parent = null;
            tpCCD測試.Parent = null;
            tp圓心校正.Parent = null;

            TeachTP = new TabPage[7] { tp教導流程, tp接觸測高, tp非接觸測高, tp基準線校正, tp切割道學習, tp靶點學習, tp圓心校正 };
            AutoTP = new TabPage[8] { tp自動流程, tp換刀流程, tp下刀點確認, tp動態刀痕, tp手動定靶, tp刀痕資料, tp重啟視覺, tpCCD測試 };

            if (SYSPara.OReadValue("bUseAlarmF").ToBoolean())
            {
                lvArmGrid.Activation = ItemActivation.OneClick;
            }
            else
            {
                lvArmGrid.Activation = ItemActivation.Standard;
            }

            if (SYSPara.UseDoubleScreen)   //v0.0.7.12 By Sanxiu 雙螢幕鎖定
            {
                if (SYSPara.Simulation == 0)
                {
                    bStateFront = !(bool)CallProc("MAA", "GetFrontScreen");
                    bStateBack = !(bool)CallProc("MAA", "GetBackScreen");
                }
                else
                {
                    bStateFront = true;
                    bStateBack = true;
                }
            }

            //CalKufBreakFuncConst();
        }

        //+ By Max For SECS
        //向GEM Form註冊DataExchange事件
        //GEM Form如有需要的資料會透過此事件向Handler要
        //或是有事件也會透過此事件通知Handler
        //使用者須在此事件中依對應的TableCode讀取相對應的參數
        /* ExchangeData資料結構
        public class ExchangeData
        {
            public string DataName;                 //資料名稱 PReadValue或OReadValue使用
            public TableCode Code;                  //OPTION、PACKAGE、PRELOADPACKAGE、REAL
            public HOSTCMD HostCMD;                 //START、STOP、PAUSE...
            public ProVLib.ProVMessage ParamMap;    //Parameter Map <參數名稱，參數值>
            public KCSDK.DataTransfer RetData;      //回傳資料可封裝在RetData中
        }
        */
        ExchangeData OnDataExchangeEvent(EventType evtType, ExchangeData exData)
        {
            switch (evtType)
            {
                case EventType.QUERY:
                    {
                        switch (exData.Code)
                        {
                            case TableCode.OPTION: //通用設定資料
                                {
                                    exData.RetData = SYSPara.OReadValue(exData.DataName);
                                }
                                break;
                            case TableCode.PACKAGE: //產品相關資料
                                {
                                    exData.RetData = SYSPara.PReadValue(exData.DataName);
                                }
                                break;
                            case TableCode.PRELOADPACKAGE: //產品管理相關資料
                                {
                                    exData.RetData = FormSet.mPackage.PreloadPackageDS.ReadValue(exData.DataName);
                                }
                                break;
                            case TableCode.REAL: //其他即時資料
                                {
                                    switch (exData.DataName)
                                    {
                                        case "MDLN":
                                            {
                                                exData.RetData = new DataTransfer(SYSPara.EQID);
                                            }
                                            break;
                                        case "SOFTREV":
                                            {
                                                exData.RetData = new DataTransfer(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString());
                                            }
                                            break;
                                        case "PPACTION":
                                            {
                                                if (exData.ParamMap.IsParameterExisted("LISTPPID"))
                                                {
                                                    string[] list = System.IO.Directory.GetFiles(FormSet.mPackage.FolderPath, "*.xml").Select(file => System.IO.Path.GetFileName(file)).ToArray();
                                                    String ppidList = string.Join(",", list);
                                                    exData.RetData = new DataTransfer(ppidList);
                                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「取得Package列表」的遠端指令");
                                                }
                                                else if (exData.ParamMap.IsParameterExisted("DELPPID"))
                                                {
                                                    String PPIDName = (String)exData.ParamMap.GetParameter("DELPPID");
                                                    if (PPIDName == "ALL")
                                                    {
                                                        string[] list = System.IO.Directory.GetFiles(FormSet.mPackageSelF.FolderPath, "*.xml").Select(file => System.IO.Path.GetFileName(file)).ToArray();
                                                        for (int i = 0; i < list.Count(); ++i)
                                                        {
                                                            string fname = string.Format(@"{0}\{1}", FormSet.mPackageSelF.FolderPath, list[i]);
                                                            System.IO.File.Delete(fname);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        string fname = string.Format(@"{0}\{1}", FormSet.mPackage.FolderPath, PPIDName);
                                                        System.IO.File.Delete(fname);
                                                    }
                                                    exData.RetData = new DataTransfer(true);
                                                    String strTemp = String.Format("收到「刪除Package : {0}」的遠端指令", PPIDName);
                                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, strTemp);
                                                }
                                                else if (exData.ParamMap.IsParameterExisted("EXISTPPID"))
                                                {
                                                    //0 = OK
                                                    //1 = Already have
                                                    //2 = No space
                                                    //3 = Invalid PPID
                                                    //4 = Busy, try later
                                                    //5 = Will not accept
                                                    //>5 = Other error
                                                    //6-63 Reserved
                                                    String PPIDName = (String)exData.ParamMap.GetParameter("EXISTPPID");
                                                    string fname = string.Format(@"{0}\{1}", FormSet.mPackage.FolderPath, PPIDName);
                                                    if (System.IO.File.Exists(fname))
                                                    {
                                                        exData.RetData = new DataTransfer(1);//Already Have
                                                    }
                                                    else
                                                    {
                                                        exData.RetData = new DataTransfer(0); //OK
                                                    }
                                                }
                                                else if (exData.ParamMap.IsParameterExisted("SELPPID"))
                                                {

                                                    String PPIDName = (String)exData.ParamMap.GetParameter("SELPPID");
                                                    string fname = string.Format(@"{0}\{1}", FormSet.mPackage.FolderPath, PPIDName);
                                                    if (System.IO.File.Exists(fname))
                                                    {
                                                        FormSet.mPackage.PreloadPackageDS.Initial(fname, PPIDName);
                                                        exData.RetData = new DataTransfer(0); //OK
                                                    }
                                                    else
                                                    {
                                                        exData.RetData = new DataTransfer(1); //NG
                                                    }

                                                }
                                                else if (exData.ParamMap.IsParameterExisted("PPID"))
                                                {
                                                    exData.RetData = new DataTransfer(FormSet.mPackage.FileName);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case EventType.WRITE:
                    {
                        switch (exData.Code)
                        {
                            case TableCode.OPTION: //通用設定資料
                                {
                                    //...
                                    exData.RetData = new DataTransfer(0); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                }
                                break;
                            case TableCode.PACKAGE: //產品相關資料
                                {
                                    //...
                                    exData.RetData = new DataTransfer(0); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                }
                                break;
                            case TableCode.REAL: //其他即時資料
                                {
                                    switch (exData.DataName)
                                    {
                                        case "EstabTimeDelay":
                                            {
                                                if (exData.ParamMap.IsParameterExisted("P1"))
                                                {
                                                    ushort setVal = (ushort)exData.ParamMap.GetParameter("P1");
                                                    exData.RetData = new DataTransfer(0); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                                }
                                                else
                                                {
                                                    exData.RetData = new DataTransfer(1); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                                }
                                            }
                                            break;
                                        case "MaxTest":
                                            {
                                                if (exData.ParamMap.IsParameterExisted("P1"))
                                                {
                                                    //ushort setVal = (ushort)exData.ParamMap.GetParameter("P1");
                                                    exData.RetData = new DataTransfer(0); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                                }
                                                else
                                                {
                                                    exData.RetData = new DataTransfer(1); //= 0 表示可設定並完成； != 0 表示不可改變或無法改變
                                                }
                                            }
                                            break;
                                        case "PPACTION":
                                            {
                                                if (exData.ParamMap.IsParameterExisted("RCVPPID"))
                                                {
                                                    String PPIDName = (String)exData.ParamMap.GetParameter("RCVPPID");
                                                    string fname = string.Format(@"{0}\{1}", FormSet.mPackage.FolderPath, PPIDName);
                                                    //FormSet.mPackage.PreloadPackageDS.ModifiedLog = true;
                                                    if (!System.IO.File.Exists(fname))
                                                    {
                                                        FormSet.mPackage.PreloadPackageDS.Initial(@"D:\AWDP10000\AWDP10000\bin\LocalData\Default.xml", "Package");

                                                        //+ By Max 20190503
                                                        if (exData.ParamMap.IsParameterExisted("PKGItemData"))
                                                        {
                                                            List<PKGItem> PKGItemList = (List<PKGItem>)exData.ParamMap.GetParameter("PKGItemData");

                                                            for (int i = 0; i < PKGItemList.Count; ++i)
                                                            {
                                                                PKGItem Item = PKGItemList[i];
                                                                switch (Item.Format)
                                                                {
                                                                    case "A":
                                                                        {
                                                                            string oldValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToString();
                                                                            string value = Item.sData;
                                                                            FormSet.mPackage.PreloadPackageDS.SetData(Item.QueryName, "String", (object)value);
                                                                            //FormSet.mPackage.PreloadPackageDS.ChangeData(DataManagement.DataType.Package, Item.QueryName, (object)value, false, "Package", ValueDataType.String);
                                                                            string newValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToString();
                                                                        }
                                                                        break;
                                                                    case "BOOL":
                                                                        {
                                                                        }
                                                                        break;
                                                                    case "F4":
                                                                    case "F8":
                                                                        {
                                                                            double oldValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToDouble();
                                                                            double value = Item.fData;
                                                                            FormSet.mPackage.PreloadPackageDS.SetData(Item.QueryName, "Double", (object)value);
                                                                            //FormSet.mPackage.PreloadPackageDS.ChangeData(DataManagement.DataType.Package, Item.QueryName, (object)value, false, "Package", ValueDataType.Double);
                                                                            double newValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToDouble();
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
                                                                            int oldValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToInt();
                                                                            int value = Item.iData;
                                                                            FormSet.mPackage.PreloadPackageDS.SetData(Item.QueryName, "Int", (object)value);
                                                                            //FormSet.mPackage.PreloadPackageDS.ChangeData(DataManagement.DataType.Package, Item.QueryName, (object)value, false, "Package", ValueDataType.Int);
                                                                            int newValue = FormSet.mPackage.PreloadPackageDS.ReadValue(Item.QueryName).ToInt();
                                                                        }
                                                                        break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    FormSet.mPackage.PreloadPackageDS.SaveFile(fname);
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case EventType.COMMAND:
                    {
                        /*
                         *  Command Return Code:
                            0 = Acknowledge, command has been performed
                            1 = Command does not exist
                            2 = Cannot perform now
                            3 = At least one parameter isinvalid
                            4 = Acknowledge, command will be performed with completion signaled later by an event
                            5 = Rejected, Already in Desired Condition
                            6 = No such object exists
                            7-63 Reserved
                        */
                        switch (exData.HostCMD)
                        {
                            case HOSTCMD.INITIAL:
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「原點復歸開始」的遠端指令");

                                    SYSPara.SysState = StateMode.SM_PACKAGELOAD_OK;

                                    SYSPara.ErrorStop = false;
                                    SYSPara.MusicOn = true;
                                    SYSPara.Alarm.ClearAll();

                                    SYSPara.SysRun = true;
                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.START:
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「生產開始」的遠端指令");

                                    SYSPara.ErrorStop = false;
                                    SYSPara.MusicOn = true;
                                    SYSPara.Alarm.ClearAll();

                                    SYSPara.SysRun = true;
                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.LOTEND:
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「結批」的遠端指令");
                                    SYSPara.LotendOk = false;
                                    SYSPara.Lotend = true;
                                    SYSPara.Alarm.Say("I0504"); //結批開始

                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.PPSELECT: //Select Package
                                {
                                    if (exData.ParamMap.IsParameterExisted("PPName"))
                                    {
                                        String strPackageName = (string)exData.ParamMap.GetParameter("PPName");
                                        if (!String.IsNullOrEmpty(strPackageName))
                                        {
                                            //Load Package ...
                                            //if (!String.IsNullOrEmpty(FormSet.mPackageSelF.FileName)) //直接換掉Package
                                            //{
                                            //    FormSet.mPackage.SetFileName(strPackageName);
                                            //    FormSet.mPackageSelF.FileName = strPackageName;
                                            //}
                                            //else //
                                            {
                                                FormSet.mPackageSelF.FileName = strPackageName;
                                                SYSPara.SysState = StateMode.SM_PACKAGELOAD_RESET;
                                            }

                                            exData.RetData = new DataTransfer(0); //Ack
                                            SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「載入料號」的遠端指令");
                                            String strMsg = String.Format("「料號」: {0}", strPackageName);
                                            SYSPara.LogSay(EnLoggerType.EnLog_COMM, strMsg);

                                            SYSPara.SysState = StateMode.SM_PACKAGELOAD_RESET;
                                        }
                                        else
                                        {
                                            exData.RetData = new DataTransfer(2); //Can't Perform Now
                                        }
                                    }
                                    else
                                    {
                                        exData.RetData = new DataTransfer(2); //Can't Perform Now
                                    }
                                }
                                break;
                            case HOSTCMD.STOP:
                                {
                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.PAUSE:
                                {
                                    if (SYSPara.SysState == StateMode.SM_ALARM)
                                    {
                                        SYSPara.ErrorStop = false;
                                        SYSPara.SysState = StateMode.SM_PAUSE;
                                    }

                                    SYSPara.MusicOn = false;
                                    FormSet.mMSS.M_Stop();

                                    SYSPara.SysRun = false;

                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「生產暫停」的遠端指令");

                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.RESUME:
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「生產繼續」的遠端指令");

                                    SYSPara.ErrorStop = false;
                                    SYSPara.MusicOn = true;
                                    SYSPara.Alarm.ClearAll();

                                    SYSPara.SysRun = true;
                                    exData.RetData = new DataTransfer(0); //Ack
                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                            case HOSTCMD.ABORT:
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "收到「生產放棄」的遠端指令");
                                    exData.RetData = new DataTransfer(0); //Ack
                                }
                                break;
                        }
                    }
                    break;
                case EventType.STATECHANGE:
                    {
                        switch (exData.DataName)
                        {
                            case "EQPOFFLINE":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "控制模式切至EQPOFFLINE");
                                }
                                break;
                            case "HOSTOFFLINE":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "控制模式切至HOSTOFFLINE");
                                }
                                break;
                            case "ONLINELOCAL":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "控制模式切至ONLINELOCAL");
                                }
                                break;
                            case "ONLINEREMOTE":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "控制模式切至ONLINEREMOTE");
                                }
                                break;
                            case "COMMUNICATING":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "連線狀態為COMMUNICATING");
                                }
                                break;
                            case "NOTCOMMUNICATING":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "連線狀態為NOTCOMMUNICATING");
                                }
                                break;
                            case "DISABLED":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "連線狀態為DISABLED");
                                }
                                break;

                        }
                    }
                    break;
                case EventType.SPOOLING:
                    {
                        switch (exData.DataName)
                        {
                            case "ACTIVE":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "Spooling 機制運作中！");
                                }
                                break;
                            case "INACTIVE":
                                {
                                    SYSPara.LogSay(EnLoggerType.EnLog_COMM, "Spooling 機制停止運作！");
                                }
                                break;
                        }
                    }
                    break;
            }

            return exData;
        }

        private void MainF1_Load(object sender, EventArgs e)
        {
            #region 架構使用 (參數初始化)
            //設定視窗使用的語系
            SYSPara.Lang.ChangeLanguage("tw");
            lbVersion.Text = string.Format("{1} Version {0}", FileVersionInfo
                .GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(), SYSPara.EQID);
            lbProjectName.Text = this.GetType().Assembly.GetName().Name;

            SYSPara.MaxScanTime = 0;
            SYSPara.MusicOn = true;
            SYSPara.HMIReady = true;
            SYSPara.HMIClose = false;
            SYSPara.InitialOk = false;

            #endregion

            #region 使用者修改 (顯示元件前置設定)

            if (bUseCCD)
            {
                VisionLoad();
                SetParent(hCCDHandle, panel_Vision.Handle);
                SetWindowPos((int)hCCDHandle, 0, (panel_Vision.Width - iVisionWidth) / 2, (panel_Vision.Height - iVisionHeight) / 2, iVisionWidth, iVisionHeight, 4);

                CallProc("Saw", "Set_CCDHandle", (int)hCCDHandle);
            }

            //Cut_graphics = pictureBox_Cut.CreateGraphics();

            //FormSet.mPackage.Initial();
            //FormSet.mOption.Initial();
            //FormSet.m內參設定.Initial();

            //安全門機制改善
            //傳送發生安全門或緊急停止開關觸發時回呼的委派函式
            if (SYSPara.SetupReadValue("安全門機制設定").ToInt() == 2)
                SYSPara.CallProc("MAA", "RecordSaftyCheckDelegate", new Action<int>(SaftyActionDelegate));

            //Woody v4.0.1.23 註冊委派給Saw
            SYSPara.CallProc("Saw", "RecordDelegateDrawLineData", new Action<string, int, int, int, bool>(DrawLineData));
            
            //Woody v4.0.1.30 主畫面顯示切割/掃靶時間
            SYSPara.CallProc("Saw", "RecordDelegateShowTime", new Action<double, double, double, double, double>(ShowAllTime));

            // For cross and vertical line callback function
            SYSPara.CallProc("Saw", "RegisterCrossLineCB", new EventHandler(ShowCrossLine));
            SYSPara.CallProc("Saw", "RegisterVerticalLineCB", new EventHandler(ShowVerticalLine));

            //+ By Max 20211030
            SYSPara.IsUseDAQ = SYSPara.SetupReadValue("UseDAQ").ToBoolean();

            //+ By Max 20210218, UI Timer的啟動移至最後
            tmUIRefresh.Enabled = true;

            #endregion
        }

        private void MainF1_Shown(object sender, EventArgs e)
        {
            //使用者登入
            UserLogin();
        }

        #endregion 基本

        #region 架構使用 (Public)

        #region 架構使用 (Public)_顯示產品名稱

        public void ShowPackageName(bool show)
        {
            string packagename = "";
            if (show)
            {
                if (string.IsNullOrEmpty(FormSet.mPackage.FolderName_Auto))
                    packagename = string.Format("{0}", FormSet.mPackage.FileName_Auto);
                else
                    packagename = string.Format("{0}_{1}", FormSet.mPackage.FolderName_Auto, FormSet.mPackage.FileName_Auto);
            }

            lbPackageName.Text = packagename;
        }

        #endregion 架構使用 (Public)_顯示產品名稱

        #region 架構使用 (Public)_關閉歸零視窗

        public void ExitInitialForm()
        {
            AllFormHide();
            SetControlSW(ControlButtonType.None);
        }

        #endregion 架構使用 (Public)_關閉歸零視窗

        #region 架構使用 (Public)_切換至歸零視窗

        public void ChangeToInitialForm()
        {
            AllFormHide();
            //把Initial Form 載入到 pnlContainer
            pnlContainer.Controls.Add(FormSet.mInitialF);
            FormSet.mInitialF.WindowState = FormWindowState.Maximized;
            FormSet.mInitialF.Reset();
            FormSet.mInitialF.Show();

            //把 Control Button 設為不可按
            SetControlSW(ControlButtonType.Initial);
        }

        #endregion 架構使用 (Public)_切換至歸零視窗

        #region 架構使用 (Public)_顯示作業開始時間

        public void ShowAutoStartTM()
        {
            lbOperationStartTM.Value = DateTime.Now.ToString("G");
            lbOperationEndTM.Value = "";
        }

        #endregion 架構使用 (Public)_顯示作業開始時間

        #region 架構使用 (Public)_顯示作業結束時間

        public void ShowAutoEndTM()
        {   //顯示作業結束時間
            lbOperationEndTM.Value = DateTime.Now.ToString("G");
        }

        #endregion 架構使用 (Public)_顯示作業結束時間

        #endregion 架構使用 (Public)

        #region 架構使用 (Private)

        #region 架構使用 (Private)_使用者登入

        private void UserLogin()
        {
            SYSPara.LoginUser = UserType.utNone;
            ChangeUser();

            bool blogin = false;

            if (SYSPara.LoginMode == 0)
            {
                while (!blogin)
                {
                    DialogResult result = FormSet.mUserLogin.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        SYSPara.LoginUser = FormSet.mUserLogin.GetUserType();
                        ChangeUser();
                        blogin = true;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("{0} 登入", GetNowUser()));
                    }
                }
            }
            else if (SYSPara.LoginMode == 1)
            {
                while (!blogin)
                {
                    DialogResult result = FormSet.mUserLoginEx.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        SYSPara.LoginUser = FormSet.mUserLoginEx.GetUserType(ref SYSPara.LoginUserID);
                        ChangeUser();
                        blogin = true;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("{0} 登入", GetNowUser()));
                    }
                }
            }
        }

        private string GetNowUser()
        {
            string s = "Operator";
            switch (SYSPara.LoginUser)
            {
                case UserType.utEngineer:
                    s = "Engineer";
                    break;
                case UserType.utAdministrator:
                    s = "Administrator";
                    break;
                case UserType.utProV:
                    s = "Designer";
                    break;
            }
            return s;
        }

        #endregion 架構使用 (Private)_使用者登入

        #region 架構使用 (Private)_更換使用者需要處理的資料

        private void ChangeUser()
        {
            FormSet.mOption.ChangeUser();
            switch (SYSPara.LoginUser)
            {
                case UserType.utNone:
                    btnManual.Enabled = false;
                    btnPackage.Enabled = false;
                    btnOption.Enabled = false;
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMSS) != -1)
                        tabMachineState.TabPages.Remove((TabPage)mtpMSS);
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMainFlow) != -1)
                        tabMachineState.TabPages.Remove((TabPage)mtpMainFlow);

                    lbUserType.Text = "No Login";
                    break;
                case UserType.utOperator:
                    btnManual.Enabled = false;
                    btnPackage.Enabled = false;
                    btnOption.Enabled = false;
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMSS) != -1)
                        tabMachineState.TabPages.Remove((TabPage)mtpMSS);
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMainFlow) != -1)
                        tabMachineState.TabPages.Remove((TabPage)mtpMainFlow);

                    if (SYSPara.LoginMode == 0)
                        lbUserType.Text = "Operator";
                    else if (SYSPara.LoginMode == 1)
                        lbUserType.Text = SYSPara.LoginUserID;
                    break;
                case UserType.utEngineer:
                    btnManual.Enabled = true;
                    btnPackage.Enabled = true;
                    btnOption.Enabled = true;
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMSS) != -1)
                        tabMachineState.TabPages.Remove((TabPage)mtpMSS);
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMainFlow) == -1)
                        tabMachineState.TabPages.Add((TabPage)mtpMainFlow);

                    if (SYSPara.LoginMode == 0)
                        lbUserType.Text = "Engineer";
                    else if (SYSPara.LoginMode == 1)
                        lbUserType.Text = SYSPara.LoginUserID;
                    break;
                case UserType.utAdministrator:
                    btnManual.Enabled = true;
                    btnPackage.Enabled = true;
                    btnOption.Enabled = true;
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMSS) == -1)
                        tabMachineState.TabPages.Add((TabPage)mtpMSS);
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMainFlow) == -1)
                        tabMachineState.TabPages.Add((TabPage)mtpMainFlow);

                    if (SYSPara.LoginMode == 0)
                        lbUserType.Text = "Administrator";
                    else if (SYSPara.LoginMode == 1)
                        lbUserType.Text = SYSPara.LoginUserID;
                    break;
                case UserType.utProV:
                    btnManual.Enabled = true;
                    btnPackage.Enabled = true;
                    btnOption.Enabled = true;
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMSS) == -1)
                        tabMachineState.TabPages.Add((TabPage)mtpMSS);
                    if (tabMachineState.TabPages.IndexOf((TabPage)mtpMainFlow) == -1)
                        tabMachineState.TabPages.Add((TabPage)mtpMainFlow);

                    lbUserType.Text = "Designer";
                    break;
            }
        }

        #endregion 架構使用 (Private)_更換使用者需要處理的資料

        #region 架構使用 (Private)_將Panel上的所有Form給隱藏

        private void AllFormHide()
        {
            foreach (Control control in pnlContainer.Controls)
                if (control is Form)
                    ((Form)control).Parent = null;
        }

        #endregion 架構使用 (Private)_將Panel上的所有Form給隱藏

        #region 架構使用 (Private)_切換 UI 畫面

        private void PageChange(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(((Button)sender).Tag);

            SYSPara.OptionStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
            SYSPara.PackageStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停

            switch (tag)
            {
                case 1: //自動操作
                    if (SYSPara.IsAutoMode)
                        return;

                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「自動操作」");

                    SYSPara.SysRun = false;
                    FormSet.mMSS.M_Stop();
                    SYSPara.SysState = StateMode.SM_PACKAGELOAD_RESET;

                    pnlContainer.Controls.Add(FormSet.mPackageSelF);
                    FormSet.mPackageSelF.WindowState = FormWindowState.Maximized;
                    FormSet.mPackageSelF.Show();
                    FormSet.mPackageSelF.Initial();

                    //把 Control Button 設為不可按
                    SetControlSW(ControlButtonType.Initial);

                    FormSet.mPackageSelF.FolderName = FormSet.mPackageSelF.FolderName;//v4.0.1.39 為了讓packageselF進行Refresh動作
                    break;
                case 2: //手動操作
                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「手動操作」");

                    //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
                    if (String.IsNullOrEmpty(SYSPara.PackageName) == false)
                    {
                        if ((bool)CallProc("Saw", "GetAutoReMoveToRunPos"))
                        {
                            iGetReMovetmrStep = 0;
                            GetReMovetmr.Enabled = true;
                        }
                    }

                    SYSPara.SysRun = false;
                    FormSet.mMSS.M_Stop();
                    SYSPara.SysState = StateMode.SM_MANUAL_RESET;

                    pnlContainer.Controls.Add(FormSet.mModuleContainer);
                    FormSet.mModuleContainer.WindowState = FormWindowState.Maximized;
                    FormSet.mModuleContainer.Show();
                    FormSet.mModuleContainer.PerformClick();

                    //把 Control Button 設為不可按
                    SetControlSW(ControlButtonType.Initial);
                    break;
                case 3: //產品設定
                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品設定」");

                    //SYSPara.SysRun = false;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    //FormSet.mMSS.M_Stop();   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停

                    pnlContainer.Controls.Add(FormSet.mPackage);
                    FormSet.mPackage.WindowState = FormWindowState.Maximized;
                    FormSet.mPackage.Show();
                    FormSet.mPackage.Initial();

                    //把 Control Button 設為不可按
                    SetControlSW(ControlButtonType.Initial);
                    break;
                case 4: //通用設定
                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「通用設定」");

                    //SYSPara.SysRun = false;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    //FormSet.mMSS.M_Stop();   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停

                    pnlContainer.Controls.Add(FormSet.mOption);
                    FormSet.mOption.WindowState = FormWindowState.Maximized;
                    FormSet.mOption.Show();
                    FormSet.mOption.RefreshAllData();
                    FormSet.mOption.InitialSubPackage();
                    break;
                case 5: //切換語系
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「語系切換」");
                    switch (SYSPara.Lang.GetNowLanguage())
                    {
                        case "tw":
                            SYSPara.Lang.ChangeLanguage("en");
                            foreach (BaseModuleInterface module in FormSet.ModuleList)
                                module.ChangeLanguage("en");
                            SYSPara.Alarm.SetLanguage("en");
                            FormSet.mOption.ChangeLang(SYSPara.Lang.GetNowLanguage());
                            CallProc("Saw", "ChangeLang", SYSPara.Lang.GetNowLanguage());
                            break;
                        case "en":
                            SYSPara.Lang.ChangeLanguage("tw");
                            foreach (BaseModuleInterface module in FormSet.ModuleList)
                                module.ChangeLanguage("tw");
                            SYSPara.Alarm.SetLanguage("tw");
                            FormSet.mOption.ChangeLang(SYSPara.Lang.GetNowLanguage());
                            CallProc("Saw", "ChangeLang", SYSPara.Lang.GetNowLanguage());
                            break;
                    }
                    //切換英文後，語系也須隨之改變
                    break;
                case 6: //History
                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「歷史記錄」");

                    pnlContainer.Controls.Add(FormSet.mLogF);
                    FormSet.mLogF.InitialLogger();
                    FormSet.mLogF.WindowState = FormWindowState.Maximized;
                    FormSet.mLogF.Show();
                    break;
            }
        }

        private void ChangeLang(string Lan)
        {
            if (Lan == "en")
            {
                tp教導流程.Text = "Teach";
                tp接觸測高.Text = "Spark Test";
                tp非接觸測高.Text = "Non-Contact Spark";
                tp基準線校正.Text = "Baseline";
                tp切割道學習.Text = "Learn Fixture";
                tp靶點學習.Text = "Learn Pattern";
                tp圓心校正.Text = "Center Correction";
                tp自動流程.Text = "Auto";
                tp換刀流程.Text = "Change Blade";
                tp下刀點確認.Text = "Mark Recheck";
                tp動態刀痕.Text = "Dyn Kerf";
                tp手動定靶.Text = "Manual Align";
                tp刀痕資料.Text = "Kerf Data";
                tp重啟視覺.Text = "Restart CCD";
                tpCCD測試.Text = "CCD";
                button_下刀點確認.Text = "Mark Recheck";
                //+ By Max 20210315, v4.0.1.54, 動態刀痕切換語系當掉問題，Dyn Check -> Dyn Kerf
                button_動態刀痕.Text = "Dyn Kerf";
            }
            else if (Lan == "tw")
            {
                tp教導流程.Text = "教導流程";
                tp接觸測高.Text = "接觸測高";
                tp非接觸測高.Text = "非接觸測高";
                tp基準線校正.Text = "基準線校正";
                tp切割道學習.Text = "切割道學習";
                tp靶點學習.Text = "靶點學習";
                tp圓心校正.Text = "圓心校正";
                tp自動流程.Text = "自動流程";
                tp換刀流程.Text = "換刀流程";
                tp下刀點確認.Text = "下刀點確認";
                tp動態刀痕.Text = "動態刀痕";
                tp手動定靶.Text = "手動定靶";
                tp刀痕資料.Text = "刀痕資料";
                tp重啟視覺.Text = "重啟視覺";
                tpCCD測試.Text = "CCD測試";
                button_下刀點確認.Text = "下刀點確認";
                button_動態刀痕.Text = "動態刀痕";
            }
        }

        #endregion 架構使用 (Private)_切換 UI 畫面

        #region 架構使用 (Private)_控制按鈕致能

        public void SetControlSW(ControlButtonType type)
        {
            switch (type)
            {
                case ControlButtonType.Initial:
                    foreach (Control control in pnlControl.Controls)
                    {
                        if (control is Button)
                        {
                            if ((control == btnStart) || (control == btnPause) || (control == btnReset))
                                control.Enabled = true;
                            else
                                control.Enabled = false;
                        }
                    }
                    break;
                case ControlButtonType.None:
                    foreach (Control control in pnlControl.Controls)
                    {
                        if (control is Button)
                            control.Enabled = true;
                    }
                    ChangeUser();
                    break;
            }
        }

        #endregion 架構使用 (Private)_控制按鈕致能

        #region 架構使用 (Private)_設備狀態顯示

        private void DisplaySysState()
        {
            if (SYSPara.SysRun)
                lbRunState.BackColor = Color.Lime;
            else
            {
                if (SYSPara.ErrorStop)
                    lbRunState.BackColor = Color.Red;
                else
                    lbRunState.BackColor = Color.Yellow;
            }

            lbRunState.Text = SYSPara.RunMode.ToString();
        }

        private TabPage GetTeachTP(string buttonText)
        {
            TabPage TP = null;

            for (int i = 0; i < TeachTP.Length; i++)
            {
                if (TeachTP[i].Text.Equals(buttonText))
                    TP = TeachTP[i];
            }
            return TP;
        }

        private TabPage GetAutoTP(string buttonText)
        {
            TabPage TP = null;

            for (int i = 0; i < AutoTP.Length; i++)
            {
                if (AutoTP[i].Text.Equals(buttonText))
                    TP = AutoTP[i];
            }
            return TP;
        }

        private void ChoiceTeach(object sender)
        {
            //CanControlMotor = false;
            ChangeLang(SYSPara.Lang.GetNowLanguage());//v4.0.1.34
            SysMode = Mode.Teach;
            tp選項.Parent = null;
            tp教導流程.Parent = null;
            TPTemp = GetTeachTP(((Button)sender).Text);
            TPTemp.Parent = tc選項;
            SYSPara.Lang.ChangeLanguage(SYSPara.Lang.GetNowLanguage());//v4.0.1.34

            switch (FormSet.mMainFlow.mSaw.OReadValue("iUseSpindleType").ToInt())
            {
                case 0://雙刀
                    comboBox_Spark.Items.Clear();
                    comboBox_Spark.Items.Add("Z1");
                    comboBox_Spark.Items.Add("Z2");
                    chkZ1NCT.Visible = true;
                    chkZ2NCT.Visible = true;
                    comboBox_基準線校正.Items.Clear();
                    comboBox_基準線校正.Items.Add("Z1");
                    comboBox_基準線校正.Items.Add("Z2");
                    break;
                case 1://Z1
                    comboBox_Spark.Items.Clear();
                    comboBox_Spark.Items.Add("Z1");
                    chkZ1NCT.Visible = true;
                    chkZ2NCT.Visible = false;
                    chkZ2NCT.Checked = false;
                    comboBox_基準線校正.Items.Clear();
                    comboBox_基準線校正.Items.Add("Z1");
                    break;
                case 2://Z2
                    comboBox_Spark.Items.Clear();
                    comboBox_Spark.Items.Add("Z2");
                    chkZ1NCT.Visible = false;
                    chkZ1NCT.Checked = false;
                    chkZ2NCT.Visible = true;
                    comboBox_基準線校正.Items.Clear();
                    comboBox_基準線校正.Items.Add("Z2");
                    break;
            }
        }

        private void ChoiceAuto(object sender)
        {
            //CanControlMotor = false;
            ChangeLang(SYSPara.Lang.GetNowLanguage());//v4.0.1.34
            SysMode = Mode.Auto;
            tp選項.Parent = null;
            tp自動流程.Parent = null;
            TPTemp = GetAutoTP(((Button)sender).Text);
            TPTemp.Parent = tc選項;
            SYSPara.Lang.ChangeLanguage(SYSPara.Lang.GetNowLanguage());//v4.0.1.34
        }

        public void CloseTeachorAuto()
        {
            ChangeLang(SYSPara.Lang.GetNowLanguage());//v4.0.1.34
            //CanControlMotor = false;
            if (SysMode == Mode.Teach)
            {
                if (TPTemp != null && TPTemp.Text != tp教導流程.Text)//v4.0.1.34
                {
                    tp教導流程.Parent = tc選項;
                    TPTemp.Parent = null;
                    TPTemp = null;
                }
                else
                {
                    SysMode = Mode.Idle;
                    tp教導流程.Parent = null;
                    tp選項.Parent = tc選項;
                }
            }
            else if (SysMode == Mode.Auto)
            {
                if (TPTemp != null && TPTemp.Text != tp自動流程.Text)//v4.0.1.34
                {
                    tp自動流程.Parent = tc選項;
                    TPTemp.Parent = null;
                    TPTemp = null;
                }
                else
                {
                    SysMode = Mode.Idle;
                    tp自動流程.Parent = null;
                    tp選項.Parent = tc選項;
                }
            }
            SYSPara.Lang.ChangeLanguage(SYSPara.Lang.GetNowLanguage());//v4.0.1.34
        }

        #endregion 架構使用 (Private)_設備狀態顯示

        #region 架構使用 (Private)_警報後置處理

        private bool bAutoSaveStep = false;   //v0.0.7.11 By Sanxiu 自動記錄，執行MainRun發生例外
        //+ By Max For SECS
        private void AfterShowAlarm(ArmMtrl armMtrl, bool bSet)
        {
            int idx = SYSPara.OReadValue("CommProtocol").ToInt();
            if (idx == 1) //SECS
            {
                FormSet.mGemF.AlarmReport(armMtrl, bSet);
            }

            //+ By Max For AlarmForm Show
            if (bSet)
            {
                AlarmForm.ShowAlarmForm(armMtrl);
            }
            else
            {
                AlarmForm.CloseAlarmForm();
            }

            //SYSPara.LogSay("SPC", armMtrl.Explain);


            if (armMtrl.AlarmLevel == "E")
            {
                String strErr = "SawError_";
                String strExplain = armMtrl.Explain;
                if (strExplain.Contains('_'))
                {
                    char[] strArray = strExplain.ToCharArray();
                    strExplain = "";
                    foreach (char c in strArray)
                    {
                        if (c != '_')
                        {
                            strExplain += c;
                        }
                    }
                }
                strErr += strExplain;

                if (armMtrl.Explain.IndexOf("MainRun發生例外") > 0)   //v0.0.7.11 By Sanxiu 自動記錄，執行MainRun發生例外
                {
                    bAutoSaveStep = true;
                }

                if (FormSet.mMainFlow.mSaw.SReadValue("ConnectHandler").ToInt() > 0)
                    CallProc("Saw", "SendData", strErr);
            }
        }

        #endregion 架構使用 (Private)_警報後置處理

        #region 架構使用 (Private)_呼叫指定模組內的指定函數功能

        public object CallProc(string ModuleName, string FuncName, params object[] args)
        {
            foreach (BaseModuleInterface obj in FormSet.ModuleList)
            {
                if (obj.Name == ModuleName)
                {
                    try
                    {
                        object result = obj.GetType().GetMethod(FuncName).Invoke(obj, args);
                        return result;
                    }
                    catch (Exception)
                    {
                        SYSPara.Alarm.Say("E0506", string.Format("{0}.{1}", ModuleName, FuncName));
                        return null;
                    };
                }
            }
            return null;
        }

        #endregion 架構使用 (Private)_呼叫指定模組內的指定函數功能

        #region 架構使用 (Private)_定時顯示

        //v0.0.7.9 By Sanxiu 主頁面破刀數值
        private float fBarMin = 1.0f;
        private float fBarMax = 100f;
        private float fValMax = 5.0f;
        private float fValMid = 1.5f;

        //v0.0.7.22 By Sanxiu 修正破刀率現示設定
        private float fGetVal = 0.0f;
        private float fFuncA = 0.0f;
        private float fFuncB = 0.0f;
        private float fFuncC = 0.0f;

        //v0.0.7.22 By Sanxiu 修正破刀率現示設定
        /// <summary>
        /// 二次曲線方程式
        /// y=ax^2+bx+c
        /// 找出a、b、c
        /// </summary>
        public void CalKufBreakFuncConst()
        {
            float fValMin = (float)CallProc("Saw", "GetKufBreak_MinValue");
            float fBarMid = (float)CallProc("Saw", "GetKufBreak_Percen");
            fValMax = (float)CallProc("Saw", "GetKufBreak_MaxValue");

            float iX1 = fValMid - fValMin;
            float IX1P = fValMid * fValMid - fValMin * fValMin;
            float iY1 = fBarMid - fBarMin;
            float iX2 = fValMax - fValMin;
            float IX2P = fValMax * fValMax - fValMin * fValMin;
            float iY2 = fBarMax - fBarMin;

            fFuncB = ((iY2 * IX1P) - (IX2P * iY1)) / (iX2 * IX1P - IX2P * iX1);
            fFuncA = (iY1 - fFuncB * iX1) / IX1P;
            fFuncC = fBarMin - fValMin * (fFuncA * fValMin + fFuncB);
        }

        //v0.0.7.12 By Sanxiu 雙螢幕鎖定
        private bool bStateFront = true;
        private bool bStateBack = true;

        /// <summary>
        /// //v0.0.7.22 By Sanxiu 修正破刀率現示設定
        /// 取得當下破刀數值的百分比值
        /// </summary>
        /// <param name="fIn"></param>
        /// <returns></returns>
        private float GetPercentage(float fIn)
        {
            float fresult = 0.0f;

            //二次曲線方程式
            //y=ax^2+bx+c
            fresult = fFuncA * fIn * fIn + fFuncB * fIn + fFuncC;

            if (fresult > fBarMax)
            {
                //大於Bar的最大值強值改成最大值
                fresult = fBarMax;
            }

            if (fresult < 1)
            {
                //小於Bar的最小值強制改成1
                fresult = 1.0f;
            }

            return fresult;
        }

        private void ScanIO()
        {
            #region 動作函數_掃描硬體按鈕IO_架構使用 (處理MAA的硬體按鈕IO對應的動作)

            if ((SYSPara.ManualControlIO) && (SYSPara.RunMode != RunModeDT.MANUAL))
                return;


            int result = ((BaseModuleInterface)FormSet.mMainFlow.mMAA).ScanIO();

            //Start Button
            if ((result & 0x01) == 1)
            {
                btnStart_Click(new object(), new EventArgs());
            }
            //Stop Button
            if (((result >> 1) & 0x01) == 1)
            {
                btnPause_Click(new object(), new EventArgs());
            }
            //Alarm Result Button
            if (((result >> 2) & 0x01) == 1)
            {
                btnReset_Click(new object(), new EventArgs());
            }

            #endregion 動作函數_掃描硬體按鈕IO_架構使用 (處理MAA的硬體按鈕IO對應的動作)
        }

        #region Gauge relation

        private void InitGauge(AGauge aGauge, float fMinVal, float fMaxVal)
        {
            AGaugeRange redRegion = aGauge.GaugeRanges.FindByName("RedRegion");
            redRegion.Color = Color.Transparent;

            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.Color = Color.Transparent;
        }

        private void InitGauge(AGauge aGauge, float fThreshold)
        {
            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.Color = Color.LightGreen;
            greenRegion.StartValue = 0;
            greenRegion.EndValue = fThreshold;
        }

        private void SetWaterGauge(AGauge aGauge, float fThreshold, float fTolerance, float fMaxFlow)
        {
            AGaugeRange redRegion = aGauge.GaugeRanges.FindByName("RedRegion");
            redRegion.Color = Color.Red;
            redRegion.StartValue = 0;
            redRegion.EndValue = fMaxFlow;

            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.Color = Color.LightGreen;
            greenRegion.StartValue = fThreshold - fTolerance;
            greenRegion.EndValue = fThreshold + fTolerance;
            //greenRegion.EndValue = fMaxFlow;
        }

        private void SetSpindleSpdGauge(AGauge aGauge, int nSpindleRPM)
        {
            AGaugeRange redRegion = aGauge.GaugeRanges.FindByName("RedRegion");
            redRegion.Color = Color.Red;
            redRegion.StartValue = 0;
            redRegion.EndValue = 30000;

            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.Color = Color.LightGreen;
            greenRegion.StartValue = (float)nSpindleRPM - Convert.ToInt32(nSpindleRPM * 0.05);
            greenRegion.EndValue = (float)nSpindleRPM + Convert.ToInt32(nSpindleRPM * 0.05);
        }

        private void SetVacuumGauge(AGauge aGauge, float fThreshold)
        {
            AGaugeRange redRegion = aGauge.GaugeRanges.FindByName("RedRegion");
            redRegion.Color = Color.Red;
            redRegion.StartValue = -100;
            redRegion.EndValue = 0;

            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.Color = Color.LightGreen;
            greenRegion.StartValue = -100;
            greenRegion.EndValue = fThreshold;
        }

        private void SetAgaugeRanges(AGauge aGauge, double dGreenMin, double dGreenMax)
        {
            AGaugeRange redRegion = aGauge.GaugeRanges.FindByName("RedRegion");
            redRegion.StartValue = aGauge.MinValue;
            redRegion.EndValue = aGauge.MaxValue;

            AGaugeRange greenRegion = aGauge.GaugeRanges.FindByName("GreenRegion");
            greenRegion.StartValue = (float)dGreenMin;
            greenRegion.EndValue = (float)dGreenMax;
        }

        #endregion

        private void tmUIRefresh_Tick(object sender, EventArgs e)
        {
            tmUIRefresh.Enabled = false;

            #region 架構使用 (Private)_定時顯示_Scofield_v4.0.1.82[修改]儀錶板可隨Package設定而變更允許值範圍

            SetAgaugeRanges(aGauge_主入力空壓值,
                    (double)SYSPara.CallProc("MAA", "GetMainAirLimit", 0),
                    (double)SYSPara.CallProc("MAA", "GetMainAirLimit", 1));

            SetAgaugeRanges(aGauge_主入力水壓值,
                (double)SYSPara.CallProc("Saw", "GetWaterPressureLimit", 0),
                (double)SYSPara.CallProc("Saw", "GetWaterPressureLimit", 1));

            SetAgaugeRanges(aGauge_主軸空壓值,
               (double)SYSPara.CallProc("Saw", "GetSpindleAirPressureLimit", 0),
               (double)SYSPara.CallProc("Saw", "GetSpindleAirPressureLimit", 1));

            if (SYSPara.PackageName.Length == 0)
            {
                InitGauge(aGauge_Z1主軸轉速值, 0, 30000);
                InitGauge(aGauge_Z2主軸轉速值, 0, 30000);

                InitGauge(aGauge_Z1噴水座流量值, 0, (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));
                InitGauge(aGauge_Z1灑水座流量值, 0, (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 1)));
                InitGauge(aGauge_Z2噴水座流量值, 0, (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));
                InitGauge(aGauge_Z2灑水座流量值, 0, (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 1)));

                InitGauge(aGauge_切割平台真空值, -100, 0);

                InitGauge(aGauge_Z1破刀偵測值, (float)((double)SYSPara.CallProc("Saw", "GetCutBreakThreshold")));
                InitGauge(aGauge_Z2破刀偵測值, (float)((double)SYSPara.CallProc("Saw", "GetCutBreakThreshold")));
            }
            else
            {
                SetSpindleSpdGauge(aGauge_Z1主軸轉速值, SYSPara.PReadValue("Spndle_rpm").ToInt());
                SetSpindleSpdGauge(aGauge_Z2主軸轉速值, SYSPara.PReadValue("Spndle_rpm").ToInt());

                SetWaterGauge(aGauge_Z1噴水座流量值,
                    (float)SYSPara.PReadValue("Shower_value").ToDouble(),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowTolerance")),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));

                SetWaterGauge(aGauge_Z1灑水座流量值, 
                    (float)SYSPara.PReadValue("Spray_value").ToDouble(),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowTolerance")),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));

                SetWaterGauge(aGauge_Z2噴水座流量值, 
                    (float)SYSPara.PReadValue("Shower_value").ToDouble(),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowTolerance")),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));

                SetWaterGauge(aGauge_Z2灑水座流量值, 
                    (float)SYSPara.PReadValue("Spray_value").ToDouble(),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowTolerance")),
                    (float)((double)SYSPara.CallProc("Saw", "GetWaterFlowMax", 0)));

                // 切割平台真空值儀表板
                SetVacuumGauge(aGauge_切割平台真空值, (float)SYSPara.PReadValue("PCBVacuumThreshold").ToDouble());
            }

            #endregion 架構使用 (Private)_定時顯示_Scofield_v4.0.1.82[修改]儀錶板可隨Package設定而變更允許值範圍

            #region 架構使用 (Private)_定時顯示_使用者修改

            ScanIO();   //v4.0.1.41 實體按鈕對應軟體click事件

            if (SYSPara.RunMode == RunModeDT.AUTO)
            {
                ShowToolData();
            }
            if (FormSet.mMainFlow.mSaw != null)
            {
                #region 儀表板相關
                if (tabMachineState.SelectedIndex == 2)
                {
                    #region 主入力水氣
                    if (tabCtrlGauges.SelectedIndex == 0)
                    {
                        //主入力空壓值
                        aGauge_主入力空壓值.Value = Convert.ToSingle((double)SYSPara.CallProc("MAA", "GetMainAIValue"));
                        aGauge_主入力空壓值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_主入力空壓值.Value.ToString("#0.00");

                        //主入力空壓值
                        aGauge_主入力水壓值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetWaterInPressAIValue"));
                        aGauge_主入力水壓值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_主入力水壓值.Value.ToString("#0.00");
                    }
                    #endregion

                    #region 主軸保護
                    if (tabCtrlGauges.SelectedIndex == 1)
                    {
                        //主軸空壓值
                        aGauge_主軸空壓值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetSpindleAirPressureAIValue"));
                        aGauge_主軸空壓值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_主軸空壓值.Value.ToString("#0.00");
                    }
                    #endregion

                    #region 主軸資訊
                    if (tabCtrlGauges.SelectedIndex == 2)
                    {
                        //v4.0.1.28 讀取主軸轉速、馬達負載、驅動器負載
                        Random rnd = new Random(Guid.NewGuid().GetHashCode());//轉速的浮動值 

                        switch (FormSet.mMainFlow.mSaw.SReadValue("dRadioGroupBox_Z1主軸控制方式選擇").ToInt())
                        {
                            case 0:
                            case 1:
                                //Z1主軸運轉值
                                aGauge_Z1主軸轉速值.Value = Convert.ToInt32((double)SYSPara.CallProc("Saw", "GetSpindleZ1Speed"));
                                aGauge_Z1主軸轉速值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1主軸轉速值.Value.ToString();

                                //Z1主軸負載值
                                aGauge_Z1主軸負載值.Value = (float)SYSPara.CallProc("Saw", "GetAnalogInLoadValue", "Z1");
                                aGauge_Z1主軸負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1主軸負載值.Value.ToString("0.0");

                                //Z1驅動器負載值
                                aGauge_Z1驅動器負載值.Value = (float)SYSPara.CallProc("Saw", "GetAnalogInLoadValue", "Z1");
                                aGauge_Z1驅動器負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1驅動器負載值.Value.ToString("0.0");
                                break;
                            case 2:
                                //Z1主軸運轉值
                                aGauge_Z1主軸轉速值.Value = Convert.ToInt32((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z1", "RPM")) + rnd.Next(-10, 10);
                                aGauge_Z1主軸轉速值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1主軸轉速值.Value.ToString();

                                //Z1主軸負載值
                                aGauge_Z1主軸負載值.Value = Convert.ToSingle((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z1", "MotorLoad"));
                                aGauge_Z1主軸負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1主軸負載值.Value.ToString("0.0");

                                //Z1驅動器負載值
                                aGauge_Z1驅動器負載值.Value = Convert.ToSingle((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z1", "AMPLoad"));
                                aGauge_Z1驅動器負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1驅動器負載值.Value.ToString("0.0");
                                break;
                        }
                        //v4.0.1.28 讀取主軸轉速、馬達負載、驅動器負載
                        switch (FormSet.mMainFlow.mSaw.SReadValue("dRadioGroupBox_Z2主軸控制方式選擇").ToInt())
                        {
                            case 0:
                            case 1:
                                //Z2主軸運轉值
                                aGauge_Z2主軸轉速值.Value = Convert.ToInt32((double)SYSPara.CallProc("Saw", "GetSpindleZ2Speed"));
                                aGauge_Z2主軸轉速值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2主軸轉速值.Value.ToString();

                                //Z2主軸負載值
                                aGauge_Z2主軸負載值.Value = (float)SYSPara.CallProc("Saw", "GetAnalogInLoadValue", "Z2");
                                aGauge_Z2主軸負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2主軸負載值.Value.ToString("0.0");

                                //Z2驅動器負載值
                                aGauge_Z2驅動器負載值.Value = (float)SYSPara.CallProc("Saw", "GetAnalogInLoadValue", "Z2");
                                aGauge_Z2驅動器負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2驅動器負載值.Value.ToString("0.0");
                                break;
                            case 2:
                                //Z1主軸運轉值
                                aGauge_Z2主軸轉速值.Value = Convert.ToInt32((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z2", "RPM")) + rnd.Next(-10, 10);
                                aGauge_Z2主軸轉速值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2主軸轉速值.Value.ToString();

                                //Z1主軸負載值
                                aGauge_Z2主軸負載值.Value = Convert.ToSingle((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z2", "MotorLoad"));
                                aGauge_Z2主軸負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2主軸負載值.Value.ToString("0.0");

                                //Z1驅動器負載值
                                aGauge_Z2驅動器負載值.Value = Convert.ToSingle((string)SYSPara.CallProc("Saw", "GetSpindleInfo", "Z2", "AMPLoad"));
                                aGauge_Z2驅動器負載值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2驅動器負載值.Value.ToString("0.0");
                                break;
                        }
                    }
                    #endregion

                    #region 切割資訊
                    if (tabCtrlGauges.SelectedIndex == 3)
                    {
                        //Z1噴水座流量值
                        aGauge_Z1噴水座流量值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetZ1BladeCoolerAIValue", true));
                        aGauge_Z1噴水座流量值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1噴水座流量值.Value.ToString("0.00");

                        //Z2噴水座流量值
                        aGauge_Z2噴水座流量值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetZ2BladeCoolerAIValue", true));
                        aGauge_Z2噴水座流量值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2噴水座流量值.Value.ToString("0.00");

                        //Z1灑水座流量值
                        aGauge_Z1灑水座流量值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetZ1BladeSprayAIValue", true));
                        aGauge_Z1灑水座流量值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1灑水座流量值.Value.ToString("0.00");

                        //Z2灑水座流量值
                        aGauge_Z2灑水座流量值.Value = Convert.ToSingle((double)SYSPara.CallProc("Saw", "GetZ2BladeSprayAIValue", true));
                        aGauge_Z2灑水座流量值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2灑水座流量值.Value.ToString("0.00");

                        //Z1破刀偵測值
                        aGauge_Z1破刀偵測值.Value = (float)SYSPara.CallProc( "Saw", "GetAnalogIn_AvgValue", "Z1");
                        aGauge_Z1破刀偵測值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z1破刀偵測值.Value.ToString("0.00");

                        //Z2破刀偵測值
                        aGauge_Z2破刀偵測值.Value = (float)SYSPara.CallProc( "Saw", "GetAnalogIn_AvgValue", "Z2");
                        aGauge_Z2破刀偵測值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_Z2破刀偵測值.Value.ToString("0.00");

                        //切割平台真空值
                        aGauge_切割平台真空值.Value = (float)((double)SYSPara.CallProc("Saw", "GetChuckTableAIValue"));
                        aGauge_切割平台真空值.GaugeLabels.FindByName("CurrentValLbl").Text = aGauge_切割平台真空值.Value.ToString("0.00");

                        #region 透光度
                        
                        fGetVal = (float)SYSPara.CallProc("Saw", "GetAnalogIn_Z1_avgValueBar");
                        fGetVal += 0.001f;

                        //+ By Max 20211030
                        if (SYSPara.IsUseDAQ)
                        {
                            labValueZ1.Text = ((int)fGetVal).ToString() + "%";
                        }
                        else
                        {
                            if (fGetVal > fValMax)
                            {
                                labValueZ1.Text = ((int)fBarMax).ToString() + "%";
                            }
                            else
                            {
                                fGetVal = GetPercentage(fGetVal);
                                labValueZ1.Text = ((int)fGetVal).ToString() + "%";
                            }
                        }
                        

                        fGetVal = (float)SYSPara.CallProc("Saw", "GetAnalogIn_Z2_avgValueBar");
                        fGetVal += 0.01f;

                        //+ By Max 20211030
                        if (SYSPara.IsUseDAQ)
                        {
                            labValueZ2.Text = ((int)fGetVal).ToString() + "%";
                        }
                        else
                        {
                            if (fGetVal > fValMax)
                            {
                                labValueZ2.Text = ((int)fBarMax).ToString() + "%";
                            }
                            else
                            {
                                fGetVal = GetPercentage(fGetVal);
                                labValueZ2.Text = ((int)fGetVal).ToString() + "%";
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion 儀表板相關

                numLabelZ1Ncs.Value = string.Format("{0:0.000}", (float)SYSPara.CallProc("Saw", "GetAnalogIn_NC_AvgValue", "Z1"));
                numLabelZ2Ncs.Value = string.Format("{0:0.000}", (float)SYSPara.CallProc("Saw", "GetAnalogIn_NC_AvgValue", "Z2"));

                //載台真空
                lbVacaValue_Chuck1.Value = string.Format("{0:0.0}", (double)SYSPara.CallProc("Saw", "GetChuckTableAIValue"));

                //v0.0.7.12 By Sanxiu 主畫面顯示即時進給速度
                numLabel_FeedSpeed.Value = string.Format("{0:0}", (double)SYSPara.CallProc("Saw", "GetNowFeedSpeed"));

                //入料
                label_入料片數.Text = string.Format("{0}", (int)SYSPara.CallProc("Saw", "GetPCBIn"));

                //出料
                label_出料片數.Text = string.Format("{0}", (int)SYSPara.CallProc("Saw", "GetPCBOut"));

                //UPH 片/hr
                label_UPH1.Text = string.Format("{0:0}", Convert.ToSingle((int)SYSPara.CallProc("Saw", "GetPCBOut")) / SYSPara.RunSecond * 3600);

                //UPH 顆/hr
                label_UPH2.Text = string.Format("{0:0}", Convert.ToSingle((int)SYSPara.CallProc("Saw", "GetPCBOut"))
                    / SYSPara.RunSecond * 3600
                    * FormSet.mMainFlow.mSaw.PReadValue("Num_col").ToInt()
                    * FormSet.mMainFlow.mSaw.PReadValue("Num_row").ToInt());

                //機台編號
                textLabel1.Value = SYSPara.MachineID;

                //視覺料號
                sVisionNameLab.Value = FormSet.mMainFlow.mSaw.PReadValue("視覺軟體程式名稱").ToString();
            }

            #endregion 架構使用 (Private)_定時顯示_使用者修改

            #region 架構使用 (Private)_定時顯示_顯示目前時間

            lbNowTime.Text = DateTime.Now.ToString("G");

            lbIdleTM.Value = UserSnippet.ToTimeString(SYSPara.IdleTM);
            lbHomeTM.Value = UserSnippet.ToTimeString(SYSPara.HomeTM);
            lbManualTM.Value = UserSnippet.ToTimeString(SYSPara.ManualTM);

            #endregion 架構使用 (Private)_定時顯示_顯示目前時間

            #region 架構使用_定時顯示_顯示運行時相關資料

            if (SYSPara.RunMode == RunModeDT.AUTO)
            {
                lbRunTM.Value = UserSnippet.ToTimeString(SYSPara.RunSecond);
                lbPauseTM.Value = UserSnippet.ToTimeString(SYSPara.StopSecond);
            }

            lbScanTM.Value = string.Format("{0:0.000}", SYSPara.ScanTimeEx);
            lbScanCNT.Value = SYSPara.ScanTime.ToString();

            #endregion 架構使用_定時顯示_顯示運行時相關資料

            #region 架構使用_定時顯示_設備狀態顯示

            DisplaySysState();

            #endregion 架構使用_定時顯示_設備狀態顯示

            #region 架構使用_定時顯示_顯示三色燈LED

            if ((bool)SYSPara.CallProc("MAA", "ScanRedLight"))
                ledRed.Value = KCSDK.ThreeColorLight.ColorLightType.RedLight;
            else
                ledRed.Value = KCSDK.ThreeColorLight.ColorLightType.GrayLight;
            if ((bool)SYSPara.CallProc("MAA", "ScanYellowLight"))
                ledYellow.Value = KCSDK.ThreeColorLight.ColorLightType.YellowLight;
            else
                ledYellow.Value = KCSDK.ThreeColorLight.ColorLightType.GrayLight;
            if ((bool)SYSPara.CallProc("MAA", "ScanGreenLight"))
                ledGreen.Value = KCSDK.ThreeColorLight.ColorLightType.GreenLight;
            else
                ledGreen.Value = KCSDK.ThreeColorLight.ColorLightType.GrayLight;

            #endregion 架構使用_定時顯示_顯示三色燈LED

            #region 異常動作記錄
            if (bAutoSaveStep)   //v0.0.7.11 By Sanxiu 自動記錄，執行MainRun發生例外
            {
                bAutoSaveStep = false;
                SYSPara.CallProc("Saw", "SaveAllSawStepIndex");
            }
            #endregion 異常動作記錄

            #region 連線狀態燈號顯示
            ledSocketConnect.Value = (bool)CallProc("Saw", "IsConnected");
            #endregion

            #region Handler入出料狀態燈號顯示
            ledCanPut.Value = (bool)CallProc("Saw", "ShowPutState");
            ledCanGet.Value = (bool)CallProc("Saw", "ShowGetState");
            #endregion

            //20180608 v0.0.7.4 出入料IO防呆
            if (SYSPara.SysState == StateMode.SM_AUTO_RUN)
            {
                //機器動作自動判斷
                CallProc("Saw", "SetPutRunSata", true);
                CallProc("Saw", "SetGetRunSata", true);
                CallProc("Saw", "SetFrontGetRun", true);   //v0.0.7.12 By Sanxiu 出料提早偷跑
            }
            else
            {
                //當機器為非Auto狀況態時，強制設為false
                CallProc("Saw", "SetPutRunSata", false);
                CallProc("Saw", "SetGetRunSata", false);
                CallProc("Saw", "SetFrontGetRun", false);   //v0.0.7.12 By Sanxiu 出料提早偷跑
            }

            #region 雙螢幕
            if (SYSPara.UseDoubleScreen)   //v0.0.7.12 By Sanxiu 雙螢幕鎖定
            {
                CallProc("MAA", "InputLock");
            }
            #endregion 雙螢幕

            //v4.0.1.23 執行顯示黃色，暫停顯示灰色
            Color teachtimercolor = Color.Transparent;
            if ((bool)CallProc("Saw", "GetTeachTimer"))
            {
                teachtimercolor = Color.Yellow;
            }
            else
            {
                teachtimercolor = Color.Transparent;
            }

            switch ((int)CallProc("Saw", "GetTeachActionNo"))
            {
                case 0:
                    button_接觸測高_執行.BackColor = Color.Transparent;
                    button_非接觸測高_執行.BackColor = Color.Transparent;
                    button_基準線校正_執行.BackColor = Color.Transparent;
                    button_切割道學習_執行.BackColor = Color.Transparent;
                    button_靶點學習_執行.BackColor = Color.Transparent;
                    button_換刀流程_執行.BackColor = Color.Transparent;
                    button_圓心校正_執行.BackColor = Color.Transparent;
                    break;
                case 1:
                    button_接觸測高_執行.BackColor = teachtimercolor;
                    break;
                case 2:
                    button_非接觸測高_執行.BackColor = teachtimercolor;
                    break;
                case 3:
                    button_基準線校正_執行.BackColor = teachtimercolor;
                    break;
                case 4:
                    button_切割道學習_執行.BackColor = teachtimercolor;
                    break;
                case 5:
                    button_靶點學習_執行.BackColor = teachtimercolor;
                    break;
                case 6:
                    button_換刀流程_執行.BackColor = teachtimercolor;
                    break;
                case 7:
                    button_圓心校正_執行.BackColor = teachtimercolor;
                    break;
            }

            // 換刀區安全門是否開啟
            CallProc("Saw", "SetCutDoorSafety", (bool)CallProc("MAA", "IsBackDoorOpen"));

            // 當綠燈運行中或歸零時，前安全門鎖自動開啟
            if ((SYSPara.RunMode == RunModeDT.AUTO && SYSPara.SysState == StateMode.SM_AUTO_RUN) ||
                (SYSPara.RunMode == RunModeDT.HOME && SYSPara.SysState == StateMode.SM_INIT_RUN))
            {
                if ((bool)CallProc("MAA", "IsFrontDoorOpen") && !FormSet.mMainFlow.mMAA.SReadValue("安全門不偵測").ToBoolean())
                {
                    CallProc("Saw", "SetFrontDoorLock", true);
                }
            }
            else
            {
                if (SYSPara.RunMode != RunModeDT.MANUAL)
                {
                    CallProc("Saw", "SetFrontDoorLock", false);
                }
            }

            CallProc("Saw", "SetUIVisible", 
                SYSPara.LoginUser == UserType.utAdministrator || SYSPara.LoginUser == UserType.utProV);

            DrawCutLine();

            lblZ1XPos.Text = SYSPara.CallProc("Saw", "GetMotorPos", "X1").ToString();
            lblZ2XPos.Text = SYSPara.CallProc("Saw", "GetMotorPos", "X2").ToString();
            lblYPos.Text   = SYSPara.CallProc("Saw", "GetMotorPos", "Y").ToString();
            lblZ1Pos.Text  = SYSPara.CallProc("Saw", "GetMotorPos", "Z1").ToString();
            lblZ2Pos.Text  = SYSPara.CallProc("Saw", "GetMotorPos", "Z2").ToString();
            lblUPos.Text   = SYSPara.CallProc("Saw", "GetMotorPos", "U").ToString();

            tmUIRefresh.Enabled = true;
        }

        #endregion 架構使用 (Private)_定時顯示

        #endregion 架構使用 (Private)

        #region 架構使用 (按鈕功能)

        private void btnStart_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「生產開始」");

            //如果載入料號
            if (SYSPara.PackageName != "")
            {
                bool CanAuto = true;
                string message = "";

                if (SYSPara.RunMode == RunModeDT.AUTO && !(bool)SYSPara.CallProc("Saw", "GetCheckBeforeStartFinish"))
                {
                    message = "生產前教導未完成";
                    CanAuto = false;
                }

                #region 磨耗值安全計算

                SYSPara.CallProc("Saw", "BladeRefresh");

                strKnifeData Data_1 = (strKnifeData)SYSPara.CallProc("Saw", "Tool1InfoGet");
                strKnifeData Data_2 = (strKnifeData)SYSPara.CallProc("Saw", "Tool2InfoGet");

                int FlangeLimit = FormSet.mMainFlow.mSaw.SReadValue("dFlandeLimit").ToInt();
                int SPCKnifeRemain_1 = (int)Data_1.dRealKnifeRemain;
                int SPCKnifeRemain_2 = (int)Data_2.dRealKnifeRemain;

                int PCBDepth = FormSet.mMainFlow.mSaw.PReadValue("PCBThickness").ToInt();
                int HeightG = FormSet.mMainFlow.mSaw.PReadValue("RubberThickness").ToInt();
                int SafeHeightH = FormSet.mMainFlow.mSaw.SReadValue("SafeHeightH").ToInt();

                DataTable DA = (DataTable)SYSPara.CallProc("Saw", "Get_CutData");

                int Raise = SafeHeightH;

                if (DA.Rows.Count > 0)//提刀高度
                    Raise = Convert.ToInt32(DA.Rows[0]["Cut_高度偏移"].ToString());

                if (Raise <= SafeHeightH) // 提刀高度需大於安全高度
                {
                    message = string.Format("刀序資料-提刀高度[" + Raise.ToString() + "]<=" + SafeHeightH.ToString() + "，請檢查");

                    CanAuto = false;
                }
                //磨耗值限制 = 實際刀露量 - ( 膠墊高度 - 高度偏移 ) - 料片厚度 - 料片與法蘭最近距離
                if (SPCKnifeRemain_1 - HeightG + Raise - PCBDepth - FlangeLimit <= 0)
                {
                    message = string.Format("不可執行，Z1磨耗量已達極限，公式:實際刀露量[{0}]-" +

                    "膠墊高度[{1}]+提刀高度[{2}]-料片厚度[{3}]-料片與法蘭最近距離[{4}]",
                    SPCKnifeRemain_1, HeightG, Raise, PCBDepth, FlangeLimit);

                    CanAuto = false;
                }
                if (SPCKnifeRemain_2 - HeightG + Raise - PCBDepth - FlangeLimit <= 0)
                {
                    message = string.Format("不可執行，Z2磨耗量已達極限，公式:實際刀露量[{0}]-" +

                    "膠墊高度[{1}]+提刀高度[{2}]-料片厚度[{3}]-料片與法蘭最近距離[{4}]",
                    SPCKnifeRemain_2, HeightG, Raise, PCBDepth, FlangeLimit);

                    CanAuto = false;
                }
                #endregion

                switch ((int)SYSPara.CallProc("Saw", "GetTeachActionNo"))
                {
                    case 0:
                        break;
                    case 1:
                        message = "請先完成接觸測高後才能繼續";
                        CanAuto = false;
                        break;
                    case 2:
                        message = "請先完成非接觸測高後才能繼續";
                        CanAuto = false;
                        break;
                    case 3:
                        message = "請先完成基準線校正後才能繼續";
                        CanAuto = false;
                        break;
                    case 4:
                        message = "請先完成切割道學習後才能繼續";
                        CanAuto = false;
                        break;
                    case 5:
                        message = "請先完成靶點學習後才能繼續";
                        CanAuto = false;
                        break;
                    case 6:
                        message = "請先完成換刀流程高後才能繼續";
                        CanAuto = false;
                        break;
                    case 7:
                        message = "請先完成圓心校正後才能繼續";
                        CanAuto = false;
                        break;
                }

                if (SYSPara.RunMode == RunModeDT.AUTO)
                {
                    if (SYSPara.PackageStop == false)   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    {
                        SYSPara.Alarm.Say("E9107");
                        return;
                    }

                    if (SYSPara.OptionStop == false)   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    {
                        SYSPara.Alarm.Say("E9106");
                        return;
                    }
                }

                if (!CanAuto)
                {
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", message);
                    //MessageBox.Show(message);
                    return;
                }
                else if (SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME || SYSPara.RunMode == RunModeDT.MANUAL)
                {
                    CloseTeachorAuto();
                    SYSPara.ErrorStop = false;
                    SYSPara.MusicOn = true;
                    SYSPara.Alarm.ClearAll();
                    SYSPara.SysRun = true;
                    SysMode = Mode.Auto;
                }
            }
            else if (SYSPara.RunMode == RunModeDT.HOME || SYSPara.RunMode == RunModeDT.MANUAL)
            {
                SYSPara.ErrorStop = false;
                SYSPara.MusicOn = true;
                SYSPara.Alarm.ClearAll();
                SYSPara.SysRun = true;
            }
            else
            {
                //MessageBox.Show("請載入料號並歸零");
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_LoadPackage_And_Home"));
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {

            if (SYSPara.SysState == StateMode.SM_ALARM)
            {
                SYSPara.ErrorStop = false;
                SYSPara.SysState = StateMode.SM_PAUSE;
            }

            //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
            if (String.IsNullOrEmpty(SYSPara.PackageName) == false)
            {
                if ((bool)CallProc("Saw", "GetAutoReMoveToRunPos"))
                {
                    iGetReMovetmrStep = 0;
                    GetReMovetmr.Enabled = true;
                }
            }

            SYSPara.MusicOn = false;
            FormSet.mMSS.M_Stop();

            SYSPara.SysRun = false;

            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「生產暫停」");
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (SYSPara.IsAutoMode)
            {
                LotendSelectForm mlotendsel = new LotendSelectForm();
                DialogResult ret = mlotendsel.ShowDialog();
                switch (ret)
                {
                    case System.Windows.Forms.DialogResult.OK: //結批
                        {
                            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結批」");
                            SYSPara.LotendOk = false;
                            SYSPara.Lotend = true;
                            SYSPara.Alarm.Say("I0504"); //結批開始
                        }
                        break;
                    case System.Windows.Forms.DialogResult.Cancel: //取消
                        SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結批歸零取消」");
                        break;
                    case System.Windows.Forms.DialogResult.Abort: //強制歸零
                        {
                            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「強制歸零」");

                            SYSPara.SysState = StateMode.SM_INIT_RESET;

                            AllFormHide();
                            //把Initial Form 載入到 pnlContainer
                            pnlContainer.Controls.Add(FormSet.mInitialF);
                            FormSet.mInitialF.WindowState = FormWindowState.Maximized;
                            FormSet.mInitialF.Reset();
                            FormSet.mInitialF.Show();

                            //把 Control Button 設為不可按
                            SetControlSW(ControlButtonType.Initial);
                        }
                        break;
                }
            }
            else
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「強制歸零」");

                SYSPara.SysState = StateMode.SM_INIT_RESET;

                AllFormHide();
                //把Initial Form 載入到 pnlContainer
                pnlContainer.Controls.Add(FormSet.mInitialF);
                FormSet.mInitialF.WindowState = FormWindowState.Maximized;
                FormSet.mInitialF.Reset();
                FormSet.mInitialF.Show();

                //把 Control Button 設為不可按
                SetControlSW(ControlButtonType.Initial);
            }

        }

        private void btnChangeUser_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("{0} 登出", GetNowUser()));

            //使用者登入
            UserLogin();

            if (SYSPara.LoginUser == UserType.utOperator)
            {
                FormSet.mOption.ExitClick();
                FormSet.mPackage.ExitClick();
                FormSet.mModuleContainer.ExitClick();
            }
            else
                FormSet.mModuleContainer.PerformClick();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「離開程式」");

            if (!SYSPara.SysRun)
            {
                if (SYSPara.UseDoubleScreen && SYSPara.Simulation == 0)   //v0.0.7.12 By Sanxiu 雙螢幕鎖定
                {
                    CallProc("MAA", "AllUnlockHID");
                }

                //DialogResult result = MessageBox.Show("是否要離開程式?", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                DialogResult result = dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Notice!", GetDialogMsgText("Text_Main3_Exit_Program_Check"));
                switch (result)
                {
                    case DialogResult.OK:
                        if (SYSPara.RunMode == RunModeDT.MANUAL)
                            SYSPara.SysState = StateMode.SM_CANCEL;

                        //關閉主軸
                        int nZ1ActualSpeed = Convert.ToInt32(CallProc("Saw", "GetSpindleInfo", "Z1", "RPM"));
                        int nZ2ActualSpeed = Convert.ToInt32(CallProc("Saw", "GetSpindleInfo", "Z2", "RPM"));
                        if (nZ1ActualSpeed >= 7500) CallProc("Saw", "StopSpindle", "Z1");
                        if (nZ2ActualSpeed >= 7500) CallProc("Saw", "StopSpindle", "Z2");

                        //Stopwatch sw = new Stopwatch();
                        //sw.Start();

                        //while (true)
                        //{
                        //    nZ1ActualSpeed = Convert.ToInt32(CallProc("Saw", "GetSpindleInfo", "Z1", "RPM"));
                        //    nZ2ActualSpeed = Convert.ToInt32(CallProc("Saw", "GetSpindleInfo", "Z2", "RPM"));

                        //    if (nZ1ActualSpeed < 7500 && nZ2ActualSpeed < 7500)
                        //    {
                        //        break;
                        //    }

                        //    if (sw.ElapsedMilliseconds > 3000)
                        //    {
                        //        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", "主軸關閉逾時");
                        //        break;
                        //    }
                        //}

                        //sw.Start();

                        //關閉附屬設備
                        CallProc("Saw", "CloseAllAncillary");
                        Thread.Sleep(200);

                        //By Max 20211025, v4.0.94.0
                        SYSPara.Alarm.ClearAll(true);
                        Thread.Sleep(200);

                        //關閉警報系統
                        SYSPara.Alarm.DisposeTH();
                        Thread.Sleep(300);

                        //關閉HMI前的動作
                        SYSPara.HMIClose = true;
                        while (SYSPara.HMIReady) ;

                        //+ By Max 20200204 For LogAnalyzer
                        try
                        {
                            SYSPara.logDB.LogSayDb(SYSPara.RunSecond, SYSPara.HomeTM, SYSPara.IdleTM, SYSPara.ManualTM, SYSPara.StopSecond);
                            SYSPara.logDB.RunBeforeClose();
                        }
                        catch (Exception) { }

                        Close();
                        break;

                    case DialogResult.Cancel:
                        break;
                }
            }
            else
            {
                SYSPara.Alarm.Say("I0501"); //禁止離開
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「警報重置」");
            SYSPara.MusicOn = false;
            SYSPara.ErrorStop = false;
            SYSPara.Alarm.ClearAll();
            if (SYSPara.SysState == StateMode.SM_ALARM || SYSPara.SysState == StateMode.SM_PAUSE)
                SYSPara.SysState = StateMode.SM_PAUSE;
        }

        private void btnResetIDLE_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「IDLE累計時間」歸零");

            SYSPara.IdleTM = 0;
        }

        private void btnResetHome_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「歸零累計時間」歸零");

            SYSPara.HomeTM = 0;
        }

        private void btnResetManual_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「手動操作累計時間」歸零");

            SYSPara.ManualTM = 0;
        }

        #region By Wolf 20210820 v4.0.92.0 增加片數歸零鍵
        private void btnResetPCB_Click(object sender, EventArgs e)
        {
            if (label_入料片數.Text != label_出料片數.Text)
            {
                //MessageBox.Show("片數不同, 無法歸零");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_Main3_In_Out_Equal_Check"));
            }
            else
            {
                label_入料片數.Text = "0";
                label_出料片數.Text = "0";
                SYSPara.CallProc("Saw", "SetPCBIn", 0);
                SYSPara.CallProc("Saw", "SetPCBOut", 0);
            }
        }
        #endregion

        #endregion 架構使用 (按鈕功能)

        #region 公用函數

        public void ShowToolData()
        {
            ShowKnifeData1 = (strKnifeData)SYSPara.CallProc("Saw", "Tool1InfoGet");
            ShowKnifeData2 = (strKnifeData)SYSPara.CallProc("Saw", "Tool2InfoGet");

            maskedTextBoxZ1ToolName.Text = ShowKnifeData1.sToolName;//Z1刀具序號
            maskedTextBoxZ2ToolName.Text = ShowKnifeData2.sToolName;//Z2刀具序號

            maskedTextBoxZ1Type.Text = ShowKnifeData1.sType;//Z1刀具型號
            maskedTextBoxZ2Type.Text = ShowKnifeData2.sType;//Z2刀具型號

            maskedTextBoxZ1Width.Text = (ShowKnifeData1.dWidth * 1000).ToString();//Z1刀寬
            maskedTextBoxZ2Width.Text = (ShowKnifeData2.dWidth * 1000).ToString();//Z2刀寬

            maskedTextBoxZ1KnifeDiameter.Text = ShowKnifeData1.dKnifeDiameter.ToString();//Z1刀外徑
            maskedTextBoxZ2KnifeDiameter.Text = ShowKnifeData2.dKnifeDiameter.ToString();//Z2刀外徑

            maskedTextBoxZ1Flange.Text = ShowKnifeData1.dFlange.ToString();//Z1法蘭外徑
            maskedTextBoxZ2Flange.Text = ShowKnifeData2.dFlange.ToString();//Z2法蘭外徑

            maskedTextBoxZ1RealMotorDistance.Text = ShowKnifeData1.dRealMotorDistance.ToString();//Z1實際里程
            maskedTextBoxZ2RealMotorDistance.Text = ShowKnifeData2.dRealMotorDistance.ToString();//Z2實際里程

            maskedTextBoxZ1Limit.Text = ShowKnifeData1.dLimit.ToString();//Z1里程限制
            maskedTextBoxZ2Limit.Text = ShowKnifeData2.dLimit.ToString();//Z2里程限制

            //Z1
            if ((bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 1))//當前磨耗
            {
                maskedTextBoxZ1RealKnifeRemain.Text = ShowKnifeData1.dRealKnifeRemain.ToString();//實際刀露量
            }
            else
            {
                maskedTextBoxZ1RealKnifeRemain.Text = "尚未完成測高";
            }
            //Z2
            if ((bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 4))//當前磨耗
            {
                maskedTextBoxZ2RealKnifeRemain.Text = ShowKnifeData2.dRealKnifeRemain.ToString();//實際刀露量
            }
            else
            {
                maskedTextBoxZ2RealKnifeRemain.Text = "尚未完成測高";
            }

        }

        #endregion 公用函數

        #region 私有函數

        private void VisionLoad()
        {
            bool bExist = false;
            try
            {
                Process[] process = Process.GetProcessesByName("ADSW11002");
                foreach (var t in process)
                    t.Kill();

                hCCDHandle = IntPtr.Zero;
                if (hCCDHandle == (IntPtr)0)
                {
                    ProcessStartInfo SInfo = new ProcessStartInfo();

                    SInfo.FileName = sCCDPath;
                    SInfo.Arguments = "";
                    Process.Start(SInfo);
                }
                while (hCCDHandle == (IntPtr)0)
                    hCCDHandle = (IntPtr)FindWindow(null, "VISIONSYSTEM");

                if (!bExist)
                    while (!IsWindowVisible(hCCDHandle)) { }

                Thread.Sleep(1000);
                return;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #region 私有函數_2020/02/05

        private bool frm異常通知顯示中 = false;
        private Point 異常通知畫面位置;
        private bool mMouseDown = false;

        #endregion 私有函數_2020/02/05

        #region 私有函數_視覺

        IntPtr hCCDHandle = (IntPtr)0;
        const bool bUseCCD = true;
        string sCCDPath = "D:\\ProVision\\Debug\\ADSW11002.exe";
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int iVisionWidth = 640;
        const int iVisionHeight = 480;
        //Microsoft.VisualBasic.PowerPacks.ShapeContainer oShapeContainer = new Microsoft.VisualBasic.PowerPacks.ShapeContainer();

        #endregion 私有函數_視覺

        #region 私有函數_安全門機制改善

        int iLastActionCode = 0;
        private void SaftyActionDelegate(int iActionCode)
        {
            if (InvokeRequired)
            {
                if (iActionCode == iLastActionCode)
                {
                    return;
                }
                iLastActionCode = iActionCode;
                BeginInvoke(new Action<int>(SaftyActionDelegate), iActionCode);
                return;
            }
            else
            {
                if (iActionCode == iLastActionCode)
                {
                    return;
                }
                iLastActionCode = iActionCode;
            }

            bool 安全門偵測 = (bool)(iActionCode == 1);
            bool 緊停偵測 = (bool)(iActionCode == 2);

            List<string> msglist = new List<string>();
            if (緊停偵測)
            {
                msglist.Add("緊停異常通知");
                msglist.Add("EMG Stop");
            }
            if (安全門偵測)
            {
                msglist.Add("安全門打開");
                msglist.Add("SafeDoor Open");
                SYSPara.安全門開啟 = true;
            }
            if (緊停偵測 || 安全門偵測)
            {
                if (!frm異常通知顯示中)
                {
                    frm異常通知顯示中 = true;
                    string msg = "";
                    foreach (string value in msglist)
                    {
                        msg += value;
                        msg += "\n";
                    }
                    msg = msg.Substring(0, msg.Length - 1);
                    lb異常訊息.Text = msg;

                    pnl異常通知.Width = 900;
                    pnl異常通知.Height = 150 * msglist.Count;
                    pnl異常通知.Left = (Width - pnl異常通知.Width) / 2;
                    pnl異常通知.Top = (Height - pnl異常通知.Height) / 2;
                    pnl異常通知.Visible = true;
                }
            }
            else
            {
                if (frm異常通知顯示中)
                {
                    frm異常通知顯示中 = false;
                    pnl異常通知.Visible = false;
                }
            }
        }

        #endregion 私有函數_安全門機制改善

        //+ By Max 20210225, 內靶掃描
        int LastAlarmCode = 0;
        private void timerTeachFlow_Tick(object sender, EventArgs e)
        {
            timerTeachFlow.Enabled = false;

            string AlarmLevel = (string)SYSPara.CallProc("Saw", "GetTeachAlarmLevel");
            int AlarmCode = (int)SYSPara.CallProc("Saw", "GetTeachAlarmCode");
            string[] Alarmmsg = (string[])SYSPara.CallProc("Saw", "GetTeachAlarmmsg");
            ArmMtrl t = SYSPara.Alarm.AlarmTable.Find(x => x.AlarmLevel == AlarmLevel && x.ErrorCode == AlarmCode && x.ModuleID == 50);
            string teachmsg = "";
            if (t != null)
            {
                switch (SYSPara.Lang.GetNowLanguage())
                {
                    case "tw":
                        //v4.0.1.32 [修改]教導流程訊息能傳入變數
                        if (Alarmmsg != null)
                            teachmsg = string.Format(t.LanCHT.Explain, Alarmmsg);
                        break;
                    case "en":
                        //v4.0.1.32 [修改]教導流程訊息能傳入變數
                        if (Alarmmsg != null)
                            teachmsg = string.Format(t.LanENG.Explain, Alarmmsg);
                        break;
                }
            }

            if (FormSet.mMainFlow.mSaw != null)
            {
                if (FormSet.mMainFlow.MainflowHomeReset)
                {
                    CloseTeachorAuto();
                    SysMode = Mode.Idle;
                    FormSet.mMainFlow.MainflowHomeReset = false;
                }

                //+ By Max 20210309, 是否顯示動態刀痕CheckBox
                chkDynamicCutOffset.Visible = (bool)SYSPara.CallProc("Saw", "CanShowDynamicCutOffsetCheckBox");

                #region 私有函數_TimerTeachFlow_tpTeach btnOpen

                if (SYSPara.PackageName != "")
                {
                    pnlZ1ST.BackColor    = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 1) ? Color.LimeGreen : Color.LightSalmon;
                    pnlZ1NCT.BackColor   = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 2) ? Color.LimeGreen : Color.LightSalmon;
                    pnlZ1Kerf.BackColor  = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 3) ? Color.LimeGreen : Color.LightSalmon;
                    pnlZ2ST.BackColor    = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 4) ? Color.LimeGreen : Color.LightSalmon;
                    pnlZ2NCT.BackColor   = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 5) ? Color.LimeGreen : Color.LightSalmon;
                    pnlZ2Kerf.BackColor  = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 6) ? Color.LimeGreen : Color.LightSalmon;
                    pnlFixture.BackColor = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 7) ? Color.LimeGreen : Color.LightSalmon;
                    pnlPCB.BackColor     = (bool)SYSPara.CallProc("Saw", "GetcheckBeforeStartStatus", 8) ? Color.LimeGreen : Color.LightSalmon;

                    //20201020 Woody 測試基準線校正
                    panel_控制.Enabled = CanControlMotor;

                }

                #endregion 私有函數_TimerTeachFlow_tpTeach btnOpen

                #region 私有函數_TimerTeachFlow_CheckSparkTeachStatus

                ledSparkTest.Value = CheckSparkTeachStatus;
                if (CheckSparkTeachStatus)
                {
                    TestSpeakLed.Value = (bool)SYSPara.CallProc("Saw", "GetSpark") ? LEDState.On : LEDState.Off;
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {
                        #region 私有函數_TimerTeachFlow_CheckSparkTeachStatus_依流程控制UI致能

                        case 0://提示
                            label_接觸測高數值.Text = teachmsg;
                            button_接觸測高_執行.Enabled = true;
                            //button_接觸測高_更換測高塊.Enabled = false;
                            button_接觸測高_停止.Enabled = true;
                            button_接觸測高_關閉.Enabled = false;
                            comboBox_Spark.Enabled = false;
                            break;
                        case 98://終止測高流程
                            label_接觸測高數值.Text = "Stopping...";
                            button_接觸測高_執行.Enabled = true;
                            //button_接觸測高_更換測高塊.Enabled = false;
                            button_接觸測高_停止.Enabled = false;
                            button_接觸測高_關閉.Enabled = false;
                            comboBox_Spark.Enabled = false;
                            break;
                        #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus_依流程控制UI致能
                    }
                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckSparkTeachStatus_中止處理

                        string Message = "Stop Finish";

                        button_接觸測高_執行.Enabled = true;
                        //button_接觸測高_更換測高塊.Enabled = false;
                        button_接觸測高_停止.Enabled = false;
                        button_接觸測高_關閉.Enabled = true;
                        comboBox_Spark.Enabled = true;
                        label_接觸測高數值.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckSparkTeachStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckSparkTeachStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckSparkTeachStatus_完成處理

                        string Message = "";
                        switch ((int)SYSPara.CallProc("Saw", "GetTeachStatus"))
                        {
                            case 2:
                                button_接觸測高_執行.Enabled = false;
                                button_接觸測高_更換測高塊.Enabled = true;
                                button_接觸測高_停止.Enabled = false;
                                button_接觸測高_關閉.Enabled = false;
                                comboBox_Spark.Enabled = false;
                                break;
                            case 0:
                            case 99:
                                button_接觸測高_執行.Enabled = true;
                                //button_接觸測高_更換測高塊.Enabled = false;
                                button_接觸測高_停止.Enabled = false;
                                button_接觸測高_關閉.Enabled = true;
                                comboBox_Spark.Enabled = true;
                                break;
                        }
                        Message += teachmsg;
                        label_接觸測高數值.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckSparkTeachStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckSparkTeachStatus_完成處理
                    }
                }
                #endregion 私有函數_TimerTeachFlow_CheckSparkTeachStatus

                #region 私有函數_TimerTeachFlow_CheckNCSTeachStatus

                ledNCT.Value = CheckNCSTeachStatus;
                if (CheckNCSTeachStatus)
                {
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {
                        #region 私有函數_TimerTeachFlow_CheckSparkTeachStatus_依流程控制UI致能

                        case 0://提示
                            label_非接觸測高數值.Text = (string)SYSPara.CallProc("Saw", "GetTeachMessageNCT");
                            button_非接觸測高_執行.Enabled = true;
                            button_非接觸測高_停止.Enabled = true;
                            button_非接觸測高_關閉.Enabled = false;
                            chkZ1NCT.Enabled = false;
                            chkZ2NCT.Enabled = false;
                            break;
                        case 98:
                            label_非接觸測高數值.Text = "Stopping...";
                            button_非接觸測高_執行.Enabled = true;
                            button_非接觸測高_停止.Enabled = false;
                            button_非接觸測高_關閉.Enabled = false;
                            chkZ1NCT.Enabled = false;
                            chkZ2NCT.Enabled = false;
                            break;

                        #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus_依流程控制UI致能
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckNCSTeachStatus_中止處理

                        string Message = "Stop Finish";

                        button_非接觸測高_執行.Enabled = true;
                        button_非接觸測高_停止.Enabled = false;
                        button_非接觸測高_關閉.Enabled = true;
                        chkZ1NCT.Enabled = true;
                        chkZ2NCT.Enabled = true;
                        label_非接觸測高數值.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckNCSTeachStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckNCSTeachStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckNCSTeachStatus_完成處理

                        string Message = "";

                        Message += (string)SYSPara.CallProc("Saw", "GetTeachMessageNCT");

                        button_非接觸測高_執行.Enabled = true;
                        button_非接觸測高_停止.Enabled = false;
                        button_非接觸測高_關閉.Enabled = true;
                        chkZ1NCT.Enabled = true;
                        chkZ2NCT.Enabled = true;
                        label_非接觸測高數值.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckNCSTeachStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckNCSTeachStatus_完成處理
                    }
                }
                #endregion 私有函數_TimerTeachFlow_CheckNCSTeachStatus

                #region 私有函數_TimerTeachFlow_CheckKerfCheckStatus

                ledBaseline.Value = CheckKerfCheckStatus;
                if (CheckKerfCheckStatus)
                {
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {
                        #region 私有函數_TimerTeachFlow_CheckKerfCheckStatus_依流程控制UI致能

                        case 0://下一步
                            label_基準線校正.Text = teachmsg;
                            CanControlMotor = false;
                            button_基準線校正_執行.Enabled = true;
                            button_基準線校正_停止.Enabled = true;
                            button_基準線校正_關閉.Enabled = false;
                            comboBox_基準線校正.Enabled = false;
                            button_基準線校正_下一步.Enabled = true;
                            button_基準線校正_跳過.Enabled = false;
                            break;
                        case 1://視覺相關
                            label_基準線校正.Text = teachmsg;
                            CanControlMotor = true;
                            button_基準線校正_執行.Enabled = true;
                            button_基準線校正_停止.Enabled = true;
                            button_基準線校正_關閉.Enabled = false;
                            comboBox_基準線校正.Enabled = false;
                            button_基準線校正_下一步.Enabled = true;
                            button_基準線校正_跳過.Enabled = false;
                            break;
                        case 3://下一步/跳過
                            label_基準線校正.Text = teachmsg;
                            CanControlMotor = false;
                            button_基準線校正_執行.Enabled = true;
                            button_基準線校正_停止.Enabled = true;
                            button_基準線校正_關閉.Enabled = false;
                            comboBox_基準線校正.Enabled = false;
                            button_基準線校正_下一步.Enabled = true;
                            button_基準線校正_跳過.Enabled = true;
                            break;
                        case 98:
                            label_基準線校正.Text = "Stopping...";
                            CanControlMotor = false;
                            button_基準線校正_執行.Enabled = true;
                            button_基準線校正_停止.Enabled = false;
                            button_基準線校正_關閉.Enabled = false;
                            comboBox_基準線校正.Enabled = false;
                            button_基準線校正_下一步.Enabled = false;
                            button_基準線校正_跳過.Enabled = false;
                            break;
                        #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus_依流程控制UI致能
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckKerfCheckStatus_中止處理

                        string Message = "Stop Finish";

                        button_基準線校正_執行.Enabled = true;
                        button_基準線校正_停止.Enabled = false;
                        button_基準線校正_關閉.Enabled = true;
                        comboBox_基準線校正.Enabled = true;
                        button_基準線校正_下一步.Enabled = false;
                        button_基準線校正_跳過.Enabled = false;

                        label_基準線校正.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckKerfCheckStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckKerfCheckStatus_完成處理

                        string Message = "";
                        switch ((int)SYSPara.CallProc("Saw", "GetTeachStatus"))
                        {
                            case 0:
                                Message = "基準線校正完成，請取出試切片";
                                break;
                            case 99:
                                Message = teachmsg + ",基準線校正未完成，請取出試切片";
                                break;
                        }

                        button_基準線校正_執行.Enabled = true;
                        button_基準線校正_停止.Enabled = false;
                        button_基準線校正_關閉.Enabled = true;
                        comboBox_基準線校正.Enabled = true;
                        button_基準線校正_下一步.Enabled = false;
                        button_基準線校正_跳過.Enabled = false;
                        label_基準線校正.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckKerfCheckStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus_完成處理
                    }
                }
                #endregion 私有函數_TimerTeachFlow_CheckKerfCheckStatus

                #region 私有函數_TimerTeachFlow_CheckLearnFixtureStatus

                ledFixture.Value = CheckLearnFixtureStatus;
                if (CheckLearnFixtureStatus)
                {
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {//0:working，1:確認，2:確認、馬達移動、3:確認、馬達移動、Pitch移動
                        #region 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_依流程控制UI致能
                        case 0://下一步
                            label_切割道學習.Text = teachmsg;
                            //+ By Max 20210218, 切割道學習，確認切割道時將馬達控制面板啟用
                            CanControlMotor = true;
                            button_切割道學習_執行.Enabled = true;
                            button_切割道學習_停止.Enabled = true;
                            button_切割道學習_關閉.Enabled = false;
                            button_切割道學習_下一步.Enabled = true;
                            button_切割道學習_跳過.Enabled = false;
                            //+ By Max 20210223, 切割學習重學
                            if(AlarmLevel == "I" && AlarmCode == 64)
                                button_切割道學習_上一步.Enabled = true;
                            else
                                button_切割道學習_上一步.Enabled = false;
                            break;
                        case 1://視覺
                            label_切割道學習.Text = teachmsg;
                            CanControlMotor = true;
                            button_切割道學習_執行.Enabled = true;
                            button_切割道學習_停止.Enabled = true;
                            button_切割道學習_關閉.Enabled = false;
                            button_切割道學習_下一步.Enabled = true;
                            button_切割道學習_跳過.Enabled = false;
                            //+ By Max 20210223, 切割學習重學
                            if(AlarmLevel == "I" && AlarmCode == 64)
                                button_切割道學習_上一步.Enabled = true;
                            else
                                button_切割道學習_上一步.Enabled = false;
                            break;
                        case 3://資料儲存
                            label_切割道學習.Text = teachmsg;
                            CanControlMotor = true;
                            button_切割道學習_執行.Enabled = true;
                            button_切割道學習_停止.Enabled = true;
                            button_切割道學習_關閉.Enabled = false;
                            button_切割道學習_下一步.Enabled = true;
                            button_切割道學習_跳過.Enabled = true;
                            //+ By Max 20210223, 切割學習重學
                            button_切割道學習_上一步.Enabled = false;
                            break;
                        case 98:
                            label_切割道學習.Text = "Stopping....";
                            CanControlMotor = false;
                            button_切割道學習_執行.Enabled = true;
                            button_切割道學習_停止.Enabled = false;
                            button_切割道學習_關閉.Enabled = false;
                            button_切割道學習_下一步.Enabled = false;
                            button_切割道學習_跳過.Enabled = false;
                            //+ By Max 20210223, 切割學習重學
                            button_切割道學習_上一步.Enabled = false;
                            break;
                        #endregion 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_依流程控制UI致能
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_中止處理

                        string Message = "Stop Finish";

                        button_切割道學習_執行.Enabled = true;
                        button_切割道學習_停止.Enabled = false;
                        button_切割道學習_關閉.Enabled = true;
                        button_切割道學習_下一步.Enabled = false;
                        button_切割道學習_跳過.Enabled = false;
                        //+ By Max 20210223, 切割學習重學
                        button_切割道學習_上一步.Enabled = false;
                        label_切割道學習.Text = Message;
                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckLearnFixtureStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_完成處理

                        string Message = "";
                        switch ((int)SYSPara.CallProc("Saw", "GetTeachStatus"))
                        {
                            case 0:
                                Message = "切割道學習完成，" + teachmsg;
                                break;
                        }
                        button_切割道學習_執行.Enabled = true;
                        button_切割道學習_停止.Enabled = false;
                        button_切割道學習_關閉.Enabled = true;
                        button_切割道學習_下一步.Enabled = false;
                        button_切割道學習_跳過.Enabled = false;
                        //+ By Max 20210223, 切割學習重學
                        button_切割道學習_上一步.Enabled = false;
                        label_切割道學習.Text = Message;
                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckLearnFixtureStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckLearnFixtureStatus_完成處理
                    }
                }

                #endregion 私有函數_TimerTeachFlow_CheckLearnFixtureStatus

                #region 私有函數_TimerTeachFlow_CheckLearnMarkStatus

                ledTarget.Value = CheckLearnMarkStatus;
                if (CheckLearnMarkStatus)
                {
                    if (AlarmCode == 73 && LastAlarmCode != AlarmCode)
                    {
                        groupBox__靶點學習_內靶.Enabled = true;
                    }
                    LastAlarmCode = AlarmCode;

                    lbl_PosX.Text = String.Format("{0}", (int)SYSPara.CallProc("Saw", "GetMotorPos", "X2"));
                    lbl_PosY.Text = String.Format("{0}", (int)SYSPara.CallProc("Saw", "GetMotorPos", "Y"));

                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {   //0:working，1:確認，2:確認、馬達移動、3:確認、馬達移動、Pitch移動
                        #region 依流程控制UI致能
                        case 0:
                            label_靶點學習.Text = teachmsg;
                            CanControlMotor = true;
                            button_靶點學習_執行.Enabled = true;
                            button_靶點學習_停止.Enabled = true;
                            button_靶點學習_關閉.Enabled = false;
                            button_靶點學習_下一步.Enabled = true;
                            button_靶點學習_跳過.Enabled = false;
                            button_靶點學習_自動對焦.Enabled = false;
                            break;
                        case 1://視覺
                            label_靶點學習.Text = teachmsg;
                            CanControlMotor = true;
                            button_靶點學習_執行.Enabled = true;
                            button_靶點學習_停止.Enabled = true;
                            button_靶點學習_關閉.Enabled = false;
                            button_靶點學習_下一步.Enabled = true;
                            button_靶點學習_跳過.Enabled = false;
                            button_靶點學習_自動對焦.Enabled = true;
                            break;
                        case 3://資料儲存
                            label_靶點學習.Text = teachmsg;
                            CanControlMotor = false;
                            button_靶點學習_執行.Enabled = true;
                            button_靶點學習_停止.Enabled = true;
                            button_靶點學習_關閉.Enabled = false;
                            button_靶點學習_下一步.Enabled = true;
                            button_靶點學習_跳過.Enabled = true;
                            button_靶點學習_自動對焦.Enabled = false;
                            break;
                        case 98:
                            label_靶點學習.Text = "Stopping....";
                            CanControlMotor = false;
                            button_靶點學習_執行.Enabled = true;
                            button_靶點學習_停止.Enabled = false;
                            button_靶點學習_關閉.Enabled = false;
                            button_靶點學習_下一步.Enabled = false;
                            button_靶點學習_跳過.Enabled = false;
                            button_靶點學習_自動對焦.Enabled = false;
                            break;
                        #endregion
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 中止處理
                        string Message = "Stop Finish";

                        button_靶點學習_執行.Enabled = true;
                        button_靶點學習_停止.Enabled = false;
                        button_靶點學習_關閉.Enabled = true;
                        button_靶點學習_下一步.Enabled = false;
                        button_靶點學習_跳過.Enabled = false;
                        button_靶點學習_自動對焦.Enabled = false;
                        label_靶點學習.Text = Message;
                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckLearnMarkStatus = false;
                        #endregion
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 完成處理
                        string Message = "";
                        switch ((int)SYSPara.CallProc("Saw", "GetTeachStatus"))
                        {
                            case 0:
                                Message = "靶點學習完成，" + teachmsg;
                                break;
                        }
                        button_靶點學習_執行.Enabled = true;
                        button_靶點學習_停止.Enabled = false;
                        button_靶點學習_關閉.Enabled = true;
                        button_靶點學習_下一步.Enabled = false;
                        button_靶點學習_跳過.Enabled = false;
                        button_靶點學習_自動對焦.Enabled = false;
                        label_靶點學習.Text = Message;
                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckLearnMarkStatus = false;
                        #endregion 完成
                    }
                }
                #endregion 私有函數_TimerTeachFlow_CheckLearnMarkStatus

                #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus

                ledKnifeChange.Value = CheckChangeKnifeStatus;
                if (CheckChangeKnifeStatus)
                {
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {   //0:work 1:確認、序號、里程 2:破刀、確認
                        #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_依流程控制UI致能

                        case 0:
                            label_換刀流程.Text = teachmsg;
                            button_換刀流程_執行.Enabled = true;
                            button_換刀流程_確認.Enabled = false;
                            button_換刀流程_停止.Enabled = true;
                            button_換刀流程_關閉.Enabled = false;
                            maskedTextBoxKnifeTagZ1.Enabled = false;
                            //+ By Max 20210315, v4.0.1.54, 刀露量UI移除
                            //maskedTextBoxSPCRemainZ1.Enabled = false;
                            //maskedTextBoxSPCRemainZ2.Enabled = false;
                            maskedTextBoxKnifeTagZ2.Enabled = false;
                            btnZ1ToolClear.Enabled = false;
                            btnZ2ToolClear.Enabled = false;
                            break;
                        case 1:
                            label_換刀流程.Text = teachmsg;
                            button_換刀流程_執行.Enabled = true;
                            button_換刀流程_確認.Enabled = true;
                            button_換刀流程_停止.Enabled = true;
                            button_換刀流程_關閉.Enabled = false;
                            maskedTextBoxKnifeTagZ1.Enabled = true;
                            //+ By Max 20210315, v4.0.1.54, 刀露量UI移除
                            //maskedTextBoxSPCRemainZ1.Enabled = true;
                            //maskedTextBoxSPCRemainZ2.Enabled = true;
                            maskedTextBoxKnifeTagZ2.Enabled = true;
                            btnZ1ToolClear.Enabled = true;
                            btnZ2ToolClear.Enabled = true;
                            break;
                        case 2:
                            label_換刀流程.Text = teachmsg;
                            button_換刀流程_執行.Enabled = true;
                            button_換刀流程_確認.Enabled = true;
                            button_換刀流程_停止.Enabled = true;
                            button_換刀流程_關閉.Enabled = false;
                            maskedTextBoxKnifeTagZ1.Enabled = false;
                            //+ By Max 20210315, v4.0.1.54, 刀露量UI移除
                            //maskedTextBoxSPCRemainZ1.Enabled = false;
                            //maskedTextBoxSPCRemainZ2.Enabled = false;
                            maskedTextBoxKnifeTagZ2.Enabled = false;
                            btnZ1ToolClear.Enabled = false;
                            btnZ2ToolClear.Enabled = false;
                            break;
                        case 98:
                            label_換刀流程.Text = "Stopping....";
                            button_換刀流程_執行.Enabled = true;
                            button_換刀流程_確認.Enabled = false;
                            button_換刀流程_停止.Enabled = false;
                            button_換刀流程_關閉.Enabled = false;
                            maskedTextBoxKnifeTagZ1.Enabled = false;
                            //+ By Max 20210315, v4.0.1.54, 刀露量UI移除
                            //maskedTextBoxSPCRemainZ1.Enabled = false;
                            //maskedTextBoxSPCRemainZ2.Enabled = false;
                            maskedTextBoxKnifeTagZ2.Enabled = false;
                            btnZ1ToolClear.Enabled = false;
                            btnZ2ToolClear.Enabled = false;
                            break;
                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_依流程控制UI致能
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_中止處理

                        string Message = "Stop Finish";

                        button_換刀流程_執行.Enabled = true;
                        button_換刀流程_確認.Enabled = false;
                        button_換刀流程_停止.Enabled = false;
                        button_換刀流程_關閉.Enabled = true;
                        maskedTextBoxKnifeTagZ1.Text = "";
                        //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
                        //maskedTextBoxSPCRemainZ1.Text = "";
                        //maskedTextBoxSPCRemainZ2.Text = "";
                        //maskedTextBoxSPCRemainZ1.Enabled = false;
                        //maskedTextBoxSPCRemainZ2.Enabled = false;
                        maskedTextBoxKnifeTagZ2.Text = "";
                        maskedTextBoxKnifeTagZ1.Enabled = false;
                        maskedTextBoxKnifeTagZ2.Enabled = false;
                        btnZ1ToolClear.Enabled = false;
                        btnZ2ToolClear.Enabled = false;

                        label_換刀流程.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckChangeKnifeStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_完成處理

                        string Message = "";

                        switch ((int)SYSPara.CallProc("Saw", "GetTeachStatus"))
                        {
                            case 0:
                                Message = "換刀完成";
                                break;
                            case 99:
                                Message = "換刀未完成，" + teachmsg;
                                break;
                        }


                        button_換刀流程_執行.Enabled = true;
                        button_換刀流程_確認.Enabled = false;
                        button_換刀流程_停止.Enabled = false;
                        button_換刀流程_關閉.Enabled = true;
                        maskedTextBoxKnifeTagZ1.Text = "";
                        //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
                        //maskedTextBoxSPCRemainZ1.Text = "";
                        //maskedTextBoxSPCRemainZ2.Text = "";
                        //maskedTextBoxSPCRemainZ1.Enabled = false;
                        //maskedTextBoxSPCRemainZ2.Enabled = false;
                        maskedTextBoxKnifeTagZ2.Text = "";
                        maskedTextBoxKnifeTagZ1.Enabled = false;
                        maskedTextBoxKnifeTagZ2.Enabled = false;
                        btnZ1ToolClear.Enabled = false;
                        btnZ2ToolClear.Enabled = false;

                        label_換刀流程.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckChangeKnifeStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_完成處理
                    }
                }

                #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus

                #region 私有函數_TimerTeachFlow_GetTargetCheckStart
                bool bTargetCheckRunning = (bool)CallProc("Saw", "GetTargetCheckStart");
                ledTargetCheck.Value = bTargetCheckRunning;
                if (bTargetCheckRunning)
                {
                    //label_下刀點確認.Text = (string)SYSPara.CallProc("Saw", "GetAutoMessage");
                    //+ By Max 20210305
                    String sAutoAlarmLevel = (string)SYSPara.CallProc("Saw", "GetAutoAlarmLevel");
                    int iAutoAlarmCode = (int)SYSPara.CallProc("Saw", "GetAutoAlarmCode");
                    String[] sAutoAlarmMsg = (string[])SYSPara.CallProc("Saw", "GetAutoMessage");
                    t = SYSPara.Alarm.AlarmTable.Find(x => x.AlarmLevel == sAutoAlarmLevel && x.ErrorCode == iAutoAlarmCode && x.ModuleID == 50);
                    teachmsg = "";
                    if (t != null)
                    {
                        try
                        {
                            switch (SYSPara.Lang.GetNowLanguage())
                            {
                                case "tw":
                                    //v4.0.1.32 [修改]教導流程訊息能傳入變數
                                    //+ By Max 20210310, Alarmmsg改sAutoAlarmmsg
                                    if (sAutoAlarmMsg != null)
                                        teachmsg = string.Format(t.LanCHT.Explain, sAutoAlarmMsg);
                                    break;
                                case "en":
                                    //v4.0.1.32 [修改]教導流程訊息能傳入變數
                                    //+ By Max 20210310, Alarmmsg改sAutoAlarmmsg
                                    if (sAutoAlarmMsg != null)
                                        teachmsg = string.Format(t.LanENG.Explain, sAutoAlarmMsg);
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            teachmsg = "";
                        }
                    }
                    //-----------------------------------------------------------------
                    label_下刀點確認.Text = teachmsg;

                    if (TPTemp == null)
                    {
                        //+ By Max 20210315, v4.1.0.54
                        ChangeLang(SYSPara.Lang.GetNowLanguage());
                        TPTemp = GetAutoTP(button_自動流程.Text);
                        //+ By Max 20210209, 如為下刀點確認則ComboBox清除重填，並將cbo_下刀點確認方向選為CH2，以便將下刀點刀序更新
                        cbo_下刀點確認.Items.Clear();
                        cbo_下刀點確認方向.Items.Clear();
                        cbo_下刀點確認靶點選擇.Items.Clear();
                        cbo_下刀點確認方向.Items.Add("CH1");
                        cbo_下刀點確認方向.Items.Add("CH2");
                        cbo_下刀點確認靶點選擇.Items.Add("NEAR");
                        cbo_下刀點確認靶點選擇.Items.Add("AWAY");
                        cbo_下刀點確認方向.SelectedIndex = 1;
                        cbo_下刀點確認靶點選擇.SelectedIndex = 0;
                    }
                    if (TPTemp != null && TPTemp.Text != button_下刀點確認.Text)
                    {
                        ChoiceAuto(button_下刀點確認);
                    }

                    switch ((int)SYSPara.CallProc("Saw", "GetAutoFlowStep", false))
                    {
                        case 0:
                            button_下刀點確認_Move.Enabled = true;
                            cbo_下刀點確認方向.Enabled = true;
                            cbo_下刀點確認.Enabled = true;
                            cbo_下刀點確認靶點選擇.Enabled = true;
                            button_下刀點確認_關閉.Enabled = true;
                            break;
                        case 1:
                            button_下刀點確認_Move.Enabled = false;
                            cbo_下刀點確認方向.Enabled = false;
                            cbo_下刀點確認.Enabled = false;
                            cbo_下刀點確認靶點選擇.Enabled = false;
                            button_下刀點確認_關閉.Enabled = false;
                            break;

                    }
                }
                #endregion

                #region 私有函數_TimerTeachFlow_iSdynamicCutRun
                bool bDynamicCutOffsetRunning = (bool)CallProc("Saw", "iSdynamicCutRun");
                ledDynamicCutOffset.Value = bDynamicCutOffsetRunning;
                if (bDynamicCutOffsetRunning)
                {
                    //label_動態刀痕.Text = (string)SYSPara.CallProc("Saw", "GetAutoMessage");
                    //+ By Max 20210305
                    String sAutoAlarmLevel = (string)SYSPara.CallProc("Saw", "GetAutoAlarmLevel");
                    int iAutoAlarmCode = (int)SYSPara.CallProc("Saw", "GetAutoAlarmCode");
                    String [] sAutoAlarmMsg = (string[])SYSPara.CallProc("Saw", "GetAutoMessage");
                    t = SYSPara.Alarm.AlarmTable.Find(x => x.AlarmLevel == sAutoAlarmLevel && x.ErrorCode == iAutoAlarmCode && x.ModuleID == 50);
                    teachmsg = "";
                    if (t != null)
                    {
                        try
                        {
                            switch (SYSPara.Lang.GetNowLanguage())
                            {
                                case "tw":
                                    //v4.0.1.32 [修改]教導流程訊息能傳入變數
                                    //+ By Max 20210310, Alarmmsg改sAutoAlarmmsg
                                    if (sAutoAlarmMsg != null)
                                        teachmsg = string.Format(t.LanCHT.Explain, sAutoAlarmMsg);
                                    break;
                                case "en":
                                    //v4.0.1.32 [修改]教導流程訊息能傳入變數
                                    //+ By Max 20210310, Alarmmsg改sAutoAlarmmsg
                                    if (sAutoAlarmMsg != null)
                                        teachmsg = string.Format(t.LanENG.Explain, sAutoAlarmMsg);
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            teachmsg = "";
                        }
                    }
                    //-----------------------------------------------------------------
                    label_動態刀痕.Text = teachmsg;

                    if (TPTemp == null)
                    {
                        //+ By Max 20210315, v4.0.1.54, 動態刀痕切換語系當掉問題
                        ChangeLang(SYSPara.Lang.GetNowLanguage());
                        TPTemp = GetAutoTP(button_自動流程.Text);
                    }
                    if (TPTemp.Text != button_動態刀痕.Text)
                    {
                        panel_控制.Parent = tp動態刀痕;
                        panel_控制.Location = new Point(3, 355);
                        panel_控制.Visible = true;

                        ChoiceAuto(button_動態刀痕);
                    }

                    switch ((int)SYSPara.CallProc("Saw", "GetAutoFlowStep", true))
                    {
                        case 0://下一步
                            btn_動態刀痕_下一步.Enabled = true;
                            btn_動態刀痕_跳過.Enabled = false;
                            button_動態刀痕補償_關閉.Enabled = false;
                            break;
                        case 1://視覺(下一步)
                            CanControlMotor = true;
                            btn_動態刀痕_下一步.Enabled = true;
                            btn_動態刀痕_跳過.Enabled = false;
                            button_動態刀痕補償_關閉.Enabled = false;
                            break;
                        case 3://(下一步、跳過)
                            btn_動態刀痕_下一步.Enabled = true;
                            btn_動態刀痕_跳過.Enabled = true;
                            button_動態刀痕補償_關閉.Enabled = false;
                            break;
                        case 99://(完成)
                            btn_動態刀痕_下一步.Enabled = false;
                            btn_動態刀痕_跳過.Enabled = false;
                            button_動態刀痕補償_關閉.Enabled = true;
                            break;

                    }
                }
                #endregion

                #region 私有函數_TimerTeachFlow_CheckOrientedCenterStatus

                if (CheckOrientedCenterStatus)
                {
                    switch ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep"))
                    {   //0:work 1:確認、序號、里程 2:破刀、確認
                        #region 私有函數_TimerTeachFlow_CheckOrientedCenterStatus_依流程控制UI致能

                        case 0:
                            CanControlMotor = false;
                            label_圓心校正.Text = teachmsg;
                            button_圓心校正_執行.Enabled = true;
                            button_圓心校正_下一步.Enabled = true;
                            button_圓心校正_停止.Enabled = true;
                            button_圓心校正_關閉.Enabled = false;
                            break;
                        case 1:
                            CanControlMotor = true;
                            label_圓心校正.Text = teachmsg;
                            button_圓心校正_執行.Enabled = true;
                            button_圓心校正_下一步.Enabled = true;
                            button_圓心校正_停止.Enabled = true;
                            button_圓心校正_關閉.Enabled = false;
                            break;
                        case 98:
                            CanControlMotor = false;
                            label_圓心校正.Text = "Stopping....";
                            button_圓心校正_執行.Enabled = true;
                            button_圓心校正_下一步.Enabled = false;
                            button_圓心校正_停止.Enabled = false;
                            button_圓心校正_關閉.Enabled = false;
                            break;
                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_依流程控制UI致能
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 2)
                    {
                        #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_中止處理

                        string Message = "Stop Finish";
                        CanControlMotor = false;
                        button_圓心校正_執行.Enabled = true;
                        button_圓心校正_下一步.Enabled = false;
                        button_圓心校正_停止.Enabled = false;
                        button_圓心校正_關閉.Enabled = true;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckOrientedCenterStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_中止處理
                    }

                    if ((int)SYSPara.CallProc("Saw", "GetTeachActionResult") == 1)
                    {
                        #region 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_完成處理

                        string Message = "";

                        Message = "圓心校正完成";
                        CanControlMotor = false;
                        button_圓心校正_執行.Enabled = true;
                        button_圓心校正_下一步.Enabled = false;
                        button_圓心校正_停止.Enabled = false;
                        button_圓心校正_關閉.Enabled = true;
                        label_圓心校正.Text = Message;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, Message);
                        CheckOrientedCenterStatus = false;

                        #endregion 私有函數_TimerTeachFlow_CheckChangeKnifeStatus_完成處理
                    }
                }

                #endregion 私有函數_TimerTeachFlow_CheckOrientedCenterStatus

                #region 破刀後處理

                button_換刀續切.Visible = (bool)SYSPara.CallProc("Saw", "GetShowChangeBladeContinue");
                button_單刀續切.Visible = (bool)SYSPara.CallProc("Saw", "GetShowSingleBladeContinue");

                #endregion 破刀後處理

                #region 私有函數_TimerTeachFlow_AutoUI

                #region 私有函數_TimerTeachFlow_AutoUI_真空

                if (SysMode == Mode.Idle)
                {
                    chkVacuumOnOff.Enabled = true;
                    chkDestroyOnOff.Enabled = true;
                }
                else
                {
                    chkVacuumOnOff.Enabled = !(bool)SYSPara.CallProc("Saw", "GetNeedCheckVaca");
                    chkDestroyOnOff.Enabled = !(bool)SYSPara.CallProc("Saw", "GetNeedCheckVaca");
                }

                if ((bool)SYSPara.CallProc("Saw", "GetCTTAirVmc"))
                    chkVacuumOnOff.BackColor = Color.GreenYellow;
                else
                    chkVacuumOnOff.BackColor = Color.Transparent;

                if ((bool)SYSPara.CallProc("Saw", "GetCTTDestroy"))
                    chkDestroyOnOff.BackColor = Color.GreenYellow;
                else
                    chkDestroyOnOff.BackColor = Color.Transparent;

                #endregion 私有函數_TimerTeachFlow_AutoUI_真空

                #endregion 私有函數_TimerTeachFlow_AutoUI

            }
            timerTeachFlow.Enabled = true;
        }

        //+ By Max 20210209 修正繪製即時切割資訊的問題，由Timer呼叫DrawCutLine函式
        public class CutLineData
        {
            public String Side { get; set; }
            public int Z1LineNo { get; set; }
            public int Z2LineNo { get; set; }
            public int CutPercentage { get; set; }
            public bool Reset { get; set; }
        }

        private CutLineData cutData = new CutLineData();

        private void DrawCutLine()
        {
            bool bIsLiveCut = true;

            if (String.IsNullOrEmpty(SYSPara.PackageName))
                return;
            lock (lockObj)
            {
                if (cutData.Reset)
                {
                    Rotate = "CH2";
                    //設定畫筆顏色
                    label_Z1.ForeColor = Z1_pen.Color;
                    label_Z2.ForeColor = Z2_pen.Color;
                    //清料清空
                    if (Cut_graphics != null)
                    {
                        Cut_graphics.Dispose();
                        Cut_graphics = null;
                        BMP.Dispose();
                    }
                    BMP = new Bitmap(pictureBox_Cut.Width, pictureBox_Cut.Height);
                    Cut_graphics = Graphics.FromImage(BMP);
                    Cut_graphics.Clear(Color.Gray);
                    //畫布設定

                    int iXPitch = (pictureBox_Cut.Width - 10) / (SYSPara.PReadValue("Num_col").ToInt());
                    int iYPitch = (pictureBox_Cut.Height - 10) / (SYSPara.PReadValue("Num_row").ToInt());

                    for (int i = 0; i <= SYSPara.PReadValue("Num_col").ToInt(); i++)
                    {
                        Cut_graphics.DrawLine(new Pen(Color.Black, 1), new Point(5 + i * iXPitch, 0)
                            , new Point(5 + i * iXPitch, pictureBox_Cut.Height));
                    }

                    for (int i = 0; i <= SYSPara.PReadValue("Num_row").ToInt(); i++)
                    {
                        Cut_graphics.DrawLine(new Pen(Color.Black, 1), new Point(0, 5 + i * iYPitch)
                            , new Point(pictureBox_Cut.Width, 5 + i * iYPitch));
                    }
                }
                else
                {
                    bIsLiveCut = SYSPara.OReadValue("bLiveCut").ToBoolean();

                    int iXPitch = (pictureBox_Cut.Width - 10) / (SYSPara.PReadValue("Num_col").ToInt());
                    int iYPitch = (pictureBox_Cut.Height - 10) / (SYSPara.PReadValue("Num_row").ToInt());
                    switch (cutData.Side)
                    {
                        case "CH2":

                            if (Rotate != cutData.Side && bIsLiveCut)
                            {
                                BMP.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                Rotate = cutData.Side;
                            }

                            if (cutData.Z1LineNo > -1)
                            {
                                if (cutData.Z1LineNo <= SYSPara.PReadValue("Num_col").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z1_pen, new Point(5 + cutData.Z1LineNo * iXPitch, 0)
                                                    , new Point(5 + cutData.Z1LineNo * iXPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                }
                            }
                            if (cutData.Z2LineNo > -1)
                            {
                                if (cutData.Z2LineNo <= SYSPara.PReadValue("Num_col").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z2_pen, new Point(5 + cutData.Z2LineNo * iXPitch, 0)
                                                    , new Point(5 + cutData.Z2LineNo * iXPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                }
                            }
                            break;
                        case "CH1":

                            if (Rotate != cutData.Side && bIsLiveCut)
                            {
                                BMP.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                Rotate = cutData.Side;
                            }

                            if (cutData.Z1LineNo > -1)
                            {
                                if (cutData.Z1LineNo <= SYSPara.PReadValue("Num_row").ToInt())
                                {
                                    if (bIsLiveCut)
                                    {
                                        Cut_graphics.DrawLine(Z1_pen,
                                            new Point(5 + cutData.Z1LineNo * iYPitch, 0),
                                            new Point(5 + cutData.Z1LineNo * iYPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                    }
                                    else
                                    {
                                        Cut_graphics.DrawLine(Z1_pen,
                                            new Point(0, 5 + cutData.Z1LineNo * iYPitch),
                                            new Point(pictureBox_Cut.Width * cutData.CutPercentage / 100, 5 + cutData.Z1LineNo * iYPitch));
                                    }
                                }
                            }
                            if (cutData.Z2LineNo > -1)
                            {
                                if (cutData.Z2LineNo <= SYSPara.PReadValue("Num_row").ToInt())
                                {
                                    if (bIsLiveCut)
                                    {
                                        Cut_graphics.DrawLine(Z2_pen,
                                            new Point(5 + cutData.Z2LineNo * iYPitch, 0),
                                            new Point(5 + cutData.Z2LineNo * iYPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                    }
                                    else
                                    {
                                        Cut_graphics.DrawLine(Z2_pen,
                                            new Point(0, 5 + cutData.Z2LineNo * iYPitch),
                                            new Point(pictureBox_Cut.Width * cutData.CutPercentage / 100, 5 + cutData.Z2LineNo * iYPitch));
                                    }
                                }
                            }
                            break;
                        case "Bottom":
                            if (cutData.Z1LineNo > -1)
                            {
                                if (cutData.Z1LineNo <= SYSPara.PReadValue("Num_row").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z1_pen, new Point(5 + cutData.Z1LineNo * iXPitch, 0)
                                                    , new Point(5 + cutData.Z1LineNo * iXPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                }
                            }
                            if (cutData.Z2LineNo > -1)
                            {
                                if (cutData.Z2LineNo <= SYSPara.PReadValue("Num_row").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z2_pen, new Point(5 + cutData.Z2LineNo * iXPitch, 0)
                                                    , new Point(5 + cutData.Z2LineNo * iXPitch, pictureBox_Cut.Height * cutData.CutPercentage / 100));
                                }
                            }
                            break;
                        case "Left":
                            if (cutData.Z1LineNo > -1)
                            {
                                if (cutData.Z1LineNo <= SYSPara.PReadValue("Num_col").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z1_pen, new Point(0, 5 + cutData.Z1LineNo * iYPitch)
                                            , new Point(pictureBox_Cut.Width * cutData.CutPercentage / 100, 5 + cutData.Z1LineNo * iYPitch));
                                }
                            }
                            if (cutData.Z2LineNo > -1)
                            {
                                if (cutData.Z2LineNo <= SYSPara.PReadValue("Num_col").ToInt())
                                {
                                    Cut_graphics.DrawLine(Z2_pen, new Point(0, 5 + cutData.Z2LineNo * iYPitch)
                                            , new Point(pictureBox_Cut.Width * cutData.CutPercentage / 100, 5 + cutData.Z2LineNo * iYPitch));
                                }
                            }
                            break;
                    }
                }

                this.pictureBox_Cut.Image = BMP;
            }
        }

        object lockObj = new object();
        string Rotate = "CH2";//目前圖畫的角度
        private void DrawLineData(String Side, int iZ1LineNo, int iZ2LineNo, int i百分比, bool Reset)
        {
            lock (lockObj)
            {
                cutData.Side = Side;
                cutData.Z1LineNo = iZ1LineNo;
                cutData.Z2LineNo = iZ2LineNo;
                cutData.CutPercentage = i百分比;
                cutData.Reset = Reset;
            }
        }

        //v4.0.1.30 主畫面顯示切割/掃靶時間
        private void ShowAllTime(double dLoadTime, double dScanTime, double dCutTime, double dCleanTime, double dUnloadTime)
        {
            BeginInvoke(new Action(() =>
            {
                label_入料時間.Text = dLoadTime.ToString("f1");
                label_掃描時間.Text = dScanTime.ToString("f1");
                label_切割時間.Text = dCutTime.ToString("f1");
                label_清洗時間.Text = dCleanTime.ToString("f1");
                label_出料時間.Text = dUnloadTime.ToString("f1");
            }));
        }

        TabPage TPTemp = null;
        TabPage[] TeachTP;
        TabPage[] AutoTP;
        bool CheckSparkTeachStatus;//掃描測高狀態
        bool CheckNCSTeachStatus;//掃描非接觸測高狀態
        bool CheckKerfCheckStatus;//掃描基準線校正狀態
        bool CheckLearnFixtureStatus;//掃描切割道學習狀態
        bool CheckLearnMarkStatus;//掃描靶點學習狀態
        bool CheckChangeKnifeStatus; //掃描換刀流程狀態
        bool CheckOrientedCenterStatus;
        bool CanControlMotor = false;

        Mode SysMode = Mode.Idle;
        enum Mode
        {
            Idle,
            Teach,
            Auto,
        }

        strKnifeData ShowKnifeData1;
        strKnifeData ShowKnifeData2;
        //v4.0.1.30
        private Bitmap BMP;// = new Bitmap(450, 450);//修正Graphics切換畫面後會消失的問題
        private Graphics Cut_graphics = null;
        private Pen Z1_pen = new Pen(Color.SkyBlue, 4);
        private Pen Z2_pen = new Pen(Color.LimeGreen, 4);

        #region For cross and vertical line callback function
        void ShowCrossLine(object sender, EventArgs e)
        {
            //NLogger.Debug("ShowCrossLine");
            //NLogger.Debug((e as ServerEventArgs).strResult);

            checkCross.CheckedChanged -= checkCross_CheckedChanged;
            checkCross.Checked = (e as ServerEventArgs).strResult == "CC,1;";
            checkCross.CheckedChanged += checkCross_CheckedChanged;
        }

        void ShowVerticalLine(object sender, EventArgs e)
        {
            //NLogger.Debug("ShowVerticalLine");
            //NLogger.Debug((e as ServerEventArgs).strResult);

            checkValigm.CheckedChanged -= checkValigm_CheckedChanged;
            checkValigm.Checked = (e as ServerEventArgs).strResult == "MKLC,1;";
            checkValigm.CheckedChanged += checkValigm_CheckedChanged;
        }
        #endregion

        private string GetDialogMsgText(string strKey)
        {
            ResourceManager resourceManager = new ResourceManager(typeof(Properties.Resources));
            if (SYSPara.Lang.GetNowLanguage() == "tw")
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("zh-TW"));
            }
            else
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        #endregion 私有函數

        #region 元件事件

        //bool bSet = true;
        private void button_異常排除紀錄_Click(object sender, EventArgs e)
        {   //v0.0.7.11 By Sanxiu 異常排除流程步驟記錄
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「異常排除記錄」");
            SYSPara.CallProc("Saw", "SaveAllSawStepIndex");
            //bSet = !bSet;
            //SYSPara.CallProc("Saw", "SetIngonreSafeDoor", bSet);
        }
        //2020/02/05
        private void lb異常訊息_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                異常通知畫面位置 = e.Location;
                mMouseDown = true;
            }
        }
        //2020/02/05
        private void lb異常訊息_MouseUp(object sender, MouseEventArgs e)
        {
            mMouseDown = false;
        }
        //2020/02/05
        private void lb異常訊息_MouseMove(object sender, MouseEventArgs e)
        {
            if (mMouseDown)
            {
                pnl異常通知.Left += (e.X - 異常通知畫面位置.X);
                pnl異常通知.Top += (e.Y - 異常通知畫面位置.Y);
            }
        }

        private void button_教導流程_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            if (SYSPara.PackageName == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_LoadPackage_And_Home"));
                //MessageBox.Show("請載入料號並歸零");
            }
            //v4.0.1.4 By Woody 按下開始先判斷狀態
            else if ((SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME) && SYSPara.SysRun)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm"));
                //MessageBox.Show("機台運行中，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (FormSet.mMainFlow.mSaw.SReadValue("ConnectHandler").ToInt() > 0 && (bool)CallProc("Saw", "GetSawTablbeChkSignal"))//v4.0.1.21 入出料中，機台不可進入教導流程
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", GetDialogMsgText("Text_Main3_Loading_Unloading_Alarm"));
                //MessageBox.Show("入/出料中，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if ((bool)CallProc("Saw", "GetVacaStatus") && SYSPara.Simulation == 0)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", GetDialogMsgText("Text_Main3_Vacuum_On_Alarm"));
                //MessageBox.Show("真空狀態下，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                SYSPara.Lang.ChangeLanguage(SYSPara.Lang.GetNowLanguage());   //v4.0.1.34
                ChoiceTeach(sender);
            }
        }

        private void button_自動流程_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            if (SYSPara.PackageName == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_LoadPackage_And_Home"));
                //MessageBox.Show("請載入料號並歸零");
            }
            else if ((int)SYSPara.CallProc("Saw", "GetTeachActionNo") != 0)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Finish_Or_Close_Teach_Flow_Hint"));
                //MessageBox.Show("請關閉或完成教導流程");
            }
            //+ By Max 20210205
            else if ((SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME) && SYSPara.SysRun)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm"));
                //MessageBox.Show("機台運行中，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                ChoiceAuto(sender);
            }

        }

        private void button_接觸測高_Click(object sender, EventArgs e)
        {
            //SYSPara.Lang.ChangeLanguage(SYSPara.Lang.GetNowLanguage());
            //ChangeLang(SYSPara.Lang.GetNowLanguage());
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            ChoiceTeach(sender);

            button_接觸測高_執行.Enabled = true;
            button_接觸測高_更換測高塊.Enabled = false;
            button_接觸測高_停止.Enabled = false;
            button_接觸測高_關閉.Enabled = true;
            comboBox_Spark.Enabled = true;
        }

        private void button_非接觸測高_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            ChoiceTeach(sender);

            button_非接觸測高_執行.Enabled = true;
            button_非接觸測高_停止.Enabled = false;
            button_非接觸測高_關閉.Enabled = true;
            chkZ1NCT.Enabled = true;
            chkZ2NCT.Enabled = true;
        }

        private void button_基準線校正_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp基準線校正;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;

            ChoiceTeach(sender);

            button_基準線校正_執行.Enabled = true;
            button_基準線校正_停止.Enabled = false;
            button_基準線校正_關閉.Enabled = true;
            comboBox_基準線校正.Enabled = true;
            button_基準線校正_下一步.Enabled = false;
            button_基準線校正_跳過.Enabled = false;

        }

        private void button_切割道學習_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp切割道學習;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;

            ChoiceTeach(sender);

            button_切割道學習_執行.Enabled = true;
            button_切割道學習_停止.Enabled = false;
            button_切割道學習_關閉.Enabled = true;
            button_切割道學習_下一步.Enabled = false;
            button_切割道學習_跳過.Enabled = false;
            //+ By Max 20210223, 切割學習重學
            button_切割道學習_上一步.Enabled = false;
        }

        private void button_靶點學習_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp靶點學習;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;
            ChoiceTeach(sender);

            button_靶點學習_執行.Enabled = true;
            button_靶點學習_停止.Enabled = false;
            button_靶點學習_關閉.Enabled = true;
            button_靶點學習_自動對焦.Enabled = false;
            button_靶點學習_下一步.Enabled = false;
            button_靶點學習_跳過.Enabled = false;
            groupBox__靶點學習_內靶.Enabled = false;
            //+ By Max 20210205
            if (SYSPara.PReadValue("TargetFind").ToInt() == 0)
            {
                groupBox__靶點學習_內靶.Visible = true;
                label_靶點學習_左上角切割位置.Text = "";
            }
            else
                groupBox__靶點學習_內靶.Visible = false;
        }

        private void button_圓心校正_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp圓心校正;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;
            ChoiceTeach(sender);

            button_圓心校正_執行.Enabled = true;
            button_圓心校正_下一步.Enabled = false;
            button_圓心校正_停止.Enabled = false;
            button_圓心校正_關閉.Enabled = true;
        }

        private void button_離開教導_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_自動流程_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_下刀點確認_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            ChoiceAuto(sender);
        }

        private void button_動態刀痕_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp動態刀痕;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;

            ChoiceAuto(sender);
        }

        private void button_換刀流程_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            ChoiceAuto(sender);

            button_換刀流程_執行.Enabled = true;
            button_換刀流程_確認.Enabled = false;
            button_換刀流程_停止.Enabled = false;
            button_換刀流程_關閉.Enabled = true;
            maskedTextBoxKnifeTagZ2.Enabled = false;
            //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
            //maskedTextBoxSPCRemainZ2.Enabled = false;
            //maskedTextBoxSPCRemainZ1.Enabled = false;
            maskedTextBoxKnifeTagZ1.Enabled = false;
            btnZ2ToolClear.Enabled = false;
            btnZ1ToolClear.Enabled = false;
        }

        private void button_手動定靶_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tp手動定靶;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;

            ChoiceAuto(sender);
            CanControlMotor = true;
        }

        private void button_刀痕資料_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            dgv_KerfData.DataSource = CallProc("Saw", "GetKerfCheckView");
            ChoiceAuto(sender);
        }

        private void button_重啟視覺_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            ChoiceAuto(sender);
        }

        private void button_CCD測試_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            panel_控制.Parent = tpCCD測試;
            panel_控制.Location = new Point(3, 355);
            panel_控制.Visible = true;

            ChoiceAuto(sender);
        }

        private void button_接觸測高_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
        }

        private void button_非接觸測高_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_基準線校正_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_切割道學習_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_靶點學習_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_換刀流程_Close_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            CloseTeachorAuto();
        }

        private void button_下刀點確認_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            //+ By Max 20210219, 下刀點確認完後，先暫停機台運作，需由使用者按下開始生產
            SYSPara.MusicOn = false;
            FormSet.mMSS.M_Stop();
            SYSPara.SysRun = false;

            CallProc("Saw", "SetTargetCheckFinish");
            //+ By Max 20210206 所有自動流程都改成按關閉直接往上跳兩層
            CloseTeachorAuto();
            CloseTeachorAuto();
            //+ By Max 20210205
            cbo_下刀點確認.Items.Clear();
            //cbo_下刀點確認方向.SelectedIndex = cbo_下刀點確認方向.Items.Count - 1;
            //cbo_下刀點確認靶點選擇.SelectedIndex = cbo_下刀點確認靶點選擇.Items.Count - 1;
        }

        private void button_動態刀痕補償_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            chkDynamicCutOffset.Checked = false;
            CallProc("Saw", "SetdynamicCutStop");
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void button_手動定靶_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void button_刀痕資料_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void button_CCD測試_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void button_圓心校正_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
        }

        private void button_重啟視覺_關閉_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void button_接觸測高_執行_Click(object sender, EventArgs e)
        {
            if (comboBox_Spark.Text == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Select_Spindle_Hint"));
                //MessageBox.Show("Please select the spindle");
            }
            else
            {
                SYSPara.CallProc("Saw", "SetTeachTimer", true);
                SYSPara.CallProc("Saw", "SparkSelect", comboBox_Spark.Text);
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
                if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 1))
                {
                    CheckSparkTeachStatus = true;
                }
            }
        }

        private void button_接觸測高_更換測高塊_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "ChangeSparkTestBlock");
            button_接觸測高_執行.Enabled = true;
            button_接觸測高_更換測高塊.Enabled = false;
            button_接觸測高_停止.Enabled = false;
            button_接觸測高_關閉.Enabled = true;
            comboBox_Spark.Enabled = true;
        }

        private void button_接觸測高_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");
        }

        private void button_非接觸測高_執行_Click(object sender, EventArgs e)
        {
            if (!chkZ1NCT.Checked && !chkZ2NCT.Checked)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Select_Spindle_Hint"));
                //MessageBox.Show("Please select the spindle");
            }
            else
            {
                SYSPara.CallProc("Saw", "SetTeachTimer", true);
                SYSPara.CallProc("Saw", "NCTSelect", "Z1", chkZ1NCT.Checked);
                SYSPara.CallProc("Saw", "NCTSelect", "Z2", chkZ2NCT.Checked);
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
                if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 2))
                {
                    CheckNCSTeachStatus = true;
                }
            }
        }

        private void button_非接觸測高_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");

        }

        private void button_基準線校正_執行_Click(object sender, EventArgs e)
        {
            if (comboBox_基準線校正.Text == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Select_Spindle_Hint"));
                //MessageBox.Show("Please select the spindle");
            }
            else
            {
                SYSPara.CallProc("Saw", "SetTeachTimer", true);
                SYSPara.CallProc("Saw", "BaseLineSelect", comboBox_基準線校正.Text);
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
                if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 3))
                {
                    CheckKerfCheckStatus = true;
                }
            }
        }

        private void button_基準線校正_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");

        }

        private void button_基準線校正_切割_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void button_靶點學習_執行_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachTimer", true);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 5))
            {
                CheckLearnMarkStatus = true;
            }
        }

        private void button_靶點學習_應用_Click(object sender, EventArgs e)
        {
            SYSPara.SysState = StateMode.SM_PAUSE;
            SYSPara.SysRun = false;
            FormSet.mMSS.M_Stop();
            SYSPara.CallProc("Saw", "SaveLearnMark");
        }

        private void button_靶點學習_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");
            //+ By Max 20210225, 內靶掃描
            groupBox__靶點學習_內靶.Enabled = false;
        }

        private void button_靶點學習_第一個點_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
            button_靶點學習_關閉.Enabled = true;
            button_靶點學習_執行.Enabled = true;
        }

        private void button_換刀流程_Run_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachTimer", true);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 6))
            {
                CheckChangeKnifeStatus = true;
            }
        }

        private void button_換刀流程_Stop_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");

        }

        private void button_換刀流程_Done_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void button_重啟視覺_執行_Click(object sender, EventArgs e)
        {
            if ((SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME) && SYSPara.SysRun)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm"));
                //MessageBox.Show("機台運行中，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (String.IsNullOrEmpty(SYSPara.PackageName))
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_No_Package_Loaded_Alarm"));
                //MessageBox.Show("料號未載入，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下重啟視覺按鈕！");

            VisionLoad();
            SetParent(hCCDHandle, panel_Vision.Handle);
            SetWindowPos((int)hCCDHandle, 0, (panel_Vision.Width - iVisionWidth) / 2, (panel_Vision.Height - iVisionHeight) / 2, iVisionWidth, iVisionHeight, 4);

            CallProc("Saw", "Set_CCDHandle", (int)hCCDHandle);
        }

        private void button_切割道學習_控制_Click(object sender, EventArgs e)
        {
            panel_控制.Location = new Point(2, 290);
            panel_控制.Visible = !panel_控制.Visible;

            //if (panel_控制.Visible)
            //    tc教導.Visible = false;          
            SYSPara.CallProc("Saw", "SetVisionRefresh");
        }

        private void button_切割道學習_Move_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 6))
            {
                CheckKerfCheckStatus = true;

                #region UI
                button_基準線校正_執行.Enabled = false;
                button_基準線校正_關閉.Enabled = false;
                button_基準線校正_停止.Enabled = true;

                //label_基準線校正.Text = "移至指定位置後按下試切...";
                #endregion
            }
        }

        private void button_Move_MouseUp(object sender, MouseEventArgs e)
        {
            string Direct = ((Button)sender).Name.Split('_')[1];
            int Pitch = 0;
            string motor = "";
            int NowPos = 0;

            switch (Direct)
            {
                case "前":
                    Pitch = Convert.ToInt32(button_Y增額.Text);
                    motor = "Y";
                    break;
                case "後":
                    Pitch = -1 * Convert.ToInt32(button_Y增額.Text);
                    motor = "Y";
                    break;
                case "右":
                    Pitch = Convert.ToInt32(button_X增額.Text);
                    motor = "X2";
                    break;
                case "左":
                    Pitch = -1 * Convert.ToInt32(button_X增額.Text);
                    motor = "X2";
                    break;
                case "上":
                    Pitch = Convert.ToInt32(button_Z增額.Text);
                    motor = "Z2";
                    break;
                case "下":
                    Pitch = -1 * Convert.ToInt32(button_Z增額.Text);
                    motor = "Z2";
                    break;
                case "順時針":
                    Pitch = Convert.ToInt32(button_θ增額.Text);
                    motor = "U";
                    break;
                case "逆時針":
                    Pitch = -1 * Convert.ToInt32(button_θ增額.Text);
                    motor = "U";
                    break;
            }

            NowPos = (int)SYSPara.CallProc("Saw", "GetMotorPos", motor);

            SYSPara.CallProc("Saw", "MotorMove", motor, NowPos + Pitch);
        }

        private void button_增額_Click(object sender, EventArgs e)
        {
            Button oButton = (Button)sender;
            switch (oButton.Text)
            {
                case "+10000":
                    oButton.Text = "+1000";
                    break;
                case "+1000":
                    oButton.Text = "+100";
                    break;
                case "+100":
                    oButton.Text = "+10";
                    break;
                case "+10":
                    oButton.Text = "+1";
                    break;
                case "+1":
                    oButton.Text = "+10000";
                    break;
            }
        }


        //v4.0.1.24移動Pitch   
        private void button_MovePitch_MouseUp(object sender, MouseEventArgs e)
        {
            string Direct = ((Button)sender).Name.Split('_')[1];
            int Pitch = 0;
            int PitchX = 0;
            int PitchY = 0;
            switch ((string)CallProc("Saw", "GetSide"))
            {
                case "CH1":
                    PitchX = SYSPara.PReadValue("Pitch_Y").ToInt();
                    PitchY = SYSPara.PReadValue("Pitch_X").ToInt();
                    break;
                case "CH2":
                    PitchX = SYSPara.PReadValue("Pitch_X").ToInt();
                    PitchY = SYSPara.PReadValue("Pitch_Y").ToInt();
                    break;
            }
            string motor = "";
            int NowPos = 0;

            switch (Direct)
            {
                case "YPitch前":
                    Pitch = PitchY;
                    motor = "Y";
                    break;
                case "YPitch後":
                    Pitch = -1 * PitchY;
                    motor = "Y";
                    break;
                case "XPitch右":
                    Pitch = PitchX;
                    motor = "X2";
                    break;
                case "XPitch左":
                    Pitch = -1 * PitchX;
                    motor = "X2";
                    break;
            }

            NowPos = (int)SYSPara.CallProc("Saw", "GetMotorPos", motor);

            SYSPara.CallProc("Saw", "MotorMove", motor, NowPos + Pitch);
        }

        private void button_切割道學習_Next_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void chkVacuumOnOff_CheckedChanged(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((CheckBox)sender).Text);

            if ((SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME) && SYSPara.SysRun)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm"));
                //MessageBox.Show("機台運行中，無法執行此操作！");
                return;
            }

            SYSPara.CallProc("Saw", "VaccumStateChange", chkVacuumOnOff.Checked);
        }

        private void chkZ1NCT_CheckedChanged(object sender, EventArgs e)
        {
            SYSPara.CallProc("Saw", "NCTSelect", "Z1", chkZ1NCT.Checked);
        }

        private void chkZ2NCT_CheckedChanged(object sender, EventArgs e)
        {
            SYSPara.CallProc("Saw", "NCTSelect", "Z2", chkZ2NCT.Checked);
        }

        private void button_基準線校正_移至靶點_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void btnZ2ToolClear_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            if (String.IsNullOrEmpty(maskedTextBoxKnifeTagZ2.Text))
            {
                maskedTextBoxKnifeTagZ2.Text = "Z2Blade";
            }

            //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
            //if (String.IsNullOrEmpty(maskedTextBoxSPCRemainZ2.Text))
            //{
            //    MessageBox.Show("請輸入Z2規格刀露量", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            try
            {
                //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
                //double 刀露量 = Convert.ToDouble(maskedTextBoxSPCRemainZ2.Text);

                if ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep") == 1)
                {
                    SYSPara.PSetValue("BladeZ2Data", "sToolNameZ2", maskedTextBoxKnifeTagZ2.Text);
                    //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除，里程數歸零
                    //SYSPara.PSetValue("BladeZ2Data", "dRealKnifeRemainZ2", 刀露量);
                    //+ By Max, 20210804, 換刀時紀錄最終里程
                    SYSPara.LogSay(EnLoggerType.EnLog_SPC, String.Format("換刀前Z2刀具里程數：{0}", SYSPara.PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble()));
                    SYSPara.PSetValue("BladeZ2Data", "dRealMotorDistanceZ2", 0, "Double");
                    //+ By Max 20210316, v4.0.1.55
                    FormSet.mPackage.PackageDS.SaveFile();
                }

                SYSPara.CallProc("Saw", "ResetBladeInfo", "Z2");
            }
            catch (FormatException)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Message", GetDialogMsgText("Text_Main3_Input_Format_Error"));
                //MessageBox.Show("輸入格式錯誤", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SYSPara.CallProc("Saw", "BladeRefresh");
            ShowToolData();
        }

        private void btnZ1ToolClear_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

            if (String.IsNullOrEmpty(maskedTextBoxKnifeTagZ1.Text))
            {
                maskedTextBoxKnifeTagZ1.Text = "Z1Blade";
            }

            //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
            //if (String.IsNullOrEmpty(maskedTextBoxSPCRemainZ1.Text))
            //{
            //    MessageBox.Show("請輸入Z1規格刀露量", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            try
            {
                //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除
                //double 刀露量 = Convert.ToDouble(maskedTextBoxSPCRemainZ1.Text);

                if ((int)SYSPara.CallProc("Saw", "GetTeachFlowStep") == 1)
                {
                    SYSPara.PSetValue("BladeZ1Data", "sToolNameZ1", maskedTextBoxKnifeTagZ1.Text);
                    //+ By Max 20210315, v4.0.1.54, 刀露量紀錄移除，里程數歸零
                    //SYSPara.PSetValue("BladeZ1Data", "dRealKnifeRemainZ1", 刀露量);
                    //+ By Max, 20210804, 換刀時紀錄最終里程
                    SYSPara.LogSay(EnLoggerType.EnLog_SPC, String.Format("換刀前Z1刀具里程數：{0}", SYSPara.PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble()));
                    SYSPara.PSetValue("BladeZ1Data", "dRealMotorDistanceZ1", 0, "Double");
                    //+ By Max 20210316, v4.0.1.55
                    FormSet.mPackage.PackageDS.SaveFile();
                }

                SYSPara.CallProc("Saw", "ResetBladeInfo", "Z1");
            }
            catch (FormatException)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Message", GetDialogMsgText("Text_Main3_Input_Format_Error"));
                //MessageBox.Show("輸入格式錯誤", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SYSPara.CallProc("Saw", "BladeRefresh");
            ShowToolData();
        }

        private void button_下刀點確認_Move_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            if (cbo_下刀點確認方向.Text == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Put_Down_Knife_Direction_Hint"));
                //MessageBox.Show("請選取下刀點方向選擇");
                return;
            }
            if (cbo_下刀點確認.Text == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Put_Down_Knife_Position_Hint"));
                //MessageBox.Show("請選取下刀點");
                return;
            }
            if (cbo_下刀點確認靶點選擇.Text == "")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Target_Point_Hint"));
                //MessageBox.Show("請選取靶點");
                return;
            }

            CallProc("Saw", "SetTargetCheck", cbo_下刀點確認方向.Text, Convert.ToInt32(cbo_下刀點確認.Text), cbo_下刀點確認靶點選擇.Text);
        }

        private void button_手動定靶_取得靶點位置_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            CallProc("Saw", "SetManualAlignmentFinish");
            CanControlMotor = false;
            CloseTeachorAuto();
            CloseTeachorAuto();
        }

        private void chkDynamicCutOffset_CheckedChanged(object sender, EventArgs e)
        {
            //v0.0.7.9 By Sanxiu 動態刀痕補償
            if (chkDynamicCutOffset.Checked)
            {
                CallProc("Saw", "SetdynamicCutFunc", true);
                chkDynamicCutOffset.Enabled = false;
                button_動態刀痕.BackColor = Color.LightSkyBlue;
            }
            else
            {
                //FormSet.mMSS.CallProc("Saw", "SetdynamicCutFunc", false);
                chkDynamicCutOffset.Enabled = true;
            }
        }

        private void btn_動態刀痕_下一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetAutoUserCheckOk");
        }

        private void btn_動態刀痕_跳過_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetAutoUserCheckSkip");
        }

        private void button_強制排除_Click(object sender, EventArgs e)
        {
            //+ By Max 20210310, 教導模式下提示先離開教導模式
            if (SysMode == Mode.Teach)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Message", GetDialogMsgText("Text_Main3_Quit_Teach_Flow_Hint"));
                //MessageBox.Show("請先離開教導模式！", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //+ By Max 20210329, v4.0.1.59，防呆卡控
            if (SYSPara.SysState == StateMode.SM_AUTO_RUN)
            {
                //if (SYSPara.Lang.GetNowLanguage() == "tw")
                //{
                //    //MessageBox.Show("機台運行中，禁止強制排除!!!", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm2"));
                //}
                //else
                //{
                //    //MessageBox.Show("The machine is running, can not force abort!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //    dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", "The machine is running, can not force abort!!!");
                //}

                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm2"));

                return;
            }
            else if (SYSPara.SysState == StateMode.SM_PAUSE)
            {
                if (FormSet.mMainFlow.mSaw.SReadValue("ConnectHandler").ToInt() > 0 && (bool)CallProc("Saw", "GetSawTablbeChkSignal"))//v4.0.1.21 入出料中，機台不可進入強排流程
                {
                    //if (SYSPara.Lang.GetNowLanguage() == "tw")
                    //{
                    //    dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", GetDialogMsgText("Text_Main3_Loading_Unloading_Alarm"));
                    //    //MessageBox.Show("入/出料中，無法執行此操作！", "警告訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}
                    //else
                    //{
                    //    dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", "Load/Unload in processing，can not do this operation！");
                    //    //MessageBox.Show("Load/Unload in processing，can not do this operation！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}

                    dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning", GetDialogMsgText("Text_Main3_Loading_Unloading_Alarm"));

                    return;
                }
            }

            if (dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Warning:", GetDialogMsgText("Text_Main3_ForceOut_Check")) == DialogResult.OK)
            //if (MessageBox.Show("確定執行強排?", "警告訊息", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);

                DialogResult res = DialogResult.No;
                if ((bool)SYSPara.CallProc("Saw", "IsTargetScanning"))
                {
                    res = dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Hint:", 
                        GetDialogMsgText("Text_Main3_ForceOut_Continue_Check"), SYSPara.Lang.GetNowLanguage(), "Yes", "No");
                    //res = MessageBox.Show("強排後是否續做?", "提示訊息", MessageBoxButtons.YesNo);
                }

                SYSPara.CallProc("Saw", "SetAutoFlowChartEnd", res == DialogResult.OK);

                //+ By Max 20210209, 下刀點確認頁簽關閉
                button_下刀點確認_關閉_Click(button_下刀點確認_關閉, null);
                //+ By Max 20210309, 動態刀痕頁簽關閉
                button_動態刀痕補償_關閉_Click(button_動態刀痕補償_關閉, null);
            }

        }

        private void button_基準線校正_下一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void button_基準線校正_跳過_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckSkip");
        }

        private void button_切割道學習_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");
        }

        private void button_切割道學習_執行_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachTimer", true);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 4))
            {
                CheckLearnFixtureStatus = true;
            }
        }

        private void button_切割道學習_下一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void button_切割道學習_上一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckPrevious");
        }

        private void button_切割道學習_跳過_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckSkip");
        }

        private void button_靶點學習_下一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        private void button_靶點學習_跳過_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckSkip");
        }

        //+ By Max 20210225, 內靶掃描
        private void button_靶點學習_左上角切割位置_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + "右上角切割位置");
            int iPos = Convert.ToInt32(SYSPara.CallProc("Saw", "GetLeftTopCutPos"));
            label_靶點學習_左上角切割位置.Text = String.Format("{0}", iPos);
            groupBox__靶點學習_內靶.Enabled = false;
        }

        private void lvArmGrid_ItemActivate(object sender, EventArgs e)
        {
            if (SYSPara.OReadValue("bUseAlarmF").ToBoolean())
            {
                if (lvArmGrid.SelectedItems[0].SubItems.Count >= 5)
                {
                    ArmMtrl m_ArmMtrl = new ArmMtrl();
                    m_ArmMtrl.ErrorCode = Convert.ToInt32(lvArmGrid.SelectedItems[0].SubItems[3].Text);
                    m_ArmMtrl.Explain = lvArmGrid.SelectedItems[0].SubItems[4].Text;
                    AlarmForm.ShowAlarmForm(m_ArmMtrl);
                }
            }
        }

        private void button_基準線校正_確認_Click(object sender, EventArgs e)
        {

        }

        private void cbo_下刀點確認方向_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbo_下刀點確認.Items.Clear();
            if (cbo_下刀點確認方向.Text == "CH2")
            {
                for (int i = 1; i <= FormSet.mMainFlow.mSaw.PReadValue("Num_col").ToInt() + 1; i++)
                {
                    cbo_下刀點確認.Items.Add(i);
                }
            }

            if (cbo_下刀點確認方向.Text == "CH1")
            {
                for (int i = 1; i <= FormSet.mMainFlow.mSaw.PReadValue("Num_row").ToInt() + 1; i++)
                {
                    cbo_下刀點確認.Items.Add(i);
                }
            }
        }

        private void button_圓心校正_執行_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachTimer", true);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 7))
            {
                CheckOrientedCenterStatus = true;
            }
        }

        private void button_圓心校正_下一步_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachUserCheckOk");
        }

        #endregion 元件事件

        private void chkDestroyOnOff_CheckedChanged(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((CheckBox)sender).Text);

            if ((SYSPara.RunMode == RunModeDT.AUTO || SYSPara.RunMode == RunModeDT.HOME) && SYSPara.SysRun)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Warning:", GetDialogMsgText("Text_Main3_Machine_Running_Alarm"));
                //MessageBox.Show("機台運行中，無法執行此操作！");
                return;
            }

            SYSPara.CallProc("Saw", "DestroyStateChange", chkDestroyOnOff.Checked);
        }
        //v4.0.1.24通訊視覺畫出水平線
        private void checkCross_CheckedChanged(object sender, EventArgs e)
        {
            if (!(bool)CallProc("Saw", "SetCCDCmd", "CC", checkCross.Checked ? new string[] { "1" } : new string[] { "0" }))
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Vision_Busy_Alarm"));
                //MessageBox.Show("視覺忙碌中");
            }
        }
        //v4.0.1.24通訊視覺畫出垂直線
        private void checkValigm_CheckedChanged(object sender, EventArgs e)
        {
            if (!(bool)CallProc("Saw", "SetCCDCmd", "MKLC", checkValigm.Checked ? new string[] { "1" } : new string[] { "0" }))
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Vision_Busy_Alarm"));
                //MessageBox.Show("視覺忙碌中");
            }
        }
        //v4.0.1.25
        private void button_靶點學習_自動對焦_Click(object sender, EventArgs e)
        {
            if (!(bool)CallProc("Saw", "SetAutoFocus", "Teach"))
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_Main3_Auto_Focus_Prohibit_Alarm"));
                //MessageBox.Show("不可執行自動對焦");
            }
        }

        #region 生產開始平台自動回位
        //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
        public void SetAutoReMoveToRunPos()
        {
            if ((bool)CallProc("Saw", "GetAutoReMoveToRunPos"))
            {
                iGetReMovetmrStep = 10;
            }
        }

        //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
        public int iGetReMovetmrStep = 0;
        private void GetReMovetmr_Tick(object sender, EventArgs e)
        {
            switch (iGetReMovetmrStep)
            {
                case 0:
                    break;
                case 10:
                    if ((bool)CallProc("Saw", "GetReMoveToRunPos") == false)
                    {
                        iGetReMovetmrStep = 20;
                    }
                    else
                    {
                        SYSPara.LogSay(EnLoggerType.EnLog_SPC, "覆位還在進行中，不作覆位資料存取");
                        iGetReMovetmrStep = 90;
                    }
                    break;
                case 20:
                    {
                        if ((bool)CallProc("Saw", "SetReMoveToRunPos", true))
                        {
                            CallProc("Saw", "GetReMotorPos", true);
                            CallProc("Saw", "GetReMotorPra", true);
                            iGetReMovetmrStep = 30;
                        }
                        else
                        {
                            SYSPara.LogSay(EnLoggerType.EnLog_SPC, "切割過程中，不作生產覆位資料存取");
                            iGetReMovetmrStep = 90;
                        }
                    }
                    break;
                case 30:
                    if ((bool)CallProc("Saw", "GetReMotorPos", false))
                    {
                        iGetReMovetmrStep = 40;
                    }
                    break;
                case 40:
                    if ((bool)CallProc("Saw", "GetReMotorPra", false))
                    {
                        SYSPara.LogSay(EnLoggerType.EnLog_SPC, "自動生產覆位資料存取");
                        iGetReMovetmrStep = 90;
                    }
                    break;
                case 90:
                    iGetReMovetmrStep = 0;
                    GetReMovetmr.Enabled = false;
                    break;
            }
        }
        #endregion

        private void button_圓心校正_停止_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下" + ((Button)sender).Text);
            SYSPara.CallProc("Saw", "SetTeachFlowChartEnd");
        }

        private void button_單刀續切_Click(object sender, EventArgs e)
        {
            SYSPara.CallProc("Saw", "SetSingleBladeContinue");
        }

        private void button_換刀續切_Click(object sender, EventArgs e)
        {
            SYSPara.CallProc("Saw", "SetChangeBladeContinue");
            SYSPara.CallProc("Saw", "SetTeachTimer", true);
            if ((bool)SYSPara.CallProc("Saw", "SetTeachActionNo", 6))
            {
                if (TPTemp == null)
                {
                    //+ By Max 20210315, v4.0.1.54
                    ChangeLang(SYSPara.Lang.GetNowLanguage());
                    TPTemp = GetAutoTP(button_自動流程.Text);
                }
                if (TPTemp.Text != button_換刀流程.Text)
                {
                    ChoiceAuto(button_換刀流程);
                }
                CheckChangeKnifeStatus = true;
            }
        }

        private void button_Z增額_Click(object sender, EventArgs e)
        {
            Button oButton = (Button)sender;
            switch (oButton.Text)
            {
                case "+100":
                    oButton.Text = "+10";
                    break;
                case "+10":
                    oButton.Text = "+1";
                    break;
                case "+1":
                    oButton.Text = "+100";
                    break;
            }
        }

        #region Composite keys

        AutoResetEvent evt = new AutoResetEvent(false);

        private void MainF3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control == true)
            {
                evt.Set();
                timerCompositeKeys.Enabled = true;
            }

            if (e.KeyCode == Keys.F && e.Control == true)
            {
                if (evt.WaitOne(0))
                {
                    if (FormBorderStyle == System.Windows.Forms.FormBorderStyle.None) FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    else FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                }
            }
        }

        private void timerCompositeKeys_Tick(object sender, EventArgs e)
        {
            evt.Reset();
        }

        #endregion
    }
}
