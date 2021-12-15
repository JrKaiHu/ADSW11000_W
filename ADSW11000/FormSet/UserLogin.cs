using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonObj;

namespace ADSW11000
{
    public partial class UserLoginF : Form
    {
        private UserType LoginUser;

        private String m_LoginUser = "";
        public String CurrentUser
        {
            get
            {
                return m_LoginUser;
            }
        }

        public UserLoginF()
        {
            InitializeComponent();
        }

        private void cbUserName_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbUserPW.Focus();
        }

        private void UserLoginF_Activated(object sender, EventArgs e)
        {
            tbUserPW.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            LoginUser = (UserType)cbUserName.SelectedIndex+1;

            bool bresult = false;
            switch (LoginUser)
            {
                case UserType.utOperator:
                    if (tbUserPW.Text == SYSPara.OPPW)//v4.0.1.40 密碼設定
                        bresult = true;
                    break;
                case UserType.utEngineer:
                    //+ By Max 20210308, mSaw修改為mMAA
                    if (tbUserPW.Text == FormSet.mMainFlow.mMAA.SReadValue("EngineerPassword").ToString())//v4.0.1.40 密碼設定
                        bresult = true;
                    break;
                case UserType.utAdministrator:
                    //+ By Max 20210308, mSaw修改為mMAA
                    if (tbUserPW.Text == FormSet.mMainFlow.mMAA.SReadValue("AdministratorPassword").ToString())//v4.0.1.40 密碼設定
                        bresult = true;
                    else if (tbUserPW.Text == SYSPara.Designer)//v4.0.1.40 密碼設定
                    {
                        LoginUser = UserType.utProV;
                        bresult = true;
                    }
                    break;
            }

            if (bresult)
            {
                this.DialogResult = DialogResult.OK;
                m_LoginUser = LoginUser.ToString();
                Close();
            }
            else
                //MessageBox.Show("Error Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", "Error Password!");

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tbUserPW_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnOk_Click(sender, e);
            }
        }

        public UserType GetUserType()
        {
            return LoginUser;
        }

        private void picUser_MouseDown(object sender, MouseEventArgs e)
        {
            //v4.0.1.40 按右鍵顯示密碼，刪除
            //if (e.Button == MouseButtons.Right)
            //{
            //    switch (cbUserName.SelectedIndex)
            //    {
            //        case 0:
            //            tbUserPW.Text = SYSPara.OPPW;
            //            break;
            //        case 1:
            //            tbUserPW.Text = SYSPara.ENGPW;
            //            break;
            //        case 2:
            //            tbUserPW.Text = "89543914";
            //            break;
            //    }
            //}
        }

        private void tbUserPW_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("" + System.Environment.SystemDirectory + "/osk.exe");
        }

        private void UserLoginF_Load(object sender, EventArgs e)
        {
            cbUserName.SelectedIndex = 0;
            tbUserPW.Text = "";
        }
    }
}
