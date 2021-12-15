namespace ADSW11000
{
    partial class CutLineSimulate
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tlpState = new System.Windows.Forms.TableLayoutPanel();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.dCutProg = new KCSDK.DDataGridView();
            this.Cut_Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_Z1使用 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Cut_Z1刀順 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_Z2使用 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Cut_Z2刀順 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_料片方向 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_高度偏移 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_切割方向 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_速度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cut_入板速度 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dCutProg)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpState
            // 
            this.tlpState.ColumnCount = 2;
            this.tlpState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.Location = new System.Drawing.Point(52, 40);
            this.tlpState.Name = "tlpState";
            this.tlpState.RowCount = 2;
            this.tlpState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpState.Size = new System.Drawing.Size(488, 500);
            this.tlpState.TabIndex = 0;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(40, 582);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(127, 52);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.OnButtonClicked);
            // 
            // btnRestore
            // 
            this.btnRestore.Location = new System.Drawing.Point(334, 582);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(127, 52);
            this.btnRestore.TabIndex = 2;
            this.btnRestore.Text = "Restore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.OnButtonClicked);
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(481, 582);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(127, 52);
            this.btnForward.TabIndex = 3;
            this.btnForward.Text = "Forward";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.OnButtonClicked);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(628, 582);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(127, 52);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.OnButtonClicked);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(187, 582);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(127, 52);
            this.btnPause.TabIndex = 5;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.OnButtonClicked);
            // 
            // dCutProg
            // 
            this.dCutProg.AllowUserToAddRows = false;
            this.dCutProg.AllowUserToDeleteRows = false;
            this.dCutProg.AllowUserToResizeColumns = false;
            this.dCutProg.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dCutProg.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dCutProg.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dCutProg.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dCutProg.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dCutProg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dCutProg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Cut_Index,
            this.Cut_Z1使用,
            this.Cut_Z1刀順,
            this.Cut_Z2使用,
            this.Cut_Z2刀順,
            this.Cut_料片方向,
            this.Cut_高度偏移,
            this.Cut_切割方向,
            this.Cut_速度,
            this.Cut_入板速度});
            this.dCutProg.DataName = "DGVCutProg";
            this.dCutProg.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dCutProg.Location = new System.Drawing.Point(694, 40);
            this.dCutProg.MultiSelect = false;
            this.dCutProg.Name = "dCutProg";
            this.dCutProg.NoChangeInAuto = false;
            this.dCutProg.ReadOnly = true;
            this.dCutProg.RowHeadersWidth = 10;
            this.dCutProg.RowHeight = 24;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dCutProg.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dCutProg.RowTemplate.Height = 24;
            this.dCutProg.Size = new System.Drawing.Size(558, 500);
            this.dCutProg.TabIndex = 57;
            this.dCutProg.TableDataSource = null;
            // 
            // Cut_Index
            // 
            this.Cut_Index.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_Index.HeaderText = "Order";
            this.Cut_Index.Name = "Cut_Index";
            this.Cut_Index.ReadOnly = true;
            this.Cut_Index.Width = 79;
            // 
            // Cut_Z1使用
            // 
            this.Cut_Z1使用.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_Z1使用.FalseValue = "False";
            this.Cut_Z1使用.HeaderText = "Z1";
            this.Cut_Z1使用.Name = "Cut_Z1使用";
            this.Cut_Z1使用.ReadOnly = true;
            this.Cut_Z1使用.TrueValue = "True";
            this.Cut_Z1使用.Width = 33;
            // 
            // Cut_Z1刀順
            // 
            this.Cut_Z1刀順.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_Z1刀順.HeaderText = "Z1 刀順";
            this.Cut_Z1刀順.Name = "Cut_Z1刀順";
            this.Cut_Z1刀順.ReadOnly = true;
            this.Cut_Z1刀順.Width = 99;
            // 
            // Cut_Z2使用
            // 
            this.Cut_Z2使用.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_Z2使用.FalseValue = "False";
            this.Cut_Z2使用.HeaderText = "Z2";
            this.Cut_Z2使用.Name = "Cut_Z2使用";
            this.Cut_Z2使用.ReadOnly = true;
            this.Cut_Z2使用.TrueValue = "True";
            this.Cut_Z2使用.Width = 33;
            // 
            // Cut_Z2刀順
            // 
            this.Cut_Z2刀順.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_Z2刀順.HeaderText = "Z2 刀順";
            this.Cut_Z2刀順.Name = "Cut_Z2刀順";
            this.Cut_Z2刀順.ReadOnly = true;
            this.Cut_Z2刀順.Width = 99;
            // 
            // Cut_料片方向
            // 
            this.Cut_料片方向.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_料片方向.HeaderText = "料片方向(CH)";
            this.Cut_料片方向.Name = "Cut_料片方向";
            this.Cut_料片方向.ReadOnly = true;
            this.Cut_料片方向.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Cut_料片方向.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cut_料片方向.Width = 82;
            // 
            // Cut_高度偏移
            // 
            this.Cut_高度偏移.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_高度偏移.HeaderText = "提刀高度(H)";
            this.Cut_高度偏移.Name = "Cut_高度偏移";
            this.Cut_高度偏移.ReadOnly = true;
            this.Cut_高度偏移.Width = 101;
            // 
            // Cut_切割方向
            // 
            this.Cut_切割方向.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_切割方向.HeaderText = "切割方向(Type)";
            this.Cut_切割方向.Name = "Cut_切割方向";
            this.Cut_切割方向.ReadOnly = true;
            this.Cut_切割方向.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Cut_切割方向.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Cut_切割方向.Width = 131;
            // 
            // Cut_速度
            // 
            this.Cut_速度.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_速度.HeaderText = "速度(mm/s)";
            this.Cut_速度.Name = "Cut_速度";
            this.Cut_速度.ReadOnly = true;
            this.Cut_速度.Width = 115;
            // 
            // Cut_入板速度
            // 
            this.Cut_入板速度.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Cut_入板速度.HeaderText = "入板速度(mm/s)";
            this.Cut_入板速度.Name = "Cut_入板速度";
            this.Cut_入板速度.ReadOnly = true;
            this.Cut_入板速度.Width = 150;
            // 
            // CutLineSimulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.dCutProg);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnForward);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.tlpState);
            this.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "CutLineSimulate";
            this.Text = "CutLineSimulate";
            this.Load += new System.EventHandler(this.CutLineSimulate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dCutProg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpState;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnPause;
        private KCSDK.DDataGridView dCutProg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_Index;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Cut_Z1使用;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_Z1刀順;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Cut_Z2使用;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_Z2刀順;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_料片方向;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_高度偏移;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_切割方向;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_速度;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cut_入板速度;
    }
}