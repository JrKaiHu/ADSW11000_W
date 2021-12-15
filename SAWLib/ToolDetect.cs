using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automation.BDaq;
using System.Drawing.Design;

namespace SAWLib
{
    public partial class ToolDetect : UserControl
    {
        #region 公有變數
        public double Value;
        public bool IsBroken;
        public double BrokenValue;
        #endregion

        #region 控制項屬性

        private ToolDataCollect mDataVCL = null;
        [Browsable(true), Category("#參數設定"), Description("選擇資料收集器")]
        public ToolDataCollect DataVCL
        {
            get { return mDataVCL; }
            set
            {
                mDataVCL = value;
                if (mDataVCL != null)
                    mDataVCL.AddDetect(this);
            }
        }

        [Browsable(true), Category("#參數設定"), Description("Channel選擇")]
        public int AIChannel { get; set; }

        private int mPercentage = 0;
        [Browsable(true), Category("#參數設定"), Description("百分比")]
        public int Percentage
        {
            get { return mPercentage; }
            set
            {
                if ((value >= 0) && (value <= 100))
                    mPercentage = value;
            }
        }

        #endregion

        public ToolDetect()
        {
            InitializeComponent();
            backColor = lbValue.BackColor;
        }

        Color backColor;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (!bShowPercentage)
            {
                lbValue.Text = string.Format("{0:0.00} V", Value);
                if (Value < 0)
                    progressBar1.Value = 0;
                else if (Value > 5)
                    progressBar1.Value = 50;
                else
                    progressBar1.Value = (int)(Value * 10);
            }
            else
            {
                lbValue.Text = string.Format("{0} %", mPercentage);
                progressBar1.Value = mPercentage;
            }
            if (Value >= 2.8f)
            {
                lbValue.BackColor = Color.Red;
            }
            else
            {
                lbValue.BackColor = backColor;
            }

            timer1.Enabled = true;
        }

        private bool bShowPercentage = false;
        private void lbValue_MouseUp(object sender, MouseEventArgs e)
        {
            bShowPercentage = !bShowPercentage;
            if (bShowPercentage)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Step = 10;
            }
            else
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 50;
                progressBar1.Step = 1;
            }

            if (Control.ModifierKeys == Keys.Control && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                String fftTitle = AIChannel == 0 ? "FFT Plot (Z1)" : "FFT Plot (Z2)";
                String idxTitle = AIChannel == 0 ? "Index Plot (Z1)" : "Index Plot (Z2)";

                PlotWrapper.Plot figFreq = new PlotWrapper.Plot(fftTitle, "Frequency", "Mag", 0, 0);
                PlotWrapper.Plot figTime = new PlotWrapper.Plot(idxTitle, "Index", "Volt", 0, 0);

                if (DataVCL.mDataOrg != null)
                    figTime.PlotData(DataVCL.mDataOrg);
                figTime.Show();

                if (DataVCL.fSpan != null && DataVCL.magLog != null)
                    figFreq.PlotData(DataVCL.fSpan, DataVCL.magLog);
                figFreq.Show();
            }
        }

        private void progressBar1_MouseUp(object sender, MouseEventArgs e)
        {
            bShowPercentage = !bShowPercentage;
            if (bShowPercentage)
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 100;
                progressBar1.Step = 10;
            }
            else
            {
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 50;
                progressBar1.Step = 1;
            }
        }
    }
}
