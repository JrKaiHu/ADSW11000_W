using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSW11000
{
    public class VisionObj
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOW = 5;

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr Child, IntPtr Parent);

        [System.Runtime.InteropServices.DllImport("coredll.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        private Process[] MyProcess = new Process[1];
        private IntPtr ipFW;
        private string EXEName;
        private string EXECaption;

        public VisionObj(string mEXEName, string mEXECaption)
        {
            EXEName = mEXEName;
            EXECaption = mEXECaption;
        }

        public void Vision_EXE_Open(string FileName)
        {
            if (FileName == "")
                return;

            try
            {
                //確保電腦內已無其他份執行檔
                while (FindVision() || Vision_EXE_Execute())
                    Vision_EXE_Close(2000);

                //載入
                Process.Start(@FileName);

                //確保執行成功
                while (!FindVision()) { };
            }
            catch
            {
                string message = "Vision execution doesn't exist. Check up on: " + @FileName;
                string caption = "Vision Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }

        public bool FindVision()
        {
            try
            {
                ipFW = (IntPtr)FindWindow(null, EXECaption);
                if (ipFW != IntPtr.Zero)
                    return true;
            }
            catch { }
            return false;
        }

        public void SetPanel(IntPtr PanelPtr)
        {
            try
            {
                if (ipFW != IntPtr.Zero)
                {
                    SetParent(ipFW, PanelPtr);
                }
            }
            catch { }
        }

        public bool Vision_EXE_Execute()
        {
            bool Flag = false;
            try
            {
                MyProcess = Process.GetProcessesByName(EXEName); //偵測執行中的外部程式
                if (MyProcess.Length > 0)
                {
                    Flag = true;
                }
            }
            catch
            {
            }
            return Flag;
        }

        public void Vision_EXE_Close(int SleepTM)
        {
            try
            {
                //確保電腦內已無其他份執行檔
                while (FindVision())
                {
                    MyProcess = Process.GetProcessesByName(EXEName); //偵測執行中的外部程式
                    MyProcess[0].Kill(); //關閉執行中的程式
                    Thread.Sleep(SleepTM);
                }
            }
            catch
            { }
        }
    }
}
