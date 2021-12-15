namespace ADSW11000
{
    partial class BladeZ2Data
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BladeZ2Data));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dFieldEdit5 = new KCSDK.DFieldEdit();
            this.dFieldEdit14 = new KCSDK.DFieldEdit();
            this.dFieldEdit13 = new KCSDK.DFieldEdit();
            this.dFieldEdit10 = new KCSDK.DFieldEdit();
            this.dFieldEdit8 = new KCSDK.DFieldEdit();
            this.dFieldEdit7 = new KCSDK.DFieldEdit();
            this.dFieldEdit6 = new KCSDK.DFieldEdit();
            this.dFieldEdit4 = new KCSDK.DFieldEdit();
            this.dFieldEdit3 = new KCSDK.DFieldEdit();
            this.dFieldEdit2 = new KCSDK.DFieldEdit();
            this.dFieldEdit1 = new KCSDK.DFieldEdit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).BeginInit();
            this.PackageContainer.Panel1.SuspendLayout();
            this.PackageContainer.Panel2.SuspendLayout();
            this.PackageContainer.SuspendLayout();
            this.pnlButton.SuspendLayout();
            this.pnlControl.SuspendLayout();
            this.panel2.SuspendLayout();
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
            // PackageContainer
            // 
            this.PackageContainer.Size = new System.Drawing.Size(1042, 696);
            // 
            // pnlButton
            // 
            this.pnlButton.Size = new System.Drawing.Size(761, 58);
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.label2);
            this.pnlControl.Controls.Add(this.label1);
            this.pnlControl.Controls.Add(this.dFieldEdit5);
            this.pnlControl.Controls.Add(this.dFieldEdit14);
            this.pnlControl.Controls.Add(this.dFieldEdit13);
            this.pnlControl.Controls.Add(this.dFieldEdit10);
            this.pnlControl.Controls.Add(this.dFieldEdit8);
            this.pnlControl.Controls.Add(this.dFieldEdit7);
            this.pnlControl.Controls.Add(this.dFieldEdit6);
            this.pnlControl.Controls.Add(this.dFieldEdit4);
            this.pnlControl.Controls.Add(this.dFieldEdit3);
            this.pnlControl.Controls.Add(this.dFieldEdit2);
            this.pnlControl.Controls.Add(this.dFieldEdit1);
            this.pnlControl.Size = new System.Drawing.Size(761, 636);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.ForeColor = System.Drawing.Color.Purple;
            this.label2.Location = new System.Drawing.Point(319, 334);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 34);
            this.label2.TabIndex = 52;
            this.label2.Text = "磨耗值限制 = 實際刀露量 - ( 膠墊高度 - 高度偏移 ) -\r\n                           料片厚度 - 料片與法蘭最近距離";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.ForeColor = System.Drawing.Color.Purple;
            this.label1.Location = new System.Drawing.Point(319, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 17);
            this.label1.TabIndex = 51;
            this.label1.Text = "實際刀露量 = (刀外徑 - 法蘭盤外徑) / 2";
            // 
            // dFieldEdit5
            // 
            this.dFieldEdit5.AutoFocus = false;
            this.dFieldEdit5.Caption = "磨耗值限制";
            this.dFieldEdit5.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit5.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit5.DataName = "dWearlimitZ2";
            this.dFieldEdit5.DataSource = this.PreloadPackageDS;
            this.dFieldEdit5.DefaultValue = null;
            this.dFieldEdit5.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit5.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit5.EditWidth = 100;
            this.dFieldEdit5.FieldValue = "";
            this.dFieldEdit5.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit5.Location = new System.Drawing.Point(16, 334);
            this.dFieldEdit5.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit5.MaxValue = 9999999;
            this.dFieldEdit5.MinValue = -9999999;
            this.dFieldEdit5.Name = "dFieldEdit5";
            this.dFieldEdit5.NoChangeInAuto = false;
            this.dFieldEdit5.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit5.StepValue = 0D;
            this.dFieldEdit5.TabIndex = 50;
            this.dFieldEdit5.Unit = "mm";
            this.dFieldEdit5.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit5.UnitWidth = 50;
            this.dFieldEdit5.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit14
            // 
            this.dFieldEdit14.AutoFocus = false;
            this.dFieldEdit14.Caption = "里程極限";
            this.dFieldEdit14.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit14.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit14.DataName = "dLimitZ2";
            this.dFieldEdit14.DataSource = this.PreloadPackageDS;
            this.dFieldEdit14.DefaultValue = null;
            this.dFieldEdit14.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit14.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit14.EditWidth = 100;
            this.dFieldEdit14.FieldValue = "";
            this.dFieldEdit14.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit14.Location = new System.Drawing.Point(16, 218);
            this.dFieldEdit14.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit14.MaxValue = 9999999;
            this.dFieldEdit14.MinValue = -9999999;
            this.dFieldEdit14.Name = "dFieldEdit14";
            this.dFieldEdit14.NoChangeInAuto = false;
            this.dFieldEdit14.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit14.StepValue = 0D;
            this.dFieldEdit14.TabIndex = 49;
            this.dFieldEdit14.Unit = "M";
            this.dFieldEdit14.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit14.UnitWidth = 50;
            this.dFieldEdit14.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit13
            // 
            this.dFieldEdit13.AutoFocus = false;
            this.dFieldEdit13.Caption = "法蘭盤外徑";
            this.dFieldEdit13.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit13.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit13.DataName = "dFlangeZ2";
            this.dFieldEdit13.DataSource = this.PreloadPackageDS;
            this.dFieldEdit13.DefaultValue = null;
            this.dFieldEdit13.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit13.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit13.EditWidth = 100;
            this.dFieldEdit13.FieldValue = "";
            this.dFieldEdit13.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit13.Location = new System.Drawing.Point(16, 190);
            this.dFieldEdit13.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit13.MaxValue = 9999999;
            this.dFieldEdit13.MinValue = -9999999;
            this.dFieldEdit13.Name = "dFieldEdit13";
            this.dFieldEdit13.NoChangeInAuto = false;
            this.dFieldEdit13.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit13.StepValue = 0D;
            this.dFieldEdit13.TabIndex = 48;
            this.dFieldEdit13.Unit = "mm";
            this.dFieldEdit13.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit13.UnitWidth = 50;
            this.dFieldEdit13.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit10
            // 
            this.dFieldEdit10.AutoFocus = false;
            this.dFieldEdit10.Caption = "實際切割距離";
            this.dFieldEdit10.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit10.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit10.DataName = "dRealMotorDistanceZ2";
            this.dFieldEdit10.DataSource = this.PreloadPackageDS;
            this.dFieldEdit10.DefaultValue = null;
            this.dFieldEdit10.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit10.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit10.EditWidth = 100;
            this.dFieldEdit10.FieldValue = "";
            this.dFieldEdit10.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit10.Location = new System.Drawing.Point(16, 247);
            this.dFieldEdit10.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit10.MaxValue = 9999999;
            this.dFieldEdit10.MinValue = -9999999;
            this.dFieldEdit10.Name = "dFieldEdit10";
            this.dFieldEdit10.NoChangeInAuto = false;
            this.dFieldEdit10.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit10.StepValue = 0D;
            this.dFieldEdit10.TabIndex = 47;
            this.dFieldEdit10.Unit = "M";
            this.dFieldEdit10.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit10.UnitWidth = 50;
            this.dFieldEdit10.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit8
            // 
            this.dFieldEdit8.AutoFocus = false;
            this.dFieldEdit8.Caption = "非接觸式量測高度";
            this.dFieldEdit8.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit8.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit8.DataName = "iSimMeasureZPosZ2";
            this.dFieldEdit8.DataSource = this.PreloadPackageDS;
            this.dFieldEdit8.DefaultValue = null;
            this.dFieldEdit8.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit8.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit8.EditWidth = 100;
            this.dFieldEdit8.FieldValue = "";
            this.dFieldEdit8.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit8.Location = new System.Drawing.Point(16, 305);
            this.dFieldEdit8.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit8.MaxValue = 9999999;
            this.dFieldEdit8.MinValue = -9999999;
            this.dFieldEdit8.Name = "dFieldEdit8";
            this.dFieldEdit8.NoChangeInAuto = false;
            this.dFieldEdit8.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit8.StepValue = 0D;
            this.dFieldEdit8.TabIndex = 46;
            this.dFieldEdit8.Unit = "um";
            this.dFieldEdit8.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit8.UnitWidth = 50;
            this.dFieldEdit8.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dFieldEdit7
            // 
            this.dFieldEdit7.AutoFocus = false;
            this.dFieldEdit7.Caption = "接觸式量測高度";
            this.dFieldEdit7.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit7.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit7.DataName = "iRealMeasureZPosZ2";
            this.dFieldEdit7.DataSource = this.PreloadPackageDS;
            this.dFieldEdit7.DefaultValue = null;
            this.dFieldEdit7.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit7.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit7.EditWidth = 100;
            this.dFieldEdit7.FieldValue = "";
            this.dFieldEdit7.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit7.Location = new System.Drawing.Point(16, 276);
            this.dFieldEdit7.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit7.MaxValue = 9999999;
            this.dFieldEdit7.MinValue = -9999999;
            this.dFieldEdit7.Name = "dFieldEdit7";
            this.dFieldEdit7.NoChangeInAuto = false;
            this.dFieldEdit7.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit7.StepValue = 0D;
            this.dFieldEdit7.TabIndex = 45;
            this.dFieldEdit7.Unit = "um";
            this.dFieldEdit7.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit7.UnitWidth = 50;
            this.dFieldEdit7.ValueType = KCSDK.ValueDataType.Int;
            // 
            // dFieldEdit6
            // 
            this.dFieldEdit6.AutoFocus = false;
            this.dFieldEdit6.Caption = "實際刀露量";
            this.dFieldEdit6.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit6.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit6.DataName = "dRealKnifeRemainZ2";
            this.dFieldEdit6.DataSource = this.PreloadPackageDS;
            this.dFieldEdit6.DefaultValue = null;
            this.dFieldEdit6.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit6.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit6.EditWidth = 100;
            this.dFieldEdit6.FieldValue = "";
            this.dFieldEdit6.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit6.Location = new System.Drawing.Point(16, 132);
            this.dFieldEdit6.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit6.MaxValue = 9999999;
            this.dFieldEdit6.MinValue = -9999999;
            this.dFieldEdit6.Name = "dFieldEdit6";
            this.dFieldEdit6.NoChangeInAuto = false;
            this.dFieldEdit6.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit6.StepValue = 0D;
            this.dFieldEdit6.TabIndex = 44;
            this.dFieldEdit6.Unit = "mm";
            this.dFieldEdit6.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit6.UnitWidth = 50;
            this.dFieldEdit6.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit4
            // 
            this.dFieldEdit4.AutoFocus = false;
            this.dFieldEdit4.Caption = "刀外徑";
            this.dFieldEdit4.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit4.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit4.DataName = "dKnifeDiameterZ2";
            this.dFieldEdit4.DataSource = this.PreloadPackageDS;
            this.dFieldEdit4.DefaultValue = null;
            this.dFieldEdit4.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit4.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit4.EditWidth = 100;
            this.dFieldEdit4.FieldValue = "";
            this.dFieldEdit4.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit4.Location = new System.Drawing.Point(16, 103);
            this.dFieldEdit4.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit4.MaxValue = 80;
            this.dFieldEdit4.MinValue = 70;
            this.dFieldEdit4.Name = "dFieldEdit4";
            this.dFieldEdit4.NoChangeInAuto = false;
            this.dFieldEdit4.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit4.StepValue = 0D;
            this.dFieldEdit4.TabIndex = 43;
            this.dFieldEdit4.Unit = "mm";
            this.dFieldEdit4.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit4.UnitWidth = 50;
            this.dFieldEdit4.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit3
            // 
            this.dFieldEdit3.AutoFocus = false;
            this.dFieldEdit3.Caption = "刀寬";
            this.dFieldEdit3.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit3.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.DataName = "dWidthZ2";
            this.dFieldEdit3.DataSource = this.PreloadPackageDS;
            this.dFieldEdit3.DefaultValue = null;
            this.dFieldEdit3.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit3.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.EditWidth = 100;
            this.dFieldEdit3.FieldValue = "";
            this.dFieldEdit3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.Location = new System.Drawing.Point(16, 74);
            this.dFieldEdit3.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit3.MaxValue = 1000;
            this.dFieldEdit3.MinValue = 0;
            this.dFieldEdit3.Name = "dFieldEdit3";
            this.dFieldEdit3.NoChangeInAuto = false;
            this.dFieldEdit3.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit3.StepValue = 0D;
            this.dFieldEdit3.TabIndex = 42;
            this.dFieldEdit3.Unit = "mm";
            this.dFieldEdit3.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.UnitWidth = 50;
            this.dFieldEdit3.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit2
            // 
            this.dFieldEdit2.AutoFocus = false;
            this.dFieldEdit2.Caption = "刀片型號";
            this.dFieldEdit2.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit2.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.DataName = "sTypeZ2";
            this.dFieldEdit2.DataSource = this.PreloadPackageDS;
            this.dFieldEdit2.DefaultValue = null;
            this.dFieldEdit2.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit2.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.EditWidth = 100;
            this.dFieldEdit2.FieldValue = "";
            this.dFieldEdit2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.Location = new System.Drawing.Point(16, 45);
            this.dFieldEdit2.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit2.MaxValue = 9999999;
            this.dFieldEdit2.MinValue = -9999999;
            this.dFieldEdit2.Name = "dFieldEdit2";
            this.dFieldEdit2.NoChangeInAuto = false;
            this.dFieldEdit2.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit2.StepValue = 0D;
            this.dFieldEdit2.TabIndex = 41;
            this.dFieldEdit2.Unit = "";
            this.dFieldEdit2.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.UnitWidth = 50;
            this.dFieldEdit2.ValueType = KCSDK.ValueDataType.String;
            // 
            // dFieldEdit1
            // 
            this.dFieldEdit1.AutoFocus = false;
            this.dFieldEdit1.Caption = "刀片序號";
            this.dFieldEdit1.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit1.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.DataName = "sToolNameZ2";
            this.dFieldEdit1.DataSource = this.PreloadPackageDS;
            this.dFieldEdit1.DefaultValue = null;
            this.dFieldEdit1.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit1.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.EditWidth = 100;
            this.dFieldEdit1.FieldValue = "";
            this.dFieldEdit1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.Location = new System.Drawing.Point(16, 16);
            this.dFieldEdit1.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit1.MaxValue = 9999999;
            this.dFieldEdit1.MinValue = -9999999;
            this.dFieldEdit1.Name = "dFieldEdit1";
            this.dFieldEdit1.NoChangeInAuto = false;
            this.dFieldEdit1.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit1.StepValue = 0D;
            this.dFieldEdit1.TabIndex = 40;
            this.dFieldEdit1.Unit = "";
            this.dFieldEdit1.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.UnitWidth = 50;
            this.dFieldEdit1.ValueType = KCSDK.ValueDataType.String;
            // 
            // BladeZ2Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 696);
            this.Name = "BladeZ2Data";
            this.Text = "BladeZ2Data";
            this.PackageContainer.Panel1.ResumeLayout(false);
            this.PackageContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).EndInit();
            this.PackageContainer.ResumeLayout(false);
            this.pnlButton.ResumeLayout(false);
            this.pnlControl.ResumeLayout(false);
            this.pnlControl.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private KCSDK.DFieldEdit dFieldEdit5;
        private KCSDK.DFieldEdit dFieldEdit14;
        private KCSDK.DFieldEdit dFieldEdit13;
        private KCSDK.DFieldEdit dFieldEdit10;
        private KCSDK.DFieldEdit dFieldEdit8;
        private KCSDK.DFieldEdit dFieldEdit7;
        private KCSDK.DFieldEdit dFieldEdit6;
        private KCSDK.DFieldEdit dFieldEdit4;
        private KCSDK.DFieldEdit dFieldEdit3;
        private KCSDK.DFieldEdit dFieldEdit2;
        private KCSDK.DFieldEdit dFieldEdit1;
    }
}