using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ADSW11000
{
    public partial class LotendSelectForm : Form
    {
        public LotendSelectForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結批」");

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「強制歸零」");

            this.DialogResult = DialogResult.Abort;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結批取消」");

            this.DialogResult = DialogResult.Cancel;
        }
    }
}
