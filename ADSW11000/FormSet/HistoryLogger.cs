using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProVLib;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ADSW11000
{
    public partial class HistoryLogger : Form
    {
        private List<string> TypeList = new List<string>();
        private DataTable dt = new DataTable("DTLog");
        private object locker = new object();
        System.Timers.Timer FlusherTM = null;

        public HistoryLogger()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            //+By Max 2019/09/11
            LoggerFactory.Logger.Add(null, EnLoggerType.EnLog_OP, "日期\t時間\t類型\t內容");

            //+ By Max 20180424
            AddTitle(EnLoggerType.EnLog_OP, "日期\t時間\t類型\t料號\t批號\t內容");

            TypeList.Clear();
            TypeList.Add("All Type");

            dgvLog.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic).SetValue(dgvLog, true, null);

            FlusherTM = new System.Timers.Timer(1);
            FlusherTM.Elapsed += new System.Timers.ElapsedEventHandler(TimerEventProcessor);
            FlusherTM.Enabled = true;
        }

        public void InitialLogger()
        {
            int idx = comboBox1.SelectedIndex;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(TypeList.ToArray());
            if (idx >= 0 && idx < comboBox1.Items.Count)
                comboBox1.SelectedIndex = idx;
            else
                comboBox1.SelectedIndex = 0;


            //讀取OEE Report輸出參數設定
            LogAnalyzer.ReportConfig.FileDir = SYSPara.SetupReadValue("ReportFileDir").ToString();
            LogAnalyzer.ReportConfig.LoadHour = SYSPara.SetupReadValue("EQPLoadTime").ToInt();
            LogAnalyzer.ReportConfig.MeanTimeType = (LogAnalyzer.ReportConfig.MeanTimeTypes)SYSPara.SetupReadValue("MeanTimeType").ToInt();

            DataTable table = SYSPara.SetupReadTable("SPCSetting");
            //+ By Max 20210218, 防止內參設定中無資料
            if (table == null)
                return;
            int index = 0;
            foreach (DataRow row in table.Rows)
            {
                String SPCName = Convert.ToString(row["colName"]);
                String SPCType = Convert.ToString(row["colType"]);
                LogAnalyzer.ReportConfig.SpcColumns[index].Name = SPCName;
                LogAnalyzer.ReportConfig.SpcColumns[index].CalcType = SPCType == "Sum" ? LogAnalyzer.SpcColumn.CalcTypes.Sum : LogAnalyzer.SpcColumn.CalcTypes.Average;
                index++;
            }
        }

        public void AddTitle(EnLoggerType type, string title)
        {
            string fname = LoggerFactory.Logger.GetLogFileName();
            List<string> dlist = new List<string>();
            dlist.Clear();

            /*
            if (File.Exists(fname))
            {
                using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader r = new StreamReader(fs, Encoding.Default))
                    {
                        string line = "";
                        while ((line = r.ReadLine()) != null)
                        {
                            dlist.Add(line);
                        }
                    }
                    fs.Close();
                }

                if (dlist[0] != title)
                {
                    File.Delete(fname);
                    dlist.Insert(0, title);
                    for (int i = 0; i < dlist.Count; i++)
                    {
                        String hMsg = dlist[i] + Environment.NewLine;
                        File.AppendAllText(fname, hMsg, System.Text.ASCIIEncoding.Default);
                    }
                }
            }
            else
            {
                String hMsg = title + Environment.NewLine;
                File.AppendAllText(fname, hMsg, System.Text.ASCIIEncoding.Default);
            }
             */

            DataColumn dc = null;
            dc = new DataColumn("日期");
            dt.Columns.Add(dc);
            dc = new DataColumn("時間");
            dt.Columns.Add(dc);
            dc = new DataColumn("類型");
            dt.Columns.Add(dc);

            dc = new DataColumn("料號");
            dt.Columns.Add(dc);
            dc = new DataColumn("批號");
            dt.Columns.Add(dc);

            dc = new DataColumn("內容");
            dt.Columns.Add(dc);
            dc = new DataColumn("錯誤代碼");
            dt.Columns.Add(dc);
            dc = new DataColumn("模組");
            dt.Columns.Add(dc);
            dc = new DataColumn("發生日期");
            dt.Columns.Add(dc);
            dc = new DataColumn("發生時間");
            dt.Columns.Add(dc);
            dc = new DataColumn("持續時間 (秒)");
            dt.Columns.Add(dc);
        }

        public void LogSay(EnLoggerType mType, string mMsg)
        {
            string stype;
            switch (mType)
            {
                case EnLoggerType.EnLog_ARM:
                    stype = "Alarm";
                    break;
                case EnLoggerType.EnLog_COMM:
                    stype = "COMM";
                    break;
                case EnLoggerType.EnLog_OP:
                    stype = "OP";
                    break;
                case EnLoggerType.EnLog_SPC:
                    stype = "SPC";
                    break;
                default:
                    stype = "Other";
                    break;
            }
            LogSay(stype, mMsg);
        }

        List<String> MsgList = new List<string>();

        private void AddMsg(string mType, string mMsg)
        {
            if (TypeList.IndexOf(mType) == -1)
                TypeList.Add(mType);

            //+ By Max 20180424
            mMsg = GetDate() + '\t' + GetTime() + '\t' + mType + '\t' + mMsg;

            lock (locker)
            {
                MsgList.Add(mMsg);
            }

            //+ By Max 20200204 For LogAnalyzer
            try
            {
                SYSPara.logDB.LogSayDb(mMsg);
            }
            catch (Exception ex) 
            {
                LoggerFactory.Logger.Items(0).AddMsg(string.Format("{0}\t{1}", "SQL", ex.Message));
                mMsg = GetDate() + '\t' + GetTime() + '\t' + "SQL" + "\t\t\t" + ex.Message;
                if (TypeList.IndexOf("SQL") == -1)
                    TypeList.Add("SQL");
                lock (locker)
                {
                    MsgList.Add(mMsg);
                }
            }
        }

        private string GetDate()
        {
            DateTime dtNow = DateTime.Now;
            string sDate = DateTime.Now.ToString("yyyy/MM/dd");
            return sDate;
        }

        private string GetTime()
        {
            DateTime dtNow = DateTime.Now;
            string sDate = DateTime.Now.ToString("HH:mm:ss");
            sDate = sDate + "." + dtNow.ToString("fff");
            return sDate;
        }

        public void LogSay(string mType, string mMsg)
        {
            //+ By Max 20180424
            LoggerFactory.Logger.Items(0).AddMsg(string.Format("{0}\t{1}", mType, mMsg));
            AddMsg(mType, mMsg);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Parent = null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //RefreshLog();
            DataView dv = new DataView(dt);
            if (comboBox1.Text != "All Type")
            {
                dv.RowFilter = "類型 = '" + comboBox1.Text + "'";
            }
            dv.Sort = "時間 DESC";
            dgvLog.DataSource = dv;
        }

        private void RefreshLog()
        {
            dgvLog.BeginInvoke((MethodInvoker)delegate()
            {
                lock (locker)
                {
                    if (MsgList.Count > 0)
                    {
                        //String str = MsgList[0];
                        foreach (String str in MsgList)
                        {
                            //+ By Max 20180424
                            string[] aryLine = str.Split('\t');

                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < 9; j++)
                            {
                                if (String.Compare(aryLine[2].Trim(), "Alarm") == 0)
                                {
                                    dr[0] = aryLine[8].Replace("\"", "").Trim();
                                    dr[1] = aryLine[9].Replace("\"", "").Trim();
                                    dr[2] = aryLine[2].Replace("\"", "").Trim();

                                    dr[3] = SYSPara.PackageName;
                                    dr[4] = SYSPara.生產批號;

                                    dr[5] = aryLine[5].Replace("\"", "").Trim();
                                    dr[6] = aryLine[6].Replace("\"", "").Trim();
                                    dr[7] = aryLine[7].Replace("\"", "").Trim();
                                    dr[8] = aryLine[0].Replace("\"", "").Trim();
                                    dr[9] = aryLine[1].Replace("\"", "").Trim();
                                    float sec = Convert.ToInt32(aryLine[10].Replace("\"", "").Trim()) / 1000F;
                                    dr[10] = String.Format("{0:0.0}", sec);
                                }
                                else
                                {
                                    dr[0] = aryLine[0].Replace("\"", "").Trim();
                                    dr[1] = aryLine[1].Replace("\"", "").Trim();
                                    dr[2] = aryLine[2].Replace("\"", "").Trim();
                                   
                                    dr[3] = SYSPara.PackageName;
                                    dr[4] = SYSPara.生產批號;

                                    dr[5] = aryLine[5].Replace("\"", "").Trim();
                                }
                            }


                            if (dt.Rows.Count + 1 > SYSPara.LogMaxLine)
                            {
                                dt.Rows.RemoveAt(dt.Rows.Count - 1);
                            }
                            dt.Rows.InsertAt(dr, 0);

                        }
                        //MsgList.RemoveAt(0);
                        MsgList.Clear();
                        DataView dv = new DataView(dt);
                        if (comboBox1.Text != "All Type")
                        {
                            dv.RowFilter = "類型 = '" + comboBox1.Text + "'";
                        }
                        dv.Sort = "時間 DESC";
                        dgvLog.DataSource = dv;
                    }
                    
                }
            });

        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                RefreshLog();
            }
            catch (Exception)
            {
            }
        }

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        string sLoggerViewerPath = "\\AppData\\LogViewer.exe";
        IntPtr hCCDHandle = (IntPtr)0;

        private void btnLoggerViewer_Click(object sender, EventArgs e)
        {
            bool bExist = false;
            try
            {
                Process[] process = Process.GetProcessesByName("LogViewer");
                foreach (var t in process)
                    t.Kill();

                hCCDHandle = IntPtr.Zero;
                if (hCCDHandle == (IntPtr)0)
                {
                    ProcessStartInfo SInfo = new ProcessStartInfo();
                    
                    SInfo.FileName = System.IO.Directory.GetCurrentDirectory() + "\\" + sLoggerViewerPath;
                    SInfo.Arguments = @"D:\Log";//SYSPara.LogPath;
                    Process.Start(SInfo);
                }
                while (hCCDHandle == (IntPtr)0)
                    hCCDHandle = (IntPtr)FindWindow(null, "LogViewer");

                if (!bExist)
                    while (!IsWindowVisible(hCCDHandle)) { }

                System.Threading.Thread.Sleep(1000);
                return;
            }
            catch(Exception ex)
            {
                String str = ex.Message;
            }
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            InitialLogger();
        }

        private void cycleWidget1_AfterSummary(object DataList)
        {
            ReportF frm = new ReportF();
            frm.Initial(DataList);
            frm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ParamTransactionF frm = new ParamTransactionF();
            frm.ShowDialog();
        }

        string sOneClickBackupPath = "\\AppData\\OneClickBackup.exe";
        private void btnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                Process[] process = Process.GetProcessesByName("OneClickBackup");
                foreach (var t in process)
                    t.Kill();

                hCCDHandle = IntPtr.Zero;
                if (hCCDHandle == (IntPtr)0)
                {
                    ProcessStartInfo SInfo = new ProcessStartInfo();

                    SInfo.FileName = System.IO.Directory.GetCurrentDirectory() + "\\" + sOneClickBackupPath;
                    SInfo.Arguments = System.IO.Directory.GetCurrentDirectory();
                    Process.Start(SInfo);
                }
                System.Threading.Thread.Sleep(1000);
                return;
            }
            catch (Exception ex)
            {
                String str = ex.Message;
            }
        }
    }
}
