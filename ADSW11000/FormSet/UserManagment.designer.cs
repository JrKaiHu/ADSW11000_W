namespace ADSW11000
{
    partial class UserManagment
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
            this.textLabel1 = new KCSDK.TextLabel();
            this.textLabel2 = new KCSDK.TextLabel();
            this.textLabel3 = new KCSDK.TextLabel();
            this.tbUserID = new System.Windows.Forms.TextBox();
            this.tbUserPW = new System.Windows.Forms.TextBox();
            this.cbUserRight = new System.Windows.Forms.ComboBox();
            this.btnAddUser = new System.Windows.Forms.Button();
            this.btnDelUser = new System.Windows.Forms.Button();
            this.btnChangeUser = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lvUserList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // textLabel1
            // 
            this.textLabel1.Caption = "人員編號";
            this.textLabel1.CaptionColor = System.Drawing.Color.Black;
            this.textLabel1.CaptionFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textLabel1.CaptionWidth = 170;
            this.textLabel1.Location = new System.Drawing.Point(15, 16);
            this.textLabel1.Name = "textLabel1";
            this.textLabel1.Size = new System.Drawing.Size(171, 35);
            this.textLabel1.TabIndex = 0;
            this.textLabel1.Value = "文字";
            this.textLabel1.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.textLabel1.ValueFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // textLabel2
            // 
            this.textLabel2.Caption = "人員密碼";
            this.textLabel2.CaptionColor = System.Drawing.Color.Black;
            this.textLabel2.CaptionFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textLabel2.CaptionWidth = 170;
            this.textLabel2.Location = new System.Drawing.Point(15, 55);
            this.textLabel2.Name = "textLabel2";
            this.textLabel2.Size = new System.Drawing.Size(171, 35);
            this.textLabel2.TabIndex = 1;
            this.textLabel2.Value = "文字";
            this.textLabel2.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.textLabel2.ValueFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // textLabel3
            // 
            this.textLabel3.Caption = "人員權限";
            this.textLabel3.CaptionColor = System.Drawing.Color.Black;
            this.textLabel3.CaptionFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.textLabel3.CaptionWidth = 170;
            this.textLabel3.Location = new System.Drawing.Point(15, 94);
            this.textLabel3.Name = "textLabel3";
            this.textLabel3.Size = new System.Drawing.Size(171, 35);
            this.textLabel3.TabIndex = 2;
            this.textLabel3.Value = "文字";
            this.textLabel3.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(137)))), ((int)(((byte)(195)))), ((int)(((byte)(97)))));
            this.textLabel3.ValueFont = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            // 
            // tbUserID
            // 
            this.tbUserID.Location = new System.Drawing.Point(189, 16);
            this.tbUserID.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tbUserID.Name = "tbUserID";
            this.tbUserID.Size = new System.Drawing.Size(171, 35);
            this.tbUserID.TabIndex = 3;
            // 
            // tbUserPW
            // 
            this.tbUserPW.Location = new System.Drawing.Point(189, 55);
            this.tbUserPW.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tbUserPW.Name = "tbUserPW";
            this.tbUserPW.Size = new System.Drawing.Size(171, 35);
            this.tbUserPW.TabIndex = 4;
            // 
            // cbUserRight
            // 
            this.cbUserRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUserRight.FormattingEnabled = true;
            this.cbUserRight.Items.AddRange(new object[] {
            "Operator",
            "Enginneer",
            "Administrator"});
            this.cbUserRight.Location = new System.Drawing.Point(189, 94);
            this.cbUserRight.Name = "cbUserRight";
            this.cbUserRight.Size = new System.Drawing.Size(171, 35);
            this.cbUserRight.TabIndex = 5;
            // 
            // btnAddUser
            // 
            this.btnAddUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnAddUser.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnAddUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddUser.Location = new System.Drawing.Point(15, 139);
            this.btnAddUser.Name = "btnAddUser";
            this.btnAddUser.Size = new System.Drawing.Size(155, 52);
            this.btnAddUser.TabIndex = 61;
            this.btnAddUser.Text = "新增人員";
            this.btnAddUser.UseVisualStyleBackColor = false;
            this.btnAddUser.Click += new System.EventHandler(this.btnAddUser_Click);
            // 
            // btnDelUser
            // 
            this.btnDelUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnDelUser.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDelUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnDelUser.Location = new System.Drawing.Point(205, 139);
            this.btnDelUser.Name = "btnDelUser";
            this.btnDelUser.Size = new System.Drawing.Size(155, 52);
            this.btnDelUser.TabIndex = 62;
            this.btnDelUser.Text = "刪除人員";
            this.btnDelUser.UseVisualStyleBackColor = false;
            this.btnDelUser.Click += new System.EventHandler(this.btnDelUser_Click);
            // 
            // btnChangeUser
            // 
            this.btnChangeUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnChangeUser.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnChangeUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnChangeUser.Location = new System.Drawing.Point(15, 197);
            this.btnChangeUser.Name = "btnChangeUser";
            this.btnChangeUser.Size = new System.Drawing.Size(155, 52);
            this.btnChangeUser.TabIndex = 63;
            this.btnChangeUser.Text = "修改資料";
            this.btnChangeUser.UseVisualStyleBackColor = false;
            this.btnChangeUser.Click += new System.EventHandler(this.btnChangeUser_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnClose.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnClose.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClose.Location = new System.Drawing.Point(205, 197);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(155, 52);
            this.btnClose.TabIndex = 64;
            this.btnClose.Text = "結       束";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lvUserList
            // 
            this.lvUserList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvUserList.FullRowSelect = true;
            this.lvUserList.Location = new System.Drawing.Point(369, 12);
            this.lvUserList.MultiSelect = false;
            this.lvUserList.Name = "lvUserList";
            this.lvUserList.Size = new System.Drawing.Size(443, 244);
            this.lvUserList.TabIndex = 65;
            this.lvUserList.UseCompatibleStateImageBehavior = false;
            this.lvUserList.View = System.Windows.Forms.View.Details;
            this.lvUserList.SelectedIndexChanged += new System.EventHandler(this.lvUserList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "人員編號";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "人員密碼";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "人員權限";
            this.columnHeader3.Width = 150;
            // 
            // UserManagment
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(821, 266);
            this.Controls.Add(this.lvUserList);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnChangeUser);
            this.Controls.Add(this.btnDelUser);
            this.Controls.Add(this.btnAddUser);
            this.Controls.Add(this.cbUserRight);
            this.Controls.Add(this.tbUserPW);
            this.Controls.Add(this.tbUserID);
            this.Controls.Add(this.textLabel3);
            this.Controls.Add(this.textLabel2);
            this.Controls.Add(this.textLabel1);
            this.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.MaximizeBox = false;
            this.Name = "UserManagment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "人員管理";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private KCSDK.TextLabel textLabel1;
        private KCSDK.TextLabel textLabel2;
        private KCSDK.TextLabel textLabel3;
        private System.Windows.Forms.TextBox tbUserID;
        private System.Windows.Forms.TextBox tbUserPW;
        private System.Windows.Forms.ComboBox cbUserRight;
        private System.Windows.Forms.Button btnAddUser;
        private System.Windows.Forms.Button btnDelUser;
        private System.Windows.Forms.Button btnChangeUser;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListView lvUserList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}