using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Drawing2D;

using KCSDK;

using ProgressBars.Basic;

namespace ADSW11000
{
    public partial class CutLineSimulate : Form
    {
        private DDataGridView m_dCutProg;

        private int m_nCurrentIdx = 0;

        private bool m_bIsCh1First;

        private bool m_bIsForward;

        private Color m_DefaultColor;

        public CutLineSimulate(DDataGridView dgv)
        {
            InitializeComponent();
            m_dCutProg = dgv;

            m_bIsCh1First = m_dCutProg.Rows[0].Cells[7].Value.ToString() == "CH1";
            m_bIsForward = true;
        }

        private void CutLineSimulate_Load(object sender, EventArgs e)
        {
            dCutProg.Rows.Clear();

            foreach(DataGridViewRow r in m_dCutProg.Rows)
            {
                dCutProg.Rows.Add(new object[] { r.Cells[0].Value, r.Cells[1].Value, r.Cells[2].Value, r.Cells[3].Value, 
                    r.Cells[4].Value, r.Cells[7].Value, r.Cells[5].Value, r.Cells[6].Value, r.Cells[8].Value, r.Cells[9].Value});
            }

            dCutProg.ClearSelection();
            m_DefaultColor = dCutProg.Rows[0].DefaultCellStyle.BackColor;

            int nColChipNum = Convert.ToInt32(FormSet.mPackage.dFieldEdit17.FieldValue);
            int nRowChipNum = Convert.ToInt32(FormSet.mPackage.dFieldEdit16.FieldValue);

            float fBoardWidth = Convert.ToSingle(FormSet.mPackage.dFieldEdit32.FieldValue);
            float fBoardHeight = Convert.ToSingle(FormSet.mPackage.dFieldEdit33.FieldValue);

            tlpState.Width = (int)(tlpState.Width * (fBoardWidth / fBoardHeight));

            tlpState.RowCount = nRowChipNum + 1 + nRowChipNum + 2 + 1;
            tlpState.ColumnCount = nColChipNum + 1 + nColChipNum + 2 + 1;

            tlpState.RowStyles.Clear();
            tlpState.ColumnStyles.Clear();

            for (int i = 0; i < tlpState.RowCount; i++)
            {
                if (i == 0 || i == (tlpState.RowCount) - 2)
                {
                    tlpState.RowStyles.Add(new RowStyle(SizeType.Percent, 3f));
                }
                else if (i == (tlpState.RowCount) - 1)
                {
                    tlpState.RowStyles.Add(new RowStyle(SizeType.Percent, 0.1f));
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        tlpState.RowStyles.Add(new RowStyle(SizeType.Percent, 70f / nRowChipNum));
                    }
                    else
                    {
                        tlpState.RowStyles.Add(new RowStyle(SizeType.Percent, 23.9f / (nRowChipNum + 1)));
                    }
                }
            }

