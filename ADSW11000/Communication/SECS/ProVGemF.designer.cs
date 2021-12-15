namespace ADSW11000
{
    partial class ProVGemF
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tcGEM = new System.Windows.Forms.TabControl();
            this.tpComm = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SECSEngine = new ProVTool.ProVSECSEngine();
            this.rdoEnable = new System.Windows.Forms.RadioButton();
            this.rdoDisable = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblWaitCR = new System.Windows.Forms.Label();
            this.lblWaitCRA = new System.Windows.Forms.Label();
            this.lblWaitDelay = new System.Windows.Forms.Label();
            this.lblCommunicating = new System.Windows.Forms.Label();
            this.lblDisabled = new System.Windows.Forms.Label();
            this.tpControl = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.btnRemote = new System.Windows.Forms.Button();
            this.btnLocal = new System.Windows.Forms.Button();
            this.btnOnLine = new System.Windows.Forms.Button();
            this.btnOffLine = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblOnLineRemote = new System.Windows.Forms.Label();
            this.lblOnLineLocal = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblEQPOffLine = new System.Windows.Forms.Label();
            this.lblAttemptOnLine = new System.Windows.Forms.Label();
            this.lblHostOffLine = new System.Windows.Forms.Label();
            this.tpProcess = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblIdle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblExecuting = new System.Windows.Forms.Label();
            this.lblSetup = new System.Windows.Forms.Label();
            this.lblInitial = new System.Windows.Forms.Label();
            this.lblPause = new System.Windows.Forms.Label();
            this.lblHome = new System.Windows.Forms.Label();
            this.lblAlarm = new System.Windows.Forms.Label();
            this.tpEQPData = new System.Windows.Forms.TabPage();
            this.dgvEVT = new System.Windows.Forms.DataGridView();
            this.dgvRPT = new System.Windows.Forms.DataGridView();
            this.dgvDV = new System.Windows.Forms.DataGridView();
            this.dgvSV = new System.Windows.Forms.DataGridView();
            this.dgvEC = new System.Windows.Forms.DataGridView();
            this.tpTerminal = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlRCV = new System.Windows.Forms.Panel();
            this.dgvRCV = new System.Windows.Forms.DataGridView();
            this.dcTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dcContent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFuncRCV = new System.Windows.Forms.Panel();
            this.btnRecognized = new System.Windows.Forms.Button();
            this.pnlSND = new System.Windows.Forms.Panel();
            this.dgvSND = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlFuncSND = new System.Windows.Forms.Panel();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.btnClearSND = new System.Windows.Forms.Button();
            this.tpLog = new System.Windows.Forms.TabPage();
            this.tcGEM.SuspendLayout();
            this.tpComm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tpControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tpProcess.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpEQPData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEVT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRPT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEC)).BeginInit();
            this.tpTerminal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlRCV.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRCV)).BeginInit();
            this.pnlFuncRCV.SuspendLayout();
            this.pnlSND.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSND)).BeginInit();
            this.pnlFuncSND.SuspendLayout();
            this.tpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(1228, 878);
            this.listBox1.TabIndex = 2;
            // 
            // tcGEM
            // 
            this.tcGEM.Controls.Add(this.tpComm);
            this.tcGEM.Controls.Add(this.tpControl);
            this.tcGEM.Controls.Add(this.tpProcess);
            this.tcGEM.Controls.Add(this.tpEQPData);
            this.tcGEM.Controls.Add(this.tpTerminal);
            this.tcGEM.Controls.Add(this.tpLog);
            this.tcGEM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGEM.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tcGEM.Location = new System.Drawing.Point(0, 0);
            this.tcGEM.Name = "tcGEM";
            this.tcGEM.SelectedIndex = 0;
            this.tcGEM.Size = new System.Drawing.Size(1242, 921);
            this.tcGEM.TabIndex = 8;
            // 
            // tpComm
            // 
            this.tpComm.Controls.Add(this.splitContainer2);
            this.tpComm.Location = new System.Drawing.Point(4, 33);
            this.tpComm.Name = "tpComm";
            this.tpComm.Padding = new System.Windows.Forms.Padding(3);
            this.tpComm.Size = new System.Drawing.Size(1234, 884);
            this.tpComm.TabIndex = 0;
            this.tpComm.Text = "Communication";
            this.tpComm.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.splitContainer2.Panel1.Controls.Add(this.SECSEngine);
            this.splitContainer2.Panel1.Controls.Add(this.rdoEnable);
            this.splitContainer2.Panel1.Controls.Add(this.rdoDisable);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel2.Controls.Add(this.lblDisabled);
            this.splitContainer2.Size = new System.Drawing.Size(1228, 878);
            this.splitContainer2.SplitterDistance = 262;
            this.splitContainer2.TabIndex = 0;
            // 
            // SECSEngine
            // 
            this.SECSEngine.ConnectMode = ProVTool.CONNECTMODE.PASSIVE;
            this.SECSEngine.DeviceID = ((ushort)(0));
            this.SECSEngine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SECSEngine.Interval = ((uint)(10u));
            this.SECSEngine.IP = "127.0.0.1";
            this.SECSEngine.LinkTestPeroid = ((long)(60));
            this.SECSEngine.Location = new System.Drawing.Point(0, 848);
            this.SECSEngine.Name = "SECSEngine";
            this.SECSEngine.Port = ((ushort)(5001));
            this.SECSEngine.Size = new System.Drawing.Size(262, 30);
            //this.SECSEngine.SpoolActive = false;
            this.SECSEngine.T3 = ((long)(45));
            this.SECSEngine.T5 = ((long)(5));
            this.SECSEngine.T6 = ((long)(5));
            this.SECSEngine.T7 = ((long)(5));
            this.SECSEngine.T8 = ((long)(3));
            this.SECSEngine.Text = "SECS";
            this.SECSEngine.SECSEvent += new ProVTool.SECSEventHandler(this.proVSECSEngine1_SECSEvent);
            // 
            // rdoEnable
            // 
            this.rdoEnable.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoEnable.Dock = System.Windows.Forms.DockStyle.Top;
            this.rdoEnable.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoEnable.Location = new System.Drawing.Point(0, 66);
            this.rdoEnable.Name = "rdoEnable";
            this.rdoEnable.Size = new System.Drawing.Size(262, 66);
            this.rdoEnable.TabIndex = 1;
            this.rdoEnable.Tag = "1";
            this.rdoEnable.Text = "Enable";
            this.rdoEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoEnable.UseVisualStyleBackColor = true;
            this.rdoEnable.CheckedChanged += new System.EventHandler(this.rdoDisable_CheckedChanged);
            // 
            // rdoDisable
            // 
            this.rdoDisable.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoDisable.Checked = true;
            this.rdoDisable.Dock = System.Windows.Forms.DockStyle.Top;
            this.rdoDisable.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rdoDisable.Location = new System.Drawing.Point(0, 0);
            this.rdoDisable.Name = "rdoDisable";
            this.rdoDisable.Size = new System.Drawing.Size(262, 66);
            this.rdoDisable.TabIndex = 0;
            this.rdoDisable.TabStop = true;
            this.rdoDisable.Tag = "0";
            this.rdoDisable.Text = "Disable";
            this.rdoDisable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoDisable.UseVisualStyleBackColor = true;
            this.rdoDisable.CheckedChanged += new System.EventHandler(this.rdoDisable_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.lblCommunicating);
            this.groupBox2.Location = new System.Drawing.Point(22, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(576, 299);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Enabled";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblWaitCR);
            this.groupBox3.Controls.Add(this.lblWaitCRA);
            this.groupBox3.Controls.Add(this.lblWaitDelay);
            this.groupBox3.Location = new System.Drawing.Point(32, 43);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(522, 163);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Not Communicating";
            // 
            // lblWaitCR
            // 
            this.lblWaitCR.BackColor = System.Drawing.Color.White;
            this.lblWaitCR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWaitCR.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblWaitCR.Location = new System.Drawing.Point(41, 57);
            this.lblWaitCR.Name = "lblWaitCR";
            this.lblWaitCR.Size = new System.Drawing.Size(194, 72);
            this.lblWaitCR.TabIndex = 2;
            this.lblWaitCR.Text = "Wait CR From Host";
            this.lblWaitCR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWaitCRA
            // 
            this.lblWaitCRA.BackColor = System.Drawing.Color.White;
            this.lblWaitCRA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWaitCRA.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblWaitCRA.Location = new System.Drawing.Point(281, 95);
            this.lblWaitCRA.Name = "lblWaitCRA";
            this.lblWaitCRA.Size = new System.Drawing.Size(194, 38);
            this.lblWaitCRA.TabIndex = 5;
            this.lblWaitCRA.Text = "Wait CRA";
            this.lblWaitCRA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblWaitDelay
            // 
            this.lblWaitDelay.BackColor = System.Drawing.Color.White;
            this.lblWaitDelay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblWaitDelay.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblWaitDelay.Location = new System.Drawing.Point(281, 57);
            this.lblWaitDelay.Name = "lblWaitDelay";
            this.lblWaitDelay.Size = new System.Drawing.Size(194, 38);
            this.lblWaitDelay.TabIndex = 4;
            this.lblWaitDelay.Text = "Wait Delay";
            this.lblWaitDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCommunicating
            // 
            this.lblCommunicating.BackColor = System.Drawing.Color.White;
            this.lblCommunicating.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCommunicating.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblCommunicating.Location = new System.Drawing.Point(180, 238);
            this.lblCommunicating.Name = "lblCommunicating";
            this.lblCommunicating.Size = new System.Drawing.Size(194, 38);
            this.lblCommunicating.TabIndex = 3;
            this.lblCommunicating.Text = "Communicating";
            this.lblCommunicating.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDisabled
            // 
            this.lblDisabled.BackColor = System.Drawing.Color.GreenYellow;
            this.lblDisabled.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDisabled.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblDisabled.Location = new System.Drawing.Point(22, 28);
            this.lblDisabled.Name = "lblDisabled";
            this.lblDisabled.Size = new System.Drawing.Size(194, 38);
            this.lblDisabled.TabIndex = 1;
            this.lblDisabled.Text = "Disabled";
            this.lblDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpControl
            // 
            this.tpControl.Controls.Add(this.splitContainer3);
            this.tpControl.Location = new System.Drawing.Point(4, 33);
            this.tpControl.Name = "tpControl";
            this.tpControl.Size = new System.Drawing.Size(1234, 884);
            this.tpControl.TabIndex = 2;
            this.tpControl.Text = "Control";
            this.tpControl.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.splitContainer3.Panel1.Controls.Add(this.btnRemote);
            this.splitContainer3.Panel1.Controls.Add(this.btnLocal);
            this.splitContainer3.Panel1.Controls.Add(this.btnOnLine);
            this.splitContainer3.Panel1.Controls.Add(this.btnOffLine);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox5);
            this.splitContainer3.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer3.Size = new System.Drawing.Size(1234, 884);
            this.splitContainer3.SplitterDistance = 261;
            this.splitContainer3.TabIndex = 0;
            // 
            // btnRemote
            // 
            this.btnRemote.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRemote.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnRemote.Location = new System.Drawing.Point(0, 144);
            this.btnRemote.Name = "btnRemote";
            this.btnRemote.Size = new System.Drawing.Size(261, 48);
            this.btnRemote.TabIndex = 7;
            this.btnRemote.Tag = "3";
            this.btnRemote.Text = "On-Line Remote";
            this.btnRemote.UseVisualStyleBackColor = true;
            this.btnRemote.Click += new System.EventHandler(this.ControlMode_OnClick);
            // 
            // btnLocal
            // 
            this.btnLocal.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLocal.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnLocal.Location = new System.Drawing.Point(0, 96);
            this.btnLocal.Name = "btnLocal";
            this.btnLocal.Size = new System.Drawing.Size(261, 48);
            this.btnLocal.TabIndex = 6;
            this.btnLocal.Tag = "2";
            this.btnLocal.Text = "On-Line Local";
            this.btnLocal.UseVisualStyleBackColor = true;
            this.btnLocal.Click += new System.EventHandler(this.ControlMode_OnClick);
            // 
            // btnOnLine
            // 
            this.btnOnLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOnLine.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOnLine.Location = new System.Drawing.Point(0, 48);
            this.btnOnLine.Name = "btnOnLine";
            this.btnOnLine.Size = new System.Drawing.Size(261, 48);
            this.btnOnLine.TabIndex = 5;
            this.btnOnLine.Tag = "1";
            this.btnOnLine.Text = "On-Line";
            this.btnOnLine.UseVisualStyleBackColor = true;
            this.btnOnLine.Click += new System.EventHandler(this.ControlMode_OnClick);
            // 
            // btnOffLine
            // 
            this.btnOffLine.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOffLine.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOffLine.Location = new System.Drawing.Point(0, 0);
            this.btnOffLine.Name = "btnOffLine";
            this.btnOffLine.Size = new System.Drawing.Size(261, 48);
            this.btnOffLine.TabIndex = 4;
            this.btnOffLine.Tag = "0";
            this.btnOffLine.Text = "Off-Line";
            this.btnOffLine.UseVisualStyleBackColor = true;
            this.btnOffLine.Click += new System.EventHandler(this.ControlMode_OnClick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblOnLineRemote);
            this.groupBox5.Controls.Add(this.lblOnLineLocal);
            this.groupBox5.Location = new System.Drawing.Point(59, 333);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(495, 152);
            this.groupBox5.TabIndex = 7;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ON-LINE";
            // 
            // lblOnLineRemote
            // 
            this.lblOnLineRemote.BackColor = System.Drawing.Color.White;
            this.lblOnLineRemote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOnLineRemote.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOnLineRemote.Location = new System.Drawing.Point(264, 60);
            this.lblOnLineRemote.Name = "lblOnLineRemote";
            this.lblOnLineRemote.Size = new System.Drawing.Size(193, 58);
            this.lblOnLineRemote.TabIndex = 5;
            this.lblOnLineRemote.Text = "REMOTE";
            this.lblOnLineRemote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOnLineLocal
            // 
            this.lblOnLineLocal.BackColor = System.Drawing.Color.White;
            this.lblOnLineLocal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOnLineLocal.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblOnLineLocal.Location = new System.Drawing.Point(28, 60);
            this.lblOnLineLocal.Name = "lblOnLineLocal";
            this.lblOnLineLocal.Size = new System.Drawing.Size(193, 58);
            this.lblOnLineLocal.TabIndex = 4;
            this.lblOnLineLocal.Text = "LOCAL";
            this.lblOnLineLocal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblEQPOffLine);
            this.groupBox4.Controls.Add(this.lblAttemptOnLine);
            this.groupBox4.Controls.Add(this.lblHostOffLine);
            this.groupBox4.Location = new System.Drawing.Point(59, 37);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(495, 231);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "OFF-LINE";
            // 
            // lblEQPOffLine
            // 
            this.lblEQPOffLine.BackColor = System.Drawing.Color.GreenYellow;
            this.lblEQPOffLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEQPOffLine.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblEQPOffLine.Location = new System.Drawing.Point(144, 49);
            this.lblEQPOffLine.Name = "lblEQPOffLine";
            this.lblEQPOffLine.Size = new System.Drawing.Size(193, 58);
            this.lblEQPOffLine.TabIndex = 1;
            this.lblEQPOffLine.Text = "EQP Off-Line";
            this.lblEQPOffLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAttemptOnLine
            // 
            this.lblAttemptOnLine.BackColor = System.Drawing.Color.White;
            this.lblAttemptOnLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAttemptOnLine.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAttemptOnLine.Location = new System.Drawing.Point(264, 130);
            this.lblAttemptOnLine.Name = "lblAttemptOnLine";
            this.lblAttemptOnLine.Size = new System.Drawing.Size(193, 58);
            this.lblAttemptOnLine.TabIndex = 2;
            this.lblAttemptOnLine.Text = "Attempt On-Line";
            this.lblAttemptOnLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHostOffLine
            // 
            this.lblHostOffLine.BackColor = System.Drawing.Color.White;
            this.lblHostOffLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHostOffLine.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHostOffLine.Location = new System.Drawing.Point(28, 130);
            this.lblHostOffLine.Name = "lblHostOffLine";
            this.lblHostOffLine.Size = new System.Drawing.Size(193, 58);
            this.lblHostOffLine.TabIndex = 3;
            this.lblHostOffLine.Text = "HOST Off-Line";
            this.lblHostOffLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpProcess
            // 
            this.tpProcess.Controls.Add(this.groupBox6);
            this.tpProcess.Controls.Add(this.lblAlarm);
            this.tpProcess.Location = new System.Drawing.Point(4, 33);
            this.tpProcess.Name = "tpProcess";
            this.tpProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tpProcess.Size = new System.Drawing.Size(1234, 884);
            this.tpProcess.TabIndex = 3;
            this.tpProcess.Text = "Process";
            this.tpProcess.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblIdle);
            this.groupBox6.Controls.Add(this.groupBox1);
            this.groupBox6.Controls.Add(this.lblPause);
            this.groupBox6.Controls.Add(this.lblHome);
            this.groupBox6.Location = new System.Drawing.Point(25, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(415, 324);
            this.groupBox6.TabIndex = 8;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "NORMAL";
            // 
            // lblIdle
            // 
            this.lblIdle.BackColor = System.Drawing.Color.GreenYellow;
            this.lblIdle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblIdle.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblIdle.Location = new System.Drawing.Point(215, 53);
            this.lblIdle.Name = "lblIdle";
            this.lblIdle.Size = new System.Drawing.Size(152, 33);
            this.lblIdle.TabIndex = 0;
            this.lblIdle.Text = "1. IDLE";
            this.lblIdle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblExecuting);
            this.groupBox1.Controls.Add(this.lblSetup);
            this.groupBox1.Controls.Add(this.lblInitial);
            this.groupBox1.Location = new System.Drawing.Point(189, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(201, 198);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Process";
            // 
            // lblExecuting
            // 
            this.lblExecuting.BackColor = System.Drawing.Color.White;
            this.lblExecuting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblExecuting.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblExecuting.Location = new System.Drawing.Point(26, 149);
            this.lblExecuting.Name = "lblExecuting";
            this.lblExecuting.Size = new System.Drawing.Size(152, 33);
            this.lblExecuting.TabIndex = 3;
            this.lblExecuting.Text = "5. AUTO";
            this.lblExecuting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSetup
            // 
            this.lblSetup.BackColor = System.Drawing.Color.White;
            this.lblSetup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSetup.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblSetup.Location = new System.Drawing.Point(26, 40);
            this.lblSetup.Name = "lblSetup";
            this.lblSetup.Size = new System.Drawing.Size(152, 33);
            this.lblSetup.TabIndex = 1;
            this.lblSetup.Text = "4. LOADPKG";
            this.lblSetup.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInitial
            // 
            this.lblInitial.BackColor = System.Drawing.Color.White;
            this.lblInitial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblInitial.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblInitial.Location = new System.Drawing.Point(26, 94);
            this.lblInitial.Name = "lblInitial";
            this.lblInitial.Size = new System.Drawing.Size(152, 33);
            this.lblInitial.TabIndex = 2;
            this.lblInitial.Text = "3. INITIAL";
            this.lblInitial.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPause
            // 
            this.lblPause.BackColor = System.Drawing.Color.White;
            this.lblPause.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPause.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblPause.Location = new System.Drawing.Point(22, 197);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(140, 33);
            this.lblPause.TabIndex = 4;
            this.lblPause.Text = "6. PAUSE";
            this.lblPause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHome
            // 
            this.lblHome.BackColor = System.Drawing.Color.White;
            this.lblHome.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblHome.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHome.Location = new System.Drawing.Point(22, 53);
            this.lblHome.Name = "lblHome";
            this.lblHome.Size = new System.Drawing.Size(140, 33);
            this.lblHome.TabIndex = 6;
            this.lblHome.Text = "2. HOME";
            this.lblHome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAlarm
            // 
            this.lblAlarm.BackColor = System.Drawing.Color.White;
            this.lblAlarm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAlarm.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAlarm.Location = new System.Drawing.Point(471, 72);
            this.lblAlarm.Name = "lblAlarm";
            this.lblAlarm.Size = new System.Drawing.Size(140, 33);
            this.lblAlarm.TabIndex = 5;
            this.lblAlarm.Text = "7. ALARM";
            this.lblAlarm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpEQPData
            // 
            this.tpEQPData.Controls.Add(this.dgvEVT);
            this.tpEQPData.Controls.Add(this.dgvRPT);
            this.tpEQPData.Controls.Add(this.dgvDV);
            this.tpEQPData.Controls.Add(this.dgvSV);
            this.tpEQPData.Controls.Add(this.dgvEC);
            this.tpEQPData.Location = new System.Drawing.Point(4, 33);
            this.tpEQPData.Name = "tpEQPData";
            this.tpEQPData.Size = new System.Drawing.Size(1234, 884);
            this.tpEQPData.TabIndex = 4;
            this.tpEQPData.Text = "EQPData";
            this.tpEQPData.UseVisualStyleBackColor = true;
            // 
            // dgvEVT
            // 
            this.dgvEVT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEVT.Location = new System.Drawing.Point(425, 380);
            this.dgvEVT.Name = "dgvEVT";
            this.dgvEVT.RowTemplate.Height = 24;
            this.dgvEVT.Size = new System.Drawing.Size(393, 345);
            this.dgvEVT.TabIndex = 4;
            // 
            // dgvRPT
            // 
            this.dgvRPT.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRPT.Location = new System.Drawing.Point(8, 380);
            this.dgvRPT.Name = "dgvRPT";
            this.dgvRPT.RowTemplate.Height = 24;
            this.dgvRPT.Size = new System.Drawing.Size(393, 345);
            this.dgvRPT.TabIndex = 3;
            // 
            // dgvDV
            // 
            this.dgvDV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDV.Location = new System.Drawing.Point(833, 16);
            this.dgvDV.Name = "dgvDV";
            this.dgvDV.RowTemplate.Height = 24;
            this.dgvDV.Size = new System.Drawing.Size(393, 345);
            this.dgvDV.TabIndex = 2;
            // 
            // dgvSV
            // 
            this.dgvSV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSV.Location = new System.Drawing.Point(425, 16);
            this.dgvSV.Name = "dgvSV";
            this.dgvSV.RowTemplate.Height = 24;
            this.dgvSV.Size = new System.Drawing.Size(393, 345);
            this.dgvSV.TabIndex = 1;
            // 
            // dgvEC
            // 
            this.dgvEC.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEC.Location = new System.Drawing.Point(8, 16);
            this.dgvEC.Name = "dgvEC";
            this.dgvEC.RowTemplate.Height = 24;
            this.dgvEC.Size = new System.Drawing.Size(393, 345);
            this.dgvEC.TabIndex = 0;
            // 
            // tpTerminal
            // 
            this.tpTerminal.Controls.Add(this.splitContainer1);
            this.tpTerminal.Location = new System.Drawing.Point(4, 33);
            this.tpTerminal.Name = "tpTerminal";
            this.tpTerminal.Size = new System.Drawing.Size(1234, 884);
            this.tpTerminal.TabIndex = 5;
            this.tpTerminal.Text = "Terminal Service";
            this.tpTerminal.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlRCV);
            this.splitContainer1.Panel1.Controls.Add(this.pnlFuncRCV);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlSND);
            this.splitContainer1.Panel2.Controls.Add(this.pnlFuncSND);
            this.splitContainer1.Size = new System.Drawing.Size(1234, 884);
            this.splitContainer1.SplitterDistance = 442;
            this.splitContainer1.TabIndex = 2;
            // 
            // pnlRCV
            // 
            this.pnlRCV.Controls.Add(this.dgvRCV);
            this.pnlRCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRCV.Location = new System.Drawing.Point(0, 0);
            this.pnlRCV.Name = "pnlRCV";
            this.pnlRCV.Size = new System.Drawing.Size(1038, 440);
            this.pnlRCV.TabIndex = 3;
            // 
            // dgvRCV
            // 
            this.dgvRCV.AllowUserToAddRows = false;
            this.dgvRCV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRCV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dcTime,
            this.dcContent});
            this.dgvRCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRCV.Location = new System.Drawing.Point(0, 0);
            this.dgvRCV.Name = "dgvRCV";
            this.dgvRCV.RowTemplate.Height = 24;
            this.dgvRCV.Size = new System.Drawing.Size(1038, 440);
            this.dgvRCV.TabIndex = 0;
            // 
            // dcTime
            // 
            this.dcTime.HeaderText = "Time";
            this.dcTime.Name = "dcTime";
            this.dcTime.ReadOnly = true;
            this.dcTime.Width = 200;
            // 
            // dcContent
            // 
            this.dcContent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dcContent.HeaderText = "Content";
            this.dcContent.Name = "dcContent";
            // 
            // pnlFuncRCV
            // 
            this.pnlFuncRCV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pnlFuncRCV.Controls.Add(this.btnRecognized);
            this.pnlFuncRCV.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlFuncRCV.Location = new System.Drawing.Point(1038, 0);
            this.pnlFuncRCV.Name = "pnlFuncRCV";
            this.pnlFuncRCV.Size = new System.Drawing.Size(194, 440);
            this.pnlFuncRCV.TabIndex = 4;
            // 
            // btnRecognized
            // 
            this.btnRecognized.Location = new System.Drawing.Point(3, 13);
            this.btnRecognized.Name = "btnRecognized";
            this.btnRecognized.Size = new System.Drawing.Size(182, 47);
            this.btnRecognized.TabIndex = 1;
            this.btnRecognized.Text = "Recognized";
            this.btnRecognized.UseVisualStyleBackColor = true;
            this.btnRecognized.Click += new System.EventHandler(this.btnRecognized_Click);
            // 
            // pnlSND
            // 
            this.pnlSND.Controls.Add(this.dgvSND);
            this.pnlSND.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSND.Location = new System.Drawing.Point(0, 0);
            this.pnlSND.Name = "pnlSND";
            this.pnlSND.Size = new System.Drawing.Size(1038, 436);
            this.pnlSND.TabIndex = 5;
            // 
            // dgvSND
            // 
            this.dgvSND.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSND.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn2});
            this.dgvSND.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSND.Location = new System.Drawing.Point(0, 0);
            this.dgvSND.Name = "dgvSND";
            this.dgvSND.RowTemplate.Height = 24;
            this.dgvSND.Size = new System.Drawing.Size(1038, 436);
            this.dgvSND.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "Content";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // pnlFuncSND
            // 
            this.pnlFuncSND.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pnlFuncSND.Controls.Add(this.btnSendMsg);
            this.pnlFuncSND.Controls.Add(this.btnClearSND);
            this.pnlFuncSND.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlFuncSND.Location = new System.Drawing.Point(1038, 0);
            this.pnlFuncSND.Name = "pnlFuncSND";
            this.pnlFuncSND.Size = new System.Drawing.Size(194, 436);
            this.pnlFuncSND.TabIndex = 6;
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Location = new System.Drawing.Point(3, 12);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(182, 47);
            this.btnSendMsg.TabIndex = 3;
            this.btnSendMsg.Text = "Send";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // btnClearSND
            // 
            this.btnClearSND.Location = new System.Drawing.Point(3, 77);
            this.btnClearSND.Name = "btnClearSND";
            this.btnClearSND.Size = new System.Drawing.Size(182, 47);
            this.btnClearSND.TabIndex = 4;
            this.btnClearSND.Text = "Clear";
            this.btnClearSND.UseVisualStyleBackColor = true;
            this.btnClearSND.Click += new System.EventHandler(this.btnClearSND_Click);
            // 
            // tpLog
            // 
            this.tpLog.Controls.Add(this.listBox1);
            this.tpLog.Location = new System.Drawing.Point(4, 33);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(3);
            this.tpLog.Size = new System.Drawing.Size(1234, 884);
            this.tpLog.TabIndex = 1;
            this.tpLog.Text = "Log";
            this.tpLog.UseVisualStyleBackColor = true;
            // 
            // ProVGemF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 921);
            this.Controls.Add(this.tcGEM);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ProVGemF";
            this.Text = "ProVGemF";
            this.Load += new System.EventHandler(this.ProVGemF_Load);
            this.tcGEM.ResumeLayout(false);
            this.tpComm.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tpControl.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tpProcess.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tpEQPData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEVT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRPT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEC)).EndInit();
            this.tpTerminal.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.pnlRCV.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRCV)).EndInit();
            this.pnlFuncRCV.ResumeLayout(false);
            this.pnlSND.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSND)).EndInit();
            this.pnlFuncSND.ResumeLayout(false);
            this.tpLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ProVTool.ProVSECSEngine SECSEngine;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TabControl tcGEM;
        private System.Windows.Forms.TabPage tpComm;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.RadioButton rdoEnable;
        private System.Windows.Forms.RadioButton rdoDisable;
        private System.Windows.Forms.TabPage tpControl;
        private System.Windows.Forms.TabPage tpLog;
        private System.Windows.Forms.Label lblCommunicating;
        private System.Windows.Forms.Label lblWaitCR;
        private System.Windows.Forms.Label lblDisabled;
        private System.Windows.Forms.Label lblWaitCRA;
        private System.Windows.Forms.Label lblWaitDelay;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label lblOnLineRemote;
        private System.Windows.Forms.Label lblOnLineLocal;
        private System.Windows.Forms.Label lblHostOffLine;
        private System.Windows.Forms.Label lblAttemptOnLine;
        private System.Windows.Forms.Label lblEQPOffLine;
        private System.Windows.Forms.Button btnRemote;
        private System.Windows.Forms.Button btnLocal;
        private System.Windows.Forms.Button btnOnLine;
        private System.Windows.Forms.Button btnOffLine;
        private System.Windows.Forms.TabPage tpProcess;
        private System.Windows.Forms.Label lblAlarm;
        private System.Windows.Forms.Label lblPause;
        private System.Windows.Forms.Label lblExecuting;
        private System.Windows.Forms.Label lblInitial;
        private System.Windows.Forms.Label lblSetup;
        private System.Windows.Forms.Label lblIdle;
        private System.Windows.Forms.TabPage tpEQPData;
        private System.Windows.Forms.DataGridView dgvDV;
        private System.Windows.Forms.DataGridView dgvSV;
        private System.Windows.Forms.DataGridView dgvEC;
        private System.Windows.Forms.TabPage tpTerminal;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnRecognized;
        private System.Windows.Forms.DataGridView dgvRCV;
        private System.Windows.Forms.Button btnClearSND;
        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.DataGridView dgvSND;
        private System.Windows.Forms.Panel pnlRCV;
        private System.Windows.Forms.Panel pnlFuncRCV;
        private System.Windows.Forms.Panel pnlSND;
        private System.Windows.Forms.Panel pnlFuncSND;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dcContent;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridView dgvEVT;
        private System.Windows.Forms.DataGridView dgvRPT;
        private System.Windows.Forms.Label lblHome;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}

