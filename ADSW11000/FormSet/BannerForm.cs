using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ADSW11000
{
    public partial class BannerForm : Form
    {
        public string mCaption;
        private bool StopThread;
        private Thread thProcess;
        private object lockobj = null;

        public BannerForm()
        {
            InitializeComponent();
            mCaption = "";

            thProcess = new Thread(RefreshUI);
            lockobj = new object();
        }

        public void RefreshUI()
        {
            Graphics g1 = picLogo.CreateGraphics();
            Graphics g2 = picText.CreateGraphics();
            Font objFont = new System.Drawing.Font("微軟正黑體", 16, FontStyle.Bold);
            int count = 0;
            string loadmsg = "";
            g1.DrawImage(picLogo.Image, 0, 0, picLogo.Width, picLogo.Height);
            while (!StopThread)
            {
                lock (lockobj)
                {
                    Thread.Sleep(100);
                    g2.Clear(Color.White);
                    loadmsg = "";
                    for (int i = 0; i < (count % 4); i++)
                    {
                        loadmsg += ".";
                    }
                    g2.DrawString(mCaption + loadmsg, objFont, System.Drawing.Brushes.Black, 0, 0);
                    count++;
                }
            }
        }

        public void Start()
        {
            thProcess.Start();
        }

        public void DisposTH()
        {
            StopThread = true;
            thProcess.Join();
        }

        //public void Stop()
        //{
        //    thProcess.Suspend();
        //}

        //public void Resume()
        //{
        //    thProcess.Resume();
        //}
    }
}
