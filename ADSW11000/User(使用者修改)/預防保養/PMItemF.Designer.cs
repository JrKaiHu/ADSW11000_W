namespace ADSW11000
{
    partial class PMItemF
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
            this.lblItem = new System.Windows.Forms.Label();
            this.txtItem = new System.Windows.Forms.TextBox();
            this.lblCycle = new System.Windows.Forms.Label();
            this.cboPMCycle = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dtPickerPM = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(9, 19);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(60, 17);
            this.lblItem.TabIndex = 0;
            this.lblItem.Text = "保養項目";
            // 
            // txtItem
            // 
            this.txtItem.Location = new System.Drawing.Point(12, 39);
            this.txtItem.Name = "txtItem";
            this.txtItem.Size = new System.Drawing.Size(144, 25);
            this.txtItem.TabIndex = 1;
            // 
            // lblCycle
            // 
            this.lblCycle.AutoSize = true;
            this.lblCycle.Location = new System.Drawing.Point(329, 19);
            this.lblCycle.Name = "lblCycle";
            this.lblCycle.Size = new System.Drawing.Size(60, 17);
            this.lblCycle.TabIndex = 2;
            this.lblCycle.Text = "保養週期";
            // 
            // cboPMCycle
            // 
            this.cboPMCycle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPMCycle.FormattingEnabled = true;
            this.cboPMCycle.Items.AddRange(new object[] {
            "季保養",
            "半年保養",
            "年度保養"});
            this.cboPMCycle.Location = new System.Drawing.Point(332, 39);
            this.cboPMCycle.Name = "cboPMCycle";
            this.cboPMCycle.Size = new System.Drawing.Size(121, 25);
            this.cboPMCycle.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(251, 94);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(102, 36);
            this.btnAdd.TabIndex = 4;
            this.btnAdd.Text = "新增";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(121, 94);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 36);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(168, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "保養基準日";
            // 
            // dtPickerPM
            // 
            this.dtPickerPM.Location = new System.Drawing.Point(162, 39);
            this.dtPickerPM.Name = "dtPickerPM";
            this.dtPickerPM.Size = new System.Drawing.Size(160, 25);
            this.dtPickerPM.TabIndex = 7;
            // 
            // PMItemF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Cyan;
            this.ClientSize = new System.Drawing.Size(465, 157);
            this.Controls.Add(this.dtPickerPM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cboPMCycle);
            this.Controls.Add(this.lblCycle);
            this.Controls.Add(this.txtItem);
            this.Controls.Add(this.lblItem);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PMItemF";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PMItemF";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.TextBox txtItem;
        private System.Windows.Forms.Label lblCycle;
        private System.Windows.Forms.ComboBox cboPMCycle;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtPickerPM;
    }
}