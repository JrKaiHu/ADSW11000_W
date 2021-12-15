namespace ADSW11000
{
    partial class AlarmForm
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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnDetail = new System.Windows.Forms.Button();
            this.PictureAlarm = new System.Windows.Forms.PictureBox();
            this.txtAlarm = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureAlarm)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnConfirm.Location = new System.Drawing.Point(442, 491);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(122, 51);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "確 認";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // btnDetail
            // 
            this.btnDetail.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnDetail.Location = new System.Drawing.Point(12, 491);
            this.btnDetail.Name = "btnDetail";
            this.btnDetail.Size = new System.Drawing.Size(122, 51);
            this.btnDetail.TabIndex = 2;
            this.btnDetail.Text = "詳 細 資 訊";
            this.btnDetail.UseVisualStyleBackColor = true;
            this.btnDetail.Click += new System.EventHandler(this.btnDetail_Click);
            // 
            // PictureAlarm
            // 
            this.PictureAlarm.BackColor = System.Drawing.SystemColors.Control;
            this.PictureAlarm.Location = new System.Drawing.Point(12, 12);
            this.PictureAlarm.Name = "PictureAlarm";
            this.PictureAlarm.Size = new System.Drawing.Size(552, 384);
            this.PictureAlarm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureAlarm.TabIndex = 1;
            this.PictureAlarm.TabStop = false;
            // 
            // txtAlarm
            // 
            this.txtAlarm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtAlarm.Font = new System.Drawing.Font("微軟正黑體", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtAlarm.ForeColor = System.Drawing.Color.Black;
            this.txtAlarm.Location = new System.Drawing.Point(12, 402);
            this.txtAlarm.Multiline = true;
            this.txtAlarm.Name = "txtAlarm";
            this.txtAlarm.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtAlarm.Size = new System.Drawing.Size(552, 83);
            this.txtAlarm.TabIndex = 3;
            // 
            // AlarmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(576, 554);
            this.Controls.Add(this.txtAlarm);
            this.Controls.Add(this.btnDetail);
            this.Controls.Add(this.PictureAlarm);
            this.Controls.Add(this.btnConfirm);
            this.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AlarmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AlarmForm";
            this.Load += new System.EventHandler(this.AlarmForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureAlarm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.PictureBox PictureAlarm;
        private System.Windows.Forms.Button btnDetail;
        private System.Windows.Forms.TextBox txtAlarm;
    }
}