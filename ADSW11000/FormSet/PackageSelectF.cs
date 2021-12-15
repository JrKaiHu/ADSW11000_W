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

namespace ADSW11000
{
    public partial class PackageSelectF : Form
    {
        public PackageSelectF()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";

            lvPackage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvPackage, true, null);  
        }

        #region 屬性

        //預設資料夾
        private string mFolderPath;
        [Browsable(false), Category("#參數設定"), Description("資料夾路徑")]
        public string FolderPath
        {
            get { return mFolderPath; }
            set
            {
                mFolderPath = value;
                RefreshFile();
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
                lvPackage.SelectItem = value;
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
                RefreshFile();
            }
        }

        #endregion

        #region 使用者函數

        //顯示可用的產品名稱清單
        public void Initial()
        {
            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";
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

        private void RefreshFile()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            string[] list = Directory.GetFiles(FolderPath, "*.xml")
                .Select(file => Path.GetFileName(file)).ToArray();

            lvPackage.BeginUpdate();
            lvPackage.Items.Clear();
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

                    item.Text = "";
                    item.SubItems.Add(s);
                    item.SubItems.Add(ToDateTimeStr(PackageDS.CreateTime));
                    item.SubItems.Add(ToDateTimeStr(PackageDS.LastTime));
                    lvPackage.Items.Add(item);
                }
            }
            lvPackage.EndUpdate();

            //給預設值
            if (string.IsNullOrEmpty(FileName))
            {
                if (lvPackage.Items.Count > 0)
                    FileName = lvPackage.Items[0].SubItems[1].Text;
                else
                    FileName = "";
            }
            else
            {
                ListViewItem item = lvPackage.FindItemWithText(FileName);
                if (item == null)
                {
                    if (lvPackage.Items.Count > 0)
                        FileName = lvPackage.Items[0].SubItems[1].Text;
                    else
                        FileName = "";
                }
            }
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
                FormSet.mPackage.SetFileName(FileName, "", true);
                FormSet.mPackage.SetLastTime();
                SYSPara.PackageName = FileName;

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
            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
                FileName = lvPackage.SelectedItems[0].SubItems[1].Text;
        }

        private void lvFolder_Resize(object sender, EventArgs e)
        {
            lvPackage.Columns[0].Width = 0;
            lvPackage.Columns[1].Width = lvPackage.Width - 480 - 35;
            lvPackage.Columns[2].Width = 240;
            lvPackage.Columns[3].Width = 240;
        }
    }
}
