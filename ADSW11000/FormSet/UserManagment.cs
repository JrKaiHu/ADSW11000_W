using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ADSW11000
{
    public partial class UserManagment : Form
    {
        public UserManagment()
        {
            InitializeComponent();
            cbUserRight.SelectedIndex = 0;

            if (SYSPara.LoginUser == UserType.utEngineer)
                cbUserRight.Items.Remove("Administrator");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        //讀入所有的使用者
        public void LoadUser()
        {
            lvUserList.Items.Clear();
            for (int i = 1; i < SYSPara.UserList.Count; i++)
            {
                ListViewItem item = new ListViewItem();
                UserDT data = SYSPara.UserList[i];
                item.Text = data.UserID;
                item.SubItems.Add(data.UserPW);
                switch (data.UserRight)
                {
                    case UserType.utOperator:
                        item.SubItems.Add("Operator");
                        break;
                    case UserType.utEngineer:
                        item.SubItems.Add("Enginneer");
                        break;
                    case UserType.utAdministrator:
                        item.SubItems.Add("Administrator");
                        break;
                }
                lvUserList.Items.Add(item);
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbUserID.Text))
            {
                MessageBox.Show("請輸入員工編號");
            }
            else
            {
                //搜尋是否有重覆的編號
                bool findit = false;
                for (int i = 0; i < SYSPara.UserList.Count; i++)
                {
                    UserDT data = SYSPara.UserList[i];
                    if (data.UserID == tbUserID.Text)
                        findit = true;
                }
                if (findit)
                {
                    MessageBox.Show("員工編號重覆");
                }
                else
                {
                    UserDT data = new UserDT();
                    data.UserID = tbUserID.Text;
                    data.UserPW = tbUserPW.Text;
                    data.UserRight = (UserType)cbUserRight.SelectedIndex+1;
                    SYSPara.UserList.Add(data);
                    LoadUser();

                    WriteTo();

                    tbUserID.Text = "";
                    tbUserPW.Text = "";
                    cbUserRight.SelectedIndex = 0;
                }
            }
        }

        private void WriteTo()
        {
            string fname = SYSPara.SysDir + @"\LocalData\User";
            File.Delete(fname);
            FileStream fs = new FileStream(fname, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //開始寫入
            string s = "";
            for (int i = 1; i < SYSPara.UserList.Count; i++)
            {
                UserDT data = SYSPara.UserList[i];
                s = data.UserID + "," + data.UserPW;
                s = s + "," + (int)data.UserRight;
                sw.WriteLine(s);
            }
            //清空緩衝區
            sw.Flush();
            //關閉流
            sw.Close();
            fs.Close();
        }

        private void lvUserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvUserList.SelectedItems.Count > 0)
            {
                ListViewItem item = lvUserList.SelectedItems[0];
                if (item != null)
                {
                    tbUserID.Text = item.SubItems[0].Text;
                    tbUserPW.Text = item.SubItems[1].Text;

                    for (int i = 0; i < SYSPara.UserList.Count; i++)
                    {
                        UserDT data = SYSPara.UserList[i];
                        if (data.UserID == tbUserID.Text)
                        {
                            cbUserRight.SelectedIndex = (int)data.UserRight-1;
                            break;
                        }
                    }
                }
            }
        }

        private void btnChangeUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbUserID.Text))
            {
                MessageBox.Show("請輸入員工編號");
            }
            else
            {
                //搜尋是否有重覆的編號
                bool findit = false;
                int index = 0;
                for (int i = 0; i < SYSPara.UserList.Count; i++)
                {
                    UserDT data = SYSPara.UserList[i];
                    if (data.UserID == tbUserID.Text)
                    {
                        index = i;
                        findit = true;
                    }
                }
                if (findit)
                {
                    UserDT data = SYSPara.UserList[index];
                    data.UserID = tbUserID.Text;
                    data.UserPW = tbUserPW.Text;
                    data.UserRight = (UserType)cbUserRight.SelectedIndex+1;
                    SYSPara.UserList[index] = data;
                    LoadUser();

                    WriteTo();

                    tbUserID.Text = "";
                    tbUserPW.Text = "";
                    cbUserRight.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("找不到員工編號");
                }
            }
        }

        private void btnDelUser_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbUserID.Text))
            {
                MessageBox.Show("請輸入員工編號");
            }
            else
            {
                //搜尋是否有重覆的編號
                bool findit = false;
                int index = 0;
                for (int i = 0; i < SYSPara.UserList.Count; i++)
                {
                    UserDT data = SYSPara.UserList[i];
                    if (data.UserID == tbUserID.Text)
                    {
                        index = i;
                        findit = true;
                    }
                }
                if (findit)
                {
                    SYSPara.UserList.RemoveAt(index);
                    LoadUser();

                    WriteTo();

                    tbUserID.Text = "";
                    tbUserPW.Text = "";
                    cbUserRight.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("找不到員工編號");
                }
            }
        }
    }
}