            for (int i = 0; i < tlpState.ColumnCount; i++)
            {
                if (i == 0 || i == (tlpState.ColumnCount) - 2)
                {
                    tlpState.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3f));
                }
                else if (i == (tlpState.ColumnCount) - 1)
                {
                    tlpState.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.1f));
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        tlpState.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70f / nColChipNum));
                    }
                    else
                    {
                        tlpState.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23.9f / (nColChipNum + 1)));
                    }
                }
            }

            int nCutLineColIdx = 0;
            int nCutLineRowIdx = 0;

            for (int i = 0; i < tlpState.RowCount; i++)
            {
                for (int j = 0; j < tlpState.ColumnCount; j++)
                {
                    if (i != (tlpState.RowCount - 1) && j != (tlpState.ColumnCount - 1))
                    {
                        if (i % 2 == 0 && j % 2 == 0)
                        {
                            Label lbl = new Label();
                            lbl.BackColor = Color.SeaGreen;
                            lbl.Dock = DockStyle.Fill;
                            //lbl.TextAlign = ContentAlignment.MiddleCenter;
                            lbl.Margin = new Padding(0);

                            tlpState.Controls.Add(lbl, j, i);

                            if (i != 0 && i != (tlpState.RowCount - 2) && j != 0 && j != (tlpState.ColumnCount - 2))
                            {
                                Bitmap bmp = new Bitmap(ADSW11000.Properties.Resources.Chip);
                                lbl.Image = new Bitmap(bmp, new Size(lbl.Width, lbl.Height));
                            }
                        }

                        if (m_bIsCh1First)
                        {
                            BasicProgressBar bpb = new BasicProgressBar();
                            bpb.Maximum = 100;
                            bpb.Minimum = 0;
                            bpb.Dock = DockStyle.Fill;
                            bpb.Margin = new Padding(0);
                            bpb.TextStyle = BasicProgressBar.TextStyleType.None;
                            //bpb.ForeColor = Color.LimeGreen;
                            bpb.BorderThickness = 0;

                            if (i % 2 == 0 && j % 2 == 1)
                            {
                                bpb.Orientation = Orientation.Vertical;
                                bpb.ForeColor = Color.FromArgb(0x00, 0x66, 0xCC);
                                bpb.BackColor = Color.LightGray;
                                bpb.Value = 100;

                                tlpState.Controls.Add(bpb, j, i);

                                if (i == 0)
                                {
                                    int nPosX = tlpState.Location.X + bpb.Location.X + bpb.Width / 2 - 30 / 2;
                                    int nPosY = tlpState.Location.Y - 30;

                                    Label lbl = new Label();
                                    lbl.Location = new Point(nPosX, nPosY);
                                    lbl.Size = new Size(30, 30);
                                    lbl.Text = (++nCutLineRowIdx).ToString();
                                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                                    Controls.Add(lbl);
                                }
                            }

                            if (i % 2 == 1 && j == 0)
                            {
                                bpb.Orientation = Orientation.Horizontal;
                                bpb.ForeColor = Color.LightGray;
                                bpb.BackColor = Color.FromArgb(0xFF, 0x51, 0x51);
                                bpb.Value = 0;

                                tlpState.Controls.Add(bpb, j, i);
                                tlpState.SetColumnSpan(bpb, tlpState.ColumnCount - 1);

                                int nPosX = tlpState.Location.X - 30;
                                int nPosY = tlpState.Location.Y + bpb.Location.Y + bpb.Height / 2 - 30 / 2;

                                Label lbl = new Label();
                                lbl.Location = new Point(nPosX, nPosY);
                                lbl.Size = new Size(30, 30);
                                lbl.Text = (++nCutLineColIdx).ToString();
                                lbl.TextAlign = ContentAlignment.MiddleCenter;
                                Controls.Add(lbl);
                            }
                        }
                        else
                        {
                            BasicProgressBar bpb = new BasicProgressBar();
                            bpb.Maximum = 100;
                            bpb.Minimum = 0;
                            bpb.Dock = DockStyle.Fill;
                            bpb.Margin = new Padding(0);
                            bpb.TextStyle = BasicProgressBar.TextStyleType.None;
                            //bpb.ForeColor = Color.LimeGreen;
                            bpb.BorderThickness = 0;

                            if (i % 2 == 1 && j % 2 == 0)
                            {
                                bpb.Orientation = Orientation.Horizontal;
                                bpb.ForeColor = Color.LightGray;
                                bpb.BackColor = Color.FromArgb(0xFF, 0x51, 0x51);
                                bpb.Value = 0;

                                tlpState.Controls.Add(bpb, j, i);

                                if (j == tlpState.ColumnCount - 2)
                                {
                                    int nPosX = tlpState.Location.X - 30;
                                    int nPosY = tlpState.Location.Y + bpb.Location.Y + bpb.Height / 2 - 30 / 2;

                                    Label lbl = new Label();
                                    lbl.Location = new Point(nPosX, nPosY);
                                    lbl.Size = new Size(30, 30);
                                    lbl.Text = (++nCutLineRowIdx).ToString();
                                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                                    Controls.Add(lbl);
                                }
                            }

                            if (j % 2 == 1 && i == 0)
                            {
                                bpb.Orientation = Orientation.Vertical;
                                bpb.ForeColor = Color.FromArgb(0x00, 0x66, 0xCC);
                                bpb.BackColor = Color.LightGray;
                                bpb.Value = 100;

                                tlpState.Controls.Add(bpb, j, i);
                                tlpState.SetRowSpan(bpb, tlpState.RowCount - 1);

                                int nPosX = tlpState.Location.X + bpb.Location.X + bpb.Width / 2 - 30 / 2;
                                int nPosY = tlpState.Location.Y - 30;

                                Label lbl = new Label();
                                lbl.Location = new Point(nPosX, nPosY);
                                lbl.Size = new Size(30, 30);
                                lbl.Text = (++nCutLineColIdx).ToString();
                                lbl.TextAlign = ContentAlignment.MiddleCenter;
                                Controls.Add(lbl);
                            }
                        }
                    }
                }
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == btnStart)
            {
                m_nCurrentIdx = 0;
                timer.Enabled = true;
                m_bIsForward = true;
            }

            else if (btn == btnPause)
            {
                m_nCurrentIdx = 0;
                timer.Enabled = false;
                m_bIsForward = false;
            }
            else if (btn == btnRestore)
            {
                m_nCurrentIdx = 0;

                for (int i = 0; i < tlpState.RowCount; i++)
                {
                    for (int j = 0; j < tlpState.ColumnCount; j++)
                    {
                        BasicProgressBar bpb = tlpState.GetControlFromPosition(j, i) as BasicProgressBar;

                        if (bpb != null)
                        {
                            bpb.Value = bpb.Orientation == Orientation.Vertical ? bpb.Maximum : bpb.Minimum;
                        }
                    }
                }

                foreach (DataGridViewRow row in dCutProg.Rows)
                { 
                    row.DefaultCellStyle.BackColor = m_DefaultColor; 
                }
            }
            else if (btn == btnForward)
            {
                timer.Enabled = true;
                m_bIsForward = false;
            }
            else if (btn == btnBack)
            {
                if (m_nCurrentIdx != 0)
                {
                    m_nCurrentIdx--;

                    DataGridViewRow dgvRow = m_dCutProg.Rows[m_nCurrentIdx];
                    bool bIsZ1 = Convert.ToBoolean(dgvRow.Cells[1].Value);
                    bool bIsZ2 = Convert.ToBoolean(dgvRow.Cells[3].Value);

                    if (m_bIsCh1First)
                    {
                        #region ch1 first

                        if (dgvRow.Cells[7].Value.ToString() == "CH1")
                        {
                            #region ch1
                            if (bIsZ1)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(0, Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1) as BasicProgressBar;
                                bpb.Value = 0;
                            }
                            if (bIsZ2)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(0, Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1) as BasicProgressBar;
                                bpb.Value = 0;
                            }
                            #endregion
                        }
                        else
                        {
                            #region ch2
                            if (bIsZ1)
                            {
                                for (int i = 0; i < tlpState.RowCount; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1, i) as BasicProgressBar;
                                        bpb.Value = 100;
                                    }
                                }
                            }

                            if (bIsZ2)
                            {
                                for (int i = 0; i < tlpState.RowCount; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1, i) as BasicProgressBar;
                                        bpb.Value = 100;
                                    }
                                }
                            }
                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region ch2 first

                        if (dgvRow.Cells[7].Value.ToString() == "CH1")
                        {
                            #region ch1
                            if (bIsZ1)
                            {
                                for (int i = 0; i < tlpState.ColumnCount; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        BasicProgressBar bpb = tlpState.GetControlFromPosition(i, Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1) as BasicProgressBar;
                                        bpb.Value = 0;
                                    }
                                }
                            }

                            if (bIsZ2)
                            {
                                for (int i = 0; i < tlpState.ColumnCount; i++)
                                {
                                    if (i % 2 == 0)
                                    {
                                        BasicProgressBar bpb = tlpState.GetControlFromPosition(i, Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1) as BasicProgressBar;
                                        bpb.Value = 0;
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region ch2
                            if (bIsZ1)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1, 0) as BasicProgressBar;
                                bpb.Value = 100;
                            }

                            if (bIsZ2)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1, 0) as BasicProgressBar;
                                bpb.Value = 100;
                            }
                            #endregion
                        }

                        #endregion
                    }
                }
            }
        }

        private void SingleStep()
        {
            if (m_nCurrentIdx >= dCutProg.RowCount) return;

            DataGridViewRow dgvRow = m_dCutProg.Rows[m_nCurrentIdx];
            bool bIsZ1 = Convert.ToBoolean(dgvRow.Cells[1].Value);
            bool bIsZ2 = Convert.ToBoolean(dgvRow.Cells[3].Value);

            bool bIsNext = true;

            dCutProg.Rows[m_nCurrentIdx].DefaultCellStyle.BackColor = Color.Red;

            if (m_bIsCh1First)
            {
                #region ch1 first

                #region ch1
                if (dgvRow.Cells[7].Value.ToString() == "CH1")
                {
                    BasicProgressBar bpb1;
                    BasicProgressBar bpb2;

                    if (bIsZ1)
                    {
                        bpb1 = tlpState.GetControlFromPosition(0, Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1) as BasicProgressBar;
                        if (bpb1.Value != 100)
                        {
                            bpb1.Value += 5;
                            bIsNext &= false;
                        }
                    }
                    if (bIsZ2)
                    {
                        bpb2 = tlpState.GetControlFromPosition(0, Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1) as BasicProgressBar;
                        if (bpb2.Value != 100)
                        {
                            bpb2.Value += 5;
                            bIsNext &= false;
                        }
                    }
                }
                #endregion

                #region ch2
                if (dgvRow.Cells[7].Value.ToString() == "CH2")
                {
                    List<BasicProgressBar> bpbLst1 = new List<BasicProgressBar>();
                    List<BasicProgressBar> bpbLst2 = new List<BasicProgressBar>();

                    if (bIsZ1)
                    {
                        for (int i = 0; i < tlpState.RowCount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1, i) as BasicProgressBar;
                                bpbLst1.Add(bpb);
                            }
                        }

                        BasicProgressBar temp = bpbLst1.Find(x => x.Value != 0);

                        if(temp != null) temp.Value -= 50;

                        if (bpbLst1[bpbLst1.Count - 1].Value != 0)
                        {
                            bIsNext &= false;
                        }
                    }

                    if (bIsZ2)
                    {
                        for (int i = 0; i < tlpState.RowCount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1, i) as BasicProgressBar;
                                bpbLst2.Add(bpb);
                            }
                        }

                        BasicProgressBar temp = bpbLst2.Find(x => x.Value != 0);
                        if(temp != null) temp.Value -= 50;

                        if (bpbLst2[bpbLst2.Count - 1].Value != 0)
                        {
                            bIsNext &= false;
                        }
                    }
                }
                #endregion

                #endregion
            }
            else
            {
                #region ch2 first

                #region ch2
                if (dgvRow.Cells[7].Value.ToString() == "CH2")
                {
                    BasicProgressBar bpb1;
                    BasicProgressBar bpb2;

                    if (bIsZ1)
                    {
                        bpb1 = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1, 0) as BasicProgressBar;
                        if (bpb1.Value != 0)
                        {
                            bpb1.Value -= 5;
                            bIsNext &= false;
                        }
                    }
                    if (bIsZ2)
                    {
                        bpb2 = tlpState.GetControlFromPosition(Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1, 0) as BasicProgressBar;
                        if (bpb2.Value != 0)
                        {
                            bpb2.Value -= 5;
                            bIsNext &= false;
                        }
                    }
                }
                #endregion

                #region ch1
                if (dgvRow.Cells[7].Value.ToString() == "CH1")
                {
                    List<BasicProgressBar> bpbLst1 = new List<BasicProgressBar>();
                    List<BasicProgressBar> bpbLst2 = new List<BasicProgressBar>(); ;

                    if (bIsZ1)
                    {
                        for (int i = 0; i < tlpState.ColumnCount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(i, Convert.ToInt32(dgvRow.Cells[2].Value) * 2 - 1) as BasicProgressBar;
                                bpbLst1.Add(bpb);
                            }
                        }

                        BasicProgressBar temp = bpbLst1.Find(x => x.Value != 100);
                        if(temp != null) temp.Value += 50;

                        if (bpbLst1[bpbLst1.Count - 1].Value != 100)
                        {
                            bIsNext &= false;
                        }
                    }

                    if (bIsZ2)
                    {
                        for (int i = 0; i < tlpState.ColumnCount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                BasicProgressBar bpb = tlpState.GetControlFromPosition(i, Convert.ToInt32(dgvRow.Cells[4].Value) * 2 - 1) as BasicProgressBar;
                                bpbLst2.Add(bpb);
                            }
                        }

                        BasicProgressBar temp = bpbLst2.Find(x => x.Value != 100);
                        if (temp != null) temp.Value += 50;

                        if (bpbLst2[bpbLst2.Count - 1].Value != 100)
                        {
                            bIsNext &= false;
                        }
                    }
                }
                #endregion

                #endregion
            }

            if (bIsNext)
            {
                m_nCurrentIdx++;

                dCutProg.Rows[m_nCurrentIdx - 1].DefaultCellStyle.BackColor = m_DefaultColor;

                if (m_nCurrentIdx == m_dCutProg.RowCount || !m_bIsForward)
                {
                    timer.Enabled = false;
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!Visible) timer.Enabled = false;

            SingleStep();
        }
    }
}
