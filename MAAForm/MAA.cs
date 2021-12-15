using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVIFM;
using ProVLib;
using CommonObj;
using System.Diagnostics;
using System.Runtime.InteropServices;
#region copy用
#endregion copy用
namespace MAA
{
    public partial class MAA : BaseModuleInterface
    {
#region 基本

        public MAA()
        {
            InitializeComponent();
            CreateComponentList();
        }

#endregion 基本
        //Record Update Kerf Data Delegate
        public SafeDoorDelegateActivated FPtrSafeDoorActivtedDelegate = null;
        public void RecordSafeDoorActivtedDelegate(SafeDoorDelegateActivated ptr)
        {
            FPtrSafeDoorActivtedDelegate = ptr;
        }

        #region 繼承函數

        //模組解構使用
        public override void DisposeTH()
        {
            base.DisposeTH();
        }

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

            #endregion
        }
        //持續偵測函數
        //持續掃描
        public override void AlwaysRun()
        {
            if (IsSimulation() != 0) return;

            //偵測空壓
            CheckInput();

            //B接
            //檢查緊停
            ChackEMG();

            //B接
            //檢查安全門
            outBit_SafeDoorNotify.Value = SafeDoor_Front.Value;//v4.0.1.21
            if (!SReadValue("安全門不偵測").ToBoolean())
                CheckSafeDoor();
        }

        //掃描硬體按鈕IO
        //bit0=StartButton / Bit1=StopButton / Bit2=AlarmResetButton 
        bool bArmRstIsSet = false;
        bool bStartIsSet = false;
        public override int ScanIO()
        {
            OB_StartButton.Value = SysRun;
            OB_StopButton.Value = !SysRun;
            OB_AlarmResetButton.Value = IB_AlarmResetButton.Value;

            byte result = 0;
            if (IB_StartButton.Value)
                bStartIsSet = true;
            if (!IB_StartButton.Value && bStartIsSet) //放開時再設定Start Bit
            {
                bStartIsSet = false;
                result |= 0x01;
            }

            if (IB_StopButton.Value)
                result |= 0x02;

            if (IB_AlarmResetButton.Value)
                bArmRstIsSet = true;
            if (!IB_AlarmResetButton.Value && bArmRstIsSet) //放開時再設定Alarm Reset Bit
            {
                bArmRstIsSet = false;
                result |= 0x04;
            }
            return result;
        }

        //歸零初始
        public override void HomeReset()
        {
            mLotendOk = true;
            mHomeOk = true;

            SSetValue("安全門不偵測", false);
            SaveFile();
        }

        //歸零
        public override bool Home() 
        {
            mHomeOk = true;
            return mHomeOk; 
        } 

        //手動相關函數
        //手動運行前置設定
        public override void ManualReset() 
        { 
        }

        //手動模式運行
        public override void ManualRun() 
        { 
        } 

        ////結批相關函數
        ////結批運行
        //public override void Lotend() 
        //{ 
        //} 

        //停止所有馬達
        public override void StopMotor()
        {
            base.StopMotor();
        }

        #endregion

        #region 私有函數

        //安全門檢查
        private bool CheckSafeDoor()
        {
            List<InBit> SafeFoorSrLst = new List<InBit>();
            SafeFoorSrLst.Add(SafeDoor_Front);
            if (SReadValue("bIgnoreCutSafeDoor").ToBoolean()) SafeFoorSrLst.Add(SafeDoor_Back);
            
            bool err = false;
            foreach (var sensor in SafeFoorSrLst)
            {
                if (sensor.Enabled)
                {
                    if (sensor.Off(10))
                    {
                        err = true;
                        ShowAlarm("E", 0, sensor.Text);
                    }
                }
            }
            return err;
        }

        //緊停按鈕檢查
        private bool ChackEMG()
        {
            InBit[] EMG = {EMGA_Front, EMGA_Back};
            bool err = false;
            for (int i = 0; i < EMG.Count(); i++)
            {          
                if (EMG[i].Enabled && EMG[i].Off(10))
                {
                    if (FPtrSafeDoorActivtedDelegate != null)
                    {
                        FPtrSafeDoorActivtedDelegate(); //通知主程式換刀安全門作動了
                    }
                    err = true;
                    ShowAlarm("E", 1, EMG[i].Text);
                }
            }
            return err;
        }

        private void CheckInput()
        {
            //持續偵測空壓
            if (SReadValue("空壓偵測").ToBoolean())
            {
                double dAirPressureNow = GetMainAIValue();
                double dAirPressureMax = SReadValue("Main_AirPressure_Max").ToDouble();
                double dAirPressureMin = SReadValue("Main_AirPressure_Min").ToDouble();

                if (dAirPressureMin >= dAirPressureMax) 
                {
                    ShowAlarm("E", 3);
                }

                if (dAirPressureNow > dAirPressureMax || dAirPressureNow < dAirPressureMin)
                {
                    ShowAlarm("W", 2, analogIn_入力空壓偵測.Text);
                }
            }
        }
        #endregion

        #region 公有函數
        //取得雙螢幕控制按鈕訊號 kent
        public bool GetFrontScreen()
        {
            return IB_FrontScreen.Value;
        }

        public bool GetBackScreen()
        {
            return IB_BackScreen.Value;
        }

