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

namespace ADSW11000
{
    public partial class MainF2 : Form
    {
        #region 架構使用 (參數定義)

        private Object mtpMSS = null;
        private Object mtpMainFlow = null;
        private Object mtpGEM = null;

        //2020/02/05
        private bool frm異常通知顯示中 = false;
        private Point 異常通知畫面位置;
        private bool mMouseDown = false;
        
        #endregion

        public MainF2()
        {
            InitializeComponent();

            #region 架構使用 (參數初始化)

            //初始變數
            Width = 1024;
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
            tpMainFlow.Controls.Add(FormSet.mMainFlow);
            FormSet.mMainFlow.Dock = DockStyle.Fill;

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
            #endregion
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

        #region 架構使用 (Public)

        //顯示產品名稱
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

        //關閉歸零視窗
        public void ExitInitialForm()
        {
            AllFormHide();
            SetControlSW(ControlButtonType.None);
        }

        //切換至歸零視窗
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

        //顯示作業開始時間
        public void ShowAutoStartTM()
        {
            lbOperationStartTM.Value = DateTime.Now.ToString("G");
            lbOperationEndTM.Value = "";
        }

        //顯示作業結束時間
        public void ShowAutoEndTM()
        {
            //顯示作業結束時間
            lbOperationEndTM.Value = DateTime.Now.ToString("G");
        }

        #endregion

        #region 架構使用 (Private)

        //使用者登入
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

        //更換使用者需要處理的資料
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

        //將Panel上的所有Form給隱藏
        private void AllFormHide()
        {
            foreach (Control control in pnlContainer.Controls)
                if (control is Form)
                    ((Form)control).Parent = null;
        }

        //切換 UI 畫面
        private void PageChange(object sender, EventArgs e)
        {
            int tag = Convert.ToInt32(((Button)sender).Tag);
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
                    break;
                case 2: //手動操作
                    AllFormHide();
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「手動操作」");

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

                    SYSPara.SysRun = false;
                    FormSet.mMSS.M_Stop();

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

                    SYSPara.SysRun = false;
                    FormSet.mMSS.M_Stop();

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
                            break;
                        case "en":
                            SYSPara.Lang.ChangeLanguage("tw");
                            foreach (BaseModuleInterface module in FormSet.ModuleList)
                                module.ChangeLanguage("tw");
                            SYSPara.Alarm.SetLanguage("tw");
                            break;
                    }
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

        //控制按鈕致能
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

        //設備狀態顯示
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

        #endregion

        //警報後置處理
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

            SYSPara.LogSay("SPC", armMtrl.Explain);
        }

        private void MainF1_Load(object sender, EventArgs e)
        {
            #region 架構使用 (參數初始化)
            //設定視窗使用的語系
            SYSPara.Lang.ChangeLanguage("tw");
            lbVersion.Text = string.Format("{1} Version {0}", FileVersionInfo
                .GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString(), SYSPara.EQID);
            lbProjectName.Text = this.GetType().Assembly.GetName().Name;

            tmUIRefresh.Enabled = true;

            SYSPara.MaxScanTime = 0;
            SYSPara.MusicOn = true;
            SYSPara.HMIReady = true;
            SYSPara.HMIClose = false;
            SYSPara.InitialOk = false;

            #endregion

            #region 使用者修改 (顯示元件前置設定)

            //FormSet.mPackage.Initial();
            //FormSet.mOption.Initial();
            //FormSet.m內參設定.Initial();

            //安全門機制改善
            //傳送發生安全門或緊急停止開關觸發時回呼的委派函式
            if (SYSPara.SetupReadValue("安全門機制設定").ToInt() == 2)
                SYSPara.CallProc("MAA", "RecordSaftyCheckDelegate", new Action<int>(SaftyActionDelegate));

            #endregion
        }

        private void MainF1_Shown(object sender, EventArgs e)
        {
            //使用者登入
            UserLogin();
        }

        #region 架構使用 (按鈕功能)

        private void btnStart_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「生產開始」");

            SYSPara.ErrorStop = false;
            SYSPara.MusicOn = true;
            SYSPara.Alarm.ClearAll();
            SYSPara.SysRun = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (SYSPara.SysState == StateMode.SM_ALARM)
            {
                SYSPara.ErrorStop = false;
                SYSPara.SysState = StateMode.SM_PAUSE;
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
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("{0} 登出",GetNowUser()));

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
                DialogResult result = MessageBox.Show("是否要離開程式?", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.OK:
                        if (SYSPara.RunMode == RunModeDT.MANUAL)
                            SYSPara.SysState = StateMode.SM_CANCEL;

                        SYSPara.Alarm.ClearAll();
                        Thread.Sleep(200);

                        //關閉警報系統
                        SYSPara.Alarm.DisposeTH();
                        Thread.Sleep(300);

                        //關閉HMI前的動作
                        SYSPara.HMIClose = true;
                        while (SYSPara.HMIReady);

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

        #endregion

        private void tmUIRefresh_Tick(object sender, EventArgs e)
        {
            tmUIRefresh.Enabled = false;

            #region 架構使用 (定時顯示)

            //顯示目前時間
            lbNowTime.Text = DateTime.Now.ToString("G");

            lbIdleTM.Value = UserSnippet.ToTimeString(SYSPara.IdleTM);
            lbHomeTM.Value = UserSnippet.ToTimeString(SYSPara.HomeTM);
            lbManualTM.Value = UserSnippet.ToTimeString(SYSPara.ManualTM);

            //顯示運行時相關資料
            if (SYSPara.RunMode == RunModeDT.AUTO)
            {
                lbRunTM.Value = UserSnippet.ToTimeString(SYSPara.RunSecond);
                lbPauseTM.Value = UserSnippet.ToTimeString(SYSPara.StopSecond);
            }

            lbScanTM.Value = string.Format("{0:0.000}", SYSPara.ScanTimeEx);
            lbScanCNT.Value = SYSPara.ScanTime.ToString();

            //設備狀態顯示
            DisplaySysState();

            //顯示三色燈LED
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

            #endregion

            #region 使用者修改

            #endregion

            tmUIRefresh.Enabled = true;
        }

        //安全門機制改善
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
    }
}
