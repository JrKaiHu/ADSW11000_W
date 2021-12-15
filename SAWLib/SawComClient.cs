using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWLib
{
    public enum SAWCOMState
    {
        IDLE = 0,
        RUNNING = 1,
        DONE = 2,
        TIMEOUT = 3,
    }

    [Designer(typeof(SawComDesigner))]
    public partial class SawComClient : Component
    {
        private ProVTool.ProVClientSocket ClientSocket = null;

        [Category("ProV")]
        [Description("Handler設定SAW端的IP位址")]
        public String IP
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定SAW端的Port號碼")]
        public int Port
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler判斷此旗標是否已連線")]
        public bool IsConnected
        {
            get;
            set;
        }

        double mReConnectTime = 5.0;
        [Category("ProV")]
        [Description("Handler設定斷線時重新連線的時間間隔（秒）")]
        public double ReConnectTime
        {
            get { return mReConnectTime; }
            set { mReConnectTime = value; }
        }

        private ProVLib.MyTimer T1 = new ProVLib.MyTimer(true);
        private System.Timers.Timer T2 = new System.Timers.Timer();


        #region 取得SAW狀態的函式區段
        /// <summary>
        /// 取狀態的部分是用呼叫函式的方式
        /// 1. 先重置Task
        /// 2. 呼叫取狀態函式
        /// 3. 判斷回傳值
        /// 4. DONE表示成功，可藉由Result取得狀態
        /// 5. TIMEOUT表示超時
        /// </summary>
        
        private bool m_CanPut = false;
        private bool m_HasRcvCanPut = false;
        private int iCanPutTask = 0;
        [Category("ProV")]
        public SAWCOMState CanPut(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if(Reset)
            {
                iCanPutTask = 0;
                m_HasRcvCanPut = false;
                m_CanPut = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iCanPutTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("CanPut\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iCanPutTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvCanPut)
                            {
                                iCanPutTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_CanPut;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }

        private bool m_PutCanLeave = false;
        private bool m_HasRcvPutCanLeave = false;
        private int iPutCanLeaveTask = 0;
        [Category("ProV")]
        public SAWCOMState PutCanLeave(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if (Reset)
            {
                iPutCanLeaveTask = 0;
                m_HasRcvPutCanLeave = false;
                m_PutCanLeave = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iPutCanLeaveTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("PutCanLeave\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iPutCanLeaveTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvPutCanLeave)
                            {
                                iPutCanLeaveTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_PutCanLeave;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }

        private bool m_CanGet = false;
        private bool m_HasRcvCanGet = false;
        private int iCanGetTask = 0;
        [Category("ProV")]
        public SAWCOMState CanGet(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if (Reset)
            {
                iCanGetTask = 0;
                m_HasRcvCanGet = false;
                m_CanGet = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iCanGetTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("PutCanLeave\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iCanGetTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvCanGet)
                            {
                                iCanGetTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_CanGet;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }

        private bool m_GetCanLeave = false;
        private bool m_HasRcvGetCanLeave = false;
        private int iGetCanLeaveTask = 0;
        [Category("ProV")]
        public SAWCOMState GetCanLeave(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if (Reset)
            {
                iGetCanLeaveTask = 0;
                m_HasRcvGetCanLeave = false;
                m_GetCanLeave = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iGetCanLeaveTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("GetCanLeave\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iGetCanLeaveTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvGetCanLeave)
                            {
                                iGetCanLeaveTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_GetCanLeave;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }

        private bool m_SawReady = false;
        private bool m_HasRcvSawReady = false;
        private int iSawReadyTask = 0;
        [Category("ProV")]
        public SAWCOMState SawReady(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if (Reset)
            {
                iSawReadyTask = 0;
                m_HasRcvSawReady = false;
                m_SawReady = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iSawReadyTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("SawReady\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iSawReadyTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvSawReady)
                            {
                                iSawReadyTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_SawReady;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }


        private bool m_LoadPackageOK = false;
        private bool m_HasRcvLoadPackageOK = false;
        private int iLoadPackageOKTask = 0;
        [Category("ProV")]
        public SAWCOMState LoadPackageOK(ref bool Result, bool Reset = false)
        {
            SAWCOMState State = SAWCOMState.IDLE;
            if (Reset)
            {
                iLoadPackageOKTask = 0;
                m_HasRcvLoadPackageOK = false;
                m_LoadPackageOK = false;
                State = SAWCOMState.IDLE;
            }
            else
            {
                switch (iLoadPackageOKTask)
                {
                    case 0:
                        {
                            T1.Restart();
                            State = SAWCOMState.RUNNING;
                            String strSnd = String.Format("LoadPackageOK\n");
                            ClientSocket.Socket.SendText(strSnd);
                            iLoadPackageOKTask = 10;
                        }
                        break;
                    case 10:
                        {
                            if (m_HasRcvLoadPackageOK)
                            {
                                iLoadPackageOKTask = 20;
                            }
                            if (T1.On(3000))
                            {
                                State = SAWCOMState.TIMEOUT;
                            }
                        }
                        break;
                    case 20:
                        {
                            Result = m_LoadPackageOK;
                            State = SAWCOMState.DONE;
                            
                        }
                        break;
                }
            }
            return State;
        }

        #endregion

        #region 設定Handler端的狀態至SAW端的屬性區段
        private bool m_PutInPos = false;
        [Category("ProV")]
        public bool PutInPos
        {
            get { return m_PutInPos; }
            set
            {
                m_PutInPos = value;
                String strSnd = String.Format("PutInPos,{0}\n", m_PutInPos ? "1" : "0");
                if (ClientSocket != null)
                    ClientSocket.Socket.SendText(strSnd);
            }
        }

        private bool m_PutOK = false;
        [Category("ProV")]
        public bool PutOK
        {
            get { return m_PutOK; }
            set
            {
                m_PutOK = value;
                String strSnd = String.Format("PutOK,{0}\n", m_PutOK ? "1" : "0");
                if (ClientSocket != null)
                    ClientSocket.Socket.SendText(strSnd);
            }
        }

        private bool m_GetInPos = false;
        [Category("ProV")]
        public bool GetInPos
        {
            get { return m_GetInPos; }
            set
            {
                m_GetInPos = value;
                String strSnd = String.Format("GetInPos,{0}\n", m_GetInPos ? "1" : "0");
                if(ClientSocket != null)
                    ClientSocket.Socket.SendText(strSnd);
            }
        }

        private bool m_GetOK = false;
        [Category("ProV")]
        public bool GetOK
        {
            get { return m_GetOK; }
            set
            {
                m_GetOK = value;
                String strSnd = String.Format("GetOK,{0}\n", m_GetOK ? "1" : "0");
                if (ClientSocket != null)
                    ClientSocket.Socket.SendText(strSnd);
            }
        }

        private String m_LoadPackageName = String.Empty;
        [Category("ProV")]
        public String LoadPackageName
        {
            get { return m_LoadPackageName; }
            set
            {
                m_LoadPackageName = value;
                String strSnd = String.Format("LoadPackageName,{0}\n", m_LoadPackageName);
                if (ClientSocket != null)
                    ClientSocket.Socket.SendText(strSnd);
            }
        }

        #endregion

        public SawComClient()
        {
            InitializeComponent();
        }

        public SawComClient(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            IsConnected = false;
            T2.Elapsed += T2_Elapsed;
            T2.Interval = mReConnectTime * 1000;
        }

        void T2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            T2.Enabled = false;
            if(!IsConnected)
                SawComConnect();
            T2.Enabled = true;
        }

        public bool SawComConnect()
        {
            if (ClientSocket == null)
            {
                ClientSocket = new ProVTool.ProVClientSocket();
                ClientSocket.IP = this.IP;
                ClientSocket.Port = this.Port;

                ClientSocket.OnRead += ClientSocket_OnRead;
                ClientSocket.OnConnect += ClientSocket_OnConnect;
                ClientSocket.OnDisconnect += ClientSocket_OnDisconnect;


                ClientSocket.Connect();
            }
            else
            {
                ClientSocket.Connect();
            }
            //T2.Start();
            return true;
        }

        void ClientSocket_OnDisconnect(object sender, System.Net.Sockets.Socket socket)
        {
            IsConnected = false;
            T2.Start();
        }

        void ClientSocket_OnConnect(object sender)
        {
            IsConnected = true;
            T2.Stop();
        }

        private void ClientSocket_OnRead(ProVTool.SocketClient sender)
        {
            String strRcv = sender.ReadText();
            if (strRcv.EndsWith("\n"))
            {
                strRcv = strRcv.Trim();

                if (strRcv.Contains("CanPut"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_CanPut = true;
                        }
                        else
                        {
                            m_CanPut = false;
                        }
                    }
                    m_HasRcvCanPut = true;
                }
                else if (strRcv.Contains("PutCanLeave"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_PutCanLeave = true;
                        }
                        else
                        {
                            m_PutCanLeave = false;
                        }
                    }

                    m_HasRcvPutCanLeave = true;
                }
                else if (strRcv.Contains("CanGet"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_CanGet = true;
                        }
                        else
                        {
                            m_CanGet = false;
                        }
                    }
                    m_HasRcvCanGet = true;
                }
                else if (strRcv.Contains("GetCanLeave"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_GetCanLeave = true;
                        }
                        else
                        {
                            m_GetCanLeave = false;
                        }
                    }
                    m_HasRcvGetCanLeave = true;
                }
                else if (strRcv.Contains("SawReady"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_SawReady = true;
                        }
                        else
                        {
                            m_SawReady = false;
                        }
                    }
                    m_HasRcvSawReady = true;
                }
                else if (strRcv.Contains("LoadPackageOK"))
                {
                    String[] StrArray = strRcv.Split(',');
                    if (StrArray.Count() == 2)
                    {
                        if (StrArray[1] == "1")
                        {
                            m_LoadPackageOK = true;
                        }
                        else
                        {
                            m_LoadPackageOK = false;
                        }
                    }
                    m_HasRcvLoadPackageOK = true;
                }
            }
        }

        public void SawComDisConnect()
        {
            ClientSocket.Disconnect();
            return;
        }
    }
}
