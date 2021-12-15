using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProVIFM;

namespace ADSW11000
{
    public partial class InitialForm : Form
    {
        #region 使用者修改 (定義各模組指標)

        //private BaseModuleInterface mTray = null;

        #endregion

        public InitialForm()
        {
            InitializeComponent();
            TopLevel = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            #region 使用者修改 (取得各模組指標)

            //mTray = (BaseModuleInterface)FormSet.mMSS.GetModule("Tray");

            #endregion
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「歸零取消」");

            tmRefresh.Enabled = false;
            SYSPara.SysState = StateMode.SM_ABORT;
            Parent = null;
        }

        private void InitialForm_Load(object sender, EventArgs e)
        {
            btnCancel.Top = 0;
            btnCancel.Left = FormSet.mInitialF.Width - FormSet.mInitialF.btnCancel.Width;
        }

        public void Reset()
        {
            tmRefresh.Enabled = true;
        }

        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            tmRefresh.Enabled = false;

            #region 使用者修改 (各模組歸零狀態的顯示)
            
            lab_MotorX1Pos.Text = "馬達X:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "X1");
            lab_MotorX2Pos.Text = "馬達X:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "X2");
            lab_MotorZ1Pos.Text = "馬達Z:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "Z1");
            lab_MotorZ2Pos.Text = "馬達Z:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "Z2");
            lab_MotorUPos.Text = "馬達U:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "U");
            lab_MotorYPos.Text = "馬達Y:" + (int)SYSPara.CallProc("Saw", "GetMotorPos", "Y");
            
            switch ((int)SYSPara.CallProc("Saw", "GetHomeFlow"))
            {
                case 0:
                    ledLabel_Z1X.Value = false;
                    ledLabel_Z1Z.Value = false;
                    ledLabel_Z2X.Value = false;
                    ledLabel_Z2Z.Value = false;
                    ledLabel_TableY.Value = false;
                    ledLabel_Tableθ.Value = false;
                    ledLabel_Spindle.Value = false;
                    ledLabel_Vision.Value = false;
                    break;
                case 1:
                    ledLabel_Z1Z.Value = true;
                    ledLabel_Z2Z.Value = true;
                    break;
                case 2:
                    ledLabel_TableY.Value = true;
                    break;
                case 3:
                    ledLabel_Tableθ.Value = true;
                    break;
                case 4:
                    ledLabel_Z1X.Value = true;
                    break;
                case 5:
                    ledLabel_Z2X.Value = true;
                    break;
                case 6:
                    ledLabel_Spindle.Value = true;
                    break;
                case 7:
                    ledLabel_Vision.Value = true;
                    break;
            }
            #endregion

            tmRefresh.Enabled = true;
        }
    }
}
