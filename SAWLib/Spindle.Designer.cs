namespace SAWLib
{
    partial class Spindle
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
            Disconnect();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Spindle));
            this.ledState = new KCSDK.LED();
            this.SuspendLayout();
            // 
            // ledState
            // 
            this.ledState.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ledState.BackgroundImage")));
            this.ledState.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ledState.Location = new System.Drawing.Point(177, 5);
            this.ledState.Name = "ledState";
            this.ledState.Size = new System.Drawing.Size(20, 20);
            this.ledState.TabIndex = 0;
            this.ledState.Value = KCSDK.LEDState.Off;
            // 
            // Spindle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.ledState);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Spindle";
            this.Size = new System.Drawing.Size(200, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private KCSDK.LED ledState;
    }
}
