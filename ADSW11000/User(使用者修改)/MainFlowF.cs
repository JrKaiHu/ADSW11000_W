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
using KCSDK;
using CommonObj;
#region copy用
#endregion copy用
namespace ADSW11000
{   //使用者修改 (主控流程)
    public partial class MainFlowF : Form
    {
#region 基本

        #region 基本_使用者修改 (定義各模組指標，方便使用)

        public BaseModuleInterface mMAA;
        public BaseModuleInterface mSaw;

        #endregion 基本_使用者修改 (定義各模組指標，方便使用)

#endregion 基本

#region 動作函數

        #region 動作函數_初始化函數

        public void Initial()
        {
        }

        #endregion 動作函數_初始化函數

        #region 動作函數_持續掃描

        public void AlwaysRun()
        {
        }

        #endregion 動作函數_持續掃描

        #region 動作函數_掃描硬體按鈕IO

        public void ScanIO()
        {
            #region 動作函數_掃描硬體按鈕IO_架構使用 (處理MAA的硬體按鈕IO對應的動作)

            if ((SYSPara.ManualControlIO) && (SYSPara.RunMode != RunModeDT.MANUAL))
                return;

            int result = ((BaseModuleInterface)mMAA).ScanIO();

            //Start Button
            if ((result & 0x01) == 1)
            {
                if (!SYSPara.SysRun)
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「生產開始」");

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

                SYSPara.ErrorStop = false;
                SYSPara.Alarm.ClearAll();
                SYSPara.MusicOn = true;
                SYSPara.SysRun = true;
            }
            //Stop Button
            if (((result >> 1) & 0x01) == 1)
            {
                if (SYSPara.SysRun)
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「生產暫停」");

                if (SYSPara.SysState == StateMode.SM_ALARM)
                {
                    SYSPara.ErrorStop = false;
                    SYSPara.SysState = StateMode.SM_PAUSE;
                }

                SYSPara.MusicOn = false;
                SYSPara.SysRun = false;
                FormSet.mMSS.M_Stop();
            }
            //Alarm Result Button
            if (((result >> 2) & 0x01) == 1)
            {
                SYSPara.Alarm.ClearAll();
                if (SYSPara.SysState == StateMode.SM_ALARM || SYSPara.SysState == StateMode.SM_PAUSE)
                {
                    SYSPara.ErrorStop = false;
                    SYSPara.SysState = StateMode.SM_PAUSE;
                }

                SYSPara.MusicOn = false;

            }

            #endregion 動作函數_掃描硬體按鈕IO_架構使用 (處理MAA的硬體按鈕IO對應的動作)
        }

        #endregion 動作函數_掃描硬體按鈕IO

        #region 動作函數_歸零前的重置

        public void HomeReset()
        {
            #region 動作函數_歸零前的重置_使用者修改 (針對各模式的歸零前置設定)

            MainflowHomeReset = true;
            FC_MF歸零_動作開始.TaskReset();

            #endregion 動作函數_歸零前的重置_使用者修改 (針對各模式的歸零前置設定)

            FormSet.mMainF3.CalKufBreakFuncConst();

            mHomeOk = false;
        }

        #endregion 動作函數_歸零前的重置

        #region 動作函數_歸零

        public bool Home()
        {
            FC_MF歸零_動作開始.MainRun();
            return mHomeOk;
        }

        #endregion 動作函數_歸零

        #region 動作函數_運轉前初始化

        public void RunReset()
        {

        }

        #endregion 動作函數_運轉前初始化

        #region 動作函數_運轉

        public void Run()
        {   //是否進行結批流程
            if (SYSPara.Lotend)
            {   //設定MainFlow模組結批完成
                SYSPara.LotendOk = true;
            }
        }

        #endregion 動作函數_運轉

        #region 動作函數_手動運行前置設定

        public void ManualReset()
        {
        }

        #endregion 動作函數_手動運行前置設定

        #region 動作函數_手動模式運行

        public void ManualRun()
        {
        }

        #endregion 動作函數_手動模式運行

#endregion 動作函數

#region 公用函數

        #region 公用函數_v0.0.7.11 By Sanxiu 空跑模式修正，自動設定依切割道作業

        public void ShowDryMode()
        {
            if (SYSPara.OReadValue("DryRun").ToBoolean())
            {
                SYSPara.ShowAlarm("I", 1104);
            }
        }

        #endregion 公用函數_v0.0.7.11 By Sanxiu 空跑模式修正，自動設定依切割道作業

        #region 公用函數_FC執行紀錄

        public string GetNowMoudleTask()
        {
            return "MF:1." + FC_MF歸零_動作開始.NowTask.Text;
        }

        #endregion 公用函數_FC執行紀錄

        #region 公用函數_基本

        public List<FlowChart> FlowChartList = new List<FlowChart>();
        public bool mHomeOk;
        public bool MainflowHomeReset = false;

        public MainFlowF()
        {
            InitializeComponent();

            TopLevel = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            //找出所有的主控FlowChart
            GetControls(this);

            #region 公用函數_使用者修改 (定義各模組指標，方便使用)

            mMAA = (BaseModuleInterface)FormSet.mMSS.GetModule("MAA");
            mSaw = (BaseModuleInterface)FormSet.mMSS.GetModule("Saw");

            #endregion 公用函數_使用者修改 (定義各模組指標，方便使用)
        }

        #endregion 公用函數_基本

#endregion 公用函數

#region 私有函數

        #region 私有函數_歸零FC相關

        private MyTimer T_Home = new MyTimer(true);

        #endregion 私有函數_歸零FC相關

        #region 私有函數_基本

        private void GetControls(Control ctr)
        {
            foreach (Control myctr in ctr.Controls)
            {   //如果傳進來的控制項有子控制項的話
                if (myctr.HasChildren == true)
                {   //就遞迴呼叫自己
                    GetControls(myctr);
                }

                if (myctr is FlowChart)
                {
                    FlowChartList.Add((FlowChart)myctr);
                }
            }
        }

        #endregion 私有函數_基本

#endregion 私有函數

#region FC

        #region FC_歸零流程

        private FlowChart.FCRESULT FC_MF歸零_動作開始_Run()
        {
            T_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_MF歸零_SAW歸零_Run()
        {
            mSaw.SetCanHome();
            T_Home.Restart();
            return FlowChart.FCRESULT.NEXT;
        }

        private FlowChart.FCRESULT FC_MF歸零_等待SAW歸零完成_Run()
        {
            bool r1 = mSaw.mHomeOk;
            if (r1)
            {
                T_Home.Restart();
                return FlowChart.FCRESULT.NEXT;
            }
            return FlowChart.FCRESULT.IDLE;
        }

        private FlowChart.FCRESULT FC_MF歸零_動作結束_Run()
        {
            mHomeOk = true;
            T_Home.Restart();
            return FlowChart.FCRESULT.IDLE;
        }

        #endregion FC_歸零流程

#endregion FC

    }
}
