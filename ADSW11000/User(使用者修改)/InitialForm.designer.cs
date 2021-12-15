namespace ADSW11000
{
    partial class InitialForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitialForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.imglist = new System.Windows.Forms.ImageList(this.components);
            this.lbMachineState = new System.Windows.Forms.Label();
            this.picMachine = new System.Windows.Forms.PictureBox();
            this.tmRefresh = new System.Windows.Forms.Timer(this.components);
            this.ledLabel_Z1X = new KCSDK.LEDLabel();
            this.ledLabel_Z1Z = new KCSDK.LEDLabel();
            this.ledLabel_Vision = new KCSDK.LEDLabel();
            this.ledLabel_Spindle = new KCSDK.LEDLabel();
            this.ledLabel_Z2Z = new KCSDK.LEDLabel();
            this.ledLabel_Tableθ = new KCSDK.LEDLabel();
            this.ledLabel_TableY = new KCSDK.LEDLabel();
            this.ledLabel_Z2X = new KCSDK.LEDLabel();
            this.lab_MotorZ1Pos = new System.Windows.Forms.Label();
            this.lab_MotorYPos = new System.Windows.Forms.Label();
            this.lab_MotorUPos = new System.Windows.Forms.Label();
            this.lab_MotorX1Pos = new System.Windows.Forms.Label();
            this.lab_MotorZ2Pos = new System.Windows.Forms.Label();
            this.lab_MotorX2Pos = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picMachine)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("微軟正黑體", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageKey = "cancel.png";
            this.btnCancel.ImageList = this.imglist;
            this.btnCancel.Location = new System.Drawing.Point(834, 1);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 84);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "取  消";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // imglist
            // 
            this.imglist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglist.ImageStream")));
            this.imglist.TransparentColor = System.Drawing.Color.White;
            this.imglist.Images.SetKeyName(0, "cancel.png");
            // 
            // lbMachineState
            // 
            this.lbMachineState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.lbMachineState.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbMachineState.Font = new System.Drawing.Font("微軟正黑體", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbMachineState.ForeColor = System.Drawing.Color.Black;
            this.lbMachineState.Location = new System.Drawing.Point(0, 0);
            this.lbMachineState.Name = "lbMachineState";
            this.lbMachineState.Size = new System.Drawing.Size(1121, 86);
            this.lbMachineState.TabIndex = 6;
            this.lbMachineState.Text = "機台初始化中";
            this.lbMachineState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picMachine
            // 
            this.picMachine.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picMachine.BackgroundImage")));
            this.picMachine.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.picMachine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMachine.Location = new System.Drawing.Point(0, 86);
            this.picMachine.Name = "picMachine";
            this.picMachine.Size = new System.Drawing.Size(1121, 638);
            this.picMachine.TabIndex = 8;
            this.picMachine.TabStop = false;
            // 
            // tmRefresh
            // 
            this.tmRefresh.Interval = 40;
            this.tmRefresh.Tick += new System.EventHandler(this.tmRefresh_Tick);
            // 
            // ledLabel_Z1X
            // 
            this.ledLabel_Z1X.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Z1X.Caption = "X1 軸";
            this.ledLabel_Z1X.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Z1X.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Z1X.Location = new System.Drawing.Point(773, 111);
            this.ledLabel_Z1X.Name = "ledLabel_Z1X";
            this.ledLabel_Z1X.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Z1X.TabIndex = 57;
            this.ledLabel_Z1X.Value = false;
            // 
            // ledLabel_Z1Z
            // 
            this.ledLabel_Z1Z.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Z1Z.Caption = "Z1 軸";
            this.ledLabel_Z1Z.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Z1Z.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Z1Z.Location = new System.Drawing.Point(773, 190);
            this.ledLabel_Z1Z.Name = "ledLabel_Z1Z";
            this.ledLabel_Z1Z.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Z1Z.TabIndex = 56;
            this.ledLabel_Z1Z.Value = false;
            // 
            // ledLabel_Vision
            // 
            this.ledLabel_Vision.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Vision.Caption = "Vision";
            this.ledLabel_Vision.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Vision.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Vision.Location = new System.Drawing.Point(29, 157);
            this.ledLabel_Vision.Name = "ledLabel_Vision";
            this.ledLabel_Vision.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Vision.TabIndex = 55;
            this.ledLabel_Vision.Value = false;
            // 
            // ledLabel_Spindle
            // 
            this.ledLabel_Spindle.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Spindle.Caption = "Spindle";
            this.ledLabel_Spindle.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Spindle.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Spindle.Location = new System.Drawing.Point(29, 111);
            this.ledLabel_Spindle.Name = "ledLabel_Spindle";
            this.ledLabel_Spindle.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Spindle.TabIndex = 54;
            this.ledLabel_Spindle.Value = false;
            // 
            // ledLabel_Z2Z
            // 
            this.ledLabel_Z2Z.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Z2Z.Caption = "Z2軸";
            this.ledLabel_Z2Z.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Z2Z.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Z2Z.Location = new System.Drawing.Point(226, 190);
            this.ledLabel_Z2Z.Name = "ledLabel_Z2Z";
            this.ledLabel_Z2Z.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Z2Z.TabIndex = 50;
            this.ledLabel_Z2Z.Value = false;
            // 
            // ledLabel_Tableθ
            // 
            this.ledLabel_Tableθ.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Tableθ.Caption = " θ 軸";
            this.ledLabel_Tableθ.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Tableθ.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Tableθ.Location = new System.Drawing.Point(484, 261);
            this.ledLabel_Tableθ.Name = "ledLabel_Tableθ";
            this.ledLabel_Tableθ.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Tableθ.TabIndex = 51;
            this.ledLabel_Tableθ.Value = false;
            // 
            // ledLabel_TableY
            // 
            this.ledLabel_TableY.BackColor = System.Drawing.Color.Black;
            this.ledLabel_TableY.Caption = " Y 軸";
            this.ledLabel_TableY.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_TableY.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_TableY.Location = new System.Drawing.Point(484, 521);
            this.ledLabel_TableY.Name = "ledLabel_TableY";
            this.ledLabel_TableY.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_TableY.TabIndex = 52;
            this.ledLabel_TableY.Value = false;
            // 
            // ledLabel_Z2X
            // 
            this.ledLabel_Z2X.BackColor = System.Drawing.Color.Black;
            this.ledLabel_Z2X.Caption = "X2 軸";
            this.ledLabel_Z2X.CaptionFont = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.ledLabel_Z2X.CpationColor = System.Drawing.Color.Yellow;
            this.ledLabel_Z2X.Location = new System.Drawing.Point(226, 111);
            this.ledLabel_Z2X.Name = "ledLabel_Z2X";
            this.ledLabel_Z2X.Size = new System.Drawing.Size(158, 30);
            this.ledLabel_Z2X.TabIndex = 53;
            this.ledLabel_Z2X.Value = false;
            // 
            // lab_MotorZ1Pos
            // 
            this.lab_MotorZ1Pos.AutoSize = true;
            this.lab_MotorZ1Pos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorZ1Pos.Location = new System.Drawing.Point(25, 261);
            this.lab_MotorZ1Pos.Name = "lab_MotorZ1Pos";
            this.lab_MotorZ1Pos.Size = new System.Drawing.Size(136, 21);
            this.lab_MotorZ1Pos.TabIndex = 61;
            this.lab_MotorZ1Pos.Text = "lab_MotorZ1Pos";
            // 
            // lab_MotorYPos
            // 
            this.lab_MotorYPos.AutoSize = true;
            this.lab_MotorYPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorYPos.Location = new System.Drawing.Point(25, 223);
            this.lab_MotorYPos.Name = "lab_MotorYPos";
            this.lab_MotorYPos.Size = new System.Drawing.Size(126, 21);
            this.lab_MotorYPos.TabIndex = 60;
            this.lab_MotorYPos.Text = "lab_MotorYPos";
            // 
            // lab_MotorUPos
            // 
            this.lab_MotorUPos.AutoSize = true;
            this.lab_MotorUPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorUPos.Location = new System.Drawing.Point(25, 337);
            this.lab_MotorUPos.Name = "lab_MotorUPos";
            this.lab_MotorUPos.Size = new System.Drawing.Size(128, 21);
            this.lab_MotorUPos.TabIndex = 59;
            this.lab_MotorUPos.Text = "lab_MotorUPos";
            // 
            // lab_MotorX1Pos
            // 
            this.lab_MotorX1Pos.AutoSize = true;
            this.lab_MotorX1Pos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorX1Pos.Location = new System.Drawing.Point(25, 375);
            this.lab_MotorX1Pos.Name = "lab_MotorX1Pos";
            this.lab_MotorX1Pos.Size = new System.Drawing.Size(137, 21);
            this.lab_MotorX1Pos.TabIndex = 58;
            this.lab_MotorX1Pos.Text = "lab_MotorX1Pos";
            // 
            // lab_MotorZ2Pos
            // 
            this.lab_MotorZ2Pos.AutoSize = true;
            this.lab_MotorZ2Pos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorZ2Pos.Location = new System.Drawing.Point(25, 299);
            this.lab_MotorZ2Pos.Name = "lab_MotorZ2Pos";
            this.lab_MotorZ2Pos.Size = new System.Drawing.Size(136, 21);
            this.lab_MotorZ2Pos.TabIndex = 62;
            this.lab_MotorZ2Pos.Text = "lab_MotorZ2Pos";
            // 
            // lab_MotorX2Pos
            // 
            this.lab_MotorX2Pos.AutoSize = true;
            this.lab_MotorX2Pos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lab_MotorX2Pos.Location = new System.Drawing.Point(25, 413);
            this.lab_MotorX2Pos.Name = "lab_MotorX2Pos";
            this.lab_MotorX2Pos.Size = new System.Drawing.Size(137, 21);
            this.lab_MotorX2Pos.TabIndex = 63;
            this.lab_MotorX2Pos.Text = "lab_MotorX2Pos";
            // 
            // InitialForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1121, 724);
            this.Controls.Add(this.lab_MotorX2Pos);
            this.Controls.Add(this.lab_MotorZ2Pos);
            this.Controls.Add(this.lab_MotorZ1Pos);
            this.Controls.Add(this.lab_MotorYPos);
            this.Controls.Add(this.lab_MotorUPos);
            this.Controls.Add(this.lab_MotorX1Pos);
            this.Controls.Add(this.ledLabel_Z1X);
            this.Controls.Add(this.ledLabel_Z1Z);
            this.Controls.Add(this.ledLabel_Vision);
            this.Controls.Add(this.ledLabel_Spindle);
            this.Controls.Add(this.ledLabel_Z2Z);
            this.Controls.Add(this.ledLabel_Tableθ);
            this.Controls.Add(this.ledLabel_TableY);
            this.Controls.Add(this.ledLabel_Z2X);
            this.Controls.Add(this.picMachine);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbMachineState);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InitialForm";
            this.Text = "初始化";
            this.Load += new System.EventHandler(this.InitialForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picMachine)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbMachineState;
        private System.Windows.Forms.ImageList imglist;
        private System.Windows.Forms.PictureBox picMachine;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Timer tmRefresh;
        private KCSDK.LEDLabel ledLabel_Z1X;
        private KCSDK.LEDLabel ledLabel_Z1Z;
        private KCSDK.LEDLabel ledLabel_Vision;
        private KCSDK.LEDLabel ledLabel_Spindle;
        private KCSDK.LEDLabel ledLabel_Z2Z;
        private KCSDK.LEDLabel ledLabel_Tableθ;
        private KCSDK.LEDLabel ledLabel_TableY;
        private KCSDK.LEDLabel ledLabel_Z2X;
        private System.Windows.Forms.Label lab_MotorZ1Pos;
        private System.Windows.Forms.Label lab_MotorYPos;
        private System.Windows.Forms.Label lab_MotorUPos;
        private System.Windows.Forms.Label lab_MotorX1Pos;
        private System.Windows.Forms.Label lab_MotorZ2Pos;
        private System.Windows.Forms.Label lab_MotorX2Pos;
    }
}