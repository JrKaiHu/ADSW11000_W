namespace ADSW11000
{
    partial class MSS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabMSS = new System.Windows.Forms.TabControl();
            this.TP_StateFlow = new System.Windows.Forms.TabPage();
            this.FC_SLOW = new ProVLib.FlowChart();
            this.FC_AUTO = new ProVLib.FlowChart();
            this.FC_T2 = new ProVLib.FlowChart();
            this.FC_T1 = new ProVLib.FlowChart();
            this.FC_HOME = new ProVLib.FlowChart();
            this.FC_IDLE = new ProVLib.FlowChart();
            this.FC_LOAD_PACKAGE_RESET = new ProVLib.FlowChart();
            this.FC_LOAD_PACKAGE = new ProVLib.FlowChart();
            this.flowChart4 = new ProVLib.FlowChart();
            this.FC_MANUAL = new ProVLib.FlowChart();
            this.FC_AUTO_RESET = new ProVLib.FlowChart();
            this.flowChart2 = new ProVLib.FlowChart();
            this.flowChart3 = new ProVLib.FlowChart();
            this.FC_DELAYRUN = new ProVLib.FlowChart();
            this.flowChart1 = new ProVLib.FlowChart();
            this.act_ModuleInitial = new ProVLib.ProVBaseActor();
            this.flowChart5 = new ProVLib.FlowChart();
            this.FC_INIT = new ProVLib.FlowChart();
            this.tbState = new KCSDK.TextLabel();
            this.tpFunction = new System.Windows.Forms.TabPage();
            this.btnPM = new System.Windows.Forms.Button();
            this.btnScanData = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnUserM = new System.Windows.Forms.Button();
            this.btnScanLang = new System.Windows.Forms.Button();
            this.btnScanIO = new System.Windows.Forms.Button();
            this.tmRefresh = new System.Windows.Forms.Timer(this.components);
            this.tabMSS.SuspendLayout();
            this.TP_StateFlow.SuspendLayout();
            this.tpFunction.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMSS
            // 
            this.tabMSS.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabMSS.Controls.Add(this.TP_StateFlow);
            this.tabMSS.Controls.Add(this.tpFunction);
            this.tabMSS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMSS.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tabMSS.ItemSize = new System.Drawing.Size(220, 40);
            this.tabMSS.Location = new System.Drawing.Point(0, 0);
            this.tabMSS.Name = "tabMSS";
            this.tabMSS.SelectedIndex = 0;
            this.tabMSS.Size = new System.Drawing.Size(831, 526);
            this.tabMSS.TabIndex = 2;
            // 
            // TP_StateFlow
            // 
            this.TP_StateFlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.TP_StateFlow.Controls.Add(this.FC_SLOW);
            this.TP_StateFlow.Controls.Add(this.FC_DELAYRUN);
            this.TP_StateFlow.Controls.Add(this.act_ModuleInitial);
            this.TP_StateFlow.Controls.Add(this.flowChart2);
            this.TP_StateFlow.Controls.Add(this.flowChart5);
            this.TP_StateFlow.Controls.Add(this.FC_INIT);
            this.TP_StateFlow.Controls.Add(this.flowChart4);
            this.TP_StateFlow.Controls.Add(this.flowChart3);
            this.TP_StateFlow.Controls.Add(this.tbState);
            this.TP_StateFlow.Controls.Add(this.FC_IDLE);
            this.TP_StateFlow.Controls.Add(this.FC_T2);
            this.TP_StateFlow.Controls.Add(this.flowChart1);
            this.TP_StateFlow.Controls.Add(this.FC_LOAD_PACKAGE);
            this.TP_StateFlow.Controls.Add(this.FC_LOAD_PACKAGE_RESET);
            this.TP_StateFlow.Controls.Add(this.FC_T1);
            this.TP_StateFlow.Controls.Add(this.FC_AUTO);
            this.TP_StateFlow.Controls.Add(this.FC_AUTO_RESET);
            this.TP_StateFlow.Controls.Add(this.FC_HOME);
            this.TP_StateFlow.Controls.Add(this.FC_MANUAL);
            this.TP_StateFlow.Location = new System.Drawing.Point(4, 44);
            this.TP_StateFlow.Name = "TP_StateFlow";
            this.TP_StateFlow.Padding = new System.Windows.Forms.Padding(3);
            this.TP_StateFlow.Size = new System.Drawing.Size(823, 478);
            this.TP_StateFlow.TabIndex = 0;
            this.TP_StateFlow.Text = "狀態流程";
            // 
            // FC_SLOW
            // 
            this.FC_SLOW.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_SLOW.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_SLOW.CASE1 = null;
            this.FC_SLOW.CASE2 = null;
            this.FC_SLOW.CASE3 = null;
            this.FC_SLOW.CASE4 = null;
            this.FC_SLOW.ContinueRun = false;
            this.FC_SLOW.EndFC = null;
            this.FC_SLOW.ErrID = 0;
            this.FC_SLOW.InAlarm = false;
            this.FC_SLOW.IsFlowHead = false;
            this.FC_SLOW.Location = new System.Drawing.Point(495, 318);
            this.FC_SLOW.LockUI = false;
            this.FC_SLOW.Message = null;
            this.FC_SLOW.MsgID = 0;
            this.FC_SLOW.Name = "FC_SLOW";
            this.FC_SLOW.NEXT = this.FC_AUTO;
            this.FC_SLOW.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_SLOW.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_SLOW.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_SLOW.OverTimeSpec = 100;
            this.FC_SLOW.Running = false;
            this.FC_SLOW.Size = new System.Drawing.Size(128, 30);
            this.FC_SLOW.SlowRunCycle = -1;
            this.FC_SLOW.StartFC = null;
            this.FC_SLOW.Text = "SLOW RUN";
            this.FC_SLOW.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_SLOW_Run);
            // 
            // FC_AUTO
            // 
            this.FC_AUTO.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_AUTO.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_AUTO.CASE1 = this.FC_T2;
            this.FC_AUTO.CASE2 = this.flowChart3;
            this.FC_AUTO.CASE3 = this.FC_DELAYRUN;
            this.FC_AUTO.CASE4 = this.flowChart2;
            this.FC_AUTO.ContinueRun = false;
            this.FC_AUTO.EndFC = null;
            this.FC_AUTO.ErrID = 0;
            this.FC_AUTO.InAlarm = false;
            this.FC_AUTO.IsFlowHead = false;
            this.FC_AUTO.Location = new System.Drawing.Point(316, 259);
            this.FC_AUTO.LockUI = false;
            this.FC_AUTO.Message = null;
            this.FC_AUTO.MsgID = 0;
            this.FC_AUTO.Name = "FC_AUTO";
            this.FC_AUTO.NEXT = this.flowChart1;
            this.FC_AUTO.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_AUTO.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_AUTO.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_AUTO.OverTimeSpec = 100;
            this.FC_AUTO.Running = false;
            this.FC_AUTO.Size = new System.Drawing.Size(150, 30);
            this.FC_AUTO.SlowRunCycle = -1;
            this.FC_AUTO.StartFC = null;
            this.FC_AUTO.Text = "AUTO";
            this.FC_AUTO.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_AUTO_Run);
            // 
            // FC_T2
            // 
            this.FC_T2.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_T2.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_T2.CASE1 = null;
            this.FC_T2.CASE2 = null;
            this.FC_T2.CASE3 = null;
            this.FC_T2.CASE4 = null;
            this.FC_T2.ContinueRun = false;
            this.FC_T2.EndFC = null;
            this.FC_T2.ErrID = 0;
            this.FC_T2.InAlarm = false;
            this.FC_T2.IsFlowHead = false;
            this.FC_T2.Location = new System.Drawing.Point(624, 259);
            this.FC_T2.LockUI = false;
            this.FC_T2.Message = null;
            this.FC_T2.MsgID = 0;
            this.FC_T2.Name = "FC_T2";
            this.FC_T2.NEXT = this.FC_T1;
            this.FC_T2.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_T2.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_T2.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_T2.OverTimeSpec = 100;
            this.FC_T2.Running = false;
            this.FC_T2.Size = new System.Drawing.Size(30, 30);
            this.FC_T2.SlowRunCycle = -1;
            this.FC_T2.StartFC = null;
            // 
            // FC_T1
            // 
            this.FC_T1.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_T1.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_T1.CASE1 = null;
            this.FC_T1.CASE2 = null;
            this.FC_T1.CASE3 = null;
            this.FC_T1.CASE4 = null;
            this.FC_T1.ContinueRun = false;
            this.FC_T1.EndFC = null;
            this.FC_T1.ErrID = 0;
            this.FC_T1.InAlarm = false;
            this.FC_T1.IsFlowHead = false;
            this.FC_T1.Location = new System.Drawing.Point(624, 81);
            this.FC_T1.LockUI = false;
            this.FC_T1.Message = null;
            this.FC_T1.MsgID = 0;
            this.FC_T1.Name = "FC_T1";
            this.FC_T1.NEXT = this.FC_HOME;
            this.FC_T1.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_T1.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_T1.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_T1.OverTimeSpec = 100;
            this.FC_T1.Running = false;
            this.FC_T1.Size = new System.Drawing.Size(30, 30);
            this.FC_T1.SlowRunCycle = -1;
            this.FC_T1.StartFC = null;
            // 
            // FC_HOME
            // 
            this.FC_HOME.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_HOME.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_HOME.CASE1 = null;
            this.FC_HOME.CASE2 = null;
            this.FC_HOME.CASE3 = null;
            this.FC_HOME.CASE4 = null;
            this.FC_HOME.ContinueRun = false;
            this.FC_HOME.EndFC = null;
            this.FC_HOME.ErrID = 0;
            this.FC_HOME.InAlarm = false;
            this.FC_HOME.IsFlowHead = false;
            this.FC_HOME.Location = new System.Drawing.Point(316, 81);
            this.FC_HOME.LockUI = false;
            this.FC_HOME.Message = null;
            this.FC_HOME.MsgID = 0;
            this.FC_HOME.Name = "FC_HOME";
            this.FC_HOME.NEXT = this.FC_IDLE;
            this.FC_HOME.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_HOME.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_HOME.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_HOME.OverTimeSpec = 100;
            this.FC_HOME.Running = false;
            this.FC_HOME.Size = new System.Drawing.Size(150, 30);
            this.FC_HOME.SlowRunCycle = -1;
            this.FC_HOME.StartFC = null;
            this.FC_HOME.Text = "HOME";
            this.FC_HOME.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_HOME_Run);
            // 
            // FC_IDLE
            // 
            this.FC_IDLE.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_IDLE.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_IDLE.CASE1 = this.FC_LOAD_PACKAGE_RESET;
            this.FC_IDLE.CASE2 = this.FC_MANUAL;
            this.FC_IDLE.CASE3 = this.FC_AUTO_RESET;
            this.FC_IDLE.CASE4 = this.flowChart2;
            this.FC_IDLE.ContinueRun = false;
            this.FC_IDLE.EndFC = null;
            this.FC_IDLE.ErrID = 0;
            this.FC_IDLE.InAlarm = false;
            this.FC_IDLE.IsFlowHead = false;
            this.FC_IDLE.Location = new System.Drawing.Point(316, 140);
            this.FC_IDLE.LockUI = false;
            this.FC_IDLE.Message = null;
            this.FC_IDLE.MsgID = 0;
            this.FC_IDLE.Name = "FC_IDLE";
            this.FC_IDLE.NEXT = this.FC_HOME;
            this.FC_IDLE.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_IDLE.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_IDLE.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_IDLE.OverTimeSpec = 300;
            this.FC_IDLE.Running = false;
            this.FC_IDLE.Size = new System.Drawing.Size(150, 30);
            this.FC_IDLE.SlowRunCycle = -1;
            this.FC_IDLE.StartFC = null;
            this.FC_IDLE.Text = "IDLE(流程進入點)";
            this.FC_IDLE.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_IDLE_Run);
            // 
            // FC_LOAD_PACKAGE_RESET
            // 
            this.FC_LOAD_PACKAGE_RESET.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_LOAD_PACKAGE_RESET.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_LOAD_PACKAGE_RESET.CASE1 = this.FC_IDLE;
            this.FC_LOAD_PACKAGE_RESET.CASE2 = null;
            this.FC_LOAD_PACKAGE_RESET.CASE3 = null;
            this.FC_LOAD_PACKAGE_RESET.CASE4 = null;
            this.FC_LOAD_PACKAGE_RESET.ContinueRun = false;
            this.FC_LOAD_PACKAGE_RESET.EndFC = null;
            this.FC_LOAD_PACKAGE_RESET.ErrID = 0;
            this.FC_LOAD_PACKAGE_RESET.InAlarm = false;
            this.FC_LOAD_PACKAGE_RESET.IsFlowHead = false;
            this.FC_LOAD_PACKAGE_RESET.Location = new System.Drawing.Point(42, 140);
            this.FC_LOAD_PACKAGE_RESET.LockUI = false;
            this.FC_LOAD_PACKAGE_RESET.Message = null;
            this.FC_LOAD_PACKAGE_RESET.MsgID = 0;
            this.FC_LOAD_PACKAGE_RESET.Name = "FC_LOAD_PACKAGE_RESET";
            this.FC_LOAD_PACKAGE_RESET.NEXT = this.FC_LOAD_PACKAGE;
            this.FC_LOAD_PACKAGE_RESET.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_LOAD_PACKAGE_RESET.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_LOAD_PACKAGE_RESET.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_LOAD_PACKAGE_RESET.OverTimeSpec = 100;
            this.FC_LOAD_PACKAGE_RESET.Running = false;
            this.FC_LOAD_PACKAGE_RESET.Size = new System.Drawing.Size(150, 30);
            this.FC_LOAD_PACKAGE_RESET.SlowRunCycle = -1;
            this.FC_LOAD_PACKAGE_RESET.StartFC = null;
            this.FC_LOAD_PACKAGE_RESET.Text = "LOAD PACKAGE RESET";
            this.FC_LOAD_PACKAGE_RESET.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_LOAD_PACKAGE_RESET_Run);
            // 
            // FC_LOAD_PACKAGE
            // 
            this.FC_LOAD_PACKAGE.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_LOAD_PACKAGE.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_LOAD_PACKAGE.CASE1 = this.FC_IDLE;
            this.FC_LOAD_PACKAGE.CASE2 = null;
            this.FC_LOAD_PACKAGE.CASE3 = null;
            this.FC_LOAD_PACKAGE.CASE4 = null;
            this.FC_LOAD_PACKAGE.ContinueRun = false;
            this.FC_LOAD_PACKAGE.EndFC = null;
            this.FC_LOAD_PACKAGE.ErrID = 0;
            this.FC_LOAD_PACKAGE.InAlarm = false;
            this.FC_LOAD_PACKAGE.IsFlowHead = false;
            this.FC_LOAD_PACKAGE.Location = new System.Drawing.Point(42, 81);
            this.FC_LOAD_PACKAGE.LockUI = false;
            this.FC_LOAD_PACKAGE.Message = null;
            this.FC_LOAD_PACKAGE.MsgID = 0;
            this.FC_LOAD_PACKAGE.Name = "FC_LOAD_PACKAGE";
            this.FC_LOAD_PACKAGE.NEXT = this.flowChart4;
            this.FC_LOAD_PACKAGE.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_LOAD_PACKAGE.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_LOAD_PACKAGE.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_LOAD_PACKAGE.OverTimeSpec = 100;
            this.FC_LOAD_PACKAGE.Running = false;
            this.FC_LOAD_PACKAGE.Size = new System.Drawing.Size(150, 30);
            this.FC_LOAD_PACKAGE.SlowRunCycle = -1;
            this.FC_LOAD_PACKAGE.StartFC = null;
            this.FC_LOAD_PACKAGE.Text = "LOAD PACKAGE";
            this.FC_LOAD_PACKAGE.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_LOAD_PACKAGE_Run);
            // 
            // flowChart4
            // 
            this.flowChart4.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowChart4.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.flowChart4.CASE1 = this.FC_IDLE;
            this.flowChart4.CASE2 = null;
            this.flowChart4.CASE3 = null;
            this.flowChart4.CASE4 = null;
            this.flowChart4.ContinueRun = false;
            this.flowChart4.EndFC = null;
            this.flowChart4.ErrID = 0;
            this.flowChart4.InAlarm = false;
            this.flowChart4.IsFlowHead = false;
            this.flowChart4.Location = new System.Drawing.Point(238, 81);
            this.flowChart4.LockUI = false;
            this.flowChart4.Message = null;
            this.flowChart4.MsgID = 0;
            this.flowChart4.Name = "flowChart4";
            this.flowChart4.NEXT = this.FC_HOME;
            this.flowChart4.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.flowChart4.OrgLocation = new System.Drawing.Point(0, 0);
            this.flowChart4.OrgSize = new System.Drawing.Size(0, 0);
            this.flowChart4.OverTimeSpec = 100;
            this.flowChart4.Running = false;
            this.flowChart4.Size = new System.Drawing.Size(50, 30);
            this.flowChart4.SlowRunCycle = -1;
            this.flowChart4.StartFC = null;
            this.flowChart4.Text = "開批";
            this.flowChart4.Run += new ProVLib.FlowChart.RunEventHandler(this.flowChart4_Run);
            // 
            // FC_MANUAL
            // 
            this.FC_MANUAL.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_MANUAL.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_MANUAL.CASE1 = null;
            this.FC_MANUAL.CASE2 = null;
            this.FC_MANUAL.CASE3 = null;
            this.FC_MANUAL.CASE4 = null;
            this.FC_MANUAL.ContinueRun = false;
            this.FC_MANUAL.EndFC = null;
            this.FC_MANUAL.ErrID = 0;
            this.FC_MANUAL.InAlarm = false;
            this.FC_MANUAL.IsFlowHead = false;
            this.FC_MANUAL.Location = new System.Drawing.Point(42, 200);
            this.FC_MANUAL.LockUI = false;
            this.FC_MANUAL.Message = null;
            this.FC_MANUAL.MsgID = 0;
            this.FC_MANUAL.Name = "FC_MANUAL";
            this.FC_MANUAL.NEXT = this.FC_IDLE;
            this.FC_MANUAL.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_MANUAL.OrgLocation = new System.Drawing.Point(28, 185);
            this.FC_MANUAL.OrgSize = new System.Drawing.Size(150, 30);
            this.FC_MANUAL.OverTimeSpec = 100;
            this.FC_MANUAL.Running = false;
            this.FC_MANUAL.Size = new System.Drawing.Size(150, 30);
            this.FC_MANUAL.SlowRunCycle = -1;
            this.FC_MANUAL.StartFC = null;
            this.FC_MANUAL.Text = "MANUAL";
            this.FC_MANUAL.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_MANUAL_Run);
            // 
            // FC_AUTO_RESET
            // 
            this.FC_AUTO_RESET.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_AUTO_RESET.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_AUTO_RESET.CASE1 = null;
            this.FC_AUTO_RESET.CASE2 = null;
            this.FC_AUTO_RESET.CASE3 = null;
            this.FC_AUTO_RESET.CASE4 = null;
            this.FC_AUTO_RESET.ContinueRun = false;
            this.FC_AUTO_RESET.EndFC = null;
            this.FC_AUTO_RESET.ErrID = 0;
            this.FC_AUTO_RESET.InAlarm = false;
            this.FC_AUTO_RESET.IsFlowHead = false;
            this.FC_AUTO_RESET.Location = new System.Drawing.Point(316, 200);
            this.FC_AUTO_RESET.LockUI = false;
            this.FC_AUTO_RESET.Message = null;
            this.FC_AUTO_RESET.MsgID = 0;
            this.FC_AUTO_RESET.Name = "FC_AUTO_RESET";
            this.FC_AUTO_RESET.NEXT = this.FC_AUTO;
            this.FC_AUTO_RESET.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_AUTO_RESET.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_AUTO_RESET.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_AUTO_RESET.OverTimeSpec = 100;
            this.FC_AUTO_RESET.Running = false;
            this.FC_AUTO_RESET.Size = new System.Drawing.Size(150, 30);
            this.FC_AUTO_RESET.SlowRunCycle = -1;
            this.FC_AUTO_RESET.StartFC = null;
            this.FC_AUTO_RESET.Text = "AUTO RESET";
            this.FC_AUTO_RESET.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_AUTO_RESET_Run);
            // 
            // flowChart2
            // 
            this.flowChart2.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowChart2.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.flowChart2.CASE1 = null;
            this.flowChart2.CASE2 = null;
            this.flowChart2.CASE3 = null;
            this.flowChart2.CASE4 = null;
            this.flowChart2.ContinueRun = false;
            this.flowChart2.EndFC = null;
            this.flowChart2.ErrID = 0;
            this.flowChart2.InAlarm = false;
            this.flowChart2.IsFlowHead = false;
            this.flowChart2.Location = new System.Drawing.Point(495, 200);
            this.flowChart2.LockUI = false;
            this.flowChart2.Message = null;
            this.flowChart2.MsgID = 0;
            this.flowChart2.Name = "flowChart2";
            this.flowChart2.NEXT = null;
            this.flowChart2.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.flowChart2.OrgLocation = new System.Drawing.Point(0, 0);
            this.flowChart2.OrgSize = new System.Drawing.Size(0, 0);
            this.flowChart2.OverTimeSpec = 100;
            this.flowChart2.Running = false;
            this.flowChart2.Size = new System.Drawing.Size(51, 30);
            this.flowChart2.SlowRunCycle = -1;
            this.flowChart2.StartFC = null;
            this.flowChart2.Text = "Close";
            this.flowChart2.Run += new ProVLib.FlowChart.RunEventHandler(this.flowChart2_Run);
            // 
            // flowChart3
            // 
            this.flowChart3.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowChart3.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.flowChart3.CASE1 = null;
            this.flowChart3.CASE2 = null;
            this.flowChart3.CASE3 = null;
            this.flowChart3.CASE4 = null;
            this.flowChart3.ContinueRun = false;
            this.flowChart3.EndFC = null;
            this.flowChart3.ErrID = 0;
            this.flowChart3.InAlarm = false;
            this.flowChart3.IsFlowHead = false;
            this.flowChart3.Location = new System.Drawing.Point(238, 200);
            this.flowChart3.LockUI = false;
            this.flowChart3.Message = null;
            this.flowChart3.MsgID = 0;
            this.flowChart3.Name = "flowChart3";
            this.flowChart3.NEXT = this.FC_IDLE;
            this.flowChart3.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.flowChart3.OrgLocation = new System.Drawing.Point(0, 0);
            this.flowChart3.OrgSize = new System.Drawing.Size(0, 0);
            this.flowChart3.OverTimeSpec = 100;
            this.flowChart3.Running = false;
            this.flowChart3.Size = new System.Drawing.Size(50, 30);
            this.flowChart3.SlowRunCycle = -1;
            this.flowChart3.StartFC = null;
            this.flowChart3.Text = "結批";
            this.flowChart3.Run += new ProVLib.FlowChart.RunEventHandler(this.flowChart3_Run);
            // 
            // FC_DELAYRUN
            // 
            this.FC_DELAYRUN.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_DELAYRUN.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_DELAYRUN.CASE1 = null;
            this.FC_DELAYRUN.CASE2 = null;
            this.FC_DELAYRUN.CASE3 = null;
            this.FC_DELAYRUN.CASE4 = null;
            this.FC_DELAYRUN.ContinueRun = false;
            this.FC_DELAYRUN.EndFC = null;
            this.FC_DELAYRUN.ErrID = 0;
            this.FC_DELAYRUN.InAlarm = false;
            this.FC_DELAYRUN.IsFlowHead = false;
            this.FC_DELAYRUN.Location = new System.Drawing.Point(316, 318);
            this.FC_DELAYRUN.LockUI = false;
            this.FC_DELAYRUN.Message = null;
            this.FC_DELAYRUN.MsgID = 0;
            this.FC_DELAYRUN.Name = "FC_DELAYRUN";
            this.FC_DELAYRUN.NEXT = this.FC_SLOW;
            this.FC_DELAYRUN.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_DELAYRUN.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_DELAYRUN.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_DELAYRUN.OverTimeSpec = 100;
            this.FC_DELAYRUN.Running = false;
            this.FC_DELAYRUN.Size = new System.Drawing.Size(150, 30);
            this.FC_DELAYRUN.SlowRunCycle = -1;
            this.FC_DELAYRUN.StartFC = null;
            this.FC_DELAYRUN.Text = "DELAY RUN";
            this.FC_DELAYRUN.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_DELAYRUN_Run);
            // 
            // flowChart1
            // 
            this.flowChart1.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowChart1.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.flowChart1.CASE1 = null;
            this.flowChart1.CASE2 = null;
            this.flowChart1.CASE3 = null;
            this.flowChart1.CASE4 = null;
            this.flowChart1.ContinueRun = false;
            this.flowChart1.EndFC = null;
            this.flowChart1.ErrID = 0;
            this.flowChart1.InAlarm = false;
            this.flowChart1.IsFlowHead = false;
            this.flowChart1.Location = new System.Drawing.Point(42, 259);
            this.flowChart1.LockUI = false;
            this.flowChart1.Message = null;
            this.flowChart1.MsgID = 0;
            this.flowChart1.Name = "flowChart1";
            this.flowChart1.NEXT = this.FC_AUTO;
            this.flowChart1.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.flowChart1.OrgLocation = new System.Drawing.Point(0, 0);
            this.flowChart1.OrgSize = new System.Drawing.Size(0, 0);
            this.flowChart1.OverTimeSpec = 100;
            this.flowChart1.Running = false;
            this.flowChart1.Size = new System.Drawing.Size(150, 30);
            this.flowChart1.SlowRunCycle = -1;
            this.flowChart1.StartFC = null;
            this.flowChart1.Text = "MANUAL";
            this.flowChart1.Run += new ProVLib.FlowChart.RunEventHandler(this.flowChart1_Run);
            // 
            // act_ModuleInitial
            // 
            this.act_ModuleInitial.ActionMode = ProVLib.eActionMode.UIMode;
            this.act_ModuleInitial.BackColor = System.Drawing.Color.Blue;
            this.act_ModuleInitial.Block = true;
            this.act_ModuleInitial.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.act_ModuleInitial.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.act_ModuleInitial.Location = new System.Drawing.Point(42, 318);
            this.act_ModuleInitial.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.act_ModuleInitial.Name = "act_ModuleInitial";
            this.act_ModuleInitial.Next = null;
            this.act_ModuleInitial.Size = new System.Drawing.Size(150, 30);
            this.act_ModuleInitial.Subject = null;
            this.act_ModuleInitial.TabIndex = 26;
            this.act_ModuleInitial.Text = "Module Initial";
            this.act_ModuleInitial.OnActorRun += new ProVLib.ProVBaseActor.ActorTaskDelegate(this.act_ModuleInitial_OnActorRun);
            // 
            // flowChart5
            // 
            this.flowChart5.BackColor = System.Drawing.Color.RoyalBlue;
            this.flowChart5.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.flowChart5.CASE1 = null;
            this.flowChart5.CASE2 = null;
            this.flowChart5.CASE3 = null;
            this.flowChart5.CASE4 = null;
            this.flowChart5.ContinueRun = false;
            this.flowChart5.EndFC = null;
            this.flowChart5.ErrID = 0;
            this.flowChart5.InAlarm = false;
            this.flowChart5.IsFlowHead = false;
            this.flowChart5.Location = new System.Drawing.Point(495, 140);
            this.flowChart5.LockUI = false;
            this.flowChart5.Message = null;
            this.flowChart5.MsgID = 0;
            this.flowChart5.Name = "flowChart5";
            this.flowChart5.NEXT = this.FC_IDLE;
            this.flowChart5.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.flowChart5.OrgLocation = new System.Drawing.Point(0, 0);
            this.flowChart5.OrgSize = new System.Drawing.Size(0, 0);
            this.flowChart5.OverTimeSpec = 100;
            this.flowChart5.Running = false;
            this.flowChart5.Size = new System.Drawing.Size(51, 30);
            this.flowChart5.SlowRunCycle = -1;
            this.flowChart5.StartFC = null;
            this.flowChart5.Text = "INIT OK";
            this.flowChart5.Run += new ProVLib.FlowChart.RunEventHandler(this.flowChart5_Run);
            // 
            // FC_INIT
            // 
            this.FC_INIT.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_INIT.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_INIT.CASE1 = null;
            this.FC_INIT.CASE2 = null;
            this.FC_INIT.CASE3 = null;
            this.FC_INIT.CASE4 = null;
            this.FC_INIT.ContinueRun = false;
            this.FC_INIT.EndFC = null;
            this.FC_INIT.ErrID = 0;
            this.FC_INIT.InAlarm = false;
            this.FC_INIT.IsFlowHead = false;
            this.FC_INIT.Location = new System.Drawing.Point(572, 140);
            this.FC_INIT.LockUI = false;
            this.FC_INIT.Message = null;
            this.FC_INIT.MsgID = 0;
            this.FC_INIT.Name = "FC_INIT";
            this.FC_INIT.NEXT = this.flowChart5;
            this.FC_INIT.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_INIT.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_INIT.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_INIT.OverTimeSpec = 100;
            this.FC_INIT.Running = false;
            this.FC_INIT.Size = new System.Drawing.Size(51, 30);
            this.FC_INIT.SlowRunCycle = -1;
            this.FC_INIT.StartFC = null;
            this.FC_INIT.Text = "INIT";
            this.FC_INIT.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_INIT_Run);
            // 
            // tbState
            // 
            this.tbState.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tbState.Caption = "設備狀態";
            this.tbState.CaptionColor = System.Drawing.Color.Black;
            this.tbState.CaptionFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbState.CaptionWidth = 100;
            this.tbState.Location = new System.Drawing.Point(42, 33);
            this.tbState.Name = "tbState";
            this.tbState.Size = new System.Drawing.Size(300, 30);
            this.tbState.TabIndex = 14;
            this.tbState.Value = "";
            this.tbState.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.tbState.ValueFont = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // tpFunction
            // 
            this.tpFunction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpFunction.Controls.Add(this.btnPM);
            this.tpFunction.Controls.Add(this.btnScanData);
            this.tpFunction.Controls.Add(this.button1);
            this.tpFunction.Controls.Add(this.btnUserM);
            this.tpFunction.Controls.Add(this.btnScanLang);
            this.tpFunction.Controls.Add(this.btnScanIO);
            this.tpFunction.Location = new System.Drawing.Point(4, 44);
            this.tpFunction.Name = "tpFunction";
            this.tpFunction.Padding = new System.Windows.Forms.Padding(3);
            this.tpFunction.Size = new System.Drawing.Size(823, 478);
            this.tpFunction.TabIndex = 3;
            this.tpFunction.Text = "其他功能";
            // 
            // btnPM
            // 
            this.btnPM.Location = new System.Drawing.Point(38, 338);
            this.btnPM.Name = "btnPM";
            this.btnPM.Size = new System.Drawing.Size(159, 44);
            this.btnPM.TabIndex = 6;
            this.btnPM.Text = "預防保養項目";
            this.btnPM.UseVisualStyleBackColor = true;
            this.btnPM.Click += new System.EventHandler(this.btnPM_Click);
            // 
            // btnScanData
            // 
            this.btnScanData.Location = new System.Drawing.Point(38, 276);
            this.btnScanData.Name = "btnScanData";
            this.btnScanData.Size = new System.Drawing.Size(159, 44);
            this.btnScanData.TabIndex = 4;
            this.btnScanData.Text = "掃描機台參數";
            this.btnScanData.UseVisualStyleBackColor = true;
            this.btnScanData.Click += new System.EventHandler(this.btnScanData_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 213);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(159, 44);
            this.button1.TabIndex = 2;
            this.button1.Text = "內參設定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnUserM
            // 
            this.btnUserM.Location = new System.Drawing.Point(38, 149);
            this.btnUserM.Name = "btnUserM";
            this.btnUserM.Size = new System.Drawing.Size(159, 44);
            this.btnUserM.TabIndex = 2;
            this.btnUserM.Text = "使用者管理";
            this.btnUserM.UseVisualStyleBackColor = true;
            this.btnUserM.Click += new System.EventHandler(this.btnUserM_Click);
            // 
            // btnScanLang
            // 
            this.btnScanLang.Location = new System.Drawing.Point(38, 86);
            this.btnScanLang.Name = "btnScanLang";
            this.btnScanLang.Size = new System.Drawing.Size(159, 44);
            this.btnScanLang.TabIndex = 1;
            this.btnScanLang.Text = "語系掃描";
            this.btnScanLang.UseVisualStyleBackColor = true;
            this.btnScanLang.Click += new System.EventHandler(this.btnScanLang_Click);
            // 
            // btnScanIO
            // 
            this.btnScanIO.Location = new System.Drawing.Point(38, 26);
            this.btnScanIO.Name = "btnScanIO";
            this.btnScanIO.Size = new System.Drawing.Size(159, 44);
            this.btnScanIO.TabIndex = 0;
            this.btnScanIO.Text = "掃描模組IO";
            this.btnScanIO.UseVisualStyleBackColor = true;
            this.btnScanIO.Click += new System.EventHandler(this.btnScanIO_Click);
            // 
            // tmRefresh
            // 
            this.tmRefresh.Enabled = true;
            this.tmRefresh.Interval = 10;
            this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
            // 
            // MSS
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(831, 526);
            this.Controls.Add(this.tabMSS);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MSS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MSS";
            this.tabMSS.ResumeLayout(false);
            this.TP_StateFlow.ResumeLayout(false);
            this.tpFunction.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabMSS;
        private System.Windows.Forms.TabPage TP_StateFlow;
        private ProVLib.FlowChart FC_T2;
        private ProVLib.FlowChart FC_T1;
        private ProVLib.FlowChart FC_HOME;
        private ProVLib.FlowChart FC_LOAD_PACKAGE_RESET;
        private ProVLib.FlowChart FC_LOAD_PACKAGE;
        private ProVLib.FlowChart FC_MANUAL;
        private ProVLib.FlowChart FC_AUTO_RESET;
        private ProVLib.FlowChart FC_AUTO;
        private ProVLib.FlowChart flowChart1;
        private ProVLib.FlowChart FC_IDLE;
        private KCSDK.TextLabel tbState;
        private System.Windows.Forms.Timer tmRefresh;
        private System.Windows.Forms.TabPage tpFunction;
        private System.Windows.Forms.Button btnScanIO;
        private ProVLib.FlowChart flowChart3;
        private System.Windows.Forms.Button btnScanLang;
        private ProVLib.FlowChart FC_INIT;
        private System.Windows.Forms.Button btnUserM;
        private ProVLib.FlowChart flowChart2;
        private ProVLib.FlowChart flowChart4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnScanData;
        private ProVLib.FlowChart flowChart5;
        private ProVLib.ProVBaseActor act_ModuleInitial;
        private System.Windows.Forms.Button btnPM;
        private ProVLib.FlowChart FC_SLOW;
        private ProVLib.FlowChart FC_DELAYRUN;
    }
}