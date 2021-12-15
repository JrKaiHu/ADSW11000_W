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
using CommonObj;

using System.Globalization;
using System.Resources;


#region copy用
#endregion copy用
namespace ADSW11000
{
    public partial class BasePackageExF : Form
    {
        #region 基本

        public BasePackageExF()
        {
            InitializeComponent();

            #region 基本_架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            lvPackage.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvPackage, true, null);

            lvFolder.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(lvFolder, true, null);

            #endregion 基本_架構使用
        }

        #endregion 基本

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

        #region 架構使用 (Public)

        //Scofield_v4.0.1.13[新增]新版料片規格設定
        public void AddRowNoNum(DDataGridView obj)
        {
            obj.Rows.Add();
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
            if (dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Warning", 
                GetDialogMsgText("Text_BasePackageExf_ClearAll_Warning")) == DialogResult.OK)
            {
                obj.Rows.Clear();
            }

            //if (MessageBox.Show("您確定要清除所有資料嗎 ?", "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            //{
            //    obj.Rows.Clear();
            //}
        }

        public void UpMove(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
            {
                if (obj.SelectedCells[0].RowIndex > 0)
                {
                    object oValue;
                    //Scofield_v4.0.1.13[修正]DataView的上下移Button第一格不會移動問題
                    for (int i = 0; i < obj.Columns.Count; i++)
                    //for (int i = 1; i < obj.Columns.Count; i++)
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
                    //Scofield_v4.0.1.12[修正]DataView的上下移Button第一格不會移動問題
                    for (int i = 0; i < obj.Columns.Count; i++)
                    //for (int i = 1; i < obj.Columns.Count; i++)
                    {
                        oValue = obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex].Cells[i].Value = obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[i].Value;
                        obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[i].Value = oValue;
                    }
                    obj.Rows[obj.SelectedCells[0].RowIndex + 1].Cells[obj.SelectedCells[0].ColumnIndex].Selected = true;
                }
            }
        }
        //Scofield_v4.0.1.13[新增]新版料片規格設定
        public void InsertRowNoNum(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
            {
                obj.Rows.Insert(obj.SelectedCells[0].RowIndex);
                //obj.Rows[obj.SelectedCells[0].RowIndex].Cells[0].Value = obj.SelectedCells[0].RowIndex;
            }
        }

        public void InsertRow(DDataGridView obj)
        {
            if (obj.SelectedCells.Count == 1)
            {
                obj.Rows.Insert(obj.SelectedCells[0].RowIndex);
                //obj.Rows[obj.SelectedCells[0].RowIndex].Cells[0].Value = obj.SelectedCells[0].RowIndex;
            }
        }

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

        public bool Editable = false;

        public void SetFileName(string filename, string foldername, bool IsAuto = false)
        {
            string fname = string.Format(@"{0}\{1}\{2}.xml", FolderPath, foldername, filename);
            if (File.Exists(fname))
            {
                PackageDS.Initial(fname, ModuleName);
                PreloadPackageDS.Initial(fname, ModuleName);
                mFileName = filename;
                mFolderName = foldername;

                if (IsAuto)
                {
                    FileName_Auto = mFileName;
                    FolderName_Auto = mFolderName;
                    lvPackage.FixItem = mFileName;
                    lvFolder.FixItem = mFolderName;
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

        public virtual void SelectChange()
        {
        }

        public virtual void AfterSave()
        {
        }

        public void Initial()
        {
            FindPackage = "";
            RefreshFolder();

            if (SYSPara.RunMode == RunModeDT.AUTO)
            {
                FolderName = FolderName_Auto;
                FileName = FileName_Auto;

                lvPackage.FixItem = FileName_Auto;
                lvFolder.FixItem = FolderName_Auto;
            }
            else
            {
                FolderName = FolderName;
                FileName = FileName;

                lvPackage.FixItem = "";
                lvFolder.FixItem = "";
            }
            InitialSubPackage();
        }

        public void AutoEnd()
        {
            lvFolder.FixItem = "";
            lvPackage.FixItem = "";
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

        #endregion 架構使用 (Public)

        #region 架構使用 (Private)

        private DataManagement m_PackageDS = new DataManagement();

        #region 架構使用 (Private)_將 Package Name 實際寫入到各模組的檔案

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

        #endregion 架構使用 (Private)_將 Package Name 實際寫入到各模組的檔案

        #region 架構使用 (Private)_更新Package內全部的元件資料

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

        private void RefreshFolder()
        {
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
                {
                    lvFolder.Items[0].Selected = true;
                    FolderName = lvFolder.Items[0].SubItems[1].Text;
                }
                else
                    FolderName = "";
            }
            else
            {
                ListViewItem item = FindItemTextWithLV(lvFolder, FolderName);
                if (item == null)
                {
                    if (lvFolder.Items.Count > 0)
                    {
                        lvFolder.Items[0].Selected = true;
                        FolderName = lvFolder.Items[0].SubItems[1].Text;
                    }
                    else
                        FolderName = "";
                }
                else
                {
                    item.Selected = true;
                }
            }
        }

        private void RefreshFile()
        {
            string path = string.Format(@"{0}\{1}\", FolderPath, FolderName);

            string[] list = Directory.GetFiles(path, "*.xml")
                .Select(file => Path.GetFileName(file)).ToArray();

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

        #endregion 架構使用 (Private)_架構使用 (Private)_更新Package內全部的元件資料

        #endregion 架構使用 (Private)

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

                string fname = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, FileName);
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

        #endregion 屬性

        #region 事件

        private void button3_Click(object sender, EventArgs e)
        {
            string NewFolderName = "NoName";
            if (UserSnippet.InputBox("Create Body Size", "Body Size :", ref NewFolderName) == DialogResult.OK)
            {
                try
                {
                    if (NewFolderName == string.Empty)
                    {
                        //MessageBox.Show("請輸入產品尺寸");
                        dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_BasePackageExf_Input_Size_Hint"));
                        return;
                    }
                }
                catch (Exception)
                {
                }

                string path = FolderPath + @"\" + NewFolderName;
                if (!Directory.Exists(path))
                {
                    DirectoryInfo info = Directory.CreateDirectory(path);
                    Thread.Sleep(200);
                    if (info.Exists)
                    {
                        mFolderName = NewFolderName;
                        RefreshFolder();
                        //FolderPath = FolderPath;
                        //FolderName = NewFolderName;
                    }
                    else
                    {
                        //MessageBox.Show(string.Format("{0} 建立失敗", NewFolderName));
                        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "",
                            string.Format("{0} {1}", NewFolderName, GetDialogMsgText("Text_BasePackageExf_Create_Fail")));
                    }
                }
                else
                {
                    //MessageBox.Show(string.Format("{0} 名稱重覆", NewFolderName));
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "",
                        string.Format("{0} {1}", GetDialogMsgText("Text_BasePackageExf_Repeated_Name_Fail")));
                }
            }
        }

        private void lvFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
                FolderName = lvFolder.SelectedItems[0].SubItems[1].Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「產品更名」-[產品設定]"));

            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
            {
                string OldName = lvFolder.SelectedItems[0].SubItems[1].Text;
                string NewName = "";
                if (UserSnippet.InputBox("Body Size Change Name", "new Name", ref NewName) == DialogResult.OK)
                {
                    string oldpath = FolderPath + @"\" + OldName;
                    string newpath = FolderPath + @"\" + NewName;

                    if (!Directory.Exists(newpath))
                    {
                        Directory.Move(oldpath, newpath);
                        Thread.Sleep(200);

                        mFolderName = NewName;
                        RefreshFolder();
                    }
                    else
                        //MessageBox.Show("產品尺寸重覆");
                        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_BasePackageExf_Repeated_Size_Fail"));
                }
            }
            else
                //MessageBox.Show("請選擇一個產品尺寸");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_BasePackageExf_Slect_Size_Hint"));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「產品刪除」-[產品設定]");

            if ((lvFolder.SelectedItems != null) && (lvFolder.SelectedItems.Count == 1))
            {
                string foldername = lvFolder.SelectedItems[0].SubItems[1].Text;

                //DialogResult result = MessageBox.Show(string.Format("確定要刪除 {0} 嗎？", foldername), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                DialogResult result = dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "Warning",
                    string.Format(GetDialogMsgText("Text_BasePackageExf_Delete_Package_Check"), foldername));
                switch (result)
                {
                    case DialogResult.OK:
                        Directory.Delete(FolderPath + @"\" + foldername, true);
                        Thread.Sleep(200);

                        mFolderName = "";
                        RefreshFolder();
                        //FolderPath = FolderPath;
                        break;
                    case DialogResult.Cancel:
                        break;
                }
            }
            else
            {
                //MessageBox.Show("請選擇一個產品尺寸");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_BasePackageExf_Slect_Size_Hint"));
            }
        }

        private void lvPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((lvPackage.SelectedItems != null) && (lvPackage.SelectedItems.Count == 1))
                FileName = lvPackage.SelectedItems[0].SubItems[1].Text;
        }

        private void tbPackage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindPackage = tbPackage.Text;
        }

        #region 事件_架構使用 (按鈕功能)

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

                string str = string.Format("機台自動生產中,目前產品設定:{0}_{1}", FolderName_Auto, FileName_Auto);
                SYSPara.LogSay("OP", str);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「新增產品」-[{0}設定]", ModuleName));

            string NewFileName = "NoName";
            if (UserSnippet.InputBox(string.Format("Create {0}", ModuleName), string.Format("{0} Name:", ModuleName), ref NewFileName) == DialogResult.OK)
            {
                try
                {
                    if (NewFileName == string.Empty)
                    {
                        //MessageBox.Show(string.Format("請輸入 {0} 名稱", ModuleName));
                        dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", string.Format(GetDialogMsgText("Text_BasePackageExf_Input_Package_Name_Hint"), ModuleName));
                        return;
                    }
                }
                catch (Exception)
                {
                }

                string fname = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, NewFileName);
                if (!File.Exists(fname))
                {
                    if (string.IsNullOrEmpty(FileName))
                        File.Create(fname).Close();
                    else
                    {
                        string srcfname = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, FileName);
                        if (File.Exists(srcfname))
                            File.Copy(srcfname, fname);
                        else
                            File.Create(fname).Close();
                    }

                    if (!File.Exists(fname))
                        //MessageBox.Show(string.Format("{0} 建立失敗", NewFileName));
                        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", string.Format("{0} 建立失敗", NewFileName));
                    else
                    {
                        FindPackage = "";

                        //重設產品日期
                        PreloadPackageDS.Initial(fname, ModuleName);
                        PreloadPackageDS.ResetTime();
                        //v4.0.1.39 視覺料號重設
                        PreloadPackageDS.SetData("視覺軟體程式名稱", "String", FolderName + "_" + NewFileName);
                        PreloadPackageDS.SaveFile();
                        mFileName = NewFileName;
                        RefreshFile();
                        //FileName = NewFileName;
                    }
                }
                else
                {
                    //MessageBox.Show(string.Format("{0} 名稱重覆", NewFileName));
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "",
                        string.Format("{0} {1}", GetDialogMsgText("Text_BasePackageExf_Repeated_Name_Fail")));
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                //MessageBox.Show("請選擇產品");
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_BasePackageExf_Select_Package_Hint"));
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
                //MessageBox.Show("請選擇產品");
                dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", GetDialogMsgText("Text_BasePackageExf_Select_Package_Hint"));
                return;
            }

            bool cansave = true;
            if ((SYSPara.IsAutoMode) && ((FileName_Auto != FileName) || (FolderName_Auto != FolderName)))
                cansave = false;

            if (cansave)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「儲存」-[{0}設定]", ModuleName));

                DataSnippet.ControlEnable(pnlControl, false);
                DataSnippet.DataSetAll(pnlControl);
                Editable = false;
                PreloadPackageDS.SaveFile();

                SetFileName(FileName, FolderName);
                InitialSubPackage();
                AfterSave();
            }
            else
                //MessageBox.Show("自動操作下，禁止存取非生產產品之設定");
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", GetDialogMsgText("Text_BasePackageExf_Prohibit_Access_Running_Package"));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「取消」-[{0}設定]", ModuleName));

            DataSnippet.DataGetAll(pnlControl);
            DataSnippet.ControlEnable(pnlControl, false);
            Editable = false;
        }

        private void btnChangeName_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「產品更名」-[{0}設定]", ModuleName));

            string NowSelPackage = null;
            if (lvPackage.SelectedItems.Count != 0)
                NowSelPackage = lvPackage.SelectItem;//4.0.1.23
            string NewName = NowSelPackage;
            if (UserSnippet.InputBox(string.Format("Change {0} to new name", ModuleName), string.Format("New {0} Name:", ModuleName), ref NewName) == DialogResult.OK)
            {
                if (NewName != NowSelPackage)
                {
                    NewName = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, NewName);
                    NowSelPackage = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, NowSelPackage);
                    File.Copy(NowSelPackage, NewName);
                    //+ By Max 20210205
                    File.Delete(NowSelPackage);
                    PreloadPackageDS.Initial(NewName, ModuleName);
                    mFileName = NewName;
                    RefreshFile();
                }
                //PreloadPackageDS.ChangePackageName(NowSelPackage, NewName);
                //PreloadPackageDS.SaveFile();
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, string.Format("使用者按下「產品刪除」-[{0}設定]", ModuleName));

            string NowSelPackage = null;
            if (lvPackage.SelectedItems.Count != 0)
                NowSelPackage = lvPackage.SelectedItems[0].SubItems[1].Text;

            //DialogResult result = MessageBox.Show(string.Format("確定要刪除 {0} 嗎？", NowSelPackage), "注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            DialogResult result = dia_Message.Instance.ShowDialog(enMsgDialogType.WARNING, "注意", string.Format("確定要刪除 {0} 嗎？", NowSelPackage));
            switch (result)
            {
                case DialogResult.OK:
                    NowSelPackage = string.Format(@"{0}\{1}\{2}.xml", FolderPath, FolderName, NowSelPackage);
                    if (File.Exists(NowSelPackage))
                    {
                        File.Delete(NowSelPackage);

                        mFileName = "";
                        RefreshFile();
                    }
                    //PreloadPackageDS.DeletePackage(NowSelPackage);
                    //PreloadPackageDS.SaveFile();
                    break;
                case DialogResult.Cancel:
                    break;
            }
        }

        private void PackageF_Load(object sender, EventArgs e)
        {
            PackageContainer.SplitterDistance = 225;
            DataSnippet.ControlEnable(pnlControl, false);
        }


        #endregion 事件_架構使用 (按鈕功能)

        #endregion 事件
    }
}