using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;//需using此才能使用user32.dll
using ProVTool;
using ProVIFM;
using CommonObj;


//Name:CMS->Communication Middle Stage
//Developer:Woody
//Version:v.0.0.0.1
//Company:ProV
//本程式提供一個中介(Client)端
//Handler可透過此程式與Server端溝通

namespace Saw
{
    public partial class CMS : Form
    {
        const int iCCDTimeOut = 5000;   //5秒
        //宣告
        IntPtr usercontrol_handle;//紀錄此使用者控制項的handle指標
        private ProVClientSocket Client = new ProVClientSocket();
        private String IP = "";
        private String Port = "";
        private bool ConnectOk = false;
        private bool DisConnectOk = false;
        private String Result = "";
        private Dictionary<String, String> ServerResult = new Dictionary<String, String>();

        int iCCDHandle = 0;
        int iWMsgFCmd_Step = 0;
        string[] sWMsgCMD = null;
        string[] sWMsgResult = null;
        ProVLib.MyTimer oCCDTimer = new ProVLib.MyTimer(true);
        bool bCCDTimerStart = false;
        //Send Message使用
        //private const int WM_MSGDATA = 0x0400;
        //private const int WM_SMCMD = 0x0401;//SM_CMD傳遞
        //private const int WM_SYSRUN = 0x0402;//SM_CMD傳遞
        //private const int WM_CCD = 0x0403;//SM_CMD傳遞
        private const int WM_COPYDATA = 0x004A;

        // for vertical and cross line
        public event EventHandler CrossLineEvent; 
        public event EventHandler VerticalLineEvent;

        //結構宣告
        public struct MSGDataStruct
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        //使用方式
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        private static extern bool PostMessage
        (
            IntPtr hWnd,
            uint Msg,
            int wParam,
            ref  MSGDataStruct lParam
        );

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage
        (
            int hWnd,
            int Msg,
            int wParam,
            ref  MSGDataStruct lParam
        );
        //建構式
        public CMS()
        {
            InitializeComponent();
            usercontrol_handle = this.Handle;
            //事件鏈結
            Client.OnConnect += Client_OnConnect;
            Client.OnDisconnect += Client_OnDisconnect;
            Client.OnRead += Client_OnRead;
        }

        //與Server-視覺進入連線觸發event
        private void Client_OnConnect(object sender)
        {
            ConnectOk = true;
        }

        //與Server-視覺失去連線觸發Event
        private void Client_OnDisconnect(object sender, System.Net.Sockets.Socket socket)
        {
            ConnectOk = false;
            DisConnectOk = true;
            dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "", "IP:" + IP + "\nPort:" + Port + "\nServer is disconnected unclearly.");
            //MessageBox.Show("IP:" + IP + "\nPort:" + Port + "\nServer端不明中斷");
        }

        //
        /// <summary>
        /// 提供設定連接Server的IP與Port
        /// </summary>
        /// <param name="IP">Server端的IP位址</param>
        /// <param name="Port">Server端的Port</param>
        /// <returns></returns>
        public bool SetIPAndPort(String IP, int Port)
        {
            this.IP = IP;
            this.Port = Convert.ToString(Port);
            if (Client != null)
            {
                Client.IP = IP;
                Client.Port = Port;
                return true;
            }
            else
                return false;
        }
        