        public void SetFrontScreen(bool bOn)
        {
            OB_FrontScreen.Value = bOn;
        }

        public void SetBackScreen(bool bOn)
        {
            OB_BackScreen.Value = bOn;
        }
        //取得紅燈訊號
        public bool ScanRedLight()
        {
            return RedLight.Value;
        }

        //取得黃燈訊號
        public bool ScanYellowLight()
        {
            return YellowLight.Value;
        }

        //取得綠燈訊號
        public bool ScanGreenLight()
        {
            return GreenLight.Value;
        }

        //回傳燈號IO
        public List<object> GetLightIO()
        {
            List<object> lightio = new List<object>();
            lightio.Add(GreenLight);
            lightio.Add(YellowLight);
            lightio.Add(RedLight);
            lightio.Add(Music1);
            lightio.Add(Music2);
            lightio.Add(Music3);
            lightio.Add(Music4);
            return lightio;
        }

        // 前安全門是否打開
        public bool IsFrontDoorOpen()
        {
            return SafeDoor_Front.Value;
        }

        // 後安全門是否打開
        public bool IsBackDoorOpen()
        {
            return SafeDoor_Back.Value;
        }

        [DllImport("user32.dll")]
        static extern bool BlockInput(bool fBlockIt);
       
        bool bBtnF = false;   //按鈕F
        bool bBtnB = false;   //按鈕B

        bool bBtnF_Signal = false;   //信號是否亮起
        bool bBtnB_Signal = false;   //信號是否亮起
        // v4.0.1.32 雙螢幕設定，選擇正確devcon.exe，呼叫字串修改
        public void InputLock()
        {
            bBtnF = IB_FrontScreen.Value;
            bBtnB = IB_BackScreen.Value;

            Process p = new Process();
            p.StartInfo.FileName = @"C:\DevManView.exe";

            if (bBtnF)
            {
                if (bBtnF_Signal == false)
                {
                    bBtnF_Signal = true;
                    OB_FrontScreen.Value = bBtnF_Signal;
                    p.StartInfo.Arguments = @"/disable " + SReadValue("sHID_TeachB").ToString();                    
                    p.Start();
                    p.WaitForExit();
                }
            }
            else
            {
                if (bBtnF_Signal == true)
                {
                    bBtnF_Signal = false;
                    OB_FrontScreen.Value = bBtnF_Signal;
                    p.StartInfo.Arguments = @"/enable " + SReadValue("sHID_TeachB").ToString();
                    p.Start();
                    p.WaitForExit();
                }
            }

            if (bBtnB)
            {
                if (bBtnB_Signal == false)
                {
                    bBtnB_Signal = true;
                    OB_BackScreen.Value = bBtnB_Signal;
                    p.StartInfo.Arguments = @"/disable " + SReadValue("sHID_TeachF").ToString();
                    p.Start();
                    p.WaitForExit();
                    
                }
            }
            else
            {
                if (bBtnB_Signal == true)
                {
                    bBtnB_Signal = false;
                    OB_BackScreen.Value = bBtnB_Signal;
                    p.StartInfo.Arguments = @"/enable " + SReadValue("sHID_TeachF").ToString();
                    p.Start();
                    p.WaitForExit();
                }
            }
        }

        /// <summary>
        /// //v0.0.7.12 By Sanxiu 雙螢幕鎖定
        /// 所有輸入設備解鎖
        /// </summary>
        public void AllUnlockHID()
        {
            BlockInput(false);

            Process p = new Process();
            p.StartInfo.FileName = @"C:\DevManView.exe";

            p.StartInfo.Arguments = @"/enable " + SReadValue("sHID_TeachB").ToString();
            p.StartInfo.Arguments = @"/enable " + SReadValue("sHID_TeachF").ToString();
            p.Start();
            p.WaitForExit();

            System.Threading.Thread.Sleep(500);

            p.Close();
        }

        /// <summary>
        /// //v0.0.7.12 By Sanxiu 手動模式加入安全門保護
        /// 秀出安全門警報
        /// </summary>
        /// <param name="iNo">安全門編號</param>
        public void ShowSafeDoorErr(int iNo)
        {
            ShowAlarm("E", 0, iNo);
        }

        #endregion

        #region 閥體相關介面函式
        //取得閥體數據
        public double GetMainAIValue()
        {
            double gap = (SReadValue("Main_Volt_PressOn").ToDouble() - SReadValue("Main_Volt_PressOff").ToDouble());
            if (gap == 0)
                return 0;
            double rate = SReadValue("Main_KPa_PressOn").ToDouble() / gap;
            return (analogIn_入力空壓偵測.Value - SReadValue("Main_Volt_PressOff").ToDouble()) * rate;
        }

        public double GetMainAirLimit(int nSel)
        {
            string str = nSel == 0 ? "Main_AirPressure_Min" : "Main_AirPressure_Max";
            return SReadValue(str).ToDouble();
        }

        //private void ShowIOPart()
        //{
        //    Music2.Visible = false;
        //    Music3.Visible = false;
        //    Music4.Visible = false;
        //}

        //開日光燈
        public void OpenLight()
        {
            outBit_日光燈.Value = !outBit_日光燈.Value;
        }
        #endregion

        private void tpSuperSetting_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                groupBox7.Visible = !groupBox7.Visible;
            }
        }
    }
}

