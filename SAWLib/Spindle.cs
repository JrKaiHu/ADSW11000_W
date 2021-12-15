using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;

namespace SAWLib
{
    [Designer(typeof(SpindleDesigner))]
    public partial class Spindle : UserControl
    {
        SerialPort sp = new SerialPort();
        string strText = string.Empty;
        int iTask = 0;
        Byte[] RcvArray = new Byte[15];
        List<byte> rcvList = new List<byte>();
        object oLocker = new object();
        AutoResetEvent _AREvt = new AutoResetEvent(false);

        public delegate void SpindleRunDelegate(eSpindleRunState State, byte ErrCode);
        public event SpindleRunDelegate OnSpindleRunState;

        BackgroundWorker bwSpindleCheck = new BackgroundWorker();
        CancellationTokenSource cts = new CancellationTokenSource();

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always), Bindable(true)]
        public override string Text
        {
            get { return this.strText; }
            set
            {
                this.strText = value;
                this.Invalidate();
            }
        }

        private Font captionFont = new Font("微軟正黑體", 12);

        public Font CaptionFont
        {
            get
            {
                return captionFont;
            }
            set
            {
                captionFont = value;
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.Enabled)
            {
                if (this.BackColor != Color.DarkGray)
                    this.BackColor = Color.DarkGray;
            }
            else
            {
                if (this.BackColor != Color.RoyalBlue)
                    this.BackColor = Color.RoyalBlue;
            }
            SizeF strSize = e.Graphics.MeasureString(this.Text, CaptionFont);
            int yPos = (this.Height - (int)strSize.Height) / 2;
            e.Graphics.DrawString(this.Text, CaptionFont, Brushes.White, new PointF(1, yPos));
        }

        bool bUseIO = false;
        [Category("ProV")]
        public bool UseIO
        {
            get { return bUseIO; }
            set { bUseIO = value; }
        }

        ProVLib.OutBit obSwitch = null;
        [Category("ProV")]
        public ProVLib.OutBit SwitchOn
        {
            get { return obSwitch; }
            set { obSwitch = value; }
        }

        ProVLib.OutBit obEnable = null;
        [Category("ProV")]
        public ProVLib.OutBit OPEnable
        {
            get { return obEnable; }
            set { obEnable = value; }
        }

        ProVLib.AnalogOut aoSpindleRPM = null;
        [Category("ProV")]
        public ProVLib.AnalogOut SpindleRPM
        {
            get { return aoSpindleRPM; }
            set { aoSpindleRPM = value; }
        }

        ProVLib.InBit ibAirPressure = null;
        [Category("ProV")]
        public ProVLib.InBit AirPressure
        {
            get { return ibAirPressure; }
            set { ibAirPressure = value; }
        }

        ProVLib.InBit ibCoolingFlow = null;
        [Category("ProV")]
        public ProVLib.InBit CoolingFlow
        {
            get { return ibCoolingFlow; }
            set { ibCoolingFlow = value; }
        }

        ProVLib.OutBit obSpindleLocker;
        public ProVLib.OutBit SpindleLocker
        {
            get { return obSpindleLocker; }
            set { obSpindleLocker = value; }
        }

        string sComPort = "COM1";
        [Category("ProV")]
        public String COMPort
        {
            get { return sComPort; }
            set { sComPort = value; }
        }

        int iBaudRate = 57600;
        [Category("ProV")]
        public int BaudRate
        {
            get { return iBaudRate; }
            set { iBaudRate = value; }
        }

        int iDataBit = 8;
        [Category("ProV")]
        public int DataBit
        {
            get { return iDataBit; }
            set { iDataBit = value; }
        }

        StopBits eStopBit = StopBits.One;
        [Category("ProV")]
        public StopBits StopBits
        {
            get { return eStopBit; }
            set { eStopBit = value; }
        }

        Parity eParityCheck = Parity.None;
        [Category("ProV")]
        public Parity ParityCheck
        {
            get { return eParityCheck; }
            set { eParityCheck = value; }
        }

        int iRefRPM = 6000;
        [Category("ProV"), Browsable(false)]
        public int RefRPM
        {
            get { return iRefRPM; }
        }

        int iActualRPM = 6000;
        [Category("ProV"), Browsable(false)]
        public int ActualRPM
        {
            get { return iActualRPM; }
        }

        float fLoadAMP = 0F;
        [Category("ProV"), Browsable(false)]
        public float LoadAMP
        {
            get { return fLoadAMP; }
        }

        float fLoadMotor = 0F;
        [Category("ProV"), Browsable(false)]
        public float LoadMotor
        {
            get { return fLoadMotor; }
        }

        ushort usStatusWord = 0;
        [Category("ProV"), Browsable(false)]
        public bool ReadyToSwitchOn
        {
            get { return (usStatusWord & 0x0001) == 0x0001; }
        }

        [Category("ProV"), Browsable(false)]
        public bool SpeedOK
        {
            get { return (usStatusWord & 0x0400) == 0x0400; }
        }

        [Category("ProV"), Browsable(false)]
        public bool SpeedZero
        {
            get { return (usStatusWord & 0x1000) == 0x1000; }
        }

        public Spindle()
        {
            InitializeComponent();
        }

        delegate bool CheckCOMPortStateDelegate();
        delegate eTaskRet ReadyToSwitchOnDelegate();

        bool IsCOMOpen()
        {
            return sp.IsOpen;
        }


        public bool Initial()
        {
            if (!this.Enabled)
                return false;

            if (bUseIO)
            {
                if (obEnable == null ||
                    obSwitch == null ||
                    aoSpindleRPM == null
                    )
                {
                    if (OnSpindleRunState != null)
                        OnSpindleRunState(eSpindleRunState.eIONotAssigned, 0x00);
                    return false;
                }
                else
                {
                    bwSpindleCheck.DoWork += bwSpindleCheck_DoWork;
                    bwSpindleCheck.WorkerSupportsCancellation = true;
                    if (bwSpindleCheck.IsBusy != true)
                    {
                        bwSpindleCheck.RunWorkerAsync(cts);
                    }
                    return true;
                }
            }
            else
            {
                if (sp.IsOpen)
                {
                    if (OnSpindleRunState != null)
                        OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                    return true;
                }
                else
                {
                    try
                    {
                        sp.PortName = sComPort;
                        sp.BaudRate = iBaudRate;
                        sp.Parity = eParityCheck;
                        sp.StopBits = eStopBit;
                        sp.DataBits = iDataBit;
                        sp.DataReceived += serialPort_DataReceived;
                        sp.Open();

                        CheckCOMPortStateDelegate checkFunc = IsCOMOpen;

                        bool bRet = SpinWait.SpinUntil(() =>
                        {
                            return checkFunc();
                        }, 3000);

                        //Change Spindle State to Ready to Switch ON
                        if (bRet)
                        {
                            ReadyToSwitchOnDelegate shutdownFunc = ShutdownDrive;
                            bool bTaskRet = SpinWait.SpinUntil(() =>
                            {
                                return (shutdownFunc() == eTaskRet.eCurrentTaskDone);
                            }, 3000);

                            if (bTaskRet)
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                            }
                            else
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                            }

                            ledState.Value = KCSDK.LEDState.Off;
                            bwSpindleCheck.DoWork += bwSpindleCheck_DoWork;
                            bwSpindleCheck.WorkerSupportsCancellation = true;
                            if (bwSpindleCheck.IsBusy != true)
                            {
                                bwSpindleCheck.RunWorkerAsync(cts);
                            }
                            return true;
                        }
                        else
                        {
                            if (OnSpindleRunState != null)
                                OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }
            }
        }

        void bwSpindleCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            CancellationTokenSource token = (CancellationTokenSource)e.Argument;
            while (!token.IsCancellationRequested)
            {
                if (!ibAirPressure.Value || !ibCoolingFlow.Value)
                    bSpindleNotSafe = true;
                else
                    bSpindleNotSafe = false;

                SpinWait.SpinUntil(() =>
                {
                    return false;
                }, 10);
            }

        }

        private bool Disconnect()
        {
            try
            {
                if (!this.Enabled)
                    return false;

                cts.Cancel();

                if (!UseIO)
                {
                    if (!sp.IsOpen)
                        return true;
                    sp.Close();
                }

                ledState.Value = KCSDK.LEDState.Off;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        //Func1:Send Shutdown Command to Drive
        //Func2:Set Reference Speed Value
        //Func3:Activate Controller
        //Func4:Operation enable  

        //主軸啟動
        //Func1 -> Func2 -> Func3 -> Func4

        //主軸關閉
        //Func3 -> Func1

        // this function compares calculated checksum with received checksum from receive buffer
        public bool CompareCheckSumm(byte[] receiveBuffer)
        {
            int length = receiveBuffer[1];
            int checksummoffset = length + 2;
            if (checksummoffset > receiveBuffer.Length - 1)
                return false;
            if (receiveBuffer[checksummoffset] == CheckSumm(receiveBuffer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //this function creates the checksum
        public byte CheckSumm(byte[] receiveBuffer)
        {
            byte Summ = 0;
            byte chk = 0xFF;

            int i = 0;
            int length = receiveBuffer[1];
            int lastIndex = length + 2;
            for (i = 1; i < lastIndex; i++)
            {
                Summ += receiveBuffer[i];
            }
            return (byte)(chk - Summ);
        }

        bool bTaskTimeout = false;
        bool bTaskInterrupt = false;

        Task<bool> TaskShutdownDrive = null;
        eTaskRet ShutdownDriveTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet ShutdownDrive()
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;
            if (
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
               )
            {
                ShutdownDriveTaskRet = eTaskRet.eAnotherTaskRunning;
                return ShutdownDriveTaskRet;
            }

            if (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun))
            {
                ShutdownDriveTaskRet = eTaskRet.eCurrentTaskRunning;
                return ShutdownDriveTaskRet;
            }
            else if (ShutdownDriveTaskRet != eTaskRet.eCurrentTaskDone && (TaskShutdownDrive != null && TaskShutdownDrive.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    ShutdownDriveTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    ShutdownDriveTaskRet = eTaskRet.eCurrentTaskDone;
                return ShutdownDriveTaskRet;
            }

            bTaskTimeout = false;
            TaskShutdownDrive = new Task<bool>(ShutdownDriveTask);
            TaskShutdownDrive.Start();   //Start Async Task
            ShutdownDriveTaskRet = eTaskRet.eCurrentTaskRunning;
            return ShutdownDriveTaskRet;
        }

        private bool ShutdownDriveTask()
        {
            bool bRet = true;
            iTask = 1;

            lock (oLocker)
                rcvList.Clear();

            if (!sp.IsOpen)
            {
                if (OnSpindleRunState != null)
                {
                    OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                }
                return false;
            }

            while (iTask < 100)
            {
                switch (iTask)
                {
                    case 1: //Shutdown Drive 
                        {
                            SpindleTask = eSpindleTask.eShutdownDrive;
                            byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                            SendWriteStr[12] = 0x06;
                            SendWriteStr[13] = 0x00;
                            SendWriteStr[14] = 0x97;

                            byte chk = 0xFF;
                            int i = 0;
                            int length = SendWriteStr[1];
                            int lastIndex = length + 2;
                            byte Summ = 0;
                            for (i = 1; i < lastIndex; i++)
                            {
                                Summ += SendWriteStr[i];
                            }
                            byte chks = (byte)(chk - Summ);

                            SendWriteStr[SendWriteStr.Length - 1] = chks;

                            sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                            iTask = 10;
                        }
                        break;
                    case 10: //Check Command Reply
                        {
                            bool bEvtRet = _AREvt.WaitOne(3000);
                            if (bEvtRet)
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                iTask = 100;
                            }
                            else
                            {
                                //Timeout
                                bTaskTimeout = true;
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                iTask = 100;
                            }
                        }
                        break;
                    case 100: //End of while loop
                        iTask = 0;
                        break;
                }
                Thread.Sleep(50);
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        Task<bool> TaskActivateController = null;
        eTaskRet ActivateControllerTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet ActivateController()
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;
            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
               )
            {
                ActivateControllerTaskRet = eTaskRet.eAnotherTaskRunning;
                return ActivateControllerTaskRet;
            }

            if (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun))
            {
                ActivateControllerTaskRet = eTaskRet.eCurrentTaskRunning;
                return ActivateControllerTaskRet;
            }
            else if (ActivateControllerTaskRet != eTaskRet.eCurrentTaskDone && (TaskActivateController != null && TaskActivateController.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    ActivateControllerTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    ActivateControllerTaskRet = eTaskRet.eCurrentTaskDone;
                return ActivateControllerTaskRet;
            }

            bTaskTimeout = false;
            TaskActivateController = new Task<bool>(ActivateControllerTask);
            TaskActivateController.Start();   //Start Async Task
            ActivateControllerTaskRet = eTaskRet.eCurrentTaskRunning;
            return ActivateControllerTaskRet;
        }

        private bool ActivateControllerTask()
        {
            bool bRet = true;
            iTask = 1;

            lock (oLocker)
                rcvList.Clear();

            if (!sp.IsOpen)
            {
                if (OnSpindleRunState != null)
                {
                    OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                }
                return false;
            }

            while (iTask < 100)
            {
                switch (iTask)
                {
                    case 1: //Activate Controller 
                        {
                            SpindleTask = eSpindleTask.eActivateController;
                            byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                            SendWriteStr[12] = 0x07;
                            SendWriteStr[13] = 0x00;
                            SendWriteStr[14] = 0x96;

                            byte chk = 0xFF;
                            int i = 0;
                            int length = SendWriteStr[1];
                            int lastIndex = length + 2;
                            byte Summ = 0;
                            for (i = 1; i < lastIndex; i++)
                            {
                                Summ += SendWriteStr[i];
                            }
                            byte chks = (byte)(chk - Summ);

                            SendWriteStr[SendWriteStr.Length - 1] = chks;

                            sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                            iTask = 10;
                        }
                        break;
                    case 10: //Check Command Reply
                        {
                            bool bEvtRet = _AREvt.WaitOne(3000);
                            if (bEvtRet)
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                iTask = 100;
                            }
                            else
                            {
                                //Timeout
                                bTaskTimeout = true;
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                iTask = 100;
                            }
                        }
                        break;
                    case 100: //End of while loop
                        iTask = 0;
                        break;
                }
                Thread.Sleep(50);
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        Task<bool> TaskOperationEnable = null;
        eTaskRet OperationEnableTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet OperationEnable()
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;

            if (obSpindleLocker.Value)
                return eTaskRet.eSpindleLockActive;

            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
               )
            {
                OperationEnableTaskRet = eTaskRet.eAnotherTaskRunning;
                return OperationEnableTaskRet;
            }

            if (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun))
            {
                OperationEnableTaskRet = eTaskRet.eCurrentTaskRunning;
                return OperationEnableTaskRet;
            }
            else if (OperationEnableTaskRet != eTaskRet.eCurrentTaskDone && (TaskOperationEnable != null && TaskOperationEnable.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    OperationEnableTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    OperationEnableTaskRet = eTaskRet.eCurrentTaskDone;
                return OperationEnableTaskRet;
            }

            bTaskTimeout = false;
            TaskOperationEnable = new Task<bool>(OperationEnableTask);
            TaskOperationEnable.Start();   //Start Async Task
            OperationEnableTaskRet = eTaskRet.eCurrentTaskRunning;
            return OperationEnableTaskRet;
        }

        private bool OperationEnableTask()
        {
            bool bRet = true;
            iTask = 1;
            lock (oLocker)
                rcvList.Clear();

            if (!sp.IsOpen)
            {
                if (OnSpindleRunState != null)
                {
                    OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                }
                return false;
            }

            while (iTask < 100)
            {
                switch (iTask)
                {
                    case 1: //Operation Enable
                        {
                            if (bSpindleNotSafe)
                            {
                                bTaskInterrupt = true;
                                iTask = 100;
                            }
                            else
                            {
                                SpindleTask = eSpindleTask.eOperationEnable;
                                byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                SendWriteStr[12] = 0x0F;
                                SendWriteStr[13] = 0x00;
                                SendWriteStr[14] = 0x8E;

                                byte chk = 0xFF;
                                int i = 0;
                                int length = SendWriteStr[1];
                                int lastIndex = length + 2;
                                byte Summ = 0;
                                for (i = 1; i < lastIndex; i++)
                                {
                                    Summ += SendWriteStr[i];
                                }
                                byte chks = (byte)(chk - Summ);

                                SendWriteStr[SendWriteStr.Length - 1] = chks;

                                sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                iTask = 10;
                            }
                        }
                        break;
                    case 10: //Check Command Reply
                        {
                            bool bEvtRet = _AREvt.WaitOne(3000);
                            if (bEvtRet)
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                iTask = 100;
                            }
                            else
                            {
                                //Timeout
                                bTaskTimeout = true;
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                iTask = 100;
                            }
                        }
                        break;
                    case 100: //End of while loop
                        iTask = 0;
                        break;
                }
                Thread.Sleep(50);
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }


        Task<bool> TaskStartRun = null;
        eTaskRet StartRunTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet StartRun(int RPM)
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;

            if (obSpindleLocker.Value)
                return eTaskRet.eSpindleLockActive;

            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
                )
            {
                StartRunTaskRet = eTaskRet.eAnotherTaskRunning;
                return StartRunTaskRet;
            }

            if (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun))
            {
                StartRunTaskRet = eTaskRet.eCurrentTaskRunning;
                return StartRunTaskRet;
            }
            else if (StartRunTaskRet != eTaskRet.eCurrentTaskDone && (TaskStartRun != null && TaskStartRun.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    StartRunTaskRet = eTaskRet.eTaskTimeout;
                }
                else if (bTaskInterrupt)
                    StartRunTaskRet = eTaskRet.eTaskInterrupt;
                else
                    StartRunTaskRet = eTaskRet.eCurrentTaskDone;
                return StartRunTaskRet;
            }

            if (RPM < 6000) RPM = 6000;
            if (RPM > 30000) RPM = 30000;

            bTaskTimeout = false;
            bTaskInterrupt = false;
            TaskStartRun = new Task<bool>(StartRunTask, RPM);
            TaskStartRun.Start();   //Start Async Task
            StartRunTaskRet = eTaskRet.eCurrentTaskRunning;
            
            return StartRunTaskRet;
        }

        eSpindleTask SpindleTask = eSpindleTask.eShutdownDrive;
        private bool StartRunTask(object rpm)
        {
            int iSetRPM = (int)rpm;
            bool bRet = true;
            iTask = 1;
            if (bUseIO)
            {
                while (iTask < 100)
                {
                    switch (iTask)
                    {
                        case 1: //Switch On
                            {
                                if (bSpindleNotSafe)
                                {
                                    iTask = 100;
                                    bTaskInterrupt = true;
                                }
                                else
                                {
                                    aoSpindleRPM.Value = iSetRPM / 6000F;
                                    obSwitch.Value = true;
                                    iTask = 10;
                                }
                            }
                            break;
                        case 10: //Check Command Reply
                            {
                                if (bSpindleNotSafe)
                                {
                                    bTaskInterrupt = true;
                                    iTask = 100;
                                }
                                else
                                {
                                    obEnable.Value = true;
                                    iTask = 100;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                }
                            }
                            break;
                        case 100: //End of while loop
                            iTask = 0;
                            break;
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {
                lock (oLocker)
                    rcvList.Clear();

                if (!sp.IsOpen)
                {
                    if (OnSpindleRunState != null)
                    {
                        OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                    }
                    return false;
                }

                while (iTask < 100)
                {
                    switch (iTask)
                    {
                        case 1: //Send Shutdown Command to Drive 
                            {
                                if (bSpindleNotSafe)
                                {
                                    bTaskInterrupt = true;
                                    iTask = 100;
                                }
                                else
                                {
                                    SpindleTask = eSpindleTask.eShutdownDrive;
                                    byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                    SendWriteStr[12] = 0x06;
                                    SendWriteStr[13] = 0x00;
                                    SendWriteStr[14] = 0x97;

                                    byte chk = 0xFF;
                                    int i = 0;
                                    int length = SendWriteStr[1];
                                    int lastIndex = length + 2;
                                    byte Summ = 0;
                                    for (i = 1; i < lastIndex; i++)
                                    {
                                        Summ += SendWriteStr[i];
                                    }
                                    byte chks = (byte)(chk - Summ);

                                    SendWriteStr[SendWriteStr.Length - 1] = chks;

                                    sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                    iTask = 10;
                                }
                            }
                            break;
                        case 10: //Check Shutdown Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    iTask = 20;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 20: //Change Spindle Speed
                            {
                                if (bSpindleNotSafe)
                                {
                                    bTaskInterrupt = true;
                                    iTask = 100;
                                }
                                else
                                {
                                    SpindleTask = eSpindleTask.eChangeSpeed;
                                    byte[] SendWriteStr = { 0x00, 0x0e, 0x02, 0x01, 0x0e, 0x8b, 0x01, 0x00, 0x00, 0x00, 0x00, 0x04, 0x80, 0x96, 0x98, 0x00, 0xa5 };   //10000
                                    SPINDLEDATA sData = new SPINDLEDATA();
                                    sData.i4Data = iSetRPM * 1000;
                                    SendWriteStr[12] = sData.b0;
                                    SendWriteStr[13] = sData.b1;
                                    SendWriteStr[14] = sData.b2;
                                    SendWriteStr[15] = sData.b3;

                                    byte chk = 0xFF;
                                    int i = 0;
                                    int length = SendWriteStr[1];
                                    int lastIndex = length + 2;
                                    byte Summ = 0;
                                    for (i = 1; i < lastIndex; i++)
                                    {
                                        Summ += SendWriteStr[i];
                                    }
                                    byte chks = (byte)(chk - Summ);

                                    SendWriteStr[SendWriteStr.Length - 1] = chks;

                                    sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                    iTask = 30;
                                }
                            }
                            break;
                        case 30: //Check Change Speed Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    iTask = 40;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 40: //Activate Controller 
                            {
                                if (bSpindleNotSafe)
                                {
                                    bTaskInterrupt = true;
                                    iTask = 100;
                                }
                                else
                                {
                                    SpindleTask = eSpindleTask.eActivateController;
                                    byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                    SendWriteStr[12] = 0x07;
                                    SendWriteStr[13] = 0x00;
                                    SendWriteStr[14] = 0x96;

                                    byte chk = 0xFF;
                                    int i = 0;
                                    int length = SendWriteStr[1];
                                    int lastIndex = length + 2;
                                    byte Summ = 0;
                                    for (i = 1; i < lastIndex; i++)
                                    {
                                        Summ += SendWriteStr[i];
                                    }
                                    byte chks = (byte)(chk - Summ);

                                    SendWriteStr[SendWriteStr.Length - 1] = chks;

                                    sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                    iTask = 50;
                                }
                            }
                            break;
                        case 50: //Check Shutdown Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    iTask = 60;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 60: //Operation Enable 
                            {
                                if (bSpindleNotSafe)
                                {
                                    bTaskInterrupt = true;
                                    iTask = 100;
                                }
                                else
                                {
                                    SpindleTask = eSpindleTask.eOperationEnable;
                                    byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                    SendWriteStr[12] = 0x0F;
                                    SendWriteStr[13] = 0x00;
                                    SendWriteStr[14] = 0x8E;

                                    byte chk = 0xFF;
                                    int i = 0;
                                    int length = SendWriteStr[1];
                                    int lastIndex = length + 2;
                                    byte Summ = 0;
                                    for (i = 1; i < lastIndex; i++)
                                    {
                                        Summ += SendWriteStr[i];
                                    }
                                    byte chks = (byte)(chk - Summ);

                                    SendWriteStr[SendWriteStr.Length - 1] = chks;

                                    sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                    iTask = 70;
                                }
                            }
                            break;
                        case 70: //Check Shutdown Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                    iTask = 100;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 100: //End of while loop
                            iTask = 0;
                            break;
                    }
                    Thread.Sleep(50);
                }
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        bool bSpindleNotSafe = false;
        Task<bool> TaskStopRun = null;
        eTaskRet StopRunTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet StopRun()
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;
            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
               )
            {
                StopRunTaskRet = eTaskRet.eAnotherTaskRunning;
                return StopRunTaskRet;
            }

            if (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun))
            {
                StopRunTaskRet = eTaskRet.eCurrentTaskRunning;
                return StopRunTaskRet;
            }
            else if (StopRunTaskRet != eTaskRet.eCurrentTaskDone && (TaskStopRun != null && TaskStopRun.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    StopRunTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    StopRunTaskRet = eTaskRet.eCurrentTaskDone;
                return StopRunTaskRet;
            }

            bTaskTimeout = false;
            TaskStopRun = new Task<bool>(StopRunTask);
            TaskStopRun.Start();   //Start Async Task
            StopRunTaskRet = eTaskRet.eCurrentTaskRunning;
            return StopRunTaskRet;
        }

        private bool StopRunTask()
        {
            bool bRet = true;
            iTask = 1;
            if (bUseIO)
            {
                while (iTask < 100)
                {
                    switch (iTask)
                    {
                        case 1: //Operation Disable
                            {
                                obEnable.Value = false;
                                iTask = 10;
                            }
                            break;
                        case 10: //Switch Off
                            {
                                obSwitch.Value = false;
                                iTask = 100;
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                            }
                            break;
                        case 100: //End of while loop
                            iTask = 0;
                            break;
                    }
                    Thread.Sleep(100);
                }
            }
            else
            {

                lock (oLocker)
                    rcvList.Clear();

                if (!sp.IsOpen)
                {
                    if (OnSpindleRunState != null)
                    {
                        OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                    }
                    return false;
                }

                while (iTask < 100)
                {
                    switch (iTask)
                    {
                        case 1: //Activate Controller 
                            {
                                SpindleTask = eSpindleTask.eShutdownDrive;
                                byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                SendWriteStr[12] = 0x07;
                                SendWriteStr[13] = 0x00;
                                SendWriteStr[14] = 0x96;

                                byte chk = 0xFF;
                                int i = 0;
                                int length = SendWriteStr[1];
                                int lastIndex = length + 2;
                                byte Summ = 0;
                                for (i = 1; i < lastIndex; i++)
                                {
                                    Summ += SendWriteStr[i];
                                }
                                byte chks = (byte)(chk - Summ);

                                SendWriteStr[SendWriteStr.Length - 1] = chks;

                                sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                iTask = 10;
                            }
                            break;
                        case 10: //Check Change Speed Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    iTask = 20;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 20: //Activate Controller 
                            {
                                SpindleTask = eSpindleTask.eActivateController;
                                byte[] SendWriteStr = { 0x00, 0x0b, 0x02, 0x01, 0x0e, 0x44, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x06, 0x00, 0x97 };
                                SendWriteStr[12] = 0x06;
                                SendWriteStr[13] = 0x00;
                                SendWriteStr[14] = 0x97;

                                byte chk = 0xFF;
                                int i = 0;
                                int length = SendWriteStr[1];
                                int lastIndex = length + 2;
                                byte Summ = 0;
                                for (i = 1; i < lastIndex; i++)
                                {
                                    Summ += SendWriteStr[i];
                                }
                                byte chks = (byte)(chk - Summ);

                                SendWriteStr[SendWriteStr.Length - 1] = chks;

                                sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                iTask = 30;
                            }
                            break;
                        case 30: //Check Shutdown Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                    iTask = 100;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 100: //End of while loop
                            iTask = 0;
                            break;
                    }
                    Thread.Sleep(50);
                }
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        Task<bool> TaskChangeSpeed = null;
        eTaskRet ChangeSpindleSpeedTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet ChangeSpindleSpeed(int RPM)
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;
            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
               )
            {
                ChangeSpindleSpeedTaskRet = eTaskRet.eAnotherTaskRunning;
                return ChangeSpindleSpeedTaskRet;
            }

            if (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun))
            {
                ChangeSpindleSpeedTaskRet = eTaskRet.eCurrentTaskRunning;
                return ChangeSpindleSpeedTaskRet;
            }
            else if (ChangeSpindleSpeedTaskRet != eTaskRet.eCurrentTaskDone && (TaskChangeSpeed != null && TaskChangeSpeed.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    ChangeSpindleSpeedTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    ChangeSpindleSpeedTaskRet = eTaskRet.eCurrentTaskDone;
                return ChangeSpindleSpeedTaskRet;
            }

            bTaskTimeout = false;
            TaskChangeSpeed = new Task<bool>(ChangeSpeedTask, RPM);
            TaskChangeSpeed.Start();   //Start Async Task
            ChangeSpindleSpeedTaskRet = eTaskRet.eAnotherTaskRunning;
            return ChangeSpindleSpeedTaskRet;
        }

        private bool ChangeSpeedTask(object RPM)
        {
            int iSetRPM = (int)RPM;
            bool bRet = true;
            iTask = 1;
            if (bUseIO)
            {
                aoSpindleRPM.Value = iSetRPM / 6000F;
            }
            else
            {
                lock (oLocker)
                    rcvList.Clear();

                if (!sp.IsOpen)
                {
                    if (OnSpindleRunState != null)
                    {
                        OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                    }
                    return false;
                }

                while (iTask < 100)
                {
                    switch (iTask)
                    {
                        case 1: //Change Spindle Speed
                            {
                                SpindleTask = eSpindleTask.eChangeSpeed;
                                byte[] SendWriteStr = { 0x00, 0x0e, 0x02, 0x01, 0x0e, 0x8b, 0x01, 0x00, 0x00, 0x00, 0x00, 0x04, 0x80, 0x96, 0x98, 0x00, 0xa5 };   //10000
                                SPINDLEDATA sData = new SPINDLEDATA();
                                sData.i4Data = iSetRPM * 1000;
                                SendWriteStr[12] = sData.b0;
                                SendWriteStr[13] = sData.b1;
                                SendWriteStr[14] = sData.b2;
                                SendWriteStr[15] = sData.b3;

                                byte chk = 0xFF;
                                int i = 0;
                                int length = SendWriteStr[1];
                                int lastIndex = length + 2;
                                byte Summ = 0;
                                for (i = 1; i < lastIndex; i++)
                                {
                                    Summ += SendWriteStr[i];
                                }
                                byte chks = (byte)(chk - Summ);

                                SendWriteStr[SendWriteStr.Length - 1] = chks;

                                sp.Write(SendWriteStr, 0, SendWriteStr.Length);

                                iTask = 10;
                            }
                            break;
                        case 10: //Check Change Speed Command Reply
                            {
                                bool bEvtRet = _AREvt.WaitOne(3000);
                                if (bEvtRet)
                                {
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                    iTask = 100;
                                }
                                else
                                {
                                    //Timeout
                                    bTaskTimeout = true;
                                    if (OnSpindleRunState != null)
                                        OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                    iTask = 100;
                                }
                            }
                            break;
                        case 100: //End of while loop
                            iTask = 0;
                            break;
                    }
                    Thread.Sleep(50);
                }
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        Task<bool> TaskReadStatus = null;
        eTaskRet ReadStatusTaskRet = eTaskRet.eCurrentTaskIdle;
        public eTaskRet ReadStatus(eReadStatusType Type = eReadStatusType.ActualVelocity)
        {
            if (!this.Enabled)
                return eTaskRet.eCurrentTaskIdle;
            if (
                (TaskShutdownDrive != null && (TaskShutdownDrive.Status == TaskStatus.Running || TaskShutdownDrive.Status == TaskStatus.WaitingToRun)) ||
                (TaskActivateController != null && (TaskActivateController.Status == TaskStatus.Running || TaskActivateController.Status == TaskStatus.WaitingToRun)) ||
                (TaskOperationEnable != null && (TaskOperationEnable.Status == TaskStatus.Running || TaskOperationEnable.Status == TaskStatus.WaitingToRun)) ||
                (TaskStartRun != null && (TaskStartRun.Status == TaskStatus.Running || TaskStartRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskStopRun != null && (TaskStopRun.Status == TaskStatus.Running || TaskStopRun.Status == TaskStatus.WaitingToRun)) ||
                (TaskChangeSpeed != null && (TaskChangeSpeed.Status == TaskStatus.Running || TaskChangeSpeed.Status == TaskStatus.WaitingToRun))
               )
            {
                ReadStatusTaskRet = eTaskRet.eAnotherTaskRunning;
                return ReadStatusTaskRet;
            }

            if (TaskReadStatus != null && (TaskReadStatus.Status == TaskStatus.Running || TaskReadStatus.Status == TaskStatus.WaitingToRun))
            {
                ReadStatusTaskRet = eTaskRet.eCurrentTaskRunning;
                return ReadStatusTaskRet;
            }
            else if (ReadStatusTaskRet != eTaskRet.eCurrentTaskDone && (TaskReadStatus != null && TaskReadStatus.Status == TaskStatus.RanToCompletion))
            {
                if (bTaskTimeout)
                {
                    bTaskTimeout = false;
                    ReadStatusTaskRet = eTaskRet.eTaskTimeout;
                }
                else
                    ReadStatusTaskRet = eTaskRet.eCurrentTaskDone;
                return ReadStatusTaskRet;
            }

            bTaskTimeout = false;
            TaskReadStatus = new Task<bool>(ReadStatusTask, Type);
            TaskReadStatus.Start();   //Start Async Task
            ReadStatusTaskRet = eTaskRet.eCurrentTaskRunning;
            return ReadStatusTaskRet;
        }

        private bool ReadStatusTask(object Type)
        {
            int iDataType = (int)Type;
            bool bRet = true;
            iTask = 1;
            lock(oLocker)
                rcvList.Clear();

            if (!sp.IsOpen)
            {
                if (OnSpindleRunState != null)
                {
                    OnSpindleRunState(eSpindleRunState.eCOMPortNotOpen, 0x00);
                }
                return false;
            }

            while (iTask < 100)
            {
                switch (iTask)
                {
                    case 1: 
                        {
                            if (iDataType == 395)
                                SpindleTask = eSpindleTask.eActualVelocityUUINT;
                            else if (iDataType == 398)
                                SpindleTask = eSpindleTask.eActualVelocity;
                            else if (iDataType == 39)
                                SpindleTask = eSpindleTask.ePowerStageLoad;
                            else if (iDataType == 65)
                                SpindleTask = eSpindleTask.eMotorLoad;
                            else if (iDataType == 67)
                                SpindleTask = eSpindleTask.eStatusWord;
                            byte[] SendSpindleSpeedMsg = { 0x00, 0x09, 0x02, 0x01, 0x0d, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFF };
                            SPINDLEDATA sData = new SPINDLEDATA();
                            sData.i4Data = iDataType;
                            SendSpindleSpeedMsg[5] = sData.b0;
                            SendSpindleSpeedMsg[6] = sData.b1;

                            byte chk = 0xFF;
                            int i = 0;
                            int length = SendSpindleSpeedMsg[1];
                            int lastIndex = length + 2;
                            byte Summ = 0;
                            for (i = 1; i < lastIndex; i++)
                            {
                                Summ += SendSpindleSpeedMsg[i];
                            }
                            byte chks = (byte)(chk - Summ);

                            SendSpindleSpeedMsg[SendSpindleSpeedMsg.Length - 1] = chks;

                            sp.Write(SendSpindleSpeedMsg, 0, SendSpindleSpeedMsg.Length);

                            iTask = 10;
                        }
                        break;
                    case 10: //Check Change Speed Command Reply
                        {
                            bool bEvtRet = _AREvt.WaitOne(3000);
                            if (bEvtRet)
                            {
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eOK, 0x00);
                                iTask = 100;
                            }
                            else
                            {
                                //Timeout
                                bTaskTimeout = true;
                                if (OnSpindleRunState != null)
                                    OnSpindleRunState(eSpindleRunState.eTimeout, 0x00);
                                iTask = 100;
                            }
                        }
                        break;
                    case 100: //End of while loop
                        iTask = 0;
                        break;
                }
                Thread.Sleep(50);
            }
            SpindleTask = eSpindleTask.eNone;
            return bRet;
        }

        
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int size = sp.BytesToRead;
            Byte[] buffer = new Byte[size];
            Int32 length = sp.Read(buffer, 0, size);
            lock (oLocker)
                rcvList.AddRange(buffer);

            if (SpindleTask == eSpindleTask.eShutdownDrive ||
                SpindleTask == eSpindleTask.eActivateController ||
                SpindleTask == eSpindleTask.eOperationEnable ||
                SpindleTask == eSpindleTask.eChangeSpeed)
            {
                if (rcvList.Count >= 7)
                {
                    if (rcvList[5] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[5]);
                    }

                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
            else if (SpindleTask == eSpindleTask.eActualVelocity)
            {
                if (rcvList.Count >= 12)
                {
                    if (rcvList[6] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[6]);
                    }

                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        byte iDataCount = rcvList[5];

                        SPINDLEDATA iData = new SPINDLEDATA();
                        iData.b0 = rcvList[7];
                        iData.b1 = rcvList[8];
                        iData.b2 = rcvList[9];
                        iData.b3 = rcvList[10];

                        this.iActualRPM = iData.i4Data / 1000;
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
            else if(SpindleTask == eSpindleTask.eActualVelocityUUINT)
            {
                if (rcvList.Count >= 12)
                {
                    if (rcvList[6] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[6]);
                    }
                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        int iDataCount = rcvList[5];

                        SPINDLEDATA iData = new SPINDLEDATA();
                        iData.b0 = rcvList[7];
                        iData.b1 = rcvList[8];
                        iData.b2 = rcvList[9];
                        iData.b3 = rcvList[10];

                        this.iRefRPM = iData.i4Data / 1000;
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
            else if (SpindleTask == eSpindleTask.ePowerStageLoad)
            {
                if (rcvList.Count >= 10)
                {
                    if (rcvList[6] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[6]);
                    }
                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        int iDataCount = rcvList[5];

                        SPINDLEDATA iData = new SPINDLEDATA();
                        iData.b0 = rcvList[7];
                        iData.b1 = rcvList[8];

                        this.fLoadAMP = iData.i4Data / 10F;
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
            else if (SpindleTask == eSpindleTask.eMotorLoad)
            {
                if (rcvList.Count >= 10)
                {
                    if (rcvList[6] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[6]);
                    }
                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        int iDataCount = rcvList[5];

                        SPINDLEDATA iData = new SPINDLEDATA();
                        iData.b0 = rcvList[7];
                        iData.b1 = rcvList[8];

                        this.fLoadMotor = iData.i4Data / 10F;
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
            else if (SpindleTask == eSpindleTask.eStatusWord)
            {
                if (rcvList.Count >= 10)
                {
                    if (rcvList[6] != 0x00)
                    {
                        if (OnSpindleRunState != null)
                            OnSpindleRunState(eSpindleRunState.eCommReplyError, rcvList[6]);
                    }
                    if (CompareCheckSumm(rcvList.ToArray()))
                    {
                        int iDataCount =rcvList[5];

                        SPINDLEDATA iData = new SPINDLEDATA();
                        iData.b0 = rcvList[7];
                        iData.b1 = rcvList[8];

                        this.usStatusWord = iData.u2Data;
                        _AREvt.Set();
                    }
                    else
                    {
                        _AREvt.Set();
                    }
                }
            }
        }
    }

    public enum eSpindleRunState
    {
        eOK = 0,
        eCOMPortNotOpen = 1,
        eTimeout = 2,
        eCommReplyError,
        eIONotAssigned
    }

    public enum eCommState
    {
        eIDLE = 0,
        eBUSY,
        eTIMEOUT,
    }

    public enum eSpindleTask
    {
        eNone = -1,
        eShutdownDrive = 0,
        eActivateController = 1,
        eOperationEnable = 2,
        eChangeSpeed = 3,
        eActualVelocity = 4,
        eActualVelocityUUINT = 5,
        ePowerStageLoad = 6,
        eMotorLoad = 7,
        eStatusWord = 8
    }

    public enum eReadStatusType
    {
        ActualVelocity = 398,
        ReferenceVelocity = 395,
        ActualDriverLoad = 39,
        ActualMotorLoad = 65,
        DeviceStatus = 67
    }

    public enum eTaskRet
    {
        eCurrentTaskIdle = -1,
        eAnotherTaskRunning = 0,
        eCurrentTaskRunning,
        eCurrentTaskDone,
        eTaskTimeout,
        eTaskInterrupt,
        eSpindleLockActive
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SPINDLEDATA
    {
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;
        [FieldOffset(4)]
        public byte b4;
        [FieldOffset(5)]
        public byte b5;
        [FieldOffset(6)]
        public byte b6;
        [FieldOffset(7)]
        public byte b7;
        [FieldOffset(0)]
        public Char c2Data;
        [FieldOffset(0)]
        public Int16 i2Data;
        [FieldOffset(0)]
        public Int32 i4Data;
        [FieldOffset(0)]
        public Int64 i8Data;
        [FieldOffset(0)]
        public UInt16 u2Data;
        [FieldOffset(0)]
        public UInt32 u4Data;
        [FieldOffset(0)]
        public UInt64 u8Data;
        [FieldOffset(0)]
        public Single f4Data;
        [FieldOffset(0)]
        public Double f8Data;
    }
}
