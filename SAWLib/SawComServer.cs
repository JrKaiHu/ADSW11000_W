using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SAWLib
{
    [Designer(typeof(SawComDesigner))]
    public partial class SawComServer : Component
    {
        private ProVTool.ProVServerSocket ServerSocket = null;
        private Thread CMDThread = null;
        private object locker = new object();
        private List<String> CMDList = new List<string>();

        [Category("ProV")]
        public String IP
        {
            get;
            set;
        }

        [Category("ProV")]
        public int Port
        {
            get;
            set;
        }

        [Category("ProV")]
        public bool IsConnected
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問是否可放料")]
        public bool CanPut
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定此旗標供SAW詢問料是否就定位")]
        public bool PutInPos
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問手臂是否可離開")]
        public bool PutCanLeave
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定此旗標供SAW詢問手臂是否離開完成")]
        public bool PutOK
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問是否可取料")]
        public bool CanGet
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定此旗標供SAW詢問手臂是否就定位")]
        public bool GetInPos
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問手臂是否可離開")]
        public bool GetCanLeave
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定此旗標供SAW詢問手臂是否離開完成")]
        public bool GetOK
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問SAW系統是否處於就緒狀態")]
        public bool SawReady
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("Handler設定此旗標供SAW詢問料號名稱")]
        public String LoadPackageName
        {
            get;
            set;
        }

        [Category("ProV")]
        [Description("SAW設定此旗標供Handler詢問SAW系統是否載料完成")]
        public bool LoadPackageOK
        {
            get;
            set;
        }

        public SawComServer()
        {
            InitializeComponent();
        }

        public SawComServer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            IsConnected = false;
            CMDList.Clear();
            CMDThread = new Thread(new ThreadStart(DoWork));
            CMDThread.IsBackground = true;
            CMDThread.Start();
        }

        public bool SawComConnect()
        {
            if (ServerSocket == null)
            {
                ServerSocket = new ProVTool.ProVServerSocket();
                ServerSocket.IP = this.IP;
                ServerSocket.Port = this.Port;

                ServerSocket.OnRead += ServerSocket_OnRead;
                ServerSocket.OnDisconnect += ServerSocket_OnDisconnect;
                ServerSocket.OnConnect += ServerSocket_OnConnect;
                ServerSocket.Connect();
            }
            else
            {
                ServerSocket.Connect();
            }
            return true;
        }

        
        private void ServerSocket_OnConnect(object sender)
        {
            IsConnected = true;
            CMDList.Clear();
        }

        private void ServerSocket_OnDisconnect(object sender, System.Net.Sockets.Socket socket)
        {
            ServerSocket.RemoveConnection(ServerSocket[0]);
            IsConnected = false;
        }

        public void SawComDisConnect()
        {
            IsConnected = false;
            ServerSocket.Disconnect();
            return;
        }

        private void ServerSocket_OnRead(ProVTool.SocketClient sender)
        {
            String strRcv = sender.ReadText();

            if (strRcv.EndsWith("\n"))
            {
                strRcv = strRcv.Trim();
                lock (locker)
                {
                    CMDList.Add(strRcv);
                }
            }
        }

        public void DoWork() 
        {
            while(true)
            {
                Thread.Sleep(1);
                lock (locker)
                {
                    if (CMDList.Count > 0)
                    {
                        String strRcv = CMDList[0];
                        CMDList.Remove(strRcv);

                        String strSnd = String.Empty;
                        if (strRcv.Contains("CanPut"))
                        {
                            if (CanPut)
                            {
                                strSnd = "CanPut,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "CanPut,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("PutCanLeave"))
                        {
                            if (PutCanLeave)
                            {
                                strSnd = "PutCanLeave,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "PutCanLeave,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("CanGet"))
                        {
                            if (CanGet)
                            {
                                strSnd = "CanGet,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "CanGet,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("GetCanLeave"))
                        {
                            if (GetCanLeave)
                            {
                                strSnd = "GetCanLeave,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "GetCanLeave,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("SawReady"))
                        {
                            if (SawReady)
                            {
                                strSnd = "SawReady,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "SawReady,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("LoadPackageOK"))
                        {
                            if (LoadPackageOK)
                            {
                                strSnd = "LoadPackageOK,1\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                            else
                            {
                                strSnd = "LoadPackageOK,0\n";
                                ServerSocket[0].SendText(strSnd);
                            }
                        }

                        if (strRcv.Contains("PutInPos"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                if (StrArray[1] == "1")
                                {
                                    PutInPos = true;
                                }
                                else
                                {
                                    PutInPos = false;
                                }
                            }
                        }

                        if (strRcv.Contains("PutInPos"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                if (StrArray[1] == "1")
                                {
                                    PutInPos = true;
                                }
                                else
                                {
                                    PutInPos = false;
                                }
                            }
                        }

                        if (strRcv.Contains("PutOK"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                if (StrArray[1] == "1")
                                {
                                    PutOK = true;
                                }
                                else
                                {
                                    PutOK = false;
                                }
                            }
                        }

                        if (strRcv.Contains("GetInPos"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                if (StrArray[1] == "1")
                                {
                                    GetInPos = true;
                                }
                                else
                                {
                                    GetInPos = false;
                                }
                            }
                        }

                        if (strRcv.Contains("GetOK"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                if (StrArray[1] == "1")
                                {
                                    GetOK = true;
                                }
                                else
                                {
                                    GetOK = false;
                                }
                            }
                        }

                        if (strRcv.Contains("LoadPackageName"))
                        {
                            String[] StrArray = strRcv.Split(',');
                            if (StrArray.Count() == 2)
                            {
                                LoadPackageName = StrArray[1];
                            }
                        }
                    }
                }
            }
        }
    }
}
