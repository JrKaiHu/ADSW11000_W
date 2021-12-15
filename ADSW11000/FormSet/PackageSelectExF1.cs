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
using System.IO;

namespace ProVSimpleProject
{
    public partial class PackageSelectExF1 : Form
    {
        public PackageSelectExF1()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";

            lvFolder.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvFolder, true, null);  
        }

        //預設資料夾
        private string mFolderPath;
        [Browsable(false), Category("#參數設定"), Description("資料夾路徑")]
        public string FolderPath
        {
            get { return mFolderPath; }
            set
            {
                mFolderPath = value;
                if (mFolderPath != "")
                    RefreshFolder();
            }
        }

        private string mFileName;
        [Browsable(false), Category("#參數設定"), Description("檔案名稱")]
        public string FileName
        {
            get { return mFileName; }
            set
            {
                mFileName = value;
                if (mFileName != "")
                {
                    foreach (ListViewItem item in lvFolder.Items)
                    {
                        if (item.SubItems[1].Text == mFileName)
                        {
                            item.Selected = true;
                            lvFolder.Select();
                            break;
                        }
                    }
                }
            }
        }

        private string mFindPackage;
        [Browsable(false), Category("#參數設定"), Description("搜尋字串")]
        public string FindPackage
        {
            get { return mFindPackage; }
            set
            {
                mFindPackage = value;
                tbSearch.Text = value;
                RefreshFolder();
            }
        }

        #region 使用者函數

        public void RefreshFolder()
        {
            string[] list = Directory.GetFiles(FolderPath, "*.xml")
                .Select(file => Path.GetFileName(file)).ToArray();

            lvFolder.BeginUpdate();
            lvFolder.Items.Clear();
            string s = "";
            bool findit = false;
            for (int i = 0; i < list.Length; i++)
            {
                findit = false;
                ListViewItem item = new ListViewItem();
                s = Path.GetFileNameWithoutExtension(list[i]);
                if (string.IsNullOrEmpty(FindPackage))
                    findit = true;
                else
                {
                    if (s.ToUpper().IndexOf(FindPackage.ToUpper()) >= 0)
                        findit = true;
                }

                if (findit)
                {
                    string fname = string.Format(@"{0}\{1}.xml", FolderPath, s);
                    PackageDS.Initial(fname, "Package");
                    if (PackageDS.PackageList.Count > 0)
                    {
                        item.Text = "";
                        item.SubItems.Add(s);
                        item.SubItems.Add(ToDateTimeStr(PackageDS.PackageList[0].CreateTime));
                        item.SubItems.Add(ToDateTimeStr(PackageDS.PackageList[0].LastTime));
                        lvFolder.Items.Add(item);
                    }
                }
            }
            lvFolder.EndUpdate();

            if (lvFolder.Items.Count == 1)
                FileName = lvFolder.Items[0].SubItems[1].Text;
            else
                FileName = "";
        }

        //顯示可用的產品名稱清單
        public void Initial(DataManagement pPackageDS)
        {
            //pPackageDS.CopyTo(PackageDS);
            //lvPackage.SelectPackage = "";
        }

        //取得選擇的產品名稱
        public string GetPackageName()
        {
            return FileName;
        }

        public void RefreshView()
        {
            FindPackage = "";
            lvFolder.SelectedItems.Clear();
            FileName = "";
        }

        #endregion

        #region 私有函數

        private string ToDateTimeStr(string TimeStr)
        {
            if ((TimeStr != null) && (TimeStr.Length == 14))
            {
                string yyyy = TimeStr.Substring(0, 4);
                string mm = TimeStr.Substring(4, 2);
                string dd = TimeStr.Substring(6, 2);
                string hh = TimeStr.Substring(8, 2);
                string MM = TimeStr.Substring(10, 2);
                string ss = TimeStr.Substring(12, 2);
                string result = string.Format("{0}/{1}/{2} {3}:{4}:{5}", yyyy, mm, dd, hh, MM, ss);
                return result;
            }
            return "";
        }

        private DataTransfer PReadValue(string field)
        {
            return PackageDS.ReadValue(DataManagement.DataType.Package, field);
        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "產品載入取消");
            SYSPara.SysState = StateMode.SM_ABORT;
            Parent = null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "產品載入確定");
            if (!string.IsNullOrEmpty(FileName))
            {
                //FormSet.mPackage.SetFileNameInAuto(FileName);
                SYSPara.SysState = StateMode.SM_PACKAGELOAD_OK;
                Parent = null;
            }
            else
                MessageBox.Show("請選擇產品");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品搜尋」");
            FindPackage = tbSearch.Text;
        }

        private void PackageSelectF_Load(object sender, EventArgs e)
        {
            btnClose.Left = FormSet.mPackageSelF.Width - btnClose.Width;
            btnOk.Left = btnClose.Left - btnOk.Width;
        }

        private void tbSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品搜尋」");
                FindPackage = tbSearch.Text;
            }
        }

        private void lvFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
            {
                string fname = string.Format(@"{0}\{1}.xml", FolderPath, lvFolder.SelectedItems[0].SubItems[1].Text);
                if (File.Exists(fname))
                {
                    PackageDS.Initial(fname, "Package");
                    PackageDS.SelectPackage = "Package";
                    //DataSnippet.DataGetAll(panel2, DataManagement.DataType.Package, "Package");
                    mFileName = lvFolder.SelectedItems[0].SubItems[1].Text;
                }
            }
        }
    }
}
