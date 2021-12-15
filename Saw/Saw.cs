using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using ProVIFM;
using ProVLib;
using SAWLib;
using KCSDK;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CommonObj;

using System.Reflection;
using System.Resources;
using System.Globalization;
using System.IO;


namespace Saw
{
    /// <summary>
    /// 資料讀取的方式: 
    /// 1. PReadValue => 產品相關資料的讀取
    /// 2. OReadValue => 通用設定資料的讀取
    /// 3. SReadValue => 模組設定資料的讀取
    /// 
    /// Log輸出的方式:
    /// 1. public void LogSay(EnLoggerType mType, params string[] msg);
    ///    LogMode=0 msg固定取用陣列第一項目值
    ///    LogMode=1 msg依類型不同取用項目值
    ///    Type="OP" 取用陣列第一項目值
    ///    Type="Alarm" [0]:模組名稱 [1]:錯誤代碼 [2]:錯誤內容 [3]:持續時間 (*由ShowAlarm函式處理)
    ///    Type="Comm" [0]:模組名稱 [1]:記錄內容
    ///    Type="SPC" [0~N]:欄位值
    /// 
    /// Alarm輸出的方式: 
    /// 1. public void ShowAlarm(string AlarmLevel, int ErrorCode);
    /// 2. public void ShowAlarm(string AlarmLevel, int ErrorCode, params object[] args);
    /// AlarmLevel: "E/e","W/w","I/i"
    /// 
    /// 常用判斷:
    /// 1. IsSimulation() => 是否為模擬狀態 0:實機 1:亂數模擬 2:純模擬
    /// 2. mCanHome => 是否可歸零
    /// 3. mHomeOk => 歸零完畢時，需設定此變數為 true
    /// 4. mLotend => 是否可結批
    /// 5. mLotendOk => 結批完畢時，需設定此變數為 true
    /// 
    /// 常用函式:
    /// 1. SetCanLotEnd: 通知模組可以進行結批
    /// 2. GetLotEndOk: 取得模組是否結批完成
    /// 3. GetLotend: 取得模組是否正在進行結批
    /// 4. SetLotEndOk: 設定模組結批完成
    /// 5. IsSimulation: 取得模組是否在模擬狀態
    /// 6. SetCanHome: 通知模組可以進行歸零
    /// 7. GetHomeOk: 取得模組歸零結果
    /// 8. SReadValue: 取得模組內部指定欄位資料
    /// 9. SSetValue: 設定模組內部指定欄位資料
    /// 10. SaveFile: 通知模組進行資料存檔
    /// 11. PReadValue: 取得產品設定指定表格/欄位資料
    /// 12. OReadValue: 取得通用設定指定欄位資料
    /// 13. GetUseModule: 取得模組開關設定
    /// 14. GetModuleID: 取得模組編號設定
    /// 
    /// 常用資料變數:
    /// 1. PackageName: 目前模組正在使用的產品名稱
    /// 2. IsAutoMode: 模組是否處理Auto模式
    /// 3. RunTM: MyTimer型別，可用於Auto流程
    /// 4. HomeTM: MyTimer型別，可用於Home流程
    /// </summary>
    /// 
    public partial class Saw : BaseModuleInterface
    {
        public Saw()
        {
            InitializeComponent();

            CreateComponentList();
        }

        #region 繼承函數

        //模組解構使用
        public override void DisposeTH()
        {
            TCPIPClose();
            mCMS.Dispose();
            base.DisposeTH();
        }

        public override void AfterShowPage()
        {
            base.AfterShowPage();
            //+ By Max 20210331, v4.0.1.60, 登入人員不是Designer時隱藏
            if (NowUser != UserType.utProV)
            {
                groupBox28.Visible = false; //測試功能群組功能登入人員不是Designer時隱藏
                dCheckBox26.Visible = false; //轉正確認功能開啟CheckBox
                dFieldEdit52.Visible = false; //轉正確認功能重試次數欄位
                dFieldEdit89.Visible = false; //刀痕檢測出入刀點卡控
            }
            else
            {
                groupBox28.Visible = true;
                dCheckBox26.Visible = true;
                dFieldEdit52.Visible = true;
                dFieldEdit89.Visible = true;
            }
        }

        private void InitialSpindle(enSpindleSel sel, int nType)
        {
            Spindle sp = sel == enSpindleSel.Z1 ? Z1Spindle : Z2Spindle;
            enSpindleCtrlType enType = (enSpindleCtrlType)nType;

            switch (enType)
            {
                case enSpindleCtrlType.預設訊號控制_IO:
                    sp.UseIO = true;
                    sp.AirPressure = null;
                    sp.CoolingFlow = null;
                    sp.OPEnable = null;
                    sp.SwitchOn = null;
                    sp.SpindleRPM = sel == enSpindleSel.Z1 ? analogOut_Z1主軸轉速類比控制 : analogOut_Z2主軸轉速類比控制;
                    sp.SpindleLocker = null;
                    break;

                case enSpindleCtrlType.進階訊號控制_IO:
                    sp.UseIO = true;
                    sp.AirPressure = inBit_主軸空壓正常;
                    sp.CoolingFlow = inBit_冰水機正常運轉中;
                    sp.OPEnable = sel == enSpindleSel.Z1 ? outBit_Z1_REST_ENABLED : outBit_Z2_REST_ENABLED;
                    sp.SwitchOn = sel == enSpindleSel.Z1 ? outBit_Z1_SWITCH : outBit_Z2_SWITCH;
                    sp.SpindleRPM = sel == enSpindleSel.Z1 ? analogOut_Z1主軸轉速類比控制 : analogOut_Z2主軸轉速類比控制;
                    sp.SpindleLocker = outBit_主軸軸承鎖定;
                    break;

                case enSpindleCtrlType.通訊控制_RS232:
                    sp.UseIO = false;
                    sp.AirPressure = inBit_主軸空壓正常;
                    sp.CoolingFlow = inBit_冰水機正常運轉中;
                    sp.SpindleLocker = outBit_主軸軸承鎖定;
                    sp.Initial();
                    break;
            }
        }

        //程式初始化
        public override void Initial()
        {
            #region 無使用IO隱藏
            foreach (AnalogOut AO in AOList)
            {
                if (AO.Enabled == false)
                    AO.Visible = false;
            }
            foreach (AnalogIn AI in AIList)
            {
                if (AI.Enabled == false)
                    AI.Visible = false;
            }
            foreach (InBit IB in InBitList)
            {
                if (IB.Enabled == false)
                    IB.Visible = false;
            }
            foreach (OutBit OB in OutBitList)
            {
                if (OB.Enabled == false)
                    OB.Visible = false;
            }
            foreach (Motor MT in MotorList)
            {
                if (MT.Enabled == false)
                    MT.Visible = false;
            }
            #endregion 無使用IO隱藏

            #region 繼承函數_程式初始化_Scofield_v0.1.7.25[修改]主軸控制改用FC

            FC_Z1主軸控制_動作開始.TaskReset();
            FC_Z2主軸控制_動作開始.TaskReset();

            Z1SpindleControlType = SReadValue("dRadioGroupBox_Z1主軸控制方式選擇").ToInt();
            InitialSpindle(enSpindleSel.Z1, Z1SpindleControlType);

            Z2SpindleControlType = SReadValue("dRadioGroupBox_Z2主軸控制方式選擇").ToInt();
            InitialSpindle(enSpindleSel.Z2, Z2SpindleControlType);

            #endregion 繼承函數_程式初始化_Scofield_v0.1.7.25[修改]主軸控制改用FC

            //v4.0.1.17預設刀露量
            Tool1_InfoReturn.dRealKnifeRemain = 3000;
            Tool2_InfoReturn.dRealKnifeRemain = 3000;

            //+ By Max 20210424, Base Pulse Count Setting
            motor_切刀橫移馬達_Z1X.BasePulseCount = SetupReadValue("Z1XBasePulse").ToInt();
            motor_切刀橫移馬達_Z2X.BasePulseCount = SetupReadValue("Z2XBasePulse").ToInt();

            motor_切刀橫移馬達_Z1X.MotorParameter.HomeException = HOMEEXP.YASKAWA;
            motor_切刀橫移馬達_Z2X.MotorParameter.HomeException = HOMEEXP.YASKAWA;

            if (SReadValue("BackClearance").ToBoolean()) 
            {
				//motor_切刀橫移馬達_Z1X.PitchCOMEnable = true;
                //motor_切刀橫移馬達_Z1X.SetPitchCompensateLoadFile(@".\Localdata\ADSW10017_Z1_X.rtl", PULSEPERMM.PULSE1000);
                //motor_切刀橫移馬達_Z2X.PitchCOMEnable = true;
                //motor_切刀橫移馬達_Z2X.SetPitchCompensateLoadFile(@".\Localdata\ADSW10017_Z2_X.rtl", PULSEPERMM.PULSE1000);
				
                string[] files = Directory.GetFiles(@".\Localdata");
                string strFileNameZ1 = "";
                string strFileNameZ2 = "";

                foreach (string s in files)
                {
                    if (s.Contains("_Z1_X.rtl"))
                    {
                        strFileNameZ1 = s;
                    }
                    else if (s.Contains("_Z2_X.rtl"))
                    {
                        strFileNameZ2 = s;
                    }
                }

                if (strFileNameZ1.Length == 0 || strFileNameZ2.Length == 0)
                {
                    dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "", "將不使用補償");
                    SSetValue("BackClearance", false);
                }
                else
                {
                    motor_切刀橫移馬達_Z1X.PitchCOMEnable = true;
                    motor_切刀橫移馬達_Z1X.SetPitchCompensateLoadFile(strFileNameZ1, PULSEPERMM.PULSE1000);
                    motor_切刀橫移馬達_Z2X.PitchCOMEnable = true;
                    motor_切刀橫移馬達_Z2X.SetPitchCompensateLoadFile(strFileNameZ2, PULSEPERMM.PULSE1000);
                }
            }

            //+ By Max 20211030
            //DAQ Initial and Start Data Collection
            if (IsUseDAQ = SetupReadValue("UseDAQ").ToBoolean())
            {
                //toolDataCollect1.DeviceName = "DemoDevice,BID#0"; 
                toolDataCollect1.DeviceName = "PCIE-1810,BID#0"; //如果使用DAQ則將DeviceName設定為PCIE-1810,BID#0
                toolDataCollect1.Initial(IsSimulation());
                toolDataCollect1.Start();
                analogIn_Z1破片檢知.Visible = false;
                analogIn_Z2破片檢知.Visible = false;
                td_Z1破片檢知.Visible = true;
                td_Z2破片檢知.Visible = true;
                lblZ1.Visible = true;
                lblZ2.Visible = true;
            }
            else
            {
                analogIn_Z1破片檢知.Visible = true;
                analogIn_Z2破片檢知.Visible = true;
                td_Z1破片檢知.Visible = false;
                td_Z2破片檢知.Visible = false;
                lblZ1.Visible = false;
                lblZ2.Visible = false;
            }
        }

        //By Max 20170914
        //0: H3, 1:S2
        public void SetMachineType(int mcType)
        {
            MCType = mcType;
        }

        bool bIsZ1XSafe2Move = false;
        public bool IsZ1XSafe2Move()
        {
            return bIsZ1XSafe2Move;
        }

        bool bIsZ2XSafe2Move = false;
        public bool IsZ2XSafe2Move()
        {
            return bIsZ2XSafe2Move;
        }

        //持續偵測函數
        public override void AlwaysRun()
        {
            //TCPIPClose();
            //FC_連線開始.TaskReset();
            CheckInput();
            //v0.0.7.20 By Sanxiu 真空泵偵測
            if (IsPumpOn())
            {
                AirPumpChk();
                //v4.0.1.17 一旦超過30分鐘，自行關閉
                if (Pump_stop_timer.On(SReadValue("iAirPumpStopTime").ToInt() * 1000))
                {
                    if (CheckVaca(0) != "")
                    {
                        //+ By Max 20210416, v4.0.1.62, 因應分開成兩個OutBit
                        outBit_真空幫浦啟動.Off();
                        outBit_真空幫浦運轉.Off();
                    }
                    Pump_stop_timer.Restart();
                }
            }
            HandleCmsCom();

            if (VisionInitail)
            {
                FC_Vision.MainRun();
            }

            //主畫面按下暫停或Alarm也需要繼續做完(切割動作無法中斷)
            if (m_bIsPauseNeedToContinue)
            {
                //FC_SAW自動_動作開始.MainRun();
            }

            if (SpindleInitail)
            {
                FC_Z1主軸控制_動作開始.TaskReset();
                FC_Z2主軸控制_動作開始.TaskReset();
                FC_連線開始.TaskReset();//v4.0.1.22
                SpindleInitail = false;
            }
            else
            {
                FC_Z1主軸控制_動作開始.MainRun();
                FC_Z2主軸控制_動作開始.MainRun();
                FC_連線開始.MainRun();//v4.0.1.22
            }
            //v4.0.1.3 By Woody 換刀區安全門開啟時，主軸關閉
            if ((!GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) || !GetInBit(inBit_換刀區安全門鎖定檢知_Z2區)) && !bCutDoorSafe)
            {
                string msg = "";
                if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                {
                    Z1SpindleStop = true;
                    msg = "Z1主軸停止";
                }
                if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    Z2SpindleStop = true;
                    msg += "Z2主軸停止";
                }
                if (msg != "")
                {
                    ShowAlarm("E", 70, "換刀區安全門未關閉" + msg);
                }
            }

            //主軸停止狀態，自動開啟換刀安全門v4.0.1.18
            if (IsSimulation() == 0)
            {
                if (!SReadValue("bIgnoreCutSafeDoor").ToBoolean() || this.RunMode != RunModeDT.MANUAL)
                {
                    //Scofield_2018.06.12_利用主軸轉速控制後安全門的RealAvgValue改成RealValue
                    if (GetInBit(inBit_Z1主軸未運轉) && GetInBit(inBit_Z1主軸達設定值) && analogIn_Z1主軸轉速.RealValue < 1500 &&
                        GetInBit(inBit_Z2主軸未運轉) && GetInBit(inBit_Z2主軸達設定值) && analogIn_Z2主軸轉速.RealValue < 1500)
                    {
                        if (GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) || GetInBit(inBit_換刀區安全門鎖定檢知_Z2區)) //後安全門為鎖住的狀態
                        {
                            SetOutBit(outBit_換刀區安全門閉鎖, false);
                        }
                    }
                }
            }

            //2018424-主軸啟動狀態，自動關閉換刀安全門v4.0.1.18
            if (IsSimulation() == 0)
            {
                if (!SReadValue("bIgnoreCutSafeDoor").ToBoolean() || this.RunMode != RunModeDT.MANUAL)
                {
                    //Scofield_2018.06.12_利用主軸轉速控制後安全門的RealAvgValue改成RealValue
                    if (analogIn_Z1主軸轉速.RealValue > 1500 || analogIn_Z2主軸轉速.RealValue > 1500)
                    {
                        if ((!GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) || !GetInBit(inBit_換刀區安全門鎖定檢知_Z2區)) && bCutDoorSafe) //後安全門為未上鎖的狀態
                        {
                            SetOutBit(outBit_換刀區安全門閉鎖, true);
                        }
                    }
                }
            }

            if (mHomeOk && IsSimulation() == 0 && 切割橫移X軸不安全())
            {
                motor_切刀橫移馬達_Z1X.FastStop();
                motor_切刀橫移馬達_Z2X.FastStop();
                ShowAlarm("E", 80);
            }

            //v0.0.7.10 By Sanxiu  L/UL安全偵測，當執行L/UL動作，Y軸不能忙錄
            if (mHomeOk)
            {
                //回Home完成才能進來
                if (IsSimulation() == 0 && SReadValue("ConnectHandler").ToInt() > 0)
                {
                    //有跟Handler連線才會檢查
                    if (GetSawTablbeChkSignal())
                    {
                        //歸零即可進行馬達掃描，掃描時機由Handler提供的Input訊號決定
                        if (IsSawTableSafePos(iChkSawTableType) == true)
                        {
                            //true代表不安全，false代表安全
                            //報警異常，上下料過程為完成，切割平台Y軸想移動
                            //LogSay(EnLoggerType.EnLog_SPC, "上下料過程完成前，切割平台不可移動");
                            ShowAlarm("E", 81);
                        }
                    }
                }
            }

            if (motor_切刀上下馬達_Z1.ReadEncPos() < SReadValue("Z2_ZCCDminValue").ToInt())
            {
                if (motor_切刀橫移馬達_Z1X.Busy())
                {
                    motor_切刀橫移馬達_Z1X.FastStop();
                    ShowAlarm("E", 20, SReadValue("Z2_ZCCDminValue").ToInt());
                }
            }

            if (motor_切刀上下馬達_Z2.ReadEncPos() < SReadValue("Z2_ZCCDminValue").ToInt())
            {
                if (motor_切刀橫移馬達_Z2X.Busy())
                {
                    motor_切刀橫移馬達_Z2X.FastStop();
                    ShowAlarm("E", 21, SReadValue("Z2_ZCCDminValue").ToInt());
                }
            }

        } //持續掃描

        public override int ScanIO() { return 0; } //掃描硬體按鈕IO

        //歸零相關函數
        public override void HomeReset()
        {
            ResetAllFlag();
            Saw_Home_Reset();
            m_bIsNeedCheckVaca = false;
            m_enTeachSel = enTeachActiionSel.無動作;
            Teach_ActionResult = 0;
            Auto_ActionNo = 0;
            Auto_ActionResult = 0;
            Z1CheckBeforeStart.Reset();
            Z2CheckBeforeStart.Reset();
            m_IsCutLineTeachDone = false;
            m_IsTargetTeachDone = false;
            m_nHomeFlow = 0;
            m_bIsPauseNeedToContinue = false;
            //+ By Max 20210312, v4.0.1.53
            SetContinueRun(false);

            //v4.0.1.0 By Woody 歸零前將CCD通訊初始
            CCDCmd = string.Empty;
            mSendCCDAction = CCDSendResult.Wait;

            bReMoveToRunPos = false;   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置

            FC_SAW歸零_動作開始.TaskReset();
            FC_Vision.TaskReset();
            FC_教導執行_動作開始.TaskReset();
            FC_SAW自動_動作開始.TaskReset();
            VisionInitail = true;
            CallAutoFocus = "";

            //v0.0.7.20 By Sanxiu 真空泵偵測
            bAirPumpStd = false;

            mHomeOk = false;
            mLotend = false;
            mLotendOk = false;
            mCanHome = false;

        } //歸零前的重置
        public override bool Home()
        {
            if (m_enZ2SpindleStatus != enSpindleStatus.Closing)
            {
                FC_SAW歸零_動作開始.MainRun();
            }
            return mHomeOk;
        } //歸零

        //運轉相關函數
        public override void ServoOn()
        {
            motor_切刀上下馬達_Z1.ServoOn(true);
            motor_切刀上下馬達_Z2.ServoOn(true);
            motor_切刀橫移馬達_Z1X.ServoOn(true);
            motor_切刀橫移馬達_Z2X.ServoOn(true);
            motor_切割平台前後馬達Y.ServoOn(true);
            motor_切割平台旋轉馬達U.ServoOn(true);
        } //Motor Servo On
        public override void ServoOff()
        {
            motor_切刀上下馬達_Z1.ServoOn(false);
            motor_切刀上下馬達_Z2.ServoOn(false);
            motor_切刀橫移馬達_Z1X.ServoOn(false);
            motor_切刀橫移馬達_Z2X.ServoOn(false);
            motor_切割平台前後馬達Y.ServoOn(false);
            motor_切割平台旋轉馬達U.ServoOn(false);
        } //Motor Servo Off
        public override void RunReset()
        {
            //v4.0.1.3 By Woody 自動流程-測高
            m_bIsAutoNeedSparkTestZ1 = false;
            m_bIsAutoNeedSparkTestZ2 = false;
            m_bIsAutoNeedNContactTestZ1 = false;
            m_bIsAutoNeedNContactTestZ2 = false;
            iSparkTestCountZ1 = 0;
            iSparkTestCountZ2 = 0;
            iNContactTestCountZ1 = 0;
            iNContactTestCountZ2 = 0;
            m_bIsTeachNeedNContactTestZ1 = false;
            m_bIsTeachNeedNContactTestZ2 = false;

            BladeRefresh();//v4.0.1.17

            bReMoveToRunPos = false;   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置

            ResetAllFlag();
            FC_Vision.TaskReset();
            FC_教導執行_動作開始.TaskReset();
            FC_SAW自動_動作開始.TaskReset();

            //v0.0.7.20 By Sanxiu 真空泵偵測
            bAirPumpStd = false;

            PCBIncount = 0;
            PCBOutcount = 0;

            //v4.0.1.23 畫筆與畫布重設
            //DelegateDrawReset(true);
            //+ By Max 20210206
            if (DelegateDrawLineData != null)
                DelegateDrawLineData(s切割_NowSide, i切割_Z1_NowLineNo, i切割_Z2_NowLineNo, i切割_Now百分比, true);

            //v4.0.1.28 入料/出料角度可自行設定
            iMoveHadinU = SReadValue("基準入料點位_θ").ToInt()
                + (int)((double)iDD馬達一轉Pulse數 * PReadValue("PCBIn_Offsetθ").ToDouble() / 360)
                + (iDD馬達一轉Pulse數 * PReadValue("iRotateInMode").ToInt() / 4) * (PReadValue("bAntiClockwise").ToBoolean() ? -1 : 1);

            iMoveInitialU = SReadValue("基準入料點位_θ").ToInt() + (int)((double)iDD馬達一轉Pulse數 * PReadValue("PCBIn_Offsetθ").ToDouble() / 360);

            iMoveHadoutU = SReadValue("基準出料點位_θ").ToInt() +
                (iDD馬達一轉Pulse數 * PReadValue("iRotateOutMode").ToInt() / 4) * (PReadValue("bAntiClockwise").ToBoolean() ? -1 : 1);

            //+ By Max 20210315, v4.0.1.54, 強制設定UseContinueMode
            SSetValue("UseContinueMode", true);
        } //運轉前初始化
        public override void Run()
        {
            if (GetAutoReMoveToRunPos())   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
            {
                if (GetReMoveToRunPos())
                {
                    bUseAutoReMove = true;
                }
                else
                {
                    bUseAutoReMove = false;
                }
            }
            else
            {
                bUseAutoReMove = false;
            }

            if (bUseAutoReMove)   //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
            {
                flowReMovePos_Start.MainRun();
            }
            else
            {
                if (m_enZ1SpindleStatus != enSpindleStatus.Closing || m_enZ2SpindleStatus != enSpindleStatus.Closing)
                {
                    FC_SAW自動_動作開始.MainRun();
                }
            }
        } //運轉
        public override void SetSpeed(bool bHome = false) { } //速度設定

        //手動相關函數
        public override void ManualReset() { } //手動運行前置設定
        public override void ManualRun() {} //手動模式運行

        //停止所有馬達
        public override void StopMotor()
        {
            //v4.0.1.20 暫停或Error會呼叫馬達暫停，同時停止教導Timer
            //但基準線校正不能停止
            if (m_enTeachSel != enTeachActiionSel.無動作 && !m_bIsTeachPauseNeedToContinue)
            {
                SetTeachTimer(false);
            }

            if (m_enZ1SpindleStatus == enSpindleStatus.Closing || m_enZ2SpindleStatus == enSpindleStatus.Closing)
            {//主軸關閉中，Z軸需上升
                motor_切刀橫移馬達_Z1X.Stop();
                motor_切刀橫移馬達_Z2X.Stop();
                motor_切割平台旋轉馬達U.Stop();
                motor_切割平台前後馬達Y.Stop();
            }
            else if (m_bIsPauseNeedToContinue || m_bIsTeachPauseNeedToContinue)
            {//Y軸需走完，或可能異常提刀
                motor_切刀橫移馬達_Z1X.Stop();
                motor_切刀橫移馬達_Z2X.Stop();
                motor_切割平台旋轉馬達U.Stop();
            }
            else
            {
                base.StopMotor();
            }
        }

        #endregion

        #region 定義變量

        const int iSpindleAir空壓偵測數值 = 3;
        const int iSpindle相關訊號TimeOut時間 = 30000;      //30秒
        const int iDD馬達一轉Pulse數 = 6815744;  //1 pulse 相等於 5.259"

        #endregion 定義變量

        #region 私有變數
        private int PCBIncount = 0;//PCB入料數量
        private int PCBOutcount = 0;//PCB出料數量
        private bool ManualAlignment = false;//手動定靶
        private bool bUseManualAlignment = false;
        private bool TargetCheckFinish = false;//下刀點確認完成
        private bool TargetCheckStart = false;//下刀點確認開始
        private string TC_Side = "";//下刀點確認選擇Channel
        private int TC_LineNo = 0;//下刀點確認選擇第幾道
        private string TC_Loc = "";//下刀點確認選擇位置(AWAY、NEAR)
        private int TC_X, TC_Y, TC_U;
        //切割過程紀錄主軸、破刀、真空異常訊息
        private string VaccError = "";

        //+ By Max 20210225, 切割水流量異常
        private String WaterFlowAbnormal = "";

        //v0.0.7.9 By Sanxiu 動態刀痕補償
        private bool bdynamicCutOffset = false;
        private bool bdynamicCutOffsetFinish = false;
        private bool bdynamicCutOffset_run = false;//是否正在進行動態刀痕辨識
        private bool bWaterTurnOn = false;
        //+ By Max 20210309, 是否顯示動態刀痕CheckBox
        private bool bCanShowDynamicCutOffset = false;

        //v4.0.1.15 By Woody
        private AutoResetEvent m_KerfCheckFinishEvt = new AutoResetEvent(false);
        //private bool kerfcheckFinish = false;
        private MarkPos KerfCheck = new MarkPos();
        private DataTable KerfCheckView = new DataTable();
        private int AlignRetryCount = 0;
        private int FindMarkRetryCount = 0;
        private int KerfCheckRetryCount = 0;

        //v4.0.1.2 By Woody 自動對焦
        private string CallAutoFocus = "";//呼叫AutoFocus的流程
        private int AutoFocusZ = 0;
        private double m_dFocusMaxScore = 0;
        private bool AutoFocusFinish = false;
        //private bool AlignmentFinish = false;
        private AutoResetEvent m_PositioningFinishEvt = new AutoResetEvent(false);

        //v4.0.1.17
        private MyTimer Pump_stop_timer = new MyTimer(true);

        //v4.0.1.25 掃靶方式，修改完下一片才會更新
        private enScanTargetMethod m_enScanMethod = 0;

        //視覺用
        private CMS mCMS = new CMS();
        private string RealCCDCmd;
        private string CCDCmd = string.Empty; //CCD指令
        private string[] CCDResult = new string[100]; //CCD回傳值
        private CCDSendResult mCCDAction; //CCD判別狀態 
        private CCDSendResult mSendCCDAction; //CCD傳送狀態
        private MyTimer Timer_VisionCom = new MyTimer(true);
        private enum CCDSendResult
        {
            Wait = 0,
            Working,
            OK,
            NG,
            InspectFail,
        }

        //視覺用

        //Scofield_v0.1.7.25[修改]主軸控制改用FC
        private bool SpindleInitail = true;

        private bool VisionInitail = false;

        enSpindleStatus m_enZ1SpindleStatus = enSpindleStatus.Stop;
        enSpindleStatus m_enZ2SpindleStatus = enSpindleStatus.Stop;

        private int Z1SpindleControlType;
        private int Z2SpindleControlType;

        //v0.0.7.9 By Sanxiu 只用單刀功能
        private bool bUseZ1_Spindle = false;
        private bool bUseZ2_Spindle = false;

        //v4.0.0.37破刀後處理(換刀續做或單刀續做)
        private bool bshow_ChangeBladeContinue = false;
        private bool bshow_SingleBladeContinue = false;
        private bool bset_ChangeBladeContinue = false;
        private bool bset_SingleBladeContinue = false;

        private MyTimer T_Auto_Z1SPControl = new MyTimer(true);
        private MyTimer T_Auto_Z2SPControl = new MyTimer(true);

        private bool Z1SpindleStart
        {
            //get { return Z1SpindleStart && bUseZ1_Spindle; }
            get;
            set;
        }

        private bool Z2SpindleStart
        {
            //get { return Z2SpindleStart && bUseZ2_Spindle; }
            get;
            set;
        }

        private bool Z1SpindleStop
        {
            get;
            set;
        }

        private bool Z2SpindleStop
        {
            get;
            set;
        }

        private MyTimer Timer_Home = new MyTimer(true);
        private int m_nHomeFlow = 0;//偵測歸零狀態

        private enHomeSpindleStatus m_enHomeSpindleStatus = 0;
        //1:開啟雙軸/Z1運轉中/Z2運轉中 
        //2.開啟雙軸/Z1運轉中/Z2停止 
        //3.開啟雙軸/Z1停止/Z2運轉中
        //4.開啟雙軸/Z1停止/Z2停止
        //5.開啟單軸Z1/Z1運轉中
        //6.開啟單軸Z1/Z1停止
        //7.開啟單軸Z2/Z2運轉中
        //8.開啟單軸Z2/Z2停止

        private AutoResetEvent m_CleanFinishEvt = new AutoResetEvent(false);
        private enCleanType m_enCleanType;
        private int iNow清洗步驟 = 0;
        private int iNow清洗次數 = 0;
        private int i總清洗次數 = 0;
        private int i清洗θ位置 = 0;
        private int i清洗DelayTime = 0;

        private int iCleanStdPos = 0;
        private int iCleanEndPos = 0;

        private string tableName = "CutClean";

        private DataTable 入料前清洗DataTable = null;
        private DataTable 掃靶前清洗DataTable = null;
        private DataTable 切割後清洗DataTable = null;
        private DataTable 目前清洗DataTable = null;

        MyTimer o清洗Timer = new MyTimer(true);

        bool m_bIsVisionSaw = false;
        private int AutoFlowStep = 0;//自動用，提供數據供外部顯示
        //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
        private int AutoFlowStepForCutOffset = 0;//自動用，提供數據供外部顯示
        private int Auto_ActionNo = 0;//自動用
        private int Auto_ActionResult = 0;//自動用
        private AutoResetEvent m_AutoFlowChartEndEvt = new AutoResetEvent(false);
        private AutoResetEvent m_ForceOutContinueEvt = new AutoResetEvent(false);
        private bool AutoUserCheckOk = false;//自動用，供外部與流程交握用(下一步)
        private bool AutoUserCheckSkip = false;//自動用，供外部與流程交握用(跳過)
        private MyTimer Timer_Auto = new MyTimer(true);//自動用
        private bool m_bIsPauseNeedToContinue = false;
        private bool m_bIsTeachPauseNeedToContinue = false;
        private bool m_bIsNeedCheckVaca = false;//宣告
        private bool ActionStopFinish = false;
        MyTimer Timer_ActionIni = new MyTimer(true);

        private bool m_IsCutLineTeachDone = false;
        private bool m_IsTargetTeachDone = false;

        CheckBeforeStart Z1CheckBeforeStart = new CheckBeforeStart();
        CheckBeforeStart Z2CheckBeforeStart = new CheckBeforeStart();

        enSpindleSel m_SparkTestSpindle = enSpindleSel.NONE;
        enSpindleSel m_BaseLineSpindle = enSpindleSel.NONE;

        enSecondSawState NowSecondSawState = enSecondSawState.NoUse;

        int i切割_Now百分比;
        string s切割_NowSide;
        int i切割_Z1_NowLineNo;
        int i切割_Z2_NowLineNo;

        private string TeachAlarmLevel = "";//教導用，提供數據供外部顯示顯示目前警示層級
        private int TeachAlarmCode = 0;//教導用，提供數據供外部顯示顯示目前警示碼
        private string[] TeachMsg;

        //+ By Max 20210305
        private string AutoAlarmLevel = ""; //自動用，提供數據供外部顯示顯示目前警示層級
        private int AutoAlarmCode = 0; //自動用，提供數據供外部顯示顯示目前警示碼
        private string[] AutoMessage;//自動用，提供數據供外部顯示


        //private string TeachMessage = "";//教導用，提供數據供外部顯示
        private enTeachActiionSel m_enTeachSel = enTeachActiionSel.無動作;//教導用
        private int Teach_ActionResult = 0;//教導用
        private bool Teach_ActionResultNCTZ1 = false;
        private bool Teach_ActionResultNCTZ2 = false;
        private bool bSetTeachFlowChartEnd = false;//教導用
        private int TeachStatus = 0;//教導狀態回傳
        private int TeachStatusNCTZ1 = 0;//Z1非接觸狀態回傳
        private int TeachStatusNCTZ2 = 0;//Z2非接觸狀態回傳
        private string TeachMessageNCTZ1 = "";//Z1非接觸狀態回傳
        private string TeachMessageNCTZ2 = "";//Z2非接觸狀態回傳
        private int TeachFlowStep;//教導用，供外部連動流程顯示UI用(0:顯示狀態與下一步 1:視覺相關，可使用馬達 2:主軸相關 98:使用強排 99:流程完成)
        private bool TeachUserCheckOk;//教導用，供外部與流程交握用(下一步)
        //+ By Max 20210220, 上一步使用
        private bool TeachUserCheckPrevious;//教導用，供外部與流程交握用(上一步)
        private bool TeachUserCheckSkip;//教導用，供外部與流程交握用(跳過)
        private MyTimer Timer_Teach = new MyTimer(true);//教導用
        private MyTimer Timer_Z1NCT = new MyTimer(true);//Z1非接觸測高用
        private MyTimer Timer_Z2NCT = new MyTimer(true);//Z2非接觸測高用

        private FixturePos TeachFixture = new FixturePos();//膠墊學習用
        private MarkPos LearnMarkPos = new MarkPos();//靶點學習用
        private MarkPos AutoMark = new MarkPos();//自動掃靶用
        private CutPos AutoCut = new CutPos();//自動切割用
        private List<Point> TopPoint = new List<Point>();//全靶用(上方)
        private List<Point> BottomPoint = new List<Point>();//全靶用(下方)
        private int MovePitch = 0;//靶點學習用
        //private bool m_bIsSearchFinish = false;
        private AutoResetEvent m_SearchTargetDoneEvt = new AutoResetEvent(false);
        private bool m_bIsTargetScanning = false;
        private bool TriPointSearchcheckL = false;//三點定位是否找到左邊角度
        private bool TriPointSearchcheckR = false;//三點定位是否找到右邊角度

        #region 馬達歸零用

        private string m_strLang = "tw";

        private string CreateDialogMsgContent(string strID)
        {
            if (m_strLang != "tw")
            {
                strID += "_En";
            }

            ResourceManager resourceManager = new ResourceManager(typeof(Properties.Resources));
            return resourceManager.GetString(strID);
        }

        Dictionary<Motor, bool> dicMotorState = new Dictionary<Motor, bool>();

        private async void btnMotorHome_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Motor motor = SearchMotorInControls(btn.Parent.Controls);

            if (dicMotorState.ContainsKey(motor) && dicMotorState[motor])
            {
                return;
            }

            if (motor != null)
            {
                string strTmp = CreateDialogMsgContent("Text_Saw_Question_Home");

                if (dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Question", string.Format(strTmp, motor.Text), m_strLang)
                    == DialogResult.OK)
                {
                    motor.HomeReset();
                    btn.BackColor = Color.Yellow;
                    dicMotorState[motor] = true;
                }

                //if (MessageBox.Show(string.Format("請問你確定要歸零{0}嗎?", motor.Text), "Question",
                //    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                //{
                //    motor.HomeReset();
                //    btn.BackColor = Color.Yellow;
                //    dicMotorState[motor] = true;
                //}
            }

            if (btn.BackColor == Color.Yellow)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                int nTimeOut = SReadValue("馬達逾時時間").ToInt();

                Task task = Task.Run(() =>
                {
                    if (motor.Name != "motor_切割平台旋轉馬達U" ||
                        (motor.Name == "motor_切割平台旋轉馬達U" && motor.MotorType == MOTORTYPE.YASKAWA_ABS))
                    {
                        while (dicMotorState[motor])
                        {
                            if (motor.Home()) break;

                            if (sw.ElapsedMilliseconds > nTimeOut)
                            {
                                motor.Stop();
                                break;
                            }

                            SpinWait.SpinUntil(() => false, 10);
                        }

                        sw.Stop();
                    }
                    else
                    {
                        int nWaitTime = 500;

                        do
                        {
                            bool bIsPass = true;

                            outBit_U軸歸零功能開啟.On();
                            SpinWait.SpinUntil(() => false, nWaitTime);

                            outBit_U軸歸零啟動.On();
                            SpinWait.SpinUntil(() => false, nWaitTime);

                            while (dicMotorState[motor])
                            {
                                bIsPass = inBit_切割平台U軸原點歸零訊號.On(500);

                                if (bIsPass)
                                {
                                    outBit_U軸歸零功能開啟.Off();
                                    outBit_U軸歸零啟動.Off();
                                    motor.SetPos(0);
                                    motor.SetEncoderPos(0);
                                    break;
                                }

                                if (sw.ElapsedMilliseconds > nTimeOut)
                                {
                                    motor.Stop();
                                    break;
                                }

                                SpinWait.SpinUntil(() => false, 10);
                            }

                            if (bIsPass) break;

                        } while(false);
                    }
                });

                await task;

                btn.BackColor = Color.Transparent;
                dicMotorState[motor] = false;
            }
        }

        private void btnMotorStop_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Motor motor = SearchMotorInControls(btn.Parent.Controls);
            if (motor != null)
            {
                dicMotorState[motor] = false;
                motor.Stop();
            }
        }

        private void btnMotorServoOn_Click(object sender, EventArgs e)
        {   
            Button btn = sender as Button;
            Motor motor = SearchMotorInControls(btn.Parent.Controls);
            if (motor != null)
            {
                motor.SetPos(motor.ReadEncPos());
                motor.ServoOn();
            }
        }

        private void btnMotorServoOff_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Motor motor = SearchMotorInControls(btn.Parent.Controls);
            if (motor != null)
            {
                motor.ServoOn(false);
            }
        }

        private void btnMotorJog_Click(object sender, EventArgs e)
        {   //操作紀錄(sender, "motor");
            KCSDK.MotorJogForm.MotorJog.Run((GroupBox)((Button)sender).Parent);
        }

        private Motor SearchMotorInControls(Control.ControlCollection ctrls)
        {
            Motor motor = null;
            foreach (var ctrl in ctrls)
            {
                if (ctrl is Motor) motor = (Motor)ctrl;
            }
            return motor;
        }

        private void dCheckBox25_KeyDown(object sender, KeyEventArgs e)
        {
            CheckBox chk1 = sender as CheckBox;

            if (e.KeyCode == Keys.A && e.Control == true)
            {
                foreach (CheckBox chk in chk1.Parent.Controls)
                    if (chk is CheckBox) chk.Checked = true;
            }

            if (e.KeyCode == Keys.U && e.Control == true)
            {
                foreach (CheckBox chk in chk1.Parent.Controls)
                    if (chk is CheckBox) chk.Checked = false;
            }
        }

        #endregion 馬達歸零用

        #region 基準線校正
        private Point BaseLineT = new Point(0, 0);
        private Point BaseLineB = new Point(0, 0);
        private int BaseLine_theta = 0;

        public void BaseLineSpeedSet()
        {
            double dCutSpeedRate;
            dCutSpeedRate = OReadValue("機台速率").ToInt() / 100d;
            motor_切刀橫移馬達_Z1X.SetSpeed(Convert.ToInt32(SReadValue("Z1切割速度_X").ToDouble() * dCutSpeedRate));
            motor_切刀橫移馬達_Z1X.SetAcceleration(Convert.ToInt32(SReadValue("Z1切割加速度_X").ToDouble() * dCutSpeedRate));
            motor_切刀橫移馬達_Z1X.SetDeceleration(Convert.ToInt32(SReadValue("Z1切割減速度_X").ToDouble() * dCutSpeedRate));
            motor_切刀橫移馬達_Z2X.SetSpeed(Convert.ToInt32(SReadValue("Z2切割速度_X").ToDouble() * dCutSpeedRate));
            motor_切刀橫移馬達_Z2X.SetAcceleration(Convert.ToInt32(SReadValue("Z2切割加速度_X").ToDouble() * dCutSpeedRate));
            motor_切刀橫移馬達_Z2X.SetDeceleration(Convert.ToInt32(SReadValue("Z2切割減速度_X").ToDouble() * dCutSpeedRate));

            motor_切刀上下馬達_Z1.SetSpeed(Convert.ToInt32(SReadValue("Z1切割速度_Z").ToDouble() * dCutSpeedRate));
            motor_切刀上下馬達_Z1.SetAcceleration(Convert.ToInt32(SReadValue("Z1切割加速度_Z").ToDouble() * dCutSpeedRate));
            motor_切刀上下馬達_Z1.SetDeceleration(Convert.ToInt32(SReadValue("Z1切割減速度_Z").ToDouble() * dCutSpeedRate));
            motor_切刀上下馬達_Z2.SetSpeed(Convert.ToInt32(SReadValue("Z2切割速度_Z").ToDouble() * dCutSpeedRate));
            motor_切刀上下馬達_Z2.SetAcceleration(Convert.ToInt32(SReadValue("Z2切割加速度_Z").ToDouble() * dCutSpeedRate));
            motor_切刀上下馬達_Z2.SetDeceleration(Convert.ToInt32(SReadValue("Z2切割減速度_Z").ToDouble() * dCutSpeedRate));

            motor_切割平台前後馬達Y.SetSpeed(100000);
            motor_切割平台前後馬達Y.SetAcceleration(500000);
            motor_切割平台前後馬達Y.SetDeceleration(500000);

            motor_切割平台旋轉馬達U.SetSpeed(Convert.ToInt32(SReadValue("切割速度_θ").ToDouble() * dCutSpeedRate));
            motor_切割平台旋轉馬達U.SetAcceleration(Convert.ToInt32(SReadValue("切割加速度_θ").ToDouble() * dCutSpeedRate));
            motor_切割平台旋轉馬達U.SetDeceleration(Convert.ToInt32(SReadValue("切割減速度_θ").ToDouble() * dCutSpeedRate));

        }


        #endregion 基準線校正

        #region 真空泵偵測
        //v0.0.7.20 By Sanxiu 真空泵偵測
        MyTimer airPumpDelayTimer = new MyTimer(true);
        private int iAirPumpChkStep = 0;
        private bool bAirPumpStd = false;
        private int PumpOpenTime = 0;

        public void AirPumpChkReset()
        {
            iAirPumpChkStep = 0;
        }

        private void AirPumpChk()
        {
            switch (iAirPumpChkStep)
            {
                case 0:
                    airPumpDelayTimer.Restart();
                    iAirPumpChkStep = 10;
                    break;
                case 10:
                    if (airPumpDelayTimer.On(SReadValue("iAirPumpSTartTimes").ToInt()))
                    {
                        iAirPumpChkStep = 20;
                    }
                    break;
                case 20:
                    //v4.0.1.32 真空PUMP可PASS訊號
                    if (IsSimulation() == 0 && !SReadValue("bIgnoreAirPump").ToBoolean())
                    {
                        if (inBit_真空幫浦運轉中.Off(100))
                        {
                            ShowAlarm("E", 33);
                        }
                        if (inBit_真空幫浦水量正常.Off(100))
                        {
                            ShowAlarm("E", 34);
                        }
                        if (inBit_真空幫浦溫度正常.Off(100))
                        {
                            ShowAlarm("E", 35);
                        }
                        if (inBit_真空幫浦無過載.Off(100))
                        {
                            ShowAlarm("E", 36);
                        }
                    }
                    break;
            }
        }

        private bool IsPumpOn()
        {
            //+ By Max 20210416, v4.0.1.62, 因應分開成兩個OutBit，啟動改成運轉
            if (bAirPumpStd != outBit_真空幫浦運轉.Value)
            {
                bAirPumpStd = outBit_真空幫浦運轉.Value;
                AirPumpChkReset();
            }
            return bAirPumpStd;
        }
        #endregion 真空泵偵測

        //v4.0.1.3 By Woody 自動流程-測高
        private bool m_bIsAutoNeedSparkTestZ1 = false;
        private bool m_bIsAutoNeedSparkTestZ2 = false;
        //private bool m_bIsAutoNeedSparkTestFinsih = false;
        private AutoResetEvent m_AutoSparkTestFinishEvt = new AutoResetEvent(false);
        private bool m_bIsAutoNeedNContactTestZ1 = false;
        private bool m_bIsAutoNeedNContactTestZ2 = false;
        //private bool AutoNeedNContactTestFinish = false;
        private AutoResetEvent m_AutoNContactTestFinishEvt = new AutoResetEvent(false);
        private int iSparkTestCountZ1 = 0;
        private int iSparkTestCountZ2 = 0;
        private int iNContactTestCountZ1 = 0;
        private int iNContactTestCountZ2 = 0;
        private bool m_bIsTeachNeedNContactTestZ1 = false;
        private bool m_bIsTeachNeedNContactTestZ2 = false;

        #region 主畫面顯示掃描/切割時間

        private Stopwatch m_SwTimeLog = new Stopwatch(); // 時間紀錄

        struct TimeLog
        {
            public double dLoadTime;
            public double dScanTime;
            public double dCutTime;
            public double dCleanTime;
            public double dUnloadTime;
        }

        TimeLog m_TimeLog;

        private Action<double, double, double, double, double> ShowAllTimeAction; //入料時間, 掃描時間, 切割時間, 清洗時間, 出料時間
        public void RecordDelegateShowTime(Action<double, double, double, double, double> ShowAllTime)
        {
            ShowAllTimeAction = ShowAllTime;
        }

        #endregion 主畫面顯示掃描/切割時間

        //+ By Max, 20210804
        private SpindleStatus Z1SpindleStatus = new SpindleStatus();
        private SpindleStatus Z2SpindleStatus = new SpindleStatus();

        //+ By Max 20211030
        private bool IsUseDAQ = false;

        #endregion 私有變數

        #region 公用變數
        #endregion 公用變數

        #region 私有函數
        //偵測水壓、主軸空壓、排水槽、漏液檢知、鼓風機、沉澱槽
        private void CheckInput()
        {
            if (IsSimulation() != 0)
                return;

            if (SReadValue("水空壓偵測").ToBoolean())
            {
                double dWaterPressureMin = SReadValue("Water_Pressure_Min").ToDouble();
                double dWaterPressureMax = SReadValue("Water_Pressure_Max").ToDouble();
                double dAirPressureMin = SReadValue("Spindle_AirPressure_Min").ToDouble();
                double dAirPressureMax = SReadValue("Spindle_AirPressure_Max").ToDouble();

                double dWaterPressureNow = GetWaterInPressAIValue();
                double dAirPressureNow = GetSpindleAirPressureAIValue();

                if (dWaterPressureNow < dWaterPressureMin || dWaterPressureNow > dWaterPressureMax )
                {
                    ShowAlarm("w", 48, analogIn_純水水壓值.Text);
                }

                if (dAirPressureNow < dAirPressureMin || dAirPressureNow > dAirPressureMax)
                {
                    ShowAlarm("w", 48, analogIn_主軸空壓值.Text);
                }

                //Woody 20201005 檢測
                if (inBit_排水槽液位正常.Off()) ShowAlarm("w", 48, inBit_排水槽液位正常.Text);

                if (SReadValue("WaterLeakageDetect").ToBoolean())
                {
                    if (inBit_漏液檢知.On()) ShowAlarm("w", 48, inBit_漏液檢知.Text);
                }

                if (inBit_鼓風機正常.Off() && RunMode == RunModeDT.AUTO) ShowAlarm("w", 48, inBit_鼓風機正常.Text);

                //+ By Max 20210316, v4.0.1.55
                if (inBit_沉澱槽液位正常.Off()) ShowAlarm("w", 48, inBit_沉澱槽液位正常.Text);
            }

            if (outBit_水槽清潔噴水啟動.Value && RunMode != RunModeDT.AUTO) outBit_水槽清潔噴水啟動.Off();
        }

        private void HandleCmsCom()
        {
            if (mCMS.CMSLogSayMessage.Count != 0)
            {
                LogSay(EnLoggerType.EnLog_COMM, mCMS.CMSLogSayMessage[0]);
                mCMS.CMSLogSayMessage.RemoveAt(0);
            }
        }

        //v0.0.7.9 By Sanxiu 只用單刀功能
        private void SetUseSpindle()
        {
            enUseSpidleType type = (enUseSpidleType)(OReadValue("iUseSpindleType").ToInt());

            switch (type)
            {
                case enUseSpidleType.雙刀:
                    bUseZ1_Spindle = true;
                    bUseZ2_Spindle = true;
                    SSetValue("ByPassZ1接觸測高", false);
                    SSetValue("ByPassZ1非接觸測高", false);
                    SSetValue("ByPassZ1基準線校正", false);
                    SSetValue("ByPassZ2接觸測高", false);
                    SSetValue("ByPassZ2非接觸測高", false);
                    SSetValue("ByPassZ2基準線校正", false);
                    break;

                case enUseSpidleType.Z1單刀:
                    bUseZ1_Spindle = true;
                    bUseZ2_Spindle = false;
                    SSetValue("ByPassZ1接觸測高", false);
                    SSetValue("ByPassZ1非接觸測高", false);
                    SSetValue("ByPassZ1基準線校正", false);
                    SSetValue("ByPassZ2接觸測高", true);
                    SSetValue("ByPassZ2非接觸測高", true);
                    SSetValue("ByPassZ2基準線校正", true);
                    SaveFile();
                    break;

                case enUseSpidleType.Z2單刀:
                    bUseZ1_Spindle = false;
                    bUseZ2_Spindle = true;
                    SSetValue("ByPassZ1接觸測高", true);
                    SSetValue("ByPassZ1非接觸測高", true);
                    SSetValue("ByPassZ1基準線校正", true);
                    SSetValue("ByPassZ2接觸測高", false);
                    SSetValue("ByPassZ2非接觸測高", false);
                    SSetValue("ByPassZ2基準線校正", false);
                    SaveFile();
                    break;
            }

            SSetValue("ByPass切割道學習", false);
            SSetValue("ByPass靶點學習", false);
        }

        #region 私有函數_Scofield_v4.0.1.9[優化]IO Function

        private void SetOutBit(OutBit OB, bool b)
        {
            string w = OB.Name;
            switch (w)
            {
                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1非接觸式測高護蓋開啟

                case "outBit_Z1非接觸式測高護蓋開啟":
                    switch (b)
                    {
                        case true:
                            outBit_Z1非接觸式測高護蓋開啟.On();
                            outBit_Z1非接觸式測高護蓋關閉.Off();
                            break;
                        case false:
                        default:
                            outBit_Z1非接觸式測高護蓋開啟.Off();
                            outBit_Z1非接觸式測高護蓋關閉.On();
                            OB.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1非接觸式測高護蓋開啟

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1非接觸式測高護蓋關閉

                case "outBit_Z1非接觸式測高護蓋關閉":
                    switch (b)
                    {
                        case true:
                            outBit_Z1非接觸式測高護蓋關閉.On();
                            outBit_Z1非接觸式測高護蓋開啟.Off();
                            break;
                        case false:
                        default:
                            outBit_Z1非接觸式測高護蓋關閉.Off();
                            outBit_Z1非接觸式測高護蓋開啟.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1非接觸式測高護蓋關閉

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2非接觸式測高護蓋開啟

                case "outBit_Z2非接觸式測高護蓋開啟":
                    switch (b)
                    {
                        case true:
                            outBit_Z2非接觸式測高護蓋開啟.On();
                            outBit_Z2非接觸式測高護蓋關閉.Off();
                            break;
                        case false:
                        default:
                            outBit_Z2非接觸式測高護蓋開啟.Off();
                            outBit_Z2非接觸式測高護蓋關閉.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2非接觸式測高護蓋開啟

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2非接觸式測高護蓋關閉

                case "outBit_Z2非接觸式測高護蓋關閉":
                    switch (b)
                    {
                        case true:
                            outBit_Z2非接觸式測高護蓋關閉.On();
                            outBit_Z2非接觸式測高護蓋開啟.Off();
                            break;
                        case false:
                        default:
                            outBit_Z2非接觸式測高護蓋關閉.Off();
                            outBit_Z2非接觸式測高護蓋開啟.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2非接觸式測高護蓋關閉

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1刀輪護蓋開啟

                case "outBit_Z1刀輪護蓋開啟":
                    switch (b)
                    {
                        case true:
                            outBit_Z1刀輪護蓋開啟.On();
                            outBit_Z1刀輪護蓋關閉.Off();
                            break;
                        case false:
                        default:
                            outBit_Z1刀輪護蓋開啟.Off();
                            outBit_Z1刀輪護蓋關閉.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1刀輪護蓋開啟

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1刀輪護蓋關閉

                case "outBit_Z1刀輪護蓋關閉":
                    switch (b)
                    {
                        case true:
                            outBit_Z1刀輪護蓋關閉.On();
                            outBit_Z1刀輪護蓋開啟.Off();
                            break;
                        case false:
                        default:
                            outBit_Z1刀輪護蓋關閉.Off();
                            outBit_Z1刀輪護蓋開啟.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1刀輪護蓋關閉

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2刀輪護蓋開啟

                case "outBit_Z2刀輪護蓋開啟":
                    switch (b)
                    {
                        case true:
                            outBit_Z2刀輪護蓋開啟.On();
                            outBit_Z2刀輪護蓋關閉.Off();
                            break;
                        case false:
                        default:
                            outBit_Z2刀輪護蓋開啟.Off();
                            outBit_Z2刀輪護蓋關閉.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2刀輪護蓋開啟

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2刀輪護蓋關閉

                case "outBit_Z2刀輪護蓋關閉":
                    switch (b)
                    {
                        case true:
                            outBit_Z2刀輪護蓋關閉.On();
                            outBit_Z2刀輪護蓋開啟.Off();
                            break;
                        case false:
                        default:
                            outBit_Z2刀輪護蓋關閉.Off();
                            outBit_Z2刀輪護蓋開啟.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2刀輪護蓋關閉

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1水開關電磁閥

                case "outBit_Z1水開關電磁閥":
                    switch (b)
                    {
                        case true:
                            outBit_Z1水開關電磁閥.On();
                            break;
                        case false:
                        default:
                            outBit_Z1水開關電磁閥.Off();
                            //analogOut_Z1噴水座流量控制.Value = 0;
                            //analogOut_Z1灑水座流量控制.Value = 0;
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z1水開關電磁閥

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2水開關電磁閥

                case "outBit_Z2水開關電磁閥":
                    switch (b)
                    {
                        case true:
                            outBit_Z2水開關電磁閥.On();
                            break;
                        case false:
                        default:
                            outBit_Z2水開關電磁閥.Off();
                            //analogOut_Z2噴水座流量控制.Value = 0;
                            //analogOut_Z2灑水座流量控制.Value = 0;
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_Z2水開關電磁閥

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_切割平台真空建立

                case "outBit_切割平台真空建立":
                    switch (b)
                    {
                        case true:
                            Pump_stop_timer.Restart();
                            outBit_真空幫浦啟動.On();
                            //+ By Max 20210416, v4.0.1.62
                            outBit_真空幫浦運轉.On();
                            outBit_切割平台真空建立.On();
                            outBit_切割平台真空破壞.Off();
                            break;
                        case false:
                        default:
                            outBit_切割平台真空建立.Off();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_切割平台真空建立

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_切割平台真空破壞

                case "outBit_切割平台真空破壞":
                    switch (b)
                    {
                        case true:
                            outBit_切割平台真空破壞.On();
                            outBit_切割平台真空建立.Off();
                            break;
                        case false:
                        default:
                            outBit_切割平台真空破壞.Off();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_切割平台真空破壞

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_換刀區安全門閉鎖

                case "outBit_換刀區安全門閉鎖":
                    switch (b)
                    {
                        case true:
                            outBit_換刀區安全門閉鎖.On();
                            outBit_換刀區安全門開鎖.Off();
                            break;
                        case false:
                        default:
                            outBit_換刀區安全門閉鎖.Off();
                            outBit_換刀區安全門開鎖.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_換刀區安全門閉鎖

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_測高塊偵測檔片接觸

                case "outBit_測高塊偵測檔片接觸":
                    switch (b)
                    {
                        case true:
                            outBit_測高塊偵測檔片接觸.On();
                            outBit_測高塊偵測檔片脫離.Off();
                            break;
                        case false:
                        default:
                            outBit_測高塊偵測檔片接觸.Off();
                            outBit_測高塊偵測檔片脫離.On();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_測高塊偵測檔片接觸

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_測高塊偵測檔片脫離

                case "outBit_測高塊偵測檔片脫離":
                    switch (b)
                    {
                        case true:
                            outBit_測高塊偵測檔片接觸.Off();
                            outBit_測高塊偵測檔片脫離.On();
                            break;
                        case false:
                        default:
                            outBit_測高塊偵測檔片接觸.On();
                            outBit_測高塊偵測檔片脫離.Off();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_outBit_測高塊偵測檔片脫離

                #region 私有函數_Scofield_v4.0.1.9[優化]IO Function_其他

                default:
                    switch (b)
                    {
                        case true:
                            OB.On();
                            break;
                        case false:
                        default:
                            OB.Off();
                            break;
                    }
                    break;

                #endregion 私有函數_Scofield_v4.0.1.9[優化]IO Function_其他
            }
        }

        #endregion 私有函數_Scofield_v4.0.1.8[新增]所有OutBit集中funtion控制

        private bool GetSparkOff()
        {
            SetOutBit(outBit_接觸式測高功能開啟, false);
            return inBit_接觸測高導通訊號.Off(SReadValue("iChkSparkDelay").ToInt());
        }

        /// <summary>
        /// 取得當下的清潔Table
        /// </summary>
        /// <param name="enCleanType"></param>
        /// <returns></returns>
        private DataTable GetCleanTable(enCleanType enCleanType)
        {
            DataTable ResultTable = null;

            var _dicLookUp = new Dictionary<enCleanType, Action>()
            {
                 {enCleanType.入料前清洗, () => ResultTable = 入料前清洗DataTable.Copy()},
                 {enCleanType.掃靶前清洗, () => ResultTable = 掃靶前清洗DataTable.Copy()},
                 {enCleanType.切割後清洗, () => ResultTable = 切割後清洗DataTable.Copy()}
            };

            if (_dicLookUp.ContainsKey(enCleanType)) _dicLookUp[enCleanType].Invoke();

            return ResultTable;
        }

        private void MotorOverTime(string s)
        {
            ShowAlarm("E", 15, s);
        }

        private bool RotateCorrect(int OrgX, int OrgY, ref Point Pos1, ref Point Pos2, ref double Angle)
        {
            int X1 = Pos1.X;
            int Y1 = Pos1.Y;
            int X2 = Pos2.X;
            int Y2 = Pos2.Y;

            int Dis_X = X1 - X2;
            int Dis_Y = Y1 - Y2;

            if (Dis_X != 0)
            {
                Angle = Math.Atan2(Dis_X, Dis_Y) * 180 / Math.PI;
            }
            if (Angle > 90)
            {
                Angle = Angle - 180;
            }
            if (Angle < -90)
            {
                Angle = Angle + 180;
            }

            if (Math.Abs(Angle) > 10)
            {
                string str = CreateDialogMsgContent("Text_Saw_RotateCorrect_Angle_Over_Ten");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                //MessageBox.Show("角度大於十度!!");
                return false;
            }
            else
            {
                Pos1.X = Convert.ToInt32((X1 - OrgX) * Math.Cos(Angle * Math.PI / 180) - (Y1 - OrgY) * Math.Sin(Angle * Math.PI / 180) + OrgX);
                Pos1.Y = Convert.ToInt32((X1 - OrgX) * Math.Sin(Angle * Math.PI / 180) + (Y1 - OrgY) * Math.Cos(Angle * Math.PI / 180) + OrgY);

                Pos2.X = Convert.ToInt32((X2 - OrgX) * Math.Cos(Angle * Math.PI / 180) - (Y2 - OrgY) * Math.Sin(Angle * Math.PI / 180) + OrgX);
                Pos2.Y = Convert.ToInt32((X2 - OrgX) * Math.Sin(Angle * Math.PI / 180) + (Y2 - OrgY) * Math.Cos(Angle * Math.PI / 180) + OrgY);
                return true;
            }
        }

        private bool RotatePoint(int OrgX, int OrgY, ref Point Pos1, ref Point Pos2, double Angle)
        {
            int X1 = Pos1.X;
            int Y1 = Pos1.Y;
            int X2 = Pos2.X;
            int Y2 = Pos2.Y;

            Pos1.X = Convert.ToInt32((X1 - OrgX) * Math.Cos(Angle * Math.PI / 180) - (Y1 - OrgY) * Math.Sin(Angle * Math.PI / 180) + OrgX);
            Pos1.Y = Convert.ToInt32((X1 - OrgX) * Math.Sin(Angle * Math.PI / 180) + (Y1 - OrgY) * Math.Cos(Angle * Math.PI / 180) + OrgY);

            Pos2.X = Convert.ToInt32((X2 - OrgX) * Math.Cos(Angle * Math.PI / 180) - (Y2 - OrgY) * Math.Sin(Angle * Math.PI / 180) + OrgX);
            Pos2.Y = Convert.ToInt32((X2 - OrgX) * Math.Sin(Angle * Math.PI / 180) + (Y2 - OrgY) * Math.Cos(Angle * Math.PI / 180) + OrgY);
            return true;
        }

        private void Saw_Home_Reset()
        {
            //Ted modify 20170606 #HomeReset時，鎖住單控#
            outBit_Z1_SWITCH.LockUI = true;
            outBit_Z2_SWITCH.LockUI = true;
            outBit_Z1_REST_ENABLED.LockUI = true;
            outBit_Z2_REST_ENABLED.LockUI = true;
            analogOut_Z1主軸轉速類比控制.LockUI = true;
            analogOut_Z2主軸轉速類比控制.LockUI = true;
            outBit_主軸軸承鎖定.LockUI = false;
        }

        private void Load_CleanData()
        {
            if (入料前清洗DataTable != null)
            {
                入料前清洗DataTable.Dispose();
                入料前清洗DataTable = null;
            }

            if (掃靶前清洗DataTable != null)
            {
                掃靶前清洗DataTable.Dispose();
                掃靶前清洗DataTable = null;
            }

            if (切割後清洗DataTable != null)
            {
                切割後清洗DataTable.Dispose();
                切割後清洗DataTable = null;
            }

            入料前清洗DataTable = PReadTable("DGVCleanBeforeLoad");
            //掃靶前清洗DataTable = PReadTable("掃靶前Clean", "CleanAfterCut");
            切割後清洗DataTable = PReadTable("DGVCleanAfterCut");

        }

        private bool Load_CutData()
        {
            bool bRet = false;

            if (AutoCut.CutData != null)
            {
                AutoCut.CutData.Dispose();
                AutoCut.CutData = null;
            }

            AutoCut.CutData = PReadTable("DGVCutProg");

            if (!AutoCut.CutData.Columns.Contains("Cut_入板速度"))
            {
                AutoCut.CutData.Columns.Add("Cut_入板速度");
            }

            //v0.0.7.9 By Sanxiu 只用單刀功能
            enUseSpidleType type = (enUseSpidleType)(OReadValue("iUseSpindleType").ToInt());

            if (OReadValue("bSeeAndCut").ToBoolean())
            {
                type = enUseSpidleType.Z2單刀;
            }

            if (type != enUseSpidleType.雙刀)
            {
                DataTable dt = new DataTable();
                enSpindleSel sel = type == enUseSpidleType.Z1單刀 ? enSpindleSel.Z1 : enSpindleSel.Z2;

                dt = Change_CutData(sel);

                if (AutoCut.CutData != null)
                {
                    AutoCut.CutData.Dispose();
                    AutoCut.CutData = null;
                }

                AutoCut.CutData = dt;
            }

            if (OReadValue("bSeeAndCut").ToBoolean())
            {
                int nCutNumCh2 = PReadValue("Num_col").ToInt() + 1;
                int nCutNumCh1 = PReadValue("Num_row").ToInt() + 1;

                #region Sort ascending by column named Cut_Z2刀順.
                Dictionary<string, int> dicIdxCut2 = new Dictionary<string, int>();
                foreach (DataRow dr in AutoCut.CutData.Rows)
                {
                    dicIdxCut2.Add(dr["Cut_Index"].ToString(), Convert.ToInt32(dr["Cut_Z2刀順"]));
                }

                foreach (KeyValuePair<string, int> item in dicIdxCut2.OrderBy(key => key.Value))
                {
                    AutoCut.CutData.MoveSpecificRowToLast(string.Format("Cut_Index = '{0}'", item.Key));
                }
                #endregion

                #region Sort descending by column named Cut_料片方向
                AutoCut.CutData = AutoCut.CutData.Sort("Cut_料片方向 DESC");
                #endregion

                #region move the edges of two side and two direction to last
                AutoCut.CutData.MoveSpecificRowToLast("Cut_料片方向 = 'CH1' and Cut_Z2刀順 = '1'");
                AutoCut.CutData.MoveSpecificRowToLast(string.Format("Cut_料片方向 = 'CH1' and Cut_Z2刀順 = '{0}'", nCutNumCh1.ToString()));
                AutoCut.CutData.MoveSpecificRowToLast("Cut_料片方向 = 'CH2' and Cut_Z2刀順 = '1'");
                AutoCut.CutData.MoveSpecificRowToLast(string.Format("Cut_料片方向 = 'CH2' and Cut_Z2刀順 = '{0}'", nCutNumCh2.ToString()));
                #endregion

                #region 為了處理邊條先看不切, 複製最後四列, 並且加入兩行表示只看不切或是只切不看
                int nRowCnt = AutoCut.CutData.Rows.Count;
                AutoCut.CutData.ImportRow(AutoCut.CutData.Rows[nRowCnt - 4]);
                AutoCut.CutData.ImportRow(AutoCut.CutData.Rows[nRowCnt - 3]);
                AutoCut.CutData.ImportRow(AutoCut.CutData.Rows[nRowCnt - 2]);
                AutoCut.CutData.ImportRow(AutoCut.CutData.Rows[nRowCnt - 1]);

                DataColumn col = new DataColumn("See", typeof(Boolean));
                col.DefaultValue = true;
                AutoCut.CutData.Columns.Add(col);

                col = new DataColumn("Cut", typeof(Boolean));
                col.DefaultValue = true;
                AutoCut.CutData.Columns.Add(col);
                #endregion

                #region 重新填回目前刀續table的index
                for (int i = 0; i < AutoCut.CutData.Rows.Count; i++)
                {
                    AutoCut.CutData.Rows[i]["Cut_Index"] = (i + 1).ToString();

                    if (i >= (AutoCut.CutData.Rows.Count - 4 * 2) &&
                        i < AutoCut.CutData.Rows.Count - 4)
                    {
                        AutoCut.CutData.Rows[i]["Cut"] = false;
                    }

                    if (i >= AutoCut.CutData.Rows.Count - 4)
                    {
                        AutoCut.CutData.Rows[i]["See"] = false;
                    }
                }
                #endregion

                NLogger.Debug("即看即切調整後刀續 +");
                PrintOut(AutoCut.CutData, true);
                NLogger.Debug("即看即切調整後刀續 -");
            }

            if (AutoCut.CutData.Rows.Count > 0)
                bRet = true;

            return bRet;

        }

        private void PrintOut(DataTable dt, bool bIsConsole = false)
        {
            DataRow[] drArr = dt.Select();

            //// Print column 0 of each returned row.
            for (int i = 0; i < drArr.Length; i++)
            {
                string strTmp = "";

                for (int j = 0; j < drArr[0].Table.Columns.Count; j++)
                {
                    //Console.Write(drArr[i][j]);
                    string str = string.Format("{0}", drArr[i][j]);
                    if (bIsConsole) Console.Write(str.PadRight(10, ' '));
                    strTmp += str.PadRight(7, ' ').PadLeft(10, ' ');
                }

                NLogger.Debug(strTmp);
                if (bIsConsole) Console.WriteLine();
            }

            if (bIsConsole) Console.WriteLine();
        }

        #region v4.0.1.28 順序掃描，0:實際轉速、1:參考轉速、2:驅動器負載、3:主軸負載

        private enSpindleParamSel enZ1ScanSel, enZ2ScanSel;
        private string Z1ActualSpeed, Z1RefSpeed, Z1LoadAMP, Z1LoadMotor;
        private string Z2ActualSpeed, Z2RefSpeed, Z2LoadAMP, Z2LoadMotor;
        private float m_flZ1LoadMotorMax, m_flZ1LoadAmpMax;
        private float m_flZ2LoadMotorMax, m_flZ2LoadAmpMax;

        private bool ScanSpindleInfo(enSpindleSel enSel)
        {
            bool sRet = false;
            Spindle sp = enSel == enSpindleSel.Z1 ? Z1Spindle : Z2Spindle;
            enSpindleParamSel enParamSel = enSel == enSpindleSel.Z1 ? enZ1ScanSel : enZ2ScanSel;

            switch (enParamSel)
            {
                case enSpindleParamSel.實際轉數:
                    {
                        if (sp.ReadStatus(eReadStatusType.ActualVelocity) == eTaskRet.eCurrentTaskDone)
                        {
                            if (enSel == enSpindleSel.Z1)
                            {
                                Z1ActualSpeed = String.Format("{0}", sp.ActualRPM);
                                enZ1ScanSel = enSpindleParamSel.參考轉數;
                            }
                            else
                            {
                                Z2ActualSpeed = String.Format("{0}", sp.ActualRPM);
                                enZ2ScanSel = enSpindleParamSel.參考轉數;
                            }

                            sRet = true;
                        }
                    }
                    break;

                case enSpindleParamSel.參考轉數:
                    {
                        if (sp.ReadStatus(eReadStatusType.ReferenceVelocity) == eTaskRet.eCurrentTaskDone)
                        {
                            if (enSel == enSpindleSel.Z1)
                            {
                                Z1RefSpeed = String.Format("{0}", sp.RefRPM);
                                enZ1ScanSel = enSpindleParamSel.驅動器負載;
                            }
                            else
                            {
                                Z2RefSpeed = String.Format("{0}", sp.RefRPM);
                                enZ2ScanSel = enSpindleParamSel.驅動器負載;
                            }

                            sRet = true;
                        }
                    }
                    break;

                case enSpindleParamSel.驅動器負載:
                    {
                        if (sp.ReadStatus(eReadStatusType.ActualDriverLoad) == eTaskRet.eCurrentTaskDone)
                        {
                            if (enSel == enSpindleSel.Z1)
                            {
                                Z1LoadAMP = String.Format("{0}", sp.LoadAMP);
                                enZ1ScanSel = enSpindleParamSel.主軸負載;
                                if (sp.LoadAMP > m_flZ1LoadAmpMax) m_flZ1LoadAmpMax = sp.LoadAMP;
                            }
                            else
                            {
                                Z2LoadAMP = String.Format("{0}", sp.LoadAMP);
                                enZ2ScanSel = enSpindleParamSel.主軸負載;
                                if (sp.LoadAMP > m_flZ2LoadAmpMax) m_flZ2LoadAmpMax = sp.LoadAMP;
                            }

                            sRet = true;
                        }
                    }
                    break;

                case enSpindleParamSel.主軸負載:
                    {
                        if (sp.ReadStatus(eReadStatusType.ActualMotorLoad) == eTaskRet.eCurrentTaskDone)
                        {
                            if (enSel == enSpindleSel.Z1)
                            {
                                Z1LoadMotor = String.Format("{0}", sp.LoadMotor);
                                enZ1ScanSel = enSpindleParamSel.實際轉數;
                                if (sp.LoadMotor > m_flZ1LoadMotorMax) m_flZ1LoadMotorMax = sp.LoadMotor;
                            }
                            else
                            {
                                Z2LoadMotor = String.Format("{0}", sp.LoadMotor);
                                enZ2ScanSel = enSpindleParamSel.實際轉數;
                                if (sp.LoadMotor > m_flZ2LoadMotorMax) m_flZ2LoadMotorMax = sp.LoadMotor;
                            }

                            sRet = true;
                        }
                    }
                    break;
            }
            return sRet;
        }

        #endregion

        public string GetSpindleInfo(string spindle, string target)
        {
            if (spindle == "Z1")
            {
                if (target == "RPM")
                    return Z1ActualSpeed;
                else if (target == "MotorLoad")
                    return Z1LoadMotor;
                else if (target == "AMPLoad")
                    return Z1LoadAMP;
                else
                    return "";
            }
            else if (spindle == "Z2")
            {
                if (target == "RPM")
                    return Z2ActualSpeed;
                else if (target == "MotorLoad")
                    return Z2LoadMotor;
                else if (target == "AMPLoad")
                    return Z2LoadAMP;
                else
                    return "";
            }
            else
                return "";
        }

        //v4.0.1.28 換刀區安全門是否開啟
        private bool bCutDoorSafe = false;
        public void SetCutDoorSafety(bool Rset)
        {
            //v4.0.1.32 新增一功能開後安全門也不會停主軸
            if (IsSimulation() != 0 || SReadValue("bIgnoreCutSafeDoor").ToBoolean())
            {
                bCutDoorSafe = true;
            }
            else
            {
                bCutDoorSafe = Rset;
            }
        }

        public void SetFrontDoorLock(bool bSet)
        {
            outBit_前安全門鎖.Value = bSet;
        }

        public DataTable Get_CutData()   //v0.0.7.8 By Sanxiu 修正Package存儲方式 
        {
            DataTable ReTable = PReadTable("DGVCutProg");
            return ReTable;
        }

        private bool Load_PCBData()
        {
            bool bRet = false;

            if (LearnMarkPos.dtScanData != null)
            {
                LearnMarkPos.dtScanData.Dispose();
                LearnMarkPos.dtScanData = null;
            }

            LearnMarkPos.dtScanData = PReadTable("DGVPCBData");

            if (LearnMarkPos.dtScanData.Rows.Count == PReadValue("Num_col").ToInt() + PReadValue("Num_row").ToInt() + 2)
                bRet = true;

            return bRet;
        }

        private bool Load_FixtureData()
        {
            bool bRet = false;

            if (TeachFixture.dtFixtureData != null)
            {
                TeachFixture.dtFixtureData.Dispose();
                TeachFixture.dtFixtureData = null;
            }

            TeachFixture.dtFixtureData = PReadTable("DGVFixtureData");

            if (TeachFixture.dtFixtureData.Rows.Count == PReadValue("Num_col").ToInt() + PReadValue("Num_row").ToInt() + 2)
                bRet = true;

            return bRet;
        }

        private bool Find_PCBData(string sSide, int iLineNo, out int iX1, out int iY1, out int iX2, out int iY2, out int iθ, out int iZ, out int iXOffset)
        {
            bool bRet = false;

            iX1 = 0;
            iY1 = 0;
            iX2 = 0;
            iY2 = 0;
            iθ = 0;
            iZ = 0;
            iXOffset = 0;

            if (LearnMarkPos.dtScanData != null)
            {
                DataRow[] SelectDataRow = LearnMarkPos.dtScanData.Select("Scan_Side='" + sSide + "' and Scan_LineNo='" + iLineNo.ToString() + "'");

                if (SelectDataRow.Length == 1)
                {
                    bRet = true;
                    iX1 = Convert.ToInt32(SelectDataRow[0]["Scan_X1"].ToString());
                    iY1 = Convert.ToInt32(SelectDataRow[0]["Scan_Y1"].ToString());
                    iX2 = Convert.ToInt32(SelectDataRow[0]["Scan_X2"].ToString());
                    iY2 = Convert.ToInt32(SelectDataRow[0]["Scan_Y2"].ToString());
                    iθ = Convert.ToInt32(SelectDataRow[0]["Scan_Angle"].ToString());
                    iZ = Convert.ToInt32(SelectDataRow[0]["Scan_Z"].ToString());
                    iXOffset = Convert.ToInt32(SelectDataRow[0]["Scan_XOffset"].ToString());
                }
            }

            return bRet;
        }

        private bool Find_FixtureData(string sSide, int iLineNo, out int iX, out int iθ, out int iXOffset)
        {
            bool bRet = false;

            iX = 0;
            iθ = 0;
            iXOffset = 0;

            if (TeachFixture.dtFixtureData != null)
            {
                DataRow[] SelectDataRow = TeachFixture.dtFixtureData.Select("Side_Fixture='" + sSide + "' and LineNo_Fixture='" + iLineNo.ToString() + "'");

                if (SelectDataRow.Length == 1)
                {
                    bRet = true;
                    iX = Convert.ToInt32(SelectDataRow[0]["X_Fixture"].ToString());
                    iθ = Convert.ToInt32(SelectDataRow[0]["Angle_Fixture"].ToString());
                    iXOffset = Convert.ToInt32(SelectDataRow[0]["XOffset_Fixture"].ToString());
                }

            }

            return bRet;
        }

        private bool ChannelRotation(string sChannel, out int θ位移)
        {
            bool bRet = true;

            string strCHT = (PReadValue("CH_Top").ToInt() + 1).ToString();
            string strCHR = (PReadValue("CH_Right").ToInt() + 1).ToString();
            string strCHB = (PReadValue("CH_Bottom").ToInt() + 1).ToString();
            string strCHL = (PReadValue("CH_Left").ToInt() + 1).ToString();

            if (sChannel == "CH" + (PReadValue("CH_Top").ToInt() + 1).ToString())
                θ位移 = 0;
            else if (sChannel == "CH" + (PReadValue("CH_Right").ToInt() + 1).ToString())
                θ位移 = 1;
            else if (sChannel == "CH" + (PReadValue("CH_Bottom").ToInt() + 1).ToString())
                θ位移 = 2;
            else if (sChannel == "CH" + (PReadValue("CH_Left").ToInt() + 1).ToString())
                θ位移 = 3;
            else
            {
                θ位移 = 0;
                bRet = false;
            }
            return bRet;
        }

        /// <summary>
        /// v0.0.7.9 By Sanxiu 只用單刀功能
        /// </summary>
        /// <param name="sCutTpe">需使用的那軸</param>
        /// <returns>轉出的單刀刀順</returns>
        private DataTable Change_CutData(enSpindleSel spindleSel)
        {
            DataTable ResultTable = AutoCut.CutData.Clone();

            //+ By Max 20210219, 破刀後單刀續切，新的DataTable須包含已切過的Row，否則會有刀序未正確執行
            for (int i = 0; i < AutoCut.Step; ++i)
            {
                ResultTable.ImportRow(AutoCut.CutData.Rows[i]);
            }

            // 修改刀序
            for (int i = AutoCut.Step; i < AutoCut.CutData.Rows.Count; i++)
            {
                if (spindleSel == enSpindleSel.Z1)
                {
                    if (Convert.ToBoolean(AutoCut.CutData.Rows[i]["Cut_Z1使用"]))
                    {
                        ResultTable.ImportRow(AutoCut.CutData.Rows[i]);
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Index"] = ResultTable.Rows.Count;
                        //ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1使用"] = AutoCut.CutData.Rows[i]["Cut_UseZ1"];
                        //ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1刀順"] = AutoCut.CutData.Rows[i]["Cut_LineNoZ1"];
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2使用"] = false;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2刀順"] = 0;
                    }

                    if (Convert.ToBoolean(AutoCut.CutData.Rows[i]["Cut_Z2使用"]))
                    {
                        ResultTable.ImportRow(AutoCut.CutData.Rows[i]);
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Index"] = ResultTable.Rows.Count;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1使用"] = AutoCut.CutData.Rows[i]["Cut_Z2使用"];
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1刀順"] = AutoCut.CutData.Rows[i]["Cut_Z2刀順"];
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2使用"] = false;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2刀順"] = 0;
                    }
                }
                else if (spindleSel == enSpindleSel.Z2)
                {
                    if (Convert.ToBoolean(AutoCut.CutData.Rows[i]["Cut_Z1使用"]))
                    {
                        ResultTable.ImportRow(AutoCut.CutData.Rows[i]);
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Index"] = ResultTable.Rows.Count;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1使用"] = false;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1刀順"] = 0;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2使用"] = AutoCut.CutData.Rows[i]["Cut_Z1使用"];
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2刀順"] = AutoCut.CutData.Rows[i]["Cut_Z1刀順"];
                    }

                    if (Convert.ToBoolean(AutoCut.CutData.Rows[i]["Cut_Z2使用"]))
                    {
                        ResultTable.ImportRow(AutoCut.CutData.Rows[i]);
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Index"] = ResultTable.Rows.Count;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1使用"] = false;
                        ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z1刀順"] = 0;
                        //ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2使用"] = AutoCut.CutData.Rows[i]["Cut_Z2使用"];
                        //ResultTable.Rows[ResultTable.Rows.Count - 1]["Cut_Z2刀順"] = AutoCut.CutData.Rows[i]["Cut_Z2刀順"];
                    }
                }
            }

            return ResultTable;
        }

        #region 破刀偵測機制

        bool m_bIsZ1err = false;
        bool m_bIsZ2err = false;
        bool m_bIsZ1Broken = false;
        bool m_bIsZ2Broken = false;

        double dZ1BaseValue;
        double dZ2BaseValue;
        MyTimer oZ1破刀DelayTimer = new MyTimer(true);
        MyTimer oZ2破刀DelayTimer = new MyTimer(true);

        //+ By Max 20211030
        private bool IsCutBroken(enSpindleSel sel, ref double dNowVal)
        {
            bool bRet = false;

            if (!SReadValue("bIgnoreCutSafeDoor").ToBoolean())
            {
                if (IsUseDAQ)
                {
                    if (sel == enSpindleSel.Z1)
                    {
                        dNowVal = td_Z1破片檢知.BrokenValue;
                        bRet = td_Z1破片檢知.IsBroken;
                    }
                    else if (sel == enSpindleSel.Z2)
                    {
                        dNowVal = td_Z2破片檢知.BrokenValue;
                        bRet = td_Z2破片檢知.IsBroken;
                    }
                }
                else
                {
                    double dBreakVolThres = SReadValue("dBreakVolThreshold").ToDouble();

                    if (sel == enSpindleSel.Z1)
                    {
                        dNowVal = analogIn_Z1破片檢知.AvgValue;
                        bRet = dNowVal > dBreakVolThres;
                    }
                    else if (sel == enSpindleSel.Z2)
                    {
                        dNowVal = analogIn_Z2破片檢知.AvgValue;
                        bRet = dNowVal > dBreakVolThres;
                    }
                }
            }

            return bRet;
        }

        private bool IsCutBrokenByRate(enSpindleSel sel, ref double dNowVal)
        {
            bool bRet = false;

            double dBreakRateofChange = SReadValue("dBreakRateofChange").ToDouble() / 100.0; // 破刀變化率
            int iBreakDetIntervalTime = SReadValue("iBreakDetIntervalTime").ToInt(); // 破刀偵測前後值間隔時間

            if (!SReadValue("bIgnoreCutSafeDoor").ToBoolean() && SReadValue("bUseBreakChkByRate").ToBoolean())
            {
                if (sel == enSpindleSel.Z1)
                {
                    if (dZ1BaseValue == 0)
                    {
                        dZ1BaseValue = analogIn_Z1破片檢知.AvgValue;
                    }
                    else
                    {
                        if (oZ1破刀DelayTimer.On(iBreakDetIntervalTime))
                        {
                            dNowVal = analogIn_Z1破片檢知.AvgValue;

                            bRet = (dNowVal - dZ1BaseValue) > (dZ1BaseValue * dBreakRateofChange);
                            if (!bRet) dZ1BaseValue = analogIn_Z1破片檢知.AvgValue;

                            oZ1破刀DelayTimer.Restart();
                        }
                    }
                }
                else if (sel == enSpindleSel.Z2)
                {
                    if (dZ2BaseValue == 0)
                    {
                        dZ2BaseValue = analogIn_Z2破片檢知.AvgValue;
                    }
                    else
                    {
                        if (oZ2破刀DelayTimer.On(iBreakDetIntervalTime))
                        {
                            dNowVal = analogIn_Z2破片檢知.AvgValue;

                            bRet = (dNowVal - dZ2BaseValue) > (dZ2BaseValue * dBreakRateofChange);
                            if (!bRet) dZ2BaseValue = analogIn_Z1破片檢知.AvgValue;

                            oZ2破刀DelayTimer.Restart();
                        }
                    }
                }
            }

            return bRet;
        }

        #endregion

        private int Z1SpindleProtection()
        {
            double dNowVal = 0;

            if (m_enZ1SpindleStatus == enSpindleStatus.Starting || m_enZ1SpindleStatus == enSpindleStatus.Run)
            {
                if (!GetInBit(inBit_主軸空壓正常))
                {
                    ShowAlarm("E", 37);//主軸控制Z1：啟動中，主軸空壓不足或訊號異常，請檢查
                    return -1;
                }
                else if (!GetInBit(inBit_冰水機正常運轉中))
                {
                    ShowAlarm("E", 38);//主軸控制Z1：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return -2;
                }
                else if (!GetInBit(inBit_Z1主軸冷卻水流量偵測))
                {
                    ShowAlarm("E", 39);//主軸控制Z1：啟動中，冷卻水流量不足或訊號異常，請檢查
                    return -3;
                }
                else if (!GetInBit(inBit_Z1主軸正常))
                {
                    ShowAlarm("E", 40);//主軸控制Z1：啟動中，變頻器或訊號異常，請檢查
                    return -4;
                }
                else if (analogIn_主軸空壓值.AvgValue < iSpindleAir空壓偵測數值)
                {
                    ShowAlarm("E", 41);//主軸控制Z1：啟動中，主軸空壓數值小於安全設定值，請檢查
                    return -5;
                }
                else if (IsCutBrokenByRate(enSpindleSel.Z1, ref dNowVal))
                {
                    ShowAlarm("E", 87, "Z1", string.Format("{0:0.000}", dZ1BaseValue), string.Format("{0:0.000}", dNowVal));   //破刀電壓變化率超出設定值
                    m_bIsZ1Broken = true;
                    return -6;
                }
                else if (IsCutBroken(enSpindleSel.Z1, ref dNowVal) || m_bIsZ1err)
                {
                    //+ By Max 20211030
                    double dBreakVolThres = IsUseDAQ ? 2.5 : SReadValue("dBreakVolThreshold").ToDouble();
                    ShowAlarm("E", 88, "Z1", string.Format("{0:0.000}", dBreakVolThres), string.Format("{0:0.000}", dNowVal));   //破刀電壓超出門檻值
                    m_bIsZ1err = false;
                    m_bIsZ1Broken = true;
                    return -7;
                }
                else if (m_enZ1SpindleStatus == enSpindleStatus.Run && inBit_Z1主軸達設定值.Off(1000))
                {
                    ShowAlarm("E", 42);//主軸控制Z1：運轉中，未達轉速設定值，請檢查
                    return -8;
                }
                else
                {
                    return 0;
                }
            }
            else return 0;
        }

        private int Z2SpindleProtection()
        {
            double dNowVal = 0;

            if (m_enZ2SpindleStatus == enSpindleStatus.Starting || m_enZ2SpindleStatus == enSpindleStatus.Run)
            {
                if (!GetInBit(inBit_主軸空壓正常))
                {
                    ShowAlarm("E", 43);//主軸控制Z2：啟動中，主軸空壓不足或訊號異常，請檢查
                    return -1;
                }
                else if (!GetInBit(inBit_冰水機正常運轉中))
                {
                    ShowAlarm("E", 44);//主軸控制Z2：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return -2;
                }
                else if (!GetInBit(inBit_Z2主軸冷卻水流量偵測))
                {
                    ShowAlarm("E", 45);//主軸控制Z2：啟動中，冷卻水流量不足或訊號異常，請檢查
                    return -3;
                }
                else if (!GetInBit(inBit_Z2主軸正常))
                {
                    ShowAlarm("E", 46);//主軸控制Z2：啟動中，變頻器或訊號異常，請檢查
                    return -4;
                }
                else if (analogIn_主軸空壓值.AvgValue < iSpindleAir空壓偵測數值)
                {
                    ShowAlarm("E", 47);//主軸控制Z1：啟動中，主軸空壓數值小於安全設定值，請檢查
                    return -5;
                }
                else if (IsCutBrokenByRate(enSpindleSel.Z2, ref dNowVal))
                {
                    ShowAlarm("E", 87, "Z2", string.Format("{0:0.000}", dZ2BaseValue), string.Format("{0:0.000}", dNowVal));   //破刀電壓變化率超出設定值
                    m_bIsZ2Broken = true;
                    return -6;
                }
                else if (IsCutBroken(enSpindleSel.Z2, ref dNowVal) || m_bIsZ2err)
                {
                    //+ By Max 20211030
                    double dBreakVolThres = IsUseDAQ ? 2.5 : SReadValue("dBreakVolThreshold").ToDouble();
                    ShowAlarm("E", 88, "Z2", string.Format("{0:0.000}", dBreakVolThres), string.Format("{0:0.000}", dNowVal));   //破刀電壓超出門檻值
                    m_bIsZ2err = false;
                    m_bIsZ2Broken = true;
                    return -7;
                }
                else if (m_enZ2SpindleStatus == enSpindleStatus.Run && inBit_Z2主軸達設定值.Off(1000))
                {
                    ShowAlarm("E", 48);//主軸控制Z2：運轉中，未達轉速設定值，請檢查
                    return -8;
                }
                else
                {
                    return 0;
                }
            }
            else return 0;
        }

        private bool MonitorSpindle()
        {
            bool bZ1 = true;
            bool bZ2 = true;

            if (IsSimulation() != 0)
            {
                return true;
            }

            bZ1 = (m_enZ1SpindleStatus == enSpindleStatus.Run) || !bUseZ1_Spindle;
            bZ2 = (m_enZ2SpindleStatus == enSpindleStatus.Run) || !bUseZ2_Spindle;

            return bZ1 && bZ2;
        }

        #region 根據產品設定的水流量計算水流量類比電壓
        private Single ConvertFromVolumnToVolt(enCutWaterSel cutWaterSel, enSpindleSel spindleSel)
        {
            double dExpectWaterFlow = 0;
            double dExpectWaterVol = 0;
            double dPowerOnVol = 0;
            double dPowerOnFlow = 0;
            double dPowerOffVol = 0;
            double dGap = 0;

            if (cutWaterSel == enCutWaterSel.SHOWER)
            {
                dExpectWaterFlow = PReadValue("Shower_value").ToDouble();
                dPowerOffVol = SReadValue(string.Format("{0}_Blade_Cooler_Volt_PressOff", spindleSel.ToString())).ToDouble();
                dPowerOnVol = SReadValue(string.Format("{0}_Blade_Cooler_Volt_PressOn", spindleSel.ToString())).ToDouble();
                dPowerOnFlow = SReadValue(string.Format("{0}_Blade_Cooler_KPa_PressOn", spindleSel.ToString())).ToDouble();

                dGap = dPowerOnVol - dPowerOffVol;

                if(dGap == 0) dExpectWaterVol = dExpectWaterFlow;
                else
                {
                    dExpectWaterVol = dExpectWaterFlow / (dPowerOnFlow / dGap) + dPowerOffVol;
                }
            }

            if (cutWaterSel == enCutWaterSel.SPRAY)
            {
                dExpectWaterFlow = PReadValue("Spray_value").ToDouble();
                dPowerOffVol = SReadValue(string.Format("{0}_Spray_Volt_PressOff", spindleSel.ToString())).ToDouble();
                dPowerOnVol = SReadValue(string.Format("{0}_Spray_Volt_PressOn", spindleSel.ToString())).ToDouble();
                dPowerOnFlow = SReadValue(string.Format("{0}_Spray_KPa_PressOn", spindleSel.ToString())).ToDouble();

                dGap = dPowerOnVol - dPowerOffVol;

                if (dGap == 0) dExpectWaterVol = dExpectWaterFlow;
                else
                {
                    dExpectWaterVol = dExpectWaterFlow / (dPowerOnFlow / dGap) + dPowerOffVol;
                }
            }

            return (Single)dExpectWaterVol;
        }
        #endregion

        #region 偵測切割水流量
        private bool CheckWater(enSpindleSel sel)
        {
            //模擬模式或未載料號直接回傳true
            if (IsSimulation() != 0 || PackageName.Length == 0) return true;

            double dShowerFlowNow = sel == enSpindleSel.Z1 ? GetZ1BladeCoolerAIValue(true) : GetZ2BladeCoolerAIValue(true);
            double dSprayFlowNow  = sel == enSpindleSel.Z1 ? GetZ1BladeSprayAIValue(true)  : GetZ2BladeSprayAIValue(true);

            double dTolerance = SReadValue("Water_Flow_Tolerance").ToDouble();
            double dShowFlowByPackage = PReadValue("Shower_value").ToDouble();
            double dSprayFlowByPackage = PReadValue("Spray_value").ToDouble();

            //bool bIsShower = (dShowerFlowNow >= dShowFlowByPackage - dTolerance) && (dShowerFlowNow <= dShowFlowByPackage + dTolerance);
            //bool bIsSpray  = (dSprayFlowNow >= dSprayFlowByPackage - dTolerance) && (dSprayFlowNow <= dSprayFlowByPackage + dTolerance);

            bool bIsShower = (dShowerFlowNow >= dShowFlowByPackage - dTolerance);
            bool bIsSpray =  (dSprayFlowNow >= dSprayFlowByPackage - dTolerance);

            return bIsShower && bIsSpray;
        }
        #endregion

        private string CheckVaca(int Product)
        {//Product= 0:料片 1:麻花捲 2:治具

            string errmsg = "";

            if (IsSimulation() != 0)//模擬狀態皆回傳0
            {
                return errmsg;
            }

            double 真空數值 = 0;

            bool On = false;

            if (Product == 0)
            {
                if (PackageName != "")
                    真空數值 = PReadValue("PCBVacuumThreshold").ToInt();
                if (真空數值 == 0)
                {
                    ShowAlarm("i", 8);
                }
            }
            else
            {
                真空數值 = SReadValue("麻花捲空壓").ToInt();
            }

            double ChuckValue = GetChuckTableAIValue();

            if (Product == 0)
            {
                On = ChuckValue <= 真空數值;
                if (!On)
                {
                    errmsg = "VacStandard" + PReadValue("PCBVacuumThreshold").ToString() + "ChuckTableVacValue" + ChuckValue.ToString();
                }
            }
            else if (Product == 1)
            {
                On = GetChuckTableAIValue() <= 真空數值;
            }
            else if (Product == 2)
            {
                //On = GetChuckTableAIValue() <= 真空數值 && GetCeramicsTableAIValue() <= 真空數值;
                On = true;
            }

            return errmsg;
        }

        #region 里程卡控
        private void CheckSpindleRunDist(enSpindleSel sel)
        {
            double dPosTmp;
            string strFieldName = sel == enSpindleSel.Z1 ? "BladeZ1Data" : "BladeZ2Data";
            string strSubFieldName = sel == enSpindleSel.Z1 ? "dRealMotorDistanceZ1" : "dRealMotorDistanceZ2";

            dPosTmp = PReadValue(strFieldName, strSubFieldName).ToDouble(); //讀回原本值

            //里程監控
            double dContorl;
            strSubFieldName = sel == enSpindleSel.Z1 ? "dLimitZ1" : "dLimitZ2";
            dContorl = PReadValue(strFieldName, strSubFieldName).ToDouble();

            if (dPosTmp > dContorl)
                ShowAlarm("I", sel == enSpindleSel.Z1 ? 5 : 6); //里程已到達
        }
        #endregion

        #region 計算兩點距離
        private double TwoPointDist(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        }
        #endregion

        #region 多軸同動
        private bool MultiMotorsG00(Dictionary<Motor, int> dicMotorPosPairs, MyTimer timer,
            [CallerMemberName] string strMemberName = "",
            [CallerLineNumber] int nSourceLineNumber = 0)
        {
            var _dicLookUp = new Dictionary<Motor, bool>();

            if (dicMotorPosPairs.ContainsKey(motor_切刀橫移馬達_Z1X) &&
                dicMotorPosPairs.ContainsKey(motor_切刀橫移馬達_Z2X))
            {
                int nDiff1 = dicMotorPosPairs[motor_切刀橫移馬達_Z1X] - motor_切刀橫移馬達_Z1X.ReadPos();
                int nDiff2 = dicMotorPosPairs[motor_切刀橫移馬達_Z2X] - motor_切刀橫移馬達_Z2X.ReadPos();

                // Z1X與Z2X同往負方向移動時, Z2X先動
                if (nDiff1 < 0 && nDiff2 < 0)
                {
                    dicMotorPosPairs = 
                        dicMotorPosPairs.OrderByDescending(key => key.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
                }
            }

            foreach (var item in dicMotorPosPairs)
            {
                _dicLookUp.Add(item.Key, item.Key.G00(item.Value));
            }

            if (!_dicLookUp.ContainsValue(false))
            {
                timer.Restart();
                return true;
            }
            else
            {
                if (timer.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    foreach (var item in _dicLookUp)
                    {
                        NLogger.Error(string.Format("{0} 移動逾時, 發生於{1}, 第{2}行呼叫時", item.Key.Text, strMemberName, nSourceLineNumber));
                        if (!item.Value) MotorOverTime(item.Key.Text);
                    }

                    timer.Restart();
                }

                return false;
            }
        }

        private bool MultiMotorsHome(List<Motor> lstMotors, MyTimer timer)
        {
            var _dicLookUp = new Dictionary<Motor, bool>();

            foreach (var item in lstMotors)
            {
                _dicLookUp.Add(item, item.Home());
            }

            if (!_dicLookUp.ContainsValue(false))
            {
                timer.Restart();
                return true;
            }
            else
            {
                if (timer.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    foreach (var item in _dicLookUp)
                        if (!item.Value) MotorOverTime(item.Key.Text);

                    timer.Restart();
                }

                return false;
            }
        }
        #endregion

        #endregion 私有函數

        #region 公用函數

        public float GetKufBreak_MinValue()
        {
            return (float)SReadValue("KufBreak_MinValue").ToDouble();
        }

        public float GetKufBreak_MaxValue()
        {
            return (float)SReadValue("KufBreak_MaxValue").ToDouble();
        }

        public float GetKufBreak_Percen()
        {
            return (float)SReadValue("KufBreak_Percen").ToDouble();
        }

        //v0.0.7.13 By Sanxiu Z2 CCD加入雷射干涉補償
        public bool GetMoterZ2XUseLaserCCD()
        {
            return motor_切刀橫移馬達_Z2X.PitchCOMEnable;
        }

        public bool SetCCDCmd(string Cmd, params string[] Para)
        {
            if ((mSendCCDAction == CCDSendResult.Wait) && (Cmd != ""))
            {
                mCCDAction = CCDSendResult.Working;
                CCDCmd = Cmd;
                RealCCDCmd = Cmd;

                for (int i = 0; i < Para.Length; i++)
                {
                    RealCCDCmd += ",";
                    RealCCDCmd += Para[i];
                }

                return true;
            }

            return false;
        }

        public void SetVisionRefresh()
        {
            if (PackageName != "")
                SetCCDCmd("REDRAW", "1");
            else
            {
                mCMS.WMsgSendCmd("REDRAW,1");
            }
        }

        public void Set_CCDHandle(int i)
        {
            mCMS.Show();
            mCMS.Hide();
            mCMS.Set_CCDHandle(i);
        }

        public void RegisterCrossLineCB(EventHandler callbackFunc)
        {
            mCMS.CrossLineEvent += callbackFunc;
        }

        public void RegisterVerticalLineCB(EventHandler callbackFunc)
        {
            mCMS.VerticalLineEvent += callbackFunc;
        }

        public void SetUIVisible(bool bIsVisible)
        {
            gp開批八項.Visible = bIsVisible;
        }

        //MAinF3控制馬達移動
        public bool MotorMove(string motor, int pos)
        {
            switch (motor)
            {
                case "X1":
                    return motor_切刀橫移馬達_Z1X.G00(pos);

                case "X2":
                    return motor_切刀橫移馬達_Z2X.G00(pos);

                case "Y":
                    return motor_切割平台前後馬達Y.G00(pos);

                case "U":
                    return motor_切割平台旋轉馬達U.G00(pos);

                case "Z1":
                    if (pos > SReadValue("Z2_ZCCDminValue").ToInt())
                    {
                        return motor_切刀上下馬達_Z1.G00(pos);
                    }
                    else
                    {
                        ShowAlarm("W", 27, pos, SReadValue("掃瞄時Z1切刀安全點位_Z").ToInt());
                        break;
                    }

                case "Z2":

                    if (pos > SReadValue("Z2_ZCCDminValue").ToInt())
                    {
                        return motor_切刀上下馬達_Z2.G00(pos);
                    }
                    else
                    {
                        ShowAlarm("W", 27, pos, SReadValue("掃瞄時Z2切刀安全點位_Z").ToInt());
                        break;
                    }
            }

            return false;
        }

        //教導用，使用者按下下一步
        public void SetTeachUserCheckOk()
        {
            TeachUserCheckOk = true;
        }

        //教導用，使用者按下上一步
        public void SetTeachUserCheckPrevious()
        {
            TeachUserCheckPrevious = true;
        }

        //教導用，使用者按下跳過
        public void SetTeachUserCheckSkip()
        {
            TeachUserCheckSkip = true;
        }

        //+ By Max 20210224, 內靶掃描
        private int iLeftTopCutPos = 0;
        private bool bLTCutPosHasGot = false;
        public int GetLeftTopCutPos()
        {
            iLeftTopCutPos = motor_切刀橫移馬達_Z2X.ReadPos();
            bLTCutPosHasGot = true;
            return iLeftTopCutPos;
        }

        //自動用，使用者按下下一步
        public void SetAutoUserCheckOk()
        {
            AutoUserCheckOk = true;
        }

        //自動用，使用者按下跳過
        public void SetAutoUserCheckSkip()
        {
            AutoUserCheckSkip = true;
        }

        public int GetTeachFlowStep()
        {
            return TeachFlowStep;
        }

        //public string GetTeachMessage()
        //{
        //    return TeachMessage;
        //}
        //v4.0.1.31 讀取警示層級
        public string GetTeachAlarmLevel()
        {
            return TeachAlarmLevel;
        }

        //+ By Max 20210305
        public string GetAutoAlarmLevel()
        {
            return AutoAlarmLevel;
        }

        //v4.0.1.31 讀取警示碼
        public int GetTeachAlarmCode()
        {
            return TeachAlarmCode;
        }

        //+ By Max 20210305
        public int GetAutoAlarmCode()
        {
            return AutoAlarmCode;
        }

        public string[] GetTeachAlarmmsg()
        {
            return TeachMsg;
        }
        public string GetTeachMessageNCT()
        {
            if (TeachMessageNCTZ1 != "" && TeachMessageNCTZ2 != "")
                return TeachMessageNCTZ1 + "/" + TeachMessageNCTZ2;
            else
                return TeachMessageNCTZ1 + TeachMessageNCTZ2;
        }

        public int GetAutoFlowStep(bool bIsCutOffset)
        {
            if (bIsCutOffset)
                return AutoFlowStepForCutOffset;
            else
                return AutoFlowStep;
        }

        public string[] GetAutoMessage()
        {
            return AutoMessage;
        }

        public void ChangeLang(string lang)
        {
            m_strLang = lang;
            //switch (lang)
            //{
            //    case "tw":
            //        dRadioGroupBox1.RadioItems = new List<string> { "不連接", "傳輸控制", "訊號" };


            //        break;
            //    case "en":
            //        dRadioGroupBox1.RadioItems = new List<string> { "NoUse", "TCP/IP", "IO" };
            //        break;
            //}
        }

        public bool GetcheckBeforeStartStatus(int i)
        {
            if (!Enum.IsDefined(typeof(enTeachSel), i)) return false;

            enTeachSel enSel = (enTeachSel)i;

            var _dicLookUp = new Dictionary<enTeachSel, Func<bool>>()
            {
                 {enTeachSel.Z1SparkTest,      () => SReadValue("ByPassZ1接觸測高").ToBoolean()   || Z1CheckBeforeStart.ZSparkTestDone},
                 {enTeachSel.Z1NonContactTest, () => SReadValue("ByPassZ1非接觸測高").ToBoolean() || Z1CheckBeforeStart.ZNCTestDone},
                 {enTeachSel.Z1BaseLineTeach,  () => SReadValue("ByPassZ1基準線校正").ToBoolean() || Z1CheckBeforeStart.ZKerfCheckDone},
                 {enTeachSel.Z2SparkTest,      () => SReadValue("ByPassZ2接觸測高").ToBoolean()   || Z2CheckBeforeStart.ZSparkTestDone},
                 {enTeachSel.Z2NonContactTest, () => SReadValue("ByPassZ2非接觸測高").ToBoolean() || Z2CheckBeforeStart.ZNCTestDone},
                 {enTeachSel.Z2BaseLineTeach,  () => SReadValue("ByPassZ2基準線校正").ToBoolean() || Z2CheckBeforeStart.ZKerfCheckDone},
                 {enTeachSel.CutLineTeach,     () => SReadValue("ByPass切割道學習").ToBoolean()   || m_IsCutLineTeachDone},
                 {enTeachSel.TargetTeach,      () => SReadValue("ByPass靶點學習").ToBoolean()     || m_IsTargetTeachDone}
            };

            if (_dicLookUp.ContainsKey(enSel)) return _dicLookUp[enSel].Invoke();
            else return false;
        }

        #region By Wolf 20210820 v4.0.92.0 非新建料號可忽略靶點學習進行生產

        bool m_bTargetTeachIgnore = false;

        public void SetTargetTeachIgnore(bool bIgnore)
        {
            m_bTargetTeachIgnore = bIgnore;
        }

        public bool GetCheckBeforeStartFinish()
        {
            bool bRet = false;
            bool bZ1ST = SReadValue("ByPassZ1接觸測高").ToBoolean() || Z1CheckBeforeStart.ZSparkTestDone;
            bool bZ1NCT = SReadValue("ByPassZ1非接觸測高").ToBoolean() || Z1CheckBeforeStart.ZNCTestDone;
            bool bZ1KC = SReadValue("ByPassZ1基準線校正").ToBoolean() || Z1CheckBeforeStart.ZKerfCheckDone;
            bool bZ2ST = SReadValue("ByPassZ2接觸測高").ToBoolean() || Z2CheckBeforeStart.ZSparkTestDone;
            bool bZ2NCT = SReadValue("ByPassZ2非接觸測高").ToBoolean() || Z2CheckBeforeStart.ZNCTestDone;
            bool bZ2KC = SReadValue("ByPassZ2基準線校正").ToBoolean() || Z2CheckBeforeStart.ZKerfCheckDone;

            if (bUseZ1_Spindle && bUseZ2_Spindle)
                bRet = bZ1ST && bZ1NCT && bZ1KC && bZ2ST && bZ2NCT && bZ2KC;
            else if (bUseZ1_Spindle && !bUseZ2_Spindle)
                bRet = bZ1ST && bZ1NCT && bZ1KC;
            else if (!bUseZ1_Spindle && bUseZ2_Spindle)
                bRet = bZ2ST && bZ2NCT && bZ2KC;

            return bRet &&
                (SReadValue("ByPass切割道學習").ToBoolean() || m_IsCutLineTeachDone) &&
                (m_bTargetTeachIgnore || SReadValue("ByPass靶點學習").ToBoolean() || m_IsTargetTeachDone);
        }

        #endregion

        public bool GetNeedCheckVaca()
        {
            return m_bIsNeedCheckVaca;
        }

        public bool GetInBit(InBit IB)
        {
            if (IsSimulation() != 0)
                return true;

            if (!bUseZ1_Spindle)
            {
                if (IB.Name == "inBit_Z1刀輪護蓋開啟" || IB.Name == "inBit_Z1刀輪護蓋關閉")
                {
                    return true;
                }
            }

            if (!bUseZ2_Spindle)
            {
                if (IB.Name == "inBit_Z2刀輪護蓋開啟" || IB.Name == "inBit_Z2刀輪護蓋關閉")
                {
                    return true;
                }
            }
            //Woody v4.0.1.32 冰水機可PASS訊號
            if (SReadValue("bIgnoreCoolingMachine").ToBoolean())
            {
                if (IB.Name == "inBit_冰水機正常運轉中")
                {
                    return true;
                }
            }

            return IB.Value;
        }

        private void SetSpeed_U(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;

            string strTmp = enMode.ToString().Substring(0, 2);
            int Spd = (int)(MachineRate * SReadValue(string.Format("{0}速度_θ", strTmp)).ToInt());
            int Acc = (int)(MachineRate * SReadValue(string.Format("{0}加速度_θ", strTmp)).ToInt());
            int Dcc = (int)(MachineRate * SReadValue(string.Format("{0}減速度_θ", strTmp)).ToInt());

            motor_切割平台旋轉馬達U.SetSpeed(Spd);
            motor_切割平台旋轉馬達U.SetAcceleration(Acc);
            motor_切割平台旋轉馬達U.SetDeceleration(Dcc);
        }

        private void SetSpeed_Y(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;
            int Spd = 0;
            int Acc = 0;
            int Dcc = 0;

            switch (enMode)
            {
                case enSpeedMode.掃瞄速度:

                    Spd = (int)(MachineRate * SReadValue("掃瞄速度_Y").ToInt());
                    Acc = (int)(MachineRate * SReadValue("掃瞄加速度_Y").ToInt());
                    Dcc = (int)(MachineRate * SReadValue("掃瞄減速度_Y").ToInt());
                    motor_切割平台前後馬達Y.SetSpeed(Spd);
                    motor_切割平台前後馬達Y.SetAcceleration(Acc);
                    motor_切割平台前後馬達Y.SetDeceleration(Dcc);

                    break;

                case enSpeedMode.切割後速度:

                    Spd = (int)(MachineRate * SReadValue("切割後速度_Y").ToInt());
                    Acc = (int)(MachineRate * SReadValue("切割後加速度_Y").ToInt());
                    Dcc = (int)(MachineRate * SReadValue("切割後減速度_Y").ToInt());
                    motor_切割平台前後馬達Y.SetSpeed(Spd);
                    motor_切割平台前後馬達Y.SetAcceleration(Acc);
                    motor_切割平台前後馬達Y.SetDeceleration(Dcc);

                    break;

                case enSpeedMode.切割速度:

                    int nCutSpeed = Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_速度"].ToString());
                    int nEnterSpeed = nCutSpeed;

                    string strTmp = AutoCut.CutData.Rows[AutoCut.Step]["Cut_入板速度"].ToString();
                    if (strTmp.Length != 0) nEnterSpeed = Convert.ToInt32(strTmp);

                    int nYCutShift = SReadValue("YCutShift").ToInt() / 1000;

                    double dDis = (double)nCutSpeed / (double)nEnterSpeed * nYCutShift;
                    Acc = (int)((double)(nCutSpeed * nCutSpeed) / (2 * dDis));
                    Acc *= 1000;

                    nCutSpeed *= 1000;

                    if (m_bIsVisionSaw)//切割速度設定
                    {
                        nCutSpeed /= 2;
                    }

                    Spd = nCutSpeed;
                    //Acc = nCutSpeed * 5;
                    Dcc = nCutSpeed * 5;
                    motor_切割平台前後馬達Y.SetSpeed(Spd);
                    motor_切割平台前後馬達Y.SetAcceleration(Acc);
                    motor_切割平台前後馬達Y.SetDeceleration(Dcc);

                    break;

                case enSpeedMode.刀痕速度:

                    //Spd = PReadValue("Cut_速度").ToInt();
                    Spd = 50000;
                    Acc = Spd * 5;
                    Dcc = Spd * 5;
                    motor_切割平台前後馬達Y.SetSpeed(Spd);
                    motor_切割平台前後馬達Y.SetAcceleration(Acc);
                    motor_切割平台前後馬達Y.SetDeceleration(Dcc);

                    break;
            }
        }

        private void SetSpeed_Z1_Z(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;

            string strTmp = enMode == enSpeedMode.掃瞄速度 ? "CCD移動" : "Z1切割";
            int Spd = (int)(MachineRate * SReadValue(string.Format("{0}速度_Z", strTmp)).ToInt());
            int Acc = (int)(MachineRate * SReadValue(string.Format("{0}加速度_Z", strTmp)).ToInt());
            int Dcc = (int)(MachineRate * SReadValue(string.Format("{0}減速度_Z", strTmp)).ToInt());

            motor_切刀上下馬達_Z1.SetSpeed(Spd);
            motor_切刀上下馬達_Z1.SetAcceleration(Acc);
            motor_切刀上下馬達_Z1.SetDeceleration(Dcc);
        }

        private void SetSpeed_Z1_X(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;

            string strTmp = enMode == enSpeedMode.掃瞄速度 ? "CCD移動" : "Z1切割";
            int Spd = (int)(MachineRate * SReadValue(string.Format("{0}速度_X", strTmp)).ToInt());
            int Acc = (int)(MachineRate * SReadValue(string.Format("{0}加速度_X", strTmp)).ToInt());
            int Dcc = (int)(MachineRate * SReadValue(string.Format("{0}減速度_X", strTmp)).ToInt());

            motor_切刀橫移馬達_Z1X.SetSpeed(Spd);
            motor_切刀橫移馬達_Z1X.SetAcceleration(Acc);
            motor_切刀橫移馬達_Z1X.SetDeceleration(Dcc);
        }

        private void SetSpeed_Z2_Z(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;

            string strTmp = enMode == enSpeedMode.掃瞄速度 ? "CCD移動" : "Z2切割";
            int Spd = (int)(MachineRate * SReadValue(string.Format("{0}速度_Z", strTmp)).ToInt());
            int Acc = (int)(MachineRate * SReadValue(string.Format("{0}加速度_Z", strTmp)).ToInt());
            int Dcc = (int)(MachineRate * SReadValue(string.Format("{0}減速度_Z", strTmp)).ToInt());

            motor_切刀上下馬達_Z2.SetSpeed(Spd);
            motor_切刀上下馬達_Z2.SetAcceleration(Acc);
            motor_切刀上下馬達_Z2.SetDeceleration(Dcc);
        }

        private void SetSpeed_Z2_X(enSpeedMode enMode)
        {
            double MachineRate = OReadValue("機台速率").ToDouble() * 0.01;

            string strTmp = enMode == enSpeedMode.掃瞄速度 ? "CCD移動" : "Z2切割";
            int Spd = (int)(MachineRate * SReadValue(string.Format("{0}速度_X", strTmp)).ToInt());
            int Acc = (int)(MachineRate * SReadValue(string.Format("{0}加速度_X", strTmp)).ToInt());
            int Dcc = (int)(MachineRate * SReadValue(string.Format("{0}減速度_X", strTmp)).ToInt());

            motor_切刀橫移馬達_Z2X.SetSpeed(Spd);
            motor_切刀橫移馬達_Z2X.SetAcceleration(Acc);
            motor_切刀橫移馬達_Z2X.SetDeceleration(Dcc);
        }

        public int Get_圓心X()
        {
            return SReadValue("圓心點位_X").ToInt();
        }

        public int Get_圓心Y()
        {
            return SReadValue("圓心點位_Y").ToInt();
        }

        public int Get非接觸測高後計算磨耗值(string i)
        {
            if (Z1CheckBeforeStart.ZNCTestSecondDone)
                return PReadValue("BladeZ1Data", "iSimMeasureZPos" + i).ToInt() - SReadValue(i + "非接觸測高位置_Z").ToInt();
            else
                return 0;
        }

        public int Get接觸測高後計算磨耗值(string i)
        {
            ////取得刀外徑
            //double _dKnifeDiameter = PReadValue("ToolDataZ2", "dKnifeDiameter").ToDouble();
            //int _iKnifeDiameter = Convert.ToInt32(_dKnifeDiameter * 1000d);

            ////取得法蘭外徑
            //double _dFlange = PReadValue("ToolDataZ2", "dFlange").ToDouble();
            //int _iFlange = Convert.ToInt32(_dFlange * 1000d);

            //刀片極限處碰到測高塊位置
            int iBaseH = SReadValue(i + "FranTeachBlockPos_Z").ToInt();

            //+刀露量規格
            int iPosTmp = iBaseH + PReadValue("Blade" + i + "Data", "dRealKnifeRemain" + i).ToInt() * 1000;

            int i磨耗值 = iPosTmp - SReadValue("切割時" + i + "切刀切割道底部位置_Z").ToInt();

            return i磨耗值;
        }

        //By Max 20170914
        public int MCType
        {
            get;
            set;
        }

        //讀取歸零流程
        public int GetHomeFlow()
        {
            return m_nHomeFlow;
        }

        //讀取馬達點位
        public int GetMotorPos(string motor)
        {
            switch (motor)
            {
                case "X1":
                    return motor_切刀橫移馬達_Z1X.ReadPos();
                case "X2":
                    return motor_切刀橫移馬達_Z2X.ReadPos();
                case "Z1":
                    return motor_切刀上下馬達_Z1.ReadPos();
                case "Z2":
                    return motor_切刀上下馬達_Z2.ReadPos();
                case "Y":
                    return motor_切割平台前後馬達Y.ReadPos();
                case "U":
                    return motor_切割平台旋轉馬達U.ReadPos();
            }
            return 0;
        }

        public bool IsMotorIdle()
        {
            if (motor_切刀橫移馬達_Z1X.IsBusy ||
               motor_切刀橫移馬達_Z2X.IsBusy ||
               motor_切刀上下馬達_Z1.IsBusy ||
               motor_切刀上下馬達_Z2.IsBusy ||
               motor_切割平台前後馬達Y.IsBusy ||
               motor_切割平台旋轉馬達U.IsBusy)
                return false;
            else
                return true;

        }

        #region 刀具管理 //ted modify 20170411
        strKnifeData Tool1_InfoReturn = new strKnifeData();
        strKnifeData Tool2_InfoReturn = new strKnifeData();

        public void BladeRefresh()//v4.0.1.17
        {
            //Z1
            Tool1_InfoReturn.sToolName = PReadValue("BladeZ1Data", "sToolNameZ1").ToString();
            Tool1_InfoReturn.sType = PReadValue("BladeZ1Data", "sTypeZ1").ToString();
            Tool1_InfoReturn.dWidth = PReadValue("BladeZ1Data", "dWidthZ1").ToDouble();
            Tool1_InfoReturn.dKnifeDiameter = PReadValue("BladeZ1Data", "dKnifeDiameterZ1").ToDouble();
            Tool1_InfoReturn.dRealKnifeRemain = PReadValue("BladeZ1Data", "dRealKnifeRemainZ1").ToDouble() * 1000;
            Tool1_InfoReturn.dFlange = PReadValue("BladeZ1Data", "dFlangeZ1").ToDouble();
            Tool1_InfoReturn.dLimit = PReadValue("BladeZ1Data", "dLimitZ1").ToDouble();
            Tool1_InfoReturn.dRealMotorDistance = PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble();
            //Z2
            Tool2_InfoReturn.sToolName = PReadValue("BladeZ2Data", "sToolNameZ2").ToString();
            Tool2_InfoReturn.sType = PReadValue("BladeZ2Data", "sTypeZ2").ToString();
            Tool2_InfoReturn.dWidth = PReadValue("BladeZ2Data", "dWidthZ2").ToDouble();
            Tool2_InfoReturn.dKnifeDiameter = PReadValue("BladeZ2Data", "dKnifeDiameterZ2").ToDouble();
            Tool2_InfoReturn.dRealKnifeRemain = PReadValue("BladeZ2Data", "dRealKnifeRemainZ2").ToDouble() * 1000;
            Tool2_InfoReturn.dFlange = PReadValue("BladeZ2Data", "dFlangeZ2").ToDouble();
            Tool2_InfoReturn.dLimit = PReadValue("BladeZ2Data", "dLimitZ2").ToDouble();
            Tool2_InfoReturn.dRealMotorDistance = PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble();
        }

        public strKnifeData Tool1InfoGet()
        {
            return Tool1_InfoReturn;
        }

        public strKnifeData Tool2InfoGet()
        {
            return Tool2_InfoReturn;
        }

        public double GetCutBreakThreshold()
        {
            return IsUseDAQ ? 2.5 : SReadValue("dBreakVolThreshold").ToDouble();
        }
        #endregion 刀具管理

        #region 閥體相關介面函式

        #region 取得閥體數據

        public double GetSpindleAirPressureAIValue()
        {
            double gap = (SReadValue("Spindle_Volt_PressOn").ToDouble() - SReadValue("Spindle_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Spindle_KPa_PressOn").ToDouble() / gap;
            return (analogIn_主軸空壓值.Value - SReadValue("Spindle_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetWaterInPressAIValue()
        {
            double gap = (SReadValue("WaterPress_Volt_PressOn").ToDouble() - SReadValue("WaterPress_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("WaterPress_KPa_PressOn").ToDouble() / gap;
            return (analogIn_純水水壓值.Value - SReadValue("WaterPress_Volt_PressOff").ToDouble()) * rate;
        }

        public bool GetVacaStatus()
        {
            double limit = PReadValue("PCBVacuumThreshold").ToDouble();
            bool table1 = GetChuckTableAIValue() < limit;
            return table1;
        }

        public double GetChuckTableAIValue()
        {
            double gap = (SReadValue("Chuck_1_Volt_PressOn").ToDouble() - SReadValue("Chuck_1_Volt_PressOff").ToDouble());
            if (gap == 0) return 0;

            double rate = SReadValue("Chuck_1_KPa_PressOn").ToDouble() / gap;
            return (analogIn_切割平台真空值.Value - SReadValue("Chuck_1_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetZ1BladeCoolerAIValue(bool bIsReal)
        {
            double gap = (SReadValue("Z1_Blade_Cooler_Volt_PressOn").ToDouble() - SReadValue("Z1_Blade_Cooler_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Z1_Blade_Cooler_KPa_PressOn").ToDouble() / gap;

            if (bIsReal)
                return (analogIn_Z1噴水座流量.Value - SReadValue("Z1_Blade_Cooler_Volt_PressOff").ToDouble()) * rate;
            else
                return (analogIn_Z1噴水座流量.AvgValue - SReadValue("Z1_Blade_Cooler_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetZ2BladeCoolerAIValue(bool bIsReal)
        {
            double gap = (SReadValue("Z2_Blade_Cooler_Volt_PressOn").ToDouble() - SReadValue("Z2_Blade_Cooler_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Z2_Blade_Cooler_KPa_PressOn").ToDouble() / gap;

            if (bIsReal)
                return (analogIn_Z2噴水座流量.Value - SReadValue("Z2_Blade_Cooler_Volt_PressOff").ToDouble()) * rate;
            else
                return (analogIn_Z2噴水座流量.AvgValue - SReadValue("Z2_Blade_Cooler_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetZ1BladeSprayAIValue(bool bIsReal)
        {
            double gap = (SReadValue("Z1_Spray_Volt_PressOn").ToDouble() - SReadValue("Z1_Spray_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Z1_Spray_KPa_PressOn").ToDouble() / gap;

            if (bIsReal)
                return (analogIn_Z1灑水座流量.Value - SReadValue("Z1_Spray_Volt_PressOff").ToDouble()) * rate;
            else
                return (analogIn_Z1灑水座流量.AvgValue - SReadValue("Z1_Spray_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetZ2BladeSprayAIValue(bool bIsReal)
        {
            double gap = (SReadValue("Z2_Spray_Volt_PressOn").ToDouble() - SReadValue("Z2_Spray_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Z2_Spray_KPa_PressOn").ToDouble() / gap;

            if (bIsReal)
                return (analogIn_Z2灑水座流量.Value - SReadValue("Z2_Spray_Volt_PressOff").ToDouble()) * rate;
            else
                return (analogIn_Z2灑水座流量.AvgValue - SReadValue("Z2_Spray_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetWaterPressureLimit(int nSel)
        {
            string str = (enLimitSel)nSel == enLimitSel.MIN ? "Water_Pressure_Min" : "Water_Pressure_Max";
            return SReadValue(str).ToDouble();
        }

        public double GetSpindleAirPressureLimit(int nSel)
        {
            string str = (enLimitSel)nSel == enLimitSel.MIN ? "Spindle_AirPressure_Min" : "Spindle_AirPressure_Max";
            return SReadValue(str).ToDouble();
        }

        public double GetWaterFlowTolerance()
        {
            return SReadValue("Water_Flow_Tolerance").ToDouble();
        }

        public double GetWaterFlowMax(enCutWaterSel enSel)
        {
            return enSel == enCutWaterSel.SHOWER ? SReadValue("Max_Blade_Flow").ToDouble() : SReadValue("Max_Spray_Flow").ToDouble();
        }

        #endregion

        //v0.0.7.12 By Sanxiu 主畫面顯示即時進給速度
        public double GetNowFeedSpeed()
        {
            return (double)(motor_切割平台前後馬達Y.ReadSpeed() / 1000.0);
        }

        //v4.0.1.30 主畫面顯示入料片數
        public int GetPCBIn()
        {
            return PCBIncount;
        }

        //v4.0.1.30 主畫面顯示入料片數
        public int GetPCBOut()
        {
            return PCBOutcount;
        }

        #region By Wolf 20210820 v4.0.92.0 增加片數歸零鍵
        public void SetPCBIn(int nPCBIncount)
        {
            PCBIncount = nPCBIncount;
        }

        public void SetPCBOut(int nPCBOutcount)
        {
            PCBOutcount = nPCBOutcount;
        }
        #endregion

        //v4.0.1.41 關程式時關閉主軸
        public void StopSpindle(string spindle)
        {
            if (spindle == "Z1")
                Z1SpindleStop = true;
            if (spindle == "Z2")
                Z2SpindleStop = true;
        }
        //v4.0.1.41 關程式時關閉附屬設備
        public void CloseAllAncillary()
        {
            SetOutBit(outBit_高倍CCD鏡頭清潔, false);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_冰水機啟動, false);
            SetOutBit(outBit_真空幫浦啟動, false);
            //+ By Max 20210416, v4.0.1.62, 因應分開成兩個OutBit，啟動與運轉同時控制
            SetOutBit(outBit_真空幫浦運轉, false);
            SetOutBit(outBit_鼓風機啟動, false);
            SetOutBit(outBit_純水加壓幫浦啟動, false);

            toolDataCollect1.Stop();
            toolDataCollect1.Dispose();
        }
        #endregion

        #region 硬體訊號
        //0.0.7.11 By Sanxiu 修正介面按鈕顯示
        public bool GetCTTAirVmc()
        {
            return outBit_切割平台真空建立.Value;
        }

        //0.0.7.11 By Sanxiu 修正介面按鈕顯示
        public bool GetCTTDestroy()
        {
            return outBit_切割平台真空破壞.Value;
        }

        //0.0.7.11 By Sanxiu 畫面顯示主軸轉速
        public double GetSpindleZ1Speed()
        {
            double d = analogIn_Z1主軸轉速.AvgValue * 6000d;
            return d;
        }

        //0.0.7.11 By Sanxiu 畫面顯示主軸轉速
        public double GetSpindleZ2Speed()
        {
            double d = analogIn_Z2主軸轉速.AvgValue * 6000d;

            return d;
        }

        //v0.0.7.12 By Sanxiu 切割中紀錄主軸負載、轉速
        public void WriteSpindleReadData()
        {
            //if (bdynamicCutOffset == false)   //執行動態刀痕時不記錄
            //{
            //    string sDataStr = "";

            //    if (bUseZ1_Spindle)
            //    {
            //        //sDataStr += "Z1," + string.Format("{0:0.000}", analogIn_Z1主軸轉速.AvgValue * 6000) + "," + string.Format("{0:0.000}", analogIn_Z1負載電流.AvgValue) + ",";
            //        sDataStr += "Z1," + string.Format("{0:0.000}", GetSpindleZ1Speed()) + "," + string.Format("{0:0.000}", analogIn_Z1負載電流.AvgValue) + ",";
            //    }

            //    if (bUseZ2_Spindle)
            //    {
            //        //sDataStr += "Z2," + string.Format("{0:0.000}", analogIn_Z2主軸轉速.AvgValue * 6000) + "," + string.Format("{0:0.000}", analogIn_Z2負載電流.AvgValue) + ",";
            //        sDataStr += "Z2," + string.Format("{0:0.000}", GetSpindleZ2Speed()) + "," + string.Format("{0:0.000}", analogIn_Z2負載電流.AvgValue) + ",";
            //    }
            //    sDataStr += "i切割_Step," + i切割_Step.ToString();
            //    LogSay(EnLoggerType.EnLog_SPC, sDataStr);
            //}
        }

        #endregion

        #region 主畫面-閥體數據
        //v0.0.7.18 By Sanxiu 非接觸測高前增加清洗和檢測電壓流程
        public float GetAnalogIn_NC_AvgValue(string strSpindleSel)
        {
            return strSpindleSel == "Z1" ? analogIn_Z1非接觸測高值.AvgValue : analogIn_Z2非接觸測高值.AvgValue;
        }

        //v0.0.7.9 By Sanxiu 主頁面破刀數值
        public float GetAnalogIn_AvgValue(string strSpindleSel)
        {
            if (IsUseDAQ)
            {
                return strSpindleSel == "Z1" ? (float)td_Z1破片檢知.Value : (float)td_Z2破片檢知.Value;
            }
            else
            {
                return strSpindleSel == "Z1" ? analogIn_Z1破片檢知.AvgValue : analogIn_Z2破片檢知.AvgValue;
            }
        }

        //v0.0.7.16 By Sanxiu 修正畫面卡住
        public float GetAnalogIn_Z1_avgValueBar()
        {
            //+ By Max 20211030
            if (IsUseDAQ)
            {
                return (float)td_Z1破片檢知.Percentage;
            }
            else
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                    return analogIn_Z1破片檢知.AvgValue;
                else
                    return 0.0f;
            }
        }

        public float GetAnalogIn_Z2_avgValueBar()
        {
            //if (IsSimulation() != 0)
            //{
            //    Random ran = new Random();
            //    float fa = (float)ran.NextDouble();
            //    fa = fa / 10;
            //    return (fa + 1.1f);
            //}

            //+ By Max 20211030
            if (IsUseDAQ)
            {
                return (float)td_Z2破片檢知.Percentage;
            }
            else
            {
                if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                    return analogIn_Z2破片檢知.AvgValue;
                else
                    return 0.0f;
            }
        }

        public float GetAnalogInLoadValue(string strSpindleSel)
        {
            return strSpindleSel == "Z1" ? analogIn_Z1負載電流.AvgValue : analogIn_Z2負載電流.AvgValue;
        }
        #endregion

        public bool SetAutoActionNo(int i)
        {
            if (Auto_ActionNo == 0)
            {
                Auto_ActionNo = i;
                Auto_ActionResult = 0;
                //+ By Max 20210305
                AutoMessage = new string[] { "" };
                AutoAlarmLevel = "";
                AutoAlarmCode = 0;
                return true;
            }
            else
                return false;
        }

        public int GetAutoActionNo()
        {
            return Auto_ActionNo;
        }

        public bool SetTeachActionNo(int i)
        {
            if (m_enTeachSel == enTeachActiionSel.無動作)
            {
                m_enTeachSel = (enTeachActiionSel)i;
                TeachStatus = 0;
                Teach_ActionResult = 0;
                TeachAlarmLevel = "";
                TeachAlarmCode = 0;
                //TeachMessage = "";
                TeachFlowStep = 0;
                TeachUserCheckOk = false;
                return true;
            }
            else
                return false;
        }

        public int GetTeachActionNo()
        {
            return (int)m_enTeachSel;
        }

        //v0.0.7.11 By Sanxiu 修正Package資料更新後的MainForm異常
        /// <summary>
        /// 點下資料更新後重載表單資料
        /// </summary>
        public void UpAlldate()
        {
            Load_CutData();
            Load_CleanData();
            Load_FixtureData();
            Load_PCBData();
        }

        /// <summary>
        /// 判斷橫移軸是否會在不安全的距離
        /// </summary>
        /// <returns>true：代表位置不安全；false：代表位置安全</returns>
        int iXMotorLinearMode = 1;
        public bool 切割橫移X軸不安全()
        {
            if (iXMotorLinearMode == 1)   //v0.5.7.28 By Sanxiu 馬達X軸加入絕對型光學尺功能
            {
                return ((motor_切刀橫移馬達_Z2X.ReadRealPos() - motor_切刀橫移馬達_Z1X.ReadRealPos())) > SReadValue("X_Motor_SafeDis").ToInt();
            }
            else
            {
                return ((motor_切刀橫移馬達_Z1X.ReadRealPos() - motor_切刀橫移馬達_Z2X.ReadRealPos())) < SReadValue("X_Motor_SafeDis").ToInt();
            }
        }

        //主畫面開關真空
        public void VaccumStateChange(bool b)
        {
            SetOutBit(outBit_切割平台真空建立, b);
        }

        //主畫面開關破真空
        public void DestroyStateChange(bool b)
        {
            SetOutBit(outBit_切割平台真空破壞, b);
        }

        //教導-接觸測高選擇主軸
        public void SparkSelect(String strSel)
        {
            if (m_SparkTestSpindle == enSpindleSel.NONE)
            {
                m_SparkTestSpindle = strSel == "Z1" ? enSpindleSel.Z1 : enSpindleSel.Z2;
            }
        }

        //教導-非接觸測高選擇主軸
        public void NCTSelect(String selectspindle, bool b)
        {
            if (selectspindle == "Z1")
                m_bIsTeachNeedNContactTestZ1 = b;
            if (selectspindle == "Z2")
                m_bIsTeachNeedNContactTestZ2 = b;
        }

        //教導-基準線校正選擇主軸
        public void BaseLineSelect(string strSel)
        {
            if (m_BaseLineSpindle == enSpindleSel.NONE)
            {
                m_BaseLineSpindle = strSel == "Z1" ? enSpindleSel.Z1 : enSpindleSel.Z2;
            }
        }

        //教導-換刀流程重設刀子資訊
        public void ResetBladeInfo(string blade)
        {
            if (blade == "Z1")
                Z1CheckBeforeStart.Reset();
            if (blade == "Z2")
                Z2CheckBeforeStart.Reset();
        }

        public TabControl GetFlowView()
        {
            return TabFlow;
        }

        //設定下刀點確認資訊
        public void SetTargetCheck(string Side, int LineNo, string Loc)
        {
            TC_Side = Side;
            TC_LineNo = LineNo;
            TC_Loc = Loc;
        }
        //設定下刀點確認完成
        public void SetTargetCheckFinish()
        {
            TargetCheckStart = false;
            TargetCheckFinish = true;
        }
        //設定下刀點確認開始
        public bool GetTargetCheckStart()
        {
            return TargetCheckStart;
        }

        //設定手動定靶
        public void SetManualAlignmentFinish()
        {
            ManualAlignment = true;
        }

        //開啟動態刀痕
        public void SetdynamicCutFunc(bool bSet)
        {
            bdynamicCutOffset = bSet;
        }
        //動態刀痕進行中
        public bool iSdynamicCutRun()
        {
            return bdynamicCutOffset_run;
        }
        //動態刀痕進行中
        public void SetdynamicCutStop()
        {
            bdynamicCutOffset_run = false;
            //+ By Max 20210209
            bdynamicCutOffsetFinish = true;
        }

        //+ By Max 20210309, 是否顯示動態刀痕CheckBox
        public bool CanShowDynamicCutOffsetCheckBox()
        {
            return bCanShowDynamicCutOffset;
        }

        //強排用
        public void SetAutoFlowChartEnd(bool bIsForceOutContinue)
        {
            m_AutoFlowChartEndEvt.Set();
            ResetAllFlag();
            PCBIncount = PCBOutcount;

            if (bIsForceOutContinue) m_ForceOutContinueEvt.Set();
            else SendData("ForceOut");
        }

        public int GetTeachStatus()
        {
            return TeachStatus;
        }

        public int GetTeachStatusNCTZ1()
        {
            return TeachStatusNCTZ1;
        }

        public int GetTeachStatusNCTZ2()
        {
            return TeachStatusNCTZ2;
        }

        //教導流程暫停用
        public void SetTeachFlowChartEnd()
        {
            bSetTeachFlowChartEnd = true;
        }

        public void ChangeSparkTestBlock()
        {
            SSetValue("Z2_Spark_X_Next", SReadValue("Z2_Spark_X_Start").ToInt());
            SSetValue("Spark_Y_Next", SReadValue("Spark_Y_Start").ToInt());
            SaveFile();
        }

        public bool GetPauseNeedToContinue()
        {
            return m_bIsPauseNeedToContinue;
        }

        public int GetAutoActionResult()
        {
            return Auto_ActionResult;
        }

        public int GetTeachActionResult()
        {
            return Teach_ActionResult;
        }

        public string Get_CutLineSide()
        {
            return s切割_NowSide;
        }

        public int Get_Z1LineNo()
        {
            return i切割_Z1_NowLineNo;
        }

        public int Get_Z2LineNo()
        {
            return i切割_Z2_NowLineNo;
        }

        public int Get_CutLine百分比()
        {
            return i切割_Now百分比;
        }

        public DataView GetKerfCheckView()
        {
            DataView dv = new DataView(KerfCheckView);
            return dv;
        }

        public void SetTeachTimer(bool bset)
        {
            if(!SysRun)
                Teach_timer.Enabled = bset;
        }

        public bool GetTeachTimer()
        {
            return Teach_timer.Enabled;
        }

        //4.0.1.24讀取目前角度
        //+ By Max 20210316, v4.0.1.55, 新增手動定靶時Pitch移動取得方向使用
        public string GetSide()
        {
            string side = "";
            if (m_enTeachSel != enTeachActiionSel.無動作)
            {
                switch (m_enTeachSel)
                {
                    case enTeachActiionSel.基準線校正:
                    case enTeachActiionSel.圓心校正:
                        side = "CH2";
                        break;
                    case enTeachActiionSel.切割道學習:
                        side = TeachFixture.strSide;
                        break;
                    case enTeachActiionSel.靶點學習:
                        side = LearnMarkPos.strSide;
                        break;
                }
            }
            else //手動定靶時Pitch移動取得方向使用
            {
                side = AutoMark.strSide;
            }
            return side;
        }

        //v0.0.7.11 By Sanxiu 異常排除流程步驟記錄
        public void SaveAllSawStepIndex()
        {
            string str = "";
            str = "歸零流程:" + FC_SAW歸零_動作開始.NowTask.ToString() +
                ",自動(執行/接收):" + FC_SAW自動_動作開始.NowTask.ToString() +
                ",自動流程:" + FC_自動_開始.NowTask.ToString() +
                ",清洗流程:" + FC_清洗_動作開始.NowTask.ToString() +
                ",四點掃靶:" + FC_四點掃靶_掃靶開始.NowTask.ToString() +
                ",全靶(N):" + FC_全靶_N字形_掃靶開始.NowTask.ToString() +
                ",三點掃靶:" + FC_四點掃靶_掃靶開始.NowTask.ToString() +
                ",下刀點確認:" + FC_下刀點確認_開始.NowTask.ToString() +
                ",動態刀痕:" + FC_動態刀痕_開始.NowTask.ToString() +
                ",刀痕檢測:" + FC_刀痕檢測_開始.NowTask.ToString() +
                ",教導/執行:" + FC_教導執行_動作開始.NowTask.ToString() +
                ",接觸測高:" + FC_接觸測高_動作開始.NowTask.ToString() +
                ",Z1非接觸測高:" + FC_Z1非接觸測高_動作開始.NowTask.ToString() +
                ",Z2非接觸測高:" + FC_Z2非接觸測高_動作開始.NowTask.ToString() +
                ",基準線校正:" + FC_基準線校正_動作開始.NowTask.ToString() +
                ",切割道學習:" + FC_切割道學習_動作開始.NowTask.ToString() +
                ",靶點學習:" + FC_靶點學習_動作開始.NowTask.ToString() +
                ",圓心校正:" + FC_圓心校正_開始.NowTask.ToString() +
                ",換刀流程:" + FC_換刀流程_動作開始.NowTask.ToString() +
                //",自動對焦:" + FC_教導_自動對焦_開始.NowTask.ToString() +
                ",自動對焦:" + (OReadValue("bFastAutoFocus").ToBoolean() ? FC_教導_快速自動對焦_開始.NowTask.ToString() : FC_教導_自動對焦_開始.NowTask.ToString()) +
                ",Z1主軸啟動關閉:" + FC_Z1主軸控制_動作開始.NowTask.ToString() +
                ",Z2主軸啟動關閉:" + FC_Z2主軸控制_動作開始.NowTask.ToString() +
                ",視覺通訊:" + FC_Vision.NowTask.ToString() +
                ",初始化:" + FC_初始化_開始.NowTask.ToString();
            LogSay(EnLoggerType.EnLog_SPC, str);
        }

        public bool GetSpark()
        {
            return inBit_接觸測高導通訊號.Value;
        }

        public bool SetAutoFocus(string s)
        {
            if (CallAutoFocus == "")
            {
                CallAutoFocus = s;
                return true;
            }
            else
            {
                return false;
            }
        }

        //4.0.0.37 破刀後處理
        public bool GetShowChangeBladeContinue()
        {
            return bshow_ChangeBladeContinue;
        }
        public bool GetShowSingleBladeContinue()
        {
            return bshow_SingleBladeContinue;
        }
        public void SetChangeBladeContinue()
        {
            //bshow_ChangeBladeContinue = false;
            bset_ChangeBladeContinue = true;
        }
        public void SetSingleBladeContinue()
        {
            //bshow_SingleBladeContinue = false;
            bset_SingleBladeContinue = true;
        }

        // 確認掃靶是否進行中
        public bool IsTargetScanning()
        {
            return m_bIsTargetScanning;
        }

        #endregion 公用函數

        #region Vision通訊流程
        private FlowChart.FCRESULT FC_是否傳送指令_Run()
        {
            if ((mSendCCDAction == CCDSendResult.Wait) && (CCDCmd != string.Empty))
            {
                mCMS.WMsgFullSendCmd();
                mSendCCDAction = CCDSendResult.Working;
                Array.Clear(CCDResult, 0, CCDResult.Length);
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_傳送指令等待結果_Run()
        {
            object[] oResult = null;
            if (IsSimulation() != 0)
            {
                oResult = new string[] { "A=0", "B=0", "C=0", "D=0", "E=0", "F=0", "G=0", "H=0", "I=0", "J=0", "K=0", "L=0", "M=80" };
                Array.Copy(oResult, CCDResult, oResult.Length);
                mCCDAction = CCDSendResult.OK;
                Timer_VisionCom.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            int iResult = mCMS.WMsgFullSendCmd(RealCCDCmd, out oResult);

            switch (iResult)
            {
                case 2://Working
                    break;

                default:
                    if (iResult == 1)
                        mCCDAction = CCDSendResult.OK;
                    else if (iResult == 3)
                        mCCDAction = CCDSendResult.InspectFail;
                    else
                        mCCDAction = CCDSendResult.NG;
                  
                    Array.Resize(ref CCDResult, oResult.Length);
                    Array.Copy(oResult, CCDResult, oResult.Length);

                    if (mCCDAction != CCDSendResult.OK)
                    {
                        if (CCDResult.Length != 0)
                        {
                            CCDResult[0] = string.Join(",", CCDResult);
                            CCDResult[0] = "[" + CCDResult[0] + "]";
                        }
                    }

                    Timer_VisionCom.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_VisionDelay_Run()
        {
            if (Timer_VisionCom.On(10))
                return FlowChart.FCRESULT.NEXT;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_VisionReset_Run()
        {
            CCDCmd = string.Empty;
            mSendCCDAction = CCDSendResult.Wait;
            return FlowChart.FCRESULT.NEXT;
        }
        #endregion Vision通訊流程

        #region Vision
        private FlowChart.FCRESULT OpenVisionLive(MyTimer timer)
        {
            if (SetCCDCmd("HMLive", "1"))
            {
                timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (timer.On(SReadValue("VisionComOverTime").ToInt()))
                {
                    ShowAlarm("w", 26);
                    timer.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT PatternMatch(MyTimer timer)
        {
            // Pattern Matc, 片數、位置索引
            //+ By Max, 20210804
            bool bRet = false;
            if (PReadValue("IgnoreFindMark").ToBoolean())
            {
                bRet = SetCCDCmd("HMFMNGMP", "0,0,1,0");
            }
            else
            {
                bRet = SetCCDCmd("HMFM", "0,0");
            }

            if (bRet)
            {
                timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (timer.On(SReadValue("VisionComOverTime").ToInt()))
                {
                    ShowAlarm("w", 27);
                    timer.Restart();
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT SelectPattern(MyTimer timer)
        {
            //MarkType:0為十字,1為三角形,2為圓形,3為矩形
            string para = AutoMark.strSide + "_TargetType";
            para = PReadValue(para).ToString();

            if (SetCCDCmd("HMSMT", para))
            {
                timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (timer.On(SReadValue("VisionComOverTime").ToInt()))
                {
                    ShowAlarm("w", 36);
                    timer.Restart();
                }
            }

            return default(FlowChart.FCRESULT);
        }
        #endregion

        private FlowChart.FCRESULT FC_初始化_開始_Run()
        {
            m_SparkTestSpindle = enSpindleSel.NONE;
            m_bIsTeachNeedNContactTestZ1 = false;
            m_bIsTeachNeedNContactTestZ2 = false;
            m_BaseLineSpindle = enSpindleSel.NONE;

            //+ By Max 20210309, 是否顯示動態刀痕CheckBox
            bCanShowDynamicCutOffset = false;

            Timer_ActionIni.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_初始化_速度設定_Run()
        {
            SetSpeed_U(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);

            Timer_ActionIni.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_初始化_測高訊號開關_Run()
        {
            if (GetSparkOff())//動作中止
            {
                Timer_ActionIni.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_ActionIni.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
                Timer_ActionIni.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_初始化_Z軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_ActionIni)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_初始化_IO初始_Run()
        {
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_Z1刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);

            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_Z2刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);

            SetOutBit(outBit_非接觸測高清潔氣旋, false);
            SetOutBit(outBit_非接觸測高清潔水柱, false);

            SetOutBit(outBit_接觸測高斷線檢測, false);

            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);

            SetOutBit(outBit_切割區增濕水簾, false);
            SetOutBit(outBit_切割區水霧隔絕氣簾, false);
            SetOutBit(outBit_水槽清潔噴水啟動, false);
            SetOutBit(outBit_水槽清潔吹氣啟動, false);
            SetOutBit(outBit_清潔風刀, false);

            bCanPutRunSate = false;
            bCanGetRunSate = false;
            bCanFrontGetRun = false;

            SetOutBit(outBit_ManualLoadOK, false);

            SetOutBit(outBit_切割平台真空破壞, false);

            if (Timer_ActionIni.On(SReadValue("汽缸動作時間").ToInt()))
            {
                bool OK = GetInBit(inBit_Z2刀輪護蓋關閉) &&
                    GetInBit(inBit_Z1刀輪護蓋關閉) &&
                    GetInBit(inBit_高倍CCD護蓋關閉);
                if (OK)
                {
                    Timer_ActionIni.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else if (Timer_ActionIni.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    Timer_ActionIni.Restart();
                    string s = "";

                    if (!GetInBit(inBit_Z2刀輪護蓋關閉))
                        s = inBit_Z2刀輪護蓋關閉.Text;
                    if (!GetInBit(inBit_Z1刀輪護蓋關閉))
                        s += "," + inBit_Z1刀輪護蓋關閉.Text;
                    if (!GetInBit(inBit_高倍CCD護蓋關閉))
                        s += "," + inBit_高倍CCD護蓋關閉.Text;

                    ShowAlarm("i", 1, s);
                    Timer_ActionIni.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_初始化_XY軸移到安全位_Run()
        {
            bool OK = GetInBit(inBit_Z1非接觸測高蓋關閉檢知) &&
                    GetInBit(inBit_Z2非接觸測高蓋關閉檢知);
            if (OK)
            {
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                    {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                    {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
                };

                if (MultiMotorsG00(dicMotorPosPairs, Timer_ActionIni)) return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                int NCT_YieldDis = SReadValue("NCT_X_YieldDis").ToInt();
                if (NCT_YieldDis == 0)
                {
                    NCT_YieldDis = 30000;
                    SSetValue("NCT_X_YieldDis", NCT_YieldDis);
                }

                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                    {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt() - NCT_YieldDis},
                    {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt() + NCT_YieldDis}
                };

                if (MultiMotorsG00(dicMotorPosPairs, Timer_ActionIni)) return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_初始化_U軸入料_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_ActionIni)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_初始化_Cover初始_Run()
        {
            SetOutBit(outBit_Z1非接觸式測高護蓋關閉, true);
            SetOutBit(outBit_Z2非接觸式測高護蓋關閉, true);

            if (Timer_ActionIni.On(SReadValue("汽缸動作時間").ToInt()))
            {
                bool OK = GetInBit(inBit_Z1非接觸測高蓋關閉檢知) &&
                    GetInBit(inBit_Z2非接觸測高蓋關閉檢知);
                if (OK)
                {
                    SetOutBit(outBit_切割平台真空建立, false);//關真空
                    Timer_ActionIni.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else if (Timer_ActionIni.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    string s = "";

                    if (!GetInBit(inBit_Z1非接觸測高蓋關閉檢知))
                        s += "," + inBit_Z1非接觸測高蓋關閉檢知.Text;

                    if (!GetInBit(inBit_Z2非接觸測高蓋關閉檢知))
                        s += "," + inBit_Z2非接觸測高蓋關閉檢知.Text;

                    ShowAlarm("i", 1, s);
                    Timer_ActionIni.Restart();
                }
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_初始化_動作結束_Run()
        {
            ActionStopFinish = true;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_等待結果_Run()
        {
            switch (mCCDAction)//設定樣式
            {
                case CCDSendResult.OK:
                    {
                        AutoMark.nSearchMarkStep = 0;
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 34);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 35);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_視覺通訊_Run()
        {
            return PatternMatch(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_自動_定位_視覺等待結果_Run()
        {
            switch (mCCDAction)//自動掃靶
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:

                    bool bIsSearchNearMark = OReadValue("bSearchNearMark").ToBoolean();
                    int nTotalSearchCnt = (int)Math.Pow(OReadValue("iSearchMark_round").ToDouble() * 2 + 1, 2.0f) - 1;

                    if (bIsSearchNearMark && AutoMark.nSearchMarkStep <= nTotalSearchCnt)
                    {
                        AutoMark.nSearchMarkStep += 1;
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE2;
                    }
                    else
                    {
                        if (m_enScanMethod == enScanTargetMethod.三點掃靶)
                        {
                            AutoMark.nSearchMarkStep = 0;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.CASE3;
                        }

                        AutoMark.nSearchMarkStep = 0;
                        ShowAlarm("i", 19);//九宮格尋靶失敗，請確認靶點清晰度或退出料片重新擺正
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }

                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 18);//視覺取像失敗，請按繼續重試
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 19);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private void FC_自動_定位_補償_BeforeRun()
        {
            AutoMark.nCurrentPos = motor_切刀橫移馬達_Z2X.ReadPos();
        }

        private FlowChart.FCRESULT FC_自動_定位_補償_Run()
        {
            bool bIsYMoveDone = false;
            bool bIsXMoveDone = false;

            bIsYMoveDone = motor_切割平台前後馬達Y.G00(AutoMark.nTempY);

            if (SReadValue("BackClearance").ToBoolean())
            {
                bIsXMoveDone = motor_切刀橫移馬達_Z2X.G00(AutoMark.nCurrentPos, AutoMark.nOffsetX);
            }
            else
            {
                bIsXMoveDone = motor_切刀橫移馬達_Z2X.G00(AutoMark.nTempX);
            }

            if (bIsXMoveDone && bIsYMoveDone)
            {
                Timer_Auto.Restart();

                if (PReadValue("IgnoreFindMark").ToBoolean())
                {
                    return FlowChart.FCRESULT.CASE1;
                }
                else
                    return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Auto.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    if (!bIsXMoveDone) MotorOverTime(motor_切刀橫移馬達_Z2X.Text);
                    if (!bIsYMoveDone) MotorOverTime(motor_切割平台前後馬達Y.Text);

                    Timer_Auto.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        //+ By Max 20210309, 僅做Pattern Match時的點位計算
        private FlowChart.FCRESULT FC_自動_定位_計算_Run()
        {
            DataView dv;
            switch (m_enScanMethod)
            {
                case enScanTargetMethod.四點掃靶:
                case enScanTargetMethod.三點掃靶:
                    switch (AutoMark.nFindMarkStep)
                    {
                        case 0://CH2左上
                        case 4://CH1左上
                            AutoMark.ptLTMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptLTMarkPos.Y = AutoMark.nTempY;
                            break;
                        case 1://CH2左下
                        case 5://CH1左下
                            AutoMark.ptLBMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptLBMarkPos.Y = AutoMark.nTempY;
                            break;
                        case 2://CH2右上
                        case 6://CH1右上
                            AutoMark.ptRTMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptRTMarkPos.Y = AutoMark.nTempY;
                            break;
                        case 3://CH2右下
                        case 7://CH1右下
                            AutoMark.ptRBMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptRBMarkPos.Y = AutoMark.nTempY;
                            break;
                    }
                    break;
                case enScanTargetMethod.半靶掃靶:
                    dv = new DataView(LearnMarkPos.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                    if (AutoMark.nFindMarkStep < 2)
                    {
                        if (AutoMark.nFindMarkStep % 2 == 0)
                        {
                            TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                        else
                        {
                            BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                    }
                    else
                    {
                        if (AutoMark.nFindMarkStep <= dv.Count)//掃上半部
                        {
                            TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                    }
                    break;
                case enScanTargetMethod.全靶掃靶_口字形:
                    dv = new DataView(LearnMarkPos.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                    if (AutoMark.nFindMarkStep < 2)
                    {
                        if (AutoMark.nFindMarkStep % 2 == 0)
                        {
                            TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                        else
                        {
                            BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                    }
                    else
                    {
                        if (AutoMark.nFindMarkStep <= dv.Count)//先掃上半部，再掃下半部
                        {
                            TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                        else
                        {
                            BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                    }
                    break;
                case enScanTargetMethod.全靶掃靶_N字形:
                    if (AutoMark.nFindMarkStep % 2 == 0)
                    {
                        TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                    }
                    else
                    {
                        BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                    }
                    break;
            }
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_成功_Run()
        {
            int Score = 90;
            int xoffset = 0;
            int yoffset = 0;

            //回傳Sh,Pos,X=?,Y=?,A=?,S=?;
            if (IsSimulation() == 0)
            {
                Score = (int)(Convert.ToDouble(CCDResult[5].Split('=')[1]) * 100);
                xoffset = -1 * (int)Convert.ToDouble(CCDResult[2].Split('=')[1]);
                yoffset = 1 * (int)Convert.ToDouble(CCDResult[3].Split('=')[1]);
                AutoMark.nTempX = motor_切刀橫移馬達_Z2X.ReadEncPos() + xoffset;
                AutoMark.nOffsetX = xoffset;
                AutoMark.nTempY = motor_切割平台前後馬達Y.ReadEncPos() + yoffset;
            }

            Timer_Auto.Restart();
            //+ By Max, 20210804
            if (PReadValue("IgnoreFindMark").ToBoolean())
                return FlowChart.FCRESULT.CASE1;
            else
                return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_Delay2_Run()
        {
            //+ By Max, 20210804
            //if (Timer_Auto.On(SReadValue("馬達整定時間").ToInt()))
            //{
            //    Timer_Auto.Restart();
            //    return FlowChart.FCRESULT.NEXT;
            //}
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_選擇圖案失敗_Run()
        {
            ShowAlarm("i", 3);
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_尋靶_Run()
        {
            int offsetx = 0;
            int offsety = 0;
            int Pitch = OReadValue("iSearchMark_pitch").ToInt();

            #region offset設定
            switch (AutoMark.nSearchMarkStep)
            {
                case 1:
                    offsetx = Pitch;
                    offsety = Pitch;
                    break;
                case 2:
                    offsetx = Pitch;
                    break;
                case 3:
                    offsetx = Pitch;
                    offsety = -Pitch;
                    break;
                case 4:
                    offsety = -Pitch;
                    break;
                case 5:
                    offsetx = -Pitch;
                    offsety = -Pitch;
                    break;
                case 6:
                    offsetx = -Pitch;
                    break;
                case 7:
                    offsetx = -Pitch;
                    offsety = Pitch;
                    break;
                case 8:
                    offsety = Pitch;
                    break;

            }
            #endregion

            int posx = AutoMark.nTempX + offsetx;
            int posy = AutoMark.nTempY + offsety;

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, posy},
                {motor_切刀橫移馬達_Z2X, posx}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_失敗_Run()
        {
            if (ManualAlignment)//使用手動定靶
            {
                ManualAlignment = false;
                bUseManualAlignment = true;
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_料片確認_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導執行_接收命令_Run()
        {
            if (mLotend)
                return FlowChart.FCRESULT.CASE1;

            switch (m_enTeachSel)
            {
                case enTeachActiionSel.接觸測高:
                    FC_接觸測高_動作開始.TaskReset();
                    break;

                case enTeachActiionSel.非接觸測高:
                    FC_非接觸測高_動作開始.TaskReset();
                    break;

                case enTeachActiionSel.基準線校正:
                    FC_基準線校正_動作開始.TaskReset();
                    break;

                case enTeachActiionSel.切割道學習:
                    FC_切割道學習_動作開始.TaskReset();
                    break;

                case enTeachActiionSel.靶點學習:
                    FC_靶點學習_動作開始.TaskReset();

                    if (OReadValue("bFastAutoFocus").ToBoolean())
                    {
                        FC_教導_快速自動對焦_開始.TaskReset();
                    }
                    else
                    {
                        FC_教導_自動對焦_開始.TaskReset();//v4.0.1.25新增自動對焦
                    }
                    break;

                case enTeachActiionSel.換刀:
                    FC_換刀流程_動作開始.TaskReset();
                    break;

                case enTeachActiionSel.圓心校正:
                    FC_圓心校正_開始.TaskReset();
                    break;
            }

            if (m_enTeachSel != enTeachActiionSel.無動作)
            {
                Teach_ActionResult = 0;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導執行_執行_Run()
        {
            if (bSetTeachFlowChartEnd)
            {
                m_bIsNeedCheckVaca = false;//bSetTeachFlowChartEnd
                m_bIsPauseNeedToContinue = false;
                //+ By Max 20210312, v4.0.1.53
                SetContinueRun(false);

                ActionStopFinish = false;
                FC_初始化_開始.TaskReset();
                TeachFlowStep = 98;
                return FlowChart.FCRESULT.CASE1;
            }

            if (Teach_ActionResult == 1)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            if (m_bIsNeedCheckVaca && !m_bIsPauseNeedToContinue)
            {
                if (CheckVaca(0) != "")
                {
                    ShowAlarm("E", 16);
                    return FlowChart.FCRESULT.IDLE;
                }
            }

            switch (m_enTeachSel)
            {
                case enTeachActiionSel.接觸測高:
                    FC_接觸測高_動作開始.MainRun();
                    break;
                case enTeachActiionSel.非接觸測高:
                    FC_非接觸測高_動作開始.MainRun();
                    break;
                case enTeachActiionSel.基準線校正:
                    FC_基準線校正_動作開始.MainRun();
                    break;
                case enTeachActiionSel.切割道學習:
                    FC_切割道學習_動作開始.MainRun();
                    break;
                case enTeachActiionSel.靶點學習:
                    if (CallAutoFocus == "Teach")
                    {
                        if (OReadValue("bFastAutoFocus").ToBoolean())
                        {
                            FC_教導_快速自動對焦_開始.MainRun();
                        }
                        else
                        {
                            FC_教導_自動對焦_開始.MainRun();//v4.0.1.25
                        }
                    }
                    else
                    {
                        CallAutoFocus = "";
                        FC_靶點學習_動作開始.MainRun();
                    }
                    break;
                case enTeachActiionSel.換刀:
                    FC_換刀流程_動作開始.MainRun();
                    break;
                case enTeachActiionSel.圓心校正:
                    FC_圓心校正_開始.MainRun();
                    break;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導執行_終止後初始化_Run()
        {
            switch (m_enTeachSel)
            {
                case enTeachActiionSel.接觸測高:
                case enTeachActiionSel.非接觸測高:
                case enTeachActiionSel.基準線校正:
                case enTeachActiionSel.切割道學習:
                case enTeachActiionSel.靶點學習: 
                case enTeachActiionSel.換刀:
                case enTeachActiionSel.圓心校正:
                    FC_初始化_開始.MainRun();
                    break;
            }

            if (ActionStopFinish)
            {
                Teach_ActionResult = 2;
                CallAutoFocus = "";
                bSetTeachFlowChartEnd = false;
                m_enTeachSel = enTeachActiionSel.無動作;
                ShowAlarm("i", 16);
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導執行_完成更新_Run()
        {
            m_enTeachSel = enTeachActiionSel.無動作;
            m_bIsNeedCheckVaca = false; //Teach完成

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_停止_Run()
        {
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_Z軸移至安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_XYU移至安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachFlowStep = 3;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 40;
                TeachUserCheckOk = false;
                TeachUserCheckSkip = false;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_動作結束_Run()
        {
            Teach_ActionResult = 1;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_動作開始_Run()
        {
            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 57;
            TeachMsg = new String[] { "換刀流程開始" };

            #region By Wolf 20210824 v4.0.92.0 換刀流程後強制測高流程必須重做
            Z1CheckBeforeStart.ZSparkTestDone = false;
            Z2CheckBeforeStart.ZSparkTestDone = false;
            #endregion

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_換刀流程_Z移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_換刀流程_其他軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt() - 30000},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt() + 30000}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                    Z1SpindleStop = true;
                if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                    Z2SpindleStop = true;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_換刀流程_主軸停止_Run()
        {
            if (IsSimulation() != 0)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 58;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (m_enZ1SpindleStatus == enSpindleStatus.Stop && m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 58;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("主軸動作時間").ToInt()))
            {
                if (m_enZ1SpindleStatus != enSpindleStatus.Stop)
                {
                    TeachStatus = 99;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 12;
                    ShowAlarm("w", 12);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                if (m_enZ2SpindleStatus != enSpindleStatus.Stop)
                {
                    TeachStatus = 99;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 13;
                    ShowAlarm("w", 13);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_刀輪護蓋開啟_Run()
        {
            SetOutBit(outBit_Z1刀輪護蓋開啟, true);
            SetOutBit(outBit_Z2刀輪護蓋開啟, true);

            if (IsSimulation() != 0)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //if (GetInBit(inBit_Z1刀輪護蓋開啟) && GetInBit(inBit_Z2刀輪護蓋開啟))
            if ((!GetInBit(inBit_Z1刀輪護蓋關閉) || !bUseZ1_Spindle) && (!GetInBit(inBit_Z2刀輪護蓋關閉) || !bUseZ2_Spindle))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                if (!GetInBit(inBit_Z1刀輪護蓋開啟) && !GetInBit(inBit_Z2刀輪護蓋開啟))
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋開啟.Text + "," + outBit_Z2刀輪護蓋開啟.Text);
                if (!GetInBit(inBit_Z1刀輪護蓋開啟))
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋開啟.Text);
                if (!GetInBit(inBit_Z2刀輪護蓋開啟))
                    ShowAlarm("i", 1, outBit_Z2刀輪護蓋開啟.Text);
                Timer_Teach.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT 等待刀靜止_Run()
        {
            if (Timer_Teach.On(SReadValue("馬達整定時間").ToInt()))
            {
                TeachUserCheckOk = false;
                SetOutBit(outBit_主軸軸承鎖定, true);
                TeachAlarmLevel = "I";
                TeachAlarmCode = 60;
                TeachMsg = new String[] { "可以開始換刀，請將更換的刀具序號和已使用里程輸入之後按下確認"};
                ShowAlarm("I", 60);
                TeachFlowStep = 1;
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_等待換刀完成_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFlowStep = 0;
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_刀輪護蓋關閉_Run()
        {
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);

            if (IsSimulation() != 0)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 59;
                //if (IsUseDAQ)
                //{
                //    TeachMsg = new String[] { "請調整破刀sensor(新刀1.2-1.4V)，請在調整之後按下確認" };
                //}
                //else
                //{
                //    TeachMsg = new String[] { "請調整破刀sensor(新刀1.1-1.2V)，請在調整之後按下確認" };
                //}
                ShowAlarm("I", 59);
                TeachFlowStep = 1;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (GetInBit(inBit_Z1刀輪護蓋關閉) && GetInBit(inBit_Z2刀輪護蓋關閉))
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 59;
                ShowAlarm("I", 59);
                TeachFlowStep = 1;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                if (!GetInBit(inBit_Z1刀輪護蓋關閉) && !GetInBit(inBit_Z2刀輪護蓋關閉))
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text + "," + outBit_Z2刀輪護蓋關閉.Text);
                if (!GetInBit(inBit_Z1刀輪護蓋關閉))
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text);
                if (!GetInBit(inBit_Z2刀輪護蓋關閉))
                    ShowAlarm("i", 1, outBit_Z2刀輪護蓋關閉.Text);
                Timer_Teach.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_等待調整破刀Sensor_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFlowStep = 0;
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_換刀流程_檢查破刀Sensor電壓_Run()
        {
            double dBreakVolThres = SReadValue("dBreakVolThreshold").ToDouble();

            bool bZ1BladeBreak = false;
            bool bZ2BladeBreak = false;
            if (IsUseDAQ)
            {
                //+ By Max, 20211209, v4.0.100.0, analogIn_Z1破片檢知 -> td_Z1破片檢知 
                bZ1BladeBreak = (td_Z1破片檢知.Value > 1.3 || td_Z1破片檢知.Value < 1.1) && bUseZ1_Spindle;
                bZ2BladeBreak = (td_Z2破片檢知.Value > 1.3 || td_Z2破片檢知.Value < 1.1) && bUseZ2_Spindle;
            }
            else
            {
                bZ1BladeBreak = (analogIn_Z1破片檢知.RealValue > dBreakVolThres) && bUseZ1_Spindle;
                bZ2BladeBreak = (analogIn_Z2破片檢知.RealValue > dBreakVolThres) && bUseZ2_Spindle;
            }


            if (IsSimulation() != 0)
            {
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (!bZ1BladeBreak && !bZ2BladeBreak)
            {
                SetOutBit(outBit_主軸軸承鎖定, false);
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (bZ1BladeBreak)
                {
                    TeachAlarmLevel = "E";
                    TeachAlarmCode = 88;
                    if (IsUseDAQ)
                    {
                        TeachMsg = new string[] { "Z1", string.Format("{0}", "1.1V ~ 1.3V"), string.Format("{0:0.000}", td_Z1破片檢知.Value) };
                    }
                    else
                    {
                        TeachMsg = new string[] { "Z1", string.Format("{0:0.000}", dBreakVolThres), string.Format("{0:0.000}", analogIn_Z1破片檢知.RealValue) };
                    }
                    ShowAlarm("E", 88, TeachMsg);
                    TeachFlowStep = 2;
                }

                if (bZ2BladeBreak)
                {
                    TeachAlarmLevel = "E";
                    TeachAlarmCode = 88;
                    if (IsUseDAQ)
                    {
                        TeachMsg = new string[] { "Z2", string.Format("{0}", "1.1V ~ 1.3V"), string.Format("{0:0.000}", td_Z2破片檢知.Value) };
                    }
                    else
                    {
                        TeachMsg = new string[] { "Z2", string.Format("{0:0.000}", dBreakVolThres), string.Format("{0:0.000}", analogIn_Z2破片檢知.RealValue) };
                    }
                    ShowAlarm("E", 88, TeachMsg);
                    TeachFlowStep = 2;
                }
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_換刀流程_Z移到安全位1_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_換刀流程_其他軸移到安全位1_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_換刀流程_動作結束_Run()
        {
            Teach_ActionResult = 1;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_切割道學習_動作開始_Run()
        {
            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 26;
            ShowAlarm("I", 26);//切割道學習開始
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_切割道學習_參數設定_Run()
        {
            TeachFixture.Reset();
            TeachFixture.strSide = "CH2";//從CH2開始學習
            LearnMarkPos.Reset();//0203
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            SetSpeed_U(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_切割道學習_XYU移至中心_Run()
        {
            bool b位置找到 = false;
            int iθ位移 = 0;

            b位置找到 = ChannelRotation(TeachFixture.strSide, out iθ位移);

            if (b位置找到)
            {
                TeachFixture.nTempθ = iMoveInitialU - iDD馬達一轉Pulse數 * iθ位移 / 4;
            }
            else
            {
                ShowAlarm("E", 90, "Learn Fixture Channel error");
                return default(FlowChart.FCRESULT);
            }

            int nPoxY = 0;
            int nPosX2 = 0;

            if (iθ位移 == 0)
            {
                nPoxY = SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
            }
            else
            {
                nPoxY = SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, TeachFixture.nTempθ},
                {motor_切割平台前後馬達Y, nPoxY},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, nPosX2}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 1;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 27;
                TeachMsg = new string[] { TeachFixture.strSide };
                ShowAlarm("I", 27);
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_視覺Live_Run()
        {
            return OpenVisionLive(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_切割道學習_等待結果_Run()
        {
            switch (mCCDAction)//等待Live
            {
                case CCDSendResult.OK:
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.NG:
                case CCDSendResult.InspectFail:
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 16;
                    TeachUserCheckOk = false;
                    TeachFlowStep = 0;
                    ShowAlarm("w", 16);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachAlarmLevel = "w";
                TeachAlarmCode = 17;
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                ShowAlarm("w", 17);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_切割道學習_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_切割道學習_確認第一基準點_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFixture.ptLTPos.X = motor_切刀橫移馬達_Z2X.ReadEncPos();
                TeachFixture.ptLTPos.Y = motor_切割平台前後馬達Y.ReadEncPos();
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 62;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_膠墊學習_馬達_Run()
        {
            int posx = 0;
            int posy = 0;
            int offsetx = 0;
            int offsety = 0;
            Point refer = new Point();

            posx = refer.X + offsetx;
            posy = refer.Y + offsety;

            if (posx <= motor_切刀橫移馬達_Z2X.SoftLimitN || posx >= motor_切刀橫移馬達_Z2X.SoftLimitP ||
                posy <= motor_切割平台前後馬達Y.SoftLimitN || posy >= motor_切割平台前後馬達Y.SoftLimitP)
            {
                TeachAlarmLevel = "w";
                TeachAlarmCode = 24;
                TeachFlowStep = 3;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, posy},
                {motor_切刀橫移馬達_Z2X, posx}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 63;
                TeachFlowStep = 3;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_確認第二基準點_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFixture.ptLBPos.X = motor_切刀橫移馬達_Z2X.ReadEncPos();
                TeachFixture.ptLBPos.Y = motor_切割平台前後馬達Y.ReadEncPos();
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 61;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_切割道學習_拉直_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, Convert.ToInt32(TeachFixture.nTempθ)},
                {motor_切割平台前後馬達Y, TeachFixture.ptLTPos.Y},
                {motor_切刀橫移馬達_Z2X, TeachFixture.ptLTPos.X},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachFlowStep = 0;
                if (TeachFixture.strSide == "CH1")
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 64;
                    TeachMsg = new string[] { TeachFixture.strSide, (TeachFixture.nLineNo - PReadValue("Num_col").ToInt()).ToString() };
                }
                else if (TeachFixture.strSide == "CH2")
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 64;
                    TeachMsg = new string[] { TeachFixture.strSide, (TeachFixture.nLineNo + 1).ToString() };
                }
                TeachUserCheckOk = false;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_計算_Run()
        {
            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            Double angle = 0;
            RotateCorrect(Org_X, Org_Y, ref TeachFixture.ptLTPos, ref TeachFixture.ptLBPos, ref angle);
            int i_angel = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);

            TeachFixture.nTempθ += i_angel;

            DataRow dr = TeachFixture.dtFixtureData.NewRow();
            DataRow dr1 = LearnMarkPos.dtScanData.NewRow();
            if (TeachFixture.strSide == "CH2")
            {
                for (int i = 0; i <= PReadValue("Num_col").ToInt(); i++)
                {
                    dr = TeachFixture.dtFixtureData.NewRow();
                    dr["Fixture_LineNo"] = (i + 1).ToString();
                    dr["Fixture_Side"] = TeachFixture.strSide;
                    dr["Fixture_X"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_X").ToInt())).ToString();
                    dr["Fixture_Angle"] = TeachFixture.nTempθ.ToString();
                    dr["Fixture_XOffset"] = "0";
                    TeachFixture.dtFixtureData.Rows.Add(dr);

                    dr1 = LearnMarkPos.dtScanData.NewRow();
                    dr1["Scan_LineNo"] = (i + 1).ToString();
                    dr1["Scan_Side"] = TeachFixture.strSide;
                    dr1["Scan_X1"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_X").ToInt())).ToString();
                    dr1["Scan_Y1"] = Org_Y - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToDouble());
                    dr1["Scan_X2"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_X").ToInt())).ToString();
                    dr1["Scan_Y2"] = Org_Y + Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToDouble());
                    dr1["Scan_Z"] = "0";
                    dr1["Scan_Angle"] = TeachFixture.nTempθ.ToString();
                    dr1["Scan_XOffset"] = "0";
                    LearnMarkPos.dtScanData.Rows.Add(dr1);
                }
            }
            else if (TeachFixture.strSide == "CH1")
            {
                for (int i = 0; i <= PReadValue("Num_row").ToInt(); i++)
                {
                    dr = TeachFixture.dtFixtureData.NewRow();
                    dr["Fixture_LineNo"] = (i + 1).ToString();
                    dr["Fixture_Side"] = TeachFixture.strSide;
                    dr["Fixture_X"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_Y").ToInt())).ToString();
                    dr["Fixture_Angle"] = TeachFixture.nTempθ.ToString();
                    dr["Fixture_XOffset"] = "0";
                    TeachFixture.dtFixtureData.Rows.Add(dr);

                    dr1 = LearnMarkPos.dtScanData.NewRow();
                    dr1["Scan_LineNo"] = (i + 1).ToString();
                    dr1["Scan_Side"] = TeachFixture.strSide;
                    dr1["Scan_X1"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_Y").ToInt())).ToString();
                    dr1["Scan_Y1"] = Org_Y - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToDouble());
                    dr1["Scan_X2"] = (TeachFixture.ptLTPos.X + (i * PReadValue("Pitch_Y").ToInt())).ToString();
                    dr1["Scan_Y2"] = Org_Y + Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToDouble());
                    dr1["Scan_Z"] = "0";
                    dr1["Scan_Angle"] = TeachFixture.nTempθ.ToString();
                    dr1["Scan_XOffset"] = "0";
                    LearnMarkPos.dtScanData.Rows.Add(dr1);
                }
            }
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_切割道學習_移到下一條_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFixture.nLineNo++;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //+ By Max 20210223, 切割學習重學
            if (TeachUserCheckPrevious)
            {
                if(TeachFixture.strSide == "CH2")
                    TeachFixture.nLineNo = 0;
                else
                    TeachFixture.nLineNo = PReadValue("Num_col").ToInt();
                //if(TeachFixture.LineNo > 0)
                //    TeachFixture.LineNo--;
                TeachUserCheckPrevious = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_切割道學習_確認_Run()
        {
            //超出CH2欄數，切換為CH1
            if (TeachFixture.strSide == "CH2" && TeachFixture.nLineNo > PReadValue("Num_col").ToInt())
            {
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 65;
                TeachMsg = new string[] { TeachFixture.strSide };
                TeachFixture.strSide = "CH1";
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            //超出CH1欄數，結束
            if (TeachFixture.strSide == "CH1" && TeachFixture.nLineNo > PReadValue("Num_col").ToInt() + PReadValue("Num_row").ToInt() + 1)
            {
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 65;
                TeachMsg = new string[] { TeachFixture.strSide };
                TeachFixture.strSide = "";
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            int posu = Convert.ToInt32(TeachFixture.dtFixtureData.Rows[TeachFixture.nLineNo]["Fixture_Angle"].ToString());
            int posx = Convert.ToInt32(TeachFixture.dtFixtureData.Rows[TeachFixture.nLineNo]["Fixture_X"].ToString());
            int posy = TeachFixture.ptLTPos.Y;

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, posu},
                {motor_切割平台前後馬達Y, posy},
                {motor_切刀橫移馬達_Z2X, posx},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachFlowStep = 0;
                if (TeachFixture.strSide == "CH1")
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 64;
                    TeachMsg = new string[] { TeachFixture.strSide, (TeachFixture.nLineNo - PReadValue("Num_col").ToInt()).ToString() };

                }
                else if (TeachFixture.strSide == "CH2")
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 64;
                    TeachMsg = new string[] { TeachFixture.strSide, (TeachFixture.nLineNo + 1).ToString() };

                }
                TeachUserCheckOk = false;

                return FlowChart.FCRESULT.CASE2;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_Z移到安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_XYU移到安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachFlowStep = 3;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 66;
                TeachUserCheckOk = false;
                TeachUserCheckSkip = false;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_動作結束_Run()
        {
            Teach_ActionResult = 1;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_Run()
        {
            //+ By Max 20210224, 內靶掃描
            if (PReadValue("TargetFind").ToInt() == 0) //內靶
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 73;
            }
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_內靶設定_Run()
        {
            //+ By Max 20210224, 內靶掃描
            if (PReadValue("TargetFind").ToInt() == 0) //內靶
            {
                if (bLTCutPosHasGot)
                {
                    bLTCutPosHasGot = false;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_計算_Run()
        {
            DataView dv = new DataView(TeachFixture.dtFixtureData);
            dv.RowFilter = "Fixture_Side = '" + LearnMarkPos.strSide + "'";

            //+ By Max 20210224, 內靶掃描
            int Pitch = Math.Abs(LearnMarkPos.ptRTMarkPos.X - LearnMarkPos.ptLTMarkPos.X) / (dv.Count - 1);
            if (PReadValue("TargetFind").ToInt() == 0) //內靶
            {
                Pitch = Math.Abs(LearnMarkPos.ptRTMarkPos.X - LearnMarkPos.ptLTMarkPos.X) / (dv.Count - 2);
            }

            int SPCPitch = Math.Abs(Convert.ToInt32(dv[1]["Fixture_X"].ToString()) - Convert.ToInt32(dv[0]["Fixture_X"].ToString()));//SPC的Pitch

            //Pitch是否超出卡控值
            if (Math.Abs(Pitch - SPCPitch) > PReadValue("Pitch_tolerance").ToInt())
            {
                ShowAlarm("i", 9, LearnMarkPos.strSide, Pitch, SPCPitch);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.IDLE;
            }

            DataRow dr = LearnMarkPos.dtScanData.NewRow();

            //+ By Max 20210224, 內靶掃描
            if (PReadValue("TargetFind").ToInt() == 0) //內靶
            {
                for (int i = 0; i < dv.Count - 1; i++)
                {
                    dr = LearnMarkPos.dtScanData.NewRow();
                    dr["Scan_LineNo"] = (i + 1).ToString();
                    dr["Scan_Side"] = LearnMarkPos.strSide;
                    dr["Scan_X1"] = Convert.ToInt32(dv[i]["Fixture_X"].ToString()) - (iLeftTopCutPos - LearnMarkPos.ptRTMarkPos.X);
                    dr["Scan_Y1"] = LearnMarkPos.ptLTMarkPos.Y.ToString();
                    dr["Scan_X2"] = Convert.ToInt32(dv[i]["Fixture_X"].ToString()) - (iLeftTopCutPos - LearnMarkPos.ptRTMarkPos.X);
                    dr["Scan_Y2"] = LearnMarkPos.ptLBMarkPos.Y.ToString();
                    dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                    dr["Scan_Angle"] = Convert.ToInt32(dv[i]["Fixture_Angle"].ToString());
                    dr["Scan_XOffset"] = iLeftTopCutPos - LearnMarkPos.ptRTMarkPos.X;
                    LearnMarkPos.dtScanData.Rows.Add(dr);
                }
            }
            else
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    dr = LearnMarkPos.dtScanData.NewRow();
                    dr["Scan_LineNo"] = (i + 1).ToString();
                    dr["Scan_Side"] = LearnMarkPos.strSide;
                    dr["Scan_X1"] = Convert.ToInt32(dv[i]["Fixture_X"].ToString());
                    dr["Scan_Y1"] = LearnMarkPos.ptLTMarkPos.Y.ToString();
                    dr["Scan_X2"] = Convert.ToInt32(dv[i]["Fixture_X"].ToString());
                    dr["Scan_Y2"] = LearnMarkPos.ptLBMarkPos.Y.ToString();
                    dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                    dr["Scan_Angle"] = Convert.ToInt32(dv[i]["Fixture_Angle"].ToString());
                    dr["Scan_XOffset"] = "0";
                    LearnMarkPos.dtScanData.Rows.Add(dr);
                }
            }

            if (LearnMarkPos.nStep == 4)
            {
                LearnMarkPos.strSide = "CH1";
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (LearnMarkPos.nStep == 7)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;

        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_移至左上點_Run()
        {
            switch (LearnMarkPos.nFindMarkStep)
            {
                case 0:
                    LearnMarkPos.nTempX = LearnMarkPos.ptLTMarkPos.X;
                    LearnMarkPos.nTempY = LearnMarkPos.ptLTMarkPos.Y;
                    break;
                case 1:
                    LearnMarkPos.nTempX = LearnMarkPos.ptLBMarkPos.X;
                    LearnMarkPos.nTempY = LearnMarkPos.ptLBMarkPos.Y;
                    break;
                case 2:
                    LearnMarkPos.nTempX = LearnMarkPos.ptRTMarkPos.X;
                    LearnMarkPos.nTempY = LearnMarkPos.ptRTMarkPos.Y;
                    break;

            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, LearnMarkPos.nTempY},
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.nTempX},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_視覺通訊_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 28);
                Timer_Teach.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_等待結果_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                    LearnMarkPos.nTempX += xoffset;
                    LearnMarkPos.nTempY += yoffset;

                    switch (LearnMarkPos.nFindMarkStep)
                    {
                        case 0:
                            LearnMarkPos.ptLTMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptLTMarkPos.Y = LearnMarkPos.nTempY;
                            break;
                        case 1:
                            LearnMarkPos.ptLBMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptLBMarkPos.Y = LearnMarkPos.nTempY;
                            break;
                        case 2:
                            LearnMarkPos.ptRTMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptRTMarkPos.Y = LearnMarkPos.nTempY;
                            break;

                    }
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        TeachUserCheckOk = false;
                        TeachFlowStep = 0;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 20;
                        ShowAlarm("w", 20);//視覺取像失敗，請按繼續重試
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 21;
                ShowAlarm("w", 21);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_移至左下點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, LearnMarkPos.ptLBMarkPos.Y},
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.ptLBMarkPos.X}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_視覺通訊1_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_等待結果1_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                    LearnMarkPos.ptLBMarkPos.X += xoffset;
                    LearnMarkPos.ptLBMarkPos.Y += yoffset;

                    int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
                    int Org_Y = SReadValue("圓心點位_Y").ToInt();
                    Double angle = 0;
                    RotateCorrect(Org_X, Org_Y, ref LearnMarkPos.ptLTMarkPos, ref LearnMarkPos.ptLBMarkPos, ref angle);
                    int i_angel = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                    LearnMarkPos.nTempθ += i_angel;

                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_NG1_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_拉直確認_Run()
        {
            int Markgap = LearnMarkPos.ptLTMarkPos.X - LearnMarkPos.ptLBMarkPos.X;
            if (Math.Abs(Markgap) <= SReadValue("CCD偏移量").ToInt())
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                LearnMarkPos.nFindMarkStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_移至右上點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, LearnMarkPos.ptRTMarkPos.Y},
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.ptRTMarkPos.X},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_視覺通訊2_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_等待結果2_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                    LearnMarkPos.ptRTMarkPos.X += xoffset;
                    LearnMarkPos.ptRTMarkPos.Y += yoffset;

                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_NG2_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_Run()
        {
            //AlignmentFinish = true;
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_定位_選擇靶點編號_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
            //選擇靶點Pattern，X為Pattern索引值,0~3
            string cmd = "HMPI";
            string index = "";
            switch (m_enScanMethod)
            {
                case enScanTargetMethod.四點掃靶:
                case enScanTargetMethod.三點掃靶:
                    switch (AutoMark.nFindMarkStep)
                    {
                        case 0://CH2左上角
                        case 2://CH2右上角
                        case 4://CH1左上角
                        case 6://CH1右上角
                            index = AutoMark.strSide + "_TargetNumberF";
                            break;
                        case 1://CH2左下角
                        case 3://CH2右下角
                        case 5://CH1左下角
                        case 7://CH1右下角
                            index = AutoMark.strSide + "_TargetNumberN";
                            break;
                    }
                    break;
                case enScanTargetMethod.半靶掃靶:
                    if (AutoMark.nFindMarkStep == 1)//左下角
                    {
                        index = AutoMark.strSide + "_TargetNumberN";
                    }
                    else//其他
                    {
                        index = AutoMark.strSide + "_TargetNumberF";
                    }
                    break;
                case enScanTargetMethod.全靶掃靶_口字形:
                    if (AutoMark.nFindMarkStep < 2)
                    {
                        if (AutoMark.nFindMarkStep % 2 == 0)
                        {
                            index = AutoMark.strSide + "_TargetNumberF";
                        }
                        else
                        {
                            index = AutoMark.strSide + "_TargetNumberN";
                        }
                    }
                    else
                    {
                        if (AutoMark.nFindMarkStep <= dv.Count)//先掃上半部，再掃下半部
                        {
                            index = AutoMark.strSide + "_TargetNumberF";
                        }
                        else
                        {
                            index = AutoMark.strSide + "_TargetNumberN";
                        }
                    }
                    break;

                case enScanTargetMethod.全靶掃靶_N字形:
                    if (AutoMark.nFindMarkStep % 2 == 0)
                    {
                        index = AutoMark.strSide + "_TargetNumberF";
                    }
                    else
                    {
                        index = AutoMark.strSide + "_TargetNumberN";
                    }
                    break;

                case enScanTargetMethod.即看即切:
                    if (AutoMark.nFindMarkStep == 0)
                    {
                        index = AutoMark.strSide + "_TargetNumberN";
                    }
                    else
                    {
                        index = AutoMark.strSide + "_TargetNumberF";
                    }
                    break;
            }

            index = PReadValue(index).ToString();

            if (SetCCDCmd(cmd, index))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            else if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 33);
                Timer_Auto.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_視覺通訊_FM_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 28);
                Timer_Auto.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_視覺等待結果_FM_Run()
        {
            DataView dv;
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    if (IsSimulation() == 0)
                    {
                        int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                        int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                        AutoMark.nTempX += xoffset;
                        AutoMark.nTempY += yoffset;
                    }

                    switch (m_enScanMethod)
                    {
                        case enScanTargetMethod.即看即切:
                            if (AutoMark.nFindMarkStep == 0)
                            {
                                AutoMark.ptLBMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptLBMarkPos.Y = AutoMark.nTempY;
                            }
                            else
                            {
                                AutoMark.ptLTMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptLTMarkPos.Y = AutoMark.nTempY;
                            }
                            break;
                        case enScanTargetMethod.四點掃靶:
                        case enScanTargetMethod.三點掃靶:
                            switch (AutoMark.nFindMarkStep)
                            {
                                case 0://CH2左上
                                case 4://CH1左上
                                    AutoMark.ptLTMarkPos.X = AutoMark.nTempX;
                                    AutoMark.ptLTMarkPos.Y = AutoMark.nTempY;
                                    break;
                                case 1://CH2左下
                                case 5://CH1左下
                                    AutoMark.ptLBMarkPos.X = AutoMark.nTempX;
                                    AutoMark.ptLBMarkPos.Y = AutoMark.nTempY;
                                    break;
                                case 2://CH2右上
                                case 6://CH1右上
                                    AutoMark.ptRTMarkPos.X = AutoMark.nTempX;
                                    AutoMark.ptRTMarkPos.Y = AutoMark.nTempY;
                                    break;
                                case 3://CH2右下
                                case 7://CH1右下
                                    AutoMark.ptRBMarkPos.X = AutoMark.nTempX;
                                    AutoMark.ptRBMarkPos.Y = AutoMark.nTempY;
                                    break;
                            }
                            break;
                        case enScanTargetMethod.半靶掃靶:
                            dv = new DataView(LearnMarkPos.dtScanData);
                            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                            if (AutoMark.nFindMarkStep < 2)
                            {
                                if (AutoMark.nFindMarkStep % 2 == 0)
                                {
                                    TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                                else
                                {
                                    BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                            }
                            else
                            {
                                if (AutoMark.nFindMarkStep <= dv.Count)//掃上半部
                                {
                                    TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                            }
                            break;
                        case enScanTargetMethod.全靶掃靶_口字形:
                            dv = new DataView(LearnMarkPos.dtScanData);
                            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                            if (AutoMark.nFindMarkStep < 2)
                            {
                                if (AutoMark.nFindMarkStep % 2 == 0)
                                {
                                    TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                                else
                                {
                                    BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                            }
                            else
                            {
                                if (AutoMark.nFindMarkStep <= dv.Count)//先掃上半部，再掃下半部
                                {
                                    TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                                else
                                {
                                    BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                                }
                            }
                            break;
                        case enScanTargetMethod.全靶掃靶_N字形:
                            if (AutoMark.nFindMarkStep % 2 == 0)
                            {
                                TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                            else
                            {
                                BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                            break;
                    }
                    FindMarkRetryCount = 0;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    FindMarkRetryCount++;
                    if (FindMarkRetryCount >= SReadValue("iVisionRetryCount").ToInt())
                    {
                        ShowAlarm("w", 20);
                        FindMarkRetryCount = 0;
                    }
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1; // 超出重試次數，警報
            }

            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 21);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_FM失敗_Run()
        {
            if (ManualAlignment)//使用手動定靶
            {
                ManualAlignment = false;
                bUseManualAlignment = true;
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.NEXT;
        }

        //private FlowChart.FCRESULT FC_四點掃靶_拉直_Run()
        //{
        //    return default(FlowChart.FCRESULT);
        //}

        private void button_主軸啟動_Click(object sender, EventArgs e)
        {
            Z1SpindleStart = true;
            Z2SpindleStart = true;
        }

        private void button_主軸關閉_Click(object sender, EventArgs e)
        {
            Z1SpindleStop = true;
            Z2SpindleStop = true;
        }

        private bool outBit_主軸軸承鎖定_IsSafeToAction(object sender)
        {
            //if (inBit_Z1主軸未運轉.Value && inBit_Z1主軸達設定值.Value && analogIn_Z1主軸轉速.RealValue < 1000 &&
            //    inBit_Z2主軸未運轉.Value && inBit_Z2主軸達設定值.Value && analogIn_Z2主軸轉速.RealValue < 1000)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataRow dr = TeachFixture.dtFixtureData.NewRow();

            dr = TeachFixture.dtFixtureData.NewRow();
            dr["LineNo"] = "10";
            dr["Side"] = TeachFixture.strSide;
            dr["X"] = (TeachFixture.ptLTPos.X + (1 * PReadValue("Pitch_X").ToInt())).ToString();
            dr["Angle"] = TeachFixture.nTempθ.ToString();
            dr["XOffset"] = "0";
            TeachFixture.dtFixtureData.Rows.Add(dr);
            PSetTable("DGVFixtureData", TeachFixture.dtFixtureData);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            FC_四點掃靶_掃靶開始.TaskReset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Load_PCBData();
            //DataView dv = new DataView(LearnMarkPos.ScanData);
            //dv.RowFilter = "Scan_Side = '" + "CH1" + "'";

            //int a = Convert.ToInt32(dv[dv.Count - 2]["Scan_X1"]);
            //int b = Convert.ToInt32(dv[dv.Count - 2]["Scan_Angle"]);
            TCPIPClose();
            FC_連線開始.TaskReset();
        }

        private FlowChart.FCRESULT FC_下刀點確認_開始_Run()
        {
            if (TC_Side != "")
            {
                if (TC_Side == "CH2")
                {
                    TC_U = Convert.ToInt32(AutoMark.dtScanData.Rows[TC_LineNo - 1]["Scan_Angle"].ToString());
                    TC_X = Convert.ToInt32(AutoMark.dtScanData.Rows[TC_LineNo - 1]["Scan_X1"].ToString());
                    if (TC_Loc == "AWAY")
                    {
                        TC_Y = Convert.ToInt32(AutoMark.dtScanData.Rows[TC_LineNo - 1]["Scan_Y1"].ToString());
                    }
                    else if (TC_Loc == "NEAR")
                    {
                        TC_Y = Convert.ToInt32(AutoMark.dtScanData.Rows[TC_LineNo - 1]["Scan_Y2"].ToString());
                    }
                }
                else if (TC_Side == "CH1")
                {
                    TC_U = Convert.ToInt32(AutoMark.dtScanData.Rows[PReadValue("Num_col").ToInt() + TC_LineNo]["Scan_Angle"].ToString());
                    TC_X = Convert.ToInt32(AutoMark.dtScanData.Rows[PReadValue("Num_col").ToInt() + TC_LineNo]["Scan_X1"].ToString());
                    if (TC_Loc == "AWAY")
                    {
                        TC_Y = Convert.ToInt32(AutoMark.dtScanData.Rows[PReadValue("Num_col").ToInt() + TC_LineNo]["Scan_Y1"].ToString());
                    }
                    else if (TC_Loc == "NEAR")
                    {
                        TC_Y = Convert.ToInt32(AutoMark.dtScanData.Rows[PReadValue("Num_col").ToInt() + TC_LineNo]["Scan_Y2"].ToString());
                    }
                }
                AutoFlowStep = 1;
                //AutoMessage = "Select[" + TC_Side + "]Line_" + TC_LineNo.ToString() + "[" + TC_Loc + "]";
                AutoMessage = new string[] { TC_Side, TC_LineNo.ToString(), TC_Loc };
                //+ By Max 20210305
                AutoAlarmLevel = "I";
                AutoAlarmCode = 42;

                ShowAlarm("I", 42, TC_Side, TC_LineNo.ToString(), TC_Loc);//"請選取下刀點檢查位置"
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            AutoFlowStep = 0;
            ShowAlarm("I", 41);//"請選取下刀點檢查位置"
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_下刀點確認_移至安全位置_Run()
        {
            if (SReadValue("AllAxisMoveSafe").ToBoolean())
            {
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                int posy = SReadValue("旋轉安全點位_Y").ToInt();
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, posy}
                };

                if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

                return FlowChart.FCRESULT.IDLE;
            }
        }

        private FlowChart.FCRESULT FC_下刀點確認_U軸移動_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, TC_U}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_下刀點確認_XYU軸移動_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, TC_X},
                {motor_切割平台前後馬達Y, TC_Y},
                {motor_切割平台旋轉馬達U, TC_U}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                //v4.0.1.41 開啟鏡頭遮蓋
                SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
                SetOutBit(outBit_高倍CCD鏡頭清潔, true);
                SetOutBit(outBit_高倍CCD視域範圍清潔, true);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_下刀點確認_完成_Run()
        {
            int posz = AutoMark.nHighBaseFocus;

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, posz}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                TC_Side = "";
                bUseManualAlignment = false;

                //+ By Max 20210314, v4.0.1.53, 調整Log紀錄位置
                LogSay(EnLoggerType.EnLog_SPC, "Auto-下刀點確認完成");

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_定位_手動定靶_Run()
        {
            DataView dv;
            int X = motor_切刀橫移馬達_Z2X.ReadEncPos();
            int Y = motor_切割平台前後馬達Y.ReadEncPos();

            int X_Gap = Math.Abs(X - AutoMark.nTempX);
            int Y_Gap = Math.Abs(Y - AutoMark.nTempY);

            if (X_Gap > PReadValue("Cut_X_tolerance").ToInt())
            {
                ShowAlarm("w", 39, "X Offset:" + X_Gap.ToString());
            }

            else if (Y_Gap > PReadValue("Cut_X_tolerance").ToInt())
            {
                ShowAlarm("w", 39, "Y Offset:" + Y_Gap.ToString());
            }

            else
            {
                AutoMark.nTempX = X;
                AutoMark.nTempY = Y;
                switch (m_enScanMethod)
                {
                    case enScanTargetMethod.即看即切:
                        if (AutoMark.nFindMarkStep == 0)
                        {
                            AutoMark.ptLBMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptLBMarkPos.Y = AutoMark.nTempY;
                        }
                        else
                        {
                            AutoMark.ptLTMarkPos.X = AutoMark.nTempX;
                            AutoMark.ptLTMarkPos.Y = AutoMark.nTempY;
                        }
                        break;
                    case enScanTargetMethod.四點掃靶:
                    case enScanTargetMethod.三點掃靶:
                        switch (AutoMark.nFindMarkStep)
                        {
                            case 0://CH2左上
                            case 4://CH1左上
                                AutoMark.ptLTMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptLTMarkPos.Y = AutoMark.nTempY;
                                break;
                            case 1://CH2左下
                            case 5://CH1左下
                                AutoMark.ptLBMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptLBMarkPos.Y = AutoMark.nTempY;
                                break;
                            case 2://CH2右上
                            case 6://CH1右上
                                AutoMark.ptRTMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptRTMarkPos.Y = AutoMark.nTempY;
                                break;
                            case 3://CH2右下
                            case 7://CH1右下
                                AutoMark.ptRBMarkPos.X = AutoMark.nTempX;
                                AutoMark.ptRBMarkPos.Y = AutoMark.nTempY;
                                break;
                        }
                        break;
                    case enScanTargetMethod.半靶掃靶:
                        dv = new DataView(LearnMarkPos.dtScanData);
                        dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                        if (AutoMark.nFindMarkStep < 2)
                        {
                            if (AutoMark.nFindMarkStep % 2 == 0)
                            {
                                TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                            else
                            {
                                BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                        }
                        else
                        {
                            if (AutoMark.nFindMarkStep <= dv.Count)//掃上半部
                            {
                                TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                        }
                        break;
                    case enScanTargetMethod.全靶掃靶_口字形:
                        dv = new DataView(LearnMarkPos.dtScanData);
                        dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
                        if (AutoMark.nFindMarkStep < 2)
                        {
                            if (AutoMark.nFindMarkStep % 2 == 0)
                            {
                                TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                            else
                            {
                                BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                        }
                        else
                        {
                            if (AutoMark.nFindMarkStep <= dv.Count)//先掃上半部，再掃下半部
                            {
                                TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                            else
                            {
                                BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                            }
                        }
                        break;
                    case enScanTargetMethod.全靶掃靶_N字形:
                        if (AutoMark.nFindMarkStep % 2 == 0)
                        {
                            TopPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                        else
                        {
                            BottomPoint.Add(new Point(AutoMark.nTempX, AutoMark.nTempY));
                        }
                        break;
                }

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.CASE1;
        }

        private FlowChart.FCRESULT FC_動態刀痕_開始_Run()
        {
            //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
            AutoFlowStepForCutOffset = 0;
            AutoMessage = new string[] { "" };
            //+ By Max 20210305
            AutoAlarmLevel = "";
            AutoAlarmCode = 0;

            SSetValue("Z1dynamicCutOffset", 0);
            SSetValue("Z2dynamicCutOffset", 0);
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_動態刀痕_關水_Run()
        {
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_切割區增濕水簾, false);
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_動態刀痕_打開鏡頭護蓋_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_動態刀痕_開啟視覺Live_Run()
        {
            return OpenVisionLive(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_動態刀痕_等待結果回傳_Run()
        {
            switch (mCCDAction)//等待Live
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.NG:
                case CCDSendResult.InspectFail:
                    //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                    AutoFlowStepForCutOffset = 0;
                    //+ By Max 20210305
                    //AutoMessage = "CCD Live NG";
                    AutoMessage = new string[] { "CCD Live NG" };
                    AutoAlarmLevel = "w";
                    AutoAlarmCode = 16;

                    ShowAlarm("w", 16);
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 0;
                //+ By Max 20210305
                //AutoMessage = "CCD Live Over Time";
                AutoMessage = new string[] { "CCD Live Over Time" };
                AutoAlarmLevel = "w";
                AutoAlarmCode = 17;

                ShowAlarm("w", 17);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_動態刀痕_NG_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_動態刀痕_檢查Z2_Run()
        {
            if (i切割_Z2_NowLineNo != -1)
            {
                AutoCut.TempX2 -= SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt();
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

        }

        private FlowChart.FCRESULT FC_動態刀痕_Z2是否補償_Run()
        {
            int Gap_X = AutoCut.TempX2 - motor_切刀橫移馬達_Z2X.ReadEncPos();
            if (AutoUserCheckOk)
            {
                if (Math.Abs(Gap_X) > PReadValue("CuttingShift").ToInt())
                {
                    //+ By Max 20210305
                    //AutoMessage = "Z2 Kerf Offset:" + Gap_X.ToString() + "is higher than" + PReadValue("CuttingShift").ToString();
                    AutoMessage = new string[] { "Z2", Gap_X.ToString(), PReadValue("CuttingShift").ToString() };
                    AutoAlarmLevel = "I";
                    AutoAlarmCode = 32;

                    ShowAlarm("I", 32, "Z2", Gap_X.ToString(), PReadValue("CuttingShift").ToString());
                }
                else
                {
                    //+ By Max 20210305
                    //AutoMessage = "Z2 Kerf Offet" + Gap_X.ToString() + "is saved";
                    AutoMessage = new string[] { Gap_X.ToString() };
                    AutoAlarmLevel = "I";
                    AutoAlarmCode = 33;

                    ShowAlarm("I", 33, "Z2", Gap_X.ToString());
                    int Z2_X = SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt();
                    SSetValue("Z2dynamicCutOffset", Gap_X);
                    SSetValue("Z2CCD與Z2切刀中心點距離_X", Z2_X + Gap_X);
                    SaveFile();
                }
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 0;
                AutoUserCheckOk = false;
                return FlowChart.FCRESULT.NEXT;
            }
            else if (AutoUserCheckSkip)
            {
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 0;
                AutoUserCheckSkip = false;
                //+ By Max 20210305
                //AutoMessage = "Choose to Ignore the Z2 Kerf Offset";
                AutoMessage = new string[] { "Choose to Ignore the Z2 Kerf Offset" };
                AutoAlarmLevel = "";
                AutoAlarmCode = 0;

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_動態刀痕_檢查Z1_Run()
        {
            if (i切割_Z1_NowLineNo != -1)
            {
                AutoCut.TempX1 -= SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_動態刀痕_Z1是否補償_Run()
        {
            int Gap_X = AutoCut.TempX1 - motor_切刀橫移馬達_Z2X.ReadEncPos();
            if (AutoUserCheckOk)
            {
                if (Math.Abs(Gap_X) > PReadValue("CuttingShift").ToInt())
                {
                    //+ By Max 20210305
                    //AutoMessage = "Z1 Kerf Offset:" + Gap_X.ToString() + "is higher than" + PReadValue("CuttingShift").ToString();
                    AutoMessage = new string[] { "Z1", Gap_X.ToString(), PReadValue("CuttingShift").ToString() };
                    AutoAlarmLevel = "I";
                    AutoAlarmCode = 32;

                    ShowAlarm("I", 32, "Z1", Gap_X.ToString(), PReadValue("CuttingShift").ToString());
                }
                else
                {
                    //+ By Max 20210305
                    //AutoMessage = "Z1 Kerf Offet" + Gap_X.ToString() + "is saved";
                    AutoMessage = new string[] { Gap_X.ToString() };
                    AutoAlarmLevel = "I";
                    AutoAlarmCode = 33;

                    ShowAlarm("I", 33, "Z1", Gap_X.ToString());
                    int Z1_X = SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();
                    SSetValue("Z1dynamicCutOffset", Gap_X);
                    SSetValue("Z2CCD與Z1切刀中心點距離_X", Z1_X + Gap_X);
                    SaveFile();
                }
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 0;
                AutoUserCheckOk = false;
                return FlowChart.FCRESULT.NEXT;
            }
            else if (AutoUserCheckSkip)
            {
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 0;
                AutoUserCheckSkip = false;
                //+ By Max 20210305
                //AutoMessage = "Choose to Ignore the Z1 Kerf Offset";
                AutoMessage = new string[] { "Choose to Ignore the Z1 Kerf Offset" };
                AutoAlarmLevel = "";
                AutoAlarmCode = 0;

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_動態刀痕_結束_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
            AutoFlowStepForCutOffset = 99;
            bWaterTurnOn = true;//做完動態刀痕，水需要再次開啟
            bdynamicCutOffset = false;
            //bdynamicCutOffset_run = false;
            //+ By Max 20210209, 動態刀痕頁面的完成按鈕來設定此變數，使其能將動態刀痕的CheckBox Enabled以及Uncheck
            //bdynamicCutOffsetFinish = true;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_Clean_CleanStepDone_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private void button_主軸啟動_Click_1(object sender, EventArgs e)
        {
            //v4.0.1.32 開關主軸時，判斷該主軸狀態
            if (m_enZ1SpindleStatus == enSpindleStatus.Stop && bUseZ1_Spindle)
                Z1SpindleStart = true;
            if (m_enZ2SpindleStatus == enSpindleStatus.Stop && bUseZ2_Spindle)
                Z2SpindleStart = true;
        }

        private void button_主軸關閉_Click_1(object sender, EventArgs e)
        {
            //v4.0.1.32 開關主軸時，判斷該主軸狀態
            if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                Z1SpindleStop = true;
            if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                Z2SpindleStop = true;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_開始_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            KerfCheckView = new DataTable();
            DataColumn[] kerfData_dc = new DataColumn[4];
            kerfData_dc[0] = new DataColumn("Kerf_Side", System.Type.GetType("System.String"));
            kerfData_dc[1] = new DataColumn("Kerf_Pos", System.Type.GetType("System.String"));
            kerfData_dc[2] = new DataColumn("Kerf_LineNo", System.Type.GetType("System.String"));
            kerfData_dc[3] = new DataColumn("Kerf_Shift", System.Type.GetType("System.String"));
            KerfCheckView.Columns.AddRange(kerfData_dc);

            KerfCheck.Reset();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_判斷掃描方式_Run()
        {

            DataRow dr = KerfCheck.dtScanData.NewRow();
            int Line = 0;

            DataView dv = new DataView(AutoMark.dtScanData);

            enCheckKerfSel enSel = (enCheckKerfSel)(OReadValue("ikerfcheckType").ToInt());

            switch (enSel)
            {
                case enCheckKerfSel.EDGE:
                    //CH2
                    dv.RowFilter = "Scan_Side = '" + "CH2" + "'";
                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = "2";
                    dr["Scan_Side"] = "CH2";
                    dr["Scan_X1"] = dv[1]["Scan_X1"];
                    dr["Scan_Y1"] = dv[1]["Scan_Y1"];
                    dr["Scan_X2"] = dv[1]["Scan_X2"];
                    dr["Scan_Y2"] = dv[1]["Scan_Y2"];
                    dr["Scan_Z"] = dv[1]["Scan_Z"];
                    dr["Scan_Angle"] = dv[1]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);

                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = (dv.Count - 1).ToString();
                    dr["Scan_Side"] = "CH2";
                    dr["Scan_X1"] = dv[dv.Count - 2]["Scan_X1"];
                    dr["Scan_Y1"] = dv[dv.Count - 2]["Scan_Y1"];
                    dr["Scan_X2"] = dv[dv.Count - 2]["Scan_X2"];
                    dr["Scan_Y2"] = dv[dv.Count - 2]["Scan_Y2"];
                    dr["Scan_Z"] = dv[dv.Count - 2]["Scan_Z"];
                    dr["Scan_Angle"] = dv[dv.Count - 2]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);

                    //CH1
                    dv.RowFilter = "Scan_Side = '" + "CH1" + "'";
                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = "2";
                    dr["Scan_Side"] = "CH1";
                    dr["Scan_X1"] = dv[1]["Scan_X1"];
                    dr["Scan_Y1"] = dv[1]["Scan_Y1"];
                    dr["Scan_X2"] = dv[1]["Scan_X2"];
                    dr["Scan_Y2"] = dv[1]["Scan_Y2"];
                    dr["Scan_Z"] = dv[1]["Scan_Z"];
                    dr["Scan_Angle"] = dv[1]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);

                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = (dv.Count - 1).ToString();
                    dr["Scan_Side"] = "CH1";
                    dr["Scan_X1"] = dv[dv.Count - 2]["Scan_X1"];
                    dr["Scan_Y1"] = dv[dv.Count - 2]["Scan_Y1"];
                    dr["Scan_X2"] = dv[dv.Count - 2]["Scan_X2"];
                    dr["Scan_Y2"] = dv[dv.Count - 2]["Scan_Y2"];
                    dr["Scan_Z"] = dv[dv.Count - 2]["Scan_Z"];
                    dr["Scan_Angle"] = dv[dv.Count - 2]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);

                    break;

                case enCheckKerfSel.DESIGNATED:
                    Line = OReadValue("ikerfCheckline").ToInt();
                    //CH2
                    dv.RowFilter = "Scan_Side = '" + "CH2" + "'";
                    if (Line > dv.Count - 1)
                    {
                        Line = dv.Count - 1;
                        OSetValue("ikerfCheckline", Line);
                    }
                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = Line.ToString();
                    dr["Scan_Side"] = "CH2";
                    dr["Scan_X1"] = dv[Line - 1]["Scan_X1"];
                    dr["Scan_Y1"] = dv[Line - 1]["Scan_Y1"];
                    dr["Scan_X2"] = dv[Line - 1]["Scan_X2"];
                    dr["Scan_Y2"] = dv[Line - 1]["Scan_Y2"];
                    dr["Scan_Z"] = dv[Line - 1]["Scan_Z"];
                    dr["Scan_Angle"] = dv[Line - 1]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);
                    //CH1
                    dv = new DataView(AutoMark.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + "CH1" + "'";
                    if (Line > dv.Count - 1)
                    {
                        Line = dv.Count - 1;
                        OSetValue("ikerfCheckline", Line);
                    }
                    dr = KerfCheck.dtScanData.NewRow();
                    dr["Scan_LineNo"] = Line.ToString();
                    dr["Scan_Side"] = "CH1";
                    dr["Scan_X1"] = dv[Line - 1]["Scan_X1"];
                    dr["Scan_Y1"] = dv[Line - 1]["Scan_Y1"];
                    dr["Scan_X2"] = dv[Line - 1]["Scan_X2"];
                    dr["Scan_Y2"] = dv[Line - 1]["Scan_Y2"];
                    dr["Scan_Z"] = dv[Line - 1]["Scan_Z"];
                    dr["Scan_Angle"] = dv[Line - 1]["Scan_Angle"];
                    dr["Scan_XOffset"] = "0";
                    KerfCheck.dtScanData.Rows.Add(dr);

                    break;

                case enCheckKerfSel.ALL:
                    //CH2
                    dv = new DataView(AutoMark.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + "CH2" + "'";
                    for (int i = 1; i < dv.Count - 1; i++)
                    {
                        dr = KerfCheck.dtScanData.NewRow();
                        dr["Scan_LineNo"] = i.ToString();
                        dr["Scan_Side"] = "CH2";
                        dr["Scan_X1"] = dv[i]["Scan_X1"];
                        dr["Scan_Y1"] = dv[i]["Scan_Y1"];
                        dr["Scan_X2"] = dv[i]["Scan_X2"];
                        dr["Scan_Y2"] = dv[i]["Scan_Y2"];
                        dr["Scan_Z"] = dv[i]["Scan_Z"];
                        dr["Scan_Angle"] = dv[i]["Scan_Angle"];
                        dr["Scan_XOffset"] = "0";
                        KerfCheck.dtScanData.Rows.Add(dr);
                    }
                    //CH1
                    dv = new DataView(AutoMark.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + "CH1" + "'";
                    for (int i = 1; i < dv.Count - 1; i++)
                    {
                        dr = KerfCheck.dtScanData.NewRow();
                        dr["Scan_LineNo"] = i.ToString();
                        dr["Scan_Side"] = "CH1";
                        dr["Scan_X1"] = dv[i]["Scan_X1"];
                        dr["Scan_Y1"] = dv[i]["Scan_Y1"];
                        dr["Scan_X2"] = dv[i]["Scan_X2"];
                        dr["Scan_Y2"] = dv[i]["Scan_Y2"];
                        dr["Scan_Z"] = dv[i]["Scan_Z"];
                        dr["Scan_Angle"] = dv[i]["Scan_Angle"];
                        dr["Scan_XOffset"] = "0";
                        KerfCheck.dtScanData.Rows.Add(dr);
                    }

                    break;
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_計算檢測位置_Run()
        {
            if (KerfCheck.nStep < KerfCheck.dtScanData.Rows.Count)
            {
                KerfCheck.nFindMarkStep = 0;//前、中、後
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_刀痕檢測_移至檢測位置_Run()
        {
            int nPosU = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Angle"].ToString());
            int nPosZ = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Z"].ToString());
            int nPosX = 0;
            int nPosY = 0;
            switch (KerfCheck.nFindMarkStep)
            {
                case 0:
                    nPosX = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_X1"].ToString());
                    nPosY = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Y1"].ToString());
                    break;
                case 1:
                    nPosX = (Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_X1"].ToString()) + Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_X2"].ToString())) / 2;
                    nPosY = (Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Y1"].ToString()) + Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Y2"].ToString())) / 2;
                    break;
                case 2:
                    nPosX = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_X2"].ToString());
                    nPosY = Convert.ToInt32(KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Y2"].ToString());
                    break;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, nPosX},
                {motor_切割平台前後馬達Y, nPosY},
                {motor_切割平台旋轉馬達U, nPosU},
                {motor_切刀上下馬達_Z2, nPosZ}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_視覺通訊_Run()
        {
            //HMIK,Sh,Pos,KerfOffset;
            //Sh為片數索引,Pos為位置索引
            string cmd = "HMIK";
            string index = PCBIncount.ToString() + "," + KerfCheck.nFindMarkStep.ToString() + "," + PReadValue("CuttingShift").ToString();

            if (SetCCDCmd(cmd, index))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            else if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 45);
                Timer_Auto.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private bool CCDResultParse(string strTarget, ref int nVal)
        {
            foreach (string str in CCDResult)
            {
                if (str.Contains(strTarget))
                {
                    string[] strResArr = str.Split('=');
                    double dVal;

                    if (!Double.TryParse(strResArr[1], out dVal))
                    {
                        return false;
                    }
                    else
                    {
                        nVal = (int)dVal;
                        return true;
                    }
                }
            }

            return false;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_等待回傳結果_Run()
        {
            //HMIK,1,Sh,Pos,Shift=?,Width=?,MaxWidth=?,ChippingArea=?,ChippingSize=?,ChippingHalfWidth=?,LeftDistance=?,RightDistance=?;
            //HMIK,0,Sh,Pos,#ERR;
            //1:OK
            //Shift:切割道中心與視域中心的距離
            //Width:切割道寬
            //MaxWidth:切割道最大寬度
            //ChippingArea:Chipping面積
            //ChippingSize:Chipping尺寸
            //ChippingHalfWidth:
            //LeftDistance:左邊切割道與圓球的距離
            //RightDistance:右邊切割道與圓球的距離
            //0:NG
            //#ERR:未找到切割道
            switch (mCCDAction)//等待切割道辨識
            {
                case CCDSendResult.OK:
                    bool bRes = true;
                    int Shift = 0;
                    int Width = 0;
                    int LeftDistance = 0;
                    int RightDistance = 0;

                    do
                    {
                        bRes = CCDResultParse("Shift", ref Shift);
                        if (!bRes) break;

                        bRes = CCDResultParse("Width", ref Width);
                        if (!bRes) break;

                        bRes = CCDResultParse("LeftDistance", ref LeftDistance);
                        if (!bRes) break;

                        bRes = CCDResultParse("RightDistance", ref RightDistance);
                        if (!bRes) break;

                    } while(false);

                    if (bRes)
                    {
                        int ToolWidth = (int)(PReadValue("BladeZ1Data", "dWidthZ1").ToDouble() * 1000);
                        int CuttingWidthLimit = PReadValue("CuttingWidth").ToInt();
                        int CuttingShiftLimit = PReadValue("CuttingShift").ToInt();

                        int nBallToEdgeDist = PReadValue("BallToEdgeDist").ToInt();
                        int nBallToEdgeLimit = PReadValue("BallToEdgeLimit").ToInt();

                        //+ By Max 20210331, v4.0.1.60, 刀痕偏移超出卡控警報
                        if (Shift > CuttingShiftLimit)
                        {
                            ShowAlarm("E", 94, Math.Abs(Shift), CuttingShiftLimit);
                        }

                        //+ By Max 20210331, v4.0.1.60, 刀寬值超出卡控警報
                        if (Width > (ToolWidth + CuttingWidthLimit))
                        {
                            ShowAlarm("E", 95, Math.Abs(Width), (ToolWidth + CuttingWidthLimit));
                        }

                        #region By Wolf 20210816, v4.0.1.91, 球到邊距離超出球到邊警報
                        if (LeftDistance > (nBallToEdgeDist + nBallToEdgeLimit))
                        {
                            ShowAlarm("E", 96, Math.Abs(LeftDistance), (nBallToEdgeDist + nBallToEdgeLimit));
                        }

                        if (RightDistance > (nBallToEdgeDist + nBallToEdgeLimit))
                        {
                            ShowAlarm("E", 96, Math.Abs(RightDistance), (nBallToEdgeDist + nBallToEdgeLimit));
                        }
                        #endregion

                        DataRow dr = KerfCheckView.NewRow();
                        //+ By Max 20210330, v4.0.1.60
                        sb.Clear();
                        dr["Kerf_Side"] = KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Side"].ToString();
                        sb.Append(String.Format("Kerf_Side: {0},", KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_Side"].ToString()));
                        dr["Kerf_Pos"] = KerfCheck.nFindMarkStep.ToString();
                        sb.Append(String.Format("Kerf_Pos: {0},", KerfCheck.nFindMarkStep.ToString()));
                        dr["Kerf_LineNo"] = KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_LineNo"].ToString();
                        sb.Append(String.Format("Kerf_LineNo: {0},", KerfCheck.dtScanData.Rows[KerfCheck.nStep]["Scan_LineNo"].ToString()));
                        dr["Kerf_Shift"] = Shift.ToString();
                        sb.Append(String.Format("Kerf_Shift: {0},", Shift.ToString()));
                        sb.Append(String.Format("LeftDis: {0},", LeftDistance));
                        sb.Append(String.Format("RightDis: {0},", RightDistance));
                        LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                        //----------------------------------------------------------
                        KerfCheckView.Rows.Add(dr);
                        KerfCheckRetryCount = 0;

                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else
                    {
                        KerfCheckRetryCount++;
                        if (KerfCheckRetryCount >= SReadValue("iVisionRetryCount").ToInt())
                        {
                            ShowAlarm("w", 46);
                            KerfCheckRetryCount = 0;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.NEXT;//超出重試次數，警報
                        }
                        else
                        {
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.CASE1;//重試
                        }
                    }

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    KerfCheckRetryCount++;
                    if (KerfCheckRetryCount >= SReadValue("iVisionRetryCount").ToInt())
                    {
                        ShowAlarm("w", 46);
                        KerfCheckRetryCount = 0;
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;//超出重試次數，警報
                    }
                    else
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;//重試
                    }
            }
            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 47);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_刀痕檢測_NG_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_刀痕檢測_資料儲存_Run()
        {
            KerfCheck.nFindMarkStep++;
            if (KerfCheck.nFindMarkStep <= 2)
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                KerfCheck.nStep++;
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_刀痕檢測_結束_Run()
        {
            m_KerfCheckFinishEvt.Set();
            return default(FlowChart.FCRESULT);
        }

        private void Teach_timer_Tick(object sender, EventArgs e)
        {
            FC_教導執行_動作開始.MainRun();
        }

        private FlowChart.FCRESULT FC_下刀點確認_視覺通訊FM_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 28);
                Timer_Auto.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_下刀點確認_等待結果_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:
                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);
                    //+ By Max 20210305
                    //AutoMessage = "X Offset" + xoffset.ToString() + ",Y Offset" + yoffset.ToString();
                    AutoMessage = new string[] { xoffset.ToString(), yoffset.ToString() };
                    AutoAlarmLevel = "I";
                    AutoAlarmCode = 74;
                    ShowAlarm("I", 74, xoffset, yoffset);

                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        //+ By Max 20210305
                        //AutoMessage = "Find Mark Fail";
                        AutoMessage = new string[] { "Find Mark Fail" };
                        AutoAlarmLevel = "w";
                        //+ By Max 20210310, 修改錯誤碼，20 改 52
                        AutoAlarmCode = 52;

                        // +  By Max 20210310, 不發警報
                        //ShowAlarm("w", 52);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
            }
            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 21);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_動態刀痕_XYZ移動_Z2_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, AutoCut.TempX2},
                {motor_切刀上下馬達_Z2, AutoMark.nHighBaseFocus},
                {motor_切割平台前後馬達Y, SReadValue("圓心點位_Y").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 1;
                AutoUserCheckOk = false;
                //+ By Max 20210305
                //AutoMessage = "Z2";// "Please Check the Kerf Of Z2 and Click Next";
                AutoMessage = new string[] { "Z2" };
                AutoAlarmLevel = "I";
                AutoAlarmCode = 43;

                ShowAlarm("I", 43, "Z2");

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_動態刀痕_XYZ移動_Z1_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, AutoCut.TempX1},
                {motor_切刀上下馬達_Z2, AutoMark.nHighBaseFocus},
                {motor_切割平台前後馬達Y, SReadValue("圓心點位_Y").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 1;
                AutoUserCheckOk = false;
                //+ By Max 20210305
                //AutoMessage = "Z1";// "Please Check the Kerf Of Z1 and Click Next";
                AutoMessage = new string[] { "Z1" };
                AutoAlarmLevel = "I";
                AutoAlarmCode = 43;

                ShowAlarm("I", 43, "Z1");

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region Handler通訊 v4.0.1.8 By Woody 新增Handler通訊
        bool bConnectHandler = false;
        string sCommBuffer = "";

        bool bPutStripFinish = false;
        bool bPutLeaveFinish = false;
        bool bGetStripFinish = false;
        bool bGetLeaveFinish = false;
        bool bHandlerError = false;
        bool bONvacOFFdes = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
        bool bOFFvacONdes = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
        bool bPickAbort = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄

        bool bGetHandlerPackageNameOK = false;
        string sHandlerPackageName = "";

        bool bCompensationTransferOK = false;
        bool bSawCanHome = false;
        bool bSawCanLotEnd = false;
        int iLowerCCDCompensation_X = 0;
        int iLowerCCDCompensation_Y = 0;
        int iLowerCCDCompensation_θ = 0;

        //+ By Max 20210316, v4.0.1.55, Vision LoadJob Failed
        bool bLoadJobSkipFirstTime = true;

        private MyTimer Socket_Timer = new MyTimer();

        private void proVClientSocket_HandlerConnection_OnConnect(object sender)
        {
            sCommBuffer = "";
            bConnectHandler = true;
        }

        private void proVClientSocket_HandlerConnection_OnDisconnect(object sender, System.Net.Sockets.Socket socket)
        {
            sCommBuffer = "";
            bConnectHandler = false;
            LogSay(EnLoggerType.EnLog_SPC, "Disconnect...");
        }

        private void proVClientSocket_HandlerConnection_OnError(object sender, int errorCode, string ErrMsg)
        {
            MessageBox.Show(ErrMsg);
            //dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", ErrMsg);
        }

        private void proVClientSocket_HandlerConnection_OnRead(ProVTool.SocketClient sender)
        {
            byte[] bResult = sender.ReadBuf();

            if (bResult.Length > 0)
            {
                sCommBuffer += Encoding.UTF8.GetString(bResult);
                LogSay(EnLoggerType.EnLog_SPC, sCommBuffer);
                if (sCommBuffer.Length > 1000) //所收到的字元超過 1000 個字
                {
                    string str = CreateDialogMsgContent("Text_Saw_Receive_Command_Too_Long");
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                    //MessageBox.Show("通訊收取命令太長 (>1000) !");
                    sCommBuffer = "";
                }
                else
                    AnalysisCommand(sCommBuffer);
            }
            bResult = null;
        }

        private void AnalysisCommand(string sData)
        {
            int i;

            for (i = 0; i < sData.Length - 1; i++)
                if (sData.Substring(i, 1) == "\r" && sData.Substring(i + 1, 1) == "\n")
                    break;
            if (i < sData.Length - 1)
            {
                CommandProcess(sData.Substring(0, i));
                if (i < sData.Length - 2)
                {
                    sCommBuffer = sData.Substring(i + 2);
                    AnalysisCommand(sCommBuffer);
                }
                else
                    sCommBuffer = "";
            }
        }
        private void CommandProcess(string sCommand)
        {
            string[] sData = sCommand.Split(new char[] { '_' });

            switch (sData.Length)
            {
                case 2: //未附帶參數指令
                    switch (sData[1])
                    {
                        case "HandlerError":    //Handler 有 Error 
                            bHandlerError = true;
                            break;
                        case "PutStripFinish":
                            bPutStripFinish = true;
                            break;
                        case "PutLeaveFinish":
                            bPutLeaveFinish = true;
                            break;
                        case "GetStripFinish":
                            bGetStripFinish = true;
                            break;
                        case "GetLeaveFinish":
                            bGetLeaveFinish = true;
                            break;
                        case "SawCanHome":
                            bSawCanHome = true;
                            break;
                        case "SawCanLotEnd":
                            bSawCanHome = true;
                            break;
                        case "ONvacOFFdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞
                            bONvacOFFdes = true;
                            break;
                        case "OFFvacONdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，關真空、開破壞
                            bOFFvacONdes = true;
                            break;
                        case "PickAbort":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，放棄
                            bPickAbort = true;
                            break;
                        case "AutoStart":
                            ResetAllFlag();
                            break;
                        default:
                            //MessageBox.Show("未定義的指令 : \"" + sCommand + "\" , 請洽程式人員 !!!");
                            string str1 = CreateDialogMsgContent("Text_Saw_CommandProcess_Undefined1");
                            string str2 = CreateDialogMsgContent("Text_Saw_CommandProcess_Undefined2");
                            dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str1 + sCommand + str2);
                            break;
                    }
                    break;
                case 3://附帶參數指令
                    string[] sPara = sData[2].Split(new char[] { ',' });

                    switch (sData[1])
                    {
                        case "LoadOffset":
                            if (sPara.Length == 3)
                            {
                                iLowerCCDCompensation_X = Convert.ToInt32(sPara[0]);
                                iLowerCCDCompensation_Y = Convert.ToInt32(sPara[1]);
                                iLowerCCDCompensation_θ = Convert.ToInt32(sPara[2]);
                                bCompensationTransferOK = true;
                            }
                            else
                            {
                                //MessageBox.Show("LowerCCDCompensation 資料格式有問題 , 請洽程式人員 !!!");
                                string str = CreateDialogMsgContent("Text_Saw_LowerCCDCompensation_DataFormat_Error");
                                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                            }
                            break;
                        case "SetPackageName":
                            if (sPara.Length == 1)
                            {
                                sHandlerPackageName = sPara[0];
                                bGetHandlerPackageNameOK = true;
                            }
                            else
                            {
                                //MessageBox.Show("SetPackageName 資料格式有問題 , 請洽程式人員 !!!");
                                string str = CreateDialogMsgContent("Text_Saw_SetPackageName_DataFormat_Error");
                                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                            }
                            break;
                        case "Vacuum":
                            if (sPara.Length == 1)
                            {
                                if (sPara[0] == "1")
                                    outBit_切割平台真空建立.On();
                                else
                                    outBit_切割平台真空建立.Off();
                            }
                            else
                            {
                                //MessageBox.Show("Vacuum Action 資料格式有問題 , 請洽程式人員 !!!");
                                string str = CreateDialogMsgContent("Text_Saw_VacuumAction_DataFormat_Error");
                                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                            }
                            break;
                        case "Destroy":
                            if (sPara.Length == 1)
                            {
                                if (sPara[0] == "1")
                                    outBit_切割平台真空破壞.On();
                                else
                                    outBit_切割平台真空破壞.Off();
                            }
                            else
                            {
                                //MessageBox.Show("Destroy Action 資料格式有問題 , 請洽程式人員 !!!");
                                string str = CreateDialogMsgContent("Text_Saw_DestroyAction_DataFormat_Error");
                                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str);
                            }
                            break;
                        default:
                            //MessageBox.Show("未定義的指令 : \"" + sCommand + "\" , 請洽程式人員 !!!");
                            string str1 = CreateDialogMsgContent("Text_Saw_CommandProcess_Undefined1");
                            string str2 = CreateDialogMsgContent("Text_Saw_CommandProcess_Undefined2");
                            dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str1 + sCommand + str2);
                            break;
                    }
                    break;
                default:
                    //MessageBox.Show("指令格式有問題 , 請洽程式人員 !!!");
                    string strTmp = CreateDialogMsgContent("Text_Saw_CommandFormat_Error");
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", strTmp);
                    break;
            }
            sData = null;
        }

        public void TCPIPConnect()
        {
            if (!bConnectHandler)
            {
                try
                {
                    proVClientSocket_HandlerConnection.IP = SReadValue("HandlerIP").ToString();
                    proVClientSocket_HandlerConnection.Port = SReadValue("HandlerPort").ToInt();
                    proVClientSocket_HandlerConnection.Connect();
                }
                catch (Exception e)
                {
                    LogSay(EnLoggerType.EnLog_ARM, e.Message);
                }
            }
        }

        public void TCPIPClose()
        {
            if (bConnectHandler)
            {
                bConnectHandler = false;
                proVClientSocket_HandlerConnection.Disconnect();
            }
        }

        public bool IsConnected()
        {
            if (proVClientSocket_HandlerConnection.Socket != null)
                return proVClientSocket_HandlerConnection.Socket.Sock.Connected;
            else
                return false;
        }

        public void ResetAllFlag()
        {
            bPutStripFinish = false;
            bPutLeaveFinish = false;
            bGetStripFinish = false;
            bGetLeaveFinish = false;
            bHandlerError = false;

            bONvacOFFdes = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
            bOFFvacONdes = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
            bPickAbort = false;   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄

            bGetHandlerPackageNameOK = false;
            sHandlerPackageName = "";

            bCompensationTransferOK = false;

            bSawCanHome = false;
            iLowerCCDCompensation_X = 0;
            iLowerCCDCompensation_Y = 0;
            iLowerCCDCompensation_θ = 0;

            bCanPutRunSate = false;
            bCanGetRunSate = false;
            bCanFrontGetRun = false;

            outBit_ManualLoadOK.Off();//手動入料完成

            //+ By Max 20210316, v4.0.1.55, Vision LoadJob Failed
            bLoadJobSkipFirstTime = true;
        }

        private object locker = new object();
        public void SendData(string sKeyword)
        {
            lock (locker)
            {
                string sTotalData = "";
                byte[] bTemp = null;
                byte[] bTotalData = null;
                string strCase = sKeyword;
                if (sKeyword.IndexOf("SawError") >= 0)
                    strCase = "SawError";

                switch (strCase)
                {
                    case "AutoStart":
                    case "SawError":
                    case "ForceOut":
                        sTotalData = "S-H_" + sKeyword; ;
                        break;
                    case "CanPutStrip":
                    case "PutCanLeave":
                    case "LoadStrip": ////+ By Max 20170410 Load Strip
                        sTotalData = "S-HL_" + sKeyword;
                        break;
                    case "CanGetStrip":
                    case "GetCanLeave":
                        sTotalData = "S-HR_" + sKeyword;
                        break;
                    default:
                        string str = CreateDialogMsgContent("Text_Saw_SendData_Error");
                        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str + sKeyword + ")");
                        //MessageBox.Show("SendData 錯誤的呼叫 !!! (" + sKeyword + ")");
                        return;
                }
                bTemp = Encoding.UTF8.GetBytes(sTotalData);
                bTotalData = new byte[bTemp.Length + 2];
                Array.Copy(bTemp, 0, bTotalData, 0, bTemp.Length);
                bTotalData[bTotalData.Length - 2] = 13;
                bTotalData[bTotalData.Length - 1] = 10;
                if (SReadValue("ConnectHandler").ToInt() > 0)
                {
                    if (proVClientSocket_HandlerConnection.Socket != null)
                    {
                        if (proVClientSocket_HandlerConnection.Socket.Sock.Connected)
                        {
                            proVClientSocket_HandlerConnection.Socket.SendBuf(bTotalData);
                        }
                    }
                }
                bTemp = null;
                bTotalData = null;
            }
        }

        public object GetResult(string sStatus)
        {   //Handler 丟給 Saw , Saw 要取得結果 
            object sRet;

            switch (sStatus)
            {
                case "HandlerError":
                    sRet = bHandlerError;
                    break;
                case "PutStripFinish":
                    sRet = bPutStripFinish;
                    break;
                case "PutLeaveFinish":
                    sRet = bPutLeaveFinish;
                    break;
                case "GetStripFinish":
                    sRet = bGetStripFinish;
                    break;
                case "GetLeaveFinish":
                    sRet = bGetLeaveFinish;
                    break;
                case "PackageOK":
                    sRet = bGetHandlerPackageNameOK;
                    break;
                case "PackageName":
                    sRet = sHandlerPackageName;
                    break;
                case "LoadOffset":   //v0.0.7.11 By Sanxiu CompensationTransferOK改LoadOffset
                    sRet = bCompensationTransferOK;
                    break;
                case "CompensationX":
                    sRet = iLowerCCDCompensation_X;
                    break;
                case "CompensationY":
                    sRet = iLowerCCDCompensation_Y;
                    break;
                case "Compensationθ":
                    sRet = iLowerCCDCompensation_θ;
                    break;
                case "SawCanHome":
                    sRet = bSawCanHome;
                    break;
                case "SawCanLotEnd":
                    sRet = bSawCanLotEnd;
                    break;
                case "ONvacOFFdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    sRet = bONvacOFFdes;
                    break;
                case "OFFvacONdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    sRet = bOFFvacONdes;
                    break;
                case "PickAbort":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    sRet = bPickAbort;
                    break;
                default:
                    sRet = null;
                    //MessageBox.Show("GetResult 錯誤的呼叫 !!! (" + sStatus + ")");
                    string str = CreateDialogMsgContent("Text_Saw_GetResult_Error");
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str + sStatus + ")");
                    break;
            }
            return sRet;
        }

        public void ResetResult(string sStatus)
        {   //Saw 重置 Handler 丟來的結果
            switch (sStatus)
            {
                case "HandlerError":
                    bHandlerError = false;
                    break;
                case "PutStripFinish":
                    bPutStripFinish = false;
                    break;
                case "PutLeaveFinish":
                    bPutLeaveFinish = false;
                    break;
                case "GetStripFinish":
                    bGetStripFinish = false;
                    break;
                case "GetLeaveFinish":
                    bGetLeaveFinish = false;
                    break;
                case "PackageOK":
                    bGetHandlerPackageNameOK = false;
                    break;
                case "PackageName":
                    sHandlerPackageName = "";
                    break;
                case "LoadOffset":   //v0.0.7.11 By Sanxiu CompensationTransferOK改LoadOffset
                    bCompensationTransferOK = false;
                    break;
                case "CompensationX":
                    iLowerCCDCompensation_X = 0;
                    break;
                case "CompensationY":
                    iLowerCCDCompensation_Y = 0;
                    break;
                case "Compensationθ":
                    iLowerCCDCompensation_θ = 0;
                    break;
                case "SawCanHome":
                    bSawCanHome = false;
                    break;
                case "SawCanLotEnd":
                    bSawCanLotEnd = false;
                    break;
                case "ONvacOFFdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    bONvacOFFdes = false;
                    break;
                case "OFFvacONdes":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    bOFFvacONdes = false;
                    break;
                case "PickAbort":   //v0.0.7.7 By Sanxiu HadOut退料失敗流程，開真空、關破壞、放棄
                    bPickAbort = false;
                    break;
                default:
                    //MessageBox.Show("ResetResult 錯誤的呼叫 !!! (" + sStatus + ")");
                    string str = CreateDialogMsgContent("Text_Saw_ResetResult_Error");
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", str + sStatus + ")");
                    break;
            }
        }

        #endregion Handler通訊

        #region 生產自動覆位
        //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
        public bool GetAutoReMoveToRunPos()
        {
            return SReadValue("bAutoReMoveToRunPos").ToBoolean();
        }

        private bool bUseAutoReMove = false;

        private bool bReMoveToRunPos = false;   //是否自動回到生產位置，true會回生產位置，false代表回位完成

        public bool GetReMoveToRunPos()
        {
            return bReMoveToRunPos;
        }

        public bool SetReMoveToRunPos(bool bSet)
        {
            if (bSet)
            {
                bReMoveToRunPos = !m_bIsPauseNeedToContinue;
            }
            else
            {
                bReMoveToRunPos = bSet;
            }

            if (bReMoveToRunPos)
            {
                flowReMovePos_Start.TaskReset();
            }

            return bReMoveToRunPos;
        }

        public bool bReMoveToRunPosOK = false;   //完成覆位動作

        struct MotorPra
        {
            public int iSPD;
            public int iACC;
            public int iDAC;
        }

        MotorPra[] ReMotorPra;

        private int iReMotorPraStep = 0;

        /// <summary>
        /// 取得所有馬達當下的速度
        /// </summary>
        /// <param name="Reset"></param>
        /// <returns></returns>
        public bool GetReMotorPra(bool Reset)
        {
            bool bResult = false;
            if (Reset)
            {
                iReMotorPraStep = 10;
                return bResult;
            }

            switch (iReMotorPraStep)
            {
                case 0:
                    break;
                case 10:
                    ReMotorPra = new MotorPra[MotorList.Count];
                    iReMotorPraStep = 20;
                    break;
                case 20:
                    for (int i = 0; i < MotorList.Count; i++)
                    {
                        ReMotorPra[i].iSPD = MotorList[i].Speed;
                        ReMotorPra[i].iACC = MotorList[i].Acceleration;
                        ReMotorPra[i].iDAC = MotorList[i].Deceleration;
                    }
                    bResult = true;
                    iReMotorPraStep = 0;
                    break;
            }
            return bResult;
        }

        /// <summary>
        /// 把速度還原給馬達
        /// </summary>
        public void SetReMotorPra()
        {
            int iCount = MotorList.Count;
            for (int i = 0; i < iCount; i++)
            {
                MotorList[i].Speed = ReMotorPra[i].iSPD;
                MotorList[i].Acceleration = ReMotorPra[i].iACC;
                MotorList[i].Deceleration = ReMotorPra[i].iDAC;
            }
        }

        private int iBackMotorZ1X = 0;
        private int iBackMotorZ2X = 0;
        private int iBackMotorZ1 = 0;
        private int iBackMotorZ2 = 0;
        private int iBackMotorY = 0;
        private int iBackMotorU = 0;

        private bool bBackMotorXok = false;
        private bool bBackMotorZok = false;
        private bool bBackMotorYok = false;
        private bool bBackMotorUok = false;

        private int iReMotorPosStep = 0;
        public bool GetReMotorPos(bool Reset)
        {
            bool bResult = false;
            if (Reset)
            {
                iReMotorPosStep = 10;
                return bResult;
            }
            switch (iReMotorPosStep)
            {
                case 0:
                    break;
                case 10:
                    if (motor_切刀上下馬達_Z1.Busy() == false)
                    {
                        iBackMotorZ1 = motor_切刀上下馬達_Z1.ReadPos();
                        iReMotorPosStep = 20;
                    }
                    break;
                case 20:
                    if (motor_切刀上下馬達_Z2.Busy() == false)
                    {
                        iBackMotorZ2 = motor_切刀上下馬達_Z2.ReadPos();
                        iReMotorPosStep = 30;
                    }
                    break;
                case 30:
                    if (motor_切刀橫移馬達_Z1X.Busy() == false)
                    {
                        iBackMotorZ1X = motor_切刀橫移馬達_Z1X.ReadPos();
                        iReMotorPosStep = 40;
                    }
                    break;
                case 40:
                    if (motor_切刀橫移馬達_Z2X.Busy() == false)
                    {
                        iBackMotorZ2X = motor_切刀橫移馬達_Z2X.ReadPos();
                        iReMotorPosStep = 50;
                    }
                    break;
                case 50:

                    if (motor_切割平台前後馬達Y.Busy() == false)
                    {
                        iBackMotorY = motor_切割平台前後馬達Y.ReadPos();
                        iReMotorPosStep = 60;
                    }
                    break;
                case 60:

                    if (motor_切割平台旋轉馬達U.Busy() == false)
                    {
                        iBackMotorU = motor_切割平台旋轉馬達U.ReadPos();
                        bResult = true;
                        iReMotorPosStep = 0;
                    }
                    break;

            }
            return bResult;
        }

        private FlowChart.FCRESULT flowReMovePos_ReSpd_Run()
        {
            if (IsSimulation() != 0)
            {
                ShowAlarm("I", 12, "位置覆位開始");
            }

            SetReMotorPra();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT flowReMovePos_ZChk_Run()
        {
            int iReadZ1 = motor_切刀上下馬達_Z1.ReadPos();
            int iReadZ2 = motor_切刀上下馬達_Z2.ReadPos();

            bool bZ1OK = Math.Abs(iReadZ1 - iBackMotorZ1) < 10;
            bool bZ2OK = Math.Abs(iReadZ2 - iBackMotorZ2) < 10;

            if (bZ1OK && bZ2OK)
            {
                bBackMotorZok = true;
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                bBackMotorZok = false;
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT flowReMovePos_ZRemove_Run()
        {
            bool bZ1OK = motor_切刀上下馬達_Z1.G00(iBackMotorZ1);
            bool bZ2OK = motor_切刀上下馬達_Z2.G00(iBackMotorZ2);

            if (bZ1OK && bZ2OK)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT flowReMovePos_XChk_Run()
        {
            int iReadZ1X = motor_切刀橫移馬達_Z1X.ReadPos();
            int iReadZ2X = motor_切刀橫移馬達_Z2X.ReadPos();

            bool bZ1xOK = Math.Abs(iReadZ1X - iBackMotorZ1X) < 10;
            bool bZ2xOK = Math.Abs(iReadZ2X - iBackMotorZ2X) < 10;

            if (bZ1xOK && bZ2xOK)
            {
                bBackMotorXok = true;
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                bBackMotorXok = false;
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT flowReMovePos_XRemove_Run()
        {
            bool bZ1xOK = motor_切刀橫移馬達_Z1X.G00(iBackMotorZ1X);
            bool bZ2xOK = motor_切刀橫移馬達_Z2X.G00(iBackMotorZ2X);

            if (bZ1xOK && bZ2xOK)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT flowReMovePos_UChk_Run()
        {
            int iReadU = motor_切割平台旋轉馬達U.ReadPos();

            if (Math.Abs(iReadU - iBackMotorU) < 10)
            {
                bBackMotorUok = true;
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                bBackMotorUok = false;
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT flowReMovePos_URemove_Run()
        {
            bool bYok = motor_切割平台前後馬達Y.G00(SReadValue("旋轉安全點位_Y").ToInt());
            if (bYok)
            {
                if (motor_切割平台旋轉馬達U.G00(iBackMotorU))
                {
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT flowReMovePos_YChk_Run()
        {
            int iReadY = motor_切割平台前後馬達Y.ReadPos();

            if (Math.Abs(iReadY - iBackMotorY) < 10)
            {
                bBackMotorYok = true;
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                bBackMotorYok = false;
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT flowReMovePos_YRemove_Run()
        {
            if (motor_切割平台前後馬達Y.G00(iBackMotorY))
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT flowReMovePos_Finish_Run()
        {
            if ((bBackMotorXok && bBackMotorZok && bBackMotorYok && bBackMotorUok) == false)
            {
                ShowAlarm("I", 12, "位置覆位完成");
            }

            SetReMoveToRunPos(false);
            return FlowChart.FCRESULT.IDLE;
        }
        #endregion  生產自動覆位

        #region 出入料IO防呆
        //20180608 v0.0.7.4 出入料IO防呆
        private bool bCanPutRunSate = false;   //保存當下是否要入料的狀態，然後更新給OutPut
        private bool bCanGetRunSate = false;   //保存當下是否要出料的狀態，然後更新給OutPut
        private bool bCanFrontGetRun = false;  //v0.0.7.12 By Sanxiu 出料提早偷跑

        /// <summary>
        /// 設定可以入料狀況，由MainFlow呼叫
        /// /// </summary>
        /// <param name="bSet"></param>bSet等於true可入料
        public void SetPutRunSata(bool bSet)
        {
            outBit_CanPutRun.Value = (bSet && bCanPutRunSate && (bReMoveToRunPos == false));
        }
        /// <summary>
        /// 設定可以出料狀況，由MainFlow呼叫
        /// /// </summary>
        /// <param name="bSet"></param>bSet等於true可出料
        public void SetGetRunSata(bool bSet)
        {
            //v0.5.7.29 By Sanxiu 機台暫停後再開始，自動回復生產位置
            outBit_CanGetRun.Value = (bSet && bCanGetRunSate && (bReMoveToRunPos == false));
        }
        public void SetFrontGetRun(bool bSet)
        {
            outBit_CanFrontGetRun.Value = (bSet && bCanFrontGetRun && (bReMoveToRunPos == false));
        }
        //v4.0.1.21 讀取入料狀態
        public bool ShowPutState()
        {
            return outBit_CanPutRun.Value;
        }
        //v4.0.1.21 讀取出料狀態
        public bool ShowGetState()
        {
            return outBit_CanGetRun.Value;
        }

        //v0.0.7.10 By Sanxiu  L/UL安全偵測，當執行L/UL動作，Y軸不能忙錄
        private int iMoveHadinY = 0;
        private int iMoveHadinU = 0;
        private int iMoveInitialU = 0;
        private int iMoveHadoutY = 0;
        private int iMoveHadoutU = 0;
        private int iChkSawTableType = 0;
        public bool IsSawTableSafePos(int itype)   //iType為0代表hadin入料，檢查是否在入料點位。為1代表hadOut出料，檢查是否在出料點位
        {
            bool b1 = false;
            bool b2 = false;

            if (itype == 0)
            {
                //入料檢查
                b1 = (System.Math.Abs(motor_切割平台前後馬達Y.ReadPos() - iMoveHadinY) > 100);
                b2 = (System.Math.Abs(motor_切割平台旋轉馬達U.ReadPos() - iMoveHadinU) > 100);
            }
            else
            {
                //出料檢查
                b1 = (System.Math.Abs(motor_切割平台前後馬達Y.ReadPos() - iMoveHadoutY) > 100);
                b2 = (System.Math.Abs(motor_切割平台旋轉馬達U.ReadPos() - iMoveHadoutU) > 100);
            }
            return (b1 || b2);
        }

        //v0.0.7.10 By Sanxiu  L/UL安全偵測，當執行L/UL動作，Y軸不能忙錄
        public bool GetSawTablbeChkSignal()
        {
            return (inBit_HADIN安全訊號.Value || inBit_HADOUT安全訊號.Value) && !inBit_PowerOn.Value;
        }

        //v0.0.7.10 By Sanxiu  L/UL安全偵測，當執行L/UL動作，Y軸不能忙錄
        public bool GetSawTablbeChkHadinSignal()
        {
            return inBit_HADIN安全訊號.Value && !inBit_PowerOn.Value;
        }

        //+ By Max 20210329, v4.0.1.59
        public bool IsHADINInProcess()
        {
            //+ By Max 20210331, v4.0.1.60, 改Not判斷
            if (!inBit_PowerOn.Value)
                return inBit_HADIN安全訊號.Value;
            else 
                return true; //Power沒ON的話，都設定為入料中
        }

        //+ By Max 20210329, v4.0.1.59
        public bool IsHADOUTInProcess()
        {
            //+ By Max 20210331, v4.0.1.60, 改Not判斷
            if (!inBit_PowerOn.Value)
                return inBit_HADOUT安全訊號.Value;
            else
                return true; //Power沒ON的話，都設定為出料中
        }

        //v0.0.7.10 By Sanxiu  L/UL安全偵測，當執行L/UL動作，Y軸不能忙錄
        public bool GetSawTablbeChkHadoutSignal()
        {
            return inBit_HADOUT安全訊號.Value && !inBit_PowerOn.Value;
        }
        #endregion 出入料IO防呆

        //+ By Max 20210308, 切割刀序合理性檢查
        private enCutSequenceCheck CutSequenceRationalityCheck()
        {
            HashSet<int> CH1Set = new HashSet<int>();
            HashSet<int> CH2Set = new HashSet<int>();
            HashSet<int> CH1OutOfRangeSet = new HashSet<int>();
            HashSet<int> CH2OutOfRangeSet = new HashSet<int>();
            int iCH1LineNum = PReadValue("Num_row").ToInt() + 1; //CH1 切割道數量
            int iCH2LineNum = PReadValue("Num_col").ToInt() + 1; //CH2 切割道數量

            string sCH = "CH1";
            int Z1Line = 1;
            int Z2Line = 1;
            foreach (DataRow r in AutoCut.CutData.Rows)
            {
                try
                {
                    //+ By Max 20210316, v4.0.1.55, Bugs修正
                    sCH = r[7].ToString();
                    if (sCH == "CH1")
                    {
                        Z1Line = Convert.ToInt32(r[2]);
                        if (Z1Line > iCH1LineNum)
                            CH1OutOfRangeSet.Add(Z1Line);
                        if (Z1Line != 0)
                            CH1Set.Add(Z1Line);
                        Z2Line = Convert.ToInt32(r[4]);
                        if (Z2Line > iCH1LineNum)
                            CH1OutOfRangeSet.Add(Z2Line);
                        if (Z2Line != 0)
                            CH1Set.Add(Z2Line);
                    }
                    else
                    {
                        Z1Line = Convert.ToInt32(r[2]);
                        if (Z1Line > iCH2LineNum)
                            CH2OutOfRangeSet.Add(Z1Line);
                        if (Z1Line != 0)
                            CH2Set.Add(Z1Line);
                        Z2Line = Convert.ToInt32(r[4]);
                        if (Z2Line > iCH2LineNum)
                            CH2OutOfRangeSet.Add(Z2Line);
                        if (Z2Line != 0)
                            CH2Set.Add(Z2Line);
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            if (CH1OutOfRangeSet.Count > 0 || CH2OutOfRangeSet.Count > 0)
            {
                for (int i = 0; i < CH1OutOfRangeSet.Count; ++i)
                {
                    ShowAlarm("w", 54, 1, CH1OutOfRangeSet.Skip(i).First(), iCH1LineNum);
                }
                for (int i = 0; i < CH2OutOfRangeSet.Count; ++i)
                {
                    ShowAlarm("w", 54, 2, CH2OutOfRangeSet.Skip(i).First(), iCH2LineNum);
                }
                return enCutSequenceCheck.CutSequenceOutOfRange;
            }

            if (CH1Set.Count != iCH1LineNum || CH2Set.Count != iCH2LineNum)
            {
                for (int i = 1; i <= iCH1LineNum; ++i)
                {
                    if (!CH1Set.Contains(i))
                    {
                        ShowAlarm("w", 53, 1, i);
                    }
                }
                for (int i = 1; i <= iCH2LineNum; ++i)
                {
                    if (!CH2Set.Contains(i))
                    {
                        ShowAlarm("w", 53, 2, i);
                    }
                }
                return enCutSequenceCheck.CutLineOutOfRange;
            }
            else
                return enCutSequenceCheck.OK;
        }

        private FlowChart.FCRESULT FC_自動_定位_開始_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_定位_Delay1_Run()
        {
            if (Timer_Auto.On(SReadValue("微定位Delay時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_Delay_Run()
        {
            if (Timer_Auto.On(SReadValue("移動至靶點Delay時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_定位_結束_Run()
        {
            m_PositioningFinishEvt.Set();
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_Z移到安全位置2_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_移至第二點_Run()
        {
            bool b位置找到 = false;
            int iθ位移 = 0;

            b位置找到 = ChannelRotation(TeachFixture.strSide, out iθ位移);

            if (b位置找到)
            {
                TeachFixture.nTempθ = iMoveInitialU - iDD馬達一轉Pulse數 * iθ位移 / 4;
            }
            else
            {
                ShowAlarm("E", 90, "Learn Fixture Channel error");
                return default(FlowChart.FCRESULT);
            }

            int nPosY;
            int nPosX2;

            if (iθ位移 == 0)
            {
                nPosY  = SReadValue("圓心點位_Y").ToInt() + Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
            }
            else
            {
                nPosY  = SReadValue("圓心點位_Y").ToInt() + Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
            }


            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, TeachFixture.nTempθ},
                {motor_切割平台前後馬達Y, nPosY},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, nPosX2}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 1;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 28;
                TeachMsg = new string[] { TeachFixture.strSide };
                ShowAlarm("I", 28);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_切割道學習_資料儲存_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 69;
                m_IsCutLineTeachDone = true;
                PSetTable("DGVFixtureData", TeachFixture.dtFixtureData);
                PSetTable("DGVPCBData", LearnMarkPos.dtScanData);
                return FlowChart.FCRESULT.NEXT;
            }
            else if (TeachUserCheckSkip)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 70;
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_移動第一點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, LearnMarkPos.nTempY},
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.nTempX}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_第一點Delay_Run()
        {
            if (Timer_Teach.On(10 * SReadValue("馬達整定時間").ToInt()))
            {
                LearnMarkPos.nFindMarkStep++;
                switch (LearnMarkPos.nFindMarkStep)
                {
                    //case 0://移到第一點
                    case 1://移到第二點
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    case 2://拉直確認
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    case 3://完成
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE2;
                }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_拉直_Run()
        {
            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            Double angle = 0;
            RotateCorrect(Org_X, Org_Y, ref LearnMarkPos.ptLTMarkPos, ref LearnMarkPos.ptLBMarkPos, ref angle);
            int i_angel = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
            //+ By Max 20210206, v4.0.1.42 讓右上點也跟著拉直角度旋轉
            RotatePoint(Org_X, Org_Y, ref LearnMarkPos.ptRTMarkPos, ref LearnMarkPos.ptRTMarkPos, angle);

            LearnMarkPos.nTempθ += i_angel;
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_拉直_完成_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_資料儲存_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 52;
                m_IsTargetTeachDone = true;
                PSetTable("DGVPCBData", LearnMarkPos.dtScanData);
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                return FlowChart.FCRESULT.NEXT;
            }
            else if (TeachUserCheckSkip)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = 53;
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_三點掃靶_掃靶開始_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_ResetIO開啟_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            //AutoMark.Reset();
            AutoMark.strSide = "CH2";
            TriPointSearchcheckR = false;
            TriPointSearchcheckL = false;
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_移至靶點_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            if (dv.Count == 0)
            {
                ShowAlarm("E", 2);//靶點資料未建立，請先學習靶點
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.IDLE;
            }

            AutoMark.nHighBaseFocus = PReadValue("iZ2_FoucusPos").ToInt();

            string strTmp = AutoMark.strSide == "CH1" ? "bCH1Between" : "bCH2Between";

            switch (AutoMark.nFindMarkStep)
            {
                case 0://左上角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X1"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[0]["Scan_Angle"]);
                    break;
                case 1://左下角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X2"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    if (!TriPointSearchcheckR)//0111
                        AutoMark.nTempθ = Convert.ToInt32(dv[0]["Scan_Angle"]);
                    break;
                case 2://右上角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X1"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[dv.Count - 1]["Scan_Angle"]);
                    break;
                case 3://右下角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X2"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X2"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    if (!TriPointSearchcheckL)//0111
                        AutoMark.nTempθ = Convert.ToInt32(dv[dv.Count - 1]["Scan_Angle"]);
                    break;

                case 4:
                    ShowAlarm("w", 49);//三點定位失敗，請強排後選擇內縮靶點
                    return FlowChart.FCRESULT.IDLE;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoMark.nTempθ},
                {motor_切割平台前後馬達Y, AutoMark.nTempY},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  AutoMark.nTempX},
                {motor_切刀上下馬達_Z1,  SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2,  AutoMark.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                if (AutoMark.nFindMarkStep == 0 && AutoMark.strSide == "CH2")
                {
                    return FlowChart.FCRESULT.CASE1;
                }
                else
                {
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_三點掃靶_視覺通訊選擇圖案_Run()
        {
            return SelectPattern(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_三點掃靶_等待結果_Run()
        {
            switch (mCCDAction)//設定樣式
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 37);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }

            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 38);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_三點掃靶_自動對焦_Run()
        {
            AutoFocusFinish = true;

            //是否使用自動對焦
            if (SReadValue("bUseChkAutoFocus").ToBoolean())
            {
                enFocusMode enMode = (enFocusMode)(SReadValue("iAutoFocusChkCycle").ToInt());

                if ((enMode == enFocusMode.首片 && PCBIncount == 1) || enMode == enFocusMode.每片)
                {
                    AutoFocusFinish = false;
                    CallAutoFocus = "Auto";

                    if (OReadValue("bFastAutoFocus").ToBoolean())
                    {
                        FC_教導_快速自動對焦_開始.TaskReset();
                    }
                    else
                    {
                        FC_教導_自動對焦_開始.TaskReset();//v4.0.1.25新增自動對焦
                    }
                }
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦開始");

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_自動對焦完成_Run()
        {
            if (AutoFocusFinish)
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦完成");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (OReadValue("bFastAutoFocus").ToBoolean())
                {
                    FC_教導_快速自動對焦_開始.MainRun();
                }
                else
                {
                    FC_教導_自動對焦_開始.MainRun();//v4.0.1.25
                }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_三點掃靶_NG_Run()
        {
            ShowAlarm("i", 14);//視覺設定圖案形狀失敗，請按繼續重試
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_開始定位_Run()
        {
            FC_自動_定位.TaskReset();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_定位完成_Run()
        {
            if (m_PositioningFinishEvt.WaitOne(0))
            {
                AutoMark.nFindMarkStep++;

                switch (AutoMark.nFindMarkStep)
                {
                    case 1:
                        if (TriPointSearchcheckR && AutoMark.ptLTMarkPos.X != 0)
                        {
                            //TriPointSearchcheckR = false;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.NEXT;
                        }
                        break;
                    case 2:
                        if (TriPointSearchcheckR && AutoMark.ptLBMarkPos.X != 0)
                        {
                            //TriPointSearchcheckR = false;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.NEXT;
                        }
                        else if (AutoMark.ptLTMarkPos.X != 0 && AutoMark.ptLBMarkPos.X != 0)//都有值代表左半邊定位成功，可先做拉直
                        {
                            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
                            int Org_Y = SReadValue("圓心點位_Y").ToInt();
                            Double angle = 0;
                            //左邊角度運算
                            RotateCorrect(Org_X, Org_Y, ref AutoMark.ptLTMarkPos, ref AutoMark.ptLBMarkPos, ref angle);
                            int i_angleL = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                            AutoMark.nLeftθ = AutoMark.nTempθ + i_angleL;
                            TriPointSearchcheckL = true;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.CASE2;
                        }
                        break;
                    case 3:
                        if (TriPointSearchcheckL && AutoMark.ptRTMarkPos.X != 0)
                        {
                            //TriPointSearchcheckL = false;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.NEXT;
                        }
                        break;
                    case 4:
                        if (TriPointSearchcheckL && AutoMark.ptRBMarkPos.X != 0)
                        {
                            //TriPointSearchcheckL = false;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.NEXT;
                        }
                        else if (AutoMark.ptRTMarkPos.X != 0 && AutoMark.ptRBMarkPos.X != 0)//都有值代表左半邊定位成功，可先做拉直
                        {
                            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
                            int Org_Y = SReadValue("圓心點位_Y").ToInt();
                            Double angle = 0;
                            //右邊角度運算
                            RotateCorrect(Org_X, Org_Y, ref AutoMark.ptRTMarkPos, ref AutoMark.ptRBMarkPos, ref angle);
                            int i_angleR = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                            AutoMark.nRightθ = AutoMark.nTempθ + i_angleR;
                            AutoMark.nFindMarkStep = 0;
                            TriPointSearchcheckR = true;
                            Timer_Auto.Restart();
                            return FlowChart.FCRESULT.CASE2;
                        }
                        break;
                }

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            FC_自動_定位.MainRun();
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_三點掃靶_計算_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            int GapLT = 0;
            int GapLB = 0;
            double Pitch = 0;
            double SPCPitch = 0;

            double X1 = AutoMark.ptRTMarkPos.X - AutoMark.ptLTMarkPos.X;
            double X2 = AutoMark.ptRBMarkPos.X - AutoMark.ptLBMarkPos.X;
            //計算Pitch

            string strTmp = AutoMark.strSide == "CH1" ? "bCH1Between" : "bCH2Between";

            if (PReadValue(strTmp).ToBoolean())
            {
                if (AutoMark.ptLTMarkPos.X != 0 && AutoMark.ptRTMarkPos.X != 0)//用上方
                {
                    Pitch = X1 / (dv.Count - 2d);
                }
                else if (AutoMark.ptLBMarkPos.X != 0 && AutoMark.ptRBMarkPos.X != 0)//用下方
                {
                    Pitch = X2 / (dv.Count - 2d);
                }
            }
            else
            {
                if (AutoMark.ptLTMarkPos.X != 0 && AutoMark.ptRTMarkPos.X != 0)//用上方
                {
                    Pitch = X1 / (dv.Count - 1d);
                }
                else if (AutoMark.ptLBMarkPos.X != 0 && AutoMark.ptRBMarkPos.X != 0)//用下方
                {
                    Pitch = X2 / (dv.Count - 1d);
                }
            }

            //用Pitch推倒左上角點或左下角點
            if (TriPointSearchcheckR)
            {
                if (AutoMark.ptLTMarkPos.X == 0)
                {
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.ptLTMarkPos.X = AutoMark.ptRTMarkPos.X - Convert.ToInt32(Pitch * (dv.Count - 2));
                    }
                    else
                    {
                        AutoMark.ptLTMarkPos.X = AutoMark.ptRTMarkPos.X - Convert.ToInt32(Pitch * (dv.Count - 1));
                    }
                    AutoMark.ptLTMarkPos.Y = AutoMark.ptRTMarkPos.Y;
                }
                else if (AutoMark.ptLBMarkPos.X == 0)
                {
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.ptLBMarkPos.X = AutoMark.ptRBMarkPos.X - Convert.ToInt32(Pitch * (dv.Count - 2));
                    }
                    else
                    {
                        AutoMark.ptLBMarkPos.X = AutoMark.ptRBMarkPos.X - Convert.ToInt32(Pitch * (dv.Count - 1));
                    }
                    AutoMark.ptLBMarkPos.Y = AutoMark.ptRBMarkPos.Y;
                }
            }

            GapLT = Convert.ToInt32(dv[0]["Scan_X1"].ToString()) - AutoMark.ptLTMarkPos.X;//左上角點偏差值
            GapLB = Convert.ToInt32(dv[0]["Scan_X2"].ToString()) - AutoMark.ptLBMarkPos.X;//左下角點偏差值

            SPCPitch = Math.Abs(Convert.ToInt32(dv[1]["Scan_X1"].ToString()) - Convert.ToInt32(dv[0]["Scan_X1"].ToString()));//SPC的Pitch

            //Pitch是否超出卡控值
            if (Math.Abs(Pitch - SPCPitch) > PReadValue("Pitch_tolerance").ToInt())
            {
                ShowAlarm("i", 9, AutoMark.strSide, Pitch, SPCPitch);
                return FlowChart.FCRESULT.IDLE;
            }
            int a = Convert.ToInt32(dv[0]["Scan_Angle"].ToString());
            double angle = Convert.ToDouble(AutoMark.nTempθ - Convert.ToInt32(dv[0]["Scan_Angle"].ToString())) / iDD馬達一轉Pulse數 * 360;

            if (Math.Abs(angle) > PReadValue("Cut_U_tolerance").ToDouble())
            {
                //double U_offset = Convert.ToInt32((-angle / 360) * iDD馬達一轉Pulse數);
                ShowAlarm("w", 25, -angle);
                return FlowChart.FCRESULT.IDLE;
            }

            //靶點位置是否超出學靶位置，會切到切割道
            if (Math.Abs(GapLT) > PReadValue("Cut_X_tolerance").ToInt() || Math.Abs(GapLB) > PReadValue("Cut_X_tolerance").ToInt())
            {
                if (AutoMark.strSide == "CH2")
                {
                    ShowAlarm("i", 20, "HADIN", (GapLT + GapLB) / 2);
                }
                else if (AutoMark.strSide == "CH1")
                {
                    ShowAlarm("i", 20, "Y", (GapLT + GapLB) / 2);
                }
                return FlowChart.FCRESULT.IDLE;
            }

            //需再轉正Woody20200929
            DataRow dr = AutoMark.dtScanData.NewRow();

            //+ By Max 20210223, 掃靶後，產出掃靶Log資訊，方便比對切偏時相關資料
            sb.Clear();
            for (int i = 0; i < dv.Count; i++)
            {
                dr = AutoMark.dtScanData.NewRow();
                dr["Scan_LineNo"] = (i + 1).ToString();
                sb.Append(String.Format("LineNo: {0},", (i + 1)));
                dr["Scan_Side"] = AutoMark.strSide;
                sb.Append(String.Format("Side: {0},", AutoMark.strSide));
                dr["Scan_X1"] = (AutoMark.ptLTMarkPos.X + Convert.ToInt32(i * Pitch)).ToString();
                sb.Append(String.Format("Scan_X1: {0},", AutoMark.ptLTMarkPos.X + Convert.ToInt32(i * Pitch)));
                dr["Scan_Y1"] = AutoMark.ptLTMarkPos.Y.ToString();
                sb.Append(String.Format("Scan_Y1: {0},", AutoMark.ptLTMarkPos.Y));
                dr["Scan_X2"] = (AutoMark.ptLBMarkPos.X + Convert.ToInt32(i * Pitch)).ToString();
                sb.Append(String.Format("Scan_X2: {0},", AutoMark.ptLBMarkPos.X + Convert.ToInt32(i * Pitch)));
                dr["Scan_Y2"] = AutoMark.ptLBMarkPos.Y.ToString();
                sb.Append(String.Format("Scan_Y2: {0},", AutoMark.ptLBMarkPos.Y));
                dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                sb.Append(String.Format("Scan_Z: {0},", (motor_切刀上下馬達_Z2.ReadEncPos())));
                dr["Scan_Angle"] = AutoMark.nTempθ.ToString();
                sb.Append(String.Format("Scan_Angle: {0},", AutoMark.nTempθ));
                dr["Scan_XOffset"] = "0";
                sb.Append(String.Format("Scan_XOffset: 0"));
                LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                sb.Clear();
                AutoMark.dtScanData.Rows.Add(dr);
            }
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_定位結束_Run()
        {
            if (AutoMark.strSide == "CH1")//CH2與CH1掃靶完成
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                AutoMark.strSide = "CH1";
                AutoMark.nFindMarkStep = 0;
                AutoMark.ptLTMarkPos = new Point();
                AutoMark.ptLBMarkPos = new Point();
                AutoMark.ptRTMarkPos = new Point();
                AutoMark.ptRBMarkPos = new Point();
                TriPointSearchcheckR = false;
                TriPointSearchcheckL = false;
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_三點掃靶_切割點位設定_Run()
        {
            m_bIsVisionSaw = SReadValue("bVisionSaw").ToBoolean();//設定VisionSaw
            AutoCut.SafeZ = SReadValue("安全點位_Z2").ToInt();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_三點掃靶_掃靶結束_Run()
        {
            m_SearchTargetDoneEvt.Set();
            //PSetTable("DGVScanData", AutoMark.ScanData);
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_三點掃靶_轉正_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            AutoMark.nHighBaseFocus = PReadValue("iZ2_FoucusPos").ToInt();

            string strTmp = AutoMark.strSide == "CH1" ? "bCH1Between" : "bCH2Between";

            switch (AutoMark.nFindMarkStep)
            {
                case 0://左上角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X1"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    AutoMark.nTempθ = AutoMark.nRightθ;
                    break;
                case 1://左下角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X2"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    AutoMark.nTempθ = AutoMark.nRightθ;
                    break;
                case 2://右上角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X1"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    AutoMark.nTempθ = AutoMark.nLeftθ;
                    break;
                case 3://右下角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X2"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X2"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    AutoMark.nTempθ = AutoMark.nLeftθ;
                    break;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoMark.nTempθ},
                {motor_切割平台前後馬達Y, AutoMark.nTempY},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  AutoMark.nTempX},
                {motor_切刀上下馬達_Z1,  SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2,  AutoMark.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_動態刀痕_Z2補償值_Run()
        {
            if (AutoUserCheckOk)
            {
                int Gap_X = AutoCut.TempX2 - motor_切刀橫移馬達_Z2X.ReadEncPos();
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 3;
                AutoUserCheckOk = false;
                AutoUserCheckSkip = false;
                //+ By Max 20210305
                //AutoMessage = "Z2 Kerf Offset" + Gap_X.ToString() + ",Click Next to Save , Click Skip to Ignore";
                AutoMessage = new string[] { "Z2", Gap_X.ToString() };
                AutoAlarmLevel = "I";
                AutoAlarmCode = 34;

                ShowAlarm("I", 34, "Z2", Gap_X.ToString());
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_動態刀痕_Z1補償值_Run()
        {
            if (AutoUserCheckOk)
            {
                int Gap_X = AutoCut.TempX1 - motor_切刀橫移馬達_Z2X.ReadEncPos();
                //+ By Max 20210311, 動態刀痕與下刀點確認變數分開
                AutoFlowStepForCutOffset = 3;
                AutoUserCheckOk = false;
                AutoUserCheckSkip = false;
                //+ By Max 20210305
                //AutoMessage = "Z1 Kerf Offset" + Gap_X.ToString() + ",Click Next to Save , Click Skip to Ignore";
                AutoMessage = new string[] { "Z1", Gap_X.ToString() };
                AutoAlarmLevel = "I";
                AutoAlarmCode = 34;

                ShowAlarm("I", 34, "Z1", Gap_X.ToString());
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_刀痕檢測_Delay_Run()
        {
            if (Timer_Auto.On(SReadValue("刀痕檢查Delay時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_全靶_N字形_掃靶開始_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_ResetIO開啟_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            AutoMark.strSide = "CH2";
            AutoMark.nFindMarkStep = 0;
            TopPoint.Clear();
            BottomPoint.Clear();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_移至靶點_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            if (dv.Count == 0)
            {
                ShowAlarm("E", 2);//靶點資料未建立，請先學習靶點
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.IDLE;
            }

            AutoMark.nHighBaseFocus = PReadValue("iZ2_FoucusPos").ToInt();
            if (AutoMark.nFindMarkStep < 2 * dv.Count)
            {
                switch (m_enScanMethod)
                {
                    case enScanTargetMethod.半靶掃靶:
                        if (AutoMark.nFindMarkStep < 2)//0與1先掃左上、右下，判斷是否超出切割道
                        {
                            if (AutoMark.nFindMarkStep % 2 == 0)
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X1"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y1"]);
                            }
                            else
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X2"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y2"]);
                            }
                            //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                            AutoMark.nTempθ = Convert.ToInt32(dv[0]["Scan_Angle"]);
                        }
                        else
                        {
                            if (AutoMark.nFindMarkStep <= dv.Count)//掃上半部
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_X1"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_Y1"]);
                                //AutoMark.HighBaseFocus = Convert.ToInt32(dv[AutoMark.FindMarkStep - 1]["Scan_Z"]);
                                AutoMark.nTempθ = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_Angle"]);
                            }
                            else
                            {
                                Timer_Auto.Restart();
                                return FlowChart.FCRESULT.CASE2;
                            }
                        }
                        break;
                    case enScanTargetMethod.全靶掃靶_口字形:
                        if (AutoMark.nFindMarkStep < 2)//0與1先掃左上、右下，判斷是否超出切割道
                        {
                            if (AutoMark.nFindMarkStep % 2 == 0)
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X1"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y1"]);
                            }
                            else
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X2"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y2"]);
                            }
                            //AutoMark.HighBaseFocus = Convert.ToInt32(dv[AutoMark.FindMarkStep / 2]["Scan_Z"]);
                            AutoMark.nTempθ = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Angle"]);
                        }
                        else
                        {
                            if (AutoMark.nFindMarkStep <= dv.Count)//先掃上半部，再掃下半部
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_X1"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_Y1"]);
                                //AutoMark.HighBaseFocus = Convert.ToInt32(dv[AutoMark.FindMarkStep - 1]["Scan_Z"]);
                                AutoMark.nTempθ = Convert.ToInt32(dv[AutoMark.nFindMarkStep - 1]["Scan_Angle"]);
                            }
                            else
                            {
                                AutoMark.nTempX = Convert.ToInt32(dv[2 * dv.Count - AutoMark.nFindMarkStep]["Scan_X2"]);
                                AutoMark.nTempY = Convert.ToInt32(dv[2 * dv.Count - AutoMark.nFindMarkStep]["Scan_Y2"]);
                                //AutoMark.HighBaseFocus = Convert.ToInt32(dv[2 * dv.Count - AutoMark.FindMarkStep]["Scan_Z"]);
                                AutoMark.nTempθ = Convert.ToInt32(dv[2 * dv.Count - AutoMark.nFindMarkStep]["Scan_Angle"]);
                            }
                        }
                        break;
                    case enScanTargetMethod.全靶掃靶_N字形:
                        if (AutoMark.nFindMarkStep % 2 == 0)
                        {
                            AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X1"]);
                            AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y1"]);
                        }
                        else
                        {
                            AutoMark.nTempX = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_X2"]);
                            AutoMark.nTempY = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Y2"]);
                        }
                        //AutoMark.HighBaseFocus = Convert.ToInt32(dv[AutoMark.FindMarkStep / 2]["Scan_Z"]);
                        AutoMark.nTempθ = Convert.ToInt32(dv[AutoMark.nFindMarkStep / 2]["Scan_Angle"]);
                        break;
                }
            }

            else
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE2;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoMark.nTempθ},
                {motor_切割平台前後馬達Y, AutoMark.nTempY},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  AutoMark.nTempX},
                {motor_切刀上下馬達_Z1,  SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2,  AutoMark.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                if (AutoMark.nFindMarkStep == 0)
                {
                    return FlowChart.FCRESULT.CASE1;
                }
                else
                {
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_自動對焦_Run()
        {
            AutoFocusFinish = true;

            //是否使用自動對焦
            if (SReadValue("bUseChkAutoFocus").ToBoolean())
            {
                enFocusMode enMode = (enFocusMode)(SReadValue("iAutoFocusChkCycle").ToInt());

                if ((enMode == enFocusMode.首片 && PCBIncount == 1) || enMode == enFocusMode.每片)
                {
                    AutoFocusFinish = false;
                    CallAutoFocus = "Auto";

                    if (OReadValue("bFastAutoFocus").ToBoolean())
                    {
                        FC_教導_快速自動對焦_開始.TaskReset();
                    }
                    else
                    {
                        FC_教導_自動對焦_開始.TaskReset();//v4.0.1.25新增自動對焦
                    }
                }
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦開始");

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_自動對焦完成_Run()
        {
            if (AutoFocusFinish)
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦完成");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (OReadValue("bFastAutoFocus").ToBoolean())
                {
                    FC_教導_快速自動對焦_開始.MainRun();
                }
                else
                {
                    FC_教導_自動對焦_開始.MainRun();//v4.0.1.25
                }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_全靶_N字形_視覺通訊選擇圖案_Run()
        {
            return SelectPattern(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_全靶_N字形_等待結果_Run()
        {
            switch (mCCDAction)//設定樣式
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 37);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }

            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 38);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_全靶_N字形_NG_Run()
        {
            ShowAlarm("i", 14);//視覺設定圖案形狀失敗，請按繼續重試
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_開始定位_Run()
        {
            FC_自動_定位.TaskReset();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_定位完成_Run()
        {
            if (m_PositioningFinishEvt.WaitOne(0))
            {
                AutoMark.nFindMarkStep++;

                if (AutoMark.nFindMarkStep == 2)//左上角與左下角掃靶完成先拉直以及切割道卡控
                {

                    DataView dv = new DataView(LearnMarkPos.dtScanData);
                    dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

                    int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
                    int Org_Y = SReadValue("圓心點位_Y").ToInt();
                    Double angle = 0;
                    Point TempTop = TopPoint[0];
                    Point TempBot = BottomPoint[0];

                    int GapLT = Convert.ToInt32(dv[0]["Scan_X1"].ToString()) - TempTop.X;//左上角點偏差值
                    int GapLB = Convert.ToInt32(dv[0]["Scan_X2"].ToString()) - TempBot.X;//左下角點偏差值                    

                    //左邊角度運算
                    RotateCorrect(Org_X, Org_Y, ref TempTop, ref TempBot, ref angle);
                    int i_angleL = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                    AutoMark.nTempθ += i_angleL;

                    if (Math.Abs(angle) > PReadValue("Cut_U_tolerance").ToDouble())
                    {
                        //double U_offset = Convert.ToInt32((-angle / 360) * iDD馬達一轉Pulse數);
                        ShowAlarm("w", 25, -angle);
                        return FlowChart.FCRESULT.IDLE;
                    }

                    //靶點位置是否超出學靶位置，會切到切割道
                    if (Math.Abs(GapLT) > PReadValue("Cut_X_tolerance").ToInt() || Math.Abs(GapLB) > PReadValue("Cut_X_tolerance").ToInt())
                    {
                        if (AutoMark.strSide == "CH2")
                        {
                            ShowAlarm("i", 20, "HADIN", (GapLT + GapLB) / 2);
                        }
                        else if (AutoMark.strSide == "CH1")
                        {
                            ShowAlarm("i", 20, "Y", (GapLT + GapLB) / 2);
                        }
                        return FlowChart.FCRESULT.IDLE;
                    }

                    DataRow dr = AutoMark.dtScanData.NewRow();
                    dr = AutoMark.dtScanData.NewRow();
                    dr["Scan_LineNo"] = (1).ToString();
                    dr["Scan_Side"] = AutoMark.strSide;
                    dr["Scan_X1"] = TempTop.X.ToString();
                    dr["Scan_Y1"] = TempTop.Y.ToString();
                    dr["Scan_X2"] = TempBot.X.ToString();
                    dr["Scan_Y2"] = TempBot.Y.ToString();
                    dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                    dr["Scan_Angle"] = AutoMark.nTempθ.ToString();
                    dr["Scan_XOffset"] = "0";
                    AutoMark.dtScanData.Rows.Add(dr);

                }

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            FC_自動_定位.MainRun();
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_全靶_N字形_計算_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            int Pitch = 0;
            int SPCPitch = 0;

            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            Double angle = 0;

            //需再轉正Woody20200929
            DataRow dr = AutoMark.dtScanData.NewRow();

            for (int i = 1; i < dv.Count; i++)
            {
                Point TempTop = new Point();
                Point TempBot = new Point();
                switch (m_enScanMethod)
                {

                    case enScanTargetMethod.半靶掃靶:
                        TempTop = TopPoint[i];
                        TempBot.X = BottomPoint[0].X + (TopPoint[i].X - TopPoint[0].X);
                        TempBot.Y = BottomPoint[0].Y + (TopPoint[i].Y - TopPoint[0].Y);
                        break;
                    case enScanTargetMethod.全靶掃靶_口字形:
                        TempTop = TopPoint[i];
                        TempBot = BottomPoint[dv.Count - i];
                        break;
                    case enScanTargetMethod.全靶掃靶_N字形:
                        TempTop = TopPoint[i];
                        TempBot = BottomPoint[i];
                        break;

                }

                //+ By Max, 4.0.1.99
                sb.Clear();
                if (bIsSimuProcess)
                {
                    String strLog = String.Format("ReSimu Process Countdown: {0}", iReSimuProcessRunCount);
                    LogSay(EnLoggerType.EnLog_SPC, strLog);
                    sb.Append(String.Format("Line = {0}, TopPosX = {1}, TopPosY = {2} ", i, TempTop.X, TempTop.Y));
                    sb.Append(String.Format("Line = {0}, BottomPosX = {0}, BottomPosY = {1} ", i, TempBot.X, TempBot.Y));
                    LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                }

                //左邊角度運算
                RotateCorrect(Org_X, Org_Y, ref TempTop, ref TempBot, ref angle);
                int i_angleL = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                //AutoMark.temp_theta += i_angleL;
                int now_angle = AutoMark.nTempθ + i_angleL;//v4.0.1.19

                SPCPitch = Math.Abs(Convert.ToInt32(dv[1]["Scan_X1"].ToString()) - Convert.ToInt32(dv[0]["Scan_X1"].ToString()));//SPC的Pitch

                Pitch = (TopPoint[i].X - TopPoint[i - 1].X);

                //Pitch是否超出卡控值
                if (Math.Abs(Pitch - SPCPitch) > PReadValue("Pitch_tolerance").ToInt())
                {
                    ShowAlarm("i", 9, AutoMark.strSide, Pitch, SPCPitch);
                    return FlowChart.FCRESULT.IDLE;
                }

                //angle = now_angle / iDD馬達一轉Pulse數 * 360;

                if (Math.Abs(angle) > PReadValue(" Cut_U_tolerance").ToDouble())
                {
                    ShowAlarm("w", 25, angle);
                    return FlowChart.FCRESULT.IDLE;
                }

                //+ By Max 20210223, 掃靶後，產出掃靶Log資訊，方便比對切偏時相關資料
                sb.Clear();

                dr = AutoMark.dtScanData.NewRow();
                dr["Scan_LineNo"] = (i + 1).ToString();
                sb.Append(String.Format("LineNo: {0},", (i + 1)));
                dr["Scan_Side"] = AutoMark.strSide;
                sb.Append(String.Format("Side: {0},", AutoMark.strSide));
                dr["Scan_X1"] = TempTop.X.ToString();
                sb.Append(String.Format("Scan_X1: {0},", TempTop.X));
                dr["Scan_Y1"] = TempTop.Y.ToString();
                sb.Append(String.Format("Scan_Y1: {0},", TempTop.Y));
                dr["Scan_X2"] = TempBot.X.ToString();
                sb.Append(String.Format("Scan_X2: {0},", TempBot.X));
                dr["Scan_Y2"] = TempBot.Y.ToString();
                sb.Append(String.Format("Scan_Y2: {0},", TempBot.Y));
                dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                sb.Append(String.Format("Scan_Z: {0},", motor_切刀上下馬達_Z2.ReadEncPos()));
                dr["Scan_Angle"] = now_angle.ToString();//v4.0.1.19
                sb.Append(String.Format("Scan_Angle: {0},", now_angle));
                dr["Scan_XOffset"] = "0";
                sb.Append(String.Format("Scan_XOffset: 0,"));
                LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                sb.Clear();

                AutoMark.dtScanData.Rows.Add(dr);
            }
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_切割點位設定_Run()
        {
            m_bIsVisionSaw = SReadValue("bVisionSaw").ToBoolean();//設定VisionSaw
            AutoCut.SafeZ = SReadValue("安全點位_Z2").ToInt();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_全靶_N字形_定位結束_Run()
        {
            if (AutoMark.strSide == "CH1")//CH2與CH1掃靶完成
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                AutoMark.strSide = "CH1";
                AutoMark.nFindMarkStep = 0;
                TopPoint.Clear();//v4.0.1.19
                BottomPoint.Clear();//v4.0.1.19
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_全靶_N字形_掃靶結束_Run()
        {
            m_SearchTargetDoneEvt.Set();
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_下刀點確認_Delay_Run()
        {
            if (Timer_Auto.On(SReadValue("馬達整定時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_連線開始_Run()
        {
            Socket_Timer.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_是否連線_Run()
        {
            try
            {
                if (SReadValue("ConnectHandler").ToInt() != 0)
                {
                    if (inBit_Socket.On() && !IsConnected() && !inBit_PowerOn.Value)//v4.0.1.25
                    {
                        TCPIPConnect();
                        Socket_Timer.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                }
            }
            catch (Exception e)
            {
                LogSay(EnLoggerType.EnLog_ARM, e.Message);
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_連線Delay_Run()
        {
            if (IsConnected())
            {
                Socket_Timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            else if (Socket_Timer.On(5000))
            {
                Socket_Timer.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_是否斷線_Run()
        {
            //v4.0.1.32 若連線中斷時，Handler會開關SocketIO，讓Saw重新連線
            if (!bConnectHandler || !inBit_Socket.Value)
            {
                TCPIPClose();
                Socket_Timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_斷線Delay_Run()
        {
            if (Socket_Timer.On(5000))
            {
                Socket_Timer.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        #region 畫刀序 v4.0.1.23
        //設定畫筆與畫布
        private Action<bool> DelegateDrawReset;
        public void RecordDelegateDrawReset(Action<bool> DrawReset)
        {
            DelegateDrawReset = DrawReset;
        }
        //按照切割刀序做畫
        private Action<String, int, int, int, bool> DelegateDrawLineData;
        public void RecordDelegateDrawLineData(Action<String, int, int, int, bool> DrawLineData)
        {
            DelegateDrawLineData = DrawLineData;
        }

        #endregion 畫刀序

        private void button_重新連線_Click(object sender, EventArgs e)
        {
            TCPIPClose();
            FC_連線開始.TaskReset();
        }

        private void button_傳送_Click(object sender, EventArgs e)
        {
            SendData(comboBox_指令.Text);
        }

        private FlowChart.FCRESULT FC_初始化_測高汽缸脫離_Run()
        {
            //v4.0.1.31 強排初始化-流程加入測高塊檔塊Reset，以免撞機
            SetOutBit(outBit_測高塊偵測檔片接觸, false);

            if (GetInBit(inBit_測高塊偵測檔片脫離))
            {
                Timer_ActionIni.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_ActionIni.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("w", 11);////測高塊偵測檔片脫離訊號異常
                Timer_ActionIni.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            MOTORTYPE type = motor_切刀橫移馬達_Z1X.MotorType;

            motor_切刀橫移馬達_Z1X.MotorType = MOTORTYPE.YASKAWA_ABS;
            label9.Text = motor_切刀橫移馬達_Z1X.BasePulseCount.ToString();
            label13.Text = motor_切刀橫移馬達_Z1X.ReadAbsEncoderPos().ToString();

            motor_切刀橫移馬達_Z1X.MotorType = type;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            motor_切刀橫移馬達_Z1X.MotorType = MOTORTYPE.NORMAL;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            motor_切刀橫移馬達_Z2X.MotorType = MOTORTYPE.NORMAL;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MOTORTYPE type = motor_切刀橫移馬達_Z2X.MotorType;

            motor_切刀橫移馬達_Z2X.MotorType = MOTORTYPE.YASKAWA_ABS;
            label17.Text = motor_切刀橫移馬達_Z2X.BasePulseCount.ToString();
            label16.Text = motor_切刀橫移馬達_Z2X.ReadAbsEncoderPos().ToString();

            motor_切刀橫移馬達_Z2X.MotorType = type;
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (groupBox7.Visible)
                {
                    groupBox7.Visible = false;
                    groupBox20.Visible = false;
                }
                else
                {
                    groupBox7.Visible = true;
                    groupBox20.Visible = true;
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m_bIsZ1err = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            m_bIsZ2err = true;
        }

        private FlowChart.FCRESULT FC_Vision_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private void FC_靶點學習_入料_等待確認入料_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_等待確認_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_NG1_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_NG2_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_換刀流程_等待換刀完成_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_換刀流程_等待調整破刀Sensor_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_切割道學習_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_切割道學習_確認第一基準點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_切割道學習_確認第二基準點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_切割道學習_移到下一條_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_拉直_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_圓心校正_放入料片_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_圓心校正_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void 確認第一點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_圓心校正_定位_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_切割道學習_資料儲存_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_靶點學習_資料儲存_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        //+ By Max 20210309, AutoUserCheckOk旗標重置改在BeforeRun設定
        private void FC_動態刀痕_Z1是否補償_BeforeRun()
        {
            AutoUserCheckOk = false;
            AutoUserCheckSkip = false;
        }

        //+ By Max 20210309, AutoUserCheckOk旗標重置改在BeforeRun設定
        private void FC_動態刀痕_Z2是否補償_BeforeRun()
        {
            AutoUserCheckOk = false;
            AutoUserCheckSkip = false;
        }

        //+ By Max 20210309, AutoUserCheckOk旗標重置改在BeforeRun設定
        private void FC_動態刀痕_Z2補償值_BeforeRun()
        {
            AutoUserCheckOk = false;
        }

        //+ By Max 20210309, AutoUserCheckOk旗標重置改在BeforeRun設定
        private void FC_動態刀痕_Z1補償值_BeforeRun()
        {
            AutoUserCheckOk = false;
        }

        #region 歸零流程

        private FlowChart.FCRESULT FC_SAW歸零_動作開始_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_是否可以歸零_Run()
        {
            if (mCanHome)
            {
                ServoOn();
                motor_切刀上下馬達_Z1.HomeReset();
                motor_切刀上下馬達_Z2.HomeReset();

                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_測高訊號關閉_Run()
        {
            if (GetSparkOff()) //HOME
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Home.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
                Timer_Home.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_Z軸歸零_Run()
        {
            if (IsSimulation() != 0)
            {
                motor_切刀上下馬達_Z1.SetPos(0);
                motor_切刀上下馬達_Z1.SetEncoderPos(0);
                motor_切刀上下馬達_Z2.SetPos(0);
                motor_切刀上下馬達_Z2.SetEncoderPos(0);

                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                List<Motor> lstMotors = new List<Motor>()
                {
                    motor_切刀上下馬達_Z1, motor_切刀上下馬達_Z2
                };

                if (MultiMotorsHome(lstMotors, Timer_Home)) return FlowChart.FCRESULT.NEXT;

                return FlowChart.FCRESULT.IDLE;
            }
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_Z軸重置位置_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))  //Home 完成後等 100 ms 重置位置 (怕不穩定)
            {
                motor_切刀上下馬達_Z1.SetPos(0);
                motor_切刀上下馬達_Z1.SetEncoderPos(0);
                motor_切刀上下馬達_Z2.SetPos(0);
                motor_切刀上下馬達_Z2.SetEncoderPos(0);

                m_nHomeFlow = 1;

                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region IO檢查

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_動作開始_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_水開啟_Run()
        {
            //v4.0.1.0 By Woody 先開水讓管路通
            //+ By Max 20210316, v4.0.1.55, 2.5修正為4
            analogOut_Z1噴水座流量控制.Value = 4.0f;
            analogOut_Z1灑水座流量控制.Value = 4.0f;
            analogOut_Z2噴水座流量控制.Value = 4.0f;
            analogOut_Z2灑水座流量控制.Value = 4.0f;

            SetOutBit(outBit_Z1水開關電磁閥, true);
            SetOutBit(outBit_Z2水開關電磁閥, true);

            //v4.0.1.31 歸零開啟真空幫浦與冰水機
            SetOutBit(outBit_真空幫浦啟動, true);
            //+ By Max 20210416, v4.0.1.62, 因應分開成兩個OutBit，啟動與運轉同時控制
            SetOutBit(outBit_真空幫浦運轉, true);
            SetOutBit(outBit_冰水機啟動, true);

            SetOutBit(outBit_鼓風機啟動, true);//v4.0.1.23歸零開啟鼓風機
            SetOutBit(outBit_純水加壓幫浦啟動, true);//v4.0.1.23歸零開啟純水PUMNP

            if (CheckWater(enSpindleSel.Z1) && CheckWater(enSpindleSel.Z2))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (Timer_Home.On(SReadValue("水開啟準備時間").ToInt()))
            {
                if (!CheckWater(enSpindleSel.Z1))
                {
                    ShowAlarm("w", 14, "Z1"); //Z1噴水/灑水流量未到達
                }
                if (!CheckWater(enSpindleSel.Z2))
                {
                    ShowAlarm("w", 14, "Z2"); //Z2噴水/灑水流量未到達
                }

                Timer_Home.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_測高汽缸脫離_Run()
        {
            //v4.0.1.31 歸零流程加入測高塊檔塊Reset，以免撞機
            SetOutBit(outBit_測高塊偵測檔片接觸, false);

            if (GetInBit(inBit_測高塊偵測檔片脫離))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Home.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("w", 11);////測高塊偵測檔片脫離訊號異常
                Timer_Home.Restart();
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_初始化IO_Run()
        {
            #region CCD/NCS
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_非接觸測高清潔水柱, false);
            SetOutBit(outBit_非接觸測高清潔氣旋, false);
            #endregion

            #region Spindle
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_Z1刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z2刀側邊清潔氣旋, false);
            SetOutBit(outBit_主軸軸承鎖定, false);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);
            SetOutBit(outBit_Z1非接觸式測高護蓋關閉, true);
            SetOutBit(outBit_Z2非接觸式測高護蓋關閉, true);
            #endregion

            #region Table
            SetOutBit(outBit_切割區增濕水簾, false);
            SetOutBit(outBit_切割區水霧隔絕氣簾, false);
            SetOutBit(outBit_水槽清潔噴水啟動, false);
            SetOutBit(outBit_水槽清潔吹氣啟動, false);
            SetOutBit(outBit_清潔風刀, false);
            SetOutBit(outBit_切割平台真空建立, false);
            #endregion

            bCanPutRunSate = false;
            bCanGetRunSate = false;
            bCanFrontGetRun = false;

            SetOutBit(outBit_ManualLoadOK, false);

            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;

        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_刀輪護蓋到位_Run()
        {
            if (GetInBit(inBit_Z1刀輪護蓋關閉) && GetInBit(inBit_Z2刀輪護蓋關閉) && GetInBit(inBit_Z1非接觸測高蓋關閉檢知) && GetInBit(inBit_Z2非接觸測高蓋關閉檢知))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Home.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                if (!GetInBit(inBit_Z1刀輪護蓋關閉))
                    ShowAlarm("I", 17);
                if (!GetInBit(inBit_Z2刀輪護蓋關閉))
                    ShowAlarm("I", 18);
                if (!GetInBit(inBit_Z1非接觸測高蓋關閉檢知))
                    ShowAlarm("I", 19);
                if (!GetInBit(inBit_Z2非接觸測高蓋關閉檢知))
                    ShowAlarm("I", 20);

                //按繼續重新開關IO
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_換刀區安全門關閉_Run()
        {
            SetOutBit(outBit_換刀區安全門閉鎖, true);

            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_換刀區安全門到位_Run()
        {
            if (IsSimulation() == 0)
            {
                if (Timer_Home.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    SetOutBit(outBit_換刀區安全門閉鎖, false);

                    if (GetInBit(inBit_換刀區安全門解鎖檢知_Z1區) || GetInBit(inBit_換刀區安全門解鎖檢知_Z2區))
                    {
                        ShowAlarm("E", 50);
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                }
                else
                {
                    if (GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) && GetInBit(inBit_換刀區安全門鎖定檢知_Z2區) && bCutDoorSafe)
                    {
                        return FlowChart.FCRESULT.NEXT;
                    }
                }
            }
            else
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_IO檢查_動作結束_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #region 各軸歸零
        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_動作開始_Run()
        {
            if (iXMotorLinearMode == 1)//X軸搭配光學尺功能
            {
                //motor_切刀橫移馬達_Z1X.MotorType = MOTORTYPE.YASKAWA_ABS;
                //motor_切刀橫移馬達_Z2X.MotorType = MOTORTYPE.YASKAWA_ABS;
                //motor_切刀橫移馬達_Z1X.MotorType = MOTORTYPE.NORMAL;
                //motor_切刀橫移馬達_Z2X.MotorType = MOTORTYPE.NORMAL;
            }
            else
            {
                motor_切刀橫移馬達_Z1X.MotorType = MOTORTYPE.NORMAL;
                motor_切刀橫移馬達_Z2X.MotorType = MOTORTYPE.NORMAL;
            }

            motor_切割平台前後馬達Y.HomeReset();
            motor_切割平台旋轉馬達U.HomeReset();
            motor_切刀橫移馬達_Z1X.HomeReset();
            motor_切刀橫移馬達_Z2X.HomeReset();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X軸往兩側歸零_Run()
        {
            if (iXMotorLinearMode == 0)
            {
                //X 軸向兩側歸零
                motor_切刀橫移馬達_Z1X.HomeDirection = true;
                motor_切刀橫移馬達_Z2X.HomeDirection = true;
                motor_切刀橫移馬達_Z1X.HomeMode = HOMEMODE.LIMITSNR;
                motor_切刀橫移馬達_Z2X.HomeMode = HOMEMODE.LIMITSNR;
            }

            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_XY軸歸零_Run()
        {
            if (IsSimulation() != 0)
            {
                motor_切刀橫移馬達_Z1X.SetPos(0);
                motor_切刀橫移馬達_Z1X.SetEncoderPos(0);
                motor_切刀橫移馬達_Z2X.SetPos(0);
                motor_切刀橫移馬達_Z2X.SetEncoderPos(0);
                motor_切割平台前後馬達Y.SetPos(0);
                motor_切割平台前後馬達Y.SetEncoderPos(0);

                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                List<Motor> lstMotors = new List<Motor>()
                {
                    motor_切刀橫移馬達_Z1X, motor_切刀橫移馬達_Z2X, motor_切割平台前後馬達Y
                };

                if (MultiMotorsHome(lstMotors, Timer_Home)) return FlowChart.FCRESULT.NEXT;

                return FlowChart.FCRESULT.IDLE;
            }
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_XY軸重置位置_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
            {
                if (iXMotorLinearMode == 0)
                {
                    motor_切刀橫移馬達_Z1X.HomeReset();
                    motor_切刀橫移馬達_Z2X.HomeReset();
                }

                motor_切割平台前後馬達Y.SetPos(0);
                motor_切割平台前後馬達Y.SetEncoderPos(0);
                m_nHomeFlow = 2;
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_Xθ軸歸零參數設定_Run()
        {
            if (iXMotorLinearMode == 0)
            {
                //X 向中心歸零 (先 Z1)
                motor_切刀橫移馬達_Z1X.HomeDirection = false;
                motor_切刀橫移馬達_Z2X.HomeDirection = false;
                motor_切刀橫移馬達_Z1X.HomeMode = HOMEMODE.HOMESNRWITHZ;
                motor_切刀橫移馬達_Z2X.HomeMode = HOMEMODE.HOMESNRWITHZ;

                ////第二段慢速歸零
                motor_切刀橫移馬達_Z1X.JogHighSpeed = 50000;
                motor_切刀橫移馬達_Z1X.JogLowSpeed = 500;
                motor_切刀橫移馬達_Z1X.SetAcceleration(100000);
                motor_切刀橫移馬達_Z1X.SetDeceleration(100000);

                motor_切刀橫移馬達_Z2X.JogHighSpeed = 50000;
                motor_切刀橫移馬達_Z2X.JogLowSpeed = 500;
                motor_切刀橫移馬達_Z2X.SetAcceleration(100000);
                motor_切刀橫移馬達_Z2X.SetDeceleration(100000);
            }

            SetOutBit(outBit_U軸歸零功能開啟, true);
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_θ軸歸零啟動_Run()
        {
            //θ 軸 Home
            if (motor_切割平台旋轉馬達U.MotorType == MOTORTYPE.YASKAWA_ABS)
            {
                List<Motor> lstMotors = new List<Motor>()
                {
                    motor_切割平台旋轉馬達U
                };

                if (MultiMotorsHome(lstMotors, Timer_Home)) return FlowChart.FCRESULT.NEXT;

                return FlowChart.FCRESULT.IDLE;
            }
            else
            {
                if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
                {
                    SetOutBit(outBit_U軸歸零啟動, true);
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                return FlowChart.FCRESULT.IDLE;
            }
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X軸往中間歸零θ軸歸零_Run()
        {
            if (iXMotorLinearMode == 1)
            {
                if (IsSimulation() != 0 ||
                   (motor_切割平台旋轉馬達U.MotorType != MOTORTYPE.YASKAWA_ABS && inBit_切割平台U軸原點歸零訊號.Value) ||
                   motor_切割平台旋轉馬達U.MotorType == MOTORTYPE.YASKAWA_ABS)
                {
                    m_nHomeFlow = 3;
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (Timer_Home.On(SReadValue("馬達逾時時間").ToInt() * 10))
                    {
                        MotorOverTime(motor_切割平台旋轉馬達U.Text);
                        Timer_Home.Restart();
                    }
                }
            }
            else
            {
                bool bX1HomeOK = motor_切刀橫移馬達_Z1X.Home();
                bool bUHomeOK = inBit_切割平台U軸原點歸零訊號.Value;

                if (IsSimulation() != 0 || (bX1HomeOK && bUHomeOK))
                {
                    m_nHomeFlow = 3;
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (Timer_Home.On(SReadValue("馬達逾時時間").ToInt() * 10))
                    {
                        if (!bX1HomeOK) MotorOverTime(motor_切刀橫移馬達_Z1X.Text);
                        if (!bUHomeOK) MotorOverTime(motor_切割平台旋轉馬達U.Text);

                        Timer_Home.Restart();
                    }
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X1θ軸重置位置_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))  //Home 完成後等 50 ms 重置位置 (怕不穩定)
            {
                motor_切刀橫移馬達_Z1X.SetPos(0);
                motor_切刀橫移馬達_Z1X.SetEncoderPos(0);

                motor_切割平台旋轉馬達U.SetPos(0);
                motor_切割平台旋轉馬達U.SetEncoderPos(0);

                SetOutBit(outBit_U軸歸零啟動, false);
                SetOutBit(outBit_U軸歸零功能開啟, false);
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X1軸回到安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Home))
            {
                m_nHomeFlow = 4;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X2軸往中間歸零_Run()
        {
            if (iXMotorLinearMode == 1)
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (IsSimulation() != 0)
                {
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (MultiMotorsHome(new List<Motor> { motor_切刀橫移馬達_Z2X }, Timer_Home)) return FlowChart.FCRESULT.NEXT;

                    return FlowChart.FCRESULT.IDLE;
                }
            }
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X2軸重置位置_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))  //Home 完成後等 100 ms 重置位置 (怕不穩定)
            {
                motor_切刀橫移馬達_Z2X.SetPos(0);
                motor_切刀橫移馬達_Z2X.SetEncoderPos(0);
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_X2軸回到安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Home))
            {
                m_nHomeFlow = 5;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_Delay_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
                return FlowChart.FCRESULT.NEXT;
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_各軸歸零_動作結束_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #region 主軸歸零

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_Run()
        {
            m_nHomeFlow = 6;
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_動作開始_Run()
        {
            SetUseSpindle(); //主軸使用設定
            m_enHomeSpindleStatus = enHomeSpindleStatus.主軸狀態初始;

            if (IsSimulation() != 0)
            {
                m_enZ1SpindleStatus = enSpindleStatus.Run;
                m_enZ2SpindleStatus = enSpindleStatus.Run;
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_啟動_Run()
        {
            if (bUseZ1_Spindle && bUseZ2_Spindle) //使用雙軸
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    //運轉中之主軸代表正常，無載入料號，停主軸
                    if (PackageName == "")
                    {
                        Z1SpindleStop = true;
                        Z2SpindleStop = true;
                    }

                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2運轉中;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                else if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    //運轉中之主軸代表正常，無載入料號，停主軸
                    if (PackageName == "")
                    {
                        Z1SpindleStop = true;
                    }

                    //Z2需啟動測試看有無異常
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else if (m_enZ1SpindleStatus == enSpindleStatus.Stop && m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    //運轉中之主軸代表正常，無載入料號，停主軸
                    if (PackageName == "")
                    {
                        Z2SpindleStop = true;
                    }

                    //Z1需啟動測試看有無異常
                    Z1SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中;
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else //(Z1SpindleStatus == eSpindleStatus.Stop && Z2SpindleStatus == eSpindleStatus.Stop)
                {
                    //Z1、Z2需啟動測試看有無異常
                    //Woody 202001005
                    Z1SpindleStart = true;
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else if (bUseZ1_Spindle) //僅使用Z1
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                {
                    //運轉中之主軸代表正常，無載入料號，停主軸
                    if (PackageName == "")
                    {
                        Z1SpindleStop = true;
                    }

                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z1運轉中;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                //Z1需啟動測試看有無異常
                else if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
                {
                    Z1SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z1停止;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else //僅使用Z2
            {
                if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    //運轉中之主軸代表正常，無載入料號，停主軸
                    if (PackageName == "")
                    {
                        Z2SpindleStop = true;
                    }

                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z2運轉中;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                //Z2需啟動測試看有無異常
                else if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z2停止;

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_啟動中_Run()
        {
            switch (m_enHomeSpindleStatus)
            {
                case enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting && m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z1停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;
            }

            if (Timer_Home.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_主軸動作完成_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_主軸運轉確認_Run()
        {
            switch (m_enHomeSpindleStatus)
            {
                case enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z1停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Home.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                default:
                    return FlowChart.FCRESULT.IDLE;
            }

            if (Timer_Home.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_等候主軸運轉_Run()
        {
            // 等主軸運轉一段時間
            if (Timer_Home.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_主軸停止_Run()
        {
            if (PackageName == "")
            {
                switch (m_enHomeSpindleStatus)
                {
                    case enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止:
                        if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                        {
                            Z2SpindleStop = true;
                        }
                        break;

                    case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中:
                        if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                        {
                            Z1SpindleStop = true;
                        }
                        break;

                    case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止:
                        if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Run)
                        {
                            Z1SpindleStop = true;
                            Z2SpindleStop = true;
                        }
                        break;

                    case enHomeSpindleStatus.開啟單軸_Z1停止:
                        if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                        {
                            Z1SpindleStop = true;
                        }
                        break;

                    case enHomeSpindleStatus.開啟單軸_Z2停止:
                        if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                        {
                            Z2SpindleStop = true;
                        }
                        break;
                }
            }

            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_主軸關閉_Run()
        {
            if (PackageName == "")
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Stop && m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                //主軸關閉是否逾時
                else if (Timer_Home.On(SReadValue("主軸動作時間").ToInt()))
                {
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }
            else
            {
                if (MonitorSpindle())
                {
                    if (OReadValue("DryRun").ToBoolean() == false)
                    {
                        //SetOutBit(outBit_真空幫浦啟動, true);
                        SetOutBit(outBit_鼓風機啟動, true);
                    }

                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_Delay_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_主軸啟動失敗_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
            {
                ShowAlarm("E", 17);
                Timer_Home.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_主軸歸零_動作結束_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #region 視覺載入Job

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_Run()
        {
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_開始_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_模擬_Run()
        {
            if (IsSimulation() == 0 && PackageName != "")
            {
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_讀取_Run()
        {
            string cmd = "LF";
            string para = PReadValue("視覺軟體程式名稱").ToString();
            if (SetCCDCmd(cmd, para))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Home.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 30);
                Timer_Home.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_等待讀取_Run()
        {
            switch (mCCDAction)//等待LF
            {
                case CCDSendResult.OK:
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    //+ By Max 20210316, v4.0.1.55, Vision LoadJob Failed
                    if (!bLoadJobSkipFirstTime)
                        ShowAlarm("w", 31);
                    bLoadJobSkipFirstTime = false;
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }

            if (Timer_Home.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 32);
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_新增_Run()
        {
            string cmd = "SF";
            string para = PReadValue("視覺軟體程式名稱").ToString();
            if (SetCCDCmd(cmd, para))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Home.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 29);
                Timer_Home.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_等待新增_Run()
        {
            switch (mCCDAction)//等待SF
            {
                case CCDSendResult.OK:
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    ShowAlarm("w", 22);//視覺儲存料號失敗
                    Timer_Home.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            if (Timer_Home.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 23);
                Timer_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_視覺載入Job_動作結束_Run()
        {
            m_nHomeFlow = 7;
            Timer_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        private FlowChart.FCRESULT FC_SAW歸零_Delay_Run()
        {
            if (Timer_Home.On(SReadValue("馬達整定時間").ToInt()))
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW歸零_動作結束_Run()
        {
            mHomeOk = true;
            return FlowChart.FCRESULT.IDLE;
        }

        #endregion 歸零流程

        #region 自動流程

        #region 接收/執行

        private FlowChart.FCRESULT FC_SAW自動_動作開始_Run()
        {
            //+ By Max, v4.0.1.99
            iReSimuProcessRunCount = 0;
            iReSimuProcessRunMP = 0;
            OSetValue("bSimuProcess", false);

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW自動_初始設定_Run()
        {
            Auto_ActionNo = 1;
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        //+ By Max, 4.0.1.99
        private int iReSimuProcessRunCount = 0;
        private int iReSimuProcessRunMP = 0;
        private bool bIsSimuProcess = false;

        private FlowChart.FCRESULT FC_SAW自動_接收動作命令_Run()
        {
            if (mLotend)
                return FlowChart.FCRESULT.CASE1;

            switch (Auto_ActionNo)
            {
                case 1: //auto
                    FC_自動_開始.TaskReset();
                    break;
            }

            if (Auto_ActionNo != 0)
            {
                Auto_ActionResult = 0;

                bIsSimuProcess = OReadValue("bSimuProcess").ToBoolean();

                //+ By Max, 4.0.1.99
                if (bIsSimuProcess)
                {
                    if (iReSimuProcessRunCount == 0 && iReSimuProcessRunMP == 0)
                    {
                        iReSimuProcessRunCount = OReadValue("SimuProcessCycle").ToInt();
                        iReSimuProcessRunMP = OReadValue("SimuProcessMP").ToInt() + 1;
                    }
                }

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_SAW自動_動作執行_Run()
        {
            if (m_AutoFlowChartEndEvt.WaitOne(0))
            {
                m_bIsNeedCheckVaca = false;
                m_bIsPauseNeedToContinue = false;
                //+ By Max 20210312, v4.0.1.53
                SetContinueRun(false);

                ActionStopFinish = false;

                //+ By Max 20210206
                if (DelegateDrawLineData != null)
                    DelegateDrawLineData(s切割_NowSide, i切割_Z1_NowLineNo, i切割_Z2_NowLineNo, i切割_Now百分比, true);//4.0.1.23清除畫布

                //+ By Max 20210310 
                //處理下刀點確認時強制排除變數重置
                TargetCheckStart = false;
                TargetCheckFinish = true;

                //處理動態刀痕時強制排除變數重置
                bdynamicCutOffset = false;
                bdynamicCutOffset_run = false;
                bdynamicCutOffsetFinish = true;

                FC_初始化_開始.TaskReset();
                return FlowChart.FCRESULT.CASE1;
            }

            if (Auto_ActionResult == 1)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            switch (Auto_ActionNo)
            {
                case 1: //Auto
                    if (!m_bIsPauseNeedToContinue)
                    {
                        //SpindleError = MonitorSpindle();
                        //BladeBrokenError = KnifeBreakCheck();
                        VaccError = CheckVaca(0);

                        //if (SpindleError != "")
                        //{
                        //    ShowAlarm("E", 70, SpindleError);
                        //}

                        //if (BladeBrokenError != "")
                        //{
                        //    ShowAlarm("E", 70, BladeBrokenError);
                        //}
                        //v4.0.1.17
                        if (m_bIsNeedCheckVaca && VaccError != "")
                        {
                            ShowAlarm("E", 70, VaccError);
                        }
                    }

                    #region 主軸負載檢查

                    if (m_flZ1LoadAmpMax > 80 || m_flZ1LoadMotorMax > 80)
                    {
                        ShowAlarm("w", 55, "Z1");
                    }

                    if (m_flZ2LoadAmpMax > 80 || m_flZ2LoadMotorMax > 80)
                    {
                        ShowAlarm("w", 55, "Z2");
                    }

                    m_flZ1LoadAmpMax = 0;
                    m_flZ2LoadAmpMax = 0;
                    m_flZ1LoadMotorMax = 0;
                    m_flZ2LoadMotorMax = 0;

                    #endregion

                    FC_自動_開始.MainRun();
                    break;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW自動_完成資料更新_Run()
        {
            m_bIsNeedCheckVaca = false;//自動完成
            Auto_ActionNo = 1;

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW自動_終止後初始化_Run()
        {
            FC_初始化_開始.MainRun();

            if (ActionStopFinish)
            {
                Auto_ActionResult = 2;
                Auto_ActionNo = 1;
                ShowAlarm("i", 16);
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_SAW自動_結批_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW自動_資料更新_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_SAW自動_動作結束_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.IDLE;
        }

        #endregion 接收/執行

        #region 自動

        private FlowChart.FCRESULT FC_自動_開始_Run()
        {
            enUseSpidleType type = (enUseSpidleType)(OReadValue("iUseSpindleType").ToInt());

            switch (type)
            {
                case enUseSpidleType.雙刀:
                    bUseZ1_Spindle = true;
                    bUseZ2_Spindle = true;
                    break;

                case enUseSpidleType.Z1單刀:
                    bUseZ1_Spindle = true;
                    bUseZ2_Spindle = false;
                    break;

                case enUseSpidleType.Z2單刀:
                    bUseZ1_Spindle = false;
                    bUseZ2_Spindle = true;
                    break;
            }

            TeachFixture.Reset();
            LearnMarkPos.Reset();
            AutoMark.Reset();
            AutoCut.Reset();
            Timer_Auto.Restart();

            //+ By Max 20210222, YieldCutting, 切割前檢查刀序與X軸相對距離
            iYieldCuttingCountdown = 0;
            bIsYieldCutting = false;

            return FlowChart.FCRESULT.NEXT;
        }

        #region 初始化

        private FlowChart.FCRESULT FC_自動_初始化_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_初始化_動作開始_Run()
        {
            string s = "";

            if (!SReadValue("ByPassZ1接觸測高").ToBoolean() && !Z1CheckBeforeStart.ZSparkTestDone)
            {
                s += "," + "Z1接觸測高未完成";
            }
            if (!SReadValue("ByPassZ2接觸測高").ToBoolean() && !Z2CheckBeforeStart.ZSparkTestDone)
            {
                s += "," + "Z2接觸測高未完成";
            }
            if (!SReadValue("ByPassZ1非接觸測高").ToBoolean() && !Z1CheckBeforeStart.ZNCTestDone)
            {
                s += "," + "Z1非接觸測高未完成";
            }
            if (!SReadValue("ByPassZ2非接觸測高").ToBoolean() && !Z2CheckBeforeStart.ZNCTestDone)
            {
                s += "," + "Z2非接觸測高未完成";
            }
            if (!SReadValue("ByPassZ1基準線校正").ToBoolean() && !Z1CheckBeforeStart.ZKerfCheckDone)
            {
                s += "," + "Z1基準線校正未完成";
            }
            if (!SReadValue("ByPassZ2基準線校正").ToBoolean() && !Z2CheckBeforeStart.ZKerfCheckDone)
            {
                s += "," + "Z2基準線校正未完成";
            }

            if (s != "")
            {
                ShowAlarm("w", 43, s);
                return FlowChart.FCRESULT.IDLE;
            }

            bool bloadFixture = Load_FixtureData();
            if (!bloadFixture)
            {
                ShowAlarm("E", 8);//切割道資料未建立，請先學習切割道！
                return FlowChart.FCRESULT.IDLE;
            }

            bool bloadPCB = Load_PCBData();
            if (!bloadPCB && !SReadValue("ByPass靶點學習").ToBoolean())
            {
                ShowAlarm("E", 2);//靶點資料未建立，請先學習靶點
                return FlowChart.FCRESULT.IDLE;
            }

            bool bloadCut = Load_CutData();
            if (!bloadCut)
            {
                ShowAlarm("E", 11);//切割刀序資料未建立，請建立刀序!
                return FlowChart.FCRESULT.IDLE;
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;

        }

        private FlowChart.FCRESULT FC_自動_初始化_速度設定_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_初始化_測高訊號關閉_Run()
        {
            if (GetSparkOff()) //自動流程初始化
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Auto.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_初始化_Z移至安全高度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_初始化_IO初始化_Run()
        {
            #region CCD/NCS
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_非接觸測高清潔水柱, false);
            SetOutBit(outBit_非接觸測高清潔氣旋, false);
            #endregion

            #region Spindle
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_Z1刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z2刀側邊清潔氣旋, false);
            SetOutBit(outBit_主軸軸承鎖定, false);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);
            #endregion

            #region Table
            SetOutBit(outBit_切割區增濕水簾, false);
            SetOutBit(outBit_切割區水霧隔絕氣簾, false);
            SetOutBit(outBit_水槽清潔噴水啟動, false);
            SetOutBit(outBit_水槽清潔吹氣啟動, false);
            SetOutBit(outBit_清潔風刀, false);

            if (!bIsSimuProcess || (bIsSimuProcess && iReSimuProcessRunCount == OReadValue("SimuProcessCycle").ToInt()))
            {
                SetOutBit(outBit_切割平台真空建立, false);
            }
            #endregion

            SetOutBit(outBit_換刀區安全門閉鎖, true);

            bCanPutRunSate = false;
            bCanGetRunSate = false;
            bCanFrontGetRun = false;

            SetOutBit(outBit_ManualLoadOK, false);

            bool bIsZ1刀輪護蓋關閉完成 = GetInBit(inBit_Z1刀輪護蓋關閉);
            bool bIsZ2刀輪護蓋關閉完成 = GetInBit(inBit_Z2刀輪護蓋關閉);

            if (bIsZ1刀輪護蓋關閉完成 && bIsZ2刀輪護蓋關閉完成)
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Auto.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                if (!bIsZ1刀輪護蓋關閉完成 && !bIsZ2刀輪護蓋關閉完成)
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text + "," + outBit_Z2刀輪護蓋關閉.Text);
                else if (!bIsZ1刀輪護蓋關閉完成)
                    ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text);
                else
                    ShowAlarm("i", 1, outBit_Z2刀輪護蓋關閉.Text);
                Timer_Auto.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_初始化_XY移至安全位置_Run()
        {
            if (SReadValue("AllAxisMoveSafe").ToBoolean())
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                    {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                    {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
                };

                if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.CASE1;

                return FlowChart.FCRESULT.IDLE;
            }
        }

        private FlowChart.FCRESULT FC_自動_初始化_U旋轉_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_初始化_其他軸移至初始_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_初始化_動作結束_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion 初始化

        #region 接觸測高

        private FlowChart.FCRESULT FC_自動_接觸測高_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_接觸測高_Z1開始_Run()
        {
            if (m_bIsAutoNeedSparkTestZ1)
            {
                SparkSelect("Z1");

                //+ By Max 20210314, v4.0.1.53, 調整Log紀錄位置
                LogSay(EnLoggerType.EnLog_SPC, "Auto-Z1接觸測高開始");
                FC_接觸測高_動作開始.TaskReset();
            }
            else m_AutoSparkTestFinishEvt.Set();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_接觸測高_Z1完成_Run()
        {
            if (m_AutoSparkTestFinishEvt.WaitOne(0))
            {
                //+ By Max 20210314, v4.0.1.53, 增加判斷式以分辨是否執行接觸測高流程
                if (m_bIsAutoNeedSparkTestZ1)
                    LogSay(EnLoggerType.EnLog_SPC, "Auto-Z1接觸測高完成");

                m_bIsAutoNeedSparkTestZ1 = false;
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_接觸測高_動作開始.MainRun();

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_接觸測高_Z2開始_Run()
        {
            if (m_bIsAutoNeedSparkTestZ2)
            {
                SparkSelect("Z2");

                //+ By Max 20210314, v4.0.1.53, 調整Log紀錄位置
                LogSay(EnLoggerType.EnLog_SPC, "Auto-Z2接觸測高開始");
                FC_接觸測高_動作開始.TaskReset();
            }
            else m_AutoSparkTestFinishEvt.Set();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_接觸測高_Z2完成_Run()
        {
            if (m_AutoSparkTestFinishEvt.WaitOne(0))
            {
                //+ By Max 20210314, v4.0.1.53, 增加判斷式以分辨是否執行接觸測高流程
                if (m_bIsAutoNeedSparkTestZ2)
                    LogSay(EnLoggerType.EnLog_SPC, "Auto-Z2接觸測高完成");

                m_bIsAutoNeedSparkTestZ2 = false;
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_接觸測高_動作開始.MainRun();

            return default(FlowChart.FCRESULT);
        }

        #endregion 接觸測高

        #region 非接觸測高

        private FlowChart.FCRESULT FC_自動_非接觸測高_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_非接觸測高_開始_Run()
        {
            if (m_bIsAutoNeedNContactTestZ1 || m_bIsAutoNeedNContactTestZ2)
            {
                //+ By Max 20210314, v4.0.1.53, 調整Log紀錄位置
                LogSay(EnLoggerType.EnLog_SPC, "Auto-非接觸測高開始Z1{0},Z2{1}", m_bIsAutoNeedNContactTestZ1.ToString(), m_bIsAutoNeedNContactTestZ2.ToString());
                FC_非接觸測高_動作開始.TaskReset();
            }
            else m_AutoNContactTestFinishEvt.Set();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_非接觸測高_完成_Run()
        {
            if (m_AutoNContactTestFinishEvt.WaitOne(0))
            {
                //+ By Max 20210314, v4.0.1.53, 增加判斷式以分辨是否執行非接觸測高流程
                if (m_bIsAutoNeedNContactTestZ1 || m_bIsAutoNeedNContactTestZ2)
                    LogSay(EnLoggerType.EnLog_SPC, "Auto-非接觸測高完成Z1{0},Z2{1}", m_bIsAutoNeedNContactTestZ1.ToString(), m_bIsAutoNeedNContactTestZ2.ToString());
                m_bIsAutoNeedNContactTestZ1 = false;
                m_bIsAutoNeedNContactTestZ2 = false;
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_非接觸測高_動作開始.MainRun();
            return default(FlowChart.FCRESULT);
        }

        #endregion 非接觸測高

        private FlowChart.FCRESULT FC_自動_入料前清洗_Run()
        {
            bool bIsClean = PCBIncount % PReadValue("CleanrBeforeLoad_freq").ToInt() == 0;
            if (bIsClean && PReadValue("bCleanBeforeLoad").ToBoolean())
            {
                m_enCleanType = enCleanType.入料前清洗;
                FC_清洗_動作開始.TaskReset();

                #region By Wolf 20210816, v4.0.1.91, 避免水殘餘至管路,造成真空開啟後管路結冰,導至載台無法吸料片
                SetOutBit(outBit_切割平台真空建立, true);
                #endregion
            }
            else
                m_CleanFinishEvt.Set();

            LogSay(EnLoggerType.EnLog_SPC, "Auto-入料前清洗開始");
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料前清洗完成_Run()
        {
            if (m_CleanFinishEvt.WaitOne(0))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-入料前清洗完成");
                Timer_Auto.Restart();

                #region By Wolf 20210819, v4.0.87.1, 避免水殘餘至管路,造成真空開啟後管路結冰,導至載台無法吸料片
                if (!bIsSimuProcess || (bIsSimuProcess && iReSimuProcessRunCount == OReadValue("SimuProcessCycle").ToInt()))
                {
                    SetOutBit(outBit_切割平台真空建立, false);
                }
                #endregion

                //+ By Max, v4.0.1.99, For SimuProcess
                if (!bIsSimuProcess || iReSimuProcessRunCount == OReadValue("SimuProcessCycle").ToInt())
                {
                    return FlowChart.FCRESULT.NEXT;
                }
                return FlowChart.FCRESULT.CASE1;
            }
            else
                FC_清洗_動作開始.MainRun();
            return default(FlowChart.FCRESULT);
        }

        #region 入料

        private FlowChart.FCRESULT FC_自動_入料_移至入料位置_Run()
        {
            iMoveHadinY = SReadValue("基準入料點位_Y").ToInt() + PReadValue("PCBIn_OffsetY").ToInt();
            //v4.0.1.28 入料/出料角度可自行設定
            iMoveHadinU = SReadValue("基準入料點位_θ").ToInt()
                + (int)((double)iDD馬達一轉Pulse數 * PReadValue("PCBIn_Offsetθ").ToDouble() / 360)
                + (iDD馬達一轉Pulse數 * PReadValue("iRotateInMode").ToInt() / 4) * (PReadValue("bAntiClockwise").ToBoolean() ? -1 : 1);

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iMoveHadinY},
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_入料_入料模式_Run()
        {
            enLoadMode mode = (enLoadMode)(OReadValue("LoadMode").ToInt());

            if (m_ForceOutContinueEvt.WaitOne(0)) // 強排續作
            {
                mode = enLoadMode.手動;
            }

            LogSay(EnLoggerType.EnLog_SPC, string.Format("Auto-{0}入料開始", mode.ToString()));
            Timer_Auto.Restart();

            if (mode == enLoadMode.自動) return FlowChart.FCRESULT.NEXT;
            else return FlowChart.FCRESULT.CASE1;
        }

        #region 自動入料

        private FlowChart.FCRESULT FC_自動_入料_自動入料_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料_入料補償_Run()
        {
            SendData("LoadStrip");
            outBit_LoadStrip.On();//v4.0.1.25
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料_入料補償完成_Run()
        {
            //SendData("LoadStrip");
            if ((bool)GetResult("LoadOffset"))
            {
                iMoveHadinY += iLowerCCDCompensation_Y;
                iMoveHadinU += iLowerCCDCompensation_θ;

                ResetResult("LoadOffset");
                outBit_LoadStrip.Off();//v4.0.1.25
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_入料_發出訊號要求入料_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iMoveHadinY},
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                iChkSawTableType = 0;
                bCanPutRunSate = true;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_入料_手臂通知入料完成_Run()
        {
            if ((bool)GetResult("PutStripFinish"))
            {
                //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
                if (bAirPumpStd)
                {
                    PumpOpenTime = PReadValue("PCBVacuumTime").ToInt();
                }
                else
                {
                    PumpOpenTime = SReadValue("iAirPumpSTartTimes").ToInt();
                }

                SetOutBit(outBit_切割平台真空建立, true);

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_入料_真空檢查_Run()
        {
            if (CheckVaca(0) == "")
            {
                ResetResult("PutStripFinish");
                m_bIsNeedCheckVaca = true;//自動流程
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
            else if (Timer_Auto.On(PumpOpenTime))
            {
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                ShowAlarm("w", 44, GetChuckTableAIValue().ToString(), PReadValue("PCBVacuumThreshold").ToString());
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_入料_通知手臂可移開_Run()
        {
            SendData("PutCanLeave");
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料_手臂移至安全位置_Run()
        {
            //+ By Max 20210329, v4.0.1.59，增加檢查I/O的狀態
            if (!IsHADINInProcess() && (bool)GetResult("PutLeaveFinish"))
            {
                ResetResult("PutLeaveFinish");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        #endregion 自動入料

        #region 手動入料

        private FlowChart.FCRESULT FC_自動_入料_手動入料_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料_通知人員入料_Run()
        {
            //+ By Max 20210325, v4.0.1.58 DryRun
            if (!OReadValue("bDryRun").ToBoolean())
                ShowAlarm("i", 17, "請手動放入料片");
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_入料_真空檢查2_Run()
        {
            SetOutBit(outBit_切割平台真空建立, true);//真空開啟

            if (CheckVaca(0) == "")
            {
                m_bIsNeedCheckVaca = true;//自動流程
                outBit_ManualLoadOK.On();//手動入料完成

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Auto.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                ShowAlarm("w", 44, GetChuckTableAIValue().ToString(), PReadValue("PCBVacuumThreshold").ToString());

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #endregion 手動入料

        private FlowChart.FCRESULT FC_自動_入料_入料完成_Run()
        {
            bCanPutRunSate = false;
            PCBIncount++;

            LogSay(EnLoggerType.EnLog_SPC, "Auto-入料完成");

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion 入料

        private FlowChart.FCRESULT FC_自動_是否掃靶_Run()
        {
            if (PReadValue("SearchMark").ToBoolean())
            {
                m_enScanMethod = (enScanTargetMethod)(PReadValue("ScanMethod").ToInt());

                var _dicLookUp = new Dictionary<enScanTargetMethod, Action>()
                {
                     {enScanTargetMethod.四點掃靶,        () => FC_四點掃靶_掃靶開始.TaskReset()},
                     {enScanTargetMethod.半靶掃靶,        () => FC_全靶_N字形_掃靶開始.TaskReset()},
                     {enScanTargetMethod.全靶掃靶_口字形, () => FC_全靶_N字形_掃靶開始.TaskReset()},
                     {enScanTargetMethod.全靶掃靶_N字形,  () => FC_全靶_N字形_掃靶開始.TaskReset()},
                     {enScanTargetMethod.三點掃靶,        () => FC_三點掃靶_掃靶開始.TaskReset()}
                };

                if (_dicLookUp.ContainsKey(m_enScanMethod)) _dicLookUp[m_enScanMethod].Invoke();

                m_bIsTargetScanning = true; // 掃靶進行中
                LogSay(EnLoggerType.EnLog_SPC, "Auto-" + m_enScanMethod.ToString() + "開始");
            }
            else
            {
                AutoMark.dtScanData = LearnMarkPos.dtScanData;
                LogSay(EnLoggerType.EnLog_SPC, "Auto-不掃靶(依切割道)開始");

                m_SearchTargetDoneEvt.Set();
            }

            m_SwTimeLog.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_掃靶完成_Run()
        {
            if (m_SearchTargetDoneEvt.WaitOne(0))
            {
                m_bIsTargetScanning = false; // 掃靶已完成
                LogSay(EnLoggerType.EnLog_SPC, "Auto-掃靶完成");

                #region 更新UI上的掃描耗時
                m_SwTimeLog.Stop();
                m_TimeLog.dScanTime = Convert.ToDouble(m_SwTimeLog.ElapsedMilliseconds / 1000.0f);
                #endregion

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                m_enScanMethod = (enScanTargetMethod)(PReadValue("ScanMethod").ToInt());

                var _dicLookUp = new Dictionary<enScanTargetMethod, Action>()
                {
                     {enScanTargetMethod.四點掃靶,        () => FC_四點掃靶_掃靶開始.MainRun()},
                     {enScanTargetMethod.半靶掃靶,        () => FC_全靶_N字形_掃靶開始.MainRun()},
                     {enScanTargetMethod.全靶掃靶_口字形, () => FC_全靶_N字形_掃靶開始.MainRun()},
                     {enScanTargetMethod.全靶掃靶_N字形,  () => FC_全靶_N字形_掃靶開始.MainRun()},
                     {enScanTargetMethod.三點掃靶,        () => FC_三點掃靶_掃靶開始.MainRun()}
                };

                if (_dicLookUp.ContainsKey(m_enScanMethod)) _dicLookUp[m_enScanMethod].Invoke();
            }

            return default(FlowChart.FCRESULT);
        }

        #region 掃靶

        #region 四點掃靶

        private FlowChart.FCRESULT FC_四點掃靶_掃靶開始_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_ResetIO開啟_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            AutoMark.strSide = "CH2";
            AutoMark.nFindMarkStep = 0;

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_移至靶點_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            if (dv.Count == 0)
            {
                ShowAlarm("E", 2);//靶點資料未建立，請先學習靶點
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.IDLE;
            }

            AutoMark.nHighBaseFocus = PReadValue("iZ2_FoucusPos").ToInt();

            string strTmp = AutoMark.strSide == "CH1" ? "bCH1Between" : "bCH2Between";

            switch (AutoMark.nFindMarkStep)
            {
                case 0://CH2 左上角
                case 4://CH1 左上角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X1"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[0]["Scan_Angle"]);
                    break;
                case 1://CH2 左下角
                case 5://CH1 左下角
                    AutoMark.nTempX = Convert.ToInt32(dv[0]["Scan_X2"]);
                    AutoMark.nTempY = Convert.ToInt32(dv[0]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[0]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[0]["Scan_Angle"]);
                    break;
                case 2://CH2 右上角
                case 6://CH1 右上角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        // 內縮一行
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X1"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y1"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[dv.Count - 1]["Scan_Angle"]);
                    break;
                case 3://CH2 右下角
                case 7://CH1 右下角
                    if (PReadValue(strTmp).ToBoolean())
                    {
                        // 內縮一行
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 2]["Scan_X2"]);
                    }
                    else
                    {
                        AutoMark.nTempX = Convert.ToInt32(dv[dv.Count - 1]["Scan_X2"]);
                    }
                    AutoMark.nTempY = Convert.ToInt32(dv[dv.Count - 1]["Scan_Y2"]);
                    //AutoMark.HighBaseFocus = Convert.ToInt32(dv[dv.Count - 1]["Scan_Z"]);
                    AutoMark.nTempθ = Convert.ToInt32(dv[dv.Count - 1]["Scan_Angle"]);
                    break;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoMark.nTempθ},
                {motor_切割平台前後馬達Y, AutoMark.nTempY},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  AutoMark.nTempX},
                {motor_切刀上下馬達_Z1,  SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2,  AutoMark.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                if (AutoMark.nFindMarkStep == 0)
                {
                    //+ By Max, v4.0.1.99
                    #region 更新UI上的掃描耗時
                    m_SwTimeLog.Stop();
                    m_TimeLog.dLoadTime = Convert.ToDouble(m_SwTimeLog.ElapsedMilliseconds / 1000.0f);
                    m_SwTimeLog.Restart();
                    #endregion
                    return FlowChart.FCRESULT.CASE1;
                }
                else
                {
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_四點掃靶_自動對焦_Run()
        {
            AutoFocusFinish = true;

            //是否使用自動對焦
            if (SReadValue("bUseChkAutoFocus").ToBoolean())
            {
                enFocusMode enMode = (enFocusMode)(SReadValue("iAutoFocusChkCycle").ToInt());

                if ((enMode == enFocusMode.首片 && PCBIncount == 1) || enMode == enFocusMode.每片)
                {
                    AutoFocusFinish = false;
                    CallAutoFocus = "Auto";

                    if (OReadValue("bFastAutoFocus").ToBoolean())
                    {
                        FC_教導_快速自動對焦_開始.TaskReset();
                    }
                    else
                    {
                        FC_教導_自動對焦_開始.TaskReset();//v4.0.1.25新增自動對焦
                    }
                }
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦開始");

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_自動對焦完成_Run()
        {
            if (AutoFocusFinish)
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-自動對焦完成");

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (OReadValue("bFastAutoFocus").ToBoolean())
                {
                    FC_教導_快速自動對焦_開始.MainRun();
                }
                else
                {
                    FC_教導_自動對焦_開始.MainRun();//v4.0.1.25
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_四點掃靶_視覺通訊_選擇圖案_Run()
        {
            return SelectPattern(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_四點掃靶_等待結果_Run()
        {
            switch (mCCDAction)//設定樣式
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 37);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }

            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 38);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_四點掃靶_NG_Run()
        {
            ShowAlarm("i", 14);//視覺設定圖案形狀失敗，請按繼續重試
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_開始定位_Run()
        {
            FC_自動_定位.TaskReset();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_定位完成_Run()
        {
            if (m_PositioningFinishEvt.WaitOne(0))
            {
                AutoMark.nFindMarkStep++;

                if (AutoMark.nFindMarkStep == 4 || AutoMark.nFindMarkStep == 8)//CH2與CH1掃靶完成
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            FC_自動_定位.MainRun();
            return default(FlowChart.FCRESULT);
        }

        //+ By Max 20210223, 掃靶後，產出掃靶Log資訊，方便比對切偏時相關資料
        StringBuilder sb = new StringBuilder();
        private FlowChart.FCRESULT FC_四點掃靶_計算_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";

            int GapLT = Convert.ToInt32(dv[0]["Scan_X1"].ToString()) - AutoMark.ptLTMarkPos.X;//左上角點偏差值
            int GapLB = Convert.ToInt32(dv[0]["Scan_X2"].ToString()) - AutoMark.ptLBMarkPos.X;//左下角點偏差值

            //靶點位置是否超出學靶位置，會切到切割道
            if (Math.Abs(GapLT) > PReadValue("Cut_X_tolerance").ToInt() || Math.Abs(GapLB) > PReadValue("Cut_X_tolerance").ToInt())
            {
                if (AutoMark.strSide == "CH2")
                {
                    ShowAlarm("i", 20, "HADIN", (GapLT + GapLB) / 2);
                }
                else if (AutoMark.strSide == "CH1")
                {
                    ShowAlarm("i", 20, "Y", (GapLT + GapLB) / 2);
                }
                return FlowChart.FCRESULT.IDLE;
            }

            Point LTBeforeRotate = AutoMark.ptLTMarkPos;
            Point LBBeforeRotate = AutoMark.ptLBMarkPos;
            Point RTBeforeRotate = AutoMark.ptRTMarkPos;
            Point RBBeforeRotate = AutoMark.ptRBMarkPos;

            //+ By Max, 4.0.1.99
            sb.Clear();
            if (bIsSimuProcess)
            {
                String strLog = String.Format("ReSimu Process Countdown: {0}", iReSimuProcessRunCount);
                LogSay(EnLoggerType.EnLog_SPC, strLog);
                sb.AppendFormat("Channel = {0}, LTPosX = {1}, LTPosY = {2} ", AutoMark.strSide, LTBeforeRotate.X, LTBeforeRotate.Y);
                sb.AppendFormat("RTPosX = {0}, RTPosY = {1} ", RTBeforeRotate.X, RTBeforeRotate.Y);
                sb.AppendFormat("LBPosX = {0}, LBPosY = {1} ", LBBeforeRotate.X, LBBeforeRotate.Y);
                sb.AppendFormat("RBPosX = {0}, RBPosY = {1}", RBBeforeRotate.X, RBBeforeRotate.Y);
                LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
            }

            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            Double angleL = 0;
            Double angleR = 0;
            Double fRotateAngle = 0;

            //左邊角度運算
            RotateCorrect(Org_X, Org_Y, ref AutoMark.ptLTMarkPos, ref AutoMark.ptLBMarkPos, ref angleL);
            int i_angleL = Convert.ToInt32((angleL / 360) * iDD馬達一轉Pulse數);
            AutoMark.nLeftθ = AutoMark.nTempθ + i_angleL;

            //右邊角度運算
            RotateCorrect(Org_X, Org_Y, ref AutoMark.ptRTMarkPos, ref AutoMark.ptRBMarkPos, ref angleR);
            int i_angleR = Convert.ToInt32((angleR / 360) * iDD馬達一轉Pulse數);
            AutoMark.nRightθ = AutoMark.nTempθ + i_angleR;

            fRotateAngle = (angleL + angleR) / 2;

            AutoMark.nTempθ = (AutoMark.nLeftθ + AutoMark.nRightθ) / 2;

            RotatePoint(Org_X, Org_Y, ref LTBeforeRotate, ref LBBeforeRotate, fRotateAngle);
            RotatePoint(Org_X, Org_Y, ref RTBeforeRotate, ref RBBeforeRotate, fRotateAngle);

            //計算兩點距離
            double DX1 = Math.Abs(RTBeforeRotate.X - LTBeforeRotate.X);//Math.Sqrt(Math.Pow((RT_BeforeRotate.X - LT_BeforeRotate.X), 2) + Math.Pow((RT_BeforeRotate.Y - LT_BeforeRotate.Y), 2));
            double DX2 = Math.Abs(RBBeforeRotate.X - LBBeforeRotate.X);//Math.Sqrt(Math.Pow((RB_BeforeRotate.X - LB_BeforeRotate.X), 2) + Math.Pow((RB_BeforeRotate.Y - LB_BeforeRotate.Y), 2));

            double DY1 = Math.Abs(RTBeforeRotate.Y - LTBeforeRotate.Y);//Math.Sqrt(Math.Pow((LT_BeforeRotate.X - LB_BeforeRotate.X), 2) + Math.Pow((LT_BeforeRotate.Y - LB_BeforeRotate.Y), 2));
            double DY2 = Math.Abs(RBBeforeRotate.Y - LBBeforeRotate.Y);//Math.Sqrt(Math.Pow((RT_BeforeRotate.X - RB_BeforeRotate.X), 2) + Math.Pow((RT_BeforeRotate.Y - RB_BeforeRotate.Y), 2));

            double SPCPitch = Math.Abs(Convert.ToInt32(dv[1]["Scan_X1"].ToString()) - Convert.ToInt32(dv[0]["Scan_X1"].ToString()));//SPC的Pitch

            double dX1Pitch, dX2Pitch, dY1Pitch, dY2Pitch;

            string strTmp = AutoMark.strSide == "CH1" ? "bCH1Between" : "bCH2Between";

            if (PReadValue(strTmp).ToBoolean())
            {
                dX1Pitch = DX1 / (Convert.ToInt32(dv.Count) - 2d);
                dX2Pitch = DX2 / (Convert.ToInt32(dv.Count) - 2d);
                dY1Pitch = DY1 / (Convert.ToInt32(dv.Count) - 2d);
                dY2Pitch = DY2 / (Convert.ToInt32(dv.Count) - 2d);
            }
            else
            {
                dX1Pitch = DX1 / (Convert.ToInt32(dv.Count) - 1d);
                dX2Pitch = DX2 / (Convert.ToInt32(dv.Count) - 1d);
                dY1Pitch = DY1 / (Convert.ToInt32(dv.Count) - 1d);
                dY2Pitch = DY2 / (Convert.ToInt32(dv.Count) - 1d);
            }

            //Pitch是否超出卡控值
            if (Math.Abs(dX1Pitch - SPCPitch) > PReadValue("Pitch_tolerance").ToInt() || Math.Abs(dX2Pitch - SPCPitch) > PReadValue("Pitch_tolerance").ToInt())
            {
                ShowAlarm("i", 9, AutoMark.strSide, dX1Pitch, SPCPitch);
                return FlowChart.FCRESULT.IDLE;
            }

            if (SReadValue("bUseChkDist").ToBoolean()) //+ By Max 20210218, 梯形卡控
            {
                double Dist_TX = TwoPointDist(AutoMark.ptLTMarkPos, AutoMark.ptRTMarkPos);
                double Dist_BX = TwoPointDist(AutoMark.ptLBMarkPos, AutoMark.ptRBMarkPos);
                if (Math.Abs(Dist_TX - Dist_BX) > SReadValue("iChkDistdistence").ToInt())
                {
                    ShowAlarm("i", 21, Dist_TX, Dist_BX, SReadValue("iChkDistdistence").ToInt());
                    return FlowChart.FCRESULT.IDLE;
                }
            }

            if (Math.Abs(fRotateAngle) > PReadValue("Cut_U_tolerance").ToDouble())
            {
                //double U_offset = Convert.ToInt32((-fRotateAngle / 360) * iDD馬達一轉Pulse數);
                ShowAlarm("w", 25, -fRotateAngle);
                return FlowChart.FCRESULT.IDLE;
            }

            //需再轉正Woody20200929
            DataRow dr = AutoMark.dtScanData.NewRow();

            //+ By Max 20210223, 掃靶後，產出掃靶Log資訊，方便比對切偏時相關資料
            sb.Clear();
            for (int i = 0; i < dv.Count; i++)
            {
                dr = AutoMark.dtScanData.NewRow();
                dr["Scan_LineNo"] = (i + 1).ToString();
                sb.Append(String.Format("LineNo: {0},", (i + 1)));
                dr["Scan_Side"] = AutoMark.strSide;
                sb.Append(String.Format("Side: {0},", AutoMark.strSide));
                dr["Scan_X1"] = (LTBeforeRotate.X + Convert.ToInt32(i * dX1Pitch)).ToString();
                sb.Append(String.Format("Scan_X1: {0},", (LTBeforeRotate.X + Convert.ToInt32(i * dX1Pitch))));
                dr["Scan_Y1"] = (LTBeforeRotate.Y + Convert.ToInt32(i * dY1Pitch)).ToString();
                sb.Append(String.Format("Scan_Y1: {0},", (LTBeforeRotate.Y + Convert.ToInt32(i * dY1Pitch))));
                dr["Scan_X2"] = (LBBeforeRotate.X + Convert.ToInt32(i * dX2Pitch)).ToString();
                sb.Append(String.Format("Scan_X2: {0},", (LBBeforeRotate.X + Convert.ToInt32(i * dX2Pitch))));
                dr["Scan_Y2"] = (LBBeforeRotate.Y + Convert.ToInt32(i * dY2Pitch)).ToString();
                sb.Append(String.Format("Scan_Y2: {0},", (LBBeforeRotate.Y + Convert.ToInt32(i * dY2Pitch))));
                dr["Scan_Z"] = motor_切刀上下馬達_Z2.ReadEncPos();
                sb.Append(String.Format("Scan_Z: {0},", (motor_切刀上下馬達_Z2.ReadEncPos())));
                dr["Scan_Angle"] = AutoMark.nTempθ.ToString();
                sb.Append(String.Format("Scan_Angle: {0},", AutoMark.nTempθ));
                if (PReadValue("TargetFind").ToInt() == 0)
                {
                    dr["Scan_XOffset"] = Convert.ToInt32(dv[0]["Scan_XOffset"].ToString());
                    sb.Append(String.Format("Scan_XOffset: {0}", Convert.ToInt32(dv[0]["Scan_XOffset"].ToString())));
                }
                else
                {
                    dr["Scan_XOffset"] = "0";
                    sb.Append(String.Format("Scan_XOffset: 0"));
                }
                LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                sb.Clear();
                AutoMark.dtScanData.Rows.Add(dr);
            }

            if (SReadValue("bIsLinearInterpolation").ToBoolean())
            {
                //+ By Max, 20210804
                LICuttingAngle(AutoMark.strSide, AutoMark.nLeftθ, AutoMark.nRightθ);
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        //+ By Max, 20210804
        private void LICuttingAngle(String sSide, int LeftAngle, int RightAngle)
        {
            DataView dv = new DataView(AutoMark.dtScanData);
            dv.RowFilter = "Scan_Side = '" + sSide + "'";
            double X0 = Convert.ToDouble(dv[0]["Scan_X1"]);
            double X1 = Convert.ToDouble(dv[dv.Count - 1]["Scan_X1"]);
            double Y0 = LeftAngle;
            double Y1 = RightAngle;
            for (int i = 0; i < dv.Count; ++i)
            {
                int X = Convert.ToInt32(dv[i]["Scan_X1"]);
                int Y = (int)((Y0 * (X1 - X) + Y1 * (X - X0)) / (X1 - X0));
                dv[i]["Scan_Angle"] = Y.ToString();
                LogSay(EnLoggerType.EnLog_SPC, String.Format("CH:{0}, Line:{1}, XPos:{2}, Angle:{3}", sSide, i, X, Y));
            }
        }

        private FlowChart.FCRESULT FC_四點掃靶_定位結束_Run()
        {
            if (AutoMark.nFindMarkStep == 8)//CH2與CH1掃靶完成
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                AutoMark.strSide = "CH1";
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

        }

        private FlowChart.FCRESULT FC_四點掃靶_切割點位設定_Run()
        {
            m_bIsVisionSaw = SReadValue("bVisionSaw").ToBoolean();//設定VisionSaw
            AutoCut.SafeZ = SReadValue("安全點位_Z2").ToInt();

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_四點掃靶_掃靶結束_Run()
        {
            m_SearchTargetDoneEvt.Set();
            //PSetTable("DGVScanData", AutoMark.ScanData);
            return default(FlowChart.FCRESULT);
        }

        #endregion

        #endregion

        private FlowChart.FCRESULT FC_自動_下刀點確認_Run()
        {
            //+ By Max 20210209，如未設定掃靶，則不執行下刀點確認
            if ((PCBIncount == 1 || bUseManualAlignment) && (PReadValue("SearchMark").ToBoolean() || IsSimulation() != 0))
            {
                //+ By Max 20210314, v4.0.1.53, 調整Log紀錄位置
                LogSay(EnLoggerType.EnLog_SPC, "Auto-下刀點確認開始");
                TargetCheckStart = true;
                TargetCheckFinish = false;
                FC_下刀點確認_開始.TaskReset();
            }
            else
            {
                TargetCheckFinish = true;
            }
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_下刀點確認完成_Run()
        {
            if (TargetCheckFinish)
            {
                //v4.0.1.41 關鏡頭遮蓋
                SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
                SetOutBit(outBit_高倍CCD鏡頭清潔, true);
                SetOutBit(outBit_高倍CCD視域範圍清潔, false);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_下刀點確認_開始.MainRun();
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_切割開始_Run()
        {
            //v4.0.1.39 Package中刀痕檢測改為切割
            if (!PReadValue("bUsecut").ToBoolean())
            {
                ShowAlarm("i", 24); //產品設定使用不切割，請確認
                m_SwTimeLog.Restart();
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            if (m_bIsVisionSaw)
            {
                //只用Z2做視覺切割
                DataTable NewTT2 = new DataTable();
                NewTT2 = Change_CutData(enSpindleSel.Z2);
                if (AutoCut.CutData != null)
                {
                    AutoCut.CutData.Dispose();
                    AutoCut.CutData = null;
                }
                AutoCut.CutData = NewTT2;
            }

            if (OReadValue("bSinkCleanWater").ToBoolean() && PackageName.Length != 0) outBit_水槽清潔噴水啟動.On();

            //+ By Max 20210312, v4.0.1.53, 刀序數量合理性檢查
            enCutSequenceCheck eRet = CutSequenceRationalityCheck();

            if (eRet == enCutSequenceCheck.CutSequenceOutOfRange)
            {
                return FlowChart.FCRESULT.IDLE;
            }
            else if (eRet == enCutSequenceCheck.CutLineOutOfRange)
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-切割開始");
                m_SwTimeLog.Restart();
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            //+ By Max 20210309, 是否顯示動態刀痕CheckBox
            bCanShowDynamicCutOffset = true;

            //+ By Max 20210331, v4.0.1.60, 修正主畫面切割時間顯示為0的Bugs
            m_SwTimeLog.Restart();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_Z軸移至安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, AutoCut.SafeZ},
                {motor_切刀上下馬達_Z2, AutoCut.SafeZ}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                i切割_Z1_NowLineNo = 0;
                i切割_Z2_NowLineNo = 0;
                i切割_Now百分比 = 0;
                SetSpeed_U(enSpeedMode.切割速度);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 主軸啟動

        private FlowChart.FCRESULT FC_自動_主軸啟動_Run()
        {
            if (m_bIsVisionSaw)
            {
                SetOutBit(outBit_Z1水開關電磁閥, false);
                SetOutBit(outBit_Z2水開關電磁閥, false);
                SetOutBit(outBit_切割區增濕水簾, false);

                SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
                SetOutBit(outBit_高倍CCD鏡頭清潔, true);
                SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            }
            else
            {
                SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
                SetOutBit(outBit_高倍CCD鏡頭清潔, true);
                SetOutBit(outBit_高倍CCD視域範圍清潔, false);

                analogOut_Z1噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z1);
                analogOut_Z1灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z1);
                analogOut_Z2噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z2);
                analogOut_Z2灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z2);

                //v4.0.1.17 Woody Close Water
                SetOutBit(outBit_Z1水開關電磁閥, true);
                SetOutBit(outBit_Z2水開關電磁閥, true);
                SetOutBit(outBit_切割區增濕水簾, true);
            }

            if (CheckWater(enSpindleSel.Z1) && CheckWater(enSpindleSel.Z2))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (Timer_Auto.On(SReadValue("水開啟準備時間").ToInt()))
            {
                if (!CheckWater(enSpindleSel.Z1))
                {
                    ShowAlarm("w", 14, "Z1"); //Z1噴水/灑水流量未到達
                }
                if (!CheckWater(enSpindleSel.Z2))
                {
                    ShowAlarm("w", 14, "Z2"); //Z2噴水/灑水流量未到達
                }
                Timer_Auto.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_開始_Run()
        {
            m_enHomeSpindleStatus = enHomeSpindleStatus.主軸狀態初始;
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_判斷狀態_Run()
        {
            if (bUseZ1_Spindle && bUseZ2_Spindle)//使用雙軸
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2運轉中;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                else if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    //Z2需啟動測試看有無異常
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else if (m_enZ1SpindleStatus == enSpindleStatus.Stop && m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    //Z1需啟動測試看有無異常
                    Z1SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else //(Z1SpindleStatus == eSpindleStatus.Stop && Z2SpindleStatus == eSpindleStatus.Stop)
                {
                    //Z1、Z2需啟動測試看有無異常
                    //Woody 202001005
                    Z1SpindleStart = true;
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else if (bUseZ1_Spindle)//僅使用Z1
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                {
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z1運轉中;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                //Z1需啟動測試看有無異常
                else if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
                {
                    Z1SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z1停止;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else//僅使用Z2
            {
                if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                {
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z2運轉中;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                //Z2需啟動測試看有無異常
                else if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    Z2SpindleStart = true;
                    m_enHomeSpindleStatus = enHomeSpindleStatus.開啟單軸_Z2停止;
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_啟動中_Run()
        {
            switch (m_enHomeSpindleStatus)
            {
                case enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting && m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z1停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;
            }

            if (Timer_Auto.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_失敗_Run()
        {
            ShowAlarm("E", 17);
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_完成_Run()
        {
            switch (m_enHomeSpindleStatus)
            {
                case enHomeSpindleStatus.開啟雙軸_Z1運轉中_Z2停止:
                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2運轉中:
                case enHomeSpindleStatus.開啟雙軸_Z1停止_Z2停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Run && m_enZ2SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z1停止:
                    if (m_enZ1SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                case enHomeSpindleStatus.開啟單軸_Z2停止:
                    if (m_enZ2SpindleStatus == enSpindleStatus.Run)
                    {
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    break;

                default:
                    return FlowChart.FCRESULT.IDLE;
            }

            if (Timer_Home.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Home.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_主軸啟動_結束_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #region 點位計算

        private FlowChart.FCRESULT FC_自動_點位計算_Run()
        {
            if (!MonitorSpindle() && !m_bIsVisionSaw)
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_點位計算_開始_Run()
        {
            if (!MonitorSpindle() && !m_bIsVisionSaw)
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private AutoResetEvent m_SeeAndCutFinishEvt = new AutoResetEvent(false);
        private FlowChart.FCRESULT FC_自動_點位計算_即看即切計算點位_Run()
        {
            NLogger.Debug("AutoCut.Step = " + AutoCut.Step.ToString());

            if (OReadValue("bSeeAndCut").ToBoolean() && Convert.ToBoolean(AutoCut.CutData.Rows[AutoCut.Step]["See"]))
            {
                FC_自動_即看即切_開始.TaskReset();
            }
            else
            {
                m_SeeAndCutFinishEvt.Set();
            }

            NLogger.Info("Auto-即看即切計算點位開始");
            LogSay(EnLoggerType.EnLog_SPC, "Auto-即看即切計算點位開始");
            Timer_Auto.Restart();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_點位計算_即看即切計算完成_Run()
        {
            if (m_SeeAndCutFinishEvt.WaitOne(0))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-即看即切計算完成");
                Timer_Auto.Restart();

                NLogger.Info("Auto-即看即切計算完成");
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_自動_即看即切_開始.MainRun();

            return default(FlowChart.FCRESULT);
        }

        //+ By Max 20210222, YieldCutting, 切割前檢查刀序與X軸相對距離
        private int iYieldCuttingCountdown = 0;
        private bool bIsYieldCutting = false;
        private FlowChart.FCRESULT FC_自動_點位計算_XYU點位計算_Run()
        {
            if (OReadValue("bSeeAndCut").ToBoolean() && !Convert.ToBoolean(AutoCut.CutData.Rows[AutoCut.Step]["Cut"]))
            {
                NLogger.Debug("AutoCut.Step = " + AutoCut.Step.ToString());
                NLogger.Debug("");

                AutoCut.Step++;
                return FlowChart.FCRESULT.CASE1;
            }

            NLogger.Debug("切割開始");

            VaccError = "";

            s切割_NowSide = AutoCut.CutData.Rows[AutoCut.Step]["Cut_料片方向"].ToString();

            DataView dv = new DataView(AutoMark.dtScanData);
            dv.RowFilter = "Scan_Side = '" + s切割_NowSide + "'";

            try
            {
                //是否使用Z1
                if (Convert.ToBoolean(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z1使用"]))
                {
                    i切割_Z1_NowLineNo = Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z1刀順"]) - 1;

                    //+ By Max 20210225, 內靶掃描
                    if (PReadValue("TargetFind").ToInt() == 0)
                    {
                        //+ By Max 20210225, 內靶最後一道，特別處理
                        if (i切割_Z1_NowLineNo == dv.Count)
                        {
                            double dPitch = (Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]) - Convert.ToInt32(dv[0]["Scan_X1"])) / (dv.Count - 1);
                            AutoCut.TempX1 = Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]) + (int)dPitch + Convert.ToInt32(dv[dv.Count - 1]["Scan_XOffset"]);
                        }
                        else
                        {
                            AutoCut.TempX1 = Convert.ToInt32(dv[i切割_Z1_NowLineNo]["Scan_X1"]) + Convert.ToInt32(dv[i切割_Z1_NowLineNo]["Scan_XOffset"]);
                        }
                    }
                    else
                    {
                        AutoCut.TempX1 = Convert.ToInt32(dv[i切割_Z1_NowLineNo]["Scan_X1"]);
                    }

                    AutoCut.TempX1 += SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();

                    AutoCut.CutZ1 =
                        SReadValue("切割時Z1切刀切割道底部位置_Z").ToInt()
                        + Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_高度偏移"]);

                }
                else
                {
                    i切割_Z1_NowLineNo = -1;
                    //AutoCut.TempX1 = 0;
                    AutoCut.TempX1 = SReadValue("安全點位_X1").ToInt();
                    AutoCut.CutZ1 = 0;
                }

                //是否使用Z2
                if (Convert.ToBoolean(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z2使用"]))
                {
                    i切割_Z2_NowLineNo = Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z2刀順"]) - 1;

                    //+ By Max 20210225, 內靶掃描
                    if (PReadValue("TargetFind").ToInt() == 0)
                    {
                        //+ By Max 20210225, 內靶最後一道，特別處理
                        if (i切割_Z2_NowLineNo == dv.Count)
                        {
                            double dPitch = (Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]) - Convert.ToInt32(dv[0]["Scan_X1"])) / (dv.Count - 1);
                            AutoCut.TempX2 = Convert.ToInt32(dv[dv.Count - 1]["Scan_X1"]) + (int)dPitch + Convert.ToInt32(dv[dv.Count - 1]["Scan_XOffset"]);
                        }
                        else
                        {
                            AutoCut.TempX2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_X1"]) + Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_XOffset"]);
                        }
                    }
                    else
                    {
                        AutoCut.TempX2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_X1"]);
                    }

                    AutoCut.TempX2 += SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt();

                    AutoCut.CutZ2 =
                        SReadValue("切割時Z2切刀切割道底部位置_Z").ToInt()
                        + Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_高度偏移"]);

                }
                else
                {
                    i切割_Z2_NowLineNo = -1;
                    //AutoCut.TempX2 = 0;
                    AutoCut.TempX2 = SReadValue("安全點位_X2").ToInt();
                    AutoCut.CutZ2 = 0;
                }

                //+ By Max 20210222, YieldCutting, 切割前檢查刀序與X軸相對距離
                int Z12X_SafeDis = SReadValue("X_Motor_SafeDis").ToInt();
                if (iXMotorLinearMode == 1)//X軸搭配絕對式光學尺功能
                {
                    if ((AutoCut.TempX2 - AutoCut.TempX1) > Z12X_SafeDis && !bIsYieldCutting)
                    {
                        bIsYieldCutting = true;
                        iYieldCuttingCountdown = 2;
                    }
                }
                else
                {
                    if ((AutoCut.TempX1 - AutoCut.TempX2) < Z12X_SafeDis && !bIsYieldCutting)
                    {
                        bIsYieldCutting = true;
                        iYieldCuttingCountdown = 2;
                    }
                }

                if (bIsYieldCutting)
                {
                    if (iYieldCuttingCountdown == 2)
                    {
                        i切割_Z2_NowLineNo = -1;
                        AutoCut.TempX2 = 0;
                        AutoCut.CutZ2 = 0;
                    }
                    else if (iYieldCuttingCountdown == 1)
                    {
                        i切割_Z1_NowLineNo = -1;
                        AutoCut.TempX1 = 0;
                        AutoCut.CutZ1 = 0;
                    }
                }

                //Z1Z2同時切，兩道角度相加除以二
                if (i切割_Z1_NowLineNo > -1 && i切割_Z2_NowLineNo > -1)
                {
                    //+ By Max 20210225, 內靶掃描
                    if (PReadValue("TargetFind").ToInt() == 0)
                    {
                        //+ By Max 20210225, 內靶最後一道，特別處理
                        if (i切割_Z1_NowLineNo == dv.Count)
                            AutoCut.TempU = (Convert.ToInt32(dv[i切割_Z1_NowLineNo - 1]["Scan_Angle"]) + Convert.ToInt32(dv[i切割_Z1_NowLineNo - 1]["Scan_Angle"])) / 2;
                        if (i切割_Z2_NowLineNo == dv.Count)
                            AutoCut.TempU = (Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Angle"]) + Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Angle"])) / 2;
                    }
                    else
                    {
                        AutoCut.TempU = (Convert.ToInt32(dv[i切割_Z1_NowLineNo]["Scan_Angle"]) + Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_Angle"])) / 2;
                    }
                }
                //Z1單刀切，使用Z1角度
                else if (i切割_Z1_NowLineNo > -1 && i切割_Z2_NowLineNo == -1)
                {
                    //+ By Max 20210225, 內靶掃描
                    //+ By Max 20210225, 內靶最後一道，特別處理
                    if (PReadValue("TargetFind").ToInt() == 0 && i切割_Z1_NowLineNo == dv.Count)
                    {
                        AutoCut.TempU = Convert.ToInt32(dv[i切割_Z1_NowLineNo - 1]["Scan_Angle"]);
                    }
                    else
                    {
                        AutoCut.TempU = Convert.ToInt32(dv[i切割_Z1_NowLineNo]["Scan_Angle"]);
                    }
                }
                //Z2單刀切，使用Z2角度
                else if (i切割_Z1_NowLineNo == -1 && i切割_Z2_NowLineNo > -1)
                {
                    //+ By Max 20210225, 內靶掃描
                    //+ By Max 20210225, 內靶最後一道，特別處理
                    if (PReadValue("TargetFind").ToInt() == 0 && i切割_Z2_NowLineNo == dv.Count)
                    {
                        AutoCut.TempU = Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Angle"]);
                    }
                    else
                    {
                        AutoCut.TempU = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_Angle"]);
                    }
                }

                i切割_Now百分比 = 0;

                if (m_bIsVisionSaw)//設置出入刀點
                {
                    //+ By Max 20210225, 內靶掃描
                    //+ By Max 20210225, 內靶最後一道，特別處理
                    if (PReadValue("TargetFind").ToInt() == 0 && i切割_Z2_NowLineNo == dv.Count)
                    {
                        AutoCut.TempX1 = 0;
                        AutoCut.TempX2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_X1"]);
                        AutoCut.YCutStartPos = Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Y1"]);
                        AutoCut.YCutEndPos = Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Y2"]);
                        AutoCut.CutZ1 = 0;
                        AutoCut.CutZ2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo - 1]["Scan_Z"]);
                    }
                    else
                    {
                        AutoCut.TempX1 = 0;
                        AutoCut.TempX2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_X1"]);
                        AutoCut.YCutStartPos = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_Y1"]);
                        AutoCut.YCutEndPos = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_Y2"]);
                        AutoCut.CutZ1 = 0;
                        AutoCut.CutZ2 = Convert.ToInt32(dv[i切割_Z2_NowLineNo]["Scan_Z"]);
                    }
                }
                else
                {
                    //Ver.1 使用固定點計算切割位置
                    //AutoCut.YCutStartPos = SReadValue("切割起始前方點位_Y").ToInt();
                    //AutoCut.YCutEndPos = SReadValue("切割起始後方點位_Y").ToInt();

                    //Ver.2 透過靶點計算切割位置
                    //AutoCut.YCutStartPos = Convert.ToInt32(dv[0]["Scan_Y1"].ToString())//v4.0.2.21 使用0取代i切割_Z2_NowLineNo，因可能只使用Z1單刀
                    //    + SReadValue("Z2CCD與切刀中心點距離_Y").ToInt()
                    //    - SReadValue("YCutShift").ToInt();


                    //AutoCut.YCutEndPos = Convert.ToInt32(dv[0]["Scan_Y2"].ToString())//v4.0.2.21 使用0取代i切割_Z2_NowLineNo，因可能只使用Z1單刀
                    //    + SReadValue("Z2CCD與切刀中心點距離_Y").ToInt()
                    //    + SReadValue("YCutShift").ToInt();    

                    //Ver.3 透過產品輸入長寬計算切割位置
                    int Cut_length = 0;
                    if (s切割_NowSide == "CH2")
                    {
                        Cut_length = PReadValue("LengthF").ToInt() / 2;
                    }
                    else//CH1
                    {
                        Cut_length = PReadValue("LengthE").ToInt() / 2;
                    }
                    AutoCut.YCutStartPos = SReadValue("圓心點位_Y").ToInt()
                        - Cut_length
                        + SReadValue("Z2CCD與切刀中心點距離_Y").ToInt()
                        - SReadValue("YCutShift").ToInt();


                    AutoCut.YCutEndPos = SReadValue("圓心點位_Y").ToInt()
                        + Cut_length
                        + SReadValue("Z2CCD與切刀中心點距離_Y").ToInt()
                        + SReadValue("YCutShift").ToInt();
                    //如果起始點低於系統切割起始點位，則可能為產品尺寸輸入錯誤，預防撞機，故使用系統切割起始點位
                    if (AutoCut.YCutStartPos < SReadValue("切割起始前方點位_Y").ToInt())
                    {
                        AutoCut.YCutStartPos = SReadValue("切割起始前方點位_Y").ToInt();
                    }
                    //如果結束點高於系統切割結束點位，則可能為產品尺寸輸入錯誤，預防撞機，故使用系統切割結束點位
                    if (AutoCut.YCutEndPos > SReadValue("切割起始後方點位_Y").ToInt())
                    {
                        AutoCut.YCutEndPos = SReadValue("切割起始後方點位_Y").ToInt();
                    }

                    if (NowSecondSawState == enSecondSawState.First)
                    {
                        AutoCut.CutZ1 += (PReadValue("PCBDepth").ToInt() / 2);
                        AutoCut.CutZ2 += (PReadValue("PCBDepth").ToInt() / 2);
                    }
                }

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_點位計算_結束_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        private FlowChart.FCRESULT FC_自動_XY到位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, AutoCut.YCutStartPos},
                {motor_切刀橫移馬達_Z1X, AutoCut.TempX1},
                {motor_切刀橫移馬達_Z2X, AutoCut.TempX2}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_U到位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoCut.TempU}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                String sCutLog = String.Format("CutSide: {0}, Z1 LineNo: {1}, CutPos: {2}, ZPos: {3}, Z2 LineNo: {4}, CutPos: {5}, ZPos: {6}",
                    s切割_NowSide, i切割_Z1_NowLineNo + 1, AutoCut.TempX1, AutoCut.CutZ1, i切割_Z2_NowLineNo + 1, AutoCut.TempX2, AutoCut.CutZ2);
                LogSay(EnLoggerType.EnLog_SPC, sCutLog);

                CheckSpindleRunDist(enSpindleSel.Z1);
                CheckSpindleRunDist(enSpindleSel.Z2);
                SetSpeed_Y(enSpeedMode.切割速度);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_Z到位_Run()
        {
            if (!MonitorSpindle() && !m_bIsVisionSaw)//切割Z軸到位
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            //+ By Max, 4.0.1.81
            if (bIsSimuProcess && iReSimuProcessRunCount > 0)
            {
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切刀上下馬達_Z1, 0},
                    {motor_切刀上下馬達_Z2, 0}
                };
                if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切刀上下馬達_Z1, AutoCut.CutZ1},
                    {motor_切刀上下馬達_Z2, AutoCut.CutZ2}
                };
                if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_水開啟_Run()
        {
            if (!MonitorSpindle() && !m_bIsVisionSaw) //切割前設置
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            if (!m_bIsVisionSaw) //切割前設置
            {
                m_bIsPauseNeedToContinue = true;
                //+ By Max 20210312, v4.0.1.53
                SetContinueRun(true);
            }

            //無使用切兩刀
            if (NowSecondSawState != enSecondSawState.First)
            {
                //+ By Max 20210222, YieldCutting, 切割前檢查刀序與X軸相對距離
                if (bIsYieldCutting)
                    iYieldCuttingCountdown -= 1;

                if (iYieldCuttingCountdown <= 0)
                {
                    bIsYieldCutting = false;
                }

                if (!bIsYieldCutting)
                {
                    AutoCut.Step += 1;
                }
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_Y移動_Run()
        {
            try
            {
                int AddDistmp = Math.Abs(AutoCut.YCutEndPos - AutoCut.YCutStartPos);
                i切割_Now百分比 = (motor_切割平台前後馬達Y.ReadPos() - AutoCut.YCutStartPos) * 100 / AddDistmp;
                bool bIsMotorYDone = motor_切割平台前後馬達Y.G00(AutoCut.YCutEndPos);

                //+ By Max, 20210804 ------------------------------------------
                if (Z1Spindle.RefRPM < Z1SpindleStatus.MinRefSpeed)
                    Z1SpindleStatus.MinRefSpeed = Z1Spindle.RefRPM;
                if (Z1Spindle.ActualRPM < Z1SpindleStatus.MinActSpeed)
                    Z1SpindleStatus.MinActSpeed = Z1Spindle.ActualRPM;
                if (Z1Spindle.LoadAMP > Z1SpindleStatus.MaxAMPLoad)
                    Z1SpindleStatus.MaxAMPLoad = Z1Spindle.LoadAMP;
                if (Z1Spindle.LoadMotor > Z1SpindleStatus.MaxMotorLoad)
                    Z1SpindleStatus.MaxMotorLoad = Z1Spindle.LoadMotor;

                if (Z2Spindle.RefRPM < Z2SpindleStatus.MinRefSpeed)
                    Z2SpindleStatus.MinRefSpeed = Z2Spindle.RefRPM;
                if (Z2Spindle.ActualRPM < Z2SpindleStatus.MinActSpeed)
                    Z2SpindleStatus.MinActSpeed = Z2Spindle.ActualRPM;
                if (Z2Spindle.LoadAMP > Z2SpindleStatus.MaxAMPLoad)
                    Z2SpindleStatus.MaxAMPLoad = Z2Spindle.LoadAMP;
                if (Z2Spindle.LoadMotor > Z2SpindleStatus.MaxMotorLoad)
                    Z2SpindleStatus.MaxMotorLoad = Z2Spindle.LoadMotor;
                // -------------------------------------------------------------

                VaccError = CheckVaca(0);

                //+ By Max 20210223, 切割過程水持續偵測
                WaterFlowAbnormal = "";
                if ((bUseZ1_Spindle && !CheckWater(enSpindleSel.Z1)) || (bUseZ2_Spindle && !CheckWater(enSpindleSel.Z2)))
                {
                    WaterFlowAbnormal = "Cutting Water Flow Abnormal!!";
                }

                //+ By Max 20210312, v4.0.1.53, 水不足判斷移至G00完畢
                if (VaccError != "")
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                //v4.0.1.23 畫刀序
                //+ By Max 20210206
                if (DelegateDrawLineData != null)
                {
                    DelegateDrawLineData(s切割_NowSide, i切割_Z1_NowLineNo, i切割_Z2_NowLineNo, i切割_Now百分比, false);
                }

                if (bIsMotorYDone)
                {
                    if (i切割_Z1_NowLineNo > -1)
                    {
                        double dPosTmp1 = PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble();
                        dPosTmp1 += AddDistmp / 1000000d; //change to Meter
                        Tool1_InfoReturn.dRealMotorDistance = dPosTmp1;//v4.0.1.17
                        //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                        PSetValue("BladeZ1Data", "dRealMotorDistanceZ1", dPosTmp1, "Double");
                        LogSay(EnLoggerType.EnLog_SPC, string.Format("[Z1]累積里程 = {0}", dPosTmp1));

                        //+ By Max 20210406, v4.0.1.61, 紀錄主軸切割時的轉速與負載
                        //Z1ActualSpeed, Z1RefSpeed, Z1LoadAMP, Z1LoadMotor;
                        //+ By Max, 20210804, 修改紀錄主軸狀態方式，改紀錄切割過程中轉速最低以及負載最高的值
                        sb.Clear();
                        sb.Append(String.Format("[Z1] Package: {0}, RefRPM: {1}, ActRPM: {2}, AmpLoad: {3}, MotorLoad: {4}",
                            PackageName, Z1SpindleStatus.MinRefSpeed, Z1SpindleStatus.MinActSpeed, Z1SpindleStatus.MaxAMPLoad, Z1SpindleStatus.MaxMotorLoad));
                        Z1SpindleStatus.Reset();
                        LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                    }

                    if (i切割_Z2_NowLineNo > -1)
                    {
                        double dPosTmp2 = PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble();
                        dPosTmp2 += AddDistmp / 1000000d;//change to Meter
                        Tool2_InfoReturn.dRealMotorDistance = dPosTmp2;//v4.0.1.17
                        //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                        PSetValue("BladeZ2Data", "dRealMotorDistanceZ2", dPosTmp2, "Double");
                        LogSay(EnLoggerType.EnLog_SPC, string.Format("[Z2]累積里程 = {0}", dPosTmp2));

                        //+ By Max 20210406, v4.0.1.61, 紀錄主軸切割時的轉速與負載
                        //Z1ActualSpeed, Z1RefSpeed, Z1LoadAMP, Z1LoadMotor;
                        //+ By Max, 20210804, 修改紀錄主軸狀態方式，改紀錄切割過程中轉速最低以及負載最高的值
                        sb.Clear();
                        sb.Append(String.Format("[Z2] Package: {0}, RefRPM: {1}, ActRPM: {2}, AmpLoad: {3}, MotorLoad: {4}",
                            PackageName, Z2SpindleStatus.MinRefSpeed, Z2SpindleStatus.MinActSpeed, Z2SpindleStatus.MaxAMPLoad, Z2SpindleStatus.MaxMotorLoad));
                        Z2SpindleStatus.Reset();
                        LogSay(EnLoggerType.EnLog_SPC, sb.ToString());
                    }

                    SetSpeed_Y(enSpeedMode.切割後速度);
                    Timer_Auto.Restart();

                    //+ By Max 20210312, v4.0.1.53, 解決切割途中水量不足提刀漏切的問題
                    if (WaterFlowAbnormal != "")
                        return FlowChart.FCRESULT.CASE1;
                    else
                        return FlowChart.FCRESULT.NEXT;

                }
                else if (Timer_Auto.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    if (!bIsMotorYDone) MotorOverTime(motor_切割平台前後馬達Y.Text);
                    Timer_Auto.Restart();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_中繼1_Run()
        {
            //v4.0.0.37 
            m_bIsZ1Broken = m_bIsZ1Broken || !bUseZ1_Spindle;
            m_bIsZ2Broken = m_bIsZ2Broken || !bUseZ2_Spindle;

            if (m_bIsZ1Broken && m_bIsZ2Broken)
            {
                m_bIsZ1Broken = false;
                m_bIsZ2Broken = false;
                //FC_換刀流程_動作開始.TaskReset();
                bshow_ChangeBladeContinue = true;
                ShowAlarm("w", 50);//請選擇[視覺功能表]->常用->換刀續切或強排
            }
            else if (m_bIsZ1Broken)
            {
                m_bIsZ1Broken = false;
                bUseZ1_Spindle = false;
                //使用Z2切割
                DataTable NewTT2 = new DataTable();
                NewTT2 = Change_CutData(enSpindleSel.Z2);

                if (AutoCut.CutData != null)
                {
                    AutoCut.CutData.Dispose();
                    AutoCut.CutData = null;
                }

                AutoCut.CutData = NewTT2;
                bshow_SingleBladeContinue = true;
                ShowAlarm("w", 51);//請選擇[視覺功能表]->常用->單刀續切或強排
            }
            else if (m_bIsZ2Broken)
            {
                m_bIsZ2Broken = false;
                bUseZ2_Spindle = false;
                //使用Z1切割
                DataTable NewTT1 = new DataTable();
                NewTT1 = Change_CutData(enSpindleSel.Z1);

                if (AutoCut.CutData != null)
                {
                    AutoCut.CutData.Dispose();
                    AutoCut.CutData = null;
                }

                AutoCut.CutData = NewTT1;
                bshow_SingleBladeContinue = true;
                ShowAlarm("w", 51);//請選擇[視覺功能表]->常用->單刀續切或強排
            }

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_中繼2_Run()
        {
            //4.0.0.37 破刀後處理
            if (bshow_ChangeBladeContinue)
            {
                if (bset_ChangeBladeContinue)
                {
                    //FC_換刀流程_動作開始.MainRun();
                    if (Teach_ActionResult == 1)
                    {
                        bshow_ChangeBladeContinue = false;
                        bset_ChangeBladeContinue = false;
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                }
                else
                    ShowAlarm("w", 50);//請選擇[視覺功能表]->常用->換刀續切或強排
            }
            else if (bshow_SingleBladeContinue)
            {
                if (bset_SingleBladeContinue)
                {
                    bshow_SingleBladeContinue = false;
                    bset_SingleBladeContinue = false;
                    Timer_Auto.Restart();
                    //+ By Max 20210319, v4.0.1.57, CASE1 -> NEXT，執行啟動主軸流程，以避免使用者手動停主軸後未將其再啟動
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                    ShowAlarm("w", 51);//請選擇[視覺功能表]->常用->單刀續切或強排
            }

            else
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_抬刀_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, AutoCut.SafeZ},
                {motor_切刀上下馬達_Z2, AutoCut.SafeZ}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_確認_Run()
        {
            //預留排除後續切 v4.0.1.34
            //+ By Max 20210225, 切割水流量異常
            if (WaterFlowAbnormal != "")
                ShowAlarm("E", 70, WaterFlowAbnormal);
            else if (VaccError != "")
                ShowAlarm("E", 70, VaccError);

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #region Z移到安全位

        private FlowChart.FCRESULT FC_自動_Z移到安全位_Run()
        {
            NLogger.Debug("切割結束");
            NLogger.Debug("");

            bWaterTurnOn = false;

            //切割刀序完成
            if (AutoCut.Step == AutoCut.CutData.Rows.Count)
            {
                s切割_NowSide = "";
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE2;
            }
            else
            {
                if (NowSecondSawState == enSecondSawState.First)
                    NowSecondSawState = enSecondSawState.Second;
                else if (NowSecondSawState == enSecondSawState.Second)
                    NowSecondSawState = enSecondSawState.First;

                if (!MonitorSpindle() && !m_bIsVisionSaw)//切割後檢查
                    return FlowChart.FCRESULT.CASE1;

                return FlowChart.FCRESULT.NEXT;
            }
        }

        private FlowChart.FCRESULT FC_自動_Z移到安全位_Z移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                //+ By Max 20210325, v4.0.1.58, 由外層FlowChart移至此處，Z軸移動完成後即可停掉PauseNeedToContinue
                m_bIsPauseNeedToContinue = false;
                //+ By Max 20210312, v4.0.1.53
                SetContinueRun(false);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_Z移到安全位_是否做動態刀痕_Run()
        {
            if (bdynamicCutOffset)
            {
                bdynamicCutOffset_run = true;
                bdynamicCutOffsetFinish = false;
                FC_動態刀痕_開始.TaskReset();
            }
            else
                bdynamicCutOffsetFinish = true;
            return FlowChart.FCRESULT.NEXT;

        }

        private FlowChart.FCRESULT FC_自動_Z移到安全位_動態刀痕完成_Run()
        {
            if (bdynamicCutOffsetFinish)
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
                FC_動態刀痕_開始.MainRun();
            return default(FlowChart.FCRESULT);
        }

        //+ By Max 20210325, v4.0.1.58, 動態刀痕完成後。Z再次移到安全點位
        private FlowChart.FCRESULT FC_切割_Z移到安全位_Z再次移到安全位_Run()
        {
            //var dicMotorPosPairs = new Dictionary<Motor, int>()
            //{
            //    {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
            //    {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            //};

            //if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            //return FlowChart.FCRESULT.IDLE;

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_Z移到安全位_動態刀痕結束開水_Run()
        {
            if (bWaterTurnOn)
            {
                analogOut_Z1噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z1);
                analogOut_Z1灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z1);
                analogOut_Z2噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z2);
                analogOut_Z2灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z2);

                SetOutBit(outBit_Z1水開關電磁閥, true);
                SetOutBit(outBit_Z2水開關電磁閥, true);
                SetOutBit(outBit_切割區增濕水簾, true);

                if (CheckWater(enSpindleSel.Z1) && CheckWater(enSpindleSel.Z2))
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }

                if (Timer_Auto.On(SReadValue("水開啟準備時間").ToInt()))
                {
                    if (!CheckWater(enSpindleSel.Z1))
                    {
                        ShowAlarm("w", 14, "Z1"); //Z1噴水/灑水流量未到達
                    }
                    if (!CheckWater(enSpindleSel.Z2))
                    {
                        ShowAlarm("w", 14, "Z2"); //Z2噴水/灑水流量未到達
                    }
                    Timer_Auto.Restart();
                }
            }
            else if (Timer_Auto.On(PReadValue("Cutting_Delay").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #endregion

        private FlowChart.FCRESULT FC_自動_關水_Run()
        {
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_切割區增濕水簾, false);

            //+ By Max 20210206
            if (DelegateDrawLineData != null)
                DelegateDrawLineData(s切割_NowSide, i切割_Z1_NowLineNo, i切割_Z2_NowLineNo, i切割_Now百分比, true);

            m_SwTimeLog.Stop();
            m_TimeLog.dCutTime = Convert.ToDouble(m_SwTimeLog.ElapsedMilliseconds / 1000.0f);

            Timer_Auto.Restart();

            //+ By Max 20210309, 是否顯示動態刀痕CheckBox
            bCanShowDynamicCutOffset = false;

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_切割結束_Run()
        {
            if (!bIsSimuProcess || (bIsSimuProcess && iReSimuProcessRunCount == 0))
            {
                PCBOutcount++;
            }

            enCheckSel touchSel = (enCheckSel)(OReadValue("iSparkTest_freq").ToInt());
            enCheckSel nonTouchSel = (enCheckSel)(OReadValue("iNonContactSparkTest_freq").ToInt());

            if (bUseZ1_Spindle)
            {
                #region 接觸測高

                if (touchSel == enCheckSel.里程數檢查)
                {
                    if (iSparkTestCountZ1 == 0)
                    {
                        iSparkTestCountZ1 = (int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble());
                    }
                    else
                    {
                        if (((int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble()) - iSparkTestCountZ1) > OReadValue("freq_STRunMeter").ToInt())
                        {
                            iSparkTestCountZ1 = (int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble());
                            m_bIsAutoNeedSparkTestZ1 = true;
                        }
                    }
                }
                else if (touchSel == enCheckSel.片數檢查)
                {
                    //+ By Max 20210316, v4.0.1.55, 改為取餘數等於0時執行
                    m_bIsAutoNeedSparkTestZ1 = PCBOutcount % OReadValue("freq_STRunMP").ToInt() == 0;
                }

                #endregion

                #region 非接觸測高

                if (nonTouchSel == enCheckSel.里程數檢查)
                {
                    if (iNContactTestCountZ1 == 0)
                    {
                        iNContactTestCountZ1 = (int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble());
                    }
                    else
                    {
                        if (((int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble()) - iNContactTestCountZ1) > OReadValue("freq_NCTRunMeter").ToInt())
                        {
                            iNContactTestCountZ1 = (int)(PReadValue("BladeZ1Data", "dRealMotorDistanceZ1").ToDouble());
                            m_bIsAutoNeedNContactTestZ1 = true;
                        }
                    }
                }
                else if (nonTouchSel == enCheckSel.片數檢查)
                {
                    //+ By Max 20210316, v4.0.1.55, 改為取餘數等於0時執行
                    m_bIsAutoNeedNContactTestZ1 = PCBOutcount % OReadValue("freq_NCTRunMP").ToInt() == 0;
                }

                #endregion
            }

            if (bUseZ2_Spindle)
            {
                #region 接觸測高

                if (touchSel == enCheckSel.里程數檢查)
                {
                    if (iSparkTestCountZ2 == 0)
                    {
                        iSparkTestCountZ2 = (int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble());
                    }
                    else
                    {
                        if (((int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble()) - iSparkTestCountZ2) > OReadValue("freq_STRunMeter").ToInt())
                        {
                            iSparkTestCountZ2 = (int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble());
                            m_bIsAutoNeedSparkTestZ2 = true;
                        }
                    }
                }
                else if (touchSel == enCheckSel.片數檢查)
                {
                    //+ By Max 20210316, v4.0.1.55, 改為取餘數等於0時執行
                    m_bIsAutoNeedSparkTestZ2 = PCBOutcount % OReadValue("freq_STRunMP").ToDouble() == 0;
                }

                #endregion

                #region 非接觸測高

                if (nonTouchSel == enCheckSel.里程數檢查)
                {
                    if (iNContactTestCountZ2 == 0)
                    {
                        iNContactTestCountZ2 = (int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble());
                    }
                    else
                    {
                        if (((int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble()) - iNContactTestCountZ2) > OReadValue("freq_NCTRunMeter").ToInt())
                        {
                            iNContactTestCountZ2 = (int)(PReadValue("BladeZ2Data", "dRealMotorDistanceZ2").ToDouble());
                            m_bIsAutoNeedNContactTestZ2 = true;
                        }
                    }
                }
                else if (nonTouchSel == enCheckSel.片數檢查)
                {
                    //+ By Max 20210316, v4.0.1.55, 改為取餘數等於0時執行
                    m_bIsAutoNeedNContactTestZ2 = PCBOutcount % OReadValue("freq_NCTRunMP").ToInt() == 0;
                }

                #endregion
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-切割完成");

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_切割後清洗_Run()
        {
            if (OReadValue("UnLoadMode").ToInt() == 0)
            {//自動出料才發出訊號讓手臂偷跑
                if (!bIsSimuProcess || (bIsSimuProcess && iReSimuProcessRunCount == 0))
                {
                    bCanFrontGetRun = true;
                }
            }

            bool bIsClean = PCBIncount % PReadValue("CleanAfterCut_freq").ToInt() == 0;
            if (PReadValue("bCleanAfterCut").ToBoolean())
            {
                m_enCleanType = enCleanType.切割後清洗;
                m_SwTimeLog.Restart();
                FC_清洗_動作開始.TaskReset();
            }
            else
            {
                m_CleanFinishEvt.Set();
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-切割後清洗開始");
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_切割後清洗完成_Run()
        {
            if (m_CleanFinishEvt.WaitOne(0))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-切割後清洗完成");
                m_SwTimeLog.Stop();
                m_TimeLog.dCleanTime = Convert.ToDouble(m_SwTimeLog.ElapsedMilliseconds / 1000.0f);

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                FC_清洗_動作開始.MainRun();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_刀痕檢測_Run()
        {
            enCheckKerfSel enSel = (enCheckKerfSel)(OReadValue("ikerfcheckType").ToInt());

            if (enSel != enCheckKerfSel.NONE && PCBIncount % OReadValue("ikerfCheckfreq").ToInt() == 0)
            {
                FC_刀痕檢測_開始.TaskReset();
            }
            else
            {
                m_KerfCheckFinishEvt.Set();
            }

            LogSay(EnLoggerType.EnLog_SPC, "Auto-刀痕檢測開始");
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_刀痕檢測完成_Run()
        {
            if (m_KerfCheckFinishEvt.WaitOne(0))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-刀痕檢測完成");
                Timer_Auto.Restart();

                //+ By Max, 4.0.1.99
                if (bIsSimuProcess)
                {
                    return FlowChart.FCRESULT.CASE1;
                }
                else
                {
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else
                FC_刀痕檢測_開始.MainRun();

            return default(FlowChart.FCRESULT);
        }

        #region 出料

        private FlowChart.FCRESULT FC_自動_出料_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_出料開始_Run()
        {
            Timer_Auto.Restart();
            m_SwTimeLog.Restart();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_移至出料點位_Run()
        {
            iMoveHadoutY = SReadValue("基準出料點位_Y").ToInt();
            //v4.0.1.28 任一角度出料
            iMoveHadoutU = SReadValue("基準出料點位_θ").ToInt() +
                (iDD馬達一轉Pulse數 * PReadValue("iRotateOutMode").ToInt() / 4) * (PReadValue("bAntiClockwise").ToBoolean() ? -1 : 1);

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iMoveHadoutY},
                {motor_切割平台旋轉馬達U, iMoveHadoutU},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto))
            {
                m_SwTimeLog.Stop();
                m_TimeLog.dUnloadTime = Convert.ToDouble(m_SwTimeLog.ElapsedMilliseconds / 1000.0f);

                iChkSawTableType = 1;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_出料_出料模式_Run()
        {
            enUnLoadMode mode = (enUnLoadMode)(OReadValue("UnLoadMode").ToInt());

            LogSay(EnLoggerType.EnLog_SPC, string.Format("Auto-{0}出料開始", mode.ToString()));
            Timer_Auto.Restart();

            if (mode == enUnLoadMode.自動) return FlowChart.FCRESULT.NEXT;
            else return FlowChart.FCRESULT.CASE1;
        }

        #region 手動出料

        private FlowChart.FCRESULT FC_自動_出料_手動出料_Run()
        {
            m_bIsNeedCheckVaca = false;//4.0.1.17
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_通知人員手動出料_Run()
        {
            SetOutBit(outBit_切割平台真空建立, false);//關真空
            //+ By Max 20210325, v4.0.1.58 DryRun
            if (!OReadValue("bDryRun").ToBoolean())
                ShowAlarm("i", 18);
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_真空檢查_Run()
        {
            SetOutBit(outBit_切割平台真空建立, true);//真空開啟
            if (CheckVaca(0) == "" && IsSimulation() == 0)
            {
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            else if (Timer_Auto.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        #endregion 手動出料

        #region 自動出料

        private FlowChart.FCRESULT FC_自動_出料_自動出料_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_發出訊息要求出料_Run()
        {
            LogSay(EnLoggerType.EnLog_SPC, "Auto-發出訊息要求出料");

            //bCanFrontGetRun = false;//v4.0.1.21
            bCanGetRunSate = true;
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_出料_手臂通知出料完成_Run()
        {
            if ((bool)GetResult("GetStripFinish"))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-手臂通知出料完成");

                bCanFrontGetRun = false;
                //bCanGetRunSate = false;//v4.0.1.21
                m_bIsNeedCheckVaca = false;
                SetOutBit(outBit_切割平台真空破壞, true);

                ResetResult("GetStripFinish");

                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_出料_通知手臂可移開_Run()
        {
            if (GetChuckTableAIValue() > -20)
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-通知手臂可移開");

                SendData("GetCanLeave");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Auto.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    ShowAlarm("w", 56, GetChuckTableAIValue().ToString());
                    Timer_Auto.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_出料_手臂移至安全位置_Run()
        {
            //+ By Max 20210329, v4.0.1.59，增加檢查I/O的狀態
            if (!IsHADOUTInProcess() && (bool)GetResult("GetLeaveFinish"))
            {
                LogSay(EnLoggerType.EnLog_SPC, "Auto-手臂移至安全位置");

                bCanGetRunSate = false;//v4.0.1.21
                ResetResult("GetLeaveFinish");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if ((bool)GetResult("ONvacOFFdes"))
            {
                SetOutBit(outBit_切割平台真空建立, true);
                ResetResult("ONvacOFFdes");
                Timer_Auto.Restart();
            }
            else if ((bool)GetResult("OFFvacONdes"))
            {
                SetOutBit(outBit_切割平台真空破壞, true);
                ResetResult("OFFvacONdes");
                Timer_Auto.Restart();
            }
            else if ((bool)GetResult("PickAbort"))
            {
                bCanGetRunSate = false;//v4.0.1.21
                ResetResult("PickAbort");
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_出料_放棄_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion 自動出料

        private FlowChart.FCRESULT FC_自動_出料_出料完成_Run()
        {
            outBit_ManualLoadOK.Off();
            LogSay(EnLoggerType.EnLog_SPC, "Auto-出料完成");
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion 出料

        private FlowChart.FCRESULT FC_自動_資料更新_Run()
        {
            if (bIsSimuProcess)
            {
                if (iReSimuProcessRunCount > 0)
                {
                    iReSimuProcessRunCount--;
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (--iReSimuProcessRunMP == 1)
                    {
                        OSetValue("bSimuProcess", false);
                        iReSimuProcessRunMP = 0;
                    }
                    else
                    {
                        iReSimuProcessRunCount = OReadValue("SimuProcessCycle").ToInt();
                    }

                    return FlowChart.FCRESULT.CASE1;
                }
            }
            else
            {
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_自動_重跑_Run()
        {
            Auto_ActionResult = 1;

            ShowAllTimeAction(m_TimeLog.dLoadTime, m_TimeLog.dScanTime, m_TimeLog.dCutTime, m_TimeLog.dCleanTime, m_TimeLog.dUnloadTime);
            LogSay(EnLoggerType.EnLog_SPC, String.Format("入料: {0:0.00} Sec, 掃描: {1:0.00} Sec, 切割: {2:0.00} Sec, 清洗: {3:0.00} Sec, 出料: {4:0.00} Sec",
                m_TimeLog.dLoadTime, m_TimeLog.dScanTime, m_TimeLog.dCutTime, m_TimeLog.dCleanTime, m_TimeLog.dUnloadTime));

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_出料結束_Run()
        {
            Auto_ActionResult = 1;

            ShowAllTimeAction(m_TimeLog.dLoadTime, m_TimeLog.dScanTime, m_TimeLog.dCutTime, m_TimeLog.dCleanTime, m_TimeLog.dUnloadTime);
            LogSay(EnLoggerType.EnLog_SPC, String.Format("入料: {0:0.00} Sec, 掃描: {1:0.00} Sec, 切割: {2:0.00} Sec, 清洗: {3:0.00} Sec, 出料: {4:0.00} Sec",
                m_TimeLog.dLoadTime, m_TimeLog.dScanTime, m_TimeLog.dCutTime, m_TimeLog.dCleanTime, m_TimeLog.dUnloadTime));

            return default(FlowChart.FCRESULT);
        }

        #endregion

        #region 即看即切

        private FlowChart.FCRESULT FC_自動_即看即切_開始_Run()
        {
            NLogger.Info("FC_自動_即看即切_開始_Run()");
            m_enScanMethod = enScanTargetMethod.即看即切;

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_ResetIO開啟_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            AutoMark.strSide = AutoCut.CutData.Rows[AutoCut.Step]["Cut_料片方向"].ToString();
            AutoMark.nFindMarkStep = 0;

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_移至靶點_Run()
        {
            DataView dv = new DataView(AutoMark.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
            int nCurrentLineIdx = Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z2刀順"]);

            if (AutoMark.nFindMarkStep == 0)
            {
                AutoMark.nTempX = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_X2"]);
                AutoMark.nTempY = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_Y2"]);
            }
            else
            {
                AutoMark.nTempX = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_X1"]);
                AutoMark.nTempY = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_Y1"]);
            }

            AutoMark.nTempθ = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_Angle"]);
            AutoMark.nHighBaseFocus = PReadValue("iZ2_FoucusPos").ToInt();

            //PrintOut(dv.ToTable());

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, AutoMark.nTempθ},
                {motor_切割平台前後馬達Y, AutoMark.nTempY},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  AutoMark.nTempX},
                {motor_切刀上下馬達_Z1,  0},
                {motor_切刀上下馬達_Z2,  AutoMark.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_視覺通訊_選擇圖案_Run()
        {
            return SelectPattern(Timer_Auto);
        }

        private FlowChart.FCRESULT FC_自動_即看即切_NG_Run()
        {
            ShowAlarm("i", 14);//視覺設定圖案形狀失敗，請按繼續重試
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_等待結果_Run()
        {
            switch (mCCDAction)//設定樣式
            {
                case CCDSendResult.OK:
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        ShowAlarm("w", 37);
                        Timer_Auto.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }

            if (Timer_Auto.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 38);
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_即看即切_開始定位_Run()
        {
            FC_自動_定位.TaskReset();
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_定位完成_Run()
        {
            if (m_PositioningFinishEvt.WaitOne(0))
            {
                AutoMark.nFindMarkStep++;

                if (AutoMark.nFindMarkStep == 2) //即看即切掃靶完成
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    Timer_Auto.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            FC_自動_定位.MainRun();
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_自動_即看即切_計算_Run()
        {
            DataView dv = new DataView(LearnMarkPos.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
            int nCurrentLineIdx = Convert.ToInt32(AutoCut.CutData.Rows[AutoCut.Step]["Cut_Z2刀順"]);

            int GapLT = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_X1"].ToString()) - AutoMark.ptLTMarkPos.X;//左上角點偏差值
            int GapLB = Convert.ToInt32(dv[nCurrentLineIdx - 1]["Scan_X2"].ToString()) - AutoMark.ptLBMarkPos.X;//左下角點偏差值

            //靶點位置是否超出學靶位置，會切到切割道
            if (Math.Abs(GapLT) > PReadValue("Cut_X_tolerance").ToInt() || Math.Abs(GapLB) > PReadValue("Cut_X_tolerance").ToInt())
            {
                if (AutoMark.strSide == "CH2")
                {
                    ShowAlarm("i", 20, "HADIN", (GapLT + GapLB) / 2);
                }
                else if (AutoMark.strSide == "CH1")
                {
                    ShowAlarm("i", 20, "Y", (GapLT + GapLB) / 2);
                }
                return FlowChart.FCRESULT.IDLE;
            }

            NLogger.Debug("角度計算前未更新 AutoMatk+");
            PrintOut(AutoMark.dtScanData, true);
            NLogger.Debug("角度計算前未更新 AutoMatk-");

            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            double dAngle = 0;

            #region 角度運算
            RotateCorrect(Org_X, Org_Y, ref AutoMark.ptLTMarkPos, ref AutoMark.ptLBMarkPos, ref dAngle);
            int nAnglePulse = Convert.ToInt32((dAngle / 360) * iDD馬達一轉Pulse數);
            AutoMark.nTempθ = AutoMark.nTempθ + nAnglePulse;
            #endregion

            #region 更新切割道X1,Y1,X2,Y2 and θ
            dv = new DataView(AutoMark.dtScanData);
            dv.RowFilter = "Scan_Side = '" + AutoMark.strSide + "'";
            dv[nCurrentLineIdx - 1]["Scan_X1"] = AutoMark.ptLTMarkPos.X.ToString();
            dv[nCurrentLineIdx - 1]["Scan_Y1"] = AutoMark.ptLTMarkPos.Y.ToString();
            dv[nCurrentLineIdx - 1]["Scan_X2"] = AutoMark.ptLBMarkPos.X.ToString();
            dv[nCurrentLineIdx - 1]["Scan_Y2"] = AutoMark.ptLBMarkPos.Y.ToString();
            dv[nCurrentLineIdx - 1]["Scan_Angle"] = AutoMark.nTempθ;
            #endregion

            NLogger.Debug("角度計算並更新後 AutoMatk+");
            PrintOut(AutoMark.dtScanData, true);
            NLogger.Debug("角度計算並更新後 AutoMatk-");

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_自動_即看即切_結束_Run()
        {
            NLogger.Info("FC_自動_即看即切_結束_Run()");

            m_SeeAndCutFinishEvt.Set();

            Timer_Auto.Restart();
            return default(FlowChart.FCRESULT);
        }

        #endregion

        #region 清洗流程

        private FlowChart.FCRESULT FC_清洗_動作開始_Run()
        {
            Load_CleanData();

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_參數設定_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);

            switch (m_enCleanType)
            {
                case enCleanType.入料前清洗:
                    tableName = "LoadClean";
                    目前清洗DataTable = 入料前清洗DataTable;
                    i總清洗次數 = PReadValue("CleanrBeforeLoad_count").ToInt();
                    i清洗DelayTime = PReadValue("CleanrBeforeLoad_Delay").ToInt();
                    outBit_切割平台真空破壞.Off();
                    outBit_切割平台真空建立.Off();
                    break;

                case enCleanType.掃靶前清洗:
                    目前清洗DataTable = 掃靶前清洗DataTable;
                    i總清洗次數 = PReadValue("掃靶前清洗次數").ToInt();
                    i清洗DelayTime = PReadValue("清洗Delay時間").ToInt();
                    SetOutBit(outBit_切割平台真空建立, true);
                    break;

                case enCleanType.切割後清洗:
                    tableName = "CutClean";
                    目前清洗DataTable = 切割後清洗DataTable;
                    i總清洗次數 = PReadValue("CleanAfterCut_count").ToInt();
                    i清洗DelayTime = PReadValue("CleanAfterCut_Delay").ToInt();
                    SetOutBit(outBit_切割平台真空建立, true);
                    break;
            }

            iNow清洗次數 = 0;

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_Z移至安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_X移至安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_是否完成清洗_Run()
        {
            if (iNow清洗次數 == i總清洗次數)
            {
                return FlowChart.FCRESULT.CASE1;
            }
            else
            {
                iNow清洗步驟 = 0;
                return FlowChart.FCRESULT.NEXT;
            }
        }

        #region 執行清洗步驟

        private FlowChart.FCRESULT FC_清洗_執行清洗步驟_Run()
        {
            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_清洗步驟是否完成_Run()
        {
            if (iNow清洗步驟 == 目前清洗DataTable.Rows.Count)
                return FlowChart.FCRESULT.CASE1;
            else
                return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_取得清洗步驟表格_Run()
        {
            bool b位置找到 = false;
            int iθ位移 = 0;

            if (目前清洗DataTable.Rows.Count < 1)
            {
                目前清洗DataTable = GetCleanTable(m_enCleanType);
            }

            string sChannel = 目前清洗DataTable.Rows[iNow清洗步驟][string.Format("{0}_Channel", tableName)].ToString();

            iCleanStdPos = SReadValue("清洗對準平台中心點位_Y").ToInt() - SReadValue("切割平台尺寸_Y").ToInt() / 2;
            iCleanEndPos = SReadValue("清洗對準平台中心點位_Y").ToInt() + SReadValue("切割平台尺寸_Y").ToInt() / 2;

            b位置找到 = ChannelRotation(sChannel, out iθ位移);
            if (b位置找到)
            {
                i清洗θ位置 = iMoveInitialU - iDD馬達一轉Pulse數 * iθ位移 / 4;
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                ShowAlarm("E", 90, "清洗步驟 Channel 未定義");
                return default(FlowChart.FCRESULT);
            }
        }

        private FlowChart.FCRESULT FC_清洗_設定清洗參數_Run()
        {
            double dCleanSpeedRate = 0;
            bool bIsOk = double.TryParse(目前清洗DataTable.Rows[iNow清洗步驟][string.Format("{0}_速度", tableName)].ToString(), out dCleanSpeedRate);

            if (bIsOk)   //v0.0.7.8 By Sanxiu 修正Package存儲方式
            {
                dCleanSpeedRate = dCleanSpeedRate * OReadValue("機台速率").ToDouble() / 10000;
                motor_切割平台前後馬達Y.SetSpeed(Convert.ToInt32(SReadValue("清洗速度_Y").ToDouble() * dCleanSpeedRate));
                motor_切割平台前後馬達Y.SetAcceleration(Convert.ToInt32(SReadValue("清洗加速度_Y").ToDouble() * dCleanSpeedRate));
                motor_切割平台前後馬達Y.SetDeceleration(Convert.ToInt32(SReadValue("清洗減速度_Y").ToDouble() * dCleanSpeedRate));

                motor_切割平台旋轉馬達U.SetSpeed(Convert.ToInt32(SReadValue("清洗速度_θ").ToDouble() * dCleanSpeedRate));
                motor_切割平台旋轉馬達U.SetAcceleration(Convert.ToInt32(SReadValue("清洗加速度_θ").ToDouble() * dCleanSpeedRate));
                motor_切割平台旋轉馬達U.SetDeceleration(Convert.ToInt32(SReadValue("清洗減速度_θ").ToDouble() * dCleanSpeedRate));
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                ShowAlarm("E", 90, "清洗速度值不為數值型態");
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_清洗_移至起始點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iCleanStdPos}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_旋轉切割平台_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, i清洗θ位置}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_開啟水氣簾_Run()
        {
            outBit_切割區增濕水簾.Value = Convert.ToBoolean(目前清洗DataTable.Rows[iNow清洗步驟][string.Format("{0}_水簾", tableName)]);
            outBit_切割區水霧隔絕氣簾.Value = Convert.ToBoolean(目前清洗DataTable.Rows[iNow清洗步驟][string.Format("{0}_氣簾", tableName)]);
            outBit_清潔風刀.Value = Convert.ToBoolean(目前清洗DataTable.Rows[iNow清洗步驟][string.Format("{0}_風刀", tableName)]);

            Timer_Auto.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_移至結束點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iCleanEndPos}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_流程延時_Run()
        {
            if (Timer_Auto.On(i清洗DelayTime))
            {
                Timer_Auto.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_清洗_移回起始點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, iCleanStdPos}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Auto)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_清洗_流程延時2_Run()
        {
            if (Timer_Auto.On(i清洗DelayTime))
            {
                iNow清洗步驟++;
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        #endregion

        private FlowChart.FCRESULT FC_清洗_清洗次數計數_Run()
        {
            iNow清洗次數++;
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_清洗_清洗完成_Run()
        {
            outBit_切割區增濕水簾.Off();
            outBit_切割區水霧隔絕氣簾.Off();
            outBit_清潔風刀.Off();
            m_CleanFinishEvt.Set();

            return default(FlowChart.FCRESULT);
        }

        #endregion 清洗流程

        #endregion 自動流程

        #region 教導流程

        #region 接觸測高流程

        private bool m_bIsSecondBottomTest = false;

        private FlowChart.FCRESULT FC_接觸測高_動作開始_Run()
        {
            if (m_SparkTestSpindle == enSpindleSel.NONE)
            {
                ShowAlarm("I", 12, "請選擇測高主軸");
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.IDLE;
            }

            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            TeachStatus = 0;
            TeachFlowStep = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 10;
            ShowAlarm("I", 10);//測高流程開始

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_檢查壽命_Run()
        {
            if (IsSimulation() == 1)
            {
                motor_切刀上下馬達_Z1.SetPos(0);
                motor_切刀上下馬達_Z1.SetEncoderPos(0);

                motor_切刀上下馬達_Z2.SetPos(0);
                motor_切刀上下馬達_Z2.SetEncoderPos(0);
            }

            int SparkXLifeCount = (SReadValue("Z2_Spark_X_End").ToInt() - SReadValue("Z2_Spark_X_Start").ToInt()) / SReadValue("Spark_PitchX").ToInt();
            int SparkYLifeCount = (SReadValue("Spark_Y_End").ToInt() - SReadValue("Spark_Y_Start").ToInt()) / SReadValue("Spark_PitchY").ToInt();

            int SparkXNowCount = (SReadValue("Z2_Spark_X_Next").ToInt() - SReadValue("Z2_Spark_X_Start").ToInt()) / SReadValue("Spark_PitchX").ToInt();
            int SparkYNowCount = (SReadValue("Spark_Y_Next").ToInt() - SReadValue("Spark_Y_Start").ToInt()) / SReadValue("Spark_PitchY").ToInt();

            int SparkTotalLifeCount = SparkXLifeCount * SparkYLifeCount;
            int SparkNowCount = SparkXLifeCount * SparkYNowCount + SparkXNowCount;

            if (SparkNowCount >= SparkTotalLifeCount)
            {
                TeachStatus = 2; //更換測高塊
                TeachAlarmLevel = "i";
                TeachAlarmCode = 22;
                ShowAlarm("i", 22); //測高未完成，測高塊超過里程，請執行測高塊更換

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            ShowAlarm("I", 30, SparkTotalLifeCount.ToString(), SparkNowCount.ToString(), (SparkTotalLifeCount - SparkNowCount).ToString());

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_Z移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_YU移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切割平台旋轉馬達U, iMoveHadinU}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                //需先設定安全極限，設定值為目前測到的高度再往下100um 
                SparkTesterZ1.ZLimitPos = SReadValue("切割時Z1切刀切割道底部位置_Z").ToInt() - SReadValue("iSparkLimit").ToInt();
                SparkTesterZ2.ZLimitPos = SReadValue("切割時Z2切刀切割道底部位置_Z").ToInt() - SReadValue("iSparkLimit").ToInt();

                //接觸式測高元件Reset
                SparkTesterZ1.Reset();
                SparkTesterZ2.Reset();

                m_bIsSecondBottomTest = false; // 第一次下迴路檢查

                return SReadValue("bIgnoreSTCircuitCheck").ToBoolean() ?
                    FlowChart.FCRESULT.CASE1 : FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_中繼_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #region 斷線偵測

        #region 下迴路檢查
        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_Run()
        {
            if (TeachStatus == 99)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            else
            {
                Timer_Teach.Restart();
                return m_bIsSecondBottomTest? FlowChart.FCRESULT.CASE2 : FlowChart.FCRESULT.NEXT;
            }
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_開始_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_測高訊號開啟_Run()
        {
            SetOutBit(outBit_接觸式測高功能開啟, true);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_檢查_Run()
        {
            if (Timer_Teach.On(SReadValue("iChkSparkDelay").ToInt()) || IsSimulation() != 0)
            {
                SetOutBit(outBit_接觸式測高功能開啟, false);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (GetInBit(inBit_接觸測高導通訊號))
            {
                SetOutBit(outBit_接觸式測高功能開啟, false);
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 1;
                ShowAlarm("w", 1); //測高未完成，測高訊號異常，請吹乾蛋糕座與底板;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_下迴路訊號開啟_Run()
        {
            SetOutBit(outBit_接觸式測高功能開啟, true);
            SetOutBit(outBit_接觸測高斷線檢測, true);
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_檢查下迴路_Run()
        {
            if (Timer_Teach.On(SReadValue("iChkSparkDelay").ToInt()) || IsSimulation() != 0)
            {
                if (GetInBit(inBit_接觸測高導通訊號))
                {
                    if (outBit_接觸測高斷線檢測.Off(100))
                    {
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                }
                else
                {
                    if (outBit_接觸測高斷線檢測.Off(100))
                    {
                        TeachStatus = 99;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 8;
                        ShowAlarm("w", 8); //測高未完成，測高訊號異常，下迴路斷線偵測異常;

                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                }
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_下迴路_結束_Run()
        {
            if (outBit_接觸式測高功能開啟.Off(100))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }
        #endregion

        #region 上迴路檢查
        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_Run()
        {
            if (TeachStatus == 99)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            else
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_開始_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_偵測檔片接觸_Run()
        {
            SetOutBit(outBit_接觸式測高功能開啟, false);
            SetOutBit(outBit_測高塊偵測檔片接觸, true);

            if (inBit_測高塊偵測檔片接觸.On(1000) || IsSimulation() != 0)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    ShowAlarm("w", 10);//測高塊偵測檔片接觸訊號異常
                    Timer_Teach.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_YU移至偵測點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, SReadValue("測高塊偵測檔片點位_Y").ToInt()},
                {motor_切割平台旋轉馬達U, SReadValue("測高塊偵測檔片點位_U").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_上迴路訊號開啟_Run()
        {
            SetOutBit(outBit_接觸式測高功能開啟, true);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_檢查上迴路_Run()
        {
            if (Timer_Teach.On(SReadValue("iChkSparkDelay").ToInt()) || IsSimulation() != 0)
            {
                if (GetInBit(inBit_接觸測高導通訊號))
                {
                    SetOutBit(outBit_測高塊偵測檔片接觸, false);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    SetOutBit(outBit_測高塊偵測檔片接觸, false);
                    TeachStatus = 99;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 9;
                    ShowAlarm("w", 9); //測高未完成，測高訊號異常，上迴路斷線偵測異常;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_測高塊偵測檔片脫離_Run()
        {
            if (GetInBit(inBit_測高塊偵測檔片脫離))
            {
                SetOutBit(outBit_接觸式測高功能開啟, false);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    if (!GetInBit(inBit_測高塊偵測檔片脫離))
                        ShowAlarm("w", 11);//測高塊偵測檔片脫離訊號異常
                    Timer_Teach.Restart();
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_YU移回安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切割平台旋轉馬達U, iMoveHadinU}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_斷線偵測_上迴路_結束_Run()
        {
            SetOutBit(outBit_接觸式測高功能開啟, false);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }
        #endregion

        #endregion

        private FlowChart.FCRESULT FC_接觸測高_檢查真空_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_XYU移動_Run()
        {
            Motor motX;
            int nPosX;

            if (m_SparkTestSpindle == enSpindleSel.Z1)
            {
                motX = motor_切刀橫移馬達_Z1X;
                nPosX = SReadValue("Z2_Spark_X_Next").ToInt() -
                    SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt() + SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();
            }
            else
            {
                motX = motor_切刀橫移馬達_Z2X;
                nPosX = SReadValue("Z2_Spark_X_Next").ToInt();
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motX, nPosX},
                {motor_切割平台前後馬達Y, SReadValue("Spark_Y_Next").ToInt()},
                {motor_切割平台旋轉馬達U, SReadValue("Spark_U").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_主軸運轉中_Run()
        {
            enSpindleStatus st = m_SparkTestSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;

            if (st == enSpindleStatus.Run)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (st == enSpindleStatus.Stop)
            {
                if (m_SparkTestSpindle == enSpindleSel.Z1)
                {
                    Z1SpindleStart = true;
                }
                else
                {
                    Z2SpindleStart = true;
                }

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_啟動中_Run()
        {
            int nAlarmCode = m_SparkTestSpindle == enSpindleSel.Z1 ? 50 : 51;
            enSpindleStatus status = m_SparkTestSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;

            if (status == enSpindleStatus.Starting)
            {
                TeachAlarmLevel = "I";
                TeachAlarmCode = nAlarmCode;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Teach.On(SReadValue("主軸動作時間").ToInt()))
                {
                    //ShowAlarm("w", 2, "SparkTest", "Z1");
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_主軸啟動_Run()
        {
            enSpindleStatus status = m_SparkTestSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;

            if (status == enSpindleStatus.Run)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (Timer_Teach.On(SReadValue("主軸動作時間").ToInt()))
                {
                    TeachStatus = 99;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 2;
                    TeachMsg = new string[] { "Spark Test", m_SparkTestSpindle.ToString() };
                    ShowAlarm("w", 2, "SparkTest", m_SparkTestSpindle.ToString());

                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_速度Reset_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.切割速度);
            SetSpeed_Z1_X(enSpeedMode.切割速度);
            SetSpeed_Z2_Z(enSpeedMode.切割速度);
            SetSpeed_Z2_X(enSpeedMode.切割速度);
            SetSpeed_Y(enSpeedMode.切割後速度);
            SetSpeed_U(enSpeedMode.切割速度);

            TeachAlarmLevel = "I";
            TeachAlarmCode = 1;
            TeachMsg = new string[] { m_SparkTestSpindle.ToString() };

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_Z移到測高起始位_Run()
        {
            enSpindleStatus spSt = m_SparkTestSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;
            Motor motZ           = m_SparkTestSpindle == enSpindleSel.Z1 ? motor_切刀上下馬達_Z1 : motor_切刀上下馬達_Z2;
            CheckBeforeStart cbs = m_SparkTestSpindle == enSpindleSel.Z1 ? Z1CheckBeforeStart : Z2CheckBeforeStart;
            int nPosSpark        = m_SparkTestSpindle == enSpindleSel.Z1 ? SReadValue("Spark_Z1").ToInt() : SReadValue("Spark_Z2").ToInt();

            if (spSt == enSpindleStatus.Stop)
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 3;
                TeachMsg = new string[] { "Spark Test", m_SparkTestSpindle .ToString()};
                ShowAlarm("w", 3, "Spark Test", m_SparkTestSpindle.ToString());

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motZ, nPosSpark}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                SetOutBit(outBit_接觸式測高功能開啟, true);

                #region 測高點位計算
                //當測高X位置超出限制，回到起始點，Y進階一格
                int Spark_Y_Next = SReadValue("Spark_Y_Next").ToInt();
                int Spark_X_Next = SReadValue("Z2_Spark_X_Next").ToInt();
                if (Spark_X_Next + SReadValue("Spark_PitchX").ToInt() > SReadValue("Z2_Spark_X_End").ToInt())
                {
                    SSetValue("Z2_Spark_X_Next", SReadValue("Z2_Spark_X_Start").ToInt());
                    SSetValue("Spark_Y_Next", Spark_Y_Next + SReadValue("Spark_PitchY").ToInt());
                }
                else
                {
                    SSetValue("Z2_Spark_X_Next", Spark_X_Next + SReadValue("Spark_PitchX").ToInt());
                }

                SaveFile();
                #endregion

                cbs.ZSparkTestDone = false;

                m_bIsSecondBottomTest = !SReadValue("bIgnoreSTCircuitCheck").ToBoolean(); // 第二次進入下迴路檢知

                return m_bIsSecondBottomTest ? FlowChart.FCRESULT.CASE1 : FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_測高_Run()
        {
            //string errormsg = KnifeBreakCheck();
            //if (errormsg != "")
            //{
            //    TeachStatus = 99;
            //    //TeachMessage = errormsg;
            //    ShowAlarm("E", 70, errormsg);
            //    Timer_Teach.Restart();
            //    return FlowChart.FCRESULT.CASE1;
            //}

            Motor mot                 = m_SparkTestSpindle == enSpindleSel.Z1 ? motor_切刀上下馬達_Z1 : motor_切刀上下馬達_Z2;
            SparkTester st            = m_SparkTestSpindle == enSpindleSel.Z1 ? SparkTesterZ1 : SparkTesterZ2;
            enSpindleStatus eSpStatus = m_SparkTestSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;

            if (eSpStatus == enSpindleStatus.Stop)
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 3;
                TeachMsg = new string[] { "Spark Test", m_SparkTestSpindle.ToString() };
                ShowAlarm("w", 3, "Spark Test", m_SparkTestSpindle.ToString());
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            bool simu = false;

            if (IsSimulation() != 0)
            {
                int LattPosZ = SReadValue(string.Format("切割時{0}切刀切割道底部位置_Z", m_SparkTestSpindle.ToString())).ToInt();
                int Now = mot.ReadEncPos();
                if (Now < (LattPosZ - 10))
                    simu = true;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            TeachAlarmLevel = "I";
            TeachAlarmCode = m_SparkTestSpindle == enSpindleSel.Z1 ? 54 : 55;
            TeachMsg = new string[] { mot.ReadEncPos().ToString() };
            eSTState SparkState = st.Run(simu);

            if (SparkState == eSTState.DONE)
            {
                if (m_SparkTestSpindle == enSpindleSel.Z1) SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
                else SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);

                int gap = st.ContactPos - SReadValue(string.Format("切割時{0}切刀切割道底部位置_Z", m_SparkTestSpindle.ToString())).ToInt();
                if (gap >= 0 || Math.Abs(gap) <= SReadValue("iSparkLimit").ToInt())
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    TeachStatus = 99;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 4;
                    TeachMsg = new string[] { m_SparkTestSpindle.ToString(), st.ContactPos.ToString(), gap.ToString(), SReadValue("iSparkLimit").ToString() };
                    ShowAlarm("w", 4, m_SparkTestSpindle.ToString(), st.ContactPos, gap.ToString(), SReadValue("iSparkLimit").ToString());

                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else if (SparkState == eSTState.OUT_OF_LIMIT || SparkState == eSTState.SENSOR_SIGNAL_ABNORMAL || SparkState == eSTState.SWITCH_PIN_ABNORMAL)
            {
                TeachStatus = 99;

                if (SparkState == eSTState.OUT_OF_LIMIT)
                {
                    TeachAlarmLevel = "E";
                    TeachAlarmCode = m_SparkTestSpindle == enSpindleSel.Z1 ? 75 : 76;
                    ShowAlarm("E", TeachAlarmCode); // 接觸測高Z軸移動量超出極限設定值
                }
                else
                {
                    TeachAlarmLevel = "w";

                    if (SparkState == eSTState.SENSOR_SIGNAL_ABNORMAL)
                        TeachAlarmCode = 5; // 測高未完成，測高塊訊號異常
                    else
                        TeachAlarmCode = 57; // 測高未完成，測高開啟輸出訊號異常

                    ShowAlarm("w", TeachAlarmCode);
                }

                if (m_SparkTestSpindle == enSpindleSel.Z1) SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
                else SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_測高訊號關閉_Run()
        {
            if (GetSparkOff())//測高完成
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
                Timer_Teach.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_回到安全速度高度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
                SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
                SetSpeed_Y(enSpeedMode.掃瞄速度);
                SetSpeed_U(enSpeedMode.掃瞄速度);

                if (TeachStatus == 0 && IsSimulation() == 0)
                {
                    if (m_SparkTestSpindle == enSpindleSel.Z1)
                    {
                        SSetValue("切割時Z1切刀切割道底部位置_Z", SparkTesterZ1.ContactPos);
                        LogSay(EnLoggerType.EnLog_SPC, "更新切割時Z1切刀切割道底部位置_Z : " + SparkTesterZ1.ContactPos.ToString());
                    }
                    if (m_SparkTestSpindle == enSpindleSel.Z2)
                    {
                        SSetValue("切割時Z2切刀切割道底部位置_Z", SparkTesterZ2.ContactPos);
                        LogSay(EnLoggerType.EnLog_SPC, "更新切割時Z2切刀切割道底部位置_Z : " + SparkTesterZ2.ContactPos.ToString());
                    }
                }

                SaveFile();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_其他軸回到起始位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                if (TeachStatus != 0) return FlowChart.FCRESULT.CASE1;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_接觸測高_測高訊號關閉1_Run()
        {
            if (GetSparkOff())//測高終止
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);//警告，測高訊號異常導致Z軸鎖定，請檢查1.訊號[inBit_接觸測高導通訊號]是否關閉  2.Z軸是否能上升
                Timer_Teach.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_接觸測高_異常_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_磨耗值計算_Run()
        {
            int iNow磨耗值 = 0;

            int FlangeLimit = SReadValue("dFlandeLimit").ToInt();
            int PCBDepth    = PReadValue("PCBThickness").ToInt();
            int HeightG     = PReadValue("RubberThickness").ToInt();
            int SafeHeightH = SReadValue("SafeHeightH").ToInt();

            DataTable DA = Get_CutData();

            int Raise = SafeHeightH;

            if (DA.Rows.Count > 0)//提刀高度
                Raise = Convert.ToInt32(DA.Rows[0]["Cut_高度偏移"].ToString());

            CheckBeforeStart cbs = m_SparkTestSpindle == enSpindleSel.Z1 ? Z1CheckBeforeStart : Z2CheckBeforeStart;
            SparkTester st       = m_SparkTestSpindle == enSpindleSel.Z1 ? SparkTesterZ1 : SparkTesterZ2;
            strKnifeData skd     = m_SparkTestSpindle == enSpindleSel.Z1 ? Tool1InfoGet() : Tool2InfoGet();
            int nSpcKnifeRemain  = (int)skd.dRealKnifeRemain;

            TeachMsg = new string[] { string.Format("{0}測高完成", m_SparkTestSpindle.ToString()) };

            if (!cbs.ZSparkTestDone)
            {
                string strTmp = string.Format("Blade{0}Data", m_SparkTestSpindle.ToString());
                string strTmp1 = string.Format("iRealMeasureZPos{0}", m_SparkTestSpindle.ToString());

                //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                PSetValue(strTmp, strTmp1, st.ContactPos, "Int");
            }

            iNow磨耗值 = Get接觸測高後計算磨耗值(m_SparkTestSpindle.ToString());
            int RealKnifeRemain = nSpcKnifeRemain - HeightG + Raise - PCBDepth - FlangeLimit - iNow磨耗值;

            //磨耗值限制 = 實際刀露量 - ( 膠墊高度 - 高度偏移 ) - 料片厚度 - 料片與法蘭最近距離
            if (RealKnifeRemain <= 0)
            {
                TeachMsg = new string[] { string.Format("{0}測高完成", m_SparkTestSpindle.ToString()),
                    string.Format("{0}磨耗量已達極限，請執行換刀，公式:實際刀露量[{1}]-" +
                    "膠墊高度[{2}]+提刀高度[{3}]-料片厚度[{4}]-料片與法蘭最近距離[{5}]-本次磨耗量[{6}]",
                    m_SparkTestSpindle.ToString(), nSpcKnifeRemain, HeightG, Raise, PCBDepth, FlangeLimit, iNow磨耗值)};
            }

            skd.sToolName = PReadValue(string.Format("Blade{0}Data.sToolName{1}", m_SparkTestSpindle.ToString(), m_SparkTestSpindle.ToString())).ToString();

            TeachMsg = new string[] { string.Format("{0}測高完成", m_SparkTestSpindle.ToString()),
                string.Format("，刀片序號: {0}，當前磨耗: {1} um，磨耗極限: {2} um", skd.sToolName, iNow磨耗值, RealKnifeRemain) };

            cbs.ZSparkTestDone = true;
            cbs.ZNCTestDone = false;

            TeachAlarmLevel = "I";
            TeachAlarmCode = 12;
            ShowAlarm("I", 12, TeachMsg);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_接觸測高_動作結束_Run()
        {
            if (m_bIsAutoNeedSparkTestZ1 || m_bIsAutoNeedSparkTestZ2)
            {
                m_AutoSparkTestFinishEvt.Set();
            }
            else
                Teach_ActionResult = 1;

            m_SparkTestSpindle = enSpindleSel.NONE;
            return FlowChart.FCRESULT.IDLE;
        }
        #endregion 接觸測高流程

        #region 非接觸測高流程

        private FlowChart.FCRESULT FC_非接觸測高_動作開始_Run()
        {
            TeachStatusNCTZ1 = 0;
            TeachStatusNCTZ2 = 0;
            TeachFlowStep = 0;
            TeachMessageNCTZ1 = "";
            TeachMessageNCTZ2 = "";

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #region 清潔

        private FlowChart.FCRESULT FC_非接觸測高_清潔_Run()
        {
            TeachMessageNCTZ1 = "";
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_非接觸測高_清潔_開啟清潔水柱_Run()
        {
            SetOutBit(outBit_非接觸測高清潔水柱, true);
            TeachMessageNCTZ1 = "開啟清潔水柱";

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_非接觸測高_清潔_Delay_Run()
        {
            if (Timer_Teach.On(SReadValue("iNcsCleanWaterDelay").ToInt()))
            {
                SetOutBit(outBit_非接觸測高清潔水柱, false);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_非接觸測高_清潔_開啟清潔氣旋_Run()
        {
            SetOutBit(outBit_非接觸測高清潔氣旋, true);
            TeachMessageNCTZ1 = "開啟清潔氣旋";

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_非接觸測高_清潔_Delay1_Run()
        {
            if (Timer_Teach.On(SReadValue("iNcsCleanAirDelay").ToInt()))
            {
                SetOutBit(outBit_非接觸測高清潔氣旋, false);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #endregion

        private FlowChart.FCRESULT FC_非接觸測高_執行軸_Run()
        {
            Teach_ActionResultNCTZ1 = true;
            Teach_ActionResultNCTZ2 = true;

            if (m_bIsAutoNeedNContactTestZ1 || m_bIsTeachNeedNContactTestZ1)
            {
                Teach_ActionResultNCTZ1 = false;
                FC_Z1非接觸測高_動作開始.TaskReset();
            }
            if (m_bIsAutoNeedNContactTestZ2 || m_bIsTeachNeedNContactTestZ2)
            {
                Teach_ActionResultNCTZ2 = false;
                FC_Z2非接觸測高_動作開始.TaskReset();
            }

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_非接觸測高_執行_Run()
        {
            if (!Teach_ActionResultNCTZ1) FC_Z1非接觸測高_動作開始.MainRun();

            if (!Teach_ActionResultNCTZ2) FC_Z2非接觸測高_動作開始.MainRun();

            if (Teach_ActionResultNCTZ1 && Teach_ActionResultNCTZ2)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_非接觸測高_完成_Run()
        {
            if (m_bIsAutoNeedNContactTestZ1 || m_bIsAutoNeedNContactTestZ2)
            {
                m_bIsAutoNeedNContactTestZ1 = false;
                m_bIsAutoNeedNContactTestZ2 = false;
                m_AutoNContactTestFinishEvt.Set();
            }
            else if (m_bIsTeachNeedNContactTestZ1 || m_bIsTeachNeedNContactTestZ2)
            {
                m_bIsTeachNeedNContactTestZ1 = false;
                m_bIsTeachNeedNContactTestZ2 = false;
                Teach_ActionResult = 1;
            }

            return default(FlowChart.FCRESULT);
        }

        #region Z1非接觸測高

        private FlowChart.FCRESULT FC_Z1非接觸測高_動作開始_Run()
        {
            TeachStatusNCTZ1 = 0;
            TeachFlowStep = 0;
            TeachMessageNCTZ1 = "Z1非接觸測高流程開始(Non Contact Spark Test Start)";
            ShowAlarm("I", 11, "Z1");//非接觸測高流程開始

            Timer_Z1NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_檢查電壓速度_Run()
        {
            if (IsSimulation() == 1)
            {
                motor_切刀上下馬達_Z1.SetPos(0);
                motor_切刀上下馬達_Z1.SetEncoderPos(0);
            }

            float fZ1v = analogIn_Z1非接觸測高值.AvgValue;
            double fSetZ1v = SReadValue("Z1NcsSetVoltage").ToDouble();
            bool CheckOK;

            if (IsSimulation() == 0)
            {
                CheckOK = Math.Abs(fZ1v - fSetZ1v) / fSetZ1v * 100 < SReadValue("NcsChkValue").ToInt();
            }
            else
                CheckOK = true;

            if (CheckOK)
            {
                SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
                SetSpeed_Y(enSpeedMode.掃瞄速度);
                SetSpeed_U(enSpeedMode.掃瞄速度);
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                TeachStatusNCTZ1 = 99;//電壓不正常
                TeachMessageNCTZ1 = "Z1非接觸測高未完成，電壓訊號異常";
                ShowAlarm("E", 77);
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_Z軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_X軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt() - 30000}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT))
            {
                TeachStatusNCTZ1 = 0;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_主軸運轉中_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Run)
            {
                TeachStatusNCTZ1 = 0;
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
            {
                Z1SpindleStart = true;
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_啟動中_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
            {
                TeachMessageNCTZ1 = "Z1啟動中";

                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Z1NCT.On(SReadValue("主軸動作時間").ToInt()))
            {
                ShowAlarm("w", 2, "NonContactSparkTest", "Z1");

                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_主軸啟動_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Run)
            {
                TeachStatusNCTZ1 = 0;

                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ1 = 99;
                ShowAlarm("w", 2, "NonContactSparkTest", "Z1");

                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_速度相關參數設定_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.切割速度);
            SetSpeed_Z1_X(enSpeedMode.切割速度);
            TeachMessageNCTZ1 = "Z1主軸運轉中";

            //需先設定安全極限，設定值為目前測到的高度再往下100um
            NCTesterZ1.ZLimitPos = SReadValue("Z1非接觸測高位置_Z").ToInt() - SReadValue("iNCTLimit").ToInt();
            NCTesterZ1.Reset();

            Timer_Z1NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_非接觸護蓋開啟_Run()
        {
            SetOutBit(outBit_Z1非接觸式測高護蓋開啟, true);

            if (GetInBit(inBit_Z1非接觸測高蓋開啟檢知))
            {
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Z1NCT.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("E", 71, "Z1", "Open");
                Timer_Z1NCT.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_X軸移到測高位_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ1 = 99;
                TeachMessageNCTZ1 = "Z1 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z1");
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("NCS_X1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_Z軸移到測高起始位_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ1 = 99;
                TeachMessageNCTZ1 = "Z1 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z1");
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("NCS_Z1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_Z1測高_Run()
        {
            if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ1 = 99;
                TeachMessageNCTZ1 = "Z1 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z1");
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            bool simu = false;
            bool simumode = false; ;

            if (IsSimulation() != 0)
            {
                simumode = true;
                int LastPosZ1 = SReadValue("Z1非接觸測高位置_Z").ToInt();
                int Now = motor_切刀上下馬達_Z1.ReadPos();

                if (!Z1CheckBeforeStart.ZNCTestDone)
                {
                    if (Now <= LastPosZ1)
                        simu = true;
                }
                else
                {
                    if (Now < (LastPosZ1 - 10))
                        simu = true;
                }
            }

            TeachMessageNCTZ1 = "Z1位置" + motor_切刀上下馬達_Z1.ReadEncPos().ToString();
            eNCSState NCSStateZ1 = NCTesterZ1.Run(simu, simumode);

            if (NCSStateZ1 == eNCSState.DONE)
            {
                int Pos = NCTesterZ1.ContactPos;
                int gap = Pos - SReadValue("Z1非接觸測高位置_Z").ToInt();

                if (gap >= 0 || Math.Abs(gap) <= SReadValue("iNCTLimit").ToInt())
                {
                    SSetValue("Z1非接觸測高位置_Z", NCTesterZ1.ContactPos);
                    SaveFile();
                }
                else
                {
                    TeachStatusNCTZ1 = 99; //與上次量測差異值過大
                    TeachMessageNCTZ1 = "Z1測高未完成，與上次差異值過大";
                    ShowAlarm("w", 7, "Z1", NCTesterZ1.ContactPos.ToString(), gap.ToString(), SReadValue("iNCTLimit").ToString());
                }

                SetSpeed_Z1_Z(enSpeedMode.切割速度);
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (NCSStateZ1 == eNCSState.OUTOFLIMIT || NCSStateZ1 == eNCSState.INITVOLTAGEMISMATCH)
            {
                if (NCSStateZ1 == eNCSState.OUTOFLIMIT)
                {
                    TeachStatusNCTZ1 = 99;
                    TeachMessageNCTZ1 = "Z1非接觸測高未完成，移動量超出極限設定值";
                    ShowAlarm("E", 62);//Z1非接觸測高Z軸移動量超出極限設定值
                }
                else if (NCSStateZ1 == eNCSState.INITVOLTAGEMISMATCH)
                {
                    TeachStatusNCTZ1 = 99;
                    TeachMessageNCTZ1 = "Z1非接觸測高未完成，電壓訊號異常";
                    ShowAlarm("E", 77);
                }

                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_回到安全高度速度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT))
            {
                SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z1_X(enSpeedMode.掃瞄速度);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_X軸移到安全位2_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt() - 30000}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT))
            {
                TeachStatusNCTZ1 = 0;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_非接觸護蓋關閉_Run()
        {
            SetOutBit(outBit_Z1非接觸式測高護蓋關閉, true);

            if (GetInBit(inBit_Z1非接觸測高蓋關閉檢知))
            {
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            else if (Timer_Z1NCT.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("E", 71, "Z1", "Close");
                Timer_Z1NCT.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_其他軸回到初始位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z1NCT))
            {
                if (TeachStatusNCTZ1 == 0)
                    return FlowChart.FCRESULT.NEXT;
                else
                    return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_Z1磨耗值計算_Run()
        {
            double RealKnifeRemain;
            int 接觸測高磨耗值 = 0;
            int 非接觸測高磨耗值 = 0;

            strKnifeData Data_1 = Tool1InfoGet();

            int FlangeLimit = SReadValue("dFlandeLimit").ToInt();
            int SPCKnifeRemain_1 = (int)Data_1.dRealKnifeRemain;

            int PCBDepth = PReadValue("PCBThickness").ToInt();
            int HeightG = PReadValue("RubberThickness").ToInt();
            int SafeHeightH = SReadValue("SafeHeightH").ToInt();

            DataTable DA = Get_CutData();

            int Raise = SafeHeightH;

            if (DA.Rows.Count > 0)//提刀高度
                Raise = Convert.ToInt32(DA.Rows[0]["Cut_高度偏移"].ToString());

            TeachMessageNCTZ1 = "Z1非接觸測高完成";
            //+ By Max 20211209, v4.0.100.0, 有成功再做磨耗值的計算
            if (TeachStatusNCTZ1 != 99)
            {
                if (!Z1CheckBeforeStart.ZNCTestDone)
                {
                    //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                    //+ By Max 20211209, v4.0.100.0 SparkTesterZ1.ContactPos -> NCTesterZ1.ContactPos
                    PSetValue("BladeZ1Data", "iSimMeasureZPosZ1", NCTesterZ1.ContactPos, "Int");
                    Z1CheckBeforeStart.ZNCTestDone = true;
                    Z1CheckBeforeStart.ZNCTestSecondDone = false;
                }
                else
                {
                    Z1CheckBeforeStart.ZNCTestSecondDone = true;

                    接觸測高磨耗值 = Get接觸測高後計算磨耗值("Z1");
                    非接觸測高磨耗值 = Get非接觸測高後計算磨耗值("Z1");

                    RealKnifeRemain = SPCKnifeRemain_1 - HeightG + Raise - PCBDepth - FlangeLimit - 接觸測高磨耗值 - 非接觸測高磨耗值;

                    //磨耗值限制 = 實際刀露量 - ( 膠墊高度 - 高度偏移 ) - 料片厚度 - 料片與法蘭最近距離
                    if (RealKnifeRemain <= 0)
                    {
                        TeachMessageNCTZ1 += string.Format("Z1磨耗量已達極限，請執行換刀，公式:實際刀露量[{0}]-" +

                        "膠墊高度[{1}]+提刀高度[{2}]-料片厚度[{3}]-料片與法蘭最近距離[{4}]-本次磨耗量[{5}]",
                        SPCKnifeRemain_1, HeightG, Raise, PCBDepth, FlangeLimit, 接觸測高磨耗值 + 非接觸測高磨耗值);

                    }

                    Tool1_InfoReturn.sToolName = PReadValue("BladeZ1Data.sToolNameZ1").ToString();

                    TeachMessageNCTZ1 += string.Format("，刀片序號: {0}，當前磨耗: {1} um，磨耗極限: {2} um", Tool1_InfoReturn.sToolName, 接觸測高磨耗值 + 非接觸測高磨耗值, RealKnifeRemain);
                }
            }

            ShowAlarm("I", 12, TeachMessageNCTZ1);
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_動作結束_Run()
        {
            Teach_ActionResultNCTZ1 = true;
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_異常_Run()
        {
            Timer_Z1NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_異常中繼1_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1非接觸測高_異常中繼2_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #region Z2非接觸測高

        private FlowChart.FCRESULT FC_Z2非接觸測高_動作開始_Run()
        {
            TeachStatusNCTZ2 = 0;
            TeachFlowStep = 0;
            TeachMessageNCTZ2 = "Z2非接觸測高流程開始(Non Contact Spark Test Start)";
            ShowAlarm("I", 11, "Z2");//非接觸測高流程開始

            Timer_Z2NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_檢查電壓速度_Run()
        {
            if (IsSimulation() == 1)
            {
                motor_切刀上下馬達_Z2.SetPos(0);
                motor_切刀上下馬達_Z2.SetEncoderPos(0);
            }

            float fZ2v = analogIn_Z2非接觸測高值.AvgValue;
            double fSetZ2v = SReadValue("Z2NcsSetVoltage").ToDouble();
            bool CheckOK;

            if (IsSimulation() == 0)
            {
                CheckOK = Math.Abs(fZ2v - fSetZ2v) / fSetZ2v * 100 < SReadValue("NcsChkValue").ToInt();
            }
            else
                CheckOK = true;

            if (CheckOK)
            {
                SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
                SetSpeed_Y(enSpeedMode.掃瞄速度);
                SetSpeed_U(enSpeedMode.掃瞄速度);
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                TeachStatusNCTZ2 = 99;//電壓不正常
                TeachMessageNCTZ2 = "Z2非接觸測高未完成，電壓訊號異常";
                ShowAlarm("E", 78);
                Timer_Z1NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_Z移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_X軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt() + 30000}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT))
            {
                TeachStatusNCTZ2 = 0;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_主軸運轉中_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Run)
            {
                TeachStatusNCTZ2 = 0;
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                Z2SpindleStart = true;
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.IDLE;

        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_啟動中_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
            {
                TeachMessageNCTZ2 = "Z2啟動中";
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Z2NCT.On(SReadValue("主軸動作時間").ToInt()))
            {
                ShowAlarm("w", 2, "NonContactSparkTest", "Z2");
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_主軸啟動_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Run)
            {
                TeachStatusNCTZ2 = 0;
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ1 = 99;
                ShowAlarm("w", 2, "NonContactSparkTest", "Z2");
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_速度相關參數設定_Run()
        {
            SetSpeed_Z2_Z(enSpeedMode.切割速度);
            SetSpeed_Z2_X(enSpeedMode.切割速度);
            TeachMessageNCTZ2 = "Z2主軸運轉中";

            //需先設定安全極限，設定值為目前測到的高度再往下1mm
            NCTesterZ2.ZLimitPos = SReadValue("Z2非接觸測高位置_Z").ToInt() - SReadValue("iNCTLimit").ToInt();
            NCTesterZ2.Reset();

            Timer_Z2NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_非接觸護蓋開啟_Run()
        {
            SetOutBit(outBit_Z2非接觸式測高護蓋開啟, true);

            if (GetInBit(inBit_Z2非接觸測高蓋開啟檢知))
            {
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Z2NCT.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("E", 71, "Z2", "Open");
                Timer_Z2NCT.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_X軸移到測高位_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ2 = 99;
                TeachMessageNCTZ2 = "Z2 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z2");
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, SReadValue("NCS_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_Z軸移到測高起始位_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ2 = 99;
                TeachMessageNCTZ2 = "Z2 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z2");
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("NCS_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_Z2測高_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
            {
                TeachStatusNCTZ2 = 99;
                TeachMessageNCTZ2 = "Z2 Spindle Stop";
                ShowAlarm("w", 3, "Non Contact Spark Test", "Z2");
                return FlowChart.FCRESULT.CASE1;
            }

            bool bIsSimuSensorOn = false;
            bool bIsSimulationMode = IsSimulation() != 0;

            if (bIsSimulationMode)
            {
                int LastPosZ2 = SReadValue("Z2非接觸測高位置_Z").ToInt();
                int Now = motor_切刀上下馬達_Z2.ReadPos();

                if (!Z2CheckBeforeStart.ZNCTestDone)
                {
                    if (Now <= LastPosZ2)
                        bIsSimuSensorOn = true;
                }
                else
                {
                    if (Now < (LastPosZ2 - 10))
                        bIsSimuSensorOn = true;
                }
            }

            TeachMessageNCTZ2 = "Z2位置" + motor_切刀上下馬達_Z2.ReadEncPos().ToString();
            eNCSState NCSStateZ2 = NCTesterZ2.Run(bIsSimuSensorOn, bIsSimulationMode);

            if (NCSStateZ2 == eNCSState.DONE)
            {
                int Pos = NCTesterZ2.ContactPos;
                int gap = Pos - SReadValue("Z2非接觸測高位置_Z").ToInt();

                if (gap >= 0 || Math.Abs(gap) <= SReadValue("iNCTLimit").ToInt())
                {
                    SSetValue("Z2非接觸測高位置_Z", NCTesterZ2.ContactPos);
                    SaveFile();
                }
                else
                {
                    TeachStatusNCTZ2 = 99; //與上次量測差異值過大
                    TeachMessageNCTZ2 = "Z2測高未完成，與上次差異值過大";
                    ShowAlarm("w", 7, "Z2", NCTesterZ2.ContactPos.ToString(), gap.ToString(), SReadValue("iNCTLimit").ToString());
                }

                SetSpeed_Z2_Z(enSpeedMode.切割速度);
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (NCSStateZ2 == eNCSState.OUTOFLIMIT || NCSStateZ2 == eNCSState.INITVOLTAGEMISMATCH)
            {

                if (NCSStateZ2 == eNCSState.OUTOFLIMIT)
                {
                    TeachStatusNCTZ1 = 99;
                    TeachMessageNCTZ2 = "Z2非接觸測高未完成，移動量超出極限設定值";
                    ShowAlarm("E", 63);//Z1非接觸測高Z軸移動量超出極限設定值
                }
                else if (NCSStateZ2 == eNCSState.INITVOLTAGEMISMATCH)
                {
                    TeachStatusNCTZ1 = 99;
                    TeachMessageNCTZ2 = "Z2非接觸測高未完成，電壓訊號異常";
                    ShowAlarm("E", 78);
                }

                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_回到安全高度速度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT))
            {
                SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
                SetSpeed_Z2_X(enSpeedMode.掃瞄速度);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_X軸移到安全位2_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt() + 30000}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT))
            {
                TeachStatusNCTZ2 = 0;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_非接觸護蓋關閉_Run()
        {
            SetOutBit(outBit_Z2非接觸式測高護蓋關閉, true);

            if (GetInBit(inBit_Z2非接觸測高蓋關閉檢知))
            {
                Timer_Z2NCT.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            else if (Timer_Z2NCT.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                ShowAlarm("E", 71, "Z2", "Close");
                Timer_Z2NCT.Restart();
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_其他軸回到初始位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Z2NCT))
            {
                if (TeachStatusNCTZ2 == 0)
                    return FlowChart.FCRESULT.NEXT;
                else
                    return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_Z2磨耗值計算_Run()
        {
            double RealKnifeRemain;
            int 接觸測高磨耗值 = 0;
            int 非接觸測高磨耗值 = 0;

            strKnifeData Data_2 = Tool2InfoGet();

            int FlangeLimit = SReadValue("dFlandeLimit").ToInt();
            int SPCKnifeRemain_2 = (int)Data_2.dRealKnifeRemain;

            int PCBDepth = PReadValue("PCBThickness").ToInt();
            int HeightG = PReadValue("RubberThickness").ToInt();
            int SafeHeightH = SReadValue("SafeHeightH").ToInt();

            DataTable DA = Get_CutData();

            int Raise = SafeHeightH;

            if (DA.Rows.Count > 0)//提刀高度
                Raise = Convert.ToInt32(DA.Rows[0]["Cut_高度偏移"].ToString());

            TeachMessageNCTZ2 = "Z2非接觸測高完成";
            //+ By Max 20211209, v4.0.100.0, 有成功再做磨耗值的計算
            if (TeachStatusNCTZ2 != 99)
            {
                if (!Z2CheckBeforeStart.ZNCTestDone)
                {
                    //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                    //+ By Max 20211209, v4.0.100.0 SparkTesterZ1.ContactPos -> NCTesterZ1.ContactPos
                    PSetValue("BladeZ2Data", "iSimMeasureZPosZ2", NCTesterZ2.ContactPos, "Int");
                    Z2CheckBeforeStart.ZNCTestDone = true;
                    Z2CheckBeforeStart.ZNCTestSecondDone = false;
                }
                else
                {
                    Z2CheckBeforeStart.ZNCTestSecondDone = true;

                    接觸測高磨耗值 = Get接觸測高後計算磨耗值("Z2");
                    非接觸測高磨耗值 = Get非接觸測高後計算磨耗值("Z2");

                    RealKnifeRemain = SPCKnifeRemain_2 - HeightG + Raise - PCBDepth - FlangeLimit - 接觸測高磨耗值 - 非接觸測高磨耗值;

                    //磨耗值限制 = 實際刀露量 - ( 膠墊高度 - 高度偏移 ) - 料片厚度 - 料片與法蘭最近距離
                    if (RealKnifeRemain <= 0)
                    {
                        TeachMessageNCTZ2 += string.Format("Z1磨耗量已達極限，請執行換刀，公式:實際刀露量[{0}]-" +

                        "膠墊高度[{1}]+提刀高度[{2}]-料片厚度[{3}]-料片與法蘭最近距離[{4}]-本次磨耗量[{5}]",
                        SPCKnifeRemain_2, HeightG, Raise, PCBDepth, FlangeLimit, 接觸測高磨耗值 + 非接觸測高磨耗值);

                    }

                    Tool2_InfoReturn.sToolName = PReadValue("BladeZ2Data.sToolNameZ2").ToString();

                    TeachMessageNCTZ2 += string.Format("，刀片序號: {0}，當前磨耗: {1} um，磨耗極限: {2} um", Tool2_InfoReturn.sToolName, 接觸測高磨耗值 + 非接觸測高磨耗值, RealKnifeRemain);
                }
            }

            ShowAlarm("I", 12, TeachMessageNCTZ2);
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_動作結束_Run()
        {
            Teach_ActionResultNCTZ2 = true;
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_異常_Run()
        {
            Timer_Z2NCT.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_異常中繼1_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2非接觸測高_異常中繼2_Run()
        {
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        #endregion

        #region 自動對焦流程
        private FlowChart.FCRESULT FC_教導_自動對焦_開始_Run()
        {
            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 71;
            TeachMsg = new string[] { motor_切刀上下馬達_Z2.ReadEncPos().ToString() };
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_Z移至安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_開啟鏡頭_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_取得對焦初始位置_Run()
        {
            m_dFocusMaxScore = 0;

            if (CallAutoFocus == "Auto") //自動模式下，且有靶點資料
            {
                AutoFocusZ = Math.Min(
                    Convert.ToInt32(LearnMarkPos.dtScanData.Rows[0]["Scan_Z"].ToString()) + SReadValue("AutoFocusLimit").ToInt(),
                    SReadValue("Z2_AutoFocusMaxValue").ToInt());
                Timer_Teach.Restart();
            }
            else
            {
                AutoFocusZ = SReadValue("Z2_AutoFocusMaxValue").ToInt();
            }

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_Z移至對焦位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, AutoFocusZ}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachMsg = new string[] { motor_切刀上下馬達_Z2.ReadEncPos().ToString() };
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_Delay_Run()
        {
            if (Timer_Teach.On(SReadValue("GrabDelayTM").ToInt()))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_傳送對焦命令_Run()
        {
            string cmd = "IAF";
            string index = "1";

            if (SetCCDCmd(cmd, index))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 40);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_取得通訊結果_Run()
        {
            switch (mCCDAction)//等待回傳結果
            {
                case CCDSendResult.OK:
                    if (IsSimulation() == 0)
                    {
                        if (m_dFocusMaxScore < Convert.ToDouble(CCDResult[0]))
                        {
                            m_dFocusMaxScore = Convert.ToDouble(CCDResult[0]);
                            LearnMarkPos.nHighBaseFocus = AutoFocusZ;
                        }
                    }
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.NG:
                case CCDSendResult.InspectFail:
                    ShowAlarm("w", 41);
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 42);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_通訊NG_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_計算_Run()
        {
            AutoFocusZ -= SReadValue("AutoFocusFeed").ToInt();

            if (CallAutoFocus == "Auto")
            {
                int nMinHeightZ = Math.Max(
                    Convert.ToInt32(LearnMarkPos.dtScanData.Rows[0]["Scan_Z"].ToString()) - SReadValue("AutoFocusLimit").ToInt(),
                    SReadValue("Z2_AutoFocusMinValue").ToInt());

                if (AutoFocusZ < nMinHeightZ)
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }
            else
            {
                if (AutoFocusZ < SReadValue("Z2_AutoFocusMinValue").ToInt())
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_Z移回對焦位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, LearnMarkPos.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                PSetValue("iZ2_FoucusPos", LearnMarkPos.nHighBaseFocus, "Int");
                SaveFile();
                TeachAlarmLevel = "I";
                TeachAlarmCode = 72;
                TeachMsg = new string[] { LearnMarkPos.nHighBaseFocus.ToString() };
                ShowAlarm("I", 72, LearnMarkPos.nHighBaseFocus.ToString());

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_自動對焦_結束_Run_1()
        {
            AutoFocusFinish = true;
            CallAutoFocus = "";
            return default(FlowChart.FCRESULT);
        }
        #endregion 自動對焦流程

        #region 快速自動對焦流程

        Dictionary<int, double> m_dicPosScorePair = new Dictionary<int, double>();
        bool m_bIsBigStep = true;

        private FlowChart.FCRESULT FC_教導_快速自動對焦_開始_Run()
        {
            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 71;
            TeachMsg = new string[] { motor_切刀上下馬達_Z2.ReadEncPos().ToString() };

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_Z移至安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_開啟鏡頭_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_取得對焦初始位置_Run()
        {
            m_dicPosScorePair.Clear();
            m_bIsBigStep = true;

            AutoFocusZ = SReadValue("Z2_AutoFocusMaxValue").ToInt();

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_Z移至對焦位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, AutoFocusZ}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachMsg = new string[] { motor_切刀上下馬達_Z2.ReadEncPos().ToString() };
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_Delay_Run()
        {
            if (Timer_Teach.On(SReadValue("GrabDelayTM").ToInt()))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_傳送對焦命令_Run()
        {
            string cmd = "IAF";
            string index = "1";

            if (SetCCDCmd(cmd, index))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 40);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_取得通訊結果_Run()
        {
            switch (mCCDAction)//等待回傳結果
            {
                case CCDSendResult.OK:
                    if (IsSimulation() == 0)
                    {
                        m_dicPosScorePair.Add(AutoFocusZ, Convert.ToDouble(CCDResult[0]));
                    }
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.NG:
                case CCDSendResult.InspectFail:
                    ShowAlarm("w", 41);
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 42);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_通訊NG_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_計算_Run()
        {
            int nCnt = m_dicPosScorePair.Count();

            if (m_bIsBigStep)
            {
                if (nCnt >= 3 &&
                    m_dicPosScorePair.ElementAt(nCnt - 1).Value < m_dicPosScorePair.ElementAt(nCnt - 2).Value &&
                    m_dicPosScorePair.ElementAt(nCnt - 2).Value < m_dicPosScorePair.ElementAt(nCnt - 3).Value)
                {
                    //AutoFocusZ += SReadValue("AutoFocusFeed").ToInt();
                    AutoFocusZ = m_dicPosScorePair.ElementAt(nCnt - 2).Key;
                    m_dicPosScorePair.Clear();
                    m_bIsBigStep = false;
                }
                else
                {
                    if (AutoFocusZ <= SReadValue("Z2_AutoFocusMinValue").ToInt())
                    {
                        AutoFocusZ = SReadValue("Z2_AutoFocusMinValue").ToInt();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else
                        AutoFocusZ -= 200;
                }

                return FlowChart.FCRESULT.CASE1;
            }
            else
            {
                if (nCnt >= 3 &&
                    m_dicPosScorePair.ElementAt(nCnt - 1).Value < m_dicPosScorePair.ElementAt(nCnt - 2).Value &&
                    m_dicPosScorePair.ElementAt(nCnt - 2).Value < m_dicPosScorePair.ElementAt(nCnt - 3).Value)
                {
                    AutoFocusZ = m_dicPosScorePair.ElementAt(nCnt - 3).Key;
                    LearnMarkPos.nHighBaseFocus = AutoFocusZ;

                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (AutoFocusZ >= SReadValue("Z2_AutoFocusMaxValue").ToInt())
                    {
                        AutoFocusZ = SReadValue("Z2_AutoFocusMaxValue").ToInt();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else
                    {
                        AutoFocusZ += SReadValue("AutoFocusFeed").ToInt();
                        return FlowChart.FCRESULT.CASE1;
                    }
                }
            }
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_Z移回對焦位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, LearnMarkPos.nHighBaseFocus}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                //+ By Max 20210324, v4.0.1.58, 傳入資料型別
                PSetValue("iZ2_FoucusPos", LearnMarkPos.nHighBaseFocus, "Int");
                SaveFile();
                TeachAlarmLevel = "I";
                TeachAlarmCode = 72;
                TeachMsg = new string[] { LearnMarkPos.nHighBaseFocus.ToString() };
                ShowAlarm("I", 72, LearnMarkPos.nHighBaseFocus.ToString());

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_教導_快速自動對焦_結束_Run()
        {
            AutoFocusFinish = true;
            CallAutoFocus = "";
            return default(FlowChart.FCRESULT);
        }

        #endregion

        #region 基準線校正
        private FlowChart.FCRESULT FC_基準線校正_動作開始_Run()
        {
            TeachStatus = 0;
            TeachFlowStep = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 13;
            TeachMsg = new string[] { m_BaseLineSpindle.ToString() };
            ShowAlarm("I", 13, m_BaseLineSpindle.ToString());//基準線校正開始

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #region 入料
        private FlowChart.FCRESULT FC_基準線校正_入料_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_動作開始_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_速度設定_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_Z軸移至焦距_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("基準線焦距").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_IO初始化_Run()
        {
            //CCD/NCS
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, false);
            SetOutBit(outBit_非接觸測高清潔水柱, false);
            SetOutBit(outBit_非接觸測高清潔氣旋, false);

            //Spindle
            SetOutBit(outBit_Z1刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);
            SetOutBit(outBit_主軸軸承鎖定, false);
            SetOutBit(outBit_Z2刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);

            //Table
            SetOutBit(outBit_切割平台真空建立, false);

            bool bZ1cover = GetInBit(inBit_Z1刀輪護蓋關閉);
            bool bZ2cover = GetInBit(inBit_Z2刀輪護蓋關閉);
            if (Timer_Teach.On(SReadValue("汽缸動作時間").ToInt()))
            {
                if (bZ1cover && bZ2cover)
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
                {
                    if (!bZ1cover && !bZ2cover)
                        ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text + "," + outBit_Z2刀輪護蓋關閉.Text);
                    else if (!bZ1cover)
                        ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text);
                    else if (!bZ2cover)
                        ShowAlarm("i", 1, outBit_Z2刀輪護蓋關閉.Text);

                    Timer_Teach.Restart();
                }
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_其他軸移至初始_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveInitialU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_等待入料_Run()
        {
            TeachFlowStep = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 14;
            ShowAlarm("I", 14);//步驟一:請在平台上放置料片
            TeachUserCheckOk = false;

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private void FC_基準線校正_入料_等待確認入料_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_等待確認入料_Run()
        {
            if (TeachUserCheckOk)
            {
                //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
                if (bAirPumpStd)
                {
                    PumpOpenTime = PReadValue("PCBVacuumTime").ToInt();
                }
                else
                {
                    PumpOpenTime = SReadValue("iAirPumpSTartTimes").ToInt();
                }

                SetOutBit(outBit_切割平台真空建立, true);
                TeachUserCheckOk = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_真空檢查_Run()
        {
            if (CheckVaca(0) == "")
            {
                m_bIsNeedCheckVaca = true;//基準線校正開始
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
            else if (Timer_Teach.On(PumpOpenTime))
            {
                SetOutBit(outBit_切割平台真空建立, false);
                TeachAlarmLevel = "E";
                TeachAlarmCode = 16;
                TeachMsg = new string[] { "Baseline" };
                ShowAlarm("E", 16, "Baseline");
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_動作結束_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }
        #endregion

        private FlowChart.FCRESULT FC_基準線校正_XYU軸在圓心_Run()
        {
            int nPosY = SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
            int nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveInitialU},
                {motor_切割平台前後馬達Y, nPosY},
                {motor_切刀橫移馬達_Z2X, nPosX2}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_視覺通訊_Run()
        {
            return OpenVisionLive(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_基準線校正_OK_Run()
        {
            switch (mCCDAction)//等待LIVE
            {
                case CCDSendResult.OK:
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    TeachFlowStep = 1;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 16;
                    TeachUserCheckOk = false;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachFlowStep = 1;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 17;
                TeachUserCheckOk = false;
                ShowAlarm("w", 17);
                Timer_Teach.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private void FC_基準線校正_NG_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachFlowStep = 0;
                TeachUserCheckOk = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_CCD遮片打開_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);

            TeachUserCheckOk = false;
            TeachFlowStep = 1;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 15;
            ShowAlarm("I", 15);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private void FC_基準線校正_是否儲存設定_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_是否儲存設定_Run()
        {
            if (TeachUserCheckOk)
            {
                int iTmp_Pos = BaseLineT.X - motor_切刀橫移馬達_Z2X.ReadEncPos();
                if (m_BaseLineSpindle == enSpindleSel.Z1)
                {
                    int Z1CCD與切刀中心點距離 = SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();
                    Z1CCD與切刀中心點距離 += iTmp_Pos;
                    SSetValue("Z2CCD與Z1切刀中心點距離_X", Z1CCD與切刀中心點距離);
                    SaveFile();
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 44;
                    TeachMsg = new string[] { iTmp_Pos.ToString() };
                    ShowAlarm("I", 44, iTmp_Pos.ToString());
                }
                else if (m_BaseLineSpindle == enSpindleSel.Z2)
                {
                    int Z2CCD與切刀中心點距離 = SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt();
                    Z2CCD與切刀中心點距離 += iTmp_Pos;
                    SSetValue("Z2CCD與Z2切刀中心點距離_X", Z2CCD與切刀中心點距離);
                    SaveFile();
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 45;
                    TeachMsg = new string[] { iTmp_Pos.ToString() };
                    ShowAlarm("I", 45, iTmp_Pos.ToString());
                }

                TeachFlowStep = 0;
                TeachUserCheckOk = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (TeachUserCheckSkip)
            {
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 46;
                ShowAlarm("I", 46);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_主軸停止_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_入料_停止_Run()
        {
            if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
                return FlowChart.FCRESULT.NEXT;
            return default(FlowChart.FCRESULT);
        }

        #region 等待點位設定完成
        private FlowChart.FCRESULT FC_基準線校正_等待點位設定完成_Run()
        {
            TeachAlarmLevel = "I";
            TeachAlarmCode = 56;
            TeachMsg = new string[] { m_BaseLineSpindle.ToString() };
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private void FC_基準線校正_設定第一點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_設定第一點_Run()
        {
            if (TeachUserCheckOk)
            {
                BaseLineT.X = motor_切刀橫移馬達_Z2X.ReadPos();
                BaseLineT.Y = motor_切割平台前後馬達Y.ReadPos();
                SSetValue("基準線焦距", motor_切刀上下馬達_Z2.ReadEncPos());
                SaveFile();
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_移到第二點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveInitialU},
                {motor_切割平台前後馬達Y, SReadValue("圓心點位_Y").ToInt() + Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt())}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 1;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 16;
                ShowAlarm("I", 16);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private void FC_基準線校正_設定第二點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_設定第二點_Run()
        {
            if (TeachUserCheckOk)
            {
                BaseLineB.X = motor_切刀橫移馬達_Z2X.ReadPos();
                BaseLineB.Y = motor_切割平台前後馬達Y.ReadPos();
                //v4.0.1.17 基準線轉正
                int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
                int Org_Y = SReadValue("圓心點位_Y").ToInt();
                Double angle = 0;
                RotateCorrect(Org_X, Org_Y, ref BaseLineT, ref BaseLineB, ref angle);
                int i_angel = Convert.ToInt32((angle / 360) * iDD馬達一轉Pulse數);
                BaseLine_theta = iMoveInitialU + i_angel;
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_轉正_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, BaseLine_theta},
                {motor_切割平台前後馬達Y, BaseLineB.Y},
                {motor_切刀橫移馬達_Z2X, BaseLineB.X}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 21;
                ShowAlarm("I", 21);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private void FC_基準線校正_移到轉正第二點_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_移到轉正第二點_Run()
        {
            if (TeachUserCheckOk)
            {
                var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, BaseLineT.Y},
                    {motor_切刀橫移馬達_Z2X, BaseLineT.X}
                };

                if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
                {
                    TeachUserCheckOk = false;
                    TeachFlowStep = 0;
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 22;
                    ShowAlarm("I", 22);

                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        private void FC_基準線校正_點位設定完成1_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private void FC_基準線校正_設定點位完成_BeforeRun()
        {
            //+ By Max 20210309
            TeachUserCheckOk = false;
        }

        private FlowChart.FCRESULT FC_基準線校正_設定點位完成_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }
        #endregion

        private FlowChart.FCRESULT FC_基準線校正_CCD遮片關閉_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_Z軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_X軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_XY軸到起始點位_Run()
        {
            Motor motX;
            int nPosX;

            if (m_BaseLineSpindle == enSpindleSel.Z1)
            {
                motX = motor_切刀橫移馬達_Z1X;
                nPosX = BaseLineT.X + SReadValue("Z2CCD與Z1切刀中心點距離_X").ToInt();
            }
            else
            {
                motX = motor_切刀橫移馬達_Z2X;
                nPosX = BaseLineT.X + SReadValue("Z2CCD與Z2切刀中心點距離_X").ToInt();
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motX, nPosX},
                {motor_切割平台前後馬達Y, SReadValue("切割起始前方點位_Y").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_速度設定_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.切割速度);
            SetSpeed_Z1_X(enSpeedMode.切割速度);
            SetSpeed_Z2_Z(enSpeedMode.切割速度);
            SetSpeed_Z2_X(enSpeedMode.切割速度);
            SetSpeed_Y(enSpeedMode.切割後速度);
            SetSpeed_U(enSpeedMode.切割速度);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_運轉中_Run()
        {
            enSpindleStatus status = m_BaseLineSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;
            if (status == enSpindleStatus.Run)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (status == enSpindleStatus.Stop)
            {
                if (m_BaseLineSpindle == enSpindleSel.Z1) Z1SpindleStart = true;
                else Z2SpindleStart = true;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_啟動中_Run()
        {
            if (m_BaseLineSpindle == enSpindleSel.Z1)
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Starting)
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 50;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }
            else if (m_BaseLineSpindle == enSpindleSel.Z2)
            {
                if (m_enZ2SpindleStatus == enSpindleStatus.Starting)
                {
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 51;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            if (Timer_Teach.On(SReadValue("主軸動作時間").ToInt()))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_主軸啟動_Run()
        {
            enSpindleStatus st = m_BaseLineSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;
            if (st == enSpindleStatus.Run)
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (Timer_Teach.On(SReadValue("主軸動作時間").ToInt()))
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 2;
                TeachMsg = new string[] { "BaseLine", m_BaseLineSpindle.ToString() };
                ShowAlarm("w", 2, "BaseLine", m_BaseLineSpindle.ToString());
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_開啟切割水_Run()
        {
            if (m_BaseLineSpindle == enSpindleSel.Z1)
            {
                if (m_enZ1SpindleStatus == enSpindleStatus.Stop)
                {
                    TeachFlowStep = 2;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 3;
                    TeachMsg = new string[] { "BaseLine", "Z1" };
                    ShowAlarm("w", 3, "BaseLine", "Z1");
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }

                analogOut_Z1噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z1);
                analogOut_Z1灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z1);

                SetOutBit(outBit_Z1水開關電磁閥, true);

                if (CheckWater(enSpindleSel.Z1))
                {
                    m_bIsTeachPauseNeedToContinue = true;
                    SetSpeed_Y(enSpeedMode.刀痕速度);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }

                if (Timer_Teach.On(SReadValue("水開啟準備時間").ToInt()))
                {
                    if (!CheckWater(enSpindleSel.Z1))
                    {
                        ShowAlarm("w", 14, "Z1"); //Z1噴水/灑水流量未到達
                    }
                    Timer_Teach.Restart();
                }
            }
            else if (m_BaseLineSpindle == enSpindleSel.Z2)
            {
                if (m_enZ2SpindleStatus == enSpindleStatus.Stop)
                {
                    TeachFlowStep = 2;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 3;
                    TeachMsg = new string[] { "BaseLine", "Z2" };
                    ShowAlarm("w", 3, "BaseLine", "Z2");
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }

                analogOut_Z2噴水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SHOWER, enSpindleSel.Z2);
                analogOut_Z2灑水座流量控制.Value = ConvertFromVolumnToVolt(enCutWaterSel.SPRAY, enSpindleSel.Z2);

                SetOutBit(outBit_Z2水開關電磁閥, true);

                if (CheckWater(enSpindleSel.Z2))
                {
                    m_bIsTeachPauseNeedToContinue = true;
                    SetSpeed_Y(enSpeedMode.刀痕速度);
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }

                if (Timer_Teach.On(SReadValue("水開啟準備時間").ToInt()))
                {
                    if (!CheckWater(enSpindleSel.Z2))
                    {
                        ShowAlarm("w", 14, "Z2"); //Z2噴水/灑水流量未到達
                    }
                    Timer_Teach.Restart();
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_Z軸移到試切位_Run()
        {
            enSpindleStatus status = m_BaseLineSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;
            Motor motZ = m_BaseLineSpindle == enSpindleSel.Z1 ? motor_切刀上下馬達_Z1 : motor_切刀上下馬達_Z2;
            Motor motX = m_BaseLineSpindle == enSpindleSel.Z1 ? motor_切刀橫移馬達_Z1X : motor_切刀橫移馬達_Z2X;

            if (status == enSpindleStatus.Stop)
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 3;
                TeachMsg = new string[] { "BaseLine", m_BaseLineSpindle.ToString() };
                ShowAlarm("w", 3, "BaseLine", m_BaseLineSpindle.ToString());
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            int nPosZ = SReadValue(string.Format("切割時{0}切刀切割道底部位置_Z", m_BaseLineSpindle.ToString())).ToInt() + 
                PReadValue("BaselineOffsetZ").ToInt();

            var dicMotorPosPairs = new Dictionary<Motor, int>() { { motZ, nPosZ } };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {              
                LogSay(EnLoggerType.EnLog_SPC, string.Format("基準線校正起始位 X : {0}, Y : {1}, Z : {2}",
                    motX.ReadPos(), motor_切割平台前後馬達Y.ReadPos(), nPosZ));
                
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_Y軸移動關水_Run()
        {
            enSpindleStatus status = m_BaseLineSpindle == enSpindleSel.Z1 ? m_enZ1SpindleStatus : m_enZ2SpindleStatus;
            Motor motX = m_BaseLineSpindle == enSpindleSel.Z1 ? motor_切刀橫移馬達_Z1X : motor_切刀橫移馬達_Z2X;
            Motor motZ = m_BaseLineSpindle == enSpindleSel.Z1 ? motor_切刀上下馬達_Z1 : motor_切刀上下馬達_Z2;

            if (status == enSpindleStatus.Stop)
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 3;
                TeachMsg = new string[] { "BaseLine", m_BaseLineSpindle.ToString() };
                ShowAlarm("w", 3, "BaseLine", m_BaseLineSpindle.ToString());

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            VaccError = CheckVaca(0);
            bool bIsWaterCheck = CheckWater(m_BaseLineSpindle);

            if (VaccError != "")
            {
                TeachStatus = 99;
                TeachAlarmLevel = "E";
                TeachAlarmCode = 70;
                TeachMsg = new string[] { VaccError };
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            if (!bIsWaterCheck && IsSimulation() == 0)
            {
                TeachStatus = 99;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 14;
                TeachMsg = new string[] { m_BaseLineSpindle.ToString() };
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台前後馬達Y, SReadValue("切割起始後方點位_Y").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                LogSay(EnLoggerType.EnLog_SPC, string.Format("基準線校正結束位 X : {0}, Y : {1}, Z : {2}",
                    motX.ReadPos(), motor_切割平台前後馬達Y.ReadPos(), motZ.ReadPos()));

                if (m_BaseLineSpindle == enSpindleSel.Z1)
                {
                    SetOutBit(outBit_Z1水開關電磁閥, false);
                }
                else if (m_BaseLineSpindle == enSpindleSel.Z2)
                {
                    SetOutBit(outBit_Z2水開關電磁閥, false);
                }

                m_bIsTeachPauseNeedToContinue = false;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_抬刀_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                ShowAlarm("E", 70, TeachMsg);
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_Z軸移到安全位1_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_速度設定1_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_XY軸移回設定點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, BaseLineT.X},
                {motor_切割平台前後馬達Y, BaseLineT.Y}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_Z2軸移到對焦點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("基準線焦距").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_CCD遮片打開1_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);
            TeachUserCheckOk = false;
            TeachFlowStep = 1;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 23;
            ShowAlarm("I", 23);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_點位設定完成1_Run()
        {
            if (TeachUserCheckOk)
            {
                int iTmp_Pos = BaseLineT.X - motor_切刀橫移馬達_Z2X.ReadEncPos();

                TeachAlarmLevel = "I";
                TeachAlarmCode = 24;
                TeachMsg = new string[] { m_BaseLineSpindle.ToString(), iTmp_Pos.ToString() };
                ShowAlarm("I", 24, m_BaseLineSpindle.ToString(), iTmp_Pos);

                TeachFlowStep = 3;
                TeachUserCheckOk = false;
                TeachUserCheckSkip = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_基準線校正_CCD遮片關閉1_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_基準線校正_Z軸移到安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_其他軸移回安全點位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveInitialU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X,  SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X,  SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                if (TeachStatus == 0)
                {
                    if (m_BaseLineSpindle == enSpindleSel.Z1)
                    {
                        Z1CheckBeforeStart.ZKerfCheckDone = true;
                    }
                    else if (m_BaseLineSpindle == enSpindleSel.Z2)
                    {
                        Z2CheckBeforeStart.ZKerfCheckDone = true;
                    }
                }

                m_BaseLineSpindle = enSpindleSel.NONE; //v4.0.1.32 基準線校正完成後須將主軸設定清空
                SetOutBit(outBit_切割平台真空建立, false); //關真空

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_基準線校正_動作結束_Run()
        {
            Teach_ActionResult = 1;
            return FlowChart.FCRESULT.IDLE;
        }
        #endregion 基準線校正

        #region 靶點學習

        private FlowChart.FCRESULT FC_靶點學習_動作開始_Run()
        {
            TeachFixture.Reset();
            if (Load_FixtureData())
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                ShowAlarm("E", 8);//切割道資料未建立，請先學習切割道！
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.IDLE;
            }
        }

        #region 入料

        private FlowChart.FCRESULT FC_靶點學習_入料_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_動作開始_Run()
        {
            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 37;
            ShowAlarm("I", 37);//靶點學習開始

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_速度設定_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_Z軸移至安全高度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_IO初始化_Run()
        {
            //CCD/NCS
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, true);
            SetOutBit(outBit_高倍CCD視域範圍清潔, false);
            SetOutBit(outBit_高倍CCD鏡頭清潔, false);
            SetOutBit(outBit_非接觸測高清潔水柱, false);
            SetOutBit(outBit_非接觸測高清潔氣旋, false);
            //CCD/NCS

            //Spindle Z1
            SetOutBit(outBit_Z1水開關電磁閥, false);
            SetOutBit(outBit_主軸軸承鎖定, false);
            SetOutBit(outBit_Z1刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z1刀輪護蓋關閉, true);

            //Spindle Z2
            SetOutBit(outBit_Z2水開關電磁閥, false);
            SetOutBit(outBit_Z2刀側邊清潔氣旋, false);
            SetOutBit(outBit_Z2刀輪護蓋關閉, true);

            //Table
            SetOutBit(outBit_切割區增濕水簾, false);

            bool r1 = GetInBit(inBit_Z1刀輪護蓋關閉);
            bool r2 = GetInBit(inBit_Z2刀輪護蓋關閉);

            if (Timer_Teach.On(SReadValue("汽缸動作時間").ToInt()))
            {
                if (r1 && r2)
                {
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (Timer_Teach.On(SReadValue("汽缸逾時時間").ToInt()))
                    {
                        if (!r1) ShowAlarm("i", 1, outBit_Z1刀輪護蓋關閉.Text);
                        if (!r2) ShowAlarm("i", 1, outBit_Z2刀輪護蓋關閉.Text);

                        Timer_Teach.Restart();
                    }
                }
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_其他軸移至初始_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_等待入料_Run()
        {
            TeachFlowStep = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 38;
            ShowAlarm("I", 38);
            TeachUserCheckOk = false;

            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_等待確認入料_Run()
        {
            if (TeachUserCheckOk)
            {
                //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
                PumpOpenTime = bAirPumpStd ? PReadValue("PCBVacuumTime").ToInt() : SReadValue("iAirPumpSTartTimes").ToInt();

                SetOutBit(outBit_切割平台真空建立, true);

                TeachUserCheckOk = false;
                TeachFlowStep = 0;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_入料_真空檢查_Run()
        {
            if (CheckVaca(0) == "")
            {
                m_bIsNeedCheckVaca = true;//靶點學習開始

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
            else
            {
                if (Timer_Teach.On(PumpOpenTime))
                {
                    SetOutBit(outBit_切割平台真空建立, false);
                    TeachAlarmLevel = "E";
                    TeachAlarmCode = 16;
                    TeachMsg = new string[] { "Learn Mark" };
                    ShowAlarm("E", 16, "Learn Mark");
                    TeachUserCheckOk = false;

                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            return default(FlowChart.FCRESULT);
        }

        #endregion

        private FlowChart.FCRESULT FC_靶點學習_參數設定_Run()
        {
            LearnMarkPos.Reset();
            LearnMarkPos.strSide = "CH2";
            LearnMarkPos.nStep = 1;//左上第一點

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_XYU移至中心點_Run()
        {
            bool b位置找到 = false;
            int iθ位移 = 0;

            b位置找到 = ChannelRotation(LearnMarkPos.strSide, out iθ位移);

            if (b位置找到)
            {
                LearnMarkPos.nTempθ = iMoveInitialU - iDD馬達一轉Pulse數 * iθ位移 / 4;
            }
            else
            {
                ShowAlarm("E", 90, "Learn Mark Channel error");
                return default(FlowChart.FCRESULT);
            }

            int nPosY = 0;
            int nPosX2 = 0;

            if (iθ位移 == 0)
            {
                nPosY = SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
            }
            else
            {
                nPosY = SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt());
                nPosX2 = SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt());
            }

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, nPosY},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, nPosX2},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
                SetOutBit(outBit_高倍CCD鏡頭清潔, true);
                SetOutBit(outBit_高倍CCD視域範圍清潔, true);
                TeachUserCheckOk = false;
                TeachFlowStep = 1;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 39;
                TeachMsg = new string[] { (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Left-Top" };
                ShowAlarm("I", 39, (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Left-Top");

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 掃靶定位

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_視覺Live_Run()
        {
            return OpenVisionLive(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_等待結果_Run()
        {
            switch (mCCDAction)//等待LIVE
            {
                case CCDSendResult.OK:
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    TeachFlowStep = 0;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 16;
                    TeachUserCheckOk = false;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }

            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 17;
                ShowAlarm("w", 17);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_等待確認_Run()
        {
            if (TeachUserCheckOk)
            {
                //LearnMarkPos.TempX = motor_Z2切刀橫移馬達_X.ReadEncPos();
                //LearnMarkPos.TempY = motor_切割平台前後馬達Y.ReadEncPos();
                LearnMarkPos.nTempX = motor_切刀橫移馬達_Z2X.ReadPos();
                LearnMarkPos.nTempY = motor_切割平台前後馬達Y.ReadPos();
                TeachFlowStep = 0;
                TeachUserCheckOk = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_視覺通訊_Run()
        {
            return PatternMatch(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_等待結果2_Run()
        {
            //回傳Sh,Pos,X=?,Y=?,A=?,S=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:
                    int Score = (int)(Convert.ToDouble(CCDResult[5].Split('=')[1]) * 100);
                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[2].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[3].Split('=')[1]);
                    LearnMarkPos.nTempX += xoffset;
                    LearnMarkPos.nTempY += yoffset;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        TeachUserCheckOk = false;
                        TeachFlowStep = 0;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 18;
                        ShowAlarm("w", 18);//視覺取像失敗，請按繼續重試
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 19;
                ShowAlarm("w", 19);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_移至中心_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.nTempX},
                {motor_切割平台前後馬達Y, LearnMarkPos.nTempY}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_NG1_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_視覺通訊1_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 28);
                Timer_Teach.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_等待結果1_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                    LearnMarkPos.nTempX += xoffset;
                    LearnMarkPos.nTempY += yoffset;
                    switch (LearnMarkPos.nStep)
                    {
                        case 1:
                        case 4:
                            LearnMarkPos.ptLTMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptLTMarkPos.Y = LearnMarkPos.nTempY;
                            break;
                        case 2:
                        case 5:
                            LearnMarkPos.ptLBMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptLBMarkPos.Y = LearnMarkPos.nTempY;
                            break;
                        case 3:
                        case 6:
                            LearnMarkPos.ptRTMarkPos.X = LearnMarkPos.nTempX;
                            LearnMarkPos.ptRTMarkPos.Y = LearnMarkPos.nTempY;
                            break;

                    }
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        TeachUserCheckOk = false;
                        TeachFlowStep = 0;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 20;
                        ShowAlarm("w", 20);//視覺取像失敗，請按繼續重試
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 21;
                ShowAlarm("w", 21);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_存檔_Run()
        {
            string cmd = "SF";
            string para = PReadValue("視覺軟體程式名稱").ToString();
            if (SetCCDCmd(cmd, para))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                ShowAlarm("w", 29);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_等待結果3_Run()
        {
            switch (mCCDAction)//等待存檔結果
            {
                case CCDSendResult.OK:
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    TeachUserCheckOk = false;
                    TeachFlowStep = 0;
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 22;
                    ShowAlarm("w", 22);//視覺儲存失敗，請按繼續重試
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 23;
                ShowAlarm("w", 23);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_結束_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_NG2_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_掃靶定位_Delay_Run()
        {
            if (Timer_Teach.On(10 * SReadValue("馬達整定時間").ToInt()))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        #endregion

        private FlowChart.FCRESULT FC_靶點學習_移至下一點_Run()
        {
            LearnMarkPos.nStep++;
            switch (LearnMarkPos.nStep)
            {
                case 2://CH2左下角
                    LearnMarkPos.nTempY += PReadValue("Num_row").ToInt() * PReadValue("Pitch_Y").ToInt();
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case 3://CH2右上角
                    LearnMarkPos.nTempX += PReadValue("Num_col").ToInt() * PReadValue("Pitch_X").ToInt();
                    LearnMarkPos.nTempY -= PReadValue("Num_row").ToInt() * PReadValue("Pitch_Y").ToInt();
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case 4://CH2計算->拉直->CH1左上角
                    LearnMarkPos.nFindMarkStep = 0;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
                case 5://CH1左下角 
                    LearnMarkPos.nTempY += PReadValue("Num_col").ToInt() * PReadValue("Pitch_X").ToInt();
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case 6://CH1右上角
                    LearnMarkPos.nTempX += PReadValue("Num_row").ToInt() * PReadValue("Pitch_Y").ToInt();
                    LearnMarkPos.nTempY -= PReadValue("Num_col").ToInt() * PReadValue("Pitch_X").ToInt();
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case 7://CH1計算->拉直
                    LearnMarkPos.nFindMarkStep = 0;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_靶點學習_移動_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, LearnMarkPos.nTempθ},
                {motor_切割平台前後馬達Y, LearnMarkPos.nTempY},
                {motor_切刀橫移馬達_Z2X, LearnMarkPos.nTempX},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 1;
                switch (LearnMarkPos.nStep)
                {
                    case 2:
                    case 5:
                        TeachAlarmLevel = "I";
                        TeachAlarmCode = 39;
                        TeachMsg = new string[] { (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Left-Bottom" };
                        ShowAlarm("I", 39, (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Left-Bottom");
                        break;
                    case 3:
                    case 6:
                        TeachAlarmLevel = "I";
                        TeachAlarmCode = 39;
                        TeachMsg = new string[] { (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Right-Top" };
                        ShowAlarm("I", 39, (LearnMarkPos.nStep + 1).ToString(), LearnMarkPos.strSide, "Right-Top");
                        break;
                }

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #endregion

        #region 圓心校正

        private int CalibrateCenter_Step = 0;//圓心校正步驟
        private Point CenterLT = new Point(0, 0);
        private Point CenterRB = new Point(0, 0);
        private Point CenterTemp = new Point(0, 0);//暫時做計算
        private int m_nOffsetX;
        private int m_nCurrentPos;

        private FlowChart.FCRESULT FC_圓心校正_開始_Run()
        {
            SetSpeed_Z1_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z1_X(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_Z(enSpeedMode.掃瞄速度);
            SetSpeed_Z2_X(enSpeedMode.掃瞄速度);
            SetSpeed_Y(enSpeedMode.掃瞄速度);
            SetSpeed_U(enSpeedMode.掃瞄速度);

            TeachFlowStep = 0;
            TeachStatus = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 67;
            CalibrateCenter_Step = 0;

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_圓心校正_Z移到安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_XYU移至安全位置_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("圓心點位_X").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "i";
                TeachAlarmCode = 17;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_放入料片_Run()
        {
            if (TeachUserCheckOk)
            {
                //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
                if (bAirPumpStd)
                {
                    PumpOpenTime = PReadValue("PCBVacuumTime").ToInt();
                }
                else
                {
                    PumpOpenTime = SReadValue("iAirPumpSTartTimes").ToInt();
                }

                SetOutBit(outBit_切割平台真空建立, true);
                TeachUserCheckOk = false;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_確認真空_Run()
        {
            if (CheckVaca(0) == "")
            {
                m_bIsNeedCheckVaca = true;//圓心校正開始
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            //v4.0.1.25 真空開啟時間因PUMP還沒開而不同
            else if (Timer_Teach.On(PumpOpenTime))
            {
                ShowAlarm("w", 44, GetChuckTableAIValue().ToString(), PReadValue("PCBVacuumThreshold").ToString());
                SetOutBit(outBit_切割平台真空建立, false);
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_移至第一點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("圓心點位_Y").ToInt() - Convert.ToInt32(PReadValue("Num_row").ToDouble() / 2 * PReadValue("Pitch_Y").ToInt())},
                {motor_切刀橫移馬達_Z2X, SReadValue("圓心點位_X").ToInt() - Convert.ToInt32(PReadValue("Num_col").ToDouble() / 2 * PReadValue("Pitch_X").ToInt())}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 47;
                ShowAlarm("I", 47);

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_Z降至對焦高度_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z2, SReadValue("基準線焦距").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_開啟CCD護蓋_Run()
        {
            SetOutBit(outBit_高倍CCD鏡頭遮蔽, false);
            SetOutBit(outBit_高倍CCD視域範圍清潔, true);
            SetOutBit(outBit_高倍CCD鏡頭清潔, true);

            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_圓心校正_視覺Live_Run()
        {
            return OpenVisionLive(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_圓心校正_等待結果_Run()
        {
            switch (mCCDAction)//等待Live
            {
                case CCDSendResult.OK:
                    TeachUserCheckOk = false;
                    TeachFlowStep = 1;
                    TeachAlarmLevel = "I";
                    TeachAlarmCode = 48;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;
                case CCDSendResult.NG:
                case CCDSendResult.InspectFail:
                    TeachAlarmLevel = "w";
                    TeachAlarmCode = 16;
                    TeachUserCheckOk = false;
                    TeachFlowStep = 0;
                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.CASE1;
            }

            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachAlarmLevel = "w";
                TeachAlarmCode = 17;
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                ShowAlarm("w", 17);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT 確認第一點_Run()
        {
            if (TeachUserCheckOk)
            {
                CenterLT.X = motor_切刀橫移馬達_Z2X.ReadPos();
                CenterLT.Y = motor_切割平台前後馬達Y.ReadPos();
                TeachUserCheckOk = false;
                SSetValue("基準線焦距", motor_切刀上下馬達_Z2.ReadEncPos());
                SaveFile();
                CalibrateCenter_Step = 1;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 12;
                CenterTemp = CenterLT;
                ShowAlarm("I", 12, "圓心校正第一點設定完成");

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        #region 定位

        private FlowChart.FCRESULT FC_圓心校正_定位_Run()
        {
            if (CalibrateCenter_Step == 1)
            {
                return FlowChart.FCRESULT.CASE1;
            }

            if (CalibrateCenter_Step == 2)
            {
                return FlowChart.FCRESULT.NEXT;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_視覺通訊PM_Run()
        {
            return PatternMatch(Timer_Teach);
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_等待結果_Run()
        {
            //回傳Sh,Pos,X=?,Y=?,A=?,S=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int Score = (int)(Convert.ToDouble(CCDResult[5].Split('=')[1]) * 100);
                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[2].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[3].Split('=')[1]);

                    CenterTemp.X += xoffset;
                    CenterTemp.Y += yoffset;

                    m_nOffsetX = xoffset;

                    Timer_Teach.Restart();
                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        TeachUserCheckOk = false;
                        TeachFlowStep = 1;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 18;
                        ShowAlarm("w", 18);//視覺取像失敗，請按繼續重試
                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }
            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 19;
                ShowAlarm("w", 19);
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_NG_Run()
        {
            if (TeachUserCheckOk)
            {
                CenterTemp.X = motor_切刀橫移馬達_Z2X.ReadPos();
                CenterTemp.Y = motor_切割平台前後馬達Y.ReadPos();
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return default(FlowChart.FCRESULT);

        }

        private void FC_圓心校正_定位_移至中心_BeforeRun()
        {
            m_nCurrentPos = motor_切刀橫移馬達_Z2X.ReadPos();
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_移至中心_Run()
        {
            bool bIsXMoveDone = false;
            bool bIsYMoveDone = motor_切割平台前後馬達Y.G00(CenterTemp.Y);

            if (SReadValue("BackClearance").ToBoolean())
            {
                bIsXMoveDone = motor_切刀橫移馬達_Z2X.G00(m_nCurrentPos, m_nOffsetX);
            }
            else
            {
                bIsXMoveDone = motor_切刀橫移馬達_Z2X.G00(CenterTemp.X);
            }

            if (bIsXMoveDone && bIsYMoveDone)
            {
                TeachFlowStep = 0;
                TeachUserCheckOk = false;
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("馬達逾時時間").ToInt()))
            {
                if (!bIsYMoveDone) MotorOverTime(motor_切割平台前後馬達Y.Text);
                if (!bIsXMoveDone) MotorOverTime(motor_切刀橫移馬達_Z2X.Text);

                Timer_Teach.Restart();
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_視覺通訊FM_Run()
        {
            string cmd = "HMGMP";//Find Mark
            string index = "1";//視域中心

            if (SetCCDCmd(cmd, index))
            {
                Timer_Teach.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                ShowAlarm("w", 28);
                Timer_Teach.Restart();
            }
            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_等待結果FM_Run()
        {
            //回傳X=?,Y=?;
            switch (mCCDAction)//等待定位結果
            {
                case CCDSendResult.OK:

                    int xoffset = -1 * (int)Convert.ToDouble(CCDResult[0].Split('=')[1]);
                    int yoffset = 1 * (int)Convert.ToDouble(CCDResult[1].Split('=')[1]);

                    CenterTemp.X += xoffset;
                    CenterTemp.Y += yoffset;
                    switch (CalibrateCenter_Step)
                    {
                        case 1:
                            CenterLT.X = CenterTemp.X;
                            CenterLT.Y = CenterTemp.Y;
                            break;
                        case 2:
                            CenterRB.X = CenterTemp.X;
                            CenterRB.Y = CenterTemp.Y;
                            break;
                    }

                    return FlowChart.FCRESULT.NEXT;

                case CCDSendResult.InspectFail:
                case CCDSendResult.NG:
                    {
                        TeachUserCheckOk = false;
                        TeachFlowStep = 1;
                        TeachAlarmLevel = "w";
                        TeachAlarmCode = 20;
                        ShowAlarm("w", 20);//視覺取像失敗，請按繼續重試

                        Timer_Teach.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
            }

            if (Timer_Teach.On(SReadValue("VisionComOverTime").ToInt()))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "w";
                TeachAlarmCode = 21;
                ShowAlarm("w", 21);

                Timer_Teach.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            return default(FlowChart.FCRESULT);
        }

        private FlowChart.FCRESULT FC_圓心校正_定位_結束_Run()
        {
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        private FlowChart.FCRESULT FC_圓心校正_自動移至第二點_Run()
        {
            CenterRB.X = 2 * SReadValue("圓心點位_X").ToInt() - CenterLT.X;
            CenterRB.Y = 2 * SReadValue("圓心點位_Y").ToInt() - CenterLT.Y;

            int posu = iMoveHadinU - (iDD馬達一轉Pulse數 / 2);

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, posu},
                {motor_切割平台前後馬達Y, CenterRB.Y},
                {motor_切刀橫移馬達_Z2X, CenterRB.X}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                TeachUserCheckOk = false;
                TeachFlowStep = 0;
                TeachAlarmLevel = "I";
                TeachAlarmCode = 68;
                CenterTemp = CenterRB;
                CalibrateCenter_Step = 2;

                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_計算_Run()
        {
            int Org_X = SReadValue("圓心點位_X").ToInt();  //機構圓心
            int Org_Y = SReadValue("圓心點位_Y").ToInt();
            int NewOrg_X = (CenterLT.X + CenterRB.X) / 2;
            int NewOrg_Y = (CenterLT.Y + CenterRB.Y) / 2;
            SSetValue("圓心點位_X", NewOrg_X);
            SSetValue("圓心點位_Y", NewOrg_Y);
            SaveFile();

            TeachFlowStep = 0;
            TeachAlarmLevel = "I";
            TeachAlarmCode = 49;
            TeachMsg = new string[] { (NewOrg_X - Org_X).ToString(), (NewOrg_Y - Org_Y).ToString() };
            Timer_Teach.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_圓心校正_Z回歸初始點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀上下馬達_Z1, SReadValue("安全點位_Z1").ToInt()},
                {motor_切刀上下馬達_Z2, SReadValue("安全點位_Z2").ToInt()}
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_XYU回歸初始點_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切割平台旋轉馬達U, iMoveHadinU},
                {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                {motor_切刀橫移馬達_Z2X, SReadValue("圓心點位_X").ToInt()},
            };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_Teach))
            {
                SetOutBit(outBit_切割平台真空建立, false);//關真空
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_圓心校正_完成_Run()
        {
            Teach_ActionResult = 1;
            return default(FlowChart.FCRESULT);
        }

        #endregion

        #endregion 教導流程

        #region 主軸啟動/關閉

        //private int m_nRotateSpeed = 0;

        #region Z1主軸控制
        private FlowChart.FCRESULT FC_Z1主軸控制_動作開始_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_初始設定_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_等待啟動訊號_Run()
        {
            if (ScanSpindleInfo(enSpindleSel.Z1))
            {
                if (Z1SpindleStart)
                {
                    Z1SpindleStart = false;
                    m_enZ1SpindleStatus = enSpindleStatus.Starting;
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 啟動中

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_動作開始_Run()
        {
            SetOutBit(outBit_換刀區安全門閉鎖, true);
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_換刀區安全門到位_Run()
        {
            if (IsSimulation() != 0)
            {
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (T_Auto_Z1SPControl.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                SetOutBit(outBit_換刀區安全門閉鎖, false);

                if (GetInBit(inBit_換刀區安全門解鎖檢知_Z1區) || GetInBit(inBit_換刀區安全門解鎖檢知_Z2區))
                {
                    ShowAlarm("E", 50);
                    return FlowChart.FCRESULT.CASE1;
                }
            }
            else
            {
                if (GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) && GetInBit(inBit_換刀區安全門鎖定檢知_Z2區) && bCutDoorSafe)
                {
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_冰水機啟動_Run()
        {
            SetOutBit(outBit_冰水機啟動, true);

            if (GetInBit(inBit_冰水機正常運轉中) && GetInBit(inBit_Z1主軸冷卻水流量偵測))
            {
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (T_Auto_Z1SPControl.On(SReadValue("主軸動作時間").ToInt()))
            {
                if (!GetInBit(inBit_冰水機正常運轉中))
                {
                    ShowAlarm("E", 38);//主軸控制Z1：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return FlowChart.FCRESULT.CASE1;
                }
                if (!GetInBit(inBit_Z1主軸冷卻水流量偵測))
                {
                    ShowAlarm("E", 39);//主軸控制Z1：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_主軸解鎖_Run()
        {
            SetOutBit(outBit_主軸軸承鎖定, false);

            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_開啟Switch_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z1SpindleStop)//考慮啟動中要停止
                    {
                        Z1SpindleStop = false;
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z1SpindleControlType != 0)
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (Z1SpindleProtection() == 0)
                    {
                        SetOutBit(outBit_Z1_SWITCH, true);
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z1SPControl.On(5000))
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;
                default:// Simulation
                    SetOutBit(outBit_Z1_SWITCH, true);
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_開啟Rest與Enabled_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z1SpindleStop)//考慮啟動中要停止
                    {
                        Z1SpindleStop = false;
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z1SpindleControlType != 0)
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (Z1SpindleProtection() == 0)
                    {
                        SetOutBit(outBit_Z1_REST_ENABLED, true);
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z1SPControl.On(5000))
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;

                default:// Simulation
                    SetOutBit(outBit_Z1_REST_ENABLED, true);
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_設定轉速值_Run()
        {
            if (Z1SpindleStop)//考慮啟動中要停止
            {
                Z1SpindleStop = false;
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            int nRotateSpd;
            if (PackageName.Length != 0)
            {
                nRotateSpd = PReadValue("Spndle_rpm").ToInt();
            }
            else
            {
                nRotateSpd = Convert.ToInt32(textBox_主軸轉速Z1.Text);//toro->記得補模組設定的欄位
            }

            if (nRotateSpd < 7500)
            {
                ShowAlarm("E", 49);//主軸控制Z1：轉速設定勿低於7500，請檢查
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            switch (Z1SpindleControlType)
            {
                case 1://元件IO控制
                case 2://元件COM控制
                    if (Z1Spindle.StartRun(nRotateSpd) == eTaskRet.eCurrentTaskDone)
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z1SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        ShowAlarm("E", 54);//主軸控制Z1：通訊控制逾時，請檢查
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else
                    {
                        return FlowChart.FCRESULT.IDLE;
                    }

                default://IO控制
                    analogOut_Z1主軸轉速類比控制.Value = (float)nRotateSpd / 6000f;
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_等待轉速到達_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z1SpindleStop)//考慮啟動中要停止
                    {
                        Z1SpindleStop = false;
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z1SpindleProtection() != 0)//啟動中有異常
                    {
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (inBit_Z1主軸達設定值.Value && !inBit_Z1主軸未運轉.Value)
                    {
                        ShowAlarm("I", 1, "Z1");//主軸控制Z1：運轉完成
                        m_enZ1SpindleStatus = enSpindleStatus.Run;
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z1SPControl.On(iSpindle相關訊號TimeOut時間))//啟動中逾時
                    {
                        if (!inBit_Z1主軸達設定值.Value)
                        {
                            ShowAlarm("E", 55);//主軸控制Z1：啟動過程中，等待轉速到達逾時或訊號異常，請檢查
                            T_Auto_Z1SPControl.Restart();
                        }
                        if (inBit_Z1主軸未運轉.Value)
                        {
                            ShowAlarm("E", 56);//主軸控制Z1：啟動過程中，主軸未運轉或訊號異常，請檢查
                            T_Auto_Z1SPControl.Restart();
                        }
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;

                default:// Simulation
                    ShowAlarm("I", 1, "Z1");//主軸控制Z1：運轉完成
                    m_enZ1SpindleStatus = enSpindleStatus.Run;
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_啟動失敗_Run()
        {
            m_enZ1SpindleStatus = enSpindleStatus.Closing;
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_動作結束_Run()
        {
            if (T_Auto_Z1SPControl.On(1000))
            {
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_啟動中_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return m_enZ1SpindleStatus == enSpindleStatus.Closing ? FlowChart.FCRESULT.CASE1 : FlowChart.FCRESULT.NEXT;
        }

        #endregion

        private FlowChart.FCRESULT FC_Z1主軸控制_等待停止訊號_Run()
        {
            int SpindleState = Z1SpindleProtection();
            if (SpindleState != 0) // 運轉中有異常
            {
                if (SpindleState >= -5) // -1 ~ -5 主軸的水、電、氣異常
                {
                    Z1SpindleStop = true;
                }
                else // -6, -7 破刀
                {
                    if (!m_bIsPauseNeedToContinue)
                    {
                        Z1SpindleStop = true;
                    }
                }
            }

            // 無異常等待主軸被停止訊號
            if (ScanSpindleInfo(enSpindleSel.Z1))
            {
                if (Z1SpindleStop)
                {
                    Z1SpindleStop = false;
                    m_enZ1SpindleStatus = enSpindleStatus.Closing;
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }

                #region By Wolf 20210816, v4.0.1.91, 修改為當發現參考轉速與產品設定轉速不同時進入速度改變修改的flowchart
                int nRotateSpd = PReadValue("Spndle_rpm").ToInt();
                if (Convert.ToInt32(Z1RefSpeed) != nRotateSpd && PackageName.Length != 0)
                {
                    // 速度改變時
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                #endregion
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 停止中

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_動作開始_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_初始設定_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_關閉測高功能_Run()
        {
            if (GetSparkOff())//關閉接觸測高:toro->需補上關閉接觸測高及非接觸測高流程
            {
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (T_Auto_Z1SPControl.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT; //用NEXT是因為就算有alarm也要繼續跑然後去停主軸
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_Z1移至安全位_Run()
        {
            if (motor_切刀上下馬達_Z1.G00(SReadValue("安全點位_Z1").ToInt()))
            {
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (T_Auto_Z1SPControl.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    MotorOverTime(motor_切刀上下馬達_Z1.Text);//主軸控制：Z1軸至安全位超時
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT; //用NEXT是因為就算有alarm也要繼續跑然後去停主軸
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_模組初始_Run()
        {
            switch (Z1SpindleControlType)
            {
                case 1:
                case 2:
                    Z1Spindle.StopRun();
                    break;
                default:
                    analogOut_Z1主軸轉速類比控制.Value = 0;
                    SetOutBit(outBit_Z1_REST_ENABLED, false);
                    SetOutBit(outBit_Z1_SWITCH, false);
                    break;
            }
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_等待完全停止_Run()
        {
            Z1Spindle.StopRun();
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (inBit_Z1主軸達設定值.Value && inBit_Z1主軸未運轉.Value)
                    {
                        Z1SpindleStart = false;
                        ShowAlarm("I", 2, "Z1");//主軸控制Z1：停止完成
                        m_enZ1SpindleStatus = enSpindleStatus.Stop;//主軸已停止
                        T_Auto_Z1SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z1SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        if (!inBit_Z1主軸達設定值.Value)
                        {
                            ShowAlarm("E", 57);//主軸控制Z1：停止過程中，等待轉速到達逾時或訊號異常，請檢查
                            T_Auto_Z1SPControl.Restart();
                        }
                        if (!inBit_Z1主軸未運轉.Value)
                        {
                            ShowAlarm("E", 64);//主軸控制Z1：停止過程中，主軸尚在運轉或訊號異常，請檢查
                            T_Auto_Z1SPControl.Restart();
                        }
                    }
                    break;

                default:// Simulation
                    Z1SpindleStart = false;
                    ShowAlarm("I", 2, "Z1");//主軸控制Z1：停止完成
                    m_enZ1SpindleStatus = enSpindleStatus.Stop;//主軸已停止
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_動作結束_Run()
        {
            T_Auto_Z1SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z1主軸控制_停止中_Run()
        {
            if (T_Auto_Z1SPControl.On(5000))
            {
                //AutoFlowStep++;
                //Z1SpindleStart = true;
                T_Auto_Z1SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #endregion

        private FlowChart.FCRESULT FC_Z1主軸控制_修改轉速_Run()
        {
            int nRotateSpd = PReadValue("Spndle_rpm").ToInt();

            if (Z1Spindle.ChangeSpindleSpeed(nRotateSpd) == eTaskRet.eCurrentTaskDone && T_Auto_Z1SPControl.On(2000))
            {
                if (inBit_Z1主軸達設定值.Value)
                {
                    Z1SpindleStart = false;
                    T_Auto_Z1SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (T_Auto_Z1SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        #region By Wolf 20210816, v4.0.1.91, 新增 "主軸控制Z1：修改轉速時，等待轉速到達逾時或訊號異常，請檢查[Z1主軸達設定值]訊號"
                        ShowAlarm("E", 99);
                        T_Auto_Z1SPControl.Restart();
                        #endregion
                    }
                }
            }

            return default(FlowChart.FCRESULT);
        }

        #endregion

        #region Z2主軸控制
        private FlowChart.FCRESULT FC_Z2主軸控制_動作開始_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_初始設定_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_等待啟動訊號_Run()
        {
            if (ScanSpindleInfo(enSpindleSel.Z2))
            {
                if (Z2SpindleStart)
                {
                    Z2SpindleStart = false;
                    m_enZ2SpindleStatus = enSpindleStatus.Starting;
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 啟動中

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return m_enZ2SpindleStatus == enSpindleStatus.Closing ? FlowChart.FCRESULT.CASE1 : FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_動作開始_Run()
        {
            SetOutBit(outBit_換刀區安全門閉鎖, true);
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_換刀區安全門到位_Run()
        {
            if (IsSimulation() != 0)
            {
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }

            if (T_Auto_Z2SPControl.On(SReadValue("汽缸逾時時間").ToInt()))
            {
                SetOutBit(outBit_換刀區安全門閉鎖, false);

                if (GetInBit(inBit_換刀區安全門解鎖檢知_Z1區) || GetInBit(inBit_換刀區安全門解鎖檢知_Z2區))
                {
                    ShowAlarm("E", 50);
                    return FlowChart.FCRESULT.CASE1;
                }
            }
            else
            {
                if (GetInBit(inBit_換刀區安全門鎖定檢知_Z1區) && GetInBit(inBit_換刀區安全門鎖定檢知_Z2區) && bCutDoorSafe)
                {
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_冰水機啟動_Run()
        {
            SetOutBit(outBit_冰水機啟動, true);
            if (GetInBit(inBit_冰水機正常運轉中) && GetInBit(inBit_Z2主軸冷卻水流量偵測))
            {
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (T_Auto_Z2SPControl.On(SReadValue("主軸動作時間").ToInt()))
            {
                if (!GetInBit(inBit_冰水機正常運轉中))
                {
                    ShowAlarm("E", 44);//主軸控制Z1：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return FlowChart.FCRESULT.CASE1;
                }
                if (!GetInBit(inBit_Z2主軸冷卻水流量偵測))
                {
                    ShowAlarm("E", 45);//主軸控制Z1：啟動中，冰水機運轉異常或訊號異常，請檢查
                    return FlowChart.FCRESULT.CASE1;
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_主軸解鎖_Run()
        {
            SetOutBit(outBit_主軸軸承鎖定, false);
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_開啟Switch_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z2SpindleStop)//考慮啟動中要停止
                    {
                        Z2SpindleStop = false;
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z2SpindleControlType != 0)
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (Z2SpindleProtection() == 0)
                    {
                        SetOutBit(outBit_Z2_SWITCH, true);
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z2SPControl.On(5000))
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;
                default:// Simulation
                    SetOutBit(outBit_Z2_SWITCH, true);
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_開啟Rest與Enabled_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z2SpindleStop)//考慮啟動中要停止
                    {
                        Z2SpindleStop = false;
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z2SpindleControlType != 0)
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (Z2SpindleProtection() == 0)
                    {
                        SetOutBit(outBit_Z2_REST_ENABLED, true);
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z2SPControl.On(5000))
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;
                default:// Simulation
                    SetOutBit(outBit_Z2_REST_ENABLED, true);
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_設定轉速值_Run()
        {
            if (Z2SpindleStop)//考慮啟動中要停止
            {
                Z2SpindleStop = false;
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            int nRotateSpd;
            if (PackageName.Length != 0)
            {
                nRotateSpd = PReadValue("Spndle_rpm").ToInt();
            }
            else
            {
                nRotateSpd = Convert.ToInt32(textBox_主軸轉速Z2.Text);//toro->記得補模組設定的欄位
            }

            if (nRotateSpd < 7500)
            {
                ShowAlarm("E", 65);//主軸控制Z2：轉速設定勿低於7500，請檢查
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.CASE1;
            }

            switch (Z2SpindleControlType)
            {
                case 1://元件IO控制
                case 2://元件COM控制
                    if (Z2Spindle.StartRun(nRotateSpd) == eTaskRet.eCurrentTaskDone)
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z2SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        ShowAlarm("E", 66);//主軸控制Z2：通訊控制逾時，請檢查
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else
                    {
                        return FlowChart.FCRESULT.IDLE;
                    }
                default://IO控制
                    analogOut_Z2主軸轉速類比控制.Value = (float)nRotateSpd / 6000f;
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_等待轉速到達_Run()
        {
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (Z2SpindleStop)//考慮啟動中要停止
                    {
                        Z2SpindleStop = false;
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (Z2SpindleProtection() != 0)//啟動中有異常
                    {
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.CASE1;
                    }
                    else if (inBit_Z2主軸達設定值.Value && !inBit_Z2主軸未運轉.Value)
                    {
                        ShowAlarm("I", 1, "Z2");//主軸控制Z2：運轉完成
                        m_enZ2SpindleStatus = enSpindleStatus.Run;
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z2SPControl.On(iSpindle相關訊號TimeOut時間))//啟動中逾時
                    {
                        if (!inBit_Z2主軸達設定值.Value)
                        {
                            ShowAlarm("E", 67);//主軸控制Z2：啟動過程中，等待轉速到達逾時或訊號異常，請檢查
                            T_Auto_Z2SPControl.Restart();
                        }
                        if (inBit_Z2主軸未運轉.Value)
                        {
                            ShowAlarm("E", 68);//主軸控制Z2：啟動過程中，主軸未運轉或訊號異常，請檢查
                            T_Auto_Z2SPControl.Restart();
                        }
                        return FlowChart.FCRESULT.CASE1;
                    }
                    break;
                default:// Simulation
                    ShowAlarm("I", 1, "Z2");//主軸控制Z2：運轉完成
                    m_enZ2SpindleStatus = enSpindleStatus.Run;
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_動作結束_Run()
        {
            if (T_Auto_Z2SPControl.On(1000))
            {
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_啟動中_啟動失敗_Run()
        {
            m_enZ2SpindleStatus = enSpindleStatus.Closing;
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        #endregion

        private FlowChart.FCRESULT FC_Z2主軸控制_等待停止訊號_Run()
        {
            int SpindleState = Z2SpindleProtection();
            if (SpindleState != 0) // 運轉中有異常
            {
                if (SpindleState >= -5) // -1 ~ -5 主軸的水、電、氣異常
                {
                    Z2SpindleStop = true;
                }
                else // -6, -7 破刀
                {
                    if (!m_bIsPauseNeedToContinue)
                    {
                        Z2SpindleStop = true;
                    }
                }
            }

            // 無異常等待主軸被停止訊號
            if (ScanSpindleInfo(enSpindleSel.Z2))
            {
                if (Z2SpindleStop)
                {
                    Z2SpindleStop = false;
                    m_enZ2SpindleStatus = enSpindleStatus.Closing;
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }

                #region By Wolf 20210816, v4.0.1.91, 修改為當發現參考轉速與產品設定轉速不同時進入速度改變修改的flowchart
                int nRotateSpd = PReadValue("Spndle_rpm").ToInt();
                if (Convert.ToInt32(Z2RefSpeed) != nRotateSpd && PackageName.Length != 0)
                {
                    // 速度改變時
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.CASE1;
                }
                #endregion
            }

            return FlowChart.FCRESULT.IDLE;
        }

        #region 停止中

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_動作開始_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_初始設定_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_關閉測高功能_Run()
        {
            if (GetSparkOff())//關閉接觸測高:toro->需補上關閉接觸測高及非接觸測高流程
            {
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else if (T_Auto_Z2SPControl.On(2 * SReadValue("iChkSparkDelay").ToInt()))
            {
                ShowAlarm("E", 18);
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT; //用NEXT是因為就算有alarm也要繼續跑然後去停主軸
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_Z2移至安全位_Run()
        {
            if (motor_切刀上下馬達_Z2.G00(SReadValue("安全點位_Z2").ToInt()))
            {
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            else
            {
                if (T_Auto_Z2SPControl.On(SReadValue("馬達逾時時間").ToInt()))
                {
                    MotorOverTime(motor_切刀上下馬達_Z2.Text);//主軸控制：Z2軸至安全位超時
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT; //用NEXT是因為就算有alarm也要繼續跑然後去停主軸
                }
            }

            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_模組初始_Run()
        {
            switch (Z2SpindleControlType)
            {
                case 1:
                case 2:
                    Z2Spindle.StopRun();
                    break;
                default:
                    analogOut_Z2主軸轉速類比控制.Value = 0;
                    SetOutBit(outBit_Z2_REST_ENABLED, false);
                    SetOutBit(outBit_Z2_SWITCH, false);
                    break;
            }
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_等待完全停止_Run()
        {
            Z2Spindle.StopRun();
            switch (IsSimulation())
            {
                case 0://正常模式
                    if (inBit_Z2主軸達設定值.Value && inBit_Z2主軸未運轉.Value)
                    {
                        Z2SpindleStart = false;
                        ShowAlarm("I", 2, "Z2");//主軸控制Z2：停止完成
                        m_enZ2SpindleStatus = enSpindleStatus.Stop;//主軸已停止
                        T_Auto_Z2SPControl.Restart();
                        return FlowChart.FCRESULT.NEXT;
                    }
                    else if (T_Auto_Z2SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        if (!inBit_Z2主軸達設定值.Value)
                        {
                            ShowAlarm("E", 69);//主軸控制Z2：停止過程中，等待轉速到達逾時或訊號異常，請檢查
                            T_Auto_Z2SPControl.Restart();
                        }
                        if (!inBit_Z2主軸未運轉.Value)
                        {
                            ShowAlarm("E", 74);//主軸控制Z2：停止過程中，主軸尚在運轉或訊號異常，請檢查
                            T_Auto_Z2SPControl.Restart();
                        }
                    }
                    break;
                default:// Simulation
                    Z2SpindleStart = false;
                    ShowAlarm("I", 2, "Z2");//主軸控制Z2：停止完成
                    m_enZ2SpindleStatus = enSpindleStatus.Stop;//主軸已停止
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_動作結束_Run()
        {
            T_Auto_Z2SPControl.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_Z2主軸控制_停止中_Run()
        {

            if (T_Auto_Z2SPControl.On(5000))
            {
                //AutoFlowStep++;
                //Z2SpindleStart = true;
                T_Auto_Z2SPControl.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;

        }

        #endregion

        private FlowChart.FCRESULT FC_Z2主軸控制_修改轉速_Run()
        {
            int nRotateSpd = PReadValue("Spndle_rpm").ToInt();

            if (Z2Spindle.ChangeSpindleSpeed(nRotateSpd) == eTaskRet.eCurrentTaskDone && T_Auto_Z2SPControl.On(2000))
            {
                if (inBit_Z2主軸達設定值.Value)
                {
                    Z2SpindleStart = false;
                    T_Auto_Z2SPControl.Restart();
                    return FlowChart.FCRESULT.NEXT;
                }
                else
                {
                    if (T_Auto_Z2SPControl.On(iSpindle相關訊號TimeOut時間))
                    {
                        #region By Wolf 20210816, v4.0.1.91, 新增 "主軸控制Z2：修改轉速時，等待轉速到達逾時或訊號異常，請檢查[Z2主軸達設定值]訊號"
                        ShowAlarm("E", 100); 
                        T_Auto_Z2SPControl.Restart();
                        #endregion
                    }
                }
            }

            return default(FlowChart.FCRESULT);
        }

        #endregion

        #endregion 主軸啟動/關閉

        private void button9_Click(object sender, EventArgs e)
        {
            SetSpeed_Z1_X(enSpeedMode.切割速度);
            SetSpeed_Z2_X(enSpeedMode.切割速度);

            //motor_切刀橫移馬達_Z2X.G00(50000);
            //motor_切刀橫移馬達_Z1X.G00(-50000);

            int nPos2 = motor_切刀橫移馬達_Z2X.ReadPos();
            int nPos1 = motor_切刀橫移馬達_Z1X.ReadPos();

            var dicMotorPosPairs = new Dictionary<Motor, int>()
            {
                {motor_切刀橫移馬達_Z2X, nPos2 + 50000},
                {motor_切刀橫移馬達_Z1X, nPos1 + 50000}
            };

            MultiMotorsG00(dicMotorPosPairs, Timer_Auto);
        }

        public void SetIngonreSafeDoor(bool bSet)
        {
            SSetValue("bIgnoreCutSafeDoor", bSet);

        }

        private FlowChart.FCRESULT FC_初始化_X軸移到安全位_Run()
        {
            var dicMotorPosPairs = new Dictionary<Motor, int>()
                {
                    {motor_切割平台前後馬達Y, SReadValue("基準入料點位_Y").ToInt()},
                    {motor_切刀橫移馬達_Z1X, SReadValue("安全點位_X1").ToInt()},
                    {motor_切刀橫移馬達_Z2X, SReadValue("安全點位_X2").ToInt()}
                };

            if (MultiMotorsG00(dicMotorPosPairs, Timer_ActionIni)) return FlowChart.FCRESULT.NEXT;

            return FlowChart.FCRESULT.IDLE;
        }
    }
}
