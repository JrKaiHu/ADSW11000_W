using LogAnalyzer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonObj;

using System.Resources;
using System.Globalization;

namespace ADSW11000
{
    public partial class ReportF : Form
    {
        List<object> TPList = new List<object>();
        List<List<object>>[] datalist = null;
        DataTable tblSPC = new DataTable();

        public ReportF()
        {
            InitializeComponent();

            TPList.Add(tabPage1);
            TPList.Add(tabPage2);
            TPList.Add(tabPage3);
            TPList.Add(tabPage4);
            TPList.Add(tabPage5);
        }

        public void Initial(object mdatalist)
        {
            //將TabControl的檔頭隱藏 ------------------------------------
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            //----------------------------------------------------------
            datalist = (List<List<object>>[])mdatalist;
            
            List<object> header = datalist[6][0]; //第一筆資料是檔頭
            foreach (String name in header)
            {
                tblSPC.Columns.Add(name);
            }
            datalist[6].RemoveAt(0); //將檔頭那一筆資料移除
            SelectShow(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
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

        private void btnLoggerViewer_Click(object sender, EventArgs e)
        {
            var reportData = new ExcelReporter(datalist);
            reportData.OutputDataSummaryAsync();
            //MessageBox.Show("輸出報告完成", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Hint", GetDialogMsgText("Text_ReportF_Output_Report_Finish_Hint"));
        }

        private void SelectShow(int no)
        {
            CheckBox[] btn = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5 };
            foreach (CheckBox obj in btn)
                obj.Checked = false;
            btn[no - 1].Checked = true;

            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add((TabPage)TPList[no - 1]);

            switch (no)
            {
                case 1: //Top 5 Jam
                    dgvLog1.Rows.Clear();
                    foreach (List<object> obj in datalist[1])
                    {
                        dgvLog1.Rows.Add();
                        for (int i = 0; i < 3; i++)
                            dgvLog1.Rows[dgvLog1.Rows.Count - 1].Cells[i].Value = obj[i];
                        dgvLog1.Rows[dgvLog1.Rows.Count - 1].Cells[3].Value = string.Format("{0:0.0}%", Convert.ToDouble(obj[3]) * 100);
                    }
                    break;
                case 2: //Module Alarm
                    dgvLog2A.Rows.Clear();
                    foreach (List<object> obj in datalist[2])
                    {
                        dgvLog2A.Rows.Add();
                        for (int i = 0; i < 2; i++)
                            dgvLog2A.Rows[dgvLog2A.Rows.Count - 1].Cells[i].Value = obj[i];
                        dgvLog2A.Rows[dgvLog2A.Rows.Count - 1].Cells[2].Value = string.Format("{0:0.0}%", Convert.ToDouble(obj[2]) * 100);
                    }

                    dgvLog2.Rows.Clear();
                    foreach (List<object> obj in datalist[3])
                    {
                        dgvLog2.Rows.Add();
                        for (int i = 0; i < 4; i++)
                            dgvLog2.Rows[dgvLog2.Rows.Count - 1].Cells[i].Value = obj[i];
                    }
                    break;
                case 3: //MTBA/MTTR
                    dgvLog3.Rows.Clear();
                    dgvLog3.Columns[0].Name = datalist[4][0][0].ToString();
                    dgvLog3.Columns[1].Name = datalist[4][0][1].ToString();
                    dgvLog3.Columns[2].Name = datalist[4][0][2].ToString();
                    for (int i = 1; i < datalist[4].Count; i++)
                    {
                        dgvLog3.Rows.Add();
                        for (int j = 0; j < 3; j++)
                            dgvLog3.Rows[dgvLog3.Rows.Count - 1].Cells[j].Value = datalist[4][i][j];
                    }
                    break;
                case 4: //嫁動率
                    dgvLog4.Rows.Clear();
                    foreach (List<object> obj in datalist[5])
                    {
                        dgvLog4.Rows.Add();
                        for (int i = 0; i < 8; i++)
                            dgvLog4.Rows[dgvLog4.Rows.Count - 1].Cells[i].Value = obj[i];
                        dgvLog4.Rows[dgvLog4.Rows.Count - 1].Cells[7].Value = string.Format("{0:0.0}%", Convert.ToDouble(obj[7]) * 100);
                    }
                    break;
                case 5: //SPC
                    tblSPC.Rows.Clear();
                    foreach (List<object> obj in datalist[6])
                    {
                        DataRow row = tblSPC.NewRow();
                        for (int i = 0; i < obj.Count; i++)
                            row[i] = obj[i];
                        tblSPC.Rows.Add(row);
                    }
                    dgvLog5.DataSource = tblSPC;
                    break;
            }
        }

        private void checkBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int tag = Convert.ToInt32(((CheckBox)sender).Tag);
            SelectShow(tag);
        }
    }
}
