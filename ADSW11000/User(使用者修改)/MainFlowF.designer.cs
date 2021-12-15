namespace ADSW11000
{
    partial class MainFlowF
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
            this.TabFlow = new System.Windows.Forms.TabControl();
            this.tpHome = new System.Windows.Forms.TabPage();
            this.FC_MF歸零_動作結束 = new ProVLib.FlowChart();
            this.FC_MF歸零_等待SAW歸零完成 = new ProVLib.FlowChart();
            this.FC_MF歸零_SAW歸零 = new ProVLib.FlowChart();
            this.FC_MF歸零_動作開始 = new ProVLib.FlowChart();
            this.tpAuto = new System.Windows.Forms.TabPage();
            this.OptionDS = new KCSDK.DataManagement(this.components);
            this.TabFlow.SuspendLayout();
            this.tpHome.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabFlow
            // 
            this.TabFlow.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.TabFlow.Controls.Add(this.tpHome);
            this.TabFlow.Controls.Add(this.tpAuto);
            this.TabFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabFlow.Font = new System.Drawing.Font("微軟正黑體", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.TabFlow.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TabFlow.ItemSize = new System.Drawing.Size(220, 40);
            this.TabFlow.Location = new System.Drawing.Point(0, 0);
            this.TabFlow.Name = "TabFlow";
            this.TabFlow.SelectedIndex = 0;
            this.TabFlow.Size = new System.Drawing.Size(982, 551);
            this.TabFlow.TabIndex = 1;
            // 
            // tpHome
            // 
            this.tpHome.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpHome.Controls.Add(this.FC_MF歸零_動作結束);
            this.tpHome.Controls.Add(this.FC_MF歸零_等待SAW歸零完成);
            this.tpHome.Controls.Add(this.FC_MF歸零_SAW歸零);
            this.tpHome.Controls.Add(this.FC_MF歸零_動作開始);
            this.tpHome.Location = new System.Drawing.Point(4, 44);
            this.tpHome.Name = "tpHome";
            this.tpHome.Padding = new System.Windows.Forms.Padding(3);
            this.tpHome.Size = new System.Drawing.Size(974, 503);
            this.tpHome.TabIndex = 0;
            this.tpHome.Text = "歸零流程";
            // 
            // FC_MF歸零_動作結束
            // 
            this.FC_MF歸零_動作結束.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_MF歸零_動作結束.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_MF歸零_動作結束.CASE1 = null;
            this.FC_MF歸零_動作結束.CASE2 = null;
            this.FC_MF歸零_動作結束.CASE3 = null;
            this.FC_MF歸零_動作結束.CASE4 = null;
            this.FC_MF歸零_動作結束.ContinueRun = false;
            this.FC_MF歸零_動作結束.EndFC = null;
            this.FC_MF歸零_動作結束.ErrID = 0;
            this.FC_MF歸零_動作結束.InAlarm = false;
            this.FC_MF歸零_動作結束.IsFlowHead = false;
            this.FC_MF歸零_動作結束.Location = new System.Drawing.Point(77, 225);
            this.FC_MF歸零_動作結束.LockUI = false;
            this.FC_MF歸零_動作結束.Message = null;
            this.FC_MF歸零_動作結束.MsgID = 0;
            this.FC_MF歸零_動作結束.Name = "FC_MF歸零_動作結束";
            this.FC_MF歸零_動作結束.NEXT = null;
            this.FC_MF歸零_動作結束.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_MF歸零_動作結束.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_MF歸零_動作結束.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_MF歸零_動作結束.OverTimeSpec = 100;
            this.FC_MF歸零_動作結束.Running = false;
            this.FC_MF歸零_動作結束.Size = new System.Drawing.Size(200, 30);
            this.FC_MF歸零_動作結束.SlowRunCycle = -1;
            this.FC_MF歸零_動作結束.StartFC = null;
            this.FC_MF歸零_動作結束.Text = "FC_MF歸零_動作結束";
            this.FC_MF歸零_動作結束.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_MF歸零_動作結束_Run);
            // 
            // FC_MF歸零_等待SAW歸零完成
            // 
            this.FC_MF歸零_等待SAW歸零完成.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_MF歸零_等待SAW歸零完成.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_MF歸零_等待SAW歸零完成.CASE1 = null;
            this.FC_MF歸零_等待SAW歸零完成.CASE2 = null;
            this.FC_MF歸零_等待SAW歸零完成.CASE3 = null;
            this.FC_MF歸零_等待SAW歸零完成.CASE4 = null;
            this.FC_MF歸零_等待SAW歸零完成.ContinueRun = false;
            this.FC_MF歸零_等待SAW歸零完成.EndFC = null;
            this.FC_MF歸零_等待SAW歸零完成.ErrID = 0;
            this.FC_MF歸零_等待SAW歸零完成.InAlarm = false;
            this.FC_MF歸零_等待SAW歸零完成.IsFlowHead = false;
            this.FC_MF歸零_等待SAW歸零完成.Location = new System.Drawing.Point(77, 168);
            this.FC_MF歸零_等待SAW歸零完成.LockUI = false;
            this.FC_MF歸零_等待SAW歸零完成.Message = null;
            this.FC_MF歸零_等待SAW歸零完成.MsgID = 0;
            this.FC_MF歸零_等待SAW歸零完成.Name = "FC_MF歸零_等待SAW歸零完成";
            this.FC_MF歸零_等待SAW歸零完成.NEXT = this.FC_MF歸零_動作結束;
            this.FC_MF歸零_等待SAW歸零完成.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_MF歸零_等待SAW歸零完成.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_MF歸零_等待SAW歸零完成.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_MF歸零_等待SAW歸零完成.OverTimeSpec = 100;
            this.FC_MF歸零_等待SAW歸零完成.Running = false;
            this.FC_MF歸零_等待SAW歸零完成.Size = new System.Drawing.Size(200, 30);
            this.FC_MF歸零_等待SAW歸零完成.SlowRunCycle = -1;
            this.FC_MF歸零_等待SAW歸零完成.StartFC = null;
            this.FC_MF歸零_等待SAW歸零完成.Text = "FC_MF歸零_等待SAW歸零完成";
            this.FC_MF歸零_等待SAW歸零完成.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_MF歸零_等待SAW歸零完成_Run);
            // 
            // FC_MF歸零_SAW歸零
            // 
            this.FC_MF歸零_SAW歸零.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_MF歸零_SAW歸零.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_MF歸零_SAW歸零.CASE1 = null;
            this.FC_MF歸零_SAW歸零.CASE2 = null;
            this.FC_MF歸零_SAW歸零.CASE3 = null;
            this.FC_MF歸零_SAW歸零.CASE4 = null;
            this.FC_MF歸零_SAW歸零.ContinueRun = false;
            this.FC_MF歸零_SAW歸零.EndFC = null;
            this.FC_MF歸零_SAW歸零.ErrID = 0;
            this.FC_MF歸零_SAW歸零.InAlarm = false;
            this.FC_MF歸零_SAW歸零.IsFlowHead = false;
            this.FC_MF歸零_SAW歸零.Location = new System.Drawing.Point(77, 111);
            this.FC_MF歸零_SAW歸零.LockUI = false;
            this.FC_MF歸零_SAW歸零.Message = null;
            this.FC_MF歸零_SAW歸零.MsgID = 0;
            this.FC_MF歸零_SAW歸零.Name = "FC_MF歸零_SAW歸零";
            this.FC_MF歸零_SAW歸零.NEXT = this.FC_MF歸零_等待SAW歸零完成;
            this.FC_MF歸零_SAW歸零.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_MF歸零_SAW歸零.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_MF歸零_SAW歸零.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_MF歸零_SAW歸零.OverTimeSpec = 100;
            this.FC_MF歸零_SAW歸零.Running = false;
            this.FC_MF歸零_SAW歸零.Size = new System.Drawing.Size(200, 30);
            this.FC_MF歸零_SAW歸零.SlowRunCycle = -1;
            this.FC_MF歸零_SAW歸零.StartFC = null;
            this.FC_MF歸零_SAW歸零.Text = "FC_MF歸零_SAW歸零";
            this.FC_MF歸零_SAW歸零.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_MF歸零_SAW歸零_Run);
            // 
            // FC_MF歸零_動作開始
            // 
            this.FC_MF歸零_動作開始.BackColor = System.Drawing.Color.RoyalBlue;
            this.FC_MF歸零_動作開始.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.FC_MF歸零_動作開始.CASE1 = null;
            this.FC_MF歸零_動作開始.CASE2 = null;
            this.FC_MF歸零_動作開始.CASE3 = null;
            this.FC_MF歸零_動作開始.CASE4 = null;
            this.FC_MF歸零_動作開始.ContinueRun = false;
            this.FC_MF歸零_動作開始.EndFC = null;
            this.FC_MF歸零_動作開始.ErrID = 0;
            this.FC_MF歸零_動作開始.InAlarm = false;
            this.FC_MF歸零_動作開始.IsFlowHead = false;
            this.FC_MF歸零_動作開始.Location = new System.Drawing.Point(77, 54);
            this.FC_MF歸零_動作開始.LockUI = false;
            this.FC_MF歸零_動作開始.Message = null;
            this.FC_MF歸零_動作開始.MsgID = 0;
            this.FC_MF歸零_動作開始.Name = "FC_MF歸零_動作開始";
            this.FC_MF歸零_動作開始.NEXT = this.FC_MF歸零_SAW歸零;
            this.FC_MF歸零_動作開始.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.FC_MF歸零_動作開始.OrgLocation = new System.Drawing.Point(0, 0);
            this.FC_MF歸零_動作開始.OrgSize = new System.Drawing.Size(0, 0);
            this.FC_MF歸零_動作開始.OverTimeSpec = 100;
            this.FC_MF歸零_動作開始.Running = false;
            this.FC_MF歸零_動作開始.Size = new System.Drawing.Size(200, 30);
            this.FC_MF歸零_動作開始.SlowRunCycle = -1;
            this.FC_MF歸零_動作開始.StartFC = null;
            this.FC_MF歸零_動作開始.Text = "FC_MF歸零_動作開始";
            this.FC_MF歸零_動作開始.Run += new ProVLib.FlowChart.RunEventHandler(this.FC_MF歸零_動作開始_Run);
            // 
            // tpAuto
            // 
            this.tpAuto.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.tpAuto.Location = new System.Drawing.Point(4, 44);
            this.tpAuto.Name = "tpAuto";
            this.tpAuto.Padding = new System.Windows.Forms.Padding(3);
            this.tpAuto.Size = new System.Drawing.Size(974, 503);
            this.tpAuto.TabIndex = 1;
            this.tpAuto.Text = "自動流程";
            // 
            // OptionDS
            // 
            this.OptionDS.ModifiedLog = false;
            this.OptionDS.ModifiedLogToDB = true;
            // 
            // MainFlowF
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(982, 551);
            this.Controls.Add(this.TabFlow);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "MainFlowF";
            this.Text = "MainFlow";
            this.TabFlow.ResumeLayout(false);
            this.tpHome.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl TabFlow;
        public System.Windows.Forms.TabPage tpHome;
        public System.Windows.Forms.TabPage tpAuto;
        public KCSDK.DataManagement OptionDS;
        private ProVLib.FlowChart FC_MF歸零_動作結束;
        private ProVLib.FlowChart FC_MF歸零_等待SAW歸零完成;
        private ProVLib.FlowChart FC_MF歸零_SAW歸零;
        private ProVLib.FlowChart FC_MF歸零_動作開始;

    }
}