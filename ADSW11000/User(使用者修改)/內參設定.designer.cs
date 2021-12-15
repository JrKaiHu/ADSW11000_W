namespace ADSW11000
{
    partial class 內參設定
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(內參設定));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbInnerParam = new System.Windows.Forms.TabControl();
            this.tpApp = new System.Windows.Forms.TabPage();
            this.dFieldEdit27 = new KCSDK.DFieldEdit();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.DGV_SPCSet = new KCSDK.DDataGridView();
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.dEdit1 = new KCSDK.DEdit();
            this.label4 = new System.Windows.Forms.Label();
            this.dComboBox2 = new KCSDK.DComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dFieldEdit1 = new KCSDK.DFieldEdit();
            this.tpSys = new System.Windows.Forms.TabPage();
            this.dCheckBox25 = new KCSDK.DCheckBox(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dFieldEdit5 = new KCSDK.DFieldEdit();
            this.dFieldEdit4 = new KCSDK.DFieldEdit();
            this.dFieldEdit3 = new KCSDK.DFieldEdit();
            this.dRadioGroupBox1 = new KCSDK.DRadioGroupBox(this.components);
            this.lblHelp = new System.Windows.Forms.Label();
            this.dCheckBox1 = new KCSDK.DCheckBox(this.components);
            this.dFieldEdit2 = new KCSDK.DFieldEdit();
            this.panel2.SuspendLayout();
            this.tbInnerParam.SuspendLayout();
            this.tpApp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_SPCSet)).BeginInit();
            this.tpSys.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.dRadioGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.Images.SetKeyName(0, "position.png");
            this.imgList.Images.SetKeyName(1, "motor.png");
            this.imgList.Images.SetKeyName(2, "setting.png");
            this.imgList.Images.SetKeyName(3, "checklist.png");
            this.imgList.Images.SetKeyName(4, "edit.png");
            this.imgList.Images.SetKeyName(5, "save.png");
            this.imgList.Images.SetKeyName(6, "cancel.png");
            this.imgList.Images.SetKeyName(7, "addrow.png");
            this.imgList.Images.SetKeyName(8, "delrow1.png");
            this.imgList.Images.SetKeyName(9, "closebtn.png");
            // 
            // label1
            // 
            this.label1.Text = "內參設定";
            // 
            // btnCancel2
            // 
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel2_Click);
            // 
            // btnSave2
            // 
            this.btnSave2.Click += new System.EventHandler(this.btnSave2_Click);
            // 
            // btnEdit2
            // 
            this.btnEdit2.Click += new System.EventHandler(this.btnEdit2_Click);
            // 
            // btnClose
            // 
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.tbInnerParam);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 58);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1229, 672);
            this.panel2.TabIndex = 5;
            // 
            // tbInnerParam
            // 
            this.tbInnerParam.Controls.Add(this.tpApp);
            this.tbInnerParam.Controls.Add(this.tpSys);
            this.tbInnerParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbInnerParam.Location = new System.Drawing.Point(0, 0);
            this.tbInnerParam.Name = "tbInnerParam";
            this.tbInnerParam.SelectedIndex = 0;
            this.tbInnerParam.Size = new System.Drawing.Size(1229, 672);
            this.tbInnerParam.TabIndex = 52;
            // 
            // tpApp
            // 
            this.tpApp.Controls.Add(this.dFieldEdit27);
            this.tpApp.Controls.Add(this.groupBox1);
            this.tpApp.Location = new System.Drawing.Point(4, 32);
            this.tpApp.Name = "tpApp";
            this.tpApp.Padding = new System.Windows.Forms.Padding(3);
            this.tpApp.Size = new System.Drawing.Size(1221, 636);
            this.tpApp.TabIndex = 0;
            this.tpApp.Text = "應用";
            this.tpApp.UseVisualStyleBackColor = true;
            // 
            // dFieldEdit27
            // 
            this.dFieldEdit27.AutoFocus = false;
            this.dFieldEdit27.Caption = "IO描述檔";
            this.dFieldEdit27.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit27.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit27.DataName = "IO描述檔";
            this.dFieldEdit27.DataSource = this.OptionDS;
            this.dFieldEdit27.DefaultValue = null;
            this.dFieldEdit27.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit27.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit27.EditWidth = 200;
            this.dFieldEdit27.FieldValue = "";
            this.dFieldEdit27.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit27.IsModified = false;
            this.dFieldEdit27.Location = new System.Drawing.Point(19, 13);
            this.dFieldEdit27.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit27.MaxValue = 5D;
            this.dFieldEdit27.MinValue = 0D;
            this.dFieldEdit27.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit27.Name = "dFieldEdit27";
            this.dFieldEdit27.NoChangeInAuto = false;
            this.dFieldEdit27.Size = new System.Drawing.Size(284, 30);
            this.dFieldEdit27.StepValue = 0D;
            this.dFieldEdit27.TabIndex = 45;
            this.dFieldEdit27.Unit = "組";
            this.dFieldEdit27.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit27.UnitWidth = 0;
            this.dFieldEdit27.ValueType = KCSDK.ValueDataType.String;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDel);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.DGV_SPCSet);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.dEdit1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dComboBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dFieldEdit1);
            this.groupBox1.Location = new System.Drawing.Point(19, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(580, 386);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OEE Report";
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(422, 157);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(146, 40);
            this.btnDel.TabIndex = 50;
            this.btnDel.Text = "刪除統計項目";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(422, 111);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(146, 40);
            this.btnAdd.TabIndex = 49;
            this.btnAdd.Text = "新增統計項目";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // DGV_SPCSet
            // 
            this.DGV_SPCSet.AllowUserToAddRows = false;
            this.DGV_SPCSet.AllowUserToDeleteRows = false;
            this.DGV_SPCSet.AllowUserToResizeColumns = false;
            this.DGV_SPCSet.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DGV_SPCSet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DGV_SPCSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_SPCSet.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colName,
            this.colType});
            this.DGV_SPCSet.DataName = "SPCSetting";
            this.DGV_SPCSet.Location = new System.Drawing.Point(24, 110);
            this.DGV_SPCSet.Name = "DGV_SPCSet";
            this.DGV_SPCSet.NoChangeInAuto = true;
            this.DGV_SPCSet.RowHeight = 30;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 14F);
            this.DGV_SPCSet.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DGV_SPCSet.RowTemplate.Height = 24;
            this.DGV_SPCSet.Size = new System.Drawing.Size(392, 270);
            this.DGV_SPCSet.TabIndex = 48;
            this.DGV_SPCSet.TableDataSource = this.OptionDS;
            // 
            // colIndex
            // 
            this.colIndex.HeaderText = "項目";
            this.colIndex.Name = "colIndex";
            // 
            // colName
            // 
            this.colName.HeaderText = "名稱";
            this.colName.Name = "colName";
            // 
            // colType
            // 
            this.colType.HeaderText = "統計方式";
            this.colType.Items.AddRange(new object[] {
            "Sum",
            "Average"});
            this.colType.Name = "colType";
            this.colType.Width = 145;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(513, 75);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(55, 30);
            this.btnBrowse.TabIndex = 16;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // dEdit1
            // 
            this.dEdit1.AutoFocus = false;
            this.dEdit1.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit1.DataName = "ReportFileDir";
            this.dEdit1.DataSource = this.OptionDS;
            this.dEdit1.DefaultValue = "D:\\";
            this.dEdit1.IsModified = false;
            this.dEdit1.Location = new System.Drawing.Point(142, 75);
            this.dEdit1.MaxValue = 0D;
            this.dEdit1.MinValue = 0D;
            this.dEdit1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit1.Name = "dEdit1";
            this.dEdit1.NoChangeInAuto = false;
            this.dEdit1.Size = new System.Drawing.Size(365, 31);
            this.dEdit1.StepValue = 0D;
            this.dEdit1.TabIndex = 15;
            this.dEdit1.Text = "D:\\";
            this.dEdit1.ValueType = KCSDK.ValueDataType.String;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label4.Location = new System.Drawing.Point(30, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 23);
            this.label4.TabIndex = 14;
            this.label4.Text = "報告輸出路徑";
            // 
            // dComboBox2
            // 
            this.dComboBox2.BackColor = System.Drawing.SystemColors.Window;
            this.dComboBox2.DataName = "MeanTimeType";
            this.dComboBox2.DataSource = this.OptionDS;
            this.dComboBox2.DefaultValue = 0;
            this.dComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dComboBox2.FormattingEnabled = true;
            this.dComboBox2.IsModified = false;
            this.dComboBox2.Items.AddRange(new object[] {
            "MTBA",
            "MTBF"});
            this.dComboBox2.Location = new System.Drawing.Point(407, 35);
            this.dComboBox2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dComboBox2.Name = "dComboBox2";
            this.dComboBox2.NoChangeInAuto = true;
            this.dComboBox2.Size = new System.Drawing.Size(161, 31);
            this.dComboBox2.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(295, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 23);
            this.label3.TabIndex = 12;
            this.label3.Text = "錯誤計算設定";
            // 
            // dFieldEdit1
            // 
            this.dFieldEdit1.AutoFocus = false;
            this.dFieldEdit1.Caption = "設備負載時間";
            this.dFieldEdit1.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit1.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.DataName = "EQPLoadTime";
            this.dFieldEdit1.DataSource = this.OptionDS;
            this.dFieldEdit1.DefaultValue = null;
            this.dFieldEdit1.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit1.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.EditWidth = 90;
            this.dFieldEdit1.FieldValue = "22";
            this.dFieldEdit1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.IsModified = false;
            this.dFieldEdit1.Location = new System.Drawing.Point(24, 34);
            this.dFieldEdit1.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit1.MaxValue = 24D;
            this.dFieldEdit1.MinValue = 1D;
            this.dFieldEdit1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit1.Name = "dFieldEdit1";
            this.dFieldEdit1.NoChangeInAuto = false;
            this.dFieldEdit1.Size = new System.Drawing.Size(260, 29);
            this.dFieldEdit1.StepValue = 0D;
            this.dFieldEdit1.TabIndex = 8;
            this.dFieldEdit1.Unit = "hrs";
            this.dFieldEdit1.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.UnitWidth = 50;
            this.dFieldEdit1.ValueType = KCSDK.ValueDataType.Int;
            // 
            // tpSys
            // 
            this.tpSys.Controls.Add(this.dCheckBox25);
            this.tpSys.Controls.Add(this.groupBox2);
            this.tpSys.Controls.Add(this.dFieldEdit3);
            this.tpSys.Controls.Add(this.dRadioGroupBox1);
            this.tpSys.Controls.Add(this.dCheckBox1);
            this.tpSys.Controls.Add(this.dFieldEdit2);
            this.tpSys.Location = new System.Drawing.Point(4, 32);
            this.tpSys.Name = "tpSys";
            this.tpSys.Padding = new System.Windows.Forms.Padding(3);
            this.tpSys.Size = new System.Drawing.Size(1221, 636);
            this.tpSys.TabIndex = 1;
            this.tpSys.Text = "系統";
            this.tpSys.UseVisualStyleBackColor = true;
            // 
            // dCheckBox25
            // 
            this.dCheckBox25.AutoSize = true;
            this.dCheckBox25.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox25.DataName = "UseDAQ";
            this.dCheckBox25.DataSource = this.OptionDS;
            this.dCheckBox25.DefaultValue = false;
            this.dCheckBox25.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox25.IsModified = false;
            this.dCheckBox25.Location = new System.Drawing.Point(371, 178);
            this.dCheckBox25.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox25.Name = "dCheckBox25";
            this.dCheckBox25.NoChangeInAuto = false;
            this.dCheckBox25.Size = new System.Drawing.Size(196, 27);
            this.dCheckBox25.TabIndex = 188;
            this.dCheckBox25.Text = "使用DAQ卡偵測破刀";
            this.dCheckBox25.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dFieldEdit5);
            this.groupBox2.Controls.Add(this.dFieldEdit4);
            this.groupBox2.Location = new System.Drawing.Point(371, 64);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 108);
            this.groupBox2.TabIndex = 54;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "絕對式光學尺基準脈波數設定";
            // 
            // dFieldEdit5
            // 
            this.dFieldEdit5.AutoFocus = false;
            this.dFieldEdit5.Caption = "Z2X";
            this.dFieldEdit5.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit5.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit5.DataName = "Z2XBasePulse";
            this.dFieldEdit5.DataSource = this.OptionDS;
            this.dFieldEdit5.DefaultValue = null;
            this.dFieldEdit5.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit5.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit5.EditWidth = 120;
            this.dFieldEdit5.FieldValue = "12018705";
            this.dFieldEdit5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit5.IsModified = false;
            this.dFieldEdit5.Location = new System.Drawing.Point(14, 64);
            this.dFieldEdit5.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit5.MaxValue = 99999999D;
            this.dFieldEdit5.MinValue = -99999999D;
            this.dFieldEdit5.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit5.Name = "dFieldEdit5";
            this.dFieldEdit5.NoChangeInAuto = false;
            this.dFieldEdit5.Size = new System.Drawing.Size(245, 29);
            this.dFieldEdit5.StepValue = 0D;
            this.dFieldEdit5.TabIndex = 54;
            this.dFieldEdit5.Unit = "Pulse";
            this.dFieldEdit5.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit5.UnitWidth = 60;
            this.dFieldEdit5.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dFieldEdit4
            // 
            this.dFieldEdit4.AutoFocus = false;
            this.dFieldEdit4.Caption = "Z1X";
            this.dFieldEdit4.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit4.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit4.DataName = "Z1XBasePulse";
            this.dFieldEdit4.DataSource = this.OptionDS;
            this.dFieldEdit4.DefaultValue = null;
            this.dFieldEdit4.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit4.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit4.EditWidth = 120;
            this.dFieldEdit4.FieldValue = "-692339";
            this.dFieldEdit4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit4.IsModified = false;
            this.dFieldEdit4.Location = new System.Drawing.Point(14, 25);
            this.dFieldEdit4.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit4.MaxValue = 99999999D;
            this.dFieldEdit4.MinValue = -99999999D;
            this.dFieldEdit4.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit4.Name = "dFieldEdit4";
            this.dFieldEdit4.NoChangeInAuto = false;
            this.dFieldEdit4.Size = new System.Drawing.Size(245, 29);
            this.dFieldEdit4.StepValue = 0D;
            this.dFieldEdit4.TabIndex = 53;
            this.dFieldEdit4.Unit = "Pulse";
            this.dFieldEdit4.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit4.UnitWidth = 60;
            this.dFieldEdit4.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dFieldEdit3
            // 
            this.dFieldEdit3.AutoFocus = false;
            this.dFieldEdit3.Caption = "設備慢速運行的整機速率";
            this.dFieldEdit3.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit3.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.DataName = "EQPSlowRunSpeed";
            this.dFieldEdit3.DataSource = this.OptionDS;
            this.dFieldEdit3.DefaultValue = null;
            this.dFieldEdit3.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit3.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.EditWidth = 50;
            this.dFieldEdit3.FieldValue = "10";
            this.dFieldEdit3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.IsModified = false;
            this.dFieldEdit3.Location = new System.Drawing.Point(18, 416);
            this.dFieldEdit3.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit3.MaxValue = 100D;
            this.dFieldEdit3.MinValue = 10D;
            this.dFieldEdit3.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit3.Name = "dFieldEdit3";
            this.dFieldEdit3.NoChangeInAuto = false;
            this.dFieldEdit3.Size = new System.Drawing.Size(332, 29);
            this.dFieldEdit3.StepValue = 0D;
            this.dFieldEdit3.TabIndex = 52;
            this.dFieldEdit3.Unit = "%";
            this.dFieldEdit3.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.UnitWidth = 50;
            this.dFieldEdit3.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dRadioGroupBox1
            // 
            this.dRadioGroupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox1.ColCount = 1;
            this.dRadioGroupBox1.Controls.Add(this.lblHelp);
            this.dRadioGroupBox1.DataName = "安全門機制設定";
            this.dRadioGroupBox1.DataSource = this.OptionDS;
            this.dRadioGroupBox1.DefaultValue = 0;
            this.dRadioGroupBox1.Font = new System.Drawing.Font("微軟正黑體", 14F, System.Drawing.FontStyle.Bold);
            this.dRadioGroupBox1.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox1.IsModified = false;
            this.dRadioGroupBox1.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox1.ItemHeight = 30;
            this.dRadioGroupBox1.ItemLeft = 10;
            this.dRadioGroupBox1.ItemTop = 30;
            this.dRadioGroupBox1.ItemWidth = 100;
            this.dRadioGroupBox1.Location = new System.Drawing.Point(18, 64);
            this.dRadioGroupBox1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox1.Name = "dRadioGroupBox1";
            this.dRadioGroupBox1.NoChangeInAuto = false;
            this.dRadioGroupBox1.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox1.RadioItems")));
            this.dRadioGroupBox1.Size = new System.Drawing.Size(332, 294);
            this.dRadioGroupBox1.TabIndex = 51;
            this.dRadioGroupBox1.TabStop = false;
            this.dRadioGroupBox1.Text = "安全門機制設定";
            // 
            // lblHelp
            // 
            this.lblHelp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblHelp.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblHelp.Location = new System.Drawing.Point(6, 131);
            this.lblHelp.Name = "lblHelp";
            this.lblHelp.Size = new System.Drawing.Size(320, 149);
            this.lblHelp.TabIndex = 3;
            this.lblHelp.Text = "進階安全門機制會在安全門開啟被偵測到時發出警報並有顯眼的提示，解除警報後啟動機台會在5秒後開始以低速運行慢速循環週期的設定次數，再切回原工作速度運行。此機制需搭配" +
    "FlowChart設定流程頭來達到此功能。詳細說明請參考軟體標準化說明文件中功能文件的安全門機制改善.pdf";
            // 
            // dCheckBox1
            // 
            this.dCheckBox1.AutoSize = true;
            this.dCheckBox1.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox1.DataName = "伺服錯誤需復歸";
            this.dCheckBox1.DataSource = this.OptionDS;
            this.dCheckBox1.DefaultValue = false;
            this.dCheckBox1.IsModified = false;
            this.dCheckBox1.Location = new System.Drawing.Point(18, 21);
            this.dCheckBox1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox1.Name = "dCheckBox1";
            this.dCheckBox1.NoChangeInAuto = false;
            this.dCheckBox1.Size = new System.Drawing.Size(191, 27);
            this.dCheckBox1.TabIndex = 46;
            this.dCheckBox1.Text = "伺服錯誤需執行復歸";
            this.dCheckBox1.UseVisualStyleBackColor = true;
            // 
            // dFieldEdit2
            // 
            this.dFieldEdit2.AutoFocus = false;
            this.dFieldEdit2.Caption = "設備慢速循環週期次數";
            this.dFieldEdit2.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit2.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.DataName = "EQPSlowRunCycle";
            this.dFieldEdit2.DataSource = this.OptionDS;
            this.dFieldEdit2.DefaultValue = null;
            this.dFieldEdit2.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit2.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.EditWidth = 50;
            this.dFieldEdit2.FieldValue = "1";
            this.dFieldEdit2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.IsModified = false;
            this.dFieldEdit2.Location = new System.Drawing.Point(18, 376);
            this.dFieldEdit2.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit2.MaxValue = 5D;
            this.dFieldEdit2.MinValue = 1D;
            this.dFieldEdit2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit2.Name = "dFieldEdit2";
            this.dFieldEdit2.NoChangeInAuto = false;
            this.dFieldEdit2.Size = new System.Drawing.Size(332, 29);
            this.dFieldEdit2.StepValue = 0D;
            this.dFieldEdit2.TabIndex = 50;
            this.dFieldEdit2.Unit = "次";
            this.dFieldEdit2.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.UnitWidth = 50;
            this.dFieldEdit2.ValueType = KCSDK.ValueDataType.Int;
            // 
            // 內參設定
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 730);
            this.Controls.Add(this.panel2);
            this.Name = "內參設定";
            this.Text = "內參設定";
            this.Load += new System.EventHandler(this.內參設定_Load);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.panel2.ResumeLayout(false);
            this.tbInnerParam.ResumeLayout(false);
            this.tpApp.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_SPCSet)).EndInit();
            this.tpSys.ResumeLayout(false);
            this.tpSys.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.dRadioGroupBox1.ResumeLayout(false);
            this.dRadioGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tbInnerParam;
        private System.Windows.Forms.TabPage tpApp;
        private KCSDK.DFieldEdit dFieldEdit27;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAdd;
        private KCSDK.DDataGridView DGV_SPCSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
        private System.Windows.Forms.Button btnBrowse;
        private KCSDK.DEdit dEdit1;
        private System.Windows.Forms.Label label4;
        private KCSDK.DComboBox dComboBox2;
        private System.Windows.Forms.Label label3;
        private KCSDK.DFieldEdit dFieldEdit1;
        private System.Windows.Forms.TabPage tpSys;
        private KCSDK.DRadioGroupBox dRadioGroupBox1;
        private System.Windows.Forms.Label lblHelp;
        private KCSDK.DCheckBox dCheckBox1;
        private KCSDK.DFieldEdit dFieldEdit2;
        private KCSDK.DFieldEdit dFieldEdit3;
        private System.Windows.Forms.GroupBox groupBox2;
        private KCSDK.DFieldEdit dFieldEdit4;
        private KCSDK.DFieldEdit dFieldEdit5;
        private KCSDK.DCheckBox dCheckBox25;

    }
}