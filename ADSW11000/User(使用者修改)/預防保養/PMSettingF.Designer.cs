namespace ADSW11000
{
    partial class PMSettingF
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scPM = new System.Windows.Forms.SplitContainer();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvPM = new System.Windows.Forms.DataGridView();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scPM)).BeginInit();
            this.scPM.Panel1.SuspendLayout();
            this.scPM.Panel2.SuspendLayout();
            this.scPM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPM)).BeginInit();
            this.SuspendLayout();
            // 
            // scPM
            // 
            this.scPM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scPM.Location = new System.Drawing.Point(0, 0);
            this.scPM.Name = "scPM";
            this.scPM.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scPM.Panel1
            // 
            this.scPM.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.scPM.Panel1.Controls.Add(this.dgvPM);
            this.scPM.Panel1.Controls.Add(this.lblTitle);
            // 
            // scPM.Panel2
            // 
            this.scPM.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.scPM.Panel2.Controls.Add(this.btnIgnore);
            this.scPM.Panel2.Controls.Add(this.btnSave);
            this.scPM.Panel2.Controls.Add(this.btnClose);
            this.scPM.Panel2.Controls.Add(this.btnAdd);
            this.scPM.Size = new System.Drawing.Size(634, 669);
            this.scPM.SplitterDistance = 590;
            this.scPM.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.BackColor = System.Drawing.Color.Blue;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTitle.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(610, 49);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "預防保養設定畫面";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgvPM
            // 
            this.dgvPM.AllowUserToAddRows = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.LightSkyBlue;
            this.dgvPM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPM.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPM.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvPM.Location = new System.Drawing.Point(12, 61);
            this.dgvPM.Name = "dgvPM";
            this.dgvPM.RowTemplate.Height = 24;
            this.dgvPM.Size = new System.Drawing.Size(610, 526);
            this.dgvPM.TabIndex = 0;
            this.dgvPM.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPM_CellClick);
            this.dgvPM.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPM_EditingControlShowing);
            this.dgvPM.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvPM_UserDeletingRow);
            // 
            // btnIgnore
            // 
            this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.btnIgnore.Location = new System.Drawing.Point(362, 8);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(133, 55);
            this.btnIgnore.TabIndex = 3;
            this.btnIgnore.Text = "忽略提示";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(139, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(121, 55);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "儲存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnClose.Location = new System.Drawing.Point(501, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(121, 55);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "關閉";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 8);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(121, 55);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "新增保養項目";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // PMSettingF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cyan;
            this.ClientSize = new System.Drawing.Size(634, 669);
            this.Controls.Add(this.scPM);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PMSettingF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PMSettingF";
            this.scPM.Panel1.ResumeLayout(false);
            this.scPM.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scPM)).EndInit();
            this.scPM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPM)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scPM;
        private System.Windows.Forms.DataGridView dgvPM;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Label lblTitle;
    }
}