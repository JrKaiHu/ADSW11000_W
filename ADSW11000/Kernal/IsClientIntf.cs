using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace IsClientIntf
{
    static class Define
    {
        public const String InSightHost = @"C:\InSightIn-Sight.exe";
        public const String UserID = "admin";
        public const int Password = 1234;
        public const String LoginWelcomeHint = "Welcome to In-Sight";
        public const String UserLoginHint = "User Log In";
        public const String EndingStr = "\r\n";
    }

    public enum EnCMDDefine { eNone = 0, eLoadJob = 1, eSelectMode = 2, eResetMode = 3, eInspect = 4, eGet2DContent = 5 };

    // 控制讀寫(命令發出者,與執行者的狀態)
    //---------------------------------------------------------------------------
    public enum TEFlagState { S_READY=0, S_TRIGGER, S_RUNNING , S_DONE, S_TIMEOUT ,S_CMD_INTERRUPT  };

    // 定義OVC 識別的結果字串代碼
    //---------------------------------------------------------------------------
    public enum TInspectRet { EV_UNDO=0 /*未執行*/, EV_PASS=1 /*辨別成功 */, EV_POSFail=2 /*定位失敗*/ , EV_OCVFail=3 /*字元辨識fail*/ };

    // 定義Command 的結果字串代碼
    //---------------------------------------------------------------------------
    public enum TResult { EV_NONE = -1 /*無狀態*/, EV_NG = 0 /*失敗*/, EV_OK = 1 /*成功 */, EV_BUSY = 2};


    public class TISClient
    {
        private ProVTool.ProVClientSocket ClientScoket = null;
        private String m_IP = "127.0.0.1";
        private int m_Port = 23;
        private bool m_IsConnected = false;
        private ProVLib.MyTimer T1 = new ProVLib.MyTimer();
        private StringBuilder sbRcv = new StringBuilder();
        private StringBuilder sbSnd = new StringBuilder();
        private EnCMDDefine eCurrentCMD = EnCMDDefine.eNone;
        private TResult cmdRet = TResult.EV_NONE;

        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        public TISClient(String MutexName = "ADAM_SHARE_MEM")
        {
            #region 避免程式重覆執行

            ClientScoket = new ProVTool.ProVClientSocket();
            ClientScoket.OnConnect += ClientScoket_OnConnect;
            ClientScoket.OnRead += ClientScoket_OnRead;
            ClientScoket.OnDisconnect += ClientScoket_OnDisconnect;
            m_IsConnected = false;

            Boolean bCreatedNew;
            //Create a new mutex using specific mutex name
            Mutex m = new Mutex(false, MutexName, out bCreatedNew);
            if (!bCreatedNew)
            {
                String strMsg = String.Format("ISClient has been run with Name: {0}!!", MutexName);
                MessageBox.Show(strMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion
            

        }

        void ClientScoket_OnConnect(object sender)
        {
            m_IsConnected = true;
        }

        public bool Connect(String IP, int Port)
        {
            m_IP = IP;
            m_Port = Port;

            ClientScoket.IP = m_IP;
            ClientScoket.Port = m_Port;
            ClientScoket.Connect();

            return true;
        }

        void ClientScoket_OnDisconnect(object sender, System.Net.Sockets.Socket socket)
        {
            m_IsConnected = false;
        }

        void ClientScoket_OnRead(ProVTool.SocketClient sender)
        {
            sbRcv.Append(sender.ReadText());
            String strRcv = sbRcv.ToString();
            int Pos = 0;
            if ((strRcv != String.Empty) && ((Pos = strRcv.IndexOf(Define.EndingStr)) >= 0))
            {
                //sbRcv.Remove(0, Pos + 2);
                sbRcv.Clear();
                switch (eCurrentCMD)
                {
                    case EnCMDDefine.eLoadJob:
                        {
                            if (strRcv.Trim() == "1")
                            {
                                cmdRet = TResult.EV_OK;
                            }
                            else
                            {
                                cmdRet = TResult.EV_NG;
                            }
                            eCurrentCMD = EnCMDDefine.eNone;
                            bLoadJob = true;
                        }
                        break;
                    case EnCMDDefine.eSelectMode:
                        {
                            if (strRcv.Trim() == "1")
                            {
                                cmdRet = TResult.EV_OK;
                            }
                            else
                            {
                                cmdRet = TResult.EV_NG;
                            }
                            eCurrentCMD = EnCMDDefine.eNone;
                            bSelectMode = true;
                        }
                        break;
                    case EnCMDDefine.eResetMode:
                        {
                            if (strRcv.Trim() == "1")
                            {
                                cmdRet = TResult.EV_OK;
                            }
                            else
                            {
                                cmdRet = TResult.EV_NG;
                            }
                            eCurrentCMD = EnCMDDefine.eNone;
                            bSE0Reset = true;
                        }
                        break;
                    case EnCMDDefine.eInspect:
                        {
                            if (strRcv.Trim() == "1")
                            {
                                cmdRet = TResult.EV_OK;
                            }
                            else
                            {
                                cmdRet = TResult.EV_NG;
                            }
                            eCurrentCMD = EnCMDDefine.eNone;
                            bInsepct2D = true;
                        }
                        break;
                    case EnCMDDefine.eGet2DContent:
                        {
                            String[] strList = strRcv.Split((char)0x0D);
                            if (strList.Count() == 2)
                            {
                                if (strList[0].Trim() == "1")
                                {
                                    String[] retList = strList[1].Split('|');
                                    if (retList[0].Contains("#WAIT"))
                                    {
                                        cmdRet = TResult.EV_BUSY;
                                    }
                                    else
                                    {
                                        Result2D = retList[0].Trim();
                                        cmdRet = TResult.EV_OK;
                                        bGet2DContent = true;
                                        eCurrentCMD = EnCMDDefine.eNone;
                                    }
                                }
                                else
                                {
                                    bGet2DContent = true;
                                    cmdRet = TResult.EV_NG;
                                    eCurrentCMD = EnCMDDefine.eNone;
                                }
                            }
                            else
                            {
                                cmdRet = TResult.EV_NG;
                                bGet2DContent = true;
                                eCurrentCMD = EnCMDDefine.eNone;
                            }
                            
                            
                        }
                        break;
                }
            }
        }

        #region LoadJob LF
        private int iTaskLoadJob = 0;
        private bool bLoadJob = false;
        public TEFlagState LoadJob(String JobName, ref TResult bRet, bool bReset = false)
        {
            TEFlagState eRet = TEFlagState.S_RUNNING;

            if (bReset)
            {
                iTaskLoadJob = 1;
                bLoadJob = false;
                eCurrentCMD = EnCMDDefine.eLoadJob;
                cmdRet = TResult.EV_NONE;
                return TEFlagState.S_READY; 
            }

            switch (iTaskLoadJob)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        String strCMD = String.Format("LF{0}.job\r\n", JobName);
                        ClientScoket.Socket.SendText(strCMD);
                        iTaskLoadJob = 10;
                    }
                    break;
                case 10:
                    {
                        if (bLoadJob)
                        {
                            iTaskLoadJob = 20;
                        }
                    }
                    break;
                case 20:
                    {
                        iTaskLoadJob = 0;
                        eRet = TEFlagState.S_DONE;
                        bRet = cmdRet;
                    }
                    break;
            }
            return eRet;
        }
        #endregion

        #region SelectMode SE3
        private bool bSelectMode = false;
        private int iTaskSelectMode = 0;
        public TEFlagState SelectMode(int iMode, ref TResult bRet, bool bReset = false)
        {
            TEFlagState eRet = TEFlagState.S_RUNNING;
            if (bReset)
            {
                bSelectMode = false;
                iTaskSelectMode = 1;
                eCurrentCMD = EnCMDDefine.eSelectMode;
                cmdRet = TResult.EV_NONE;
                return TEFlagState.S_READY;
            }

            switch(iTaskSelectMode)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        String strCMD = String.Format("SE{0}\r\n", iMode);
                        ClientScoket.Socket.SendText(strCMD);
                        iTaskSelectMode = 10;
                    }
                    break;
                case 10:
                    {
                        if(bSelectMode)
                        {
                            iTaskSelectMode = 20;
                        }
                    }
                    break;
                case 20:
                    {
                        iTaskSelectMode = 0;
                        eRet = TEFlagState.S_DONE;
                        bRet = cmdRet;
                    }
                    break;

            }
            return eRet;
        }
        #endregion

        #region Reset SE0
        private bool bSE0Reset = false;
        private int iTaskReset = 0;
        public TEFlagState ResetMode(ref TResult bRet, bool bReset = false)
        {
            TEFlagState eRet = TEFlagState.S_RUNNING;
            if (bReset)
            {
                bSE0Reset = false;
                iTaskReset = 1;
                eCurrentCMD = EnCMDDefine.eResetMode;
                cmdRet = TResult.EV_NONE;
                return TEFlagState.S_READY;
            }
            switch (iTaskReset)
            {
                case 0:
                    break;
                case 1:
                    {
                        String strCMD = String.Format("SE{0}\r\n", 0);
                        ClientScoket.Socket.SendText(strCMD);
                        iTaskReset = 10;
                    }
                    break;
                case 10:
                    {
                        if (bSE0Reset)
                        {
                            iTaskReset = 20;
                        }

                    }
                    break;
                case 20:
                    {
                        iTaskReset = 0;
                        eRet = TEFlagState.S_DONE;
                        bRet = cmdRet;
                    }
                    break;
            }

            return eRet;
        }

        #endregion

        #region Indpect SE8
        private bool bInsepct2D = false;
        private int iTaskInspect2D = 0;
        public TEFlagState Indpect2D(ref TResult bRet, bool bReset = false)
        {
            TEFlagState eRet = TEFlagState.S_RUNNING;
            if (bReset)
            {
                bInsepct2D = false;
                iTaskInspect2D = 1;
                eCurrentCMD = EnCMDDefine.eInspect;
                cmdRet = TResult.EV_NONE;
                return TEFlagState.S_READY;
            }

            switch (iTaskInspect2D)
            {
                case 0:
                    {
                    }
                    break;
                case 1:
                    {
                        String strCMD = String.Format("SE8\r\n");
                        ClientScoket.Socket.SendText(strCMD);
                        iTaskInspect2D = 10;
                    }
                    break;
                case 10:
                    {
                        if (bInsepct2D)
                        {
                            iTaskInspect2D = 20;
                        }
                    }
                    break;
                case 20:
                    {
                        iTaskInspect2D = 0;
                        eRet = TEFlagState.S_DONE;
                        bRet = cmdRet;
                    }
                    break;
                   
            }
            return eRet;
        }
        #endregion

        #region Get 2D Content GVC029
        private bool bGet2DContent = false;
        private int iTaskGet2DContent = 0;
        private String Result2D = String.Empty;
        private int RetryCount = 0;
        public TEFlagState Get2DContent(ref TResult bRet, ref String Value2D, bool bReset = false)
        {
            TEFlagState eRet = TEFlagState.S_RUNNING;
            if(bReset)
            {
                RetryCount = 0;
                bGet2DContent = false;
                iTaskGet2DContent = 1;
                Result2D = String.Empty;
                eCurrentCMD = EnCMDDefine.eGet2DContent;
                cmdRet = TResult.EV_NONE;
                return TEFlagState.S_READY;
            }

            switch (iTaskGet2DContent)
            {
                case 0:
                    break;
                case 1:
                    {
                        String strCMD = String.Format("GVC029\r\n");
                        ClientScoket.Socket.SendText(strCMD);
                        iTaskGet2DContent = 10;
                    }
                    break;
                case 10:
                    {
                        if (bGet2DContent)
                        {

                            iTaskGet2DContent = 20;
                        }
                        else
                        {
                            iTaskGet2DContent = 15;
                            T1.Restart();
                        }
                    }
                    break;
                case 15:
                    {
                        if (T1.On(500))
                        {
                            if (RetryCount++ > 5)
                            {
                                cmdRet = TResult.EV_NG;
                                iTaskGet2DContent = 20;
                            }
                            else
                            {
                                iTaskGet2DContent = 1;
                            }
                        }
                    }
                    break;
                case 20:
                    {
                        Value2D = Result2D;
                        iTaskGet2DContent = 0;
                        eRet = TEFlagState.S_DONE;
                        bRet = cmdRet;
                    }
                    break;
            }
            return eRet;
        }
        #endregion
    }
}
