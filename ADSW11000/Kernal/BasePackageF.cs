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

namespace ADSW11000
{
    public partial class BasePackageF : Form
    {
        public bool Editable = false;

        private DataManagement m_PackageDS = new DataManagement();
        public DataManagement PackageDS
        {
            get
            {
                m_PackageDS.ModifiedLog = true;
                return m_PackageDS;
            }
            set
            {
                m_PackageDS = value;
            }
        }

        public string FileName_Auto = string.Empty;
        public string FolderName_Auto = string.Empty;

        public BasePackageF()
        {
            InitializeComponent();

            #region 架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            lvPackage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvPackage, true, null);  

            #endregion
        }
        public Control ParentObj { get; set; }
        #region 屬性

        //資料模組名稱
        private string mModuleName;
        [Browsable(false), Category("#參數設定"), Description("資料模組名稱")]
        public string ModuleName
        {
            get { return mModuleName; }
            set { mModuleName = value; }
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

                string fname = string.Format(@"{0}\{1}.xml", FolderPath, FileName);
                if (File.Exists(fname))
                {
                    PreloadPackageDS.Initial(fname, ModuleName);
                    DataSnippet.DataGetAll(pnlControl);
                    DataSnippet.ControlEnable(pnlControl, false, SYSPara.IsAutoMode);

                    SelectChange();
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
                RefreshFile();
            }
        }

        #endregion

        #region 架構使用 (Public)

        public virtual void SelectChange()
        {
        }

        public virtual void AfterSave()
        {
        }

        public void SetFileName(string filename, string foldername, bool IsAuto = false)
        {
            string fname = string.Format(@"{0}\{1}.xml", FolderPath, filename);
            if (File.Exists(fname))
            {
                PackageDS.Initial(fname, ModuleName);
                PreloadPackageDS.Initial(fname, ModuleName);
                mFileName = filename;

                if (IsAuto)
                {
                    FileName_Auto = mFileName;
                    FolderName_Auto = foldername;
                    lvPackage.FixItem = mFileName;
                }
            }
        }

        public void ExitClick()
        {
            DataSnippet.ControlEnable(pnlControl, false);
            Parent = null;
        }

        public bool IsLogExist()
        {
            return PreloadPackageDS.IsLogExist();
        }

        public string GetLog()
        {
            return PreloadPackageDS.GetLog();
        }

        public void Initial()
        {
            FindPackage = "";
            InitialSubPackage();

            if (SYSPara.RunMode == RunModeDT.AUTO)
            {
                FileName = FileName_Auto;
                lvPackage.FixItem = FileName_Auto;
            }
        }

        //設定產品的最後一次使用日期
        public void SetLastTime()
        {
            PreloadPackageDS.SetLastTime();
        }

        //關聯式子表格相關設定
        public virtual void InitialSubPackage()
        {
        }

        public void AutoEnd()
        {
            lvPackage.FixItem = "";
        }

        #endregion

        #region 架構使用 (Private)

        //將 Package Name 實際寫入到各模組的檔案
        private void AddPackageToFile()
        {
            //處理 Stage Module
            foreach (Control control in PackageContainer.Panel2.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).SetData();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData();
                }

                if (control.HasChildren)
                {
                    AddPackageToFileCallBack(control);
                }
            }

