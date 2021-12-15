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

namespace ADSW11000
{
    public partial class 內參密碼輸入 : Form
    {
        public string Value = "";

        public 內參密碼輸入()
        {
            InitializeComponent();

            Value = "";
            textBox1.Text = Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Value = textBox1.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(Value))
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", "請輸入密碼");
                //MessageBox.Show("請輸入密碼");
                return;
            }
            else if (Value != "89543914")
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", "密碼錯誤");
                //MessageBox.Show("密碼錯誤");
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.Text = "89543914";
        }
    }
}
