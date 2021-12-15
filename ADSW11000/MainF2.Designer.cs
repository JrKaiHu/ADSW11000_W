namespace ADSW11000
{
    partial class MainF2
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainF2));
            this.pnlControl = new System.Windows.Forms.Panel();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnOption = new System.Windows.Forms.Button();
            this.ledGreen = new KCSDK.ThreeColorLight();
            this.ledYellow = new KCSDK.ThreeColorLight();
            this.ledRed = new KCSDK.ThreeColorLight();
            this.btnAuto = new System.Windows.Forms.Button();
            this.lbNowTime = new System.Windows.Forms.Label();
            this.lbVersion = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnPackage = new System.Windows.Forms.Button();
            this.btnManual = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.btnLang = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lbRunState = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlPackage = new System.Windows.Forms.Panel();
            this.lbPackage = new System.Windows.Forms.Label();
            this.lbProjectName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbPackageName = new System.Windows.Forms.Label();
            this.pnlUser = new System.Windows.Forms.Panel();
            this.btnChangeUser = new System.Windows.Forms.Button();
            this.lbUserType = new System.Windows.Forms.Label();
            this.pnlAlarmGrid = new System.Windows.Forms.Panel();
            this.lvArmGrid = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.tabMachineState = new System.Windows.Forms.TabControl();
            this.tpProductionState = new System.Windows.Forms.TabPage();
            this.tpKernalData = new System.Windows.Forms.TabPage();
            this.TB_授權有效性 = new KCSDK.TextLabel();
            this.TB_授權模式 = new KCSDK.TextLabel();
            this.btnResetManual = new System.Windows.Forms.Button();
            this.btnResetHome = new System.Windows.Forms.Button();
            this.btnResetIDLE = new System.Windows.Forms.Button();
            this.lbManualTM = new KCSDK.TextLabel();
            this.lbHomeTM = new KCSDK.TextLabel();
            this.lbIdleTM = new KCSDK.TextLabel();
            this.lbPauseTM = new KCSDK.TextLabel();
            this.lbRunTM = new KCSDK.TextLabel();
            this.lbOperationEndTM = new KCSDK.TextLabel();
            this.lbOperationStartTM = new KCSDK.TextLabel();
            this.lbScanCNT = new KCSDK.NumLabel();
            this.lbScanTM = new KCSDK.NumLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.tpMSS = new System.Windows.Forms.TabPage();
            this.tpMainFlow = new System.Windows.Forms.TabPage();
            this.tmUIRefresh = new System.Windows.Forms.Timer(this.components);
            this.pnl異常通知 = new System.Windows.Forms.Panel();
            this.lb異常訊息 = new System.Windows.Forms.Label();
            this.tpCommunication = new System.Windows.Forms.TabPage();
            this.pnlControl.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlPackage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlUser.SuspendLayout();
            this.pnlAlarmGrid.SuspendLayout();
            this.pnlContainer.SuspendLayout();
            this.tabMachineState.SuspendLayout();
            this.tpProductionState.SuspendLayout();
            this.tpKernalData.SuspendLayout();
            this.pnl異常通知.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControl
            // 
            this.pnlControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(42)))), ((int)(((byte)(50)))));
            this.pnlControl.Controls.Add(this.btnHome);
            this.pnlControl.Controls.Add(this.btnOption);
            this.pnlControl.Controls.Add(this.ledGreen);
            this.pnlControl.Controls.Add(this.ledYellow);
            this.pnlControl.Controls.Add(this.ledRed);
            this.pnlControl.Controls.Add(this.btnAuto);
            this.pnlControl.Controls.Add(this.lbNowTime);
            this.pnlControl.Controls.Add(this.lbVersion);
            this.pnlControl.Controls.Add(this.btnExit);
            this.pnlControl.Controls.Add(this.btnPackage);
            this.pnlControl.Controls.Add(this.btnManual);
            this.pnlControl.Controls.Add(this.btnHistory);
            this.pnlControl.Controls.Add(this.btnLang);
            this.pnlControl.Controls.Add(this.btnReset);
            this.pnlControl.Controls.Add(this.btnPause);
            this.pnlControl.Controls.Add(this.btnStart);
            this.pnlControl.Controls.Add(this.lbRunState);
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlControl.Location = new System.Drawing.Point(1164, 2);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(200, 764);
            this.pnlControl.TabIndex = 0;
            // 
            // btnHome
            // 
            this.btnHome.AutoSize = true;
            this.btnHome.BackgroundImage = global::ADSW11000.Properties.Resources.btnHome_tw;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnHome.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHome.Location = new System.Drawing.Point(2, 328);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(197, 45);
            this.btnHome.TabIndex = 36;
            this.btnHome.Tag = "1";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnOption
            // 
            this.btnOption.AutoSize = true;
            this.btnOption.BackgroundImage = global::ADSW11000.Properties.Resources.btnOption_tw;
            this.btnOption.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnOption.FlatAppearance.BorderSize = 0;
            this.btnOption.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOption.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnOption.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOption.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOption.Location = new System.Drawing.Point(2, 528);
            this.btnOption.Name = "btnOption";
            this.btnOption.Size = new System.Drawing.Size(197, 45);
            this.btnOption.TabIndex = 35;
            this.btnOption.Tag = "4";
            this.btnOption.UseVisualStyleBackColor = true;
            this.btnOption.Click += new System.EventHandler(this.PageChange);
            // 
            // ledGreen
            // 
            this.ledGreen.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ledGreen.BackgroundImage")));
            this.ledGreen.Location = new System.Drawing.Point(136, 73);
            this.ledGreen.Name = "ledGreen";
            this.ledGreen.Size = new System.Drawing.Size(40, 40);
            this.ledGreen.TabIndex = 34;
            this.ledGreen.Value = KCSDK.ThreeColorLight.ColorLightType.GreenLight;
            // 
            // ledYellow
            // 
            this.ledYellow.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ledYellow.BackgroundImage")));
            this.ledYellow.Location = new System.Drawing.Point(80, 73);
            this.ledYellow.Name = "ledYellow";
            this.ledYellow.Size = new System.Drawing.Size(40, 40);
            this.ledYellow.TabIndex = 33;
            this.ledYellow.Value = KCSDK.ThreeColorLight.ColorLightType.YellowLight;
            // 
            // ledRed
            // 
            this.ledRed.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ledRed.BackgroundImage")));
            this.ledRed.Location = new System.Drawing.Point(24, 73);
            this.ledRed.Name = "ledRed";
            this.ledRed.Size = new System.Drawing.Size(40, 40);
            this.ledRed.TabIndex = 32;
            this.ledRed.Value = KCSDK.ThreeColorLight.ColorLightType.RedLight;
            // 
            // btnAuto
            // 
            this.btnAuto.AutoSize = true;
            this.btnAuto.BackgroundImage = global::ADSW11000.Properties.Resources.btnAuto_tw;
            this.btnAuto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAuto.FlatAppearance.BorderSize = 0;
            this.btnAuto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnAuto.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnAuto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAuto.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAuto.Location = new System.Drawing.Point(2, 378);
            this.btnAuto.Name = "btnAuto";
            this.btnAuto.Size = new System.Drawing.Size(197, 45);
            this.btnAuto.TabIndex = 31;
            this.btnAuto.Tag = "1";
            this.btnAuto.UseVisualStyleBackColor = true;
            this.btnAuto.Click += new System.EventHandler(this.PageChange);
            // 
            // lbNowTime
            // 
            this.lbNowTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(42)))), ((int)(((byte)(50)))));
            this.lbNowTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbNowTime.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbNowTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbNowTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbNowTime.Location = new System.Drawing.Point(0, 712);
            this.lbNowTime.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lbNowTime.Name = "lbNowTime";
            this.lbNowTime.Size = new System.Drawing.Size(200, 29);
            this.lbNowTime.TabIndex = 29;
            this.lbNowTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbVersion
            // 
            this.lbVersion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(42)))), ((int)(((byte)(50)))));
            this.lbVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lbVersion.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbVersion.ForeColor = System.Drawing.Color.Silver;
            this.lbVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbVersion.Location = new System.Drawing.Point(0, 741);
            this.lbVersion.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lbVersion.Name = "lbVersion";
            this.lbVersion.Size = new System.Drawing.Size(200, 23);
            this.lbVersion.TabIndex = 30;
            this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnExit
            // 
            this.btnExit.AutoSize = true;
            this.btnExit.BackgroundImage = global::ADSW11000.Properties.Resources.btnExit_tw;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnExit.Location = new System.Drawing.Point(2, 674);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(197, 45);
            this.btnExit.TabIndex = 25;
            this.btnExit.Tag = "5";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPackage
            // 
            this.btnPackage.AutoSize = true;
            this.btnPackage.BackgroundImage = global::ADSW11000.Properties.Resources.btnPackage_tw;
            this.btnPackage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPackage.FlatAppearance.BorderSize = 0;
            this.btnPackage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPackage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnPackage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPackage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPackage.Location = new System.Drawing.Point(2, 478);
            this.btnPackage.Name = "btnPackage";
            this.btnPackage.Size = new System.Drawing.Size(197, 45);
            this.btnPackage.TabIndex = 24;
            this.btnPackage.Tag = "3";
            this.btnPackage.UseVisualStyleBackColor = true;
            this.btnPackage.Click += new System.EventHandler(this.PageChange);
            // 
            // btnManual
            // 
            this.btnManual.AutoSize = true;
            this.btnManual.BackgroundImage = global::ADSW11000.Properties.Resources.btnManual_tw;
            this.btnManual.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnManual.FlatAppearance.BorderSize = 0;
            this.btnManual.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnManual.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManual.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnManual.Location = new System.Drawing.Point(2, 428);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(197, 45);
            this.btnManual.TabIndex = 23;
            this.btnManual.Tag = "2";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.Click += new System.EventHandler(this.PageChange);
            // 
            // btnHistory
            // 
            this.btnHistory.AutoSize = true;
            this.btnHistory.BackgroundImage = global::ADSW11000.Properties.Resources.btnHistory_tw;
            this.btnHistory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHistory.FlatAppearance.BorderSize = 0;
            this.btnHistory.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistory.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHistory.Location = new System.Drawing.Point(2, 624);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(197, 45);
            this.btnHistory.TabIndex = 22;
            this.btnHistory.Tag = "6";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.PageChange);
            // 
            // btnLang
            // 
            this.btnLang.AutoSize = true;
            this.btnLang.BackgroundImage = global::ADSW11000.Properties.Resources.btnLang_tw;
            this.btnLang.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLang.FlatAppearance.BorderSize = 0;
            this.btnLang.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLang.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnLang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLang.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLang.Location = new System.Drawing.Point(2, 574);
            this.btnLang.Name = "btnLang";
            this.btnLang.Size = new System.Drawing.Size(197, 45);
            this.btnLang.TabIndex = 21;
            this.btnLang.Tag = "5";
            this.btnLang.UseVisualStyleBackColor = true;
            this.btnLang.Click += new System.EventHandler(this.PageChange);
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.BackgroundImage = global::ADSW11000.Properties.Resources.btnReset_tw;
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReset.Location = new System.Drawing.Point(2, 250);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(197, 64);
            this.btnReset.TabIndex = 20;
            this.btnReset.Tag = "";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnPause
            // 
            this.btnPause.AutoEllipsis = true;
            this.btnPause.BackgroundImage = global::ADSW11000.Properties.Resources.btnPause_tw;
            this.btnPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPause.Location = new System.Drawing.Point(2, 186);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(197, 64);
            this.btnPause.TabIndex = 19;
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStart
            // 
            this.btnStart.BackgroundImage = global::ADSW11000.Properties.Resources.btnStart_tw;
            this.btnStart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(92)))), ((int)(((byte)(92)))));
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStart.Location = new System.Drawing.Point(2, 122);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(197, 64);
            this.btnStart.TabIndex = 18;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lbRunState
            // 
            this.lbRunState.BackColor = System.Drawing.Color.Yellow;
            this.lbRunState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbRunState.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbRunState.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold);
            this.lbRunState.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbRunState.Location = new System.Drawing.Point(0, 0);
            this.lbRunState.Name = "lbRunState";
            this.lbRunState.Size = new System.Drawing.Size(200, 63);
            this.lbRunState.TabIndex = 1;
            this.lbRunState.Text = "IDLE";
            this.lbRunState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            this.pnlTop.Controls.Add(this.pnlPackage);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(2, 2);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1162, 63);
            this.pnlTop.TabIndex = 3;
            // 
            // pnlPackage
            // 
            this.pnlPackage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.pnlPackage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlPackage.Controls.Add(this.lbPackage);
            this.pnlPackage.Controls.Add(this.lbProjectName);
            this.pnlPackage.Controls.Add(this.pictureBox1);
            this.pnlPackage.Controls.Add(this.lbPackageName);
            this.pnlPackage.Controls.Add(this.pnlUser);
            this.pnlPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPackage.Location = new System.Drawing.Point(0, 0);
            this.pnlPackage.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlPackage.Name = "pnlPackage";
            this.pnlPackage.Size = new System.Drawing.Size(1162, 63);
            this.pnlPackage.TabIndex = 4;
            // 
            // lbPackage
            // 
            this.lbPackage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbPackage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPackage.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbPackage.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold);
            this.lbPackage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbPackage.Location = new System.Drawing.Point(361, 0);
            this.lbPackage.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lbPackage.Name = "lbPackage";
            this.lbPackage.Size = new System.Drawing.Size(134, 59);
            this.lbPackage.TabIndex = 8;
            this.lbPackage.Text = "產品名稱";
            this.lbPackage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbProjectName
            // 
            this.lbProjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbProjectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbProjectName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbProjectName.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold);
            this.lbProjectName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbProjectName.Location = new System.Drawing.Point(200, 0);
            this.lbProjectName.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lbProjectName.Name = "lbProjectName";
            this.lbProjectName.Size = new System.Drawing.Size(161, 59);
            this.lbProjectName.TabIndex = 2;
            this.lbProjectName.Text = "專案名稱";
            this.lbProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::ADSW11000.Properties.Resources.provlogo;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 59);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // lbPackageName
            // 
            this.lbPackageName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPackageName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbPackageName.Font = new System.Drawing.Font("微軟正黑體", 18F);
            this.lbPackageName.Location = new System.Drawing.Point(499, 9);
            this.lbPackageName.Name = "lbPackageName";
            this.lbPackageName.Size = new System.Drawing.Size(428, 40);
            this.lbPackageName.TabIndex = 6;
            this.lbPackageName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlUser
            // 
            this.pnlUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.pnlUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pnlUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlUser.Controls.Add(this.btnChangeUser);
            this.pnlUser.Controls.Add(this.lbUserType);
            this.pnlUser.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlUser.Location = new System.Drawing.Point(931, 0);
            this.pnlUser.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pnlUser.Name = "pnlUser";
            this.pnlUser.Size = new System.Drawing.Size(227, 59);
            this.pnlUser.TabIndex = 5;
            // 
            // btnChangeUser
            // 
            this.btnChangeUser.BackgroundImage = global::ADSW11000.Properties.Resources.btnChangeUser_tw;
            this.btnChangeUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnChangeUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnChangeUser.Location = new System.Drawing.Point(2, 1);
            this.btnChangeUser.Name = "btnChangeUser";
            this.btnChangeUser.Size = new System.Drawing.Size(55, 55);
            this.btnChangeUser.TabIndex = 3;
            this.btnChangeUser.UseVisualStyleBackColor = true;
            this.btnChangeUser.Click += new System.EventHandler(this.btnChangeUser_Click);
            // 
            // lbUserType
            // 
            this.lbUserType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbUserType.Dock = System.Windows.Forms.DockStyle.Right;
            this.lbUserType.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold);
            this.lbUserType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbUserType.Location = new System.Drawing.Point(58, 0);
            this.lbUserType.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lbUserType.Name = "lbUserType";
            this.lbUserType.Size = new System.Drawing.Size(167, 57);
            this.lbUserType.TabIndex = 2;
            this.lbUserType.Text = "Not Login";
            this.lbUserType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlAlarmGrid
            // 
            this.pnlAlarmGrid.Controls.Add(this.lvArmGrid);
            this.pnlAlarmGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAlarmGrid.Location = new System.Drawing.Point(2, 600);
            this.pnlAlarmGrid.Name = "pnlAlarmGrid";
            this.pnlAlarmGrid.Size = new System.Drawing.Size(1162, 166);
            this.pnlAlarmGrid.TabIndex = 7;
            // 
            // lvArmGrid
            // 
            this.lvArmGrid.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvArmGrid.BackColor = System.Drawing.Color.White;
            this.lvArmGrid.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvArmGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvArmGrid.Font = new System.Drawing.Font("微軟正黑體", 14.25F);
            this.lvArmGrid.FullRowSelect = true;
            this.lvArmGrid.GridLines = true;
            this.lvArmGrid.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvArmGrid.HideSelection = false;
            this.lvArmGrid.Location = new System.Drawing.Point(0, 0);
            this.lvArmGrid.MultiSelect = false;
            this.lvArmGrid.Name = "lvArmGrid";
            this.lvArmGrid.Size = new System.Drawing.Size(1162, 166);
            this.lvArmGrid.TabIndex = 0;
            this.lvArmGrid.UseCompatibleStateImageBehavior = false;
            this.lvArmGrid.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 5;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "日期時間";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 250;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "模組/元件";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "錯誤碼";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "錯誤說明";
            this.columnHeader5.Width = 650;
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.tabMachineState);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(2, 65);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(1162, 535);
            this.pnlContainer.TabIndex = 8;
            // 
            // tabMachineState
            // 
            this.tabMachineState.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabMachineState.Controls.Add(this.tpProductionState);
            this.tabMachineState.Controls.Add(this.tpKernalData);
            this.tabMachineState.Controls.Add(this.tpMSS);
            this.tabMachineState.Controls.Add(this.tpMainFlow);
            this.tabMachineState.Controls.Add(this.tpCommunication);
            this.tabMachineState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMachineState.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold);
            this.tabMachineState.ItemSize = new System.Drawing.Size(220, 40);
            this.tabMachineState.Location = new System.Drawing.Point(0, 0);
            this.tabMachineState.Name = "tabMachineState";
            this.tabMachineState.SelectedIndex = 0;
            this.tabMachineState.Size = new System.Drawing.Size(1162, 535);
            this.tabMachineState.TabIndex = 0;
            // 
            // tpProductionState
            // 
            this.tpProductionState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpProductionState.Controls.Add(this.pnl異常通知);
            this.tpProductionState.Font = new System.Drawing.Font("微軟正黑體", 15.75F);
            this.tpProductionState.Location = new System.Drawing.Point(4, 44);
            this.tpProductionState.Name = "tpProductionState";
            this.tpProductionState.Padding = new System.Windows.Forms.Padding(3);
            this.tpProductionState.Size = new System.Drawing.Size(1154, 487);
            this.tpProductionState.TabIndex = 0;
            this.tpProductionState.Text = "生 產 狀 態";
            // 
            // tpKernalData
            // 
            this.tpKernalData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpKernalData.Controls.Add(this.TB_授權有效性);
            this.tpKernalData.Controls.Add(this.TB_授權模式);
            this.tpKernalData.Controls.Add(this.btnResetManual);
            this.tpKernalData.Controls.Add(this.btnResetHome);
            this.tpKernalData.Controls.Add(this.btnResetIDLE);
            this.tpKernalData.Controls.Add(this.lbManualTM);
            this.tpKernalData.Controls.Add(this.lbHomeTM);
            this.tpKernalData.Controls.Add(this.lbIdleTM);
            this.tpKernalData.Controls.Add(this.lbPauseTM);
            this.tpKernalData.Controls.Add(this.lbRunTM);
            this.tpKernalData.Controls.Add(this.lbOperationEndTM);
            this.tpKernalData.Controls.Add(this.lbOperationStartTM);
            this.tpKernalData.Controls.Add(this.lbScanCNT);
            this.tpKernalData.Controls.Add(this.lbScanTM);
            this.tpKernalData.Controls.Add(this.label1);
            this.tpKernalData.Font = new System.Drawing.Font("微軟正黑體", 15.75F);
            this.tpKernalData.Location = new System.Drawing.Point(4, 44);
            this.tpKernalData.Name = "tpKernalData";
            this.tpKernalData.Padding = new System.Windows.Forms.Padding(3);
            this.tpKernalData.Size = new System.Drawing.Size(1154, 487);
            this.tpKernalData.TabIndex = 1;
            this.tpKernalData.Text = "核心資料/生產數據";
            // 
            // TB_授權有效性
            // 
            this.TB_授權有效性.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.TB_授權有效性.Caption = "授權有效性";
            this.TB_授權有效性.CaptionColor = System.Drawing.Color.Black;
            this.TB_授權有效性.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TB_授權有效性.CaptionWidth = 150;
            this.TB_授權有效性.Location = new System.Drawing.Point(20, 361);
            this.TB_授權有效性.Name = "TB_授權有效性";
            this.TB_授權有效性.Size = new System.Drawing.Size(370, 30);
            this.TB_授權有效性.TabIndex = 77;
            this.TB_授權有效性.Value = "";
            this.TB_授權有效性.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.TB_授權有效性.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TB_授權有效性.Visible = false;
            // 
            // TB_授權模式
            // 
            this.TB_授權模式.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.TB_授權模式.Caption = "授權模式";
            this.TB_授權模式.CaptionColor = System.Drawing.Color.Black;
            this.TB_授權模式.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TB_授權模式.CaptionWidth = 150;
            this.TB_授權模式.Location = new System.Drawing.Point(20, 330);
            this.TB_授權模式.Name = "TB_授權模式";
            this.TB_授權模式.Size = new System.Drawing.Size(370, 30);
            this.TB_授權模式.TabIndex = 76;
            this.TB_授權模式.Value = "";
            this.TB_授權模式.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.TB_授權模式.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TB_授權模式.Visible = false;
            // 
            // btnResetManual
            // 
            this.btnResetManual.Location = new System.Drawing.Point(393, 113);
            this.btnResetManual.Name = "btnResetManual";
            this.btnResetManual.Size = new System.Drawing.Size(36, 30);
            this.btnResetManual.TabIndex = 72;
            this.btnResetManual.Text = "Z";
            this.btnResetManual.UseVisualStyleBackColor = true;
            this.btnResetManual.Click += new System.EventHandler(this.btnResetManual_Click);
            // 
            // btnResetHome
            // 
            this.btnResetHome.Location = new System.Drawing.Point(393, 82);
            this.btnResetHome.Name = "btnResetHome";
            this.btnResetHome.Size = new System.Drawing.Size(36, 30);
            this.btnResetHome.TabIndex = 71;
            this.btnResetHome.Text = "Z";
            this.btnResetHome.UseVisualStyleBackColor = true;
            this.btnResetHome.Click += new System.EventHandler(this.btnResetHome_Click);
            // 
            // btnResetIDLE
            // 
            this.btnResetIDLE.Location = new System.Drawing.Point(393, 51);
            this.btnResetIDLE.Name = "btnResetIDLE";
            this.btnResetIDLE.Size = new System.Drawing.Size(36, 30);
            this.btnResetIDLE.TabIndex = 70;
            this.btnResetIDLE.Text = "Z";
            this.btnResetIDLE.UseVisualStyleBackColor = true;
            this.btnResetIDLE.Click += new System.EventHandler(this.btnResetIDLE_Click);
            // 
            // lbManualTM
            // 
            this.lbManualTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbManualTM.Caption = "手動操作累計時間";
            this.lbManualTM.CaptionColor = System.Drawing.Color.Black;
            this.lbManualTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbManualTM.CaptionWidth = 150;
            this.lbManualTM.Location = new System.Drawing.Point(20, 113);
            this.lbManualTM.Name = "lbManualTM";
            this.lbManualTM.Size = new System.Drawing.Size(370, 30);
            this.lbManualTM.TabIndex = 69;
            this.lbManualTM.Value = "";
            this.lbManualTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbManualTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbHomeTM
            // 
            this.lbHomeTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbHomeTM.Caption = "歸零累計時間";
            this.lbHomeTM.CaptionColor = System.Drawing.Color.Black;
            this.lbHomeTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbHomeTM.CaptionWidth = 150;
            this.lbHomeTM.Location = new System.Drawing.Point(20, 82);
            this.lbHomeTM.Name = "lbHomeTM";
            this.lbHomeTM.Size = new System.Drawing.Size(370, 30);
            this.lbHomeTM.TabIndex = 68;
            this.lbHomeTM.Value = "";
            this.lbHomeTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbHomeTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbIdleTM
            // 
            this.lbIdleTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbIdleTM.Caption = "IDLE累計時間";
            this.lbIdleTM.CaptionColor = System.Drawing.Color.Black;
            this.lbIdleTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbIdleTM.CaptionWidth = 150;
            this.lbIdleTM.Location = new System.Drawing.Point(20, 51);
            this.lbIdleTM.Name = "lbIdleTM";
            this.lbIdleTM.Size = new System.Drawing.Size(370, 30);
            this.lbIdleTM.TabIndex = 67;
            this.lbIdleTM.Value = "";
            this.lbIdleTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbIdleTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbPauseTM
            // 
            this.lbPauseTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbPauseTM.Caption = "機台暫停時間";
            this.lbPauseTM.CaptionColor = System.Drawing.Color.Black;
            this.lbPauseTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbPauseTM.CaptionWidth = 150;
            this.lbPauseTM.Location = new System.Drawing.Point(20, 237);
            this.lbPauseTM.Name = "lbPauseTM";
            this.lbPauseTM.Size = new System.Drawing.Size(370, 30);
            this.lbPauseTM.TabIndex = 64;
            this.lbPauseTM.Value = "";
            this.lbPauseTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbPauseTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbRunTM
            // 
            this.lbRunTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbRunTM.Caption = "機台運行時間";
            this.lbRunTM.CaptionColor = System.Drawing.Color.Black;
            this.lbRunTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbRunTM.CaptionWidth = 150;
            this.lbRunTM.Location = new System.Drawing.Point(20, 206);
            this.lbRunTM.Name = "lbRunTM";
            this.lbRunTM.Size = new System.Drawing.Size(370, 30);
            this.lbRunTM.TabIndex = 63;
            this.lbRunTM.Value = "";
            this.lbRunTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbRunTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbOperationEndTM
            // 
            this.lbOperationEndTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbOperationEndTM.Caption = "作業結束時間";
            this.lbOperationEndTM.CaptionColor = System.Drawing.Color.Black;
            this.lbOperationEndTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbOperationEndTM.CaptionWidth = 150;
            this.lbOperationEndTM.Location = new System.Drawing.Point(20, 175);
            this.lbOperationEndTM.Name = "lbOperationEndTM";
            this.lbOperationEndTM.Size = new System.Drawing.Size(370, 30);
            this.lbOperationEndTM.TabIndex = 62;
            this.lbOperationEndTM.Value = "";
            this.lbOperationEndTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbOperationEndTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbOperationStartTM
            // 
            this.lbOperationStartTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.lbOperationStartTM.Caption = "開始作業時間";
            this.lbOperationStartTM.CaptionColor = System.Drawing.Color.Black;
            this.lbOperationStartTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbOperationStartTM.CaptionWidth = 150;
            this.lbOperationStartTM.Location = new System.Drawing.Point(20, 144);
            this.lbOperationStartTM.Name = "lbOperationStartTM";
            this.lbOperationStartTM.Size = new System.Drawing.Size(370, 30);
            this.lbOperationStartTM.TabIndex = 61;
            this.lbOperationStartTM.Value = "";
            this.lbOperationStartTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbOperationStartTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbScanCNT
            // 
            this.lbScanCNT.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbScanCNT.Caption = "Scan Count";
            this.lbScanCNT.CaptionColor = System.Drawing.Color.Black;
            this.lbScanCNT.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbScanCNT.CaptionWidth = 150;
            this.lbScanCNT.CheckLimit = false;
            this.lbScanCNT.Location = new System.Drawing.Point(20, 299);
            this.lbScanCNT.LowerLimit = 0D;
            this.lbScanCNT.Name = "lbScanCNT";
            this.lbScanCNT.OverLimitColor = System.Drawing.Color.Red;
            this.lbScanCNT.Size = new System.Drawing.Size(370, 30);
            this.lbScanCNT.TabIndex = 66;
            this.lbScanCNT.Unit = "t/s";
            this.lbScanCNT.UnitFont = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            this.lbScanCNT.UnitWidth = 57;
            this.lbScanCNT.UpperLimit = 0D;
            this.lbScanCNT.Value = "0";
            this.lbScanCNT.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbScanCNT.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // lbScanTM
            // 
            this.lbScanTM.BGColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbScanTM.Caption = "Scan Time";
            this.lbScanTM.CaptionColor = System.Drawing.Color.Black;
            this.lbScanTM.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbScanTM.CaptionWidth = 150;
            this.lbScanTM.CheckLimit = false;
            this.lbScanTM.Location = new System.Drawing.Point(20, 268);
            this.lbScanTM.LowerLimit = 0D;
            this.lbScanTM.Name = "lbScanTM";
            this.lbScanTM.OverLimitColor = System.Drawing.Color.Red;
            this.lbScanTM.Size = new System.Drawing.Size(370, 30);
            this.lbScanTM.TabIndex = 65;
            this.lbScanTM.Unit = "ms";
            this.lbScanTM.UnitFont = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            this.lbScanTM.UnitWidth = 57;
            this.lbScanTM.UpperLimit = 0D;
            this.lbScanTM.Value = "0";
            this.lbScanTM.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.lbScanTM.ValueFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(20, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(370, 30);
            this.label1.TabIndex = 44;
            this.label1.Text = "系統資訊";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpMSS
            // 
            this.tpMSS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpMSS.Location = new System.Drawing.Point(4, 44);
            this.tpMSS.Name = "tpMSS";
            this.tpMSS.Padding = new System.Windows.Forms.Padding(3);
            this.tpMSS.Size = new System.Drawing.Size(1154, 487);
            this.tpMSS.TabIndex = 2;
            this.tpMSS.Text = "MSS";
            // 
            // tpMainFlow
            // 
            this.tpMainFlow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpMainFlow.Location = new System.Drawing.Point(4, 44);
            this.tpMainFlow.Name = "tpMainFlow";
            this.tpMainFlow.Padding = new System.Windows.Forms.Padding(3);
            this.tpMainFlow.Size = new System.Drawing.Size(1154, 487);
            this.tpMainFlow.TabIndex = 3;
            this.tpMainFlow.Text = "主控流程";
            // 
            // tmUIRefresh
            // 
            this.tmUIRefresh.Interval = 10;
            this.tmUIRefresh.Tick += new System.EventHandler(this.tmUIRefresh_Tick);
            // 
            // pnl異常通知
            // 
            this.pnl異常通知.BackColor = System.Drawing.Color.Red;
            this.pnl異常通知.Controls.Add(this.lb異常訊息);
            this.pnl異常通知.Location = new System.Drawing.Point(1017, 6);
            this.pnl異常通知.Name = "pnl異常通知";
            this.pnl異常通知.Size = new System.Drawing.Size(131, 114);
            this.pnl異常通知.TabIndex = 5;
            this.pnl異常通知.Visible = false;
            // 
            // lb異常訊息
            // 
            this.lb異常訊息.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb異常訊息.Font = new System.Drawing.Font("微軟正黑體", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lb異常訊息.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lb異常訊息.Location = new System.Drawing.Point(0, 0);
            this.lb異常訊息.Name = "lb異常訊息";
            this.lb異常訊息.Size = new System.Drawing.Size(131, 114);
            this.lb異常訊息.TabIndex = 0;
            this.lb異常訊息.Text = "緊停異常通知\r\nEMG Stop";
            this.lb異常訊息.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpCommunication
            // 
            this.tpCommunication.Location = new System.Drawing.Point(4, 44);
            this.tpCommunication.Name = "tpCommunication";
            this.tpCommunication.Size = new System.Drawing.Size(1154, 487);
            this.tpCommunication.TabIndex = 4;
            this.tpCommunication.Text = "通訊相關";
            this.tpCommunication.UseVisualStyleBackColor = true;
            // 
            // MainF2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1366, 768);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.pnlAlarmGrid);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlControl);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "MainF2";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "主頁";
            this.Load += new System.EventHandler(this.MainF1_Load);
            this.Shown += new System.EventHandler(this.MainF1_Shown);
            this.pnlControl.ResumeLayout(false);
            this.pnlControl.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlPackage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlUser.ResumeLayout(false);
            this.pnlAlarmGrid.ResumeLayout(false);
            this.pnlContainer.ResumeLayout(false);
            this.tabMachineState.ResumeLayout(false);
            this.tpProductionState.ResumeLayout(false);
            this.tpKernalData.ResumeLayout(false);
            this.pnl異常通知.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlControl;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlPackage;
        private System.Windows.Forms.Panel pnlUser;
        private System.Windows.Forms.Button btnChangeUser;
        private System.Windows.Forms.Label lbUserType;
        private System.Windows.Forms.Label lbProjectName;
        private System.Windows.Forms.Label lbPackageName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel pnlAlarmGrid;
        public System.Windows.Forms.ListView lvArmGrid;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label lbPackage;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.TabControl tabMachineState;
        private System.Windows.Forms.TabPage tpProductionState;
        private System.Windows.Forms.TabPage tpKernalData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbRunState;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnPackage;
        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.Button btnLang;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnAuto;
        private System.Windows.Forms.Label lbNowTime;
        private System.Windows.Forms.Label lbVersion;
        private KCSDK.ThreeColorLight ledGreen;
        private KCSDK.ThreeColorLight ledYellow;
        private KCSDK.ThreeColorLight ledRed;
        private KCSDK.NumLabel lbScanCNT;
        private KCSDK.NumLabel lbScanTM;
        private KCSDK.TextLabel lbPauseTM;
        private KCSDK.TextLabel lbRunTM;
        private KCSDK.TextLabel lbOperationEndTM;
        private KCSDK.TextLabel lbOperationStartTM;
        private System.Windows.Forms.Timer tmUIRefresh;
        private System.Windows.Forms.TabPage tpMSS;
        private System.Windows.Forms.Button btnOption;
        private System.Windows.Forms.TabPage tpMainFlow;
        private System.Windows.Forms.Button btnHome;
        private KCSDK.TextLabel lbManualTM;
        private KCSDK.TextLabel lbHomeTM;
        private KCSDK.TextLabel lbIdleTM;
        private System.Windows.Forms.Button btnResetManual;
        private System.Windows.Forms.Button btnResetHome;
        private System.Windows.Forms.Button btnResetIDLE;
        private KCSDK.TextLabel TB_授權有效性;
        private KCSDK.TextLabel TB_授權模式;
        public System.Windows.Forms.Panel pnl異常通知;
        private System.Windows.Forms.Label lb異常訊息;
        private System.Windows.Forms.TabPage tpCommunication;
    }
}

