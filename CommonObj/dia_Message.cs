using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonObj
{
    public partial class dia_Message : Form
    {
        private static readonly dia_Message instance = new dia_Message();

        public static dia_Message Instance
        {
            get
            {
                return instance;
            }
        }

        private DialogResult m_DialogRes = DialogResult.Cancel;

        private const int WS_SYSMENU = 0x80000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }

        private dia_Message()
        {
            InitializeComponent();

            Bitmap bmpYes = new Bitmap(Properties.Resources.Ok, new Size(15, 15));
            btnYes.Image = bmpYes;
            Bitmap bmpNo = new Bitmap(Properties.Resources.Cancel, new Size(15, 15));
            btnNo.Image = bmpNo;
        }

        public DialogResult ShowDialog(enMsgDialogType type, string strTitle, string strMessage,
            string strLang = "tw", string strOkBtnText = "", string strCancelText = "")
        {
            lblTitle.Text = strTitle;
            lblContent.Text = strMessage;

            if (Visible) return DialogResult.OK;

            if (type == enMsgDialogType.WARNING)
            {
                btnNo.Visible = true;
                btnYes.Left = (ClientRectangle.Width - btnYes.Width - btnNo.Width) / 3;
                btnNo.Left = btnYes.Right + ((ClientRectangle.Width - btnYes.Width - btnNo.Width) / 3);
                pbTypeImg.Image = Properties.Resources.Warning;

                tableLayoutPanel1.SetColumn(btnNo, 6);
                tableLayoutPanel1.SetColumn(btnYes, 4);

                if (strOkBtnText.Length != 0) btnYes.Text = strOkBtnText;
                if (strCancelText.Length != 0) btnNo.Text = strCancelText;
            }
            else
            {
                pbTypeImg.Image = type == enMsgDialogType.NOTIFY ? Properties.Resources.Notify_Yellow : Properties.Resources.Error;

                btnNo.Visible = false;
                btnYes.Left = (ClientRectangle.Width - btnYes.Width) / 2;
                tableLayoutPanel1.SetColumn(btnNo, 4);
                tableLayoutPanel1.SetColumn(btnYes, 6);
            }

            if (strLang != "tw")
            {
                Text = type.ToString();
            }
            else
            {
                if (type == enMsgDialogType.NOTIFY) Text = "提示訊息";
                if (type == enMsgDialogType.WARNING) Text = "警告訊息";
                if (type == enMsgDialogType.ERROR) Text = "錯誤訊息";
            }

            //return base.ShowDialog();
            ShowDialog();

            return m_DialogRes;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if(btn == btnYes)
            {
                m_DialogRes = DialogResult.OK;
            }
            else if(btn == btnNo)
            {
                m_DialogRes = DialogResult.Cancel;
            }

            Close();
        }
    }
}
