namespace ADSW11000
{
    partial class UserLoginF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLoginF));
            this.lbUserName = new System.Windows.Forms.Label();
            this.lbUserPW = new System.Windows.Forms.Label();
            this.tbUserPW = new System.Windows.Forms.TextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.cbUserName = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            this.SuspendLayout();
            // 
            // lbUserName
            // 
            this.lbUserName.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbUserName.Location = new System.Drawing.Point(185, 13);
            this.lbUserName.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lbUserName.Name = "lbUserName";
            this.lbUserName.Size = new System.Drawing.Size(131, 45);
            this.lbUserName.TabIndex = 1;
            this.lbUserName.Text = "使用者代碼";
            this.lbUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbUserPW
            // 
            this.lbUserPW.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbUserPW.Location = new System.Drawing.Point(185, 67);
            this.lbUserPW.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lbUserPW.Name = "lbUserPW";
            this.lbUserPW.Size = new System.Drawing.Size(131, 45);
            this.lbUserPW.TabIndex = 3;
            this.lbUserPW.Text = "使用者密碼";
            this.lbUserPW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbUserPW
            // 
            this.tbUserPW.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbUserPW.Location = new System.Drawing.Point(318, 69);
            this.tbUserPW.Name = "tbUserPW";
            this.tbUserPW.PasswordChar = '*';
            this.tbUserPW.Size = new System.Drawing.Size(218, 43);
            this.tbUserPW.TabIndex = 4;
            this.tbUserPW.Click += new System.EventHandler(this.tbUserPW_Click);
            this.tbUserPW.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbUserPW_KeyPress);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "cancel.png");
            this.imageList1.Images.SetKeyName(1, "ok.png");
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageKey = "cancel.png";
            this.btnCancel.ImageList = this.imageList1;
            this.btnCancel.Location = new System.Drawing.Point(377, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(126, 60);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取  消";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.ImageKey = "ok.png";
            this.btnOK.ImageList = this.imageList1;
            this.btnOK.Location = new System.Drawing.Point(213, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(126, 60);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "確  定";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // picUser
            // 
            this.picUser.BackgroundImage = global::ADSW11000.Properties.Resources.UserLogin;
            this.picUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUser.Location = new System.Drawing.Point(5, 7);
            this.picUser.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(175, 182);
            this.picUser.TabIndex = 0;
            this.picUser.TabStop = false;
            this.picUser.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picUser_MouseDown);
            // 
            // cbUserName
            // 
            this.cbUserName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbUserName.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cbUserName.FormattingEnabled = true;
            this.cbUserName.Items.AddRange(new object[] {
            "Operator",
            "Engineer",
            "Administrator"});
            this.cbUserName.Location = new System.Drawing.Point(318, 15);
            this.cbUserName.Name = "cbUserName";
            this.cbUserName.Size = new System.Drawing.Size(218, 43);
            this.cbUserName.TabIndex = 2;
            this.cbUserName.SelectedIndexChanged += new System.EventHandler(this.cbUserName_SelectedIndexChanged);
            // 
            // UserLoginF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.ClientSize = new System.Drawing.Size(550, 196);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbUserPW);
            this.Controls.Add(this.lbUserPW);
            this.Controls.Add(this.cbUserName);
            this.Controls.Add(this.lbUserName);
            this.Controls.Add(this.picUser);
            this.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserLoginF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "使用者登入";
            this.TransparencyKey = System.Drawing.Color.Red;
            this.Activated += new System.EventHandler(this.UserLoginF_Activated);
            this.Load += new System.EventHandler(this.UserLoginF_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picUser;
        private System.Windows.Forms.Label lbUserName;
        private System.Windows.Forms.Label lbUserPW;
        private System.Windows.Forms.TextBox tbUserPW;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ComboBox cbUserName;
    }
}