using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ProVLib;

namespace ADSW11000
{
    public class LoggerFactory
    {
        private static readonly object Mutex = new object();
        private static volatile LoggerFactory _instance;

        //private TAsyncMsgFlusher tOP = null;
        //private TAsyncMsgFlusher tARM = null;
        //private TAsyncMsgFlusher tSPC = null;
        //private TAsyncMsgFlusher tCOMM = null;

        private TAsyncMsgFlusher tLOG = null;

        private LoggerFactory() 
        {
            m_List.Capacity = 2;
        }

        public static LoggerFactory Logger
        {
            get
            {
                if (_instance == null)
                {
                    lock (Mutex)
                    {
                        if (_instance == null)
                            _instance = new LoggerFactory();
                    }
                }
                return _instance;
            }
        }

        public TAsyncMsgFlusher Items(int index)
        {
            return m_List[(int)index];
        }

        public void Add(ListBox Displayer, EnLoggerType enLogType, String sHint = "", bool bEnableWriteLog = true)
        {
            String str = SYSPara.LogPath;
            //str += string.Format("\\Log\\{0:yyyy}\\{0:MM}\\{0:dd}\\", DateTime.Now);
            //if (!Directory.Exists(str))
            //{
            //    Directory.CreateDirectory(str);
            //}

            tLOG = new TAsyncMsgFlusher(500, str, "LOG", Displayer, 1, "csv", sHint);
            tLOG.OnFileIOException = new TAsyncMsgFlusher.FileIOExceptionDelegate(LogFileIOException);
            tLOG.SplitWithTab = true;
            m_List.Add(tLOG);

            //if (enLogType == EnLoggerType.EnLog_ARM)
            //{
            //    tARM = new TAsyncMsgFlusher(500, str, "ARM", Displayer);
            //    m_List.Add(tARM);
            //}
            //else if (enLogType == EnLoggerType.EnLog_OP)
            //{
            //    tOP = new TAsyncMsgFlusher(500, str, "OP", Displayer);
            //    m_List.Add(tOP);
            //}
            //else if (enLogType == EnLoggerType.EnLog_SPC)
            //{
            //    tSPC = new TAsyncMsgFlusher(500, str, "SPC", Displayer);
            //    m_List.Add(tSPC);
            //}
            //else if (enLogType == EnLoggerType.EnLog_COMM)
            //{
            //    tCOMM = new TAsyncMsgFlusher(500, str, "COMM", Displayer);
            //    m_List.Add(tCOMM);
            //}
        }

        private void LogFileIOException(String Message)
        {
            SYSPara.Alarm.Say("w0502");
        }

        public string GetLogFileName()
        {
            return tLOG.Now_LogFileName();
            //switch (type)
            //{
            //    case EnLoggerType.EnLog_OP:
            //        return tOP.Now_LogFileName();
            //    case EnLoggerType.EnLog_SPC:
            //        return tSPC.Now_LogFileName();
            //    case EnLoggerType.EnLog_ARM:
            //        return tARM.Now_LogFileName();
            //    case EnLoggerType.EnLog_COMM:
            //        return tCOMM.Now_LogFileName();
            //}
            //return null;
        }

        private List<TAsyncMsgFlusher> m_List = new List<TAsyncMsgFlusher>();
    }

}
