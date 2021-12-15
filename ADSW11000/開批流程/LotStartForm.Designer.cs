﻿namespace ProVSimpleProject
{
    partial class LotStartForm
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
            this.uiActor1 = new ProVLib.UIActor();
            this.uiActor2 = new ProVLib.UIActor();
            this.SuspendLayout();
            // 
            // uiActor1
            // 
            this.uiActor1.BackColor = System.Drawing.Color.Blue;
            this.uiActor1.Block = true;
            this.uiActor1.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.uiActor1.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiActor1.Location = new System.Drawing.Point(472, 54);
            this.uiActor1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiActor1.Name = "uiActor1";
            this.uiActor1.Next = this.uiActor2;
            this.uiActor1.Size = new System.Drawing.Size(200, 30);
            this.uiActor1.Subject = null;
            this.uiActor1.TabIndex = 0;
            this.uiActor1.Text = "開批";
            // 
            // uiActor2
            // 
            this.uiActor2.BackColor = System.Drawing.Color.Blue;
            this.uiActor2.Block = true;
            this.uiActor2.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.uiActor2.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.uiActor2.Location = new System.Drawing.Point(472, 107);
            this.uiActor2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiActor2.Name = "uiActor2";
            this.uiActor2.Next = null;
            this.uiActor2.Size = new System.Drawing.Size(200, 30);
            this.uiActor2.Subject = null;
            this.uiActor2.TabIndex = 0;
            this.uiActor2.Text = "結束";
            this.uiActor2.OnUpdateUI += new ProVLib.UIActor.UpdateUIDelegate(this.uiActor2_OnUpdateUI);
            // 
            // LotStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 646);
            this.Controls.Add(this.uiActor2);
            this.Controls.Add(this.uiActor1);
            this.Name = "LotStartForm";
            this.Text = "LotStartForm";
            this.ResumeLayout(false);

        }

        #endregion

        private ProVLib.UIActor uiActor1;
        private ProVLib.UIActor uiActor2;
    }
}