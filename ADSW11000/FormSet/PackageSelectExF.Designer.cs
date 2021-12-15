namespace ADSW11000
{
    partial class PackageSelectExF
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageSelectExF));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.pnlButton = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSerialID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLotID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPartID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.PackageDS = new KCSDK.DataManagement(this.components);
            this.lvFolder = new KCSDK.ListViewEx();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvPackage = new KCSDK.ListViewEx();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            // 
            // pnlButton
            // 
            this.pnlButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlButton.Controls.Add(this.label5);
            this.pnlButton.Controls.Add(this.tbSerialID);
            this.pnlButton.Controls.Add(this.label4);
            this.pnlButton.Controls.Add(this.tbLotID);
            this.pnlButton.Controls.Add(this.label3);
            this.pnlButton.Controls.Add(this.tbPartID);
            this.pnlButton.Controls.Add(this.label2);
            this.pnlButton.Controls.Add(this.btnOk);
            this.pnlButton.Controls.Add(this.tbSearch);
            this.pnlButton.Controls.Add(this.btnClose);
            this.pnlButton.Controls.Add(this.label1);
            this.pnlButton.Controls.Add(this.btnSearch);
            this.pnlButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButton.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.pnlButton.Location = new System.Drawing.Point(0, 0);
            this.pnlButton.Name = "pnlButton";
            this.pnlButton.Size = new System.Drawing.Size(1198, 134);
            this.pnlButton.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(728, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(205, 30);
            this.label5.TabIndex = 14;
            this.label5.Text = "Serial ID";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbSerialID
            // 
            this.tbSerialID.Location = new System.Drawing.Point(733, 88);
            this.tbSerialID.Name = "tbSerialID";
            this.tbSerialID.Size = new System.Drawing.Size(200, 35);
            this.tbSerialID.TabIndex = 13;
            this.tbSerialID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbSerialID_KeyPress);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(517, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(205, 30);
            this.label4.TabIndex = 12;
            this.label4.Text = "Lot ID";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLotID
            // 
            this.tbLotID.Location = new System.Drawing.Point(522, 88);
            this.tbLotID.Name = "tbLotID";
            this.tbLotID.Size = new System.Drawing.Size(200, 35);
            this.tbLotID.TabIndex = 11;
            this.tbLotID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbLotID_KeyPress);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(302, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(205, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "Part ID";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbPartID
            // 
            this.tbPartID.Location = new System.Drawing.Point(307, 88);
            this.tbPartID.Name = "tbPartID";
            this.tbPartID.Size = new System.Drawing.Size(200, 35);
            this.tbPartID.TabIndex = 9;
            this.tbPartID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(302, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 35);
            this.label2.TabIndex = 8;
            this.label2.Text = "產品搜尋";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnOk.ImageKey = "save.png";
            this.btnOk.ImageList = this.imgList;
            this.btnOk.Location = new System.Drawing.Point(940, 20);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(110, 95);
            this.btnOk.TabIndex = 7;
            this.btnOk.Text = "確定";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // tbSearch
            // 
            this.tbSearch.Location = new System.Drawing.Point(416, 15);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(352, 35);
            this.tbSearch.TabIndex = 6;
            this.tbSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbSearch_KeyDown);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnClose.ImageKey = "cancel.png";
            this.btnClose.ImageList = this.imgList;
            this.btnClose.Location = new System.Drawing.Point(1060, 20);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 95);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "結束";
            this.btnClose.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
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
            this.label1.Size = new System.Drawing.Size(296, 132);
            this.label1.TabIndex = 3;
            this.label1.Text = "產品選擇";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSearch
            // 
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.ImageKey = "position.png";
            this.btnSearch.ImageList = this.imgList;
            this.btnSearch.Location = new System.Drawing.Point(774, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(159, 50);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "搜尋";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // PackageDS
            // 
            this.PackageDS.ModifiedLog = false;
            this.PackageDS.ModifiedLogToDB = false;
            // 
            // lvFolder
            // 
            this.lvFolder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader7});
            this.lvFolder.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvFolder.FixItem = "";
            this.lvFolder.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lvFolder.FullRowSelect = true;
            this.lvFolder.GridLines = true;
            this.lvFolder.Location = new System.Drawing.Point(0, 134);
            this.lvFolder.MultiSelect = false;
            this.lvFolder.Name = "lvFolder";
            this.lvFolder.SelectItem = "";
            this.lvFolder.Size = new System.Drawing.Size(291, 365);
            this.lvFolder.SubItemIndex = 1;
            this.lvFolder.TabIndex = 20;
            this.lvFolder.UseCompatibleStateImageBehavior = false;
            this.lvFolder.View = System.Windows.Forms.View.Details;
            this.lvFolder.指定項目時的前景顏色 = System.Drawing.Color.Black;
            this.lvFolder.指定項目時的背景顏色 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lvFolder.點選項目時的前景顏色 = System.Drawing.Color.White;
            this.lvFolder.點選項目時的背景顏色 = System.Drawing.SystemColors.Highlight;
            this.lvFolder.SelectedIndexChanged += new System.EventHandler(this.lvFolder_SelectedIndexChanged);
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 1;
            this.columnHeader8.Width = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.DisplayIndex = 0;
            this.columnHeader7.Text = "產品尺寸";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 252;
            // 
            // lvPackage
            // 
            this.lvPackage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader1,
            this.columnHeader2});
            this.lvPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPackage.FixItem = "";
            this.lvPackage.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lvPackage.FullRowSelect = true;
            this.lvPackage.GridLines = true;
            this.lvPackage.Location = new System.Drawing.Point(291, 134);
            this.lvPackage.MultiSelect = false;
            this.lvPackage.Name = "lvPackage";
            this.lvPackage.SelectItem = "";
            this.lvPackage.Size = new System.Drawing.Size(907, 365);
            this.lvPackage.SubItemIndex = 1;
            this.lvPackage.TabIndex = 21;
            this.lvPackage.UseCompatibleStateImageBehavior = false;
            this.lvPackage.View = System.Windows.Forms.View.Details;
            this.lvPackage.指定項目時的前景顏色 = System.Drawing.Color.Black;
            this.lvPackage.指定項目時的背景顏色 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lvPackage.點選項目時的前景顏色 = System.Drawing.Color.White;
            this.lvPackage.點選項目時的背景顏色 = System.Drawing.SystemColors.Highlight;
            this.lvPackage.SelectedIndexChanged += new System.EventHandler(this.lvPackage_SelectedIndexChanged);
            this.lvPackage.Resize += new System.EventHandler(this.lvPackage_Resize);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 0;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "產品名稱";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 400;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "建立日期";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 240;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "最後使用日期";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 240;
            // 
            // PackageSelectExF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(1198, 499);
            this.Controls.Add(this.lvPackage);
            this.Controls.Add(this.lvFolder);
            this.Controls.Add(this.pnlButton);
            this.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "PackageSelectExF";
            this.Text = "PackageSelectF";
            this.Load += new System.EventHandler(this.PackageSelectF_Load);
            this.pnlButton.ResumeLayout(false);
            this.pnlButton.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.Button btnOk;
        public KCSDK.DataManagement PackageDS;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbSerialID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbLotID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPartID;
        private KCSDK.ListViewEx lvFolder;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private KCSDK.ListViewEx lvPackage;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}