using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ADSW11000
{
    public partial class UserLoginEx : Form
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

        private bool m_PMSet = false;

        public UserLoginEx()
        {
            InitializeComponent();
            m_PMSet = false;
        }

        public UserLoginEx(bool bPMSet)
        {
            InitializeComponent();
            m_PMSet = bPMSet;

            lbUserPW.Visible = false;
            tbUserPW.Visible = false;
        }

        private void UserLoginEx_Activated(object sender, EventArgs e)
        {
            tbUserID.Focus();
        }

        private int UserExistByFile(string UserID, string UserPW)
        {
            int result = 0;
            for (int i = 0; i < SYSPara.UserList.Count; i++)
            {
                UserDT data = SYSPara.UserList[i];
                if (data.UserID == UserID)
                {
                    if (data.UserPW == UserPW)
                    {
                        result = 1;
                        LoginUser = data.UserRight;
                        return result;
                    }
                    else
                    {
                        result = 2;
                    }
                }
            }
            return result;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string UserID = tbUserID.Text;
            if (m_PMSet)
            {
                this.DialogResult = DialogResult.OK;
                m_LoginUser = UserID;
                Close();   
            }
            else
            {
                if (UserID.Length != 0)
                {
                    int result = UserExistByFile(UserID, tbUserPW.Text);

                    switch (result)
                    {
                        case 0: //指令錯誤
                            MessageBox.Show("Query fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 1: //密碼正確
                            this.DialogResult = DialogResult.OK;
                            m_LoginUser = UserID;
                            Close();
                            break;
                        case 2: //密碼不正確
                            MessageBox.Show("Error Password!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        case 3: //使用者不存在
                            MessageBox.Show("User not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Please Input UserID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        public UserType GetUserType(ref string mUserID)
        {
            mUserID = tbUserID.Text.ToUpper();
            return LoginUser;
        }

        private void tbUserID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                tbUserPW.Focus();
            }
        }

        private void tbUserPW_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnOK_Click(sender, e);
            }
        }

        private void UserLoginEx_Load(object sender, EventArgs e)
        {
            tbUserID.Text = "";
            tbUserPW.Text = "";
        }
    }
}
