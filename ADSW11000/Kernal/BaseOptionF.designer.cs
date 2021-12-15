namespace ADSW11000
{
    partial class BaseOptionF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseOptionF));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel2 = new System.Windows.Forms.Button();
            this.btnSave2 = new System.Windows.Forms.Button();
            this.btnEdit2 = new System.Windows.Forms.Button();
            this.OptionDS = new KCSDK.DataManagement(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlButton.SuspendLayout();
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
            // OptionDS
            // 
            this.OptionDS.ModifiedLog = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // BaseOptionF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1229, 730);
            this.Controls.Add(this.pnlButton);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "BaseOptionF";
            this.Text = "OptionF";
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Panel pnlButton;
        public KCSDK.DataManagement OptionDS;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnCancel2;
        public System.Windows.Forms.Button btnSave2;
        public System.Windows.Forms.Button btnEdit2;
        public System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}