            //將更新後的資料存成檔案
            PreloadPackageDS.SaveFile();
            //把各 Module 的 Recipe Data 讀入
            PreloadPackageDS.LoadFile();
        }
        private void AddPackageToFileCallBack(Control ctrl)
        {
            foreach (Control control in ctrl.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).SetData();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData();
                }

                if (control.HasChildren)
                    AddPackageToFileCallBack(control);
            }
        }

        //更新Package內全部的元件資料
        public void RefreshAllData()
        {
            //處理 Stage Module
            foreach (Control control in PackageContainer.Panel2.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).RefreshDisplay();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).RefreshDisplay();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).RefreshDisplay();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).RefreshDisplay();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).RefreshDisplay();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).RefreshDisplay();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).RefreshDisplay();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).RefreshDisplay();
                }

                if (control.HasChildren)
                {
                    RefreshAllDataCallBack(control);
                }
            }

            //將更新後的資料存成檔案
            PreloadPackageDS.SaveFile();
            //把各 Module 的 Recipe Data 讀入
            PreloadPackageDS.LoadFile();
        }
        private void RefreshAllDataCallBack(Control ctrl)
        {
            foreach (Control control in ctrl.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).RefreshDisplay();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).RefreshDisplay();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).RefreshDisplay();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).RefreshDisplay();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).RefreshDisplay();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).RefreshDisplay();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).RefreshDisplay();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).RefreshDisplay();
                }

                if (control.HasChildren)
                    RefreshAllDataCallBack(control);
            }
        }

        private ListViewItem FindItemTextWithLV(ListView LV, string LVText)
        {
            foreach (ListViewItem item in LV.Items)
            {
                if (item.SubItems[1].Text == LVText)
                    return item;
            }
            return null;
        }

        private void RefreshFile()
        {
            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

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
                    item.Text = "";
                    item.SubItems.Add(s);

                    lvPackage.Items.Add(item);
                }
            }
            lvPackage.EndUpdate();

            //給預設值
            if (string.IsNullOrEmpty(FileName))
            {
                if (lvPackage.Items.Count > 0)
                {
                    lvPackage.Items[0].Selected = true;
                    FileName = lvPackage.Items[0].SubItems[1].Text;
                }
                else
                    FileName = "";
            }
            else
            {
                ListViewItem item = FindItemTextWithLV(lvPackage, FileName);
                if (item == null)
                {
                    if (lvPackage.Items.Count > 0)
                    {
                        lvPackage.Items[0].Selected = true;
                        FileName = lvPackage.Items[0].SubItems[1].Text;
                    }
                    else
                        FileName = "";
                }
                else
                {
                    item.Selected = true;
                }
            }
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

            //子表格用
            if ((Tag != null) && (pParent != Tag))
            {
                pParent.Controls.Add((Control)Tag);
                ((Form)Tag).WindowState = FormWindowState.Maximized;
                ((Form)Tag).Show();

                //當子表格返回時，刷新主表格資料
                FormSet.mPackage.RefreshAllData();
            }
            else
            {
                //把 Control Button 設為不可按 解除
                if (FormSet.mMainF1 != null)
                    FormSet.mMainF1.SetControlSW(ControlButtonType.None);
                if (FormSet.mMainF2 != null)
                    FormSet.mMainF2.SetControlSW(ControlButtonType.None);
                if (FormSet.mMainF3 != null)
                    FormSet.mMainF3.SetControlSW(ControlButtonType.None);
            }

            //+ By Max 20191121 For Package Operation in Auto Mode
            if (SYSPara.IsAutoMode)
            {
                //判斷 如果AutoRun 就把設定指回到該PackageName
                SetFileName(FileName_Auto, FolderName_Auto);

                string str = string.Format("機台自動生產中,目前產品設定:{0}", FileName_Auto);
                SYSPara.LogSay("OP", str);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                MessageBox.Show("請選擇產品");
                return;
            }

            //+ By Max 20191121 For Package Operation in Auto Mode
            if (SYSPara.IsAutoMode)
            {
                SYSPara.SysRun = false;
                FormSet.mMSS.M_Stop();
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
            if (SYSPara.IsAutoMode && (FileName_Auto != FileName))
                cansave = false;

            if (cansave)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「儲存」-[{0}設定]", ModuleName));

                DataSnippet.ControlEnable(pnlControl, false);
                DataSnippet.DataSetAll(pnlControl);
                Editable = false;
                PreloadPackageDS.SaveFile();

                AfterSave();

                //+ By Max For SECS
                int idx = SYSPara.OReadValue("CommProtocol").ToInt();
                if (idx == 1) //SECS
                {
                    FormSet.mGemF.PackageOperation(FileName, PPOPState.MODIFY);
                }
            }
            else
                MessageBox.Show("自動操作下，禁止存取非生產產品之設定");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「取消」-[{0}設定]", ModuleName));

            DataSnippet.DataGetAll(pnlControl);
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
                    {
                        File.Create(fname).Close();

                        //+ By Max For SECS
                        int idx = SYSPara.OReadValue("CommProtocol").ToInt();
                        if (idx == 1) //SECS
                        {
                            FormSet.mGemF.PackageOperation(NewFileName, PPOPState.CREATE);
                        }
                    }
                    else
                    {
                        string srcfname = string.Format(@"{0}\{1}.xml", FolderPath, FileName);
                        File.Copy(srcfname, fname);

                        //重設產品日期
                        PreloadPackageDS.Initial(fname, ModuleName);
                        PreloadPackageDS.ResetTime();

                        //+ By Max For SECS
                        int idx = SYSPara.OReadValue("CommProtocol").ToInt();
                        if (idx == 1) //SECS
                        {
                            FormSet.mGemF.PackageOperation(NewFileName, PPOPState.CREATE);
                        }
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
        //將 Package Name 實際寫入到各模組的檔案
        private void AddPackageToFile(string PackageName, string srcPackageName)
        {
            //處理 Stage Module
            foreach (Control control in PackageContainer.Panel2.Controls)
            {
                if (control is DEdit)
                {
                    ((DEdit)control).SetData();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData();
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
                    ((DEdit)control).SetData();
                }
                else if (control is DCheckBox)
                {
                    ((DCheckBox)control).SetData();
                }
                else if (control is DRadioGroupBox)
                {
                    ((DRadioGroupBox)control).SetData();
                }
                else if (control is DFieldEdit)
                {
                    ((DFieldEdit)control).SetData();
                }
                else if (control is DComboBox)
                {
                    ((DComboBox)control).SetData();
                }
                else if (control is DPosEdit)
                {
                    ((DPosEdit)control).SetData();
                }
                else if (control is TFieldCB)
                {
                    ((TFieldCB)control).SetData();
                }
                else if (control is DDataGridView)
                {
                    ((DDataGridView)control).SetData();
                }

                if (control.HasChildren)
                    AddPackageToFileCallBack(control, PackageName, srcPackageName);
            }
        }
        private void lvFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
                FileName = lvPackage.SelectedItems[0].SubItems[1].Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「產品更名」-[產品設定]"));

            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
            {
                string OldName = lvPackage.SelectedItems[0].SubItems[1].Text;
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

            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
            {
                string filename = lvPackage.SelectedItems[0].SubItems[1].Text;

                DialogResult result = MessageBox.Show(string.Format("確定要刪除 {0} 嗎？", filename), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.OK:
                        {
                            if (FileName_Auto == FileName)
                            {
                                MessageBox.Show("無法刪除正在生產的料號！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            string fname = string.Format(@"{0}\{1}.xml", FolderPath, filename);
                            File.Delete(fname);
                            for (int i = 0; i < 5; i++)
                            {
                                fname = string.Format(@"{0}\{1}_{2}.cfg", FolderPath, filename,i+1);
                                File.Delete(fname);
                            }

                            FindPackage = "";

                            //+ By Max For SECS
                            int idx = SYSPara.OReadValue("CommProtocol").ToInt();
                            if (idx == 1) //SECS
                            {
                                FormSet.mGemF.PackageOperation(filename, PPOPState.DELETE);
                            }
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