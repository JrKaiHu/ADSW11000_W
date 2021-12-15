using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSW11000
{
    public partial class PMSettingF : Form
    {
        private DataSet dsPM = new DataSet("PMSet");
        private bool m_bEditMode = true;

        private bool m_HasPMItem = false;
        public bool HasPMItem
        {
            get { return m_HasPMItem; }
        }

        DataView dv = null;
        public PMSettingF(bool bEditMode)
        {
            InitializeComponent();

            PMActionList.Add("完成保養");
            PMActionList.Add("跳過此次");
            PMActionList.Add("週期重置");

            m_bEditMode = bEditMode;
            if (m_bEditMode)
            {
                lblTitle.Text = "預防保養設定";
                btnAdd.Visible = true;
                btnIgnore.Visible = false;
                btnSave.Visible = true;
                btnClose.Visible = true;
                btnClose.Text = "關閉";
                dgvPM.AllowUserToDeleteRows = true;
            }
            else
            {
                lblTitle.Text = "預防保養提示";
                btnAdd.Visible = false;
                btnIgnore.Visible = true;
                btnSave.Visible = false;
                btnClose.Visible = true;
                btnClose.Text = "停止開批";
                dgvPM.AllowUserToDeleteRows = false;
            }

            DataTable dt = new DataTable("ItemDT");
            DataColumn col1 = new DataColumn("保養項目");
            dt.Columns.Add(col1);
            col1 = new DataColumn("保養週期");
            dt.Columns.Add(col1);
            col1 = new DataColumn("到期日", System.Type.GetType("System.DateTime"));
            dt.Columns.Add(col1);
            col1 = new DataColumn("需保養", System.Type.GetType("System.Boolean"));
            dt.Columns.Add(col1);
            col1 = new DataColumn("保養狀態", System.Type.GetType("System.String"));
            dt.Columns.Add(col1);
            dsPM.Tables.Add(dt);

            if (System.IO.File.Exists("PMSetting.xml"))
            {
                dsPM.Clear();
                dsPM.ReadXml("PMSetting.xml");
                dv = new DataView(dsPM.Tables["ItemDT"]);

                if (m_bEditMode)
                {
                    dgvPM.DataSource = dv;
                    dgvPM.Columns[0].ReadOnly = true;
                    dgvPM.Columns[1].ReadOnly = true;
                    dgvPM.Columns[2].ReadOnly = true;
                    dgvPM.Columns[3].Visible = false;
                    dgvPM.Columns[4].Visible = false;

                    DataGridViewComboBoxColumn pmActionComboBox = Create_ComboBox();
                    dgvPM.Columns.Add(pmActionComboBox);

                    for (int i = 0; i < dsPM.Tables["ItemDT"].Rows.Count; ++i)
                    {
                        dgvPM.Rows[i].Cells[5].Value = "跳過此次";
                    }

                }
                else
                {
                    DateTime dtNow = DateTime.Now;

                    for (int i = 0; i < dsPM.Tables["ItemDT"].Rows.Count; ++i)
                    {
                        dsPM.Tables["ItemDT"].Rows[i]["需保養"] = false;
                        DateTime d = (DateTime)dsPM.Tables["ItemDT"].Rows[i][2];
                        if (d.Subtract(dtNow).Ticks < 0)
                        {
                            dsPM.Tables["ItemDT"].Rows[i]["需保養"] = true;
                            m_HasPMItem = true;
                        }
                    }

                    dv.RowFilter = "需保養 = 'true'";

                    dgvPM.DataSource = dv;
                    dgvPM.Columns[0].ReadOnly = true;
                    dgvPM.Columns[1].ReadOnly = true;
                    dgvPM.Columns[2].ReadOnly = true;
                    dgvPM.Columns[3].Visible = false;
                    dgvPM.Columns[4].Visible = false;
                   
                }
            }
            
        }

        List<String> PMActionList = new List<string>();
        public DataGridViewComboBoxColumn Create_ComboBox()
        {
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.Name = "狀態";
            col.DataSource = PMActionList;
            col.HeaderText = "保養狀態";
            col.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;

            return col;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PMItem Item = new PMItem();
            PMItemF pmItem = new PMItemF(ref Item);

            bool bNewAddItem = false;
            if (dsPM.Tables["ItemDT"].Rows.Count == 0)
                bNewAddItem = true;

            if (pmItem.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataRow row = dsPM.Tables["ItemDT"].NewRow();
                row["保養項目"] = Item.ItemName;
                DateTime dt = Item.PMDate;// DateTime.Now;
                
                switch (Item.PMCycle)
                {
                    case ePMCycle.eSeason:
                        {
                            row["保養週期"] = "季保養";
                            dt = dt.AddMonths(3);
                        }
                        break;
                    case ePMCycle.eSemiAnnually:
                        {
                            row["保養週期"] = "半年保養";
                            dt = dt.AddMonths(6);
                        }
                        break;
                    case ePMCycle.eAnnually:
                        {
                            row["保養週期"] = "年度保養";
                            dt = dt.AddMonths(12);
                        }
                        break;
                }

                row["到期日"] = dt.ToString();
                row["需保養"] = false;
                row["保養狀態"] = "新增保養";
                dsPM.Tables["ItemDT"].Rows.Add(row);
            }

            if (bNewAddItem)
            {
                dv = new DataView(dsPM.Tables["ItemDT"]);
                dgvPM.DataSource = dv;
                dgvPM.Columns[0].ReadOnly = true;
                dgvPM.Columns[1].ReadOnly = true;
                dgvPM.Columns[2].ReadOnly = true;
                dgvPM.Columns[3].Visible = false;
                dgvPM.Columns[4].Visible = false;
                DataGridViewComboBoxColumn pmActionComboBox = Create_ComboBox();
                dgvPM.Columns.Add(pmActionComboBox);

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            String sLoginUser = String.Empty;

            if (SYSPara.LoginMode == 0)
            {
                FormSet.mUserLoginEx = new UserLoginEx(true);
                DialogResult result = FormSet.mUserLoginEx.ShowDialog();
                sLoginUser = FormSet.mUserLoginEx.CurrentUser;
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                sLoginUser = FormSet.mUserLogin.CurrentUser;
            }

            for (int i = 0; i < dgvPM.Rows.Count; ++i)
            {
                if (String.IsNullOrEmpty((String)dgvPM.Rows[i].Cells["狀態"].Value) && !(String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "新增保養") == 0))
                {
                    dgvPM.Rows[i].Cells["保養狀態"].Value = "未設定";
                }
            }

            for (int i = 0; i < dgvPM.Rows.Count; ++i)
            {
                if (String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "完成保養") == 0)
                {
                    String strLog = String.Format("使用者[{0}]完成[{1}]的預防保養項目", sLoginUser, (String)dgvPM.Rows[i].Cells["保養項目"].Value);
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                    DateTime dt = DateTime.Now;

                    if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "季保養")
                    {
                        dt = dt.AddMonths(3);
                    }
                    else if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "半年保養")
                    {
                        dt = dt.AddMonths(6);
                    }
                    else
                    {
                        dt = dt.AddMonths(12);
                    }

                    dgvPM.Rows[i].Cells["到期日"].Value = dt.ToString();
                }
                else if (String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "跳過此次") == 0)
                {
                    String strLog = String.Format("使用者[{0}]跳過[{1}]的預防保養項目", sLoginUser, (String)dgvPM.Rows[i].Cells["保養項目"].Value);
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                    DateTime dt = DateTime.Now;

                    if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "季保養")
                    {
                        dt = dt.AddMonths(3);
                    }
                    else if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "半年保養")
                    {
                        dt = dt.AddMonths(6);
                    }
                    else
                    {
                        dt = dt.AddMonths(12);
                    }

                    dgvPM.Rows[i].Cells["到期日"].Value = dt.ToString();
                }
                else if (String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "週期重置") == 0)
                {
                    String strLog = String.Format("使用者[{0}]重置[{1}]的預防保養項目", sLoginUser, (String)dgvPM.Rows[i].Cells["保養項目"].Value);
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                    DateTime dt = dtReset;

                    if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "季保養")
                    {
                        dt = dt.AddMonths(3);
                    }
                    else if (dgvPM.Rows[i].Cells["保養週期"].Value.ToString() == "半年保養")
                    {
                        dt = dt.AddMonths(6);
                    }
                    else
                    {
                        dt = dt.AddMonths(12);
                    }

                    dgvPM.Rows[i].Cells["到期日"].Value = dt.ToString();
                }
                else if (String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "新增保養") == 0)
                {
                    String strLog = String.Format("使用者[{0}]新增[{1}]的預防保養項目", sLoginUser, (String)dgvPM.Rows[i].Cells["保養項目"].Value);
                    SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                    dgvPM.Rows[i].Cells["保養狀態"].Value = "完成保養";
                }
                //else if (String.Compare((String)dgvPM.Rows[i].Cells["保養狀態"].Value, "未設定") == 0)
                //{
                //    String strLog = String.Format("[{0}]的預防保養項目未設定保養狀態!", (String)dgvPM.Rows[i].Cells["保養項目"].Value);
                //    SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                //}
            }

            for (int i = 0; i < dsPM.Tables["ItemDT"].Rows.Count; ++i)
            {
                dsPM.Tables["ItemDT"].Rows[i]["需保養"] = false;
            }

            dsPM.AcceptChanges();
            dsPM.WriteXml("PMSetting.xml", XmlWriteMode.WriteSchema);
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下預防保養設定畫面「儲存」按鈕");
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (m_bEditMode)
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下預防保養設定畫面「關閉」按鈕");
            }
            else
            {
                SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下預防保養畫面「停止開批」按鈕");
            }
            this.Close();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            if (SYSPara.LoginUser != UserType.utNone)
            {
                String sLoginUser = String.Empty;

                if (SYSPara.LoginMode == 0)
                {
                    FormSet.mUserLoginEx = new UserLoginEx(true);
                    DialogResult result = FormSet.mUserLoginEx.ShowDialog();
                    sLoginUser = FormSet.mUserLoginEx.CurrentUser;
                }
                else
                {
                    sLoginUser = FormSet.mUserLogin.CurrentUser;
                }

                String strLog = String.Format("使用者[{0}]按下預防保養提示畫面「忽略提示」按鈕", sLoginUser);
                SYSPara.LogSay(EnLoggerType.EnLog_OP, strLog);
                dsPM.AcceptChanges();
                dsPM.WriteXml("PMSetting.xml", XmlWriteMode.WriteSchema);
            }
            this.Close();
        }

        private void dgvPM_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            String sLoginUser = String.Empty;

            if (SYSPara.LoginMode == 0)
            {
                FormSet.mUserLoginEx = new UserLoginEx(true);
                DialogResult result = FormSet.mUserLoginEx.ShowDialog();
                sLoginUser = FormSet.mUserLoginEx.CurrentUser;
            }
            else
            {
                sLoginUser = FormSet.mUserLogin.CurrentUser;
            }

            String strAdd = String.Format("使用者[{0}]刪除[{1}]的預防保養項目", sLoginUser, e.Row.Cells["保養項目"].Value);
            SYSPara.LogSay(EnLoggerType.EnLog_OP, strAdd);
        }

        private void dgvPM_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //DataGridView dgv = (DataGridView)sender;
            if (e.Control is ComboBox)
            {
                ComboBox cb = e.Control as ComboBox;
                if (cb != null)
                {
                    //給予初始選項
                    cb.SelectedIndex = 0;
                    //自己寫ComboBox選項改變的事件註冊
                    cb.SelectedIndexChanged += AutoChangedMethodNameComboBoxSelectionIndexChanged;
                    //自己寫ComboBox離開Focus的時候要取消其他事件註冊
                    cb.Leave += AutoChangedMethodNameComboBoxLeave;
                }
            }
        }

        private DateTime dtReset = DateTime.Now;
        private void AutoChangedMethodNameComboBoxSelectionIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int selectedindex = cb.SelectedIndex;
            if (selectedindex > -1)
            {
                string selectedMethodName = PMActionList[selectedindex];

                DataGridViewComboBoxEditingControl dgvcb = (DataGridViewComboBoxEditingControl)sender;
                int rowidx = dgvcb.EditingControlRowIndex;
                dgvPM.Rows[rowidx].Cells["保養狀態"].Value = selectedMethodName;
                if (String.Compare(selectedMethodName, "週期重置") == 0)
                {
                    PMItem Item = new PMItem();
                    Item.ItemName = dgvPM.Rows[rowidx].Cells["保養項目"].Value.ToString();
                    PMItemF pmItem = new PMItemF(ref Item, true);
                    pmItem.ShowDialog();
                    dtReset = Item.PMDate;
                }
            }
        }

        private void AutoChangedMethodNameComboBoxLeave(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            cb.SelectedIndexChanged -= AutoChangedMethodNameComboBoxSelectionIndexChanged;
        }

        private void dgvPM_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //cb.DroppedDown=true搭配將DGV的EditMode改成EditOnEnter可以實現單點就展開ComboBox
            if (dgvPM.Rows.Count <= e.RowIndex) return;
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex < 0) return;
            if (dgvPM.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
            {
                dgvPM.BeginEdit(true);
                ((ComboBox)dgvPM.EditingControl).DroppedDown = true;
            }
        }


    }

    public enum ePMCycle
    {
        eSeason = 0,
        eSemiAnnually,
        eAnnually
    }

    public class PMItem
    {
        public String ItemName
        {
            get;
            set;
        }

        public ePMCycle PMCycle
        {
            get;
            set;
        }

        public DateTime PMDate
        {
            get;
            set;
        }
    }
}
