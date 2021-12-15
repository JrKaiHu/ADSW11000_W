namespace SAWLib
{
    partial class NCTester
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblPos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPos
            // 
            this.lblPos.BackColor = System.Drawing.Color.ForestGreen;
            this.lblPos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPos.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblPos.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblPos.ForeColor = System.Drawing.Color.White;
            this.lblPos.Location = new System.Drawing.Point(110, 0);
            this.lblPos.Name = "lblPos";
            this.lblPos.Size = new System.Drawing.Size(90, 30);
            this.lblPos.TabIndex = 0;
            this.lblPos.Text = "0";
            this.lblPos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NCTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.Controls.Add(this.lblPos);
            this.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "NCTester";
            this.Size = new System.Drawing.Size(200, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPos;
    }
}
