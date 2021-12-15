namespace ADSW11000
{
    partial class BasePackageF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasePackageF));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.PackageContainer = new System.Windows.Forms.SplitContainer();
            this.lvPackage = new KCSDK.ListViewEx();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel2 = new System.Windows.Forms.Panel();
            this.tbPackage = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnImgList = new System.Windows.Forms.ImageList(this.components);
            this.lbPackage = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.pnlControl = new System.Windows.Forms.Panel();
            this.pnlButton = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.PreloadPackageDS = new KCSDK.DataManagement(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).BeginInit();
            this.PackageContainer.Panel1.SuspendLayout();
            this.PackageContainer.Panel2.SuspendLayout();
            this.PackageContainer.SuspendLayout();
            this.panel2.SuspendLayout();
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
            // PackageContainer
            // 
            this.PackageContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PackageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PackageContainer.Location = new System.Drawing.Point(0, 0);
            this.PackageContainer.Name = "PackageContainer";
            // 
            // PackageContainer.Panel1
            // 
            this.PackageContainer.Panel1.Controls.Add(this.lvPackage);
            this.PackageContainer.Panel1.Controls.Add(this.panel2);
            this.PackageContainer.Panel1MinSize = 275;
            // 
            // PackageContainer.Panel2
            // 
            this.PackageContainer.Panel2.Controls.Add(this.pnlControl);
            this.PackageContainer.Panel2.Controls.Add(this.pnlButton);
            this.PackageContainer.Size = new System.Drawing.Size(1054, 561);
            this.PackageContainer.SplitterDistance = 275;
            this.PackageContainer.TabIndex = 5;
            // 
            // lvPackage
            // 
            this.lvPackage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPackage.FixItem = "";
            this.lvPackage.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lvPackage.FullRowSelect = true;
            this.lvPackage.GridLines = true;
            this.lvPackage.Location = new System.Drawing.Point(0, 95);
            this.lvPackage.MultiSelect = false;
            this.lvPackage.Name = "lvPackage";
            this.lvPackage.SelectItem = "";
            this.lvPackage.Size = new System.Drawing.Size(273, 464);
            this.lvPackage.SubItemIndex = 1;
            this.lvPackage.TabIndex = 17;
            this.lvPackage.UseCompatibleStateImageBehavior = false;
            this.lvPackage.View = System.Windows.Forms.View.Details;
            this.lvPackage.指定項目時的前景顏色 = System.Drawing.Color.Black;
            this.lvPackage.指定項目時的背景顏色 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lvPackage.點選項目時的前景顏色 = System.Drawing.Color.White;
            this.lvPackage.點選項目時的背景顏色 = System.Drawing.SystemColors.Highlight;
            this.lvPackage.SelectedIndexChanged += new System.EventHandler(this.lvFolder_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "產品名稱";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 240;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tbPackage);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.lbPackage);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(273, 95);
            this.panel2.TabIndex = 16;
            // 
            // tbPackage
            // 
            this.tbPackage.Location = new System.Drawing.Point(119, 52);
            this.tbPackage.Name = "tbPackage";
            this.tbPackage.Size = new System.Drawing.Size(146, 35);
            this.tbPackage.TabIndex = 14;
            this.tbPackage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPackage_KeyDown);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImageKey = "delete.png";
            this.button1.ImageList = this.btnImgList;
            this.button1.Location = new System.Drawing.Point(180, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 43);
            this.button1.TabIndex = 13;
            this.button1.Text = "刪除";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnImgList
            // 
            this.btnImgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("btnImgList.ImageStream")));
            this.btnImgList.TransparentColor = System.Drawing.Color.White;
            this.btnImgList.Images.SetKeyName(0, "close.png");
            this.btnImgList.Images.SetKeyName(1, "new.png");
            this.btnImgList.Images.SetKeyName(2, "change.png");
            this.btnImgList.Images.SetKeyName(3, "delete.png");
            // 
            // lbPackage
            // 
            this.lbPackage.Location = new System.Drawing.Point(10, 52);
            this.lbPackage.Name = "lbPackage";
            this.lbPackage.Size = new System.Drawing.Size(103, 35);
            this.lbPackage.TabIndex = 15;
            this.lbPackage.Text = "產品搜尋";
            this.lbPackage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.ImageKey = "change.png";
            this.button2.ImageList = this.btnImgList;
            this.button2.Location = new System.Drawing.Point(93, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 43);
            this.button2.TabIndex = 12;
            this.button2.Text = "更名";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.ImageKey = "new.png";
            this.button3.ImageList = this.btnImgList;
            this.button3.Location = new System.Drawing.Point(6, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(85, 43);
            this.button3.TabIndex = 11;
            this.button3.Text = "新增";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // pnlControl
            // 
            this.pnlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlControl.Location = new System.Drawing.Point(0, 58);
            this.pnlControl.Name = "pnlControl";
            this.pnlControl.Size = new System.Drawing.Size(773, 501);
            this.pnlControl.TabIndex = 6;
            // 
            // pnlButton
            // 
            this.pnlButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlButton.Controls.Add(this.btnClose);
            this.pnlButton.Controls.Add(this.btnCancel);
            this.pnlButton.Controls.Add(this.btnSave);
            this.pnlButton.Controls.Add(this.btnEdit);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButton.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.pnlButton.Location = new System.Drawing.Point(0, 0);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(773, 58);
            this.pnlButton.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClose.ImageKey = "closebtn.png";
            this.btnClose.ImageList = this.imgList;
            this.btnClose.Location = new System.Drawing.Point(321, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 50);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "結束";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.ImageKey = "cancel.png";
            this.btnCancel.ImageList = this.imgList;
            this.btnCancel.Location = new System.Drawing.Point(215, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 50);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.ImageKey = "save.png";
            this.btnSave.ImageList = this.imgList;
            this.btnSave.Location = new System.Drawing.Point(109, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 50);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "儲存";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEdit.ImageKey = "edit.png";
            this.btnEdit.ImageList = this.imgList;
            this.btnEdit.Location = new System.Drawing.Point(3, 3);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 50);
            this.btnEdit.TabIndex = 0;
            this.btnEdit.Text = "編輯";
            this.btnEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // PreloadPackageDS
            // 
            this.PreloadPackageDS.ModifiedLog = true;
            // 
            // BasePackageF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1054, 561);
            this.Controls.Add(this.PackageContainer);
            this.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "BasePackageF";
            this.Text = "PackageF";
            this.Load += new System.EventHandler(this.PackageF_Load);
            this.PackageContainer.Panel1.ResumeLayout(false);
            this.PackageContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).EndInit();
            this.PackageContainer.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ImageList btnImgList;
        public System.Windows.Forms.SplitContainer PackageContainer;
        public System.Windows.Forms.Panel pnlButton;
        public System.Windows.Forms.Button btnClose;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnEdit;
        public System.Windows.Forms.Panel pnlControl;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox tbPackage;
        private System.Windows.Forms.Label lbPackage;
        public KCSDK.DataManagement PreloadPackageDS;
        private KCSDK.ListViewEx lvPackage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}