namespace ADSW11000
{
    partial class CutLineGenerator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CutLineGenerator));
            this.btnCancel = new System.Windows.Forms.Button();
            this.rdoZ1 = new System.Windows.Forms.RadioButton();
            this.rdoZ2 = new System.Windows.Forms.RadioButton();
            this.rdoZ1Z2 = new System.Windows.Forms.RadioButton();
            this.cboChannel = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoTwoSideCut = new System.Windows.Forms.RadioButton();
            this.rdoOneSideCut = new System.Windows.Forms.RadioButton();
            this.rdoDefault = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCutOffset = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCutSpeed = new System.Windows.Forms.TextBox();
            this.btnAutoGen = new System.Windows.Forms.Button();
            this.txtCH1Num = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCH2Num = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.grpEnterSpeed = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEnterSpeed = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpEnterSpeed.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(441, 426);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(118, 48);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // rdoZ1
            // 
            this.rdoZ1.AutoSize = true;
            this.rdoZ1.Location = new System.Drawing.Point(16, 28);
            this.rdoZ1.Name = "rdoZ1";
            this.rdoZ1.Size = new System.Drawing.Size(46, 24);
            this.rdoZ1.TabIndex = 1;
            this.rdoZ1.Text = "Z1";
            this.rdoZ1.UseVisualStyleBackColor = true;
            // 
            // rdoZ2
            // 
            this.rdoZ2.AutoSize = true;
            this.rdoZ2.Location = new System.Drawing.Point(82, 28);
            this.rdoZ2.Name = "rdoZ2";
            this.rdoZ2.Size = new System.Drawing.Size(46, 24);
            this.rdoZ2.TabIndex = 2;
            this.rdoZ2.Text = "Z2";
            this.rdoZ2.UseVisualStyleBackColor = true;
            // 
            // rdoZ1Z2
            // 
            this.rdoZ1Z2.AutoSize = true;
            this.rdoZ1Z2.Checked = true;
            this.rdoZ1Z2.Location = new System.Drawing.Point(148, 28);
            this.rdoZ1Z2.Name = "rdoZ1Z2";
            this.rdoZ1Z2.Size = new System.Drawing.Size(72, 24);
            this.rdoZ1Z2.TabIndex = 3;
            this.rdoZ1Z2.TabStop = true;
            this.rdoZ1Z2.Text = "Z1/Z2";
            this.rdoZ1Z2.UseVisualStyleBackColor = true;
            // 
            // cboChannel
            // 
            this.cboChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboChannel.FormattingEnabled = true;
            this.cboChannel.Items.AddRange(new object[] {
            "CH1",
            "CH2"});
            this.cboChannel.Location = new System.Drawing.Point(16, 28);
            this.cboChannel.Name = "cboChannel";
            this.cboChannel.Size = new System.Drawing.Size(85, 28);
            this.cboChannel.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox1.Controls.Add(this.rdoZ1);
            this.groupBox1.Controls.Add(this.rdoZ2);
            this.groupBox1.Controls.Add(this.rdoZ1Z2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(228, 69);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "刀輪選擇(Spindle)";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox2.Controls.Add(this.cboChannel);
            this.groupBox2.Location = new System.Drawing.Point(261, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(130, 69);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CH First";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox3.Controls.Add(this.cboType);
            this.groupBox3.Location = new System.Drawing.Point(411, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(160, 69);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "模式選擇(Type)";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Type1",
            "Type2"});
            this.cboType.Location = new System.Drawing.Point(16, 28);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(85, 28);
            this.cboType.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox4.Controls.Add(this.rdoTwoSideCut);
            this.groupBox4.Controls.Add(this.rdoOneSideCut);
            this.groupBox4.Controls.Add(this.rdoDefault);
            this.groupBox4.Location = new System.Drawing.Point(12, 88);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(228, 145);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "刀序排列(Seq)";
            // 
            // rdoTwoSideCut
            // 
            this.rdoTwoSideCut.AutoSize = true;
            this.rdoTwoSideCut.Checked = true;
            this.rdoTwoSideCut.Location = new System.Drawing.Point(16, 64);
            this.rdoTwoSideCut.Name = "rdoTwoSideCut";
            this.rdoTwoSideCut.Size = new System.Drawing.Size(184, 24);
            this.rdoTwoSideCut.TabIndex = 2;
            this.rdoTwoSideCut.TabStop = true;
            this.rdoTwoSideCut.Text = "雙邊料先切(Two Side)";
            this.rdoTwoSideCut.UseVisualStyleBackColor = true;
            // 
            // rdoOneSideCut
            // 
            this.rdoOneSideCut.AutoSize = true;
            this.rdoOneSideCut.Location = new System.Drawing.Point(16, 100);
            this.rdoOneSideCut.Name = "rdoOneSideCut";
            this.rdoOneSideCut.Size = new System.Drawing.Size(199, 24);
            this.rdoOneSideCut.TabIndex = 1;
            this.rdoOneSideCut.Text = "單邊料先切(Single Side)";
            this.rdoOneSideCut.UseVisualStyleBackColor = true;
            this.rdoOneSideCut.Visible = false;
            // 
            // rdoDefault
            // 
            this.rdoDefault.AutoSize = true;
            this.rdoDefault.Location = new System.Drawing.Point(16, 28);
            this.rdoDefault.Name = "rdoDefault";
            this.rdoDefault.Size = new System.Drawing.Size(124, 24);
            this.rdoDefault.TabIndex = 0;
            this.rdoDefault.Text = "預設(Default)";
            this.rdoDefault.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtCutOffset);
            this.groupBox5.Location = new System.Drawing.Point(261, 88);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(130, 69);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "提刀高度(H)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "um";
            // 
            // txtCutOffset
            // 
            this.txtCutOffset.Location = new System.Drawing.Point(16, 28);
            this.txtCutOffset.Name = "txtCutOffset";
            this.txtCutOffset.Size = new System.Drawing.Size(73, 29);
            this.txtCutOffset.TabIndex = 0;
            this.txtCutOffset.Text = "800";
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.txtCutSpeed);
            this.groupBox6.Location = new System.Drawing.Point(411, 88);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(160, 69);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "切割速度(Speed)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "mm/s";
            // 
            // txtCutSpeed
            // 
            this.txtCutSpeed.Location = new System.Drawing.Point(16, 28);
            this.txtCutSpeed.Name = "txtCutSpeed";
            this.txtCutSpeed.Size = new System.Drawing.Size(73, 29);
            this.txtCutSpeed.TabIndex = 0;
            this.txtCutSpeed.Text = "170";
            // 
            // btnAutoGen
            // 
            this.btnAutoGen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAutoGen.Location = new System.Drawing.Point(302, 426);
            this.btnAutoGen.Name = "btnAutoGen";
            this.btnAutoGen.Size = new System.Drawing.Size(118, 48);
            this.btnAutoGen.TabIndex = 16;
            this.btnAutoGen.Text = "Create";
            this.btnAutoGen.UseVisualStyleBackColor = true;
            this.btnAutoGen.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnAutoGen_MouseMove);
            // 
            // txtCH1Num
            // 
            this.txtCH1Num.Enabled = false;
            this.txtCH1Num.Location = new System.Drawing.Point(215, 377);
            this.txtCH1Num.Name = "txtCH1Num";
            this.txtCH1Num.Size = new System.Drawing.Size(69, 29);
            this.txtCH1Num.TabIndex = 21;
            this.txtCH1Num.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(211, 354);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 20);
            this.label4.TabIndex = 20;
            this.label4.Text = "CH1 Num";
            // 
            // txtCH2Num
            // 
            this.txtCH2Num.Enabled = false;
            this.txtCH2Num.Location = new System.Drawing.Point(101, 259);
            this.txtCH2Num.Name = "txtCH2Num";
            this.txtCH2Num.Size = new System.Drawing.Size(62, 29);
            this.txtCH2Num.TabIndex = 19;
            this.txtCH2Num.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 262);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 20);
            this.label3.TabIndex = 18;
            this.label3.Text = "CH2 Num";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 299);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(184, 175);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // grpEnterSpeed
            // 
            this.grpEnterSpeed.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.grpEnterSpeed.Controls.Add(this.label5);
            this.grpEnterSpeed.Controls.Add(this.txtEnterSpeed);
            this.grpEnterSpeed.Location = new System.Drawing.Point(410, 164);
            this.grpEnterSpeed.Name = "grpEnterSpeed";
            this.grpEnterSpeed.Size = new System.Drawing.Size(160, 69);
            this.grpEnterSpeed.TabIndex = 11;
            this.grpEnterSpeed.TabStop = false;
            this.grpEnterSpeed.Text = "入板速度(Speed)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(95, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 20);
            this.label5.TabIndex = 1;
            this.label5.Text = "mm/s";
            // 
            // txtEnterSpeed
            // 
            this.txtEnterSpeed.Location = new System.Drawing.Point(16, 28);
            this.txtEnterSpeed.Name = "txtEnterSpeed";
            this.txtEnterSpeed.Size = new System.Drawing.Size(73, 29);
            this.txtEnterSpeed.TabIndex = 0;
            this.txtEnterSpeed.Text = "170";
            // 
            // CutLineGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(582, 502);
            this.Controls.Add(this.grpEnterSpeed);
            this.Controls.Add(this.txtCH1Num);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtCH2Num);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnAutoGen);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "CutLineGenerator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CutLineGenerator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CutLineGenerator_FormClosed);
            this.Load += new System.EventHandler(this.CutLineGenerator_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpEnterSpeed.ResumeLayout(false);
            this.grpEnterSpeed.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rdoZ1;
        private System.Windows.Forms.RadioButton rdoZ2;
        private System.Windows.Forms.RadioButton rdoZ1Z2;
        private System.Windows.Forms.ComboBox cboChannel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdoDefault;
        private System.Windows.Forms.RadioButton rdoTwoSideCut;
        private System.Windows.Forms.RadioButton rdoOneSideCut;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCutOffset;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtCutSpeed;
        private System.Windows.Forms.Button btnAutoGen;
        private System.Windows.Forms.TextBox txtCH1Num;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCH2Num;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox grpEnterSpeed;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtEnterSpeed;
    }
}