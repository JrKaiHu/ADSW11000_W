namespace ADSW11000
{
    partial class UserLoginEx
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLoginEx));
            this.btnCancel = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            this.tbUserPW = new System.Windows.Forms.TextBox();
            this.lbUserPW = new System.Windows.Forms.Label();
            this.lbUserName = new System.Windows.Forms.Label();
            this.picUser = new System.Windows.Forms.PictureBox();
            this.tbUserID = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageKey = "cancel.png";
            this.btnCancel.ImageList = this.imageList1;
            this.btnCancel.Location = new System.Drawing.Point(377, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(126, 60);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "取  消";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "cancel.png");
            this.imageList1.Images.SetKeyName(1, "ok.png");
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
            this.btnOK.TabIndex = 12;
            this.btnOK.Text = "確  定";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbUserPW
            // 
            this.tbUserPW.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbUserPW.Location = new System.Drawing.Point(318, 69);
            this.tbUserPW.Name = "tbUserPW";
            this.tbUserPW.PasswordChar = '*';
            this.tbUserPW.Size = new System.Drawing.Size(218, 43);
            this.tbUserPW.TabIndex = 11;
            this.tbUserPW.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbUserPW_KeyPress);
            // 
            // lbUserPW
            // 
            this.lbUserPW.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbUserPW.Location = new System.Drawing.Point(185, 67);
            this.lbUserPW.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lbUserPW.Name = "lbUserPW";
            this.lbUserPW.Size = new System.Drawing.Size(131, 45);
            this.lbUserPW.TabIndex = 10;
            this.lbUserPW.Text = "使用者密碼";
            this.lbUserPW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbUserName
            // 
            this.lbUserName.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbUserName.Location = new System.Drawing.Point(185, 13);
            this.lbUserName.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.lbUserName.Name = "lbUserName";
            this.lbUserName.Size = new System.Drawing.Size(131, 45);
            this.lbUserName.TabIndex = 8;
            this.lbUserName.Text = "使用者代碼";
            this.lbUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // picUser
            // 
            this.picUser.BackgroundImage = global::ADSW11000.Properties.Resources.UserLogin;
            this.picUser.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picUser.Location = new System.Drawing.Point(5, 7);
            this.picUser.Margin = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.picUser.Name = "picUser";
            this.picUser.Size = new System.Drawing.Size(175, 182);
            this.picUser.TabIndex = 7;
            this.picUser.TabStop = false;
            // 
            // tbUserID
            // 
            this.tbUserID.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.tbUserID.Location = new System.Drawing.Point(318, 13);
            this.tbUserID.Name = "tbUserID";
            this.tbUserID.Size = new System.Drawing.Size(218, 43);
            this.tbUserID.TabIndex = 14;
            this.tbUserID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbUserID_KeyPress);
            // 
            // UserLoginEx
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(550, 196);
            this.Controls.Add(this.tbUserID);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbUserPW);
            this.Controls.Add(this.lbUserPW);
            this.Controls.Add(this.lbUserName);
            this.Controls.Add(this.picUser);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "UserLoginEx";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "使用者登入";
            this.Activated += new System.EventHandler(this.UserLoginEx_Activated);
            this.Load += new System.EventHandler(this.UserLoginEx_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picUser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox tbUserPW;
        private System.Windows.Forms.Label lbUserPW;
        private System.Windows.Forms.Label lbUserName;
        private System.Windows.Forms.PictureBox picUser;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox tbUserID;

    }
}