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
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using KCSDK;
using Ionic.Zip;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ADSW11000
{
    public partial class MSS : Form
    {
        //錯誤掃描
        private bool ScanStopThread;
        private Thread thScanAlarm;
        private object LockScanAlarmObj = new object();
        private List<FlowChart> FlowChartList = new List<FlowChart>();

        //通用函數的動作執行緒
        private bool StopMotionThread;
        private Thread thMotion;
        private string MotionCall;
        private bool ModuleInitialOk = false;
        private bool bIntoFromManual = false;   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置

        private delegate void ShowPackageNameDelegate(bool Show); //Main Form Display PackageName Invoke 處理
        private delegate void ExitInitialFormDelegate(); //關閉歸零視窗
        private delegate void ChangeToInitialFormDelegate(); //切換至歸零視窗
        private delegate void ShowAutoStartTMDelegate(); //顯示開始生產的時間
        private delegate void ShowAutoEndTMDelegate(); //顯示結束生產的時間
        private delegate void SetControlSWDelegate(ControlButtonType type);
        private delegate void AutoEndDelegate();

        //安全門機制改善 2020/02/18 kent
        private MyTimer T01 = new MyTimer();

        public MSS()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            FC_INIT.TaskReset();

            thScanAlarm = new Thread(thScanAlarmProcess);
            ScanStopThread = false;
            thScanAlarm.Start();

            MotionCall = "";
            thMotion = new Thread(MotionProcess);
            StopMotionThread = false;
            thMotion.Start();

            //找出所有的FlowChart
            GetControls(this);
        }

        #region 私有函數

        //設定模組內部處理函式
        private void InitModuleProcess()
        {
            //建立模組內部處理函數
            foreach (BaseModuleInterface module in FormSet.ModuleList)
            {
                module.SetArmProcess(ArmProcess);
                module.SetLogProcess(LogProcess);
                //module.SetValueProcess(SetValueProcess);
            }
        }

        //模組內部處理
        private void ArmProcess(Control module, ArmDT pArm)
        {
            string ErrCode = string.Format("{0}{1:00}{2:00}", pArm.AlarmLevel, pArm.ModuleID, pArm.ErrorCode);

            //+ By Max 20190820
            bool bNeedHome = SYSPara.SetupReadValue("伺服錯誤需復歸").ToBoolean();
            if (bNeedHome && String.Compare(ErrCode, "E0023") == 0)
            {
                SYSPara.InitialOk = false;
            }

            if ((pArm.args != null) && (pArm.args.Count() > 0))
                SYSPara.Alarm.Say(ErrCode, module, pArm.args);
            else
                SYSPara.Alarm.Say(ErrCode, module);
        }
        private void LogProcess(Control module, LogDT pLog)
        {
            //EnLoggerType type = (EnLoggerType)pLog.Type;
            SYSPara.LogSay(pLog.Type, pLog.Msg);
        }
        //private void SetValueProcess(Control module, SetValueDT data)
        //{
        //    switch (data.Type)
        //    {
        //        case "Package":
        //            FormSet.mPackage.SetValue(data.TableName, data.DataName, data.FieldName, data.Value, data.IsAuto, data.ValueType);
        //            foreach (BaseModuleInterface pmodule in FormSet.ModuleList)
        //                pmodule.PackageData.SetValue(data.DataName, data.FieldName, data.Value);
        //            FormSet.mPackage.PackageData.SetValue(data.DataName, data.FieldName, data.Value);
        //            break;
        //        //case "Option":
        //        //    FormSet.mOption.SetValue(data.FieldName, data.Value, data.IsAuto);
        //        //    foreach (BaseModuleInterface pmodule in FormSet.ModuleList)
        //        //        FormSet.mOption.OptionDS.CopyTo(pmodule.OptionDS);
        //        //    FormSet.mOption.OptionDS.CopyTo(FormSet.mMainFlow.OptionDS);
        //        //    break;
        //    }
        //}

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

                if (myctr is FlowChart)
                    FlowChartList.Add((FlowChart)myctr);
            }
        }

        #endregion

        #region 各模組錯誤掃描

        private void thScanAlarmProcess()
        {
            while (!ScanStopThread)
            {
                lock (LockScanAlarmObj)
                {
                    #region 錯誤掃描

                    int i;

                    //狀態流程錯誤掃描
                    for (i = 0; i < FlowChartList.Count; i++)
                    {
                        FlowChart pflow = FlowChartList[i];
                        if (pflow.ErrID != 0)
                        {
                            if (pflow.ErrID == 62)
                            {
                                pflow.ErrID = 0;
                                continue;
                            }
                            string errcode = string.Format("E00{0:00}", pflow.ErrID);
                            if (pflow.ErrID == 62)
                                errcode = string.Format("W00{0:00}", pflow.ErrID);
                            SYSPara.Alarm.Say(errcode, this, pflow.Name);
                            pflow.ErrID = 0;
                        }
                    }

                    //主控流程錯誤掃描
                    if (FormSet.mMainFlow != null)
                    {
                        for (i = 0; i < FormSet.mMainFlow.FlowChartList.Count; i++)
                        {
                            FlowChart pflow = FormSet.mMainFlow.FlowChartList[i];
                            if (pflow.ErrID != 0)
                            {
                                if (pflow.ErrID == 62)
                                {
                                    pflow.ErrID = 0;
                                    continue;
                                }
                                string errcode = string.Format("E00{0:00}", pflow.ErrID);
                                if (pflow.ErrID == 62)
                                    errcode = string.Format("W00{0:00}", pflow.ErrID);
                                SYSPara.Alarm.Say(errcode, FormSet.mMainFlow, pflow.Name);
                                pflow.ErrID = 0;
                            }
                        }
                    }

                    #endregion

                    #region Log掃描

                    if (FormSet.mOption != null)
                    {
                        if (FormSet.mOption.IsLogExist())
                            SYSPara.LogSay("Setting", FormSet.mOption.GetLog());
                    }

                    if (FormSet.mPackage != null)
                    {
                        if (FormSet.mPackage.IsLogExist())
                            SYSPara.LogSay("Setting", FormSet.mPackage.GetLog());
                    }

                    if (FormSet.m內參設定 != null)
                    {
                        if (FormSet.m內參設定.IsLogExist())
                            SYSPara.LogSay("Setting", FormSet.m內參設定.GetLog());
                    }

                    #endregion

                    #region 使用者自訂視窗Log掃描

                    //if (FormSet.mTrayTable != null)
                    //{
                    //    if (FormSet.mTrayTable.IsLogExist())
                    //        SYSPara.LogSay(EnLoggerType.EnLog_OP, FormSet.mTrayTable.GetLog());
                    //}

                    #endregion

                    Thread.Sleep(1);
                }
            }
        }

        #endregion

        #region 使用者函數

        //常態執行
        public void AlwaysRun()
        {
            M_AlwaysRun();
        }

        //主流程
        public void MainRun()
        {
            FC_INIT.MainRun();
        }

        //解構
        public void DisposeTH()
        {
            ScanStopThread = true;
            thScanAlarm.Join();

            StopMotionThread = true;
            thMotion.Join();

            FormSet.mOption.OptionDS.DisposeTH();
            FormSet.mPackage.PreloadPackageDS.DisposeTH();
            FormSet.mPackage.PackageDS.DisposeTH();
            FormSet.m內參設定.OptionDS.DisposeTH();

            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.DisposeTH();
        }

        //取得指定模組參考
        public object GetModule(string ModuleName)
        {
            foreach (BaseModuleInterface obj in FormSet.ModuleList)
            {
                if (obj.Name == ModuleName)
                {
                    return obj;
                }
            }
            return null;
        }

        //依權限開啟功能
        public void ChangeUser()
        {
            btnScanIO.Visible = SYSPara.LoginUser >= UserType.utAdministrator;
            btnScanLang.Visible = SYSPara.LoginUser >= UserType.utAdministrator;
            btnUserM.Visible = ((SYSPara.LoginUser >= UserType.utEngineer) && (SYSPara.LoginMode == 1));
            button1.Visible = SYSPara.LoginUser >= UserType.utAdministrator;
            btnScanData.Visible = SYSPara.LoginUser >= UserType.utAdministrator;
            //button2.Visible = SYSPara.LoginUser >= UserType.utOperator;
            btnPM.Visible = SYSPara.LoginUser >= UserType.utEngineer;
        }

        #endregion

        #region 動作函數

        private void MotionProcess()
        {
            while (!StopMotionThread)
            {
                switch (MotionCall)
                {
                    case "M_Initial":
                        M_Initial();
                        MotionCall = "";
                        break;

                    case "M_HomeReset":
                        M_HomeReset();
                        MotionCall = "";
                        break;

                    case "M_ServoOn":
                        M_ServoOn();
                        MotionCall = "";
                        break;

                    case "M_ServoOff":
                        M_ServoOff();
                        MotionCall = "";
                        break;

                    case "M_ManualReset":
                        M_ManualReset();
                        MotionCall = "";
                        break;

                    case "M_RunReset":
                        M_RunReset();
                        MotionCall = "";
                        break;
                }

                Thread.Sleep(1);
            }
        }

        //軟體初始化
        private void M_Initial()
        {
            string name = "";
            try
            {
                name = "Setup";
                FormSet.m內參設定.Initial();
                name = "Option";
                FormSet.mOption.Initial();
                name = "Package";
                FormSet.mPackage.Initial();
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    module.ServoOn();
                    module.Initial();
                }
                name = "MainFlow";
                FormSet.mMainFlow.Initial();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0516", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //各模組歸零前置設定
        private void M_HomeReset()
        {
            string name = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                    {
                        module.HomeReset();
                        module.SetSpeed(true);
                    }
                }
                name = "MainFlow";
                FormSet.mMainFlow.HomeReset();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0511", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //各模組歸零
        private bool M_Home()
        {
            string name = "";
            try
            {
                bool bState = true;
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                        bState &= module.Home();
                }

                name = "MainFlow";
                bState &= FormSet.mMainFlow.Home();
                return bState;
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0513");
                MessageBox.Show(ex.StackTrace.ToString());
            }
            return false;
        }

        //馬達 Servo On
        private void M_ServoOn()
        {
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.ServoOn();
        }

        //馬達 Servo Off
        private void M_ServoOff()
        {
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.ServoOff();
        }

        //手動流程前置設定
        private void M_ManualReset()
        {
            string name = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                        module.ManualReset();
                }
                name = "MainFlow";
                FormSet.mMainFlow.ManualReset();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0514", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }
        private void M_ManualRun()
        {
            string name = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                        module.ManualRun();
                }
                name = "MainFlow";
                FormSet.mMainFlow.ManualRun();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0515", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //常態偵測
        private void M_AlwaysRun()
        {
            string name = "";
            try
            {
                name = "MainFlow";
                //FormSet.mMainFlow.ScanIO(); // v4.0.1.41 實體按鈕對應軟體click事件

                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                        module.AlwaysRun();
                }
                name = "MainFlow";
                FormSet.mMainFlow.AlwaysRun();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0502", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //自動模式前置設定
        private void M_RunReset()
        {
            string name = "";
            try
            {
                SYSPara.Lotend = false;
                SYSPara.LotendOk = false;
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                    {
                        module.RunReset();

                        //設定各模組速度
                        module.SetSpeed();
                    }
                }
                name = "MainFlow";
                FormSet.mMainFlow.RunReset();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0512", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //自動模式運行
        private void M_AutoRun()
        {
            string name = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    name = module.Name;
                    if (module.GetUseModule())
                        module.Run();
                }
                name = "MainFlow";
                FormSet.mMainFlow.Run();
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0503", name);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //持續運行至特定流程
        private void M_NonStopRun()
        {
            string modulename = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    modulename = module.Name;
                    if (module.GetUseContinueMode() && module.GetContinueRun())
                        module.Run();
                }
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0503", modulename);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //持續運行至特定流程
        private void M_NonStopHome()
        {
            string modulename = "";
            try
            {
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    modulename = module.Name;
                    if (module.GetUseContinueMode() && module.GetContinueRun())
                        module.Home();
                }
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0518", modulename);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        private void M_SetSpeed(int SpeedRate)
        {
            string modulename = "";
            try
            {
                SYSPara.OSetValue("機台速率", SpeedRate);
                foreach (BaseModuleInterface module in FormSet.ModuleList)
                {
                    modulename = module.Name;
                    module.SetSpeed();
                }
            }
            catch (Exception ex)
            {
                SYSPara.Alarm.Say("E0503", modulename);
                MessageBox.Show(ex.StackTrace.ToString());
            }
        }

        //停止動作
        public void M_Stop()
        {
            //停止計時
            MyTimer.TellTimerSystemStop();

            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.StopMotor();
        }  

        #endregion

        private FlowChart.FCRESULT FC_HOME_Run()
        {
            if (MotionCall != "")
                return FlowChart.FCRESULT.IDLE;

            ExitInitialFormDelegate mExitInitialF;
            ShowPackageNameDelegate mShowPackageName;
            switch (SYSPara.SysState)
            {
                case StateMode.SM_INIT_RUN:
                    if (SYSPara.SysRun)
                    {
                        if (M_Home())
                        {
                            SYSPara.InitialOk = true;
                            SYSPara.RunMode = RunModeDT.IDLE;

                            M_Stop();
                            SYSPara.SysRun = false;

                            if (SYSPara.PackageName != "")
                                SYSPara.SysState = StateMode.SM_AUTO_RESET;
                            else
                            {
                                switch (SYSPara.ScreenMode)
                                {
                                    case 0: //自動縮放
                                    case 1: //1440x900
                                        mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF1.ShowPackageName);
                                        FormSet.mMainF1.BeginInvoke(mShowPackageName, new object[] { false });
                                        break;
                                    case 2: //1024x768
                                        mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF2.ShowPackageName);
                                        FormSet.mMainF2.BeginInvoke(mShowPackageName, new object[] { false });
                                        break;
                                    case 3: //1366x768
                                        mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF3.ShowPackageName);
                                        FormSet.mMainF3.BeginInvoke(mShowPackageName, new object[] { false });
                                        break;
                                }

                                SYSPara.SysState = StateMode.SM_IDLE;
                            }

                            SYSPara.LogSay(EnLoggerType.EnLog_OP, "歸零完成");
                            SYSPara.Alarm.Say("I0508");

                            switch (SYSPara.ScreenMode)
                            {
                                case 0: //自動縮放
                                case 1: //1440x900
                                    mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                                    FormSet.mMainF1.BeginInvoke(mExitInitialF);
                                    break;
                                case 2: //1024x768
                                    mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                                    FormSet.mMainF2.BeginInvoke(mExitInitialF);
                                    break;
                                case 3: //1366x768
                                    mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                                    FormSet.mMainF3.BeginInvoke(mExitInitialF);
                                    break;
                            }
                            return FlowChart.FCRESULT.NEXT;
                        }
                    }
                    else
                    {
                        if (SYSPara.ErrorStop)
                            SYSPara.SysState = StateMode.SM_ALARM;
                        else
                            SYSPara.SysState = StateMode.SM_PAUSE;
                    }
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ABORT:
                    AutoEndDelegate autoend = new AutoEndDelegate(FormSet.mPackage.AutoEnd);
                    FormSet.mPackage.BeginInvoke(autoend);

                    SYSPara.PackageName = "";
                    SYSPara.RunMode = RunModeDT.IDLE;
                    M_Stop();
                    SYSPara.SysRun = false;
                    SYSPara.SysState = StateMode.SM_IDLE;

                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "取消歸零流程");
                    SYSPara.Alarm.Say("I0509");

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                            FormSet.mMainF1.BeginInvoke(mExitInitialF);

                            mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF1.ShowPackageName);
                            FormSet.mMainF1.BeginInvoke(mShowPackageName, new object[] { false });
                            break;
                        case 2: //1024x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                            FormSet.mMainF2.BeginInvoke(mExitInitialF);

                            mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF2.ShowPackageName);
                            FormSet.mMainF2.BeginInvoke(mShowPackageName, new object[] { false });
                            break;
                        case 3: //1366x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                            FormSet.mMainF3.BeginInvoke(mExitInitialF);

                            mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF3.ShowPackageName);
                            FormSet.mMainF3.BeginInvoke(mShowPackageName, new object[] { false });
                            break;
                    }

                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_PAUSE:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_INIT_RUN;
                    //特定模組持續動作
                    M_NonStopHome();
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ALARM:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_INIT_RUN;
                    //特定模組持續動作
                    M_NonStopHome();
                    return FlowChart.FCRESULT.IDLE;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_MANUAL_Run()
        {
            if (MotionCall != "")
                return FlowChart.FCRESULT.IDLE;

            ExitInitialFormDelegate mExitInitialF;
            SetControlSWDelegate mSetControlSW;
            switch (SYSPara.SysState)
            {
                case StateMode.SM_MANUAL_RUN:
                    if (SYSPara.SysRun)
                        M_ManualRun();
                    else
                    {
                        if (SYSPara.ErrorStop)
                            SYSPara.SysState = StateMode.SM_ALARM;
                        else
                            SYSPara.SysState = StateMode.SM_PAUSE;
                    }
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ABORT:
                    SYSPara.SysRun = false;
                    M_Stop();
                    SYSPara.ManualControlIO = false;
                    SYSPara.RunMode = RunModeDT.IDLE;
                    SYSPara.SysState = StateMode.SM_IDLE;

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                            FormSet.mMainF1.BeginInvoke(mExitInitialF);
                            break;
                        case 2: //1024x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                            FormSet.mMainF2.BeginInvoke(mExitInitialF);
                            break;
                        case 3: //1366x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                            FormSet.mMainF3.BeginInvoke(mExitInitialF);
                            break;
                    }
                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_CANCEL:
                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mSetControlSW = new SetControlSWDelegate(FormSet.mMainF1.SetControlSW);
                            FormSet.mMainF1.BeginInvoke(mSetControlSW, new object[] { ControlButtonType.None });
                            break;
                        case 2: //1024x768
                            mSetControlSW = new SetControlSWDelegate(FormSet.mMainF2.SetControlSW);
                            FormSet.mMainF2.BeginInvoke(mSetControlSW, new object[] { ControlButtonType.None });
                            break;
                        case 3: //1366x768
                            mSetControlSW = new SetControlSWDelegate(FormSet.mMainF3.SetControlSW);
                            FormSet.mMainF3.BeginInvoke(mSetControlSW, new object[] { ControlButtonType.None });
                            break;
                    }

                    SYSPara.SysRun = false;
                    M_Stop();
                    SYSPara.ManualControlIO = false;
                    SYSPara.RunMode = RunModeDT.IDLE;
                    SYSPara.SysState = StateMode.SM_IDLE;
                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_PAUSE:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ALARM:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_MANUAL_RESET:
                    MotionCall = "M_ManualReset";
                    SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    SYSPara.SysRun = false;
                    break;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_AUTO_RESET_Run()
        {
            try
            {
                SYSPara.RunSecond = 0;
                SYSPara.StopSecond = 0;

                MotionCall = "M_RunReset";
                //M_RunReset();
                SYSPara.SysState = StateMode.SM_AUTO_RUN;

                ShowAutoStartTMDelegate mShowAutoStartTM;

                switch (SYSPara.ScreenMode)
                {
                    case 0: //自動縮放
                    case 1: //1440x900
                        mShowAutoStartTM = new ShowAutoStartTMDelegate(FormSet.mMainF1.ShowAutoStartTM);
                        FormSet.mMainF1.BeginInvoke(mShowAutoStartTM);
                        break;
                    case 2: //1024x768
                        mShowAutoStartTM = new ShowAutoStartTMDelegate(FormSet.mMainF2.ShowAutoStartTM);
                        FormSet.mMainF2.BeginInvoke(mShowAutoStartTM);
                        break;
                    case 3: //1366x768
                        mShowAutoStartTM = new ShowAutoStartTMDelegate(FormSet.mMainF3.ShowAutoStartTM);
                        FormSet.mMainF3.BeginInvoke(mShowAutoStartTM);
                        break;
                }
            }
            catch (Exception)
            {
                SYSPara.Alarm.Say("E0512");
                SYSPara.SysState = StateMode.SM_IDLE;
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_AUTO_Run()
        {
            if (MotionCall != "")
                return FlowChart.FCRESULT.IDLE;

            if (SYSPara.HMIClose)
                return FlowChart.FCRESULT.CASE4;

            ShowAutoEndTMDelegate mShowAutoEndTM;
            switch (SYSPara.SysState)
            {
                case StateMode.SM_INIT_RESET:
                    {
                        AutoEndDelegate autoend = new AutoEndDelegate(FormSet.mPackage.AutoEnd);
                        FormSet.mPackage.BeginInvoke(autoend);

                        M_Stop();
                        SYSPara.SysRun = false;
                        SYSPara.InitialOk = false;

                        SYSPara.PackageName = ""; 
                        SYSPara.IsAutoMode = false;

                        MotionCall = "M_HomeReset";
                        //M_HomeReset();
                        SYSPara.RunMode = RunModeDT.HOME;
                        SYSPara.SysState = StateMode.SM_INIT_RUN;

                        SYSPara.LogSay(EnLoggerType.EnLog_OP, "進入歸零流程");

                        switch (SYSPara.ScreenMode)
                        {
                            case 0: //自動縮放
                            case 1: //1440x900
                                mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF1.ShowAutoEndTM);
                                FormSet.mMainF1.BeginInvoke(mShowAutoEndTM);
                                break;
                            case 2: //1024x768
                                mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF2.ShowAutoEndTM);
                                FormSet.mMainF2.BeginInvoke(mShowAutoEndTM);
                                break;
                            case 3: //1366x768
                                mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF3.ShowAutoEndTM);
                                FormSet.mMainF3.BeginInvoke(mShowAutoEndTM);
                                break;
                        }
                    }
                    return FlowChart.FCRESULT.CASE1;
                case StateMode.SM_MANUAL_RESET:
                    SYSPara.ManualControlIO = true;
                    SYSPara.SysRun = false;
                    M_Stop();

                    MotionCall = "M_ManualReset";
                    //M_ManualReset();

                    bIntoFromManual = true;   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
                    FormSet.mMainF3.SetAutoReMoveToRunPos();   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置

                    SYSPara.RunMode = RunModeDT.MANUAL;
                    SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_AUTO_RUN:
                    {
                        if (!SYSPara.InitialOk)
                        {
                            SYSPara.Alarm.Say("E0519");
                            return FlowChart.FCRESULT.IDLE;
                        }
                        if (SYSPara.SysRun)
                        {
                            M_AutoRun();
                            if (SYSPara.LotendOk)
                            {
                                AutoEndDelegate autoend = new AutoEndDelegate(FormSet.mPackage.AutoEnd);
                                FormSet.mPackage.BeginInvoke(autoend);

                                FormSet.mLotEndF.LotEnd();
                                return FlowChart.FCRESULT.CASE2;
                            }
                        }
                        else
                        {
                            if (SYSPara.ErrorStop)
                                SYSPara.SysState = StateMode.SM_ALARM;
                            else
                            {
                                if (bIntoFromManual == false)   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
                                {
                                    FormSet.mMainF3.SetAutoReMoveToRunPos();
                                }
                                bIntoFromManual = false;   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置

                                SYSPara.SysState = StateMode.SM_PAUSE;
                            }
                        }
                        return FlowChart.FCRESULT.IDLE;
                    }
                case StateMode.SM_PAUSE:
                    if (SYSPara.SysRun)
                    {
                        //安全門機制改善 2020/02/18 kent
                        if (SYSPara.安全門開啟)
                        {
                            T01.Restart();
                            return FlowChart.FCRESULT.CASE3;
                        }
                        SYSPara.SysState = StateMode.SM_AUTO_RUN;
                    }

                    //特定模組持續動作
                    M_NonStopRun();
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ALARM:
                    if (SYSPara.SysRun)
                    {
                        //安全門機制改善 2020/02/18 kent
                        if (SYSPara.安全門開啟)
                        {
                            T01.Restart();
                            return FlowChart.FCRESULT.CASE3;
                        }
                        SYSPara.SysState = StateMode.SM_AUTO_RUN;
                    }

                    //特定模組持續動作
                    M_NonStopRun();
                    return FlowChart.FCRESULT.IDLE;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        //安全門機制改善
        double OrgSysSpeedRate = 100;
        private FlowChart.FCRESULT FC_DELAYRUN_Run()
        {
            if (T01.On(5000))
            {
                //安全門機制改善
                int iEQPSlowRunCycle = SYSPara.SetupReadValue("EQPSlowRunCycle").ToInt();
                FlowChart.ResetRunCycle(iEQPSlowRunCycle);
                SYSPara.安全門開啟 = false;

                int iEQPSlowRunSpeed = SYSPara.SetupReadValue("EQPSlowRunSpeed").ToInt();

                M_SetSpeed(iEQPSlowRunSpeed);

                SYSPara.ErrorStop = false;
                SYSPara.MusicOn = true;
                SYSPara.Alarm.ClearAll();
                //SYSPara.SysRun = true;
                SYSPara.Alarm.Say("W0602");
                return FlowChart.FCRESULT.NEXT;
            }

            SYSPara.Alarm.Say("W0601");
            //特定模組持續動作
            M_NonStopRun();
            return default(FlowChart.FCRESULT);
        }

        //安全門機制改善
        private FlowChart.FCRESULT FC_SLOW_Run()
        {
            if (SYSPara.HMIClose)
            {
                M_SetSpeed((int)OrgSysSpeedRate);
                FlowChart.ResetRunCycle(0);

                return FlowChart.FCRESULT.NEXT;
            }

            switch (SYSPara.SysState)
            {
                case StateMode.SM_INIT_RESET:
                    M_SetSpeed((int)OrgSysSpeedRate);
                    FlowChart.ResetRunCycle(0);

                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_MANUAL_RESET:
                    M_Stop();
                    M_SetSpeed((int)OrgSysSpeedRate); //還原機台速率
                    FlowChart.ResetRunCycle(0);

                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_AUTO_RUN:
                    {
                        if (!SYSPara.InitialOk)
                        {
                            SYSPara.Alarm.Say("E0519");
                            return FlowChart.FCRESULT.IDLE;
                        }
                        if (SYSPara.SysRun)
                        {
                            M_AutoRun();
                            if (SYSPara.Lotend)
                            {
                                M_SetSpeed((int)OrgSysSpeedRate); //還原機台速率
                                //FormSet.mLotEndF.LotEnd();
                                return FlowChart.FCRESULT.NEXT;
                            }
                            if (FlowChart.IsAllSlowCycleDone)
                            {
                                //M_Stop();
                                M_SetSpeed((int)OrgSysSpeedRate); //還原機台速率
                                FlowChart.ResetRunCycle(0);
                                SYSPara.Alarm.ClearAll();
                                return FlowChart.FCRESULT.NEXT;
                            }
                        }
                        else
                        {
                            if (SYSPara.ErrorStop)
                                SYSPara.SysState = StateMode.SM_ALARM;
                            else
                                SYSPara.SysState = StateMode.SM_PAUSE;
                        }
                        return FlowChart.FCRESULT.IDLE;
                    }
                case StateMode.SM_PAUSE:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_AUTO_RUN;

                    //特定模組持續動作
                    M_NonStopRun();
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ALARM:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_AUTO_RUN;

                    //特定模組持續動作
                    M_NonStopRun();
                    return FlowChart.FCRESULT.IDLE;
            }
            SYSPara.Alarm.Say("W0602");
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT flowChart1_Run()
        {
            if (MotionCall != "")
                return FlowChart.FCRESULT.IDLE;

            ExitInitialFormDelegate mExitInitialF;
            //SetControlSWDelegate func;
            //SYSPara.SysRun = false;
            switch (SYSPara.SysState)
            {
                case StateMode.SM_MANUAL_RUN:
                    if (SYSPara.SysRun)
                        M_ManualRun();
                    else
                    {
                        if (SYSPara.ErrorStop)
                            SYSPara.SysState = StateMode.SM_ALARM;
                        else
                            SYSPara.SysState = StateMode.SM_PAUSE;
                    }
                    return FlowChart.FCRESULT.IDLE;
                //+ By Max For SECS SM_ABORT -> SM_CANCEL
                case StateMode.SM_CANCEL:
                    SYSPara.SysRun = false;
                    M_Stop();
                    SYSPara.ManualControlIO = false;
                    SYSPara.RunMode = RunModeDT.AUTO;
                    SYSPara.SysState = StateMode.SM_AUTO_RUN;

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                            FormSet.mMainF1.BeginInvoke(mExitInitialF);
                            break;
                        case 2: //1024x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                            FormSet.mMainF2.BeginInvoke(mExitInitialF);
                            break;
                        case 3: //1366x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                            FormSet.mMainF3.BeginInvoke(mExitInitialF);
                            break;
                    }
                    return FlowChart.FCRESULT.NEXT;
                case StateMode.SM_PAUSE:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_ALARM:
                    if (SYSPara.SysRun)
                        SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.IDLE;
                case StateMode.SM_MANUAL_RESET:
                    MotionCall = "M_ManualReset";
                    SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    SYSPara.SysRun = false;
                    break;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            tmRefresh.Enabled = false;

            tbState.Value = SYSPara.SysState.ToString();

            if (SYSPara.LoginMode == 1)
                btnUserM.Visible = (SYSPara.LoginUser >= UserType.utEngineer);
            else
                btnUserM.Visible = false;

            tmRefresh.Enabled = true;
        }

        private FlowChart.FCRESULT FC_LOAD_PACKAGE_RESET_Run()
        {
            try
            {
                SYSPara.SysRun = false;
                ExitInitialFormDelegate mExitInitialF;
                switch (SYSPara.SysState)
                {
                    case StateMode.SM_PACKAGELOAD_OK:
                        ShowPackageNameDelegate mShowPackageName;
                        switch (SYSPara.ScreenMode)
                        {
                            case 0: //自動縮放
                            case 1: //1440x900
                                mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF1.ShowPackageName);
                                FormSet.mMainF1.BeginInvoke(mShowPackageName, new object[] { true });
                                break;
                            case 2: //1024x768
                                mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF2.ShowPackageName);
                                FormSet.mMainF2.BeginInvoke(mShowPackageName, new object[] { true });
                                break;
                            case 3: //1366x768
                                mShowPackageName = new ShowPackageNameDelegate(FormSet.mMainF3.ShowPackageName);
                                FormSet.mMainF3.BeginInvoke(mShowPackageName, new object[] { true });
                                break;
                        }

                        return FlowChart.FCRESULT.NEXT;
                    case StateMode.SM_ABORT:
                        SYSPara.SysState = StateMode.SM_IDLE;
                        SYSPara.PackageName = "";

                        switch (SYSPara.ScreenMode)
                        {
                            case 0: //自動縮放
                            case 1: //1440x900
                                mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                                FormSet.mMainF1.BeginInvoke(mExitInitialF);
                                break;
                            case 2: //1024x768
                                mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                                FormSet.mMainF2.BeginInvoke(mExitInitialF);
                                break;
                            case 3: //1366x768
                                mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                                FormSet.mMainF3.BeginInvoke(mExitInitialF);
                                break;
                        }

                        return FlowChart.FCRESULT.CASE1;
                }
            }
            catch (Exception)
            {
                SYSPara.Alarm.Say("E0510");
                SYSPara.SysState = StateMode.SM_IDLE;
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_LOAD_PACKAGE_Run()
        {
            ExitInitialFormDelegate mExitInitialF;

            try
            {
                SYSPara.SysRun = false;
                SYSPara.InitialOk = false;

                //執行開批流程
                FormSet.mLotStartF.LotStart();
            }
            catch (Exception)
            {
                SYSPara.Alarm.Say("E0517");
                SYSPara.SysState = StateMode.SM_IDLE;

                switch (SYSPara.ScreenMode)
                {
                    case 0: //自動縮放
                    case 1: //1440x900
                        mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                        FormSet.mMainF1.BeginInvoke(mExitInitialF);
                        break;
                    case 2: //1024x768
                        mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                        FormSet.mMainF2.BeginInvoke(mExitInitialF);
                        break;
                    case 3: //1366x768
                        mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                        FormSet.mMainF3.BeginInvoke(mExitInitialF);
                        break;
                }

                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.NEXT;
        }

        private void btnScanIO_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「掃描模組IO」");

            IOMapping io = new IOMapping();
            io.ScanIOToFile(SYSPara.SysDir + @"\LocalData\IOTable.xls", FormSet.ModuleList);
            MessageBox.Show("IOTable.xls 儲存完成");
        }

        private void btnScanLang_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「語系掃描」");

            SYSPara.Lang.SaveToFile();
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.SaveLanguageToFile();

            MessageBox.Show("語系儲存完成");
        }

        private FlowChart.FCRESULT FC_INIT_Run()
        {
            if (SYSPara.HMIReady)
            {
                //設備軟體初始化用
                SYSPara.IsAutoMode = false;
                SYSPara.PackageName = "";

                SYSPara.SysRun = false;
                SYSPara.InitialOk = false;
                SYSPara.SysState = StateMode.SM_IDLE;
                SYSPara.RunMode = RunModeDT.IDLE;

                ModuleInitialOk = false;
                ProVMessage msg = new ProVMessage();
                act_ModuleInitial.AddMessage(msg);               

                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_IDLE_Run()
        {
            SYSPara.SysRun = false;
            SYSPara.ManualControlIO = false;

            if (SYSPara.HMIClose)
                return FlowChart.FCRESULT.CASE4;

            switch (SYSPara.SysState)
            {
                case StateMode.SM_PACKAGELOAD_RESET:
                    return FlowChart.FCRESULT.CASE1;
                case StateMode.SM_MANUAL_RESET:
                    SYSPara.RunMode = RunModeDT.MANUAL;
                    SYSPara.ManualControlIO = true;
                    MotionCall = "M_ManualReset";
                    SYSPara.SysState = StateMode.SM_MANUAL_RUN;
                    return FlowChart.FCRESULT.CASE2;
                case StateMode.SM_AUTO_RESET:
                    SYSPara.IsAutoMode = true;
                    SYSPara.RunMode = RunModeDT.AUTO;
                    return FlowChart.FCRESULT.CASE3;
                case StateMode.SM_INIT_RESET:
                    SYSPara.IsAutoMode = false;
                    SYSPara.PackageName = "";

                    MotionCall = "M_HomeReset";
                    SYSPara.InitialOk = false;
                    SYSPara.RunMode = RunModeDT.HOME;
                    SYSPara.SysState = StateMode.SM_INIT_RUN;
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private void btnUserM_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「使用者管理」");

            FormSet.mUserM = new UserManagment();
            FormSet.mUserM.LoadUser();
            FormSet.mUserM.ShowDialog();
        }

        private FlowChart.FCRESULT flowChart2_Run()
        {
            M_ServoOff();
            SYSPara.HMIReady = false;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT flowChart4_Run()
        {
            ExitInitialFormDelegate mExitInitialF;

            switch (FormSet.mLotStartF.LotStartResult)
            {
                case ActionDT.ResultType.OK:
                    MotionCall = "M_HomeReset";
                    SYSPara.RunMode = RunModeDT.HOME;
                    SYSPara.SysState = StateMode.SM_INIT_RUN;

                    //SYSPara.Alarm.Say("I0519");
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "進入歸零流程");

                    //+ By Noke 20200925 每開批就檢查是否要備份並刪除資料
                    SYSPara.logDB.BackupOneYearData();

                    ChangeToInitialFormDelegate mChangeToInitial;
                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mChangeToInitial = new ChangeToInitialFormDelegate(FormSet.mMainF1.ChangeToInitialForm);
                            FormSet.mMainF1.BeginInvoke(mChangeToInitial);
                            break;
                        case 2: //1024x768
                            mChangeToInitial = new ChangeToInitialFormDelegate(FormSet.mMainF2.ChangeToInitialForm);
                            FormSet.mMainF2.BeginInvoke(mChangeToInitial);
                            break;
                        case 3: //1366x768
                            mChangeToInitial = new ChangeToInitialFormDelegate(FormSet.mMainF3.ChangeToInitialForm);
                            FormSet.mMainF3.BeginInvoke(mChangeToInitial);
                            break;
                    }

                    //+ By Max For SECS
                    int idx = SYSPara.OReadValue("CommProtocol").ToInt();
                    if (idx == 1) //SECS
                    {
                        SYSPara.SysRun = true;
                    }

                    return FlowChart.FCRESULT.NEXT;

                case ActionDT.ResultType.NG:
                    //SYSPara.Alarm.Say("w0517");
                    SYSPara.SysState = StateMode.SM_IDLE;

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                            FormSet.mMainF1.BeginInvoke(mExitInitialF);
                            break;
                        case 2: //1024x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                            FormSet.mMainF2.BeginInvoke(mExitInitialF);
                            break;
                        case 3: //1366x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                            FormSet.mMainF3.BeginInvoke(mExitInitialF);
                            break;
                    }
                    return FlowChart.FCRESULT.CASE1;
            }

            switch (SYSPara.SysState)
            {
                case StateMode.SM_ABORT:
                    SYSPara.SysState = StateMode.SM_IDLE;
                    SYSPara.PackageName = "";

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF1.ExitInitialForm);
                            FormSet.mMainF1.BeginInvoke(mExitInitialF);
                            break;
                        case 2: //1024x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF2.ExitInitialForm);
                            FormSet.mMainF2.BeginInvoke(mExitInitialF);
                            break;
                        case 3: //1366x768
                            mExitInitialF = new ExitInitialFormDelegate(FormSet.mMainF3.ExitInitialForm);
                            FormSet.mMainF3.BeginInvoke(mExitInitialF);
                            break;
                    }
                    return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT flowChart3_Run()
        {
            ShowAutoEndTMDelegate mShowAutoEndTM;
            switch (FormSet.mLotEndF.LotEndResult)
            {
                case ActionDT.ResultType.OK:
                    SYSPara.IsAutoMode = false;
                    SYSPara.PackageName = "";

                    SYSPara.RunMode = RunModeDT.IDLE;
                    SYSPara.SysState = StateMode.SM_IDLE;

                    SYSPara.Alarm.Say("I0505"); //結批完成

                    switch (SYSPara.ScreenMode)
                    {
                        case 0: //自動縮放
                        case 1: //1440x900
                            mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF1.ShowAutoEndTM);
                            FormSet.mMainF1.BeginInvoke(mShowAutoEndTM);
                            break;
                        case 2: //1024x768
                            mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF2.ShowAutoEndTM);
                            FormSet.mMainF2.BeginInvoke(mShowAutoEndTM);
                            break;
                        case 3: //1366x768
                            mShowAutoEndTM = new ShowAutoEndTMDelegate(FormSet.mMainF3.ShowAutoEndTM);
                            FormSet.mMainF3.BeginInvoke(mShowAutoEndTM);
                            break;
                    }
                    return FlowChart.FCRESULT.NEXT;

                case ActionDT.ResultType.NG:
                    SYSPara.IsAutoMode = false;
                    SYSPara.PackageName = "";

                    SYSPara.RunMode = RunModeDT.IDLE;
                    SYSPara.SysState = StateMode.SM_IDLE;

                    SYSPara.Alarm.Say("w0517");
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        //+ By Max For SECS
        private void btnScanData_Click(object sender, EventArgs e)
        {
            String SCANDATA = SYSPara.SysDir + @"\Localdata\SCANDATA.xls";

            if (File.Exists(SCANDATA))
                File.Delete(SCANDATA);

            try
            {
                using (FileStream fs = new FileStream(SCANDATA, FileMode.CreateNew, FileAccess.Write))
                {
                    //建立Excel 2003檔案
                    IWorkbook workbook = new HSSFWorkbook();

                    ISheet sheet = workbook.CreateSheet("SCANDATA");
                    int LineCNT = 0;
                    //寫入模組表頭
                    sheet.CreateRow(LineCNT);
                    HSSFCellStyle cs = (HSSFCellStyle)workbook.CreateCellStyle();
                    cs.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cs.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cs.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cs.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cs.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    cs.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                    cs.FillPattern = FillPattern.SolidForeground;

                    sheet.GetRow(LineCNT).CreateCell(0).SetCellValue("參數名稱");
                    sheet.GetRow(LineCNT).CreateCell(1).SetCellValue("資料類型");

                    foreach (HSSFCell cell in sheet.GetRow(LineCNT).Cells)
                    {
                        cell.CellStyle = cs;
                    }
                    LineCNT++;

                    int index = 1;
                    foreach (DataManagement.FieldData fd in FormSet.mOption.OptionDS.SettingList)
                    {
                        sheet.CreateRow(index);
                        string name = fd.DataName;
                        sheet.GetRow(index).CreateCell(0).SetCellValue(fd.DataName);
                        sheet.GetRow(index).CreateCell(1).SetCellValue("通用設定");
                        index++;
                    }

                    foreach (DataManagement.FieldData fd in FormSet.mPackage.PackageDS.PackageList[0].Fields)
                    {
                        sheet.CreateRow(index);
                        string name = fd.DataName;
                        sheet.GetRow(index).CreateCell(0).SetCellValue(fd.DataName);
                        sheet.GetRow(index).CreateCell(1).SetCellValue("產品設定");
                        index++;
                    }

                    sheet.AutoSizeColumn(0);
                    sheet.AutoSizeColumn(1);

                    workbook.Write(fs);
                    fs.Close();
                    workbook = null;

                    MessageBox.Show("SCANDATA.xls 儲存完成");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("SCANDATA.xls Writing Failed!!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            內參密碼輸入 frm = new 內參密碼輸入();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FormSet.m內參設定.RefreshAllData("Setup");
                FormSet.m內參設定.InitialSubPackage();
                FormSet.m內參設定.ShowDialog();
            }
        }

        private FlowChart.FCRESULT flowChart5_Run()
        {
            if (ModuleInitialOk)
                return FlowChart.FCRESULT.NEXT;
            return default(FlowChart.FCRESULT);
        }

        private ProVMessage act_ModuleInitial_OnActorRun(ProVMessage msg)
        {
            M_Initial();

            FormSet.ModuleList.Sort((x, y) => { return -((BaseModuleInterface)x).SortIndex.CompareTo(((BaseModuleInterface)y).SortIndex); });
            FormSet.mModuleContainer.Initial();

            SYSPara.Lang.ChangeLanguage("tw");

            InitModuleProcess();
            ModuleInitialOk = true;
            return default(ProVMessage);
        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private void btnPM_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「預防保養項目」");
            PMSettingF pmSetForm = new PMSettingF(true);
            pmSetForm.ShowDialog();
        }
    }
}
