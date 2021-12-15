using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ProVIFM;
using KCSDK;
using LogAnalyzer;
using CommonObj;

namespace ADSW11000
{
    public partial class ParamTransactionF : Form
    {

        Dictionary<string, string> FileList = new Dictionary<string, string>();

        public ParamTransactionF()
        {
            InitializeComponent();

            dgvLog.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.NonPublic).SetValue(dgvLog, true, null);
        }

        private void InitialTypeList()
        {
            string TablePath = string.Empty;
            string TableName = string.Empty;
            string filename = string.Empty;

            TreeNode SubNode = tvTypeList.Nodes.Add("子表格設定");

            #region 內參設定
            TablePath = string.Format(@"{0}\LocalData\", SYSPara.SysDir);
            TableName = "Setup";
            filename = string.Format(@"{0}\{1}.xml", TablePath, TableName);
            if (File.Exists(filename))
            {
                FileList.Add(TableName, TablePath);
                tvTypeList.Nodes.Add("內參設定");
                InitialSubTableList(TablePath, TableName, SubNode);
            }
            #endregion

            #region 通用設定
            TablePath = string.Format(@"{0}\LocalData\", SYSPara.SysDir);
            TableName = "MachineSet";
            filename = string.Format(@"{0}\{1}.xml", TablePath, TableName);
            if (File.Exists(filename))
            {
                FileList.Add(TableName, TablePath);
                tvTypeList.Nodes.Add("通用設定");
                InitialSubTableList(TablePath, TableName, SubNode);
            }
            #endregion

            #region 模組設定

            TreeNode node = tvTypeList.Nodes.Add("模組設定");
            foreach (BaseModuleInterface module in FormSet.ModuleList)
            {
                TableName = module.Name;
                TablePath = string.Format(@"{0}\Module\{1}", SYSPara.SysDir, TableName);
                filename = string.Format(@"{0}\{1}.xml", TablePath, TableName);
                if (File.Exists(filename))
                {
                    FileList.Add(TableName, TablePath);
                    node.Nodes.Add(TableName);
                    InitialSubTableList(TablePath, TableName, SubNode);
                }
            }
            #endregion

            #region 產品設定
            node = tvTypeList.Nodes.Add("產品設定");
            TablePath = string.Format(@"{0}\LocalData\Package\", SYSPara.SysDir);
            string[] list = Directory.GetFiles(TablePath, "*.xml").Select(file => Path.GetFileName(file)).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                TableName = Path.GetFileNameWithoutExtension(list[i]);

                FileList.Add(TableName, TablePath);
                node.Nodes.Add(TableName);
                InitialSubTableList(TablePath, TableName, SubNode);
            }
            #endregion

            tvTypeList.Nodes.Remove(SubNode);
            tvTypeList.Nodes.Add(SubNode);
        }

        private void InitialSubTableList(string TablePath, string TableName, TreeNode SubNode)
        {
            string fname = string.Format(@"{0}\{1}.xml", TablePath, TableName);
            TempDS.Initial(fname, TableName);

            //子表格
            string s = string.Empty;
            foreach (DataManagement.FieldData field in TempDS.FieldList)
            {
                if (field.Type == "SubPackage")
                {
                    string subfolderpath = field.SubPackage.mTablePath;
                    if (Directory.Exists(subfolderpath))
                    {
                        string[] sublist = Directory.GetFiles(subfolderpath, "*.xml").Select(file => Path.GetFileName(file)).ToArray();
                        for (int j = 0; j < sublist.Length; j++)
                        {
                            s = Path.GetFileNameWithoutExtension(sublist[j]);
                            FileList.Add(s, subfolderpath);
                            SubNode.Nodes.Add(s);
                        }
                    }
                }
            }
        }

        private void ParamTransactionF_Load(object sender, EventArgs e)
        {
            tvTypeList.Nodes.Clear();
            InitialTypeList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            List<string> rulelist = new List<string>();
            foreach (TreeNode node in tvTypeList.Nodes)
            {
                switch (node.Text)
                {
                    case "通用設定":
                        if ((node.Checked) && (FileList.ContainsKey("MachineSet")))
                            rulelist.Add("MachineSet");
                        break;
                    case "內參設定":
                        if ((node.Checked)&&(FileList.ContainsKey("Setup")))
                            rulelist.Add("Setup");
                        break; 
                }
                foreach (TreeNode subnode in node.Nodes)
                {
                    if ((subnode.Checked) && (FileList.ContainsKey(subnode.Text)))
                        rulelist.Add(subnode.Text);
                }
            }

            object start = null;
            object end = null;
            string fieldname = string.Empty;
            if (checkBox1.Checked)
                start = dtp1.Value;
            if (checkBox2.Checked)
                end = dtp2.Value;
            if (checkBox3.Checked)
                fieldname = textBox1.Text;
            List<DataModifyShowData> list = SYSPara.logDB.QueryDataModifyLog(start, end, rulelist, fieldname);
            
            dgvLog.DataSource = null;
            dgvLog.Rows.Clear();
            dgvLog.Columns.Clear();
            dgvLog.DataSource = list;
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (dgvLog.SelectedCells.Count > 0)
            {
                int colindex = dgvLog.SelectedCells[0].ColumnIndex;
                int rowindex = dgvLog.SelectedCells[0].RowIndex;

                DateTime restoretime = (DateTime)dgvLog.Rows[rowindex].Cells[0].Value;
                restoretime = restoretime.AddMilliseconds(1);
                string typename = dgvLog.Rows[rowindex].Cells[1].Value.ToString();
                string fieldname = dgvLog.Rows[rowindex].Cells[2].Value.ToString();
                DateSelectF frm = new DateSelectF();
                if (colindex < 2)
                    frm.Initial(0, restoretime, typename);
                else
                    frm.Initial(1, restoretime, typename, fieldname);

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    bool restoreresult = false;
                    if (colindex < 2)
                        restoreresult = RestoreParam(restoretime, typename, string.Empty);
                    else
                        restoreresult = RestoreParam(restoretime, typename, fieldname);

                    if (restoreresult)
                    {
                        //MessageBox.Show("還原成功");
                        dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", "還原成功");
                    }
                    else
                    {
                        //MessageBox.Show("還原失敗");
                        dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", "還原失敗");
                    }

                }
            }
        }

        //還原指定類型所有的欄位值至指定日期前
        private bool RestoreParam(DateTime restoretime, string typename, string fieldname)
        {
            string TablePath = "";
            string TableName = typename;
            bool findit = false;
            bool result = true;
            #region 搜尋TablePath & TableName
            if (FileList.ContainsKey(typename))
            {
                if (File.Exists(string.Format(@"{0}\{1}.xml", FileList[typename], typename)))
                {
                    TablePath = FileList[typename];
                    findit = true;
                }
            }
            #endregion

            if (findit)
            {
                try
                {
                    //載入參數異動資料                
                    List<string> rulelist = new List<string>();
                    rulelist.Add(TableName);
                    List<DataModifyData> list = SYSPara.logDB.QueryDataModifyFullLog(restoretime, DateTime.Now, rulelist, fieldname);

                    if (list.Count > 0)
                    {
                        //將設定檔讀入
                        string fname = string.Format(@"{0}\{1}.xml", TablePath, TableName);
                        TempDS.Initial(fname, TableName);

                        TempDS.ModifiedLogToDB = true;
                        TempDS.ModifiedLog = true;

                        //還原至指定時間前的資料
                        for (int i = 0; i < list.Count; i++)
                        {
                            switch (list[i].ValueType)
                            {
                                case "Table":
                                    {
                                        #region 處理Table資料
                                        if (!string.IsNullOrEmpty(list[i].OldValue))
                                        {
                                            DataTable table = TempDS.GetData(list[i].DataName);
                                            table.Columns.Clear();
                                            table.Rows.Clear();
                                            string[] rowlist = list[i].OldValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (rowlist.Length > 0)
                                            {
                                                string[] collist = rowlist[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                                //建立欄位
                                                for (int j = 0; j < collist.Length; j++)
                                                    table.Columns.Add(collist[j]);

                                                for (int j = 1; j < rowlist.Length; j++)
                                                {
                                                    collist = rowlist[j].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                    DataRow row = table.NewRow();
                                                    for (int k = 0; k < collist.Length; k++)
                                                        row[k] = collist[k];
                                                    table.Rows.Add(row);
                                                }
                                            }
                                            TempDS.SetData(list[i].DataName, "Table", table);
                                        }
                                        #endregion
                                    }
                                    break;
                                case "SubPackage":
                                    {
                                        #region 處理 SubPackage 資料
                                        string[] ss = list[i].OldValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (ss.Length == 2)
                                            TempDS.SetSubData(list[i].DataName, "SubPackage", ss[0], ss[1]);
                                        #endregion
                                    }
                                    break;
                                default:
                                    TempDS.SetData(list[i].DataName, list[i].ValueType, list[i].OldValue);
                                    break;
                            }
                            //刪掉處理完的欄位
                            list.RemoveAll(data => data.DataName == list[i].DataName);
                            i = -1;
                        }

                        TempDS.SaveFile();

                        TempDS.ModifiedLogToDB = false;
                        TempDS.ModifiedLog = false;
                    }
                }
                catch (Exception) { result = false; }
            }
            return result;
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            tvTypeList.ExpandAll();
        }

        private void btnCollapse_Click(object sender, EventArgs e)
        {
            tvTypeList.CollapseAll();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvTypeList.Nodes)
            {
                node.Checked = true;
                foreach (TreeNode subnode in node.Nodes)
                    subnode.Checked = true;
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvTypeList.Nodes)
            {
                node.Checked = false;
                foreach (TreeNode subnode in node.Nodes)
                    subnode.Checked = false;
            }
        }
    }
}