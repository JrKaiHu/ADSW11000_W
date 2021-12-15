using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSW11000
{
    public partial class AlarmForm : Form
    {
        private static readonly object Mutex = new object();
        private static volatile AlarmForm _instance;       

        private static ArmMtrl m_ArmMtrl = null;
        private AlarmForm()
        {
            InitializeComponent();
        }

        static AlarmForm Form
        {
            get
            {
                if (_instance != null && _instance.IsDisposed)
                {
                    lock (Mutex)
                    {
                        if (_instance != null && _instance.IsDisposed)
                            _instance = new AlarmForm();
                    }
                }
                else
                {
                    lock (Mutex)
                    {
                        if (_instance == null)
                            _instance = new AlarmForm();
                    }
                }
                return _instance;
            }
        }
        
        public static void ShowAlarmForm(ArmMtrl armMtrl)
        {
            m_ArmMtrl = armMtrl;
            String ErrSource = string.Format("{0:0}{1:0000}{2}", m_ArmMtrl.AlarmLevel.ToUpper(), m_ArmMtrl.ModuleID * 100 + m_ArmMtrl.ErrorCode, ".jpg");

            String imagePath = SYSPara.SysDir + "\\localdata\\AlarmResource\\" + ErrSource;

            if (System.IO.File.Exists(imagePath))
            {
                String ErrPdf = string.Format("{0:0}{1:0000}{2}", m_ArmMtrl.AlarmLevel.ToUpper(), m_ArmMtrl.ModuleID * 100 + m_ArmMtrl.ErrorCode, ".pdf");
                String pdfPath = SYSPara.SysDir + "\\localdata\\AlarmResource\\" + ErrPdf;
                Form.btnDetail.Visible = System.IO.File.Exists(pdfPath) ? true : false;
                Form.ShowDialog();
            }
        }

        public static void CloseAlarmForm()
        {
            if (Form.InvokeRequired)
            {
                Form.Invoke(new MethodInvoker(CloseAlarmForm));
                return;
            }
            Form.Close();
            Form.Dispose();
        }

        private void AlarmForm_Load(object sender, EventArgs e)
        {
            String ErrSource = string.Format("{0:0}{1:0000}{2}", m_ArmMtrl.AlarmLevel.ToUpper(), m_ArmMtrl.ModuleID * 100 + m_ArmMtrl.ErrorCode, ".jpg");

            String imagePath = SYSPara.SysDir + "\\localdata\\AlarmResource\\" + ErrSource;

            if(System.IO.File.Exists(imagePath))
            {
                PictureAlarm.Image = Image.FromFile(imagePath);
                txtAlarm.Text = m_ArmMtrl.Explain;
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            String ErrSource = string.Format("{0:0}{1:0000}{2}", m_ArmMtrl.AlarmLevel.ToUpper(), m_ArmMtrl.ModuleID * 100 + m_ArmMtrl.ErrorCode, ".pdf");

            String pdfPath = SYSPara.SysDir + "\\localdata\\AlarmResource\\" + ErrSource;

            if (System.IO.File.Exists(pdfPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(pdfPath);
                Process.Start(startInfo);
            }
        }
    }
}