        /// <summary>
        /// 與連線Server連線
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            if (Client != null)
            {
                try
                {
                    Client.Connect();
                }
                catch
                {
                    ConnectOk = false;
                }
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 讀取連線是否成功
        /// </summary>
        /// <returns></returns>
        public bool Connect_Result()
        {
            return ConnectOk;
        }

        /// <summary>
        /// 與Sever斷線
        /// </summary>
        /// <returns></returns>
        public bool DisConnect()
        {
            if (ConnectOk)
            {
                try
                {
                    Client.Disconnect();
                    ConnectOk = false;
                    DisConnectOk = true;
                }
                catch
                {
                    DisConnectOk = false;
                }
            }
            return true;
        }

        /// <summary>
        /// 讀取斷線是否成功
        /// </summary>
        /// <returns></returns>
        public bool DisConnect_Result()
        {
            return DisConnectOk;
        }

        public void Set_CCDHandle(int iSet_CCDHandle)
        {
            iCCDHandle = iSet_CCDHandle;
        }

        /// <summary>
        /// Socket-Client端送命令給Server端
        /// </summary>
        /// <param name="ClientCmd"></param>
        /// <returns></returns>
        public bool SendCmd(String ClientCmd)
        {
            //連線成功，才能送命令
            if (ConnectOk)
            {
                Client.Socket.SendText(ClientCmd);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Send Message通訊命令傳遞
        /// </summary>
        /// <param name="WindowName">Sever端視窗名稱，例"Server"</param>
        /// <param name="iMsg">傳送訊息的記憶體位置，例0x004A</param>
        /// <param name="dwData"></param>
        /// <param name="strURL">通訊命令</param>
        public void SendCmd(string WindowName, int iMsg, IntPtr dwData, string ClientCmd)
        {
            MSGDataStruct cds;
            if (ClientCmd == null)
                ClientCmd = "";
            cds.dwData = dwData;
            cds.lpData = ClientCmd;    //消息
            cds.cbData = System.Text.Encoding.Default.GetBytes(ClientCmd).Length + 1;
            //命令傳送
            SendMessage(FindWindow(null, WindowName), iMsg, 0, ref cds);
        }

        /// <summary>
        /// Send Message通訊命令傳遞
        /// </summary>
        /// <param name="iCCDHDL">CCD 視窗的 Handle 值</param>
        /// <param name="iMsg">傳送訊息的記憶體位置，例0x004A</param>
        /// <param name="dwData"></param>
        /// <param name="strURL">通訊命令</param>
        public void SendCmd(int iCCDHDL, int iMsg, IntPtr dwData, string ClientCmd)
        {
            MSGDataStruct cds;
            if (ClientCmd == null)
                ClientCmd = "";
            cds.dwData = dwData;
            cds.lpData = ClientCmd;    //消息
            cds.cbData = System.Text.Encoding.Default.GetBytes(ClientCmd).Length + 1;
            //命令傳送
            SendMessage(iCCDHDL, iMsg, 0, ref cds);
        }

        /// <summary>
        /// Window Message 送命令給 CCD
        /// </summary>
        /// <param name="sCmd">命令</param>      
        /// 

        public List<string> CMSLogSayMessage = new List<string>() { };

        public void WMsgSendCmd(string sCmd)
        {
           SendCmd(iCCDHandle, 0x004A, (IntPtr)0, sCmd);
        }

        public bool WMsgFullSendCmd()
        {
            bool bRet = false;

            if (iWMsgFCmd_Step == 0)
            {
                iWMsgFCmd_Step = 1;
                sWMsgResult = null;
                sWMsgResult = null;
                bCCDTimerStart = false;
                bRet = true;
            }
            return bRet;
        }

        public int WMsgFullSendCmd(string sCmd)
        {
            int iRet = 2;   //-1:Busy 太久 , 0: 失敗 , 1:成功 , 2:Busy , 3:輸入指令個數為 0 , 4:回傳指令個數為 0 , 5:輸入指令不等於回傳結果指令 , 6:回傳結果有問題
           
            switch (iWMsgFCmd_Step)
            {
                case 0:
                    break;
                case 1:
                    sWMsgCMD = sCmd.Split(',');
                    if (sWMsgCMD.Length > 0)
                    {
                        WMsgSendCmd(sCmd);
                        iWMsgFCmd_Step = 10;
                    }
                    else
                    {
                        iRet = 3;
                        sWMsgCMD = null;
                        sWMsgResult = null;
                        iWMsgFCmd_Step = 0;
                    }
                    break;
                case 10:
                    string sResult = GetResult(sWMsgCMD[0]);

                    sResult = sResult.Substring(0, sResult.Length - 1);
                    sWMsgResult = sResult.Split(',');
                    if (sWMsgResult.Length > 0)
                    {
                        if (sWMsgCMD[0] == sWMsgResult[0])
                        {
                            switch (sWMsgResult[1])
                            { 
                                case "0":
                                case "1":
                                    iRet = Convert.ToInt32(sWMsgResult[1]);
                                    sWMsgCMD = null;
                                    sWMsgResult = null;
                                    iWMsgFCmd_Step = 0;
                                    break;
                                case "2":
                                    if (bCCDTimerStart)
                                    {
                                        if (oCCDTimer.On(iCCDTimeOut))
                                        {
                                            iRet = -1;
                                            sWMsgCMD = null;
                                            sWMsgResult = null;
                                            iWMsgFCmd_Step = 0;
                                        }
                                        else
                                        {
                                            iRet = 2;
                                            sWMsgResult = null;
                                        }
                                    }
                                    else
                                    {
                                        iRet = 2;
                                        bCCDTimerStart = true;
                                        oCCDTimer.Restart();
                                    }
                                    sWMsgResult = null;
                                    break;
                                default:
                                    iRet = 6;
                                    sWMsgCMD = null;
                                    sWMsgResult = null;
                                    iWMsgFCmd_Step = 0;
                                    break;
                            }
                        }
                        else
                        {
                            iRet = 5;
                            sWMsgCMD = null;
                            sWMsgResult = null;
                            iWMsgFCmd_Step = 0;
                        }
                    }
                    else
                    {
                        iRet = 4;
                        sWMsgCMD = null;
                        sWMsgResult=null;
                        iWMsgFCmd_Step = 0;
                    }
                    break;
            }
            return iRet;
        }

        public int WMsgFullSendCmd(string sCmd, out object[] paras)
        {      
            int iRet = 2;
            // 0: 失敗，1:辨識成功，2:Busy，3:辨識失敗

            paras = null;

            switch (iWMsgFCmd_Step)
            {
                case 0:
                    break;
                case 1:
                    {
                        sWMsgCMD = sCmd.Split(',');
                        if (sWMsgCMD.Length > 0)
                        {
                            String strSend = String.Format("[SND] {0}", sCmd);
                            CMSLogSayMessage.Add(strSend);

                            WMsgSendCmd(sCmd);
                            iWMsgFCmd_Step = 10;
                        }
                        else//輸入指令個數為 0 
                        {
                            iRet = 0;
                            paras = new string[1];
                            paras[0] = "Cmd is empty";


                            sWMsgCMD = null;
                            sWMsgResult = null;
                            iWMsgFCmd_Step = 0;
                        }
                    }
                    break;
                case 10:
                    string sResult = GetResult(sWMsgCMD[0]);

                    sResult = sResult.Substring(0, sResult.Length - 1);
                    String strRcv = String.Format("[RCV] {0}", sResult);

                    sWMsgResult = sResult.Split(',');
                    if (sWMsgResult.Length > 0)
                    {
                        if (sWMsgCMD[0] == sWMsgResult[0])
                        {
                            switch (sWMsgResult[1])
                            {
                                case "0":
                                case "1":
                                    {
                                        CMSLogSayMessage.Add(strRcv);//Record
                                        
                                        iRet = Convert.ToInt32(sWMsgResult[1]);

                                        if (strRcv.Contains("ERR") && iRet == 1)//辨識失敗
                                            iRet = 3;

                                        paras = new string[sWMsgResult.Length - 2];
                                        for (int i = 2; i < sWMsgResult.Length; i++)
                                            paras[i - 2] = sWMsgResult[i];


                                        sWMsgCMD = null;
                                        sWMsgResult = null;
                                        iWMsgFCmd_Step = 0;

                                        
                                    }
                                    break;
                                case "2":
                                    if (bCCDTimerStart)
                                    {
                                        if (oCCDTimer.On(iCCDTimeOut))
                                        {
                                            iRet = 0;
                                            paras = new string[1];
                                            paras[0] = "Vision communication timeout";

                                            sWMsgCMD = null;
                                            sWMsgResult = null;
                                            iWMsgFCmd_Step = 0;
                                        }
                                        else
                                        {
                                            iRet = 2;
                                            sWMsgResult = null;
                                        }
                                    }
                                    else
                                    {
                                        iRet = 2;
                                        bCCDTimerStart = true;
                                        oCCDTimer.Restart();
                                    }
                                    sWMsgResult = null;
                                    break;
                                default:
                                    iRet = 0;
                                    paras = new string[1];
                                    paras[0] = "Result is not define";
                                    CMSLogSayMessage.Add(strRcv);//Record

                                    sWMsgCMD = null;
                                    sWMsgResult = null;
                                    iWMsgFCmd_Step = 0;
                                    break;
                            }
                        }
                        else//輸入指令不等於回傳結果指令
                        {
                            iRet = 0;
                            paras = new string[1];
                            paras[0] = "Result is not equal to cmd";
                            CMSLogSayMessage.Add(strRcv);//Record

                            sWMsgCMD = null;
                            sWMsgResult = null;
                            iWMsgFCmd_Step = 0;
                        }
                    }
                    else//回傳指令個數為 0
                    {
                        iRet = 0;
                        paras = new string[1];
                        paras[0] = "Result is empty";
                        CMSLogSayMessage.Add(strRcv);//Record

                        sWMsgCMD = null;
                        sWMsgResult = null;
                        iWMsgFCmd_Step = 0;
                    }
                    break;
            }
            
            return iRet;
        }

        //[event]接收到Server端回傳命令觸發
        private void Client_OnRead(ProVTool.SocketClient sender)
        {
            lock (ServerResult)
            {
                String temp = sender.ReadText();
                char[] delimiterChars = { ',' };
                String[] Words = temp.Split(delimiterChars);
                ServerResult.Add(Words[0], temp);
            }
        }
        //命令接收
        protected override void WndProc(ref System.Windows.Forms.Message e)
        {
            switch (e.Msg)
            {
                case WM_COPYDATA:
                    {
                        lock (ServerResult)
                        {
                            MSGDataStruct cds = (MSGDataStruct)e.GetLParam(typeof(MSGDataStruct));
                            string strCmdText = string.Empty;
                            strCmdText = cds.lpData.ToString();
                            char[] delimiterChars = { ',' };
                            String[] Words = strCmdText.Split(delimiterChars);

                            if (Words[0] == "CC" && (Words[1] == "1;" || Words[1] == "0;"))
                            {
                                ServerEventArgs evtArgs = new ServerEventArgs();
                                evtArgs.strResult = strCmdText;
                                CrossLineEvent(this, evtArgs);
                            }

                            if (Words[0] == "MKLC" && (Words[1] == "1;" || Words[1] == "0;"))
                            {
                                ServerEventArgs evtArgs = new ServerEventArgs();
                                evtArgs.strResult = strCmdText;
                                VerticalLineEvent(this, evtArgs);
                            }

                            if (ServerResult.ContainsKey(Words[0]))
                            {
                                ServerResult[Words[0]] = strCmdText;
                            }
                            else
                            {
                                ServerResult.Add(Words[0], strCmdText);
                            }
                        }
                    }
                    break;
            }

            base.WndProc(ref e);           
        }

        /// <summary>
        /// Client端讀取Server端回傳結果
        /// </summary>
        /// <param name="Key">欲讀取結果的通訊指令</param>
        /// <returns></returns>
        public String GetResult(String Key)
        {
            lock (ServerResult)
            {
                if (ServerResult.TryGetValue(Key, out Result))
                {
                    ServerResult.Remove(Key);
                    return Result;
                }
                else
                {
                    //2:表示Working
                    return Key + ",2;";
                }
            }
        }
        /// <summary>
        /// 得到此usercontrol的handle
        /// </summary>
        /// <returns></returns>
        public int GetUerControlHandle()
        {
            return (int)usercontrol_handle;
        }   
    }    
}
