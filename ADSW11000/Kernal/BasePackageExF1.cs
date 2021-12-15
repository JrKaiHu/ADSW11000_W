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
using System.Threading;

namespace ProVSimpleProject
{
    public partial class BasePackageExF1 : Form
    {
        public bool Editable = false;

        public BasePackageExF1()
        {
            InitializeComponent();

            #region 架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            ParentObj = null;
            lvFolder.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvFolder, true, null);  


            #endregion
        }

        #region 屬性

        //資料模組名稱
        private string mModuleName;
        [Browsable(false), Category("#參數設定"), Description("資料模組名稱")]
        public string ModuleName
        {
            get { return mModuleName; }
            set
            {
                mModuleName = value;
                if (mModuleName != "")
                {
                    string sFilePath = SYSPara.SysDir + string.Format(@"\LocalData\Package\{0}.xml", mModuleName);
                    PackageDS.Initial(sFilePath, mModuleName);

                    //lbTitle.Text = string.Format("{0}設定", mModuleName);
                }
            }
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
                tbPackage.Text = value;
                RefreshFolder();
            }
        }

        [Browsable(false), Category("#參數設定"), Description("結束時返回的視窗指標")]
        public Control ParentObj { get; set; }

        #endregion

        #region 架構使用 (Public)

        public object CallProc(string ModuleName, string FuncName, params object[] paras)
        {
            return FormSet.mMSS.CallProc(ModuleName, FuncName, paras);
        }

        public virtual void SelectChange()
        {
        }

        public virtual void AfterSave()
        {
        }

        public DataTransfer PReadValue(string field)
        {
            PackageDS.SelectPackage = "Package";
            return PackageDS.ReadValue(DataManagement.DataType.Package, field);
        }

        public DataTransfer OReadValue(string field)
        {
            return FormSet.mOption.ReadValue(field);
        }

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
                    item.Text = "";
                    item.SubItems.Add(s);

                    lvFolder.Items.Add(item);
                }
            }
            lvFolder.EndUpdate();

            if (lvFolder.Items.Count == 1)
                FileName = lvFolder.Items[0].SubItems[1].Text;
            else
                FileName = "";
        }

        public void SetFileName(string filename)
        {
            string fname = string.Format(@"{0}\{1}.xml", FolderPath, filename);
            if (File.Exists(fname))
            {
                PackageDS.Initial(fname, "Package");
                PackageDS.SelectPackage = filename;
                mFileName = filename;
            }
        }

        public void ExitClick()
        {
            DataSnippet.ControlEnable(pnlControl, false);
            Parent = null;
        }

        public bool IsLogExist()
        {
            return PackageDS.IsLogExist();
        }

        public string GetLog()
        {
            return PackageDS.GetLog();
        }

        //更新 ListView UI
        public void RefreshView()
        {
            lvFolder.SelectedItems.Clear();
            FileName = "";
        }

        #endregion

        #region 架構使用 (Private)

        //將 Package Name 實際寫入到各模組的檔案
        private void AddPackageToFile(string PackageName, string srcPackageName)
        {
            //處理 Stage Module
            foreach (Control control in PackageContainer.Panel2.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).SetData(PackageName);
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData(PackageName);
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData(PackageName);
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData(PackageName);
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData(PackageName);
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData(PackageName);
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData(PackageName);
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData(PackageName);
                }

                if (control.HasChildren)
                {
                    AddPackageToFileCallBack(control, PackageName, srcPackageName);
                }
            }

            //將更新後的資料存成檔案
            PackageDS.SaveFile();
            //把各 Module 的 Recipe Data 讀入
            PackageDS.LoadFile();
        }
        private void AddPackageToFileCallBack(Control ctrl, string PackageName, string srcPackageName)
        {
            foreach (Control control in ctrl.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).SetData(PackageName);
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData(PackageName);
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData(PackageName);
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData(PackageName);
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData(PackageName);
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData(PackageName);
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData(PackageName);
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData(PackageName);
                }

                if (control.HasChildren)
                    AddPackageToFileCallBack(control, PackageName, srcPackageName);
            }
        }

        //更新Package內全部的元件資料
        public void RefreshAllData(string PackageName)
        {
            //處理 Stage Module
            foreach (Control control in PackageContainer.Panel2.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).RefreshDisplay(PackageName);
                    ((TFieldCB)control).TableSource.PackageNameInAuto = ((TFieldCB)control).FieldValue;
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).RefreshDisplay(PackageName);
                }

                if (control.HasChildren)
                {
                    RefreshAllDataCallBack(control, PackageName);
                }
            }

            //將更新後的資料存成檔案
            PackageDS.SaveFile();
            //把各 Module 的 Recipe Data 讀入
            PackageDS.LoadFile();
        }
        private void RefreshAllDataCallBack(Control ctrl, string PackageName)
        {
            foreach (Control control in ctrl.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).RefreshDisplay(PackageName);
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).RefreshDisplay(PackageName);
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).RefreshDisplay(PackageName);
                    ((TFieldCB)control).TableSource.PackageNameInAuto = ((TFieldCB)control).FieldValue;
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).RefreshDisplay(PackageName);
                }

                if (control.HasChildren)
                    RefreshAllDataCallBack(control, PackageName);
            }
        }

        public void SetFileNameInAuto(string filename)
        {
            PackageDS.PackageNameInAuto = filename;
        }

        #endregion

        #region 架構使用 (按鈕功能)

        private void btnClose_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「結束」-[{0}設定]", ModuleName));

            Control pParent = Parent;

            DataSnippet.ControlEnable(pnlControl, false);
            Editable = false;
            this.Parent = null;

            if (ParentObj != null)
            {
                pParent.Controls.Add(ParentObj);
                ((Form)ParentObj).WindowState = FormWindowState.Maximized;
                ((Form)ParentObj).Show();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("請選擇產品");
                return;
            }

            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「編輯」-[{0}設定]", ModuleName));

            DataSnippet.ControlEnable(pnlControl, true, SYSPara.IsAutoMode);
            Editable = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("請選擇產品");
                return;
            }

            bool cansave = true;
            if (SYSPara.IsAutoMode && (PackageDS.PackageNameInAuto != FileName))
                cansave = false;

            if (cansave)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「儲存」-[{0}設定]", ModuleName));

                DataSnippet.ControlEnable(pnlControl, false);
                DataSnippet.DataSetAll(pnlControl, DataManagement.DataType.Package, "Package");
                Editable = false;
                PackageDS.SaveFile();

                AfterSave();
            }
            else
                MessageBox.Show("自動操作下，禁止存取非生產產品之設定");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「取消」-[{0}設定]", ModuleName));

            DataSnippet.DataGetAll(pnlControl, DataManagement.DataType.Package, "Package");
            DataSnippet.ControlEnable(pnlControl, false);
            Editable = false;
        }

        private void PackageF_Load(object sender, EventArgs e)
        {
            PackageContainer.SplitterDistance = 225;
            DataSnippet.ControlEnable(pnlControl, false);
        }

        public void AddRow(DDataGridView obj)
        {
            obj.Rows.Add();
            obj.Rows[obj.Rows.Count - 1].Cells[0].Value = obj.Rows.Count;
        }

        public void DelRow(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
                obj.Rows.RemoveAt(obj.SelectedCells[0].RowIndex);
        }

        public void ClearAll(DDataGridView obj)
        {
            if (MessageBox.Show("您確定要清除所有切割順序資料嗎 ?", "詢問", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                obj.Rows.Clear();
            }
        }

        public void UpMove(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
            {
                if (obj.SelectedCells[0].RowIndex > 0)
                {
                    object oValue;

                    for (int i = 1; i < obj.Columns.Count; i++)
                    {
                        oValue = obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value = obj.Rows[obj.SelectedCells[0].RowIndex - 1].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex - 1].Cells[i].Value = oValue;
                    }
                    obj.Rows[obj.SelectedCells[0].RowIndex - 1].Cells[obj.SelectedCells[0].ColumnIndex].Selected = true;
                }
            }
        }

        public void DownMove(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
            {
                if (obj.SelectedCells[0].RowIndex < obj.Rows.Count - 1)
                {
                    object oValue;

                    for (int i = 1; i < obj.Columns.Count; i++)
                    {
                        oValue = obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value = obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[i].Value = oValue;
                    }
                    obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[obj.SelectedCells[0].ColumnIndex].Selected = true;
                }
            }
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            string NewFileName = "NoName";
            if (UserSnippet.InputBox("Create Package", "Package Name :", ref NewFileName) == DialogResult.OK)
            {
                try
                {
                    if (string.IsNullOrEmpty(NewFileName))
                    {
                        MessageBox.Show("請輸入產品名稱");
                        return;
                    }
                }
                catch (Exception)
                {
                }

                string fname = string.Format(@"{0}\{1}.xml", FolderPath, NewFileName);
                if (!File.Exists(fname))
                {
                    if (string.IsNullOrEmpty(FileName))
                        File.Create(fname);
                    else
                    {
                        string srcfname = string.Format(@"{0}\{1}.xml", FolderPath, FileName);
                        File.Copy(srcfname,fname);
                    }

                    if (!File.Exists(fname))
                        MessageBox.Show(string.Format("{0} 建立失敗", NewFileName));
                    else
                    {
                        FindPackage = "";
                        FileName = NewFileName;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("{0} 名稱重覆", NewFileName));
                }
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
                    DataSnippet.DataGetAll(pnlControl, DataManagement.DataType.Package, "Package");
                    DataSnippet.ControlEnable(pnlControl, false, SYSPara.IsAutoMode);
                    mFileName = lvFolder.SelectedItems[0].SubItems[1].Text;

                    SelectChange();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「產品更名」-[產品設定]"));

            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
            {
                string OldName = lvFolder.SelectedItems[0].SubItems[1].Text;
                string NewName = OldName;
                if (UserSnippet.InputBox("Change Package Name", "New Name", ref NewName) == DialogResult.OK)
                {
                    string oldpath = string.Format(@"{0}\{1}.xml", FolderPath, OldName);
                    string newpath = string.Format(@"{0}\{1}.xml", FolderPath, NewName);

                    if (!File.Exists(newpath))
                    {
                        File.Copy(oldpath, newpath);
                        File.Delete(oldpath);
                        for (int i = 0; i < 5; i++)
                        {
                            oldpath = string.Format(@"{0}\{1}_{2}.cfg", FolderPath, OldName, i + 1);
                            newpath = string.Format(@"{0}\{1}_{2}.cfg", FolderPath, NewName, i + 1);
                            if (!File.Exists(newpath))
                            {
                                if (File.Exists(oldpath))
                                {
                                    File.Copy(oldpath, newpath);
                                    File.Delete(oldpath);
                                }
                            }
                        }

                        FindPackage = "";
                    }
                    else
                        MessageBox.Show("產品名稱重覆");
                }
            }
            else
                MessageBox.Show("請選擇一個產品");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品刪除」-[產品設定]");

            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
            {
                string filename = lvFolder.SelectedItems[0].SubItems[1].Text;

                DialogResult result = MessageBox.Show(string.Format("確定要刪除 {0} 嗎？", filename), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.OK:
                        {
                            string fname = string.Format(@"{0}\{1}.xml", FolderPath, filename);
                            File.Delete(fname);
                            for (int i = 0; i < 5; i++)
                            {
                                fname = string.Format(@"{0}\{1}_{2}.cfg", FolderPath, filename,i+1);
                                File.Delete(fname);
                            }

                            FindPackage = "";
                        }
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
            else
                MessageBox.Show("請選擇一個產品");
        }

        private void tbPackage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindPackage = tbPackage.Text;
        }
    }
}