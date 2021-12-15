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
using CommonObj;
using System.Resources;
using System.Globalization;

namespace ADSW11000
{
    public partial class PackageSelectExF : Form
    {
        public PackageSelectExF()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";

            lvPackage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvPackage, true, null);

            lvFolder.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvFolder, true, null);
        }

        #region 屬性

        //產品尺寸資料夾路徑
        private string mFolderPath;
        [Browsable(false), Category("#參數設定"), Description("產品尺寸資料夾路徑")]
        public string FolderPath
        {
            get { return mFolderPath; }
            set
            {
                mFolderPath = value;
                RefreshFolder();              
            }
        }

        private string mFolderName;
        [Browsable(false), Category("#參數設定"), Description("資料夾名稱")]
        public string FolderName
        {
            get { return mFolderName; }
            set
            {
                mFolderName = value;
                lvFolder.SelectItem = value;
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

        //取得PartID
        public string GetPartID()
        {
            return tbPartID.Text;
        }
        //取得LotID
        public string GetLotID()
        {
            return tbLotID.Text;
        }

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

        private void RefreshFolder()
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            if (!string.IsNullOrEmpty(FolderPath))
            {
                string[] list = Directory.GetDirectories(FolderPath);

                lvFolder.Items.Clear();
                for (int i = 0; i < list.Length; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = "";
                    item.SubItems.Add(Path.GetFileNameWithoutExtension(list[i]));

                    lvFolder.Items.Add(item);
                }
            }

            //給預設值
            if (string.IsNullOrEmpty(FolderName))
            {
                if (lvFolder.Items.Count > 0)
                    FolderName = lvFolder.Items[0].SubItems[1].Text;
                else
                    FolderName = "";
            }
            else
            {
                ListViewItem item = lvFolder.FindItemWithText(FolderName);
                if (item == null)
                {
                    if (lvFolder.Items.Count > 0)
                        FolderName = lvFolder.Items[0].SubItems[1].Text;
                    else
                        FolderName = "";
                }
            }
        }

        private void RefreshFile()
        {
            string path = string.Format(@"{0}\{1}\", FolderPath, FolderName);
            string[] list = Directory.GetFiles(path, "*.xml");

            lvPackage.BeginUpdate();
            lvPackage.Items.Clear();
            string s = "";
            bool findit = false;
            for (int i = 0; i < list.Length; i++)
            {
                findit = false;
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
                    ListViewItem item = new ListViewItem();
                    item.Text = "";
                    item.SubItems.Add(s);

                    PackageDS.Initial(list[i], "Package");

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

        private string GetDialogMsgText(string strKey)
        {
            ResourceManager resourceManager = new ResourceManager(typeof(Properties.Resources));
            if (SYSPara.Lang.GetNowLanguage() == "tw")
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("zh-TW"));
            }
            else
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("en-US"));
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
                FormSet.mPackage.SetFileName(FileName, FolderName, true);

                #region By Wolf 20210819 v4.0.87.1 非新建料號可忽略靶點學習進行生產
                string strLastTime = "";
                foreach (ListViewItem item in lvPackage.Items)
                {
                    if (item.SubItems[1].Text == lvPackage.SelectItem)
                    {
                        strLastTime = item.SubItems[3].Text;
                    }
                }

                SYSPara.CallProc("Saw", "SetTargetTeachIgnore", strLastTime.Length != 0);
                #endregion

                FormSet.mPackage.SetLastTime();

                SYSPara.PackageName = FileName;

                SYSPara.SysState = StateMode.SM_PACKAGELOAD_OK;
                Parent = null;

                //20200803
                FormSet.mMainFlow.ShowDryMode();   //v0.0.7.11 By Sanxiu 空跑模式修正，自動設定依切割道作業
            }
            else
                //MessageBox.Show("請選擇產品");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_PackageSelectExf_Select_Package_Hint"));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品搜尋」");
            FindPackage = tbSearch.Text;
        }

        private void PackageSelectF_Load(object sender, EventArgs e)
        {
            //btnClose.Top = 0;
            btnClose.Left = FormSet.mPackageSelF.Width - btnClose.Width;
            //btnClose.Height = pnlButton.Height - 4;

            //btnOk.Top = 0;
            btnOk.Left = btnClose.Left - btnOk.Width;
            //btnOk.Height = pnlButton.Height - 4;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者輸入「Part ID」");
                tbLotID.Focus();
            }
        }

        private void tbLotID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者輸入「Lot ID」");
                tbSerialID.Focus();
            }
        }

        private void tbSerialID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者輸入「Lot ID」");
            }
        }

        private void lvFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
                FolderName = lvFolder.SelectedItems[0].SubItems[1].Text;             
        }

        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品搜尋」");
                FindPackage = tbSearch.Text;
            }
        }

        private void lvPackage_Resize(object sender, EventArgs e)
        {
            lvPackage.Columns[0].Width = 0;
            lvPackage.Columns[1].Width = lvPackage.Width - 480 - 35;
            lvPackage.Columns[2].Width = 240;
            lvPackage.Columns[3].Width = 240;
        }

        private void lvPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
                FileName = lvPackage.SelectedItems[0].SubItems[1].Text;
        }
    }
}
