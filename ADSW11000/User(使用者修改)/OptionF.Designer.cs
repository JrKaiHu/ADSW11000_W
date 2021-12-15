namespace ADSW11000
{
    partial class OptionF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionF));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnSave2 = new System.Windows.Forms.Button();
            this.btnEdit2 = new System.Windows.Forms.Button();
            this.dFieldEdit2 = new KCSDK.DFieldEdit();
            this.OptionDS = new KCSDK.DataManagement(this.components);
            this.dCheckBox1 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox23 = new KCSDK.DCheckBox(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dEdit9 = new KCSDK.DEdit();
            this.dRadioGroupBox3 = new KCSDK.DRadioGroupBox(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dEdit7 = new KCSDK.DEdit();
            this.dRadioGroupBox1 = new KCSDK.DRadioGroupBox(this.components);
            this.dRadioGroupBox4 = new KCSDK.DRadioGroupBox(this.components);
            this.dRadioGroupBox5 = new KCSDK.DRadioGroupBox(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dEdit1 = new KCSDK.DEdit();
            this.dEdit2 = new KCSDK.DEdit();
            this.dRadioGroupBox6 = new KCSDK.DRadioGroupBox(this.components);
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dEdit3 = new KCSDK.DEdit();
            this.dEdit4 = new KCSDK.DEdit();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dFieldEdit1 = new KCSDK.DFieldEdit();
            this.dFieldEdit43 = new KCSDK.DFieldEdit();
            this.dCheckBox14 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox15 = new KCSDK.DCheckBox(this.components);
            this.dRadioGroupBox2 = new KCSDK.DRadioGroupBox(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.dEdit6 = new KCSDK.DEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.dEdit5 = new KCSDK.DEdit();
            this.dCheckBox2 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox3 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox4 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox5 = new KCSDK.DCheckBox(this.components);
            this.dCheckBox6 = new KCSDK.DCheckBox(this.components);
            this.pnlButton.SuspendLayout();
            this.dRadioGroupBox3.SuspendLayout();
            this.dRadioGroupBox5.SuspendLayout();
            this.dRadioGroupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.White;
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
            // pnlButton
            // 
            this.pnlButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlButton.Controls.Add(this.btnClose);
            this.pnlButton.Controls.Add(this.label1);
            this.pnlButton.Controls.Add(this.btnCancel2);
            this.pnlButton.Controls.Add(this.btnSave2);
            this.pnlButton.Controls.Add(this.btnEdit2);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButton.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.pnlButton.Location = new System.Drawing.Point(0, 0);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(1229, 58);
            this.pnlButton.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.ImageKey = "closebtn.png";
            this.btnClose.ImageList = this.imgList;
            this.btnClose.Location = new System.Drawing.Point(550, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 50);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "結束";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.ForeColor = System.Drawing.Color.Green;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 56);
            this.label1.TabIndex = 3;
            this.label1.Text = "通用設定";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label1_MouseUp);
            // 
            // btnCancel2
            // 
            this.btnCancel2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel2.ImageKey = "cancel.png";
            this.btnCancel2.ImageList = this.imgList;
            this.btnCancel2.Location = new System.Drawing.Point(444, 3);
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(100, 50);
            this.btnCancel2.TabIndex = 2;
            this.btnCancel2.Text = "取消";
            this.btnCancel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel2.UseVisualStyleBackColor = true;
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel2_Click);
            // 
            // btnSave2
            // 
            this.btnSave2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave2.ImageKey = "save.png";
            this.btnSave2.ImageList = this.imgList;
            this.btnSave2.Location = new System.Drawing.Point(338, 3);
            this.btnSave2.Name = "btnSave2";
            this.btnSave2.Size = new System.Drawing.Size(100, 50);
            this.btnSave2.TabIndex = 1;
            this.btnSave2.Text = "儲存";
            this.btnSave2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave2.UseVisualStyleBackColor = true;
            this.btnSave2.Click += new System.EventHandler(this.btnSave2_Click);
            // 
            // btnEdit2
            // 
            this.btnEdit2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEdit2.ImageKey = "edit.png";
            this.btnEdit2.ImageList = this.imgList;
            this.btnEdit2.Location = new System.Drawing.Point(232, 3);
            this.btnEdit2.Name = "btnEdit2";
            this.btnEdit2.Size = new System.Drawing.Size(100, 50);
            this.btnEdit2.TabIndex = 0;
            this.btnEdit2.Text = "編輯";
            this.btnEdit2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEdit2.UseVisualStyleBackColor = true;
            this.btnEdit2.Click += new System.EventHandler(this.btnEdit2_Click);
            // 
            // dFieldEdit2
            // 
            this.dFieldEdit2.AutoFocus = false;
            this.dFieldEdit2.Caption = "機台速率";
            this.dFieldEdit2.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit2.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.DataName = "機台速率";
            this.dFieldEdit2.DataSource = this.OptionDS;
            this.dFieldEdit2.DefaultValue = null;
            this.dFieldEdit2.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit2.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.EditWidth = 100;
            this.dFieldEdit2.FieldValue = "0";
            this.dFieldEdit2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.IsModified = false;
            this.dFieldEdit2.Location = new System.Drawing.Point(24, 81);
            this.dFieldEdit2.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit2.MaxValue = 100D;
            this.dFieldEdit2.MinValue = 10D;
            this.dFieldEdit2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit2.Name = "dFieldEdit2";
            this.dFieldEdit2.NoChangeInAuto = false;
            this.dFieldEdit2.Size = new System.Drawing.Size(309, 29);
            this.dFieldEdit2.StepValue = 0D;
            this.dFieldEdit2.TabIndex = 7;
            this.dFieldEdit2.Unit = "%";
            this.dFieldEdit2.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.UnitWidth = 40;
            this.dFieldEdit2.ValueType = KCSDK.ValueDataType.Int;
            // 
            // OptionDS
            // 
            this.OptionDS.ModifiedLog = true;
            this.OptionDS.ModifiedLogToDB = true;
            // 
            // dCheckBox1
            // 
            this.dCheckBox1.AutoSize = true;
            this.dCheckBox1.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox1.DataName = "bDryRun";
            this.dCheckBox1.DataSource = this.OptionDS;
            this.dCheckBox1.DefaultValue = false;
            this.dCheckBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox1.IsModified = false;
            this.dCheckBox1.Location = new System.Drawing.Point(952, 261);
            this.dCheckBox1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox1.Name = "dCheckBox1";
            this.dCheckBox1.NoChangeInAuto = true;
            this.dCheckBox1.Size = new System.Drawing.Size(92, 24);
            this.dCheckBox1.TabIndex = 6;
            this.dCheckBox1.Text = "空跑測試";
            this.dCheckBox1.UseVisualStyleBackColor = true;
            this.dCheckBox1.Visible = false;
            // 
            // dCheckBox23
            // 
            this.dCheckBox23.AutoSize = true;
            this.dCheckBox23.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox23.DataName = "bUseAlarmF";
            this.dCheckBox23.DataSource = this.OptionDS;
            this.dCheckBox23.DefaultValue = false;
            this.dCheckBox23.IsModified = false;
            this.dCheckBox23.Location = new System.Drawing.Point(952, 291);
            this.dCheckBox23.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox23.Name = "dCheckBox23";
            this.dCheckBox23.NoChangeInAuto = false;
            this.dCheckBox23.Size = new System.Drawing.Size(121, 24);
            this.dCheckBox23.TabIndex = 175;
            this.dCheckBox23.Text = "可視化Alarm";
            this.dCheckBox23.UseVisualStyleBackColor = true;
            this.dCheckBox23.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(128, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 20);
            this.label7.TabIndex = 60;
            this.label7.Text = "第";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(228, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 20);
            this.label8.TabIndex = 59;
            this.label8.Text = "道檢查";
            // 
            // dEdit9
            // 
            this.dEdit9.AutoFocus = false;
            this.dEdit9.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit9.DataName = "ikerfCheckline";
            this.dEdit9.DataSource = this.OptionDS;
            this.dEdit9.DefaultValue = null;
            this.dEdit9.IsModified = false;
            this.dEdit9.Location = new System.Drawing.Point(160, 91);
            this.dEdit9.MaxValue = 1000D;
            this.dEdit9.MinValue = 2D;
            this.dEdit9.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit9.Name = "dEdit9";
            this.dEdit9.NoChangeInAuto = false;
            this.dEdit9.Size = new System.Drawing.Size(61, 29);
            this.dEdit9.StepValue = 0D;
            this.dEdit9.TabIndex = 58;
            this.dEdit9.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dRadioGroupBox3
            // 
            this.dRadioGroupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox3.ColCount = 1;
            this.dRadioGroupBox3.Controls.Add(this.label5);
            this.dRadioGroupBox3.Controls.Add(this.label6);
            this.dRadioGroupBox3.Controls.Add(this.label7);
            this.dRadioGroupBox3.Controls.Add(this.dEdit7);
            this.dRadioGroupBox3.Controls.Add(this.label8);
            this.dRadioGroupBox3.Controls.Add(this.dEdit9);
            this.dRadioGroupBox3.DataName = "ikerfcheckType";
            this.dRadioGroupBox3.DataSource = this.OptionDS;
            this.dRadioGroupBox3.DefaultValue = 0;
            this.dRadioGroupBox3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox3.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dRadioGroupBox3.IsModified = false;
            this.dRadioGroupBox3.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox3.ItemHeight = 30;
            this.dRadioGroupBox3.ItemLeft = 10;
            this.dRadioGroupBox3.ItemTop = 30;
            this.dRadioGroupBox3.ItemWidth = 100;
            this.dRadioGroupBox3.Location = new System.Drawing.Point(619, 340);
            this.dRadioGroupBox3.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox3.Name = "dRadioGroupBox3";
            this.dRadioGroupBox3.NoChangeInAuto = false;
            this.dRadioGroupBox3.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox3.RadioItems")));
            this.dRadioGroupBox3.Size = new System.Drawing.Size(300, 201);
            this.dRadioGroupBox3.TabIndex = 57;
            this.dRadioGroupBox3.TabStop = false;
            this.dRadioGroupBox3.Text = "切割道檢測選擇(Check Kerf)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 161);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "頻率 : 每隔";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(162, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 20);
            this.label6.TabIndex = 5;
            this.label6.Text = "片";
            // 
            // dEdit7
            // 
            this.dEdit7.AutoFocus = false;
            this.dEdit7.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit7.DataName = "ikerfCheckfreq";
            this.dEdit7.DataSource = this.OptionDS;
            this.dEdit7.DefaultValue = null;
            this.dEdit7.IsModified = false;
            this.dEdit7.Location = new System.Drawing.Point(96, 157);
            this.dEdit7.MaxValue = 1000D;
            this.dEdit7.MinValue = 1D;
            this.dEdit7.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit7.Name = "dEdit7";
            this.dEdit7.NoChangeInAuto = false;
            this.dEdit7.Size = new System.Drawing.Size(61, 29);
            this.dEdit7.StepValue = 0D;
            this.dEdit7.TabIndex = 4;
            this.dEdit7.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dRadioGroupBox1
            // 
            this.dRadioGroupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox1.ColCount = 1;
            this.dRadioGroupBox1.DataName = "LoadMode";
            this.dRadioGroupBox1.DataSource = this.OptionDS;
            this.dRadioGroupBox1.DefaultValue = 0;
            this.dRadioGroupBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox1.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox1.IsModified = false;
            this.dRadioGroupBox1.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox1.ItemHeight = 30;
            this.dRadioGroupBox1.ItemLeft = 10;
            this.dRadioGroupBox1.ItemTop = 30;
            this.dRadioGroupBox1.ItemWidth = 100;
            this.dRadioGroupBox1.Location = new System.Drawing.Point(24, 141);
            this.dRadioGroupBox1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox1.Name = "dRadioGroupBox1";
            this.dRadioGroupBox1.NoChangeInAuto = true;
            this.dRadioGroupBox1.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox1.RadioItems")));
            this.dRadioGroupBox1.Size = new System.Drawing.Size(145, 97);
            this.dRadioGroupBox1.TabIndex = 161;
            this.dRadioGroupBox1.TabStop = false;
            this.dRadioGroupBox1.Text = "入料(Load)";
            // 
            // dRadioGroupBox4
            // 
            this.dRadioGroupBox4.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox4.ColCount = 1;
            this.dRadioGroupBox4.DataName = "UnLoadMode";
            this.dRadioGroupBox4.DataSource = this.OptionDS;
            this.dRadioGroupBox4.DefaultValue = 0;
            this.dRadioGroupBox4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox4.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox4.IsModified = false;
            this.dRadioGroupBox4.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox4.ItemHeight = 30;
            this.dRadioGroupBox4.ItemLeft = 10;
            this.dRadioGroupBox4.ItemTop = 30;
            this.dRadioGroupBox4.ItemWidth = 100;
            this.dRadioGroupBox4.Location = new System.Drawing.Point(24, 280);
            this.dRadioGroupBox4.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox4.Name = "dRadioGroupBox4";
            this.dRadioGroupBox4.NoChangeInAuto = true;
            this.dRadioGroupBox4.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox4.RadioItems")));
            this.dRadioGroupBox4.Size = new System.Drawing.Size(143, 97);
            this.dRadioGroupBox4.TabIndex = 196;
            this.dRadioGroupBox4.TabStop = false;
            this.dRadioGroupBox4.Text = "出料(UnLoad)";
            // 
            // dRadioGroupBox5
            // 
            this.dRadioGroupBox5.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox5.ColCount = 1;
            this.dRadioGroupBox5.Controls.Add(this.label4);
            this.dRadioGroupBox5.Controls.Add(this.label3);
            this.dRadioGroupBox5.Controls.Add(this.dEdit1);
            this.dRadioGroupBox5.Controls.Add(this.dEdit2);
            this.dRadioGroupBox5.DataName = "iSparkTest_freq";
            this.dRadioGroupBox5.DataSource = this.OptionDS;
            this.dRadioGroupBox5.DefaultValue = 0;
            this.dRadioGroupBox5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox5.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox5.IsModified = false;
            this.dRadioGroupBox5.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox5.ItemHeight = 30;
            this.dRadioGroupBox5.ItemLeft = 10;
            this.dRadioGroupBox5.ItemTop = 30;
            this.dRadioGroupBox5.ItemWidth = 100;
            this.dRadioGroupBox5.Location = new System.Drawing.Point(209, 141);
            this.dRadioGroupBox5.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox5.Name = "dRadioGroupBox5";
            this.dRadioGroupBox5.NoChangeInAuto = false;
            this.dRadioGroupBox5.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox5.RadioItems")));
            this.dRadioGroupBox5.Size = new System.Drawing.Size(372, 122);
            this.dRadioGroupBox5.TabIndex = 197;
            this.dRadioGroupBox5.TabStop = false;
            this.dRadioGroupBox5.Text = "接觸式測高(SparkTest)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(323, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 20);
            this.label4.TabIndex = 200;
            this.label4.Text = "片";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(323, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 20);
            this.label3.TabIndex = 199;
            this.label3.Text = "公尺";
            // 
            // dEdit1
            // 
            this.dEdit1.AutoFocus = false;
            this.dEdit1.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit1.DataName = "freq_STRunMP";
            this.dEdit1.DataSource = this.OptionDS;
            this.dEdit1.DefaultValue = null;
            this.dEdit1.IsModified = false;
            this.dEdit1.Location = new System.Drawing.Point(221, 88);
            this.dEdit1.MaxValue = 1000D;
            this.dEdit1.MinValue = 0D;
            this.dEdit1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit1.Name = "dEdit1";
            this.dEdit1.NoChangeInAuto = false;
            this.dEdit1.Size = new System.Drawing.Size(96, 29);
            this.dEdit1.StepValue = 0D;
            this.dEdit1.TabIndex = 198;
            this.dEdit1.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dEdit2
            // 
            this.dEdit2.AutoFocus = false;
            this.dEdit2.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit2.DataName = "freq_STRunMeter";
            this.dEdit2.DataSource = this.OptionDS;
            this.dEdit2.DefaultValue = null;
            this.dEdit2.IsModified = false;
            this.dEdit2.Location = new System.Drawing.Point(221, 55);
            this.dEdit2.MaxValue = 6000D;
            this.dEdit2.MinValue = 0D;
            this.dEdit2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit2.Name = "dEdit2";
            this.dEdit2.NoChangeInAuto = false;
            this.dEdit2.Size = new System.Drawing.Size(96, 29);
            this.dEdit2.StepValue = 0D;
            this.dEdit2.TabIndex = 199;
            this.dEdit2.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dRadioGroupBox6
            // 
            this.dRadioGroupBox6.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox6.ColCount = 1;
            this.dRadioGroupBox6.Controls.Add(this.label9);
            this.dRadioGroupBox6.Controls.Add(this.label10);
            this.dRadioGroupBox6.Controls.Add(this.dEdit3);
            this.dRadioGroupBox6.Controls.Add(this.dEdit4);
            this.dRadioGroupBox6.DataName = "iNonContactSparkTest_freq";
            this.dRadioGroupBox6.DataSource = this.OptionDS;
            this.dRadioGroupBox6.DefaultValue = 0;
            this.dRadioGroupBox6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox6.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox6.IsModified = false;
            this.dRadioGroupBox6.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox6.ItemHeight = 30;
            this.dRadioGroupBox6.ItemLeft = 10;
            this.dRadioGroupBox6.ItemTop = 30;
            this.dRadioGroupBox6.ItemWidth = 100;
            this.dRadioGroupBox6.Location = new System.Drawing.Point(209, 299);
            this.dRadioGroupBox6.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox6.Name = "dRadioGroupBox6";
            this.dRadioGroupBox6.NoChangeInAuto = false;
            this.dRadioGroupBox6.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox6.RadioItems")));
            this.dRadioGroupBox6.Size = new System.Drawing.Size(372, 122);
            this.dRadioGroupBox6.TabIndex = 199;
            this.dRadioGroupBox6.TabStop = false;
            this.dRadioGroupBox6.Text = "非接觸式測高(NonContactSparkTest)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(323, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 20);
            this.label9.TabIndex = 200;
            this.label9.Text = "公尺";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(323, 92);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(25, 20);
            this.label10.TabIndex = 199;
            this.label10.Text = "片";
            // 
            // dEdit3
            // 
            this.dEdit3.AutoFocus = false;
            this.dEdit3.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit3.DataName = "freq_NCTRunMP";
            this.dEdit3.DataSource = this.OptionDS;
            this.dEdit3.DefaultValue = null;
            this.dEdit3.IsModified = false;
            this.dEdit3.Location = new System.Drawing.Point(221, 89);
            this.dEdit3.MaxValue = 1000D;
            this.dEdit3.MinValue = 0D;
            this.dEdit3.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit3.Name = "dEdit3";
            this.dEdit3.NoChangeInAuto = false;
            this.dEdit3.Size = new System.Drawing.Size(96, 29);
            this.dEdit3.StepValue = 0D;
            this.dEdit3.TabIndex = 198;
            this.dEdit3.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dEdit4
            // 
            this.dEdit4.AutoFocus = false;
            this.dEdit4.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit4.DataName = "freq_NCTRunMeter";
            this.dEdit4.DataSource = this.OptionDS;
            this.dEdit4.DefaultValue = null;
            this.dEdit4.IsModified = false;
            this.dEdit4.Location = new System.Drawing.Point(221, 56);
            this.dEdit4.MaxValue = 6000D;
            this.dEdit4.MinValue = 0D;
            this.dEdit4.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit4.Name = "dEdit4";
            this.dEdit4.NoChangeInAuto = false;
            this.dEdit4.Size = new System.Drawing.Size(96, 29);
            this.dEdit4.StepValue = 0D;
            this.dEdit4.TabIndex = 199;
            this.dEdit4.ValueType = KCSDK.ValueDataType.Double;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dFieldEdit1);
            this.groupBox3.Controls.Add(this.dFieldEdit43);
            this.groupBox3.Controls.Add(this.dCheckBox14);
            this.groupBox3.Controls.Add(this.dCheckBox15);
            this.groupBox3.Location = new System.Drawing.Point(619, 141);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(299, 176);
            this.groupBox3.TabIndex = 200;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "靶點判別 Fail 時措施";
            // 
            // dFieldEdit1
            // 
            this.dFieldEdit1.AutoFocus = false;
            this.dFieldEdit1.Caption = "搜尋進幾量";
            this.dFieldEdit1.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit1.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.DataName = "iSearchMark_pitch";
            this.dFieldEdit1.DataSource = this.OptionDS;
            this.dFieldEdit1.DefaultValue = null;
            this.dFieldEdit1.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit1.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.EditWidth = 80;
            this.dFieldEdit1.FieldValue = "";
            this.dFieldEdit1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.IsModified = false;
            this.dFieldEdit1.Location = new System.Drawing.Point(16, 59);
            this.dFieldEdit1.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit1.MaxValue = 1000D;
            this.dFieldEdit1.MinValue = 0D;
            this.dFieldEdit1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit1.Name = "dFieldEdit1";
            this.dFieldEdit1.NoChangeInAuto = false;
            this.dFieldEdit1.Size = new System.Drawing.Size(237, 29);
            this.dFieldEdit1.StepValue = 0D;
            this.dFieldEdit1.TabIndex = 202;
            this.dFieldEdit1.Unit = "um";
            this.dFieldEdit1.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.UnitWidth = 40;
            this.dFieldEdit1.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dFieldEdit43
            // 
            this.dFieldEdit43.AutoFocus = false;
            this.dFieldEdit43.Caption = "周邊搜尋層數";
            this.dFieldEdit43.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit43.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit43.DataName = "iSearchMark_round";
            this.dFieldEdit43.DataSource = this.OptionDS;
            this.dFieldEdit43.DefaultValue = "1";
            this.dFieldEdit43.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit43.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit43.EditWidth = 80;
            this.dFieldEdit43.FieldValue = "";
            this.dFieldEdit43.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit43.IsModified = false;
            this.dFieldEdit43.Location = new System.Drawing.Point(16, 134);
            this.dFieldEdit43.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit43.MaxValue = 1D;
            this.dFieldEdit43.MinValue = 0D;
            this.dFieldEdit43.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit43.Name = "dFieldEdit43";
            this.dFieldEdit43.NoChangeInAuto = false;
            this.dFieldEdit43.Size = new System.Drawing.Size(237, 29);
            this.dFieldEdit43.StepValue = 0D;
            this.dFieldEdit43.TabIndex = 8;
            this.dFieldEdit43.Unit = "層";
            this.dFieldEdit43.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit43.UnitWidth = 40;
            this.dFieldEdit43.ValueType = KCSDK.ValueDataType.Int;
            this.dFieldEdit43.Visible = false;
            // 
            // dCheckBox14
            // 
            this.dCheckBox14.AutoSize = true;
            this.dCheckBox14.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox14.DataName = "bUseManualAlign";
            this.dCheckBox14.DataSource = this.OptionDS;
            this.dCheckBox14.DefaultValue = false;
            this.dCheckBox14.IsModified = false;
            this.dCheckBox14.Location = new System.Drawing.Point(16, 99);
            this.dCheckBox14.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox14.Name = "dCheckBox14";
            this.dCheckBox14.NoChangeInAuto = false;
            this.dCheckBox14.Size = new System.Drawing.Size(220, 24);
            this.dCheckBox14.TabIndex = 3;
            this.dCheckBox14.Text = "找不到靶點時手動定義靶點";
            this.dCheckBox14.UseVisualStyleBackColor = true;
            // 
            // dCheckBox15
            // 
            this.dCheckBox15.AutoSize = true;
            this.dCheckBox15.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox15.DataName = "bSearchNearMark";
            this.dCheckBox15.DataSource = this.OptionDS;
            this.dCheckBox15.DefaultValue = false;
            this.dCheckBox15.IsModified = false;
            this.dCheckBox15.Location = new System.Drawing.Point(16, 24);
            this.dCheckBox15.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox15.Name = "dCheckBox15";
            this.dCheckBox15.NoChangeInAuto = false;
            this.dCheckBox15.Size = new System.Drawing.Size(220, 24);
            this.dCheckBox15.TabIndex = 3;
            this.dCheckBox15.Text = "找不到靶點時進行周邊搜尋";
            this.dCheckBox15.UseVisualStyleBackColor = true;
            // 
            // dRadioGroupBox2
            // 
            this.dRadioGroupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.dRadioGroupBox2.ColCount = 1;
            this.dRadioGroupBox2.DataName = "iUseSpindleType";
            this.dRadioGroupBox2.DataSource = this.OptionDS;
            this.dRadioGroupBox2.DefaultValue = 0;
            this.dRadioGroupBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dRadioGroupBox2.ForeColor = System.Drawing.Color.Black;
            this.dRadioGroupBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dRadioGroupBox2.IsModified = false;
            this.dRadioGroupBox2.ItemFont = new System.Drawing.Font("微軟正黑體", 12F);
            this.dRadioGroupBox2.ItemHeight = 30;
            this.dRadioGroupBox2.ItemLeft = 10;
            this.dRadioGroupBox2.ItemTop = 30;
            this.dRadioGroupBox2.ItemWidth = 100;
            this.dRadioGroupBox2.Location = new System.Drawing.Point(24, 419);
            this.dRadioGroupBox2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dRadioGroupBox2.Name = "dRadioGroupBox2";
            this.dRadioGroupBox2.NoChangeInAuto = true;
            this.dRadioGroupBox2.RadioItems = ((System.Collections.Generic.List<string>)(resources.GetObject("dRadioGroupBox2.RadioItems")));
            this.dRadioGroupBox2.Size = new System.Drawing.Size(160, 122);
            this.dRadioGroupBox2.TabIndex = 201;
            this.dRadioGroupBox2.TabStop = false;
            this.dRadioGroupBox2.Text = "切割刀(Spindle)";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(455, 472);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(25, 20);
            this.label11.TabIndex = 211;
            this.label11.Text = "片";
            this.label11.Visible = false;
            // 
            // dEdit6
            // 
            this.dEdit6.AutoFocus = false;
            this.dEdit6.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit6.DataName = "SimuProcessMP";
            this.dEdit6.DataSource = this.OptionDS;
            this.dEdit6.DefaultValue = "1";
            this.dEdit6.IsModified = false;
            this.dEdit6.Location = new System.Drawing.Point(414, 469);
            this.dEdit6.MaxValue = 5D;
            this.dEdit6.MinValue = 1D;
            this.dEdit6.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit6.Name = "dEdit6";
            this.dEdit6.NoChangeInAuto = false;
            this.dEdit6.Size = new System.Drawing.Size(35, 29);
            this.dEdit6.StepValue = 0D;
            this.dEdit6.TabIndex = 210;
            this.dEdit6.ValueType = KCSDK.ValueDataType.Int;
            this.dEdit6.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(383, 472);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 20);
            this.label2.TabIndex = 209;
            this.label2.Text = "次";
            this.label2.Visible = false;
            // 
            // dEdit5
            // 
            this.dEdit5.AutoFocus = false;
            this.dEdit5.BackColor = System.Drawing.SystemColors.Window;
            this.dEdit5.DataName = "SimuProcessCycle";
            this.dEdit5.DataSource = this.OptionDS;
            this.dEdit5.DefaultValue = "1";
            this.dEdit5.IsModified = false;
            this.dEdit5.Location = new System.Drawing.Point(343, 469);
            this.dEdit5.MaxValue = 5D;
            this.dEdit5.MinValue = 1D;
            this.dEdit5.ModifiedColor = System.Drawing.Color.Aqua;
            this.dEdit5.Name = "dEdit5";
            this.dEdit5.NoChangeInAuto = false;
            this.dEdit5.Size = new System.Drawing.Size(35, 29);
            this.dEdit5.StepValue = 0D;
            this.dEdit5.TabIndex = 208;
            this.dEdit5.ValueType = KCSDK.ValueDataType.Int;
            this.dEdit5.Visible = false;
            // 
            // dCheckBox2
            // 
            this.dCheckBox2.AutoSize = true;
            this.dCheckBox2.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox2.DataName = "bSimuProcess";
            this.dCheckBox2.DataSource = this.OptionDS;
            this.dCheckBox2.DefaultValue = false;
            this.dCheckBox2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox2.IsModified = false;
            this.dCheckBox2.Location = new System.Drawing.Point(219, 471);
            this.dCheckBox2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox2.Name = "dCheckBox2";
            this.dCheckBox2.NoChangeInAuto = false;
            this.dCheckBox2.Size = new System.Drawing.Size(124, 24);
            this.dCheckBox2.TabIndex = 207;
            this.dCheckBox2.Text = "虛擬切割每片";
            this.dCheckBox2.UseVisualStyleBackColor = true;
            this.dCheckBox2.Visible = false;
            // 
            // dCheckBox3
            // 
            this.dCheckBox3.AutoSize = true;
            this.dCheckBox3.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox3.DataName = "bSeeAndCut";
            this.dCheckBox3.DataSource = this.OptionDS;
            this.dCheckBox3.DefaultValue = false;
            this.dCheckBox3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox3.IsModified = false;
            this.dCheckBox3.Location = new System.Drawing.Point(952, 171);
            this.dCheckBox3.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox3.Name = "dCheckBox3";
            this.dCheckBox3.NoChangeInAuto = true;
            this.dCheckBox3.Size = new System.Drawing.Size(92, 24);
            this.dCheckBox3.TabIndex = 212;
            this.dCheckBox3.Text = "即看即切";
            this.dCheckBox3.UseVisualStyleBackColor = true;
            // 
            // dCheckBox4
            // 
            this.dCheckBox4.AutoSize = true;
            this.dCheckBox4.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox4.DataName = "bSinkCleanWater";
            this.dCheckBox4.DataSource = this.OptionDS;
            this.dCheckBox4.DefaultValue = false;
            this.dCheckBox4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox4.IsModified = false;
            this.dCheckBox4.Location = new System.Drawing.Point(952, 141);
            this.dCheckBox4.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox4.Name = "dCheckBox4";
            this.dCheckBox4.NoChangeInAuto = true;
            this.dCheckBox4.Size = new System.Drawing.Size(156, 24);
            this.dCheckBox4.TabIndex = 213;
            this.dCheckBox4.Text = "水槽清潔噴水啟動";
            this.dCheckBox4.UseVisualStyleBackColor = true;
            // 
            // dCheckBox5
            // 
            this.dCheckBox5.AutoSize = true;
            this.dCheckBox5.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox5.DataName = "bFastAutoFocus";
            this.dCheckBox5.DataSource = this.OptionDS;
            this.dCheckBox5.DefaultValue = false;
            this.dCheckBox5.IsModified = false;
            this.dCheckBox5.Location = new System.Drawing.Point(952, 201);
            this.dCheckBox5.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox5.Name = "dCheckBox5";
            this.dCheckBox5.NoChangeInAuto = false;
            this.dCheckBox5.Size = new System.Drawing.Size(124, 24);
            this.dCheckBox5.TabIndex = 214;
            this.dCheckBox5.Text = "快速自動對焦";
            this.dCheckBox5.UseVisualStyleBackColor = true;
            // 
            // dCheckBox6
            // 
            this.dCheckBox6.AutoSize = true;
            this.dCheckBox6.BackColor = System.Drawing.SystemColors.Control;
            this.dCheckBox6.DataName = "bLiveCut";
            this.dCheckBox6.DataSource = this.OptionDS;
            this.dCheckBox6.DefaultValue = false;
            this.dCheckBox6.IsModified = false;
            this.dCheckBox6.Location = new System.Drawing.Point(952, 231);
            this.dCheckBox6.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox6.Name = "dCheckBox6";
            this.dCheckBox6.NoChangeInAuto = false;
            this.dCheckBox6.Size = new System.Drawing.Size(124, 24);
            this.dCheckBox6.TabIndex = 215;
            this.dCheckBox6.Text = "切割畫面旋轉";
            this.dCheckBox6.UseVisualStyleBackColor = true;
            // 
            // OptionF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1229, 730);
            this.Controls.Add(this.dCheckBox6);
            this.Controls.Add(this.dCheckBox5);
            this.Controls.Add(this.dCheckBox4);
            this.Controls.Add(this.dCheckBox3);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.dEdit6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dEdit5);
            this.Controls.Add(this.dCheckBox2);
            this.Controls.Add(this.dRadioGroupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.dRadioGroupBox6);
            this.Controls.Add(this.dRadioGroupBox5);
            this.Controls.Add(this.dRadioGroupBox4);
            this.Controls.Add(this.dRadioGroupBox3);
            this.Controls.Add(this.dCheckBox23);
            this.Controls.Add(this.dRadioGroupBox1);
            this.Controls.Add(this.pnlButton);
            this.Controls.Add(this.dCheckBox1);
            this.Controls.Add(this.dFieldEdit2);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "OptionF";
            this.Text = "OptionF";
            this.pnlButton.ResumeLayout(false);
            this.dRadioGroupBox3.ResumeLayout(false);
            this.dRadioGroupBox3.PerformLayout();
            this.dRadioGroupBox5.ResumeLayout(false);
            this.dRadioGroupBox5.PerformLayout();
            this.dRadioGroupBox6.ResumeLayout(false);
            this.dRadioGroupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnCancel2;
        private System.Windows.Forms.Button btnSave2;
        private System.Windows.Forms.Button btnEdit2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        public KCSDK.DataManagement OptionDS;
        private KCSDK.DCheckBox dCheckBox1;
        private KCSDK.DFieldEdit dFieldEdit2;
        private KCSDK.DCheckBox dCheckBox23;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private KCSDK.DEdit dEdit9;
        private KCSDK.DRadioGroupBox dRadioGroupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private KCSDK.DEdit dEdit7;
        private KCSDK.DRadioGroupBox dRadioGroupBox1;
        private KCSDK.DRadioGroupBox dRadioGroupBox4;
        private KCSDK.DRadioGroupBox dRadioGroupBox5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private KCSDK.DEdit dEdit1;
        private KCSDK.DEdit dEdit2;
        private KCSDK.DRadioGroupBox dRadioGroupBox6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private KCSDK.DEdit dEdit3;
        private KCSDK.DEdit dEdit4;
        private System.Windows.Forms.GroupBox groupBox3;
        private KCSDK.DFieldEdit dFieldEdit43;
        private KCSDK.DCheckBox dCheckBox14;
        private KCSDK.DCheckBox dCheckBox15;
        private KCSDK.DRadioGroupBox dRadioGroupBox2;
        private KCSDK.DFieldEdit dFieldEdit1;
        private System.Windows.Forms.Label label11;
        private KCSDK.DEdit dEdit6;
        private System.Windows.Forms.Label label2;
        private KCSDK.DEdit dEdit5;
        private KCSDK.DCheckBox dCheckBox2;
        private KCSDK.DCheckBox dCheckBox3;
        private KCSDK.DCheckBox dCheckBox4;
        private KCSDK.DCheckBox dCheckBox5;
        private KCSDK.DCheckBox dCheckBox6;
    }
}