namespace ProVSimpleProject
{
    partial class PackageExF1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackageExF1));
            this.PackageData = new KCSDK.TPackageData(this.components);
            this.btnUpdate = new System.Windows.Forms.Button();
            this.dFieldEdit1 = new KCSDK.DFieldEdit();
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).BeginInit();
            this.PackageContainer.Panel1.SuspendLayout();
            this.PackageContainer.Panel2.SuspendLayout();
            this.PackageContainer.SuspendLayout();
            this.pnlButton.SuspendLayout();
            this.pnlControl.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
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
            // PackageDS
            // 
            this.PackageDS.SourceName = "Package";
            // 
            // PackageContainer
            // 
            this.PackageContainer.Size = new System.Drawing.Size(1240, 743);
            this.PackageContainer.SplitterDistance = 278;
            // 
            // pnlButton
            // 
            this.pnlButton.Controls.Add(this.btnUpdate);
            this.pnlButton.Size = new System.Drawing.Size(956, 58);
            this.pnlButton.Controls.SetChildIndex(this.btnEdit, 0);
            this.pnlButton.Controls.SetChildIndex(this.btnSave, 0);
            this.pnlButton.Controls.SetChildIndex(this.btnCancel, 0);
            this.pnlButton.Controls.SetChildIndex(this.btnClose, 0);
            this.pnlButton.Controls.SetChildIndex(this.btnUpdate, 0);
            // 
            // pnlControl
            // 
            this.pnlControl.Controls.Add(this.dFieldEdit1);
            this.pnlControl.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.pnlControl.Size = new System.Drawing.Size(956, 683);
            // 
            // panel2
            // 
            this.panel2.Size = new System.Drawing.Size(276, 97);
            // 
            // PackageData
            // 
            this.PackageData.DataSource = this.PackageDS;
            this.PackageData.PackageByFile = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdate.ImageKey = "setting.png";
            this.btnUpdate.ImageList = this.imgList;
            this.btnUpdate.Location = new System.Drawing.Point(427, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(144, 50);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "更新資料";
            this.btnUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // dFieldEdit1
            // 
            this.dFieldEdit1.Caption = "視覺檔案名稱";
            this.dFieldEdit1.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit1.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.DataName = "視覺JobName";
            this.dFieldEdit1.DataSource = this.PackageDS;
            this.dFieldEdit1.DataType = KCSDK.DataManagement.DataType.Package;
            this.dFieldEdit1.DefaultValue = null;
            this.dFieldEdit1.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit1.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.EditWidth = 150;
            this.dFieldEdit1.FieldValue = "";
            this.dFieldEdit1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit1.Location = new System.Drawing.Point(43, 27);
            this.dFieldEdit1.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit1.MaxValue = 9999999;
            this.dFieldEdit1.MinValue = -9999999;
            this.dFieldEdit1.Name = "dFieldEdit1";
            this.dFieldEdit1.NoChangeInAuto = true;
            this.dFieldEdit1.Size = new System.Drawing.Size(300, 29);
            this.dFieldEdit1.StepValue = 0D;
            this.dFieldEdit1.TabIndex = 2;
            this.dFieldEdit1.Unit = "";
            this.dFieldEdit1.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit1.UnitWidth = 0;
            this.dFieldEdit1.ValueType = KCSDK.ValueDataType.String;
            // 
            // PackageExF1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1240, 743);
            this.Name = "PackageExF1";
            this.PackageContainer.Panel1.ResumeLayout(false);
            this.PackageContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PackageContainer)).EndInit();
            this.PackageContainer.ResumeLayout(false);
            this.pnlButton.ResumeLayout(false);
            this.pnlControl.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public KCSDK.TPackageData PackageData;
        private System.Windows.Forms.Button btnUpdate;
        private KCSDK.DFieldEdit dFieldEdit1;


    }
}