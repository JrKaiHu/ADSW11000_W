using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KCSDK;

namespace ADSW11000
{
    //使用者修改 (設備通用參數放置處)
    public partial class OptionF : Form
    {
        public OptionF()
        {
            InitializeComponent();

            #region 架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            RefreshAllData();

            #endregion
        }

        public void RefreshAllData()
        {            
            OptionDS.Initial(SYSPara.SysDir + @"\LocalData\MachineSet.xml", "SYS");
            DataSnippet.DataGetAll(this);
            DataSnippet.ControlEnable(this, false);
        }

        public void Initial()
        {
        }

        //使用者權限切換後的頁面變化控制
        public void ChangeUser()
        {
            switch (SYSPara.LoginUser)
            {
                case UserType.utNone:
                    break;
                case UserType.utOperator:
                    break;
                case UserType.utEngineer:
                    break;
                case UserType.utAdministrator:
                    break;
            }
        }

        #region 架構使用

        public bool IsLogExist()
        {
            return OptionDS.IsLogExist();
        }

        public string GetLog()
        {
            return OptionDS.GetLog();
        }

        public void ExitClick()
        {
            DataSnippet.ControlEnable(this, false);
            Parent = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結束」-[通用設定]");

            DataSnippet.ControlEnable(this, false);
            Parent = null;
            SYSPara.OptionStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
        }

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「編輯」-[通用設定]");
            DataSnippet.ControlEnable(this, true, SYSPara.IsAutoMode);
            SYSPara.OptionStop = false;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
            SYSPara.SysRun = false;   //測試Auto下不停機
            FormSet.mMSS.M_Stop();    //測試Auto下不停機
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「儲存」-[通用設定]");

            SYSPara.OptionStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
            DataSnippet.ControlEnable(this, false);
            DataSnippet.DataSetAll(this);
            OptionDS.SaveFile();         
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「取消」-[通用設定]");

            SYSPara.OptionStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
            DataSnippet.DataGetAll(this);
            DataSnippet.ControlEnable(this, false);
        }

        //關聯式子表格相關設定
        public void InitialSubPackage()
        {
            #region 使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)
            //tFieldCB1.EditForm = FormSet.mToolF;
            //FormSet.mToolF.Tag = this;
            #endregion
        }

        #endregion
        public void ChangeLang(string lang)
        {
            switch (lang)
            {
                case "tw":
                    label3.Text = "公尺";
                    label4.Text = "片";
                    label5.Text = "頻率:每隔";
                    label6.Text = "片";
                    label7.Text = "第";
                    label8.Text = "道檢查";
                    label9.Text = "公尺";
                    label10.Text = "片";
                    break;
                case "en":
                    label3.Text = "M";
                    label4.Text = "MP";
                    label5.Text = "Freq:";
                    label6.Text = "MP";
                    label7.Text = "The";
                    label8.Text = "Line";
                    label9.Text = "M";
                    label10.Text = "MP";
                    break;
            }
        }

        //+ By Max 20210325, v4.0.1.58, 空跑測試CheckBox隱藏
        private void label1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                dCheckBox1.Visible = !dCheckBox1.Visible;
                dCheckBox2.Visible = !dCheckBox2.Visible;
                dEdit5.Visible = !dEdit5.Visible;
                dEdit6.Visible = !dEdit6.Visible;
                label2.Visible = !label2.Visible;
                label11.Visible = !label11.Visible;
                
            }
        }
    }
}
