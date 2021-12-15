namespace MAA
{
    partial class MAA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MAA));
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.dFieldEdit_vacuum_off_volt = new KCSDK.DFieldEdit();
            this.dFieldEdit_vaccum_on_p = new KCSDK.DFieldEdit();
            this.dFieldEdit_vacuum_on_volt = new KCSDK.DFieldEdit();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dFieldEdit_back_touchpanel = new KCSDK.DFieldEdit();
            this.dFieldEdit_front_touchpanel = new KCSDK.DFieldEdit();
            this.dCheckBox_switch_mouse = new KCSDK.DCheckBox();
            this.tabControl_MAAIO = new System.Windows.Forms.TabControl();
            this.tabPage_MAA = new System.Windows.Forms.TabPage();
            this.outBit_SafeDoorNotify = new ProVLib.OutBit();
            this.OB_FrontScreen = new ProVLib.OutBit();
            this.OB_BackScreen = new ProVLib.OutBit();
            this.IB_BackScreen = new ProVLib.InBit();
            this.IB_FrontScreen = new ProVLib.InBit();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.EMGA_Back = new ProVLib.InBit();
            this.EMGA_Front = new ProVLib.InBit();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.SafeDoor_Back = new ProVLib.InBit();
            this.SafeDoor_Front = new ProVLib.InBit();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.analogIn_入力空壓偵測 = new ProVLib.AnalogIn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Music4 = new ProVLib.OutBit();
            this.Music3 = new ProVLib.OutBit();
            this.Music1 = new ProVLib.OutBit();
            this.Music2 = new ProVLib.OutBit();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.outBit_日光燈 = new ProVLib.OutBit();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.RedLight = new ProVLib.OutBit();
            this.YellowLight = new ProVLib.OutBit();
            this.GreenLight = new ProVLib.OutBit();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.OB_AlarmResetButton = new ProVLib.OutBit();
            this.OB_StopButton = new ProVLib.OutBit();
            this.OB_StartButton = new ProVLib.OutBit();
            this.IB_StartButton = new ProVLib.InBit();
            this.IB_StopButton = new ProVLib.InBit();
            this.IB_AlarmResetButton = new ProVLib.InBit();
            this.dCheckBox_SafeDoor = new KCSDK.DCheckBox();
            this.dCheckBox1 = new KCSDK.DCheckBox();
            this.dFieldEdit150 = new KCSDK.DFieldEdit();
            this.dFieldEdit151 = new KCSDK.DFieldEdit();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.dFieldEdit2 = new KCSDK.DFieldEdit();
            this.dFieldEdit3 = new KCSDK.DFieldEdit();
            this.tabMain.SuspendLayout();
            this.tpControl.SuspendLayout();
            this.tpSetting.SuspendLayout();
            this.tpFlow.SuspendLayout();
            this.tpSuperSetting.SuspendLayout();
            this.TabFlow.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.tabControl_MAAIO.SuspendLayout();
            this.tabPage_MAA.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox9.SuspendLayout();
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
            // 
            // tabMain
            // 
            this.tabMain.Size = new System.Drawing.Size(1028, 651);
            // 
            // tpControl
            // 
            this.tpControl.Controls.Add(this.tabControl_MAAIO);
            this.tpControl.Size = new System.Drawing.Size(1020, 583);
            // 
            // tpSetting
            // 
            this.tpSetting.Controls.Add(this.groupBox9);
            this.tpSetting.Controls.Add(this.dFieldEdit150);
            this.tpSetting.Size = new System.Drawing.Size(1020, 583);
            this.tpSetting.Controls.SetChildIndex(this.dFieldEdit150, 0);
            this.tpSetting.Controls.SetChildIndex(this.groupBox9, 0);
            // 
            // tpSuperSetting
            // 
            this.tpSuperSetting.Controls.Add(this.dFieldEdit151);
            this.tpSuperSetting.Controls.Add(this.dCheckBox1);
            this.tpSuperSetting.Controls.Add(this.dCheckBox_SafeDoor);
            this.tpSuperSetting.Controls.Add(this.dCheckBox_switch_mouse);
            this.tpSuperSetting.Controls.Add(this.groupBox7);
            this.tpSuperSetting.Controls.Add(this.groupBox11);
            this.tpSuperSetting.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tpSuperSetting.Size = new System.Drawing.Size(1020, 583);
            this.tpSuperSetting.Click += new System.EventHandler(this.tpSuperSetting_Click);
            this.tpSuperSetting.Controls.SetChildIndex(this.groupBox11, 0);
            this.tpSuperSetting.Controls.SetChildIndex(this.groupBox7, 0);
            this.tpSuperSetting.Controls.SetChildIndex(this.dCheckBox_switch_mouse, 0);
            this.tpSuperSetting.Controls.SetChildIndex(this.dCheckBox_SafeDoor, 0);
            this.tpSuperSetting.Controls.SetChildIndex(this.dCheckBox1, 0);
            this.tpSuperSetting.Controls.SetChildIndex(this.dFieldEdit151, 0);
            // 
            // tpAuto
            // 
            this.tpAuto.Location = new System.Drawing.Point(4, 38);
            this.tpAuto.Size = new System.Drawing.Size(985, 424);
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.dFieldEdit_vacuum_off_volt);
            this.groupBox11.Controls.Add(this.dFieldEdit_vaccum_on_p);
            this.groupBox11.Controls.Add(this.dFieldEdit_vacuum_on_volt);
            this.groupBox11.Location = new System.Drawing.Point(24, 78);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(349, 127);
            this.groupBox11.TabIndex = 47;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "入力空壓偵測";
            // 
            // dFieldEdit_vacuum_off_volt
            // 
            this.dFieldEdit_vacuum_off_volt.AutoFocus = false;
            this.dFieldEdit_vacuum_off_volt.Caption = "空壓關閉時電壓";
            this.dFieldEdit_vacuum_off_volt.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit_vacuum_off_volt.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_off_volt.DataName = "Main_Volt_PressOff";
            this.dFieldEdit_vacuum_off_volt.DataSource = this.SetDS;
            this.dFieldEdit_vacuum_off_volt.DefaultValue = null;
            this.dFieldEdit_vacuum_off_volt.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit_vacuum_off_volt.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit_vacuum_off_volt.EditWidth = 100;
            this.dFieldEdit_vacuum_off_volt.FieldValue = "0";
            this.dFieldEdit_vacuum_off_volt.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_off_volt.IsModified = false;
            this.dFieldEdit_vacuum_off_volt.Location = new System.Drawing.Point(12, 31);
            this.dFieldEdit_vacuum_off_volt.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit_vacuum_off_volt.MaxValue = 99999D;
            this.dFieldEdit_vacuum_off_volt.MinValue = 0D;
            this.dFieldEdit_vacuum_off_volt.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit_vacuum_off_volt.Name = "dFieldEdit_vacuum_off_volt";
            this.dFieldEdit_vacuum_off_volt.NoChangeInAuto = false;
            this.dFieldEdit_vacuum_off_volt.Size = new System.Drawing.Size(320, 29);
            this.dFieldEdit_vacuum_off_volt.StepValue = 0D;
            this.dFieldEdit_vacuum_off_volt.TabIndex = 41;
            this.dFieldEdit_vacuum_off_volt.Unit = "volt";
            this.dFieldEdit_vacuum_off_volt.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_off_volt.UnitWidth = 40;
            this.dFieldEdit_vacuum_off_volt.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit_vaccum_on_p
            // 
            this.dFieldEdit_vaccum_on_p.AutoFocus = false;
            this.dFieldEdit_vaccum_on_p.Caption = "空壓開啟時壓力";
            this.dFieldEdit_vaccum_on_p.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit_vaccum_on_p.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vaccum_on_p.DataName = "Main_KPa_PressOn";
            this.dFieldEdit_vaccum_on_p.DataSource = this.SetDS;
            this.dFieldEdit_vaccum_on_p.DefaultValue = null;
            this.dFieldEdit_vaccum_on_p.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit_vaccum_on_p.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit_vaccum_on_p.EditWidth = 100;
            this.dFieldEdit_vaccum_on_p.FieldValue = "0";
            this.dFieldEdit_vaccum_on_p.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vaccum_on_p.IsModified = false;
            this.dFieldEdit_vaccum_on_p.Location = new System.Drawing.Point(12, 89);
            this.dFieldEdit_vaccum_on_p.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit_vaccum_on_p.MaxValue = 1000D;
            this.dFieldEdit_vaccum_on_p.MinValue = -1000D;
            this.dFieldEdit_vaccum_on_p.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit_vaccum_on_p.Name = "dFieldEdit_vaccum_on_p";
            this.dFieldEdit_vaccum_on_p.NoChangeInAuto = false;
            this.dFieldEdit_vaccum_on_p.Size = new System.Drawing.Size(320, 29);
            this.dFieldEdit_vaccum_on_p.StepValue = 0D;
            this.dFieldEdit_vaccum_on_p.TabIndex = 43;
            this.dFieldEdit_vaccum_on_p.Unit = "MPa";
            this.dFieldEdit_vaccum_on_p.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vaccum_on_p.UnitWidth = 40;
            this.dFieldEdit_vaccum_on_p.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit_vacuum_on_volt
            // 
            this.dFieldEdit_vacuum_on_volt.AutoFocus = false;
            this.dFieldEdit_vacuum_on_volt.Caption = "空壓開啟時電壓";
            this.dFieldEdit_vacuum_on_volt.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit_vacuum_on_volt.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_on_volt.DataName = "Main_Volt_PressOn";
            this.dFieldEdit_vacuum_on_volt.DataSource = this.SetDS;
            this.dFieldEdit_vacuum_on_volt.DefaultValue = null;
            this.dFieldEdit_vacuum_on_volt.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit_vacuum_on_volt.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit_vacuum_on_volt.EditWidth = 100;
            this.dFieldEdit_vacuum_on_volt.FieldValue = "0";
            this.dFieldEdit_vacuum_on_volt.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_on_volt.IsModified = false;
            this.dFieldEdit_vacuum_on_volt.Location = new System.Drawing.Point(12, 60);
            this.dFieldEdit_vacuum_on_volt.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit_vacuum_on_volt.MaxValue = 99999D;
            this.dFieldEdit_vacuum_on_volt.MinValue = 0D;
            this.dFieldEdit_vacuum_on_volt.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit_vacuum_on_volt.Name = "dFieldEdit_vacuum_on_volt";
            this.dFieldEdit_vacuum_on_volt.NoChangeInAuto = false;
            this.dFieldEdit_vacuum_on_volt.Size = new System.Drawing.Size(320, 29);
            this.dFieldEdit_vacuum_on_volt.StepValue = 0D;
            this.dFieldEdit_vacuum_on_volt.TabIndex = 42;
            this.dFieldEdit_vacuum_on_volt.Unit = "volt";
            this.dFieldEdit_vacuum_on_volt.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_vacuum_on_volt.UnitWidth = 40;
            this.dFieldEdit_vacuum_on_volt.ValueType = KCSDK.ValueDataType.Double;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dFieldEdit_back_touchpanel);
            this.groupBox7.Controls.Add(this.dFieldEdit_front_touchpanel);
            this.groupBox7.Location = new System.Drawing.Point(24, 211);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(630, 116);
            this.groupBox7.TabIndex = 48;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "電腦輸入設備 HID";
            // 
            // dFieldEdit_back_touchpanel
            // 
            this.dFieldEdit_back_touchpanel.AutoFocus = false;
            this.dFieldEdit_back_touchpanel.Caption = "背面觸控螢幕";
            this.dFieldEdit_back_touchpanel.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit_back_touchpanel.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_back_touchpanel.DataName = "sHID_TeachB";
            this.dFieldEdit_back_touchpanel.DataSource = this.SetDS;
            this.dFieldEdit_back_touchpanel.DefaultValue = null;
            this.dFieldEdit_back_touchpanel.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit_back_touchpanel.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit_back_touchpanel.EditWidth = 450;
            this.dFieldEdit_back_touchpanel.FieldValue = "0";
            this.dFieldEdit_back_touchpanel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_back_touchpanel.IsModified = false;
            this.dFieldEdit_back_touchpanel.Location = new System.Drawing.Point(12, 74);
            this.dFieldEdit_back_touchpanel.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit_back_touchpanel.MaxValue = 1000D;
            this.dFieldEdit_back_touchpanel.MinValue = -1000D;
            this.dFieldEdit_back_touchpanel.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit_back_touchpanel.Name = "dFieldEdit_back_touchpanel";
            this.dFieldEdit_back_touchpanel.NoChangeInAuto = false;
            this.dFieldEdit_back_touchpanel.Size = new System.Drawing.Size(599, 29);
            this.dFieldEdit_back_touchpanel.StepValue = 0D;
            this.dFieldEdit_back_touchpanel.TabIndex = 44;
            this.dFieldEdit_back_touchpanel.Unit = "";
            this.dFieldEdit_back_touchpanel.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_back_touchpanel.UnitWidth = 0;
            this.dFieldEdit_back_touchpanel.ValueType = KCSDK.ValueDataType.String;
            // 
            // dFieldEdit_front_touchpanel
            // 
            this.dFieldEdit_front_touchpanel.AutoFocus = false;
            this.dFieldEdit_front_touchpanel.Caption = "正面觸控螢幕";
            this.dFieldEdit_front_touchpanel.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit_front_touchpanel.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_front_touchpanel.DataName = "sHID_TeachF";
            this.dFieldEdit_front_touchpanel.DataSource = this.SetDS;
            this.dFieldEdit_front_touchpanel.DefaultValue = null;
            this.dFieldEdit_front_touchpanel.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit_front_touchpanel.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit_front_touchpanel.EditWidth = 450;
            this.dFieldEdit_front_touchpanel.FieldValue = "0";
            this.dFieldEdit_front_touchpanel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_front_touchpanel.IsModified = false;
            this.dFieldEdit_front_touchpanel.Location = new System.Drawing.Point(12, 45);
            this.dFieldEdit_front_touchpanel.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit_front_touchpanel.MaxValue = 1000D;
            this.dFieldEdit_front_touchpanel.MinValue = -1000D;
            this.dFieldEdit_front_touchpanel.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit_front_touchpanel.Name = "dFieldEdit_front_touchpanel";
            this.dFieldEdit_front_touchpanel.NoChangeInAuto = false;
            this.dFieldEdit_front_touchpanel.Size = new System.Drawing.Size(599, 29);
            this.dFieldEdit_front_touchpanel.StepValue = 0D;
            this.dFieldEdit_front_touchpanel.TabIndex = 43;
            this.dFieldEdit_front_touchpanel.Unit = "";
            this.dFieldEdit_front_touchpanel.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit_front_touchpanel.UnitWidth = 0;
            this.dFieldEdit_front_touchpanel.ValueType = KCSDK.ValueDataType.String;
            // 
            // dCheckBox_switch_mouse
            // 
            this.dCheckBox_switch_mouse.AutoSize = true;
            this.dCheckBox_switch_mouse.DataName = "LockMouseKey";
            this.dCheckBox_switch_mouse.DataSource = this.SetDS;
            this.dCheckBox_switch_mouse.DefaultValue = false;
            this.dCheckBox_switch_mouse.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox_switch_mouse.IsModified = false;
            this.dCheckBox_switch_mouse.Location = new System.Drawing.Point(36, 333);
            this.dCheckBox_switch_mouse.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox_switch_mouse.Name = "dCheckBox_switch_mouse";
            this.dCheckBox_switch_mouse.NoChangeInAuto = false;
            this.dCheckBox_switch_mouse.Size = new System.Drawing.Size(172, 24);
            this.dCheckBox_switch_mouse.TabIndex = 146;
            this.dCheckBox_switch_mouse.Text = "滑鼠、鍵盤一併切換";
            this.dCheckBox_switch_mouse.UseVisualStyleBackColor = true;
            // 
            // tabControl_MAAIO
            // 
            this.tabControl_MAAIO.Controls.Add(this.tabPage_MAA);
            this.tabControl_MAAIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_MAAIO.Location = new System.Drawing.Point(0, 0);
            this.tabControl_MAAIO.Name = "tabControl_MAAIO";
            this.tabControl_MAAIO.SelectedIndex = 0;
            this.tabControl_MAAIO.Size = new System.Drawing.Size(1016, 579);
            this.tabControl_MAAIO.TabIndex = 2;
            // 
            // tabPage_MAA
            // 
            this.tabPage_MAA.Controls.Add(this.outBit_SafeDoorNotify);
            this.tabPage_MAA.Controls.Add(this.OB_FrontScreen);
            this.tabPage_MAA.Controls.Add(this.OB_BackScreen);
            this.tabPage_MAA.Controls.Add(this.IB_BackScreen);
            this.tabPage_MAA.Controls.Add(this.IB_FrontScreen);
            this.tabPage_MAA.Controls.Add(this.groupBox6);
            this.tabPage_MAA.Controls.Add(this.groupBox5);
            this.tabPage_MAA.Controls.Add(this.groupBox4);
            this.tabPage_MAA.Controls.Add(this.groupBox3);
            this.tabPage_MAA.Controls.Add(this.groupBox8);
            this.tabPage_MAA.Controls.Add(this.groupBox2);
            this.tabPage_MAA.Controls.Add(this.groupBox1);
            this.tabPage_MAA.Location = new System.Drawing.Point(4, 30);
            this.tabPage_MAA.Name = "tabPage_MAA";
            this.tabPage_MAA.Size = new System.Drawing.Size(1008, 545);
            this.tabPage_MAA.TabIndex = 0;
            this.tabPage_MAA.Text = "MAA";
            this.tabPage_MAA.UseVisualStyleBackColor = true;
            // 
            // outBit_SafeDoorNotify
            // 
            this.outBit_SafeDoorNotify.ActionCount = 0;
            this.outBit_SafeDoorNotify.BackColor = System.Drawing.Color.RoyalBlue;
            this.outBit_SafeDoorNotify.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.outBit_SafeDoorNotify.ErrID = 0;
            this.outBit_SafeDoorNotify.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.outBit_SafeDoorNotify.InAlarm = false;
            this.outBit_SafeDoorNotify.IOPort = "1206";
            this.outBit_SafeDoorNotify.IOType = ProVLib.EIOType.IOHSL;
            this.outBit_SafeDoorNotify.IsUseActionCount = false;
            this.outBit_SafeDoorNotify.Location = new System.Drawing.Point(705, 181);
            this.outBit_SafeDoorNotify.LockUI = false;
            this.outBit_SafeDoorNotify.Message = null;
            this.outBit_SafeDoorNotify.MsgID = 0;
            this.outBit_SafeDoorNotify.Name = "outBit_SafeDoorNotify";
            this.outBit_SafeDoorNotify.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.outBit_SafeDoorNotify.RetryCount = 10;
            this.outBit_SafeDoorNotify.Running = false;
            this.outBit_SafeDoorNotify.Size = new System.Drawing.Size(238, 30);
            this.outBit_SafeDoorNotify.Text = "通知Handler操作區安全門開啟";
            this.outBit_SafeDoorNotify.Value = false;
            // 
            // OB_FrontScreen
            // 
            this.OB_FrontScreen.ActionCount = 0;
            this.OB_FrontScreen.BackColor = System.Drawing.Color.RoyalBlue;
            this.OB_FrontScreen.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.OB_FrontScreen.ErrID = 0;
            this.OB_FrontScreen.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.OB_FrontScreen.InAlarm = false;
            this.OB_FrontScreen.IOPort = "";
            this.OB_FrontScreen.IOType = ProVLib.EIOType.IOHSL;
            this.OB_FrontScreen.IsUseActionCount = false;
            this.OB_FrontScreen.Location = new System.Drawing.Point(705, 96);
            this.OB_FrontScreen.LockUI = false;
            this.OB_FrontScreen.Message = null;
            this.OB_FrontScreen.MsgID = 0;
            this.OB_FrontScreen.Name = "OB_FrontScreen";
            this.OB_FrontScreen.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.OB_FrontScreen.RetryCount = 10;
            this.OB_FrontScreen.Running = false;
            this.OB_FrontScreen.Size = new System.Drawing.Size(238, 30);
            this.OB_FrontScreen.Text = "操作區螢幕控制按鈕燈";
            this.OB_FrontScreen.Value = false;
            // 
            // OB_BackScreen
            // 
            this.OB_BackScreen.ActionCount = 0;
            this.OB_BackScreen.BackColor = System.Drawing.Color.RoyalBlue;
            this.OB_BackScreen.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.OB_BackScreen.ErrID = 42;
            this.OB_BackScreen.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.OB_BackScreen.InAlarm = false;
            this.OB_BackScreen.IOPort = "";
            this.OB_BackScreen.IOType = ProVLib.EIOType.IOHSL;
            this.OB_BackScreen.IsUseActionCount = false;
            this.OB_BackScreen.Location = new System.Drawing.Point(705, 128);
            this.OB_BackScreen.LockUI = false;
            this.OB_BackScreen.Message = "Output Set Value Error";
            this.OB_BackScreen.MsgID = -6100;
            this.OB_BackScreen.Name = "OB_BackScreen";
            this.OB_BackScreen.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.OB_BackScreen.RetryCount = 10;
            this.OB_BackScreen.Running = false;
            this.OB_BackScreen.Size = new System.Drawing.Size(238, 30);
            this.OB_BackScreen.Text = "換刀區螢幕控制按鈕燈";
            this.OB_BackScreen.Value = false;
            // 
            // IB_BackScreen
            // 
            this.IB_BackScreen.BackColor = System.Drawing.Color.RoyalBlue;
            this.IB_BackScreen.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.IB_BackScreen.ErrID = 0;
            this.IB_BackScreen.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.IB_BackScreen.InAlarm = false;
            this.IB_BackScreen.IOPort = "";
            this.IB_BackScreen.IOType = ProVLib.EIOType.IOHSL;
            this.IB_BackScreen.Location = new System.Drawing.Point(705, 64);
            this.IB_BackScreen.LockUI = false;
            this.IB_BackScreen.Message = null;
            this.IB_BackScreen.MsgID = 0;
            this.IB_BackScreen.Name = "IB_BackScreen";
            this.IB_BackScreen.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.IB_BackScreen.Running = false;
            this.IB_BackScreen.Simu_Mode = ProVLib.SIMULATION_MODE.S_CONDITION;
            this.IB_BackScreen.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_BLINK;
            this.IB_BackScreen.Simu_OutPort1 = "2104";
            this.IB_BackScreen.Simu_OutPort2 = "2105";
            this.IB_BackScreen.Simu_RandomNum = 2;
            this.IB_BackScreen.Simu_RandomTime = 1000;
            this.IB_BackScreen.Simu_ReflectDelayTm = 100;
            this.IB_BackScreen.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.IB_BackScreen.Simu_Reverse = false;
            this.IB_BackScreen.Size = new System.Drawing.Size(238, 30);
            this.IB_BackScreen.Text = "換刀區螢幕控制按鈕";
            // 
            // IB_FrontScreen
            // 
            this.IB_FrontScreen.BackColor = System.Drawing.Color.RoyalBlue;
            this.IB_FrontScreen.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.IB_FrontScreen.ErrID = 0;
            this.IB_FrontScreen.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.IB_FrontScreen.InAlarm = false;
            this.IB_FrontScreen.IOPort = "";
            this.IB_FrontScreen.IOType = ProVLib.EIOType.IOHSL;
            this.IB_FrontScreen.Location = new System.Drawing.Point(705, 32);
            this.IB_FrontScreen.LockUI = false;
            this.IB_FrontScreen.Message = null;
            this.IB_FrontScreen.MsgID = 0;
            this.IB_FrontScreen.Name = "IB_FrontScreen";
            this.IB_FrontScreen.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.IB_FrontScreen.Running = false;
            this.IB_FrontScreen.Simu_Mode = ProVLib.SIMULATION_MODE.S_CONDITION;
            this.IB_FrontScreen.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_BLINK;
            this.IB_FrontScreen.Simu_OutPort1 = "2104";
            this.IB_FrontScreen.Simu_OutPort2 = "2105";
            this.IB_FrontScreen.Simu_RandomNum = 2;
            this.IB_FrontScreen.Simu_RandomTime = 1000;
            this.IB_FrontScreen.Simu_ReflectDelayTm = 100;
            this.IB_FrontScreen.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.IB_FrontScreen.Simu_Reverse = false;
            this.IB_FrontScreen.Size = new System.Drawing.Size(238, 30);
            this.IB_FrontScreen.Text = "操作區螢幕控制按鈕";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.EMGA_Back);
            this.groupBox6.Controls.Add(this.EMGA_Front);
            this.groupBox6.Location = new System.Drawing.Point(3, 245);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(216, 106);
            this.groupBox6.TabIndex = 25;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "緊急停止";
            // 
            // EMGA_Back
            // 
            this.EMGA_Back.BackColor = System.Drawing.Color.RoyalBlue;
            this.EMGA_Back.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.EMGA_Back.ErrID = 0;
            this.EMGA_Back.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.EMGA_Back.InAlarm = false;
            this.EMGA_Back.IOPort = "";
            this.EMGA_Back.IOType = ProVLib.EIOType.IOHSL;
            this.EMGA_Back.Location = new System.Drawing.Point(6, 61);
            this.EMGA_Back.LockUI = false;
            this.EMGA_Back.Message = null;
            this.EMGA_Back.MsgID = 0;
            this.EMGA_Back.Name = "EMGA_Back";
            this.EMGA_Back.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.EMGA_Back.Running = false;
            this.EMGA_Back.Simu_Mode = ProVLib.SIMULATION_MODE.S_RANDOM;
            this.EMGA_Back.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_KEEP;
            this.EMGA_Back.Simu_OutPort1 = null;
            this.EMGA_Back.Simu_OutPort2 = null;
            this.EMGA_Back.Simu_RandomNum = 2;
            this.EMGA_Back.Simu_RandomTime = 100;
            this.EMGA_Back.Simu_ReflectDelayTm = 100;
            this.EMGA_Back.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.EMGA_Back.Simu_Reverse = false;
            this.EMGA_Back.Size = new System.Drawing.Size(200, 30);
            this.EMGA_Back.Text = "緊急停止_換刀區(B)";
            // 
            // EMGA_Front
            // 
            this.EMGA_Front.BackColor = System.Drawing.Color.RoyalBlue;
            this.EMGA_Front.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.EMGA_Front.ErrID = 0;
            this.EMGA_Front.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.EMGA_Front.InAlarm = false;
            this.EMGA_Front.IOPort = "";
            this.EMGA_Front.IOType = ProVLib.EIOType.IOHSL;
            this.EMGA_Front.Location = new System.Drawing.Point(6, 29);
            this.EMGA_Front.LockUI = false;
            this.EMGA_Front.Message = null;
            this.EMGA_Front.MsgID = 0;
            this.EMGA_Front.Name = "EMGA_Front";
            this.EMGA_Front.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.EMGA_Front.Running = false;
            this.EMGA_Front.Simu_Mode = ProVLib.SIMULATION_MODE.S_RANDOM;
            this.EMGA_Front.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_KEEP;
            this.EMGA_Front.Simu_OutPort1 = null;
            this.EMGA_Front.Simu_OutPort2 = null;
            this.EMGA_Front.Simu_RandomNum = 0;
            this.EMGA_Front.Simu_RandomTime = 100;
            this.EMGA_Front.Simu_ReflectDelayTm = 100;
            this.EMGA_Front.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.EMGA_Front.Simu_Reverse = true;
            this.EMGA_Front.Size = new System.Drawing.Size(200, 30);
            this.EMGA_Front.Text = "緊急停止_操作區(B)";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.SafeDoor_Back);
            this.groupBox5.Controls.Add(this.SafeDoor_Front);
            this.groupBox5.Location = new System.Drawing.Point(472, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(215, 101);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "安全門";
            // 
            // SafeDoor_Back
            // 
            this.SafeDoor_Back.BackColor = System.Drawing.Color.RoyalBlue;
            this.SafeDoor_Back.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.SafeDoor_Back.ErrID = 0;
            this.SafeDoor_Back.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.SafeDoor_Back.InAlarm = false;
            this.SafeDoor_Back.IOPort = "";
            this.SafeDoor_Back.IOType = ProVLib.EIOType.IOHSL;
            this.SafeDoor_Back.Location = new System.Drawing.Point(6, 61);
            this.SafeDoor_Back.LockUI = false;
            this.SafeDoor_Back.Message = null;
            this.SafeDoor_Back.MsgID = 0;
            this.SafeDoor_Back.Name = "SafeDoor_Back";
            this.SafeDoor_Back.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.SafeDoor_Back.Running = false;
            this.SafeDoor_Back.Simu_Mode = ProVLib.SIMULATION_MODE.S_RANDOM;
            this.SafeDoor_Back.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_KEEP;
            this.SafeDoor_Back.Simu_OutPort1 = null;
            this.SafeDoor_Back.Simu_OutPort2 = null;
            this.SafeDoor_Back.Simu_RandomNum = 2;
            this.SafeDoor_Back.Simu_RandomTime = 100;
            this.SafeDoor_Back.Simu_ReflectDelayTm = 100;
            this.SafeDoor_Back.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.SafeDoor_Back.Simu_Reverse = false;
            this.SafeDoor_Back.Size = new System.Drawing.Size(200, 30);
            this.SafeDoor_Back.Text = "安全門_換刀區";
            // 
            // SafeDoor_Front
            // 
            this.SafeDoor_Front.BackColor = System.Drawing.Color.RoyalBlue;
            this.SafeDoor_Front.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.SafeDoor_Front.ErrID = 0;
            this.SafeDoor_Front.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.SafeDoor_Front.InAlarm = false;
            this.SafeDoor_Front.IOPort = "";
            this.SafeDoor_Front.IOType = ProVLib.EIOType.IOHSL;
            this.SafeDoor_Front.Location = new System.Drawing.Point(6, 29);
            this.SafeDoor_Front.LockUI = false;
            this.SafeDoor_Front.Message = null;
            this.SafeDoor_Front.MsgID = 0;
            this.SafeDoor_Front.Name = "SafeDoor_Front";
            this.SafeDoor_Front.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.SafeDoor_Front.Running = false;
            this.SafeDoor_Front.Simu_Mode = ProVLib.SIMULATION_MODE.S_RANDOM;
            this.SafeDoor_Front.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_KEEP;
            this.SafeDoor_Front.Simu_OutPort1 = null;
            this.SafeDoor_Front.Simu_OutPort2 = null;
            this.SafeDoor_Front.Simu_RandomNum = 2;
            this.SafeDoor_Front.Simu_RandomTime = 100;
            this.SafeDoor_Front.Simu_ReflectDelayTm = 100;
            this.SafeDoor_Front.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.SafeDoor_Front.Simu_Reverse = false;
            this.SafeDoor_Front.Size = new System.Drawing.Size(200, 30);
            this.SafeDoor_Front.Text = "安全門_操作區";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.analogIn_入力空壓偵測);
            this.groupBox4.Location = new System.Drawing.Point(237, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(217, 76);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "其他";
            // 
            // analogIn_入力空壓偵測
            // 
            this.analogIn_入力空壓偵測.AIRange = ((ushort)(0));
            this.analogIn_入力空壓偵測.BackColor = System.Drawing.Color.RoyalBlue;
            this.analogIn_入力空壓偵測.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.analogIn_入力空壓偵測.ConvertFactor = 1F;
            this.analogIn_入力空壓偵測.ErrID = 0;
            this.analogIn_入力空壓偵測.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.analogIn_入力空壓偵測.InAlarm = false;
            this.analogIn_入力空壓偵測.IOPort = null;
            this.analogIn_入力空壓偵測.IOType = ProVLib.EIOType.IOHSL;
            this.analogIn_入力空壓偵測.IsUpToDate = false;
            this.analogIn_入力空壓偵測.Location = new System.Drawing.Point(6, 29);
            this.analogIn_入力空壓偵測.LockUI = false;
            this.analogIn_入力空壓偵測.Message = null;
            this.analogIn_入力空壓偵測.MsgID = 0;
            this.analogIn_入力空壓偵測.Name = "analogIn_入力空壓偵測";
            this.analogIn_入力空壓偵測.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.analogIn_入力空壓偵測.Running = false;
            this.analogIn_入力空壓偵測.ShowAvgVal = true;
            this.analogIn_入力空壓偵測.ShowRealVal = true;
            this.analogIn_入力空壓偵測.Size = new System.Drawing.Size(200, 30);
            this.analogIn_入力空壓偵測.Text = "入力空壓(1-5V)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Music4);
            this.groupBox3.Controls.Add(this.Music3);
            this.groupBox3.Controls.Add(this.Music1);
            this.groupBox3.Controls.Add(this.Music2);
            this.groupBox3.Location = new System.Drawing.Point(237, 96);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(217, 164);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "系統音樂";
            // 
            // Music4
            // 
            this.Music4.ActionCount = 0;
            this.Music4.BackColor = System.Drawing.Color.RoyalBlue;
            this.Music4.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.Music4.Enabled = false;
            this.Music4.ErrID = 42;
            this.Music4.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.Music4.InAlarm = false;
            this.Music4.IOPort = "";
            this.Music4.IOType = ProVLib.EIOType.IOHSL;
            this.Music4.IsUseActionCount = false;
            this.Music4.Location = new System.Drawing.Point(6, 123);
            this.Music4.LockUI = false;
            this.Music4.Message = "Output Set Value Error";
            this.Music4.MsgID = -6100;
            this.Music4.Name = "Music4";
            this.Music4.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.Music4.RetryCount = 10;
            this.Music4.Running = false;
            this.Music4.Size = new System.Drawing.Size(200, 30);
            this.Music4.Text = "音樂 - 4";
            this.Music4.Value = false;
            // 
            // Music3
            // 
            this.Music3.ActionCount = 0;
            this.Music3.BackColor = System.Drawing.Color.RoyalBlue;
            this.Music3.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.Music3.Enabled = false;
            this.Music3.ErrID = 42;
            this.Music3.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.Music3.InAlarm = false;
            this.Music3.IOPort = "";
            this.Music3.IOType = ProVLib.EIOType.IOHSL;
            this.Music3.IsUseActionCount = false;
            this.Music3.Location = new System.Drawing.Point(6, 91);
            this.Music3.LockUI = false;
            this.Music3.Message = "Output Set Value Error";
            this.Music3.MsgID = -6100;
            this.Music3.Name = "Music3";
            this.Music3.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.Music3.RetryCount = 10;
            this.Music3.Running = false;
            this.Music3.Size = new System.Drawing.Size(200, 30);
            this.Music3.Text = "音樂 - 3";
            this.Music3.Value = false;
            // 
            // Music1
            // 
            this.Music1.ActionCount = 0;
            this.Music1.BackColor = System.Drawing.Color.RoyalBlue;
            this.Music1.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.Music1.ErrID = 0;
            this.Music1.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.Music1.InAlarm = false;
            this.Music1.IOPort = "";
            this.Music1.IOType = ProVLib.EIOType.IOHSL;
            this.Music1.IsUseActionCount = false;
            this.Music1.Location = new System.Drawing.Point(6, 27);
            this.Music1.LockUI = false;
            this.Music1.Message = null;
            this.Music1.MsgID = 0;
            this.Music1.Name = "Music1";
            this.Music1.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.Music1.RetryCount = 10;
            this.Music1.Running = false;
            this.Music1.Size = new System.Drawing.Size(200, 30);
            this.Music1.Text = "音樂 - 1";
            this.Music1.Value = false;
            // 
            // Music2
            // 
            this.Music2.ActionCount = 0;
            this.Music2.BackColor = System.Drawing.Color.RoyalBlue;
            this.Music2.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.Music2.Enabled = false;
            this.Music2.ErrID = 42;
            this.Music2.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.Music2.InAlarm = false;
            this.Music2.IOPort = "";
            this.Music2.IOType = ProVLib.EIOType.IOHSL;
            this.Music2.IsUseActionCount = false;
            this.Music2.Location = new System.Drawing.Point(6, 59);
            this.Music2.LockUI = false;
            this.Music2.Message = "Output Set Value Error";
            this.Music2.MsgID = -6100;
            this.Music2.Name = "Music2";
            this.Music2.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.Music2.RetryCount = 10;
            this.Music2.Running = false;
            this.Music2.Size = new System.Drawing.Size(200, 30);
            this.Music2.Text = "音樂 - 2";
            this.Music2.Value = false;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.outBit_日光燈);
            this.groupBox8.Location = new System.Drawing.Point(472, 120);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(215, 75);
            this.groupBox8.TabIndex = 20;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "日光燈";
            // 
            // outBit_日光燈
            // 
            this.outBit_日光燈.ActionCount = 0;
            this.outBit_日光燈.BackColor = System.Drawing.Color.RoyalBlue;
            this.outBit_日光燈.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.outBit_日光燈.ErrID = 0;
            this.outBit_日光燈.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.outBit_日光燈.InAlarm = false;
            this.outBit_日光燈.IOPort = "";
            this.outBit_日光燈.IOType = ProVLib.EIOType.IOHSL;
            this.outBit_日光燈.IsUseActionCount = true;
            this.outBit_日光燈.Location = new System.Drawing.Point(8, 29);
            this.outBit_日光燈.LockUI = false;
            this.outBit_日光燈.Message = null;
            this.outBit_日光燈.MsgID = 0;
            this.outBit_日光燈.Name = "outBit_日光燈";
            this.outBit_日光燈.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.outBit_日光燈.RetryCount = 10;
            this.outBit_日光燈.Running = false;
            this.outBit_日光燈.Size = new System.Drawing.Size(198, 30);
            this.outBit_日光燈.Text = "日光燈";
            this.outBit_日光燈.Value = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RedLight);
            this.groupBox2.Controls.Add(this.YellowLight);
            this.groupBox2.Controls.Add(this.GreenLight);
            this.groupBox2.Location = new System.Drawing.Point(471, 211);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 142);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "警示燈";
            // 
            // RedLight
            // 
            this.RedLight.ActionCount = 0;
            this.RedLight.BackColor = System.Drawing.Color.RoyalBlue;
            this.RedLight.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.RedLight.ErrID = 0;
            this.RedLight.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.RedLight.InAlarm = false;
            this.RedLight.IOPort = "";
            this.RedLight.IOType = ProVLib.EIOType.IOHSL;
            this.RedLight.IsUseActionCount = false;
            this.RedLight.Location = new System.Drawing.Point(6, 93);
            this.RedLight.LockUI = false;
            this.RedLight.Message = null;
            this.RedLight.MsgID = 0;
            this.RedLight.Name = "RedLight";
            this.RedLight.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.RedLight.RetryCount = 10;
            this.RedLight.Running = false;
            this.RedLight.Size = new System.Drawing.Size(200, 30);
            this.RedLight.Text = "紅色訊號燈";
            this.RedLight.Value = false;
            // 
            // YellowLight
            // 
            this.YellowLight.ActionCount = 0;
            this.YellowLight.BackColor = System.Drawing.Color.RoyalBlue;
            this.YellowLight.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.YellowLight.ErrID = 0;
            this.YellowLight.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.YellowLight.InAlarm = false;
            this.YellowLight.IOPort = "";
            this.YellowLight.IOType = ProVLib.EIOType.IOHSL;
            this.YellowLight.IsUseActionCount = false;
            this.YellowLight.Location = new System.Drawing.Point(6, 61);
            this.YellowLight.LockUI = false;
            this.YellowLight.Message = null;
            this.YellowLight.MsgID = 0;
            this.YellowLight.Name = "YellowLight";
            this.YellowLight.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.YellowLight.RetryCount = 10;
            this.YellowLight.Running = false;
            this.YellowLight.Size = new System.Drawing.Size(200, 30);
            this.YellowLight.Text = "黃色訊號燈";
            this.YellowLight.Value = false;
            // 
            // GreenLight
            // 
            this.GreenLight.ActionCount = 0;
            this.GreenLight.BackColor = System.Drawing.Color.RoyalBlue;
            this.GreenLight.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.GreenLight.ErrID = 0;
            this.GreenLight.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.GreenLight.InAlarm = false;
            this.GreenLight.IOPort = "";
            this.GreenLight.IOType = ProVLib.EIOType.IOHSL;
            this.GreenLight.IsUseActionCount = false;
            this.GreenLight.Location = new System.Drawing.Point(6, 29);
            this.GreenLight.LockUI = false;
            this.GreenLight.Message = null;
            this.GreenLight.MsgID = 0;
            this.GreenLight.Name = "GreenLight";
            this.GreenLight.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.GreenLight.RetryCount = 10;
            this.GreenLight.Running = false;
            this.GreenLight.Size = new System.Drawing.Size(200, 30);
            this.GreenLight.Text = "綠色訊號燈";
            this.GreenLight.Value = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OB_AlarmResetButton);
            this.groupBox1.Controls.Add(this.OB_StopButton);
            this.groupBox1.Controls.Add(this.OB_StartButton);
            this.groupBox1.Controls.Add(this.IB_StartButton);
            this.groupBox1.Controls.Add(this.IB_StopButton);
            this.groupBox1.Controls.Add(this.IB_AlarmResetButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 236);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "機台按鈕";
            // 
            // OB_AlarmResetButton
            // 
            this.OB_AlarmResetButton.ActionCount = 0;
            this.OB_AlarmResetButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.OB_AlarmResetButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.OB_AlarmResetButton.ErrID = 0;
            this.OB_AlarmResetButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.OB_AlarmResetButton.InAlarm = false;
            this.OB_AlarmResetButton.IOPort = "";
            this.OB_AlarmResetButton.IOType = ProVLib.EIOType.IOHSL;
            this.OB_AlarmResetButton.IsUseActionCount = false;
            this.OB_AlarmResetButton.Location = new System.Drawing.Point(6, 93);
            this.OB_AlarmResetButton.LockUI = false;
            this.OB_AlarmResetButton.Message = null;
            this.OB_AlarmResetButton.MsgID = 0;
            this.OB_AlarmResetButton.Name = "OB_AlarmResetButton";
            this.OB_AlarmResetButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.OB_AlarmResetButton.RetryCount = 10;
            this.OB_AlarmResetButton.Running = false;
            this.OB_AlarmResetButton.Size = new System.Drawing.Size(200, 30);
            this.OB_AlarmResetButton.Text = "Alarm Reset指示燈";
            this.OB_AlarmResetButton.Value = false;
            // 
            // OB_StopButton
            // 
            this.OB_StopButton.ActionCount = 0;
            this.OB_StopButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.OB_StopButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.OB_StopButton.ErrID = 0;
            this.OB_StopButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.OB_StopButton.InAlarm = false;
            this.OB_StopButton.IOPort = "";
            this.OB_StopButton.IOType = ProVLib.EIOType.IOHSL;
            this.OB_StopButton.IsUseActionCount = false;
            this.OB_StopButton.Location = new System.Drawing.Point(6, 61);
            this.OB_StopButton.LockUI = false;
            this.OB_StopButton.Message = null;
            this.OB_StopButton.MsgID = 0;
            this.OB_StopButton.Name = "OB_StopButton";
            this.OB_StopButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.OB_StopButton.RetryCount = 10;
            this.OB_StopButton.Running = false;
            this.OB_StopButton.Size = new System.Drawing.Size(200, 30);
            this.OB_StopButton.Text = "停止指示燈";
            this.OB_StopButton.Value = false;
            // 
            // OB_StartButton
            // 
            this.OB_StartButton.ActionCount = 0;
            this.OB_StartButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.OB_StartButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.OB_StartButton.ErrID = 0;
            this.OB_StartButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.OB_StartButton.InAlarm = false;
            this.OB_StartButton.IOPort = "";
            this.OB_StartButton.IOType = ProVLib.EIOType.IOHSL;
            this.OB_StartButton.IsUseActionCount = false;
            this.OB_StartButton.Location = new System.Drawing.Point(6, 29);
            this.OB_StartButton.LockUI = false;
            this.OB_StartButton.Message = null;
            this.OB_StartButton.MsgID = 0;
            this.OB_StartButton.Name = "OB_StartButton";
            this.OB_StartButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.OB_StartButton.RetryCount = 10;
            this.OB_StartButton.Running = false;
            this.OB_StartButton.Size = new System.Drawing.Size(200, 30);
            this.OB_StartButton.Text = "啟動指示燈";
            this.OB_StartButton.Value = false;
            // 
            // IB_StartButton
            // 
            this.IB_StartButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.IB_StartButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.IB_StartButton.ErrID = 0;
            this.IB_StartButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.IB_StartButton.InAlarm = false;
            this.IB_StartButton.IOPort = "";
            this.IB_StartButton.IOType = ProVLib.EIOType.IOHSL;
            this.IB_StartButton.Location = new System.Drawing.Point(6, 131);
            this.IB_StartButton.LockUI = false;
            this.IB_StartButton.Message = null;
            this.IB_StartButton.MsgID = 0;
            this.IB_StartButton.Name = "IB_StartButton";
            this.IB_StartButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.IB_StartButton.Running = false;
            this.IB_StartButton.Simu_Mode = ProVLib.SIMULATION_MODE.S_CONDITION;
            this.IB_StartButton.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_BLINK;
            this.IB_StartButton.Simu_OutPort1 = "2100";
            this.IB_StartButton.Simu_OutPort2 = null;
            this.IB_StartButton.Simu_RandomNum = 0;
            this.IB_StartButton.Simu_RandomTime = 100;
            this.IB_StartButton.Simu_ReflectDelayTm = 10;
            this.IB_StartButton.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.IB_StartButton.Simu_Reverse = false;
            this.IB_StartButton.Size = new System.Drawing.Size(200, 30);
            this.IB_StartButton.Text = "啟動鈕偵測";
            // 
            // IB_StopButton
            // 
            this.IB_StopButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.IB_StopButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.IB_StopButton.ErrID = 0;
            this.IB_StopButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.IB_StopButton.InAlarm = false;
            this.IB_StopButton.IOPort = "";
            this.IB_StopButton.IOType = ProVLib.EIOType.IOHSL;
            this.IB_StopButton.Location = new System.Drawing.Point(6, 163);
            this.IB_StopButton.LockUI = false;
            this.IB_StopButton.Message = null;
            this.IB_StopButton.MsgID = 0;
            this.IB_StopButton.Name = "IB_StopButton";
            this.IB_StopButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.IB_StopButton.Running = false;
            this.IB_StopButton.Simu_Mode = ProVLib.SIMULATION_MODE.S_CONDITION;
            this.IB_StopButton.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_BLINK;
            this.IB_StopButton.Simu_OutPort1 = "2101";
            this.IB_StopButton.Simu_OutPort2 = null;
            this.IB_StopButton.Simu_RandomNum = 0;
            this.IB_StopButton.Simu_RandomTime = 100;
            this.IB_StopButton.Simu_ReflectDelayTm = 10;
            this.IB_StopButton.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.IB_StopButton.Simu_Reverse = false;
            this.IB_StopButton.Size = new System.Drawing.Size(200, 30);
            this.IB_StopButton.Text = "停止鈕偵測";
            // 
            // IB_AlarmResetButton
            // 
            this.IB_AlarmResetButton.BackColor = System.Drawing.Color.RoyalBlue;
            this.IB_AlarmResetButton.CaptionFont = new System.Drawing.Font("微軟正黑體", 10F);
            this.IB_AlarmResetButton.ErrID = 0;
            this.IB_AlarmResetButton.HSLSpeed = ProVLib.EHSLSPEED.HSL6M;
            this.IB_AlarmResetButton.InAlarm = false;
            this.IB_AlarmResetButton.IOPort = "";
            this.IB_AlarmResetButton.IOType = ProVLib.EIOType.IOHSL;
            this.IB_AlarmResetButton.Location = new System.Drawing.Point(6, 195);
            this.IB_AlarmResetButton.LockUI = false;
            this.IB_AlarmResetButton.Message = null;
            this.IB_AlarmResetButton.MsgID = 0;
            this.IB_AlarmResetButton.Name = "IB_AlarmResetButton";
            this.IB_AlarmResetButton.ObjType = ProVLib.EObjType.VOID_TYPE;
            this.IB_AlarmResetButton.Running = false;
            this.IB_AlarmResetButton.Simu_Mode = ProVLib.SIMULATION_MODE.S_CONDITION;
            this.IB_AlarmResetButton.Simu_OnOffCondition = ProVLib.SIMULATION_ONOFFCONDITION.SRR_BLINK;
            this.IB_AlarmResetButton.Simu_OutPort1 = "2102";
            this.IB_AlarmResetButton.Simu_OutPort2 = null;
            this.IB_AlarmResetButton.Simu_RandomNum = 0;
            this.IB_AlarmResetButton.Simu_RandomTime = 100;
            this.IB_AlarmResetButton.Simu_ReflectDelayTm = 10;
            this.IB_AlarmResetButton.Simu_ReflectRule = ProVLib.SIMULATION_REFLECTRULE.SRR_ON_OFF;
            this.IB_AlarmResetButton.Simu_Reverse = false;
            this.IB_AlarmResetButton.Size = new System.Drawing.Size(200, 30);
            this.IB_AlarmResetButton.Text = "AlarmReset鈕偵測";
            // 
            // dCheckBox_SafeDoor
            // 
            this.dCheckBox_SafeDoor.AutoSize = true;
            this.dCheckBox_SafeDoor.DataName = "安全門不偵測";
            this.dCheckBox_SafeDoor.DataSource = this.SetDS;
            this.dCheckBox_SafeDoor.DefaultValue = false;
            this.dCheckBox_SafeDoor.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox_SafeDoor.IsModified = false;
            this.dCheckBox_SafeDoor.Location = new System.Drawing.Point(399, 78);
            this.dCheckBox_SafeDoor.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox_SafeDoor.Name = "dCheckBox_SafeDoor";
            this.dCheckBox_SafeDoor.NoChangeInAuto = false;
            this.dCheckBox_SafeDoor.Size = new System.Drawing.Size(124, 24);
            this.dCheckBox_SafeDoor.TabIndex = 147;
            this.dCheckBox_SafeDoor.Text = "安全門不偵測";
            this.dCheckBox_SafeDoor.UseVisualStyleBackColor = true;
            // 
            // dCheckBox1
            // 
            this.dCheckBox1.AutoSize = true;
            this.dCheckBox1.DataName = "空壓偵測";
            this.dCheckBox1.DataSource = this.SetDS;
            this.dCheckBox1.DefaultValue = false;
            this.dCheckBox1.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dCheckBox1.IsModified = false;
            this.dCheckBox1.Location = new System.Drawing.Point(399, 114);
            this.dCheckBox1.ModifiedColor = System.Drawing.Color.Aqua;
            this.dCheckBox1.Name = "dCheckBox1";
            this.dCheckBox1.NoChangeInAuto = false;
            this.dCheckBox1.Size = new System.Drawing.Size(92, 24);
            this.dCheckBox1.TabIndex = 148;
            this.dCheckBox1.Text = "空壓偵測";
            this.dCheckBox1.UseVisualStyleBackColor = true;
            // 
            // dFieldEdit150
            // 
            this.dFieldEdit150.AutoFocus = false;
            this.dFieldEdit150.Caption = "工程師密碼";
            this.dFieldEdit150.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit150.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit150.DataName = "EngineerPassword";
            this.dFieldEdit150.DataSource = this.SetDS;
            this.dFieldEdit150.DefaultValue = null;
            this.dFieldEdit150.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit150.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit150.EditWidth = 180;
            this.dFieldEdit150.FieldValue = "";
            this.dFieldEdit150.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit150.IsModified = false;
            this.dFieldEdit150.Location = new System.Drawing.Point(26, 76);
            this.dFieldEdit150.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit150.MaxValue = 0D;
            this.dFieldEdit150.MinValue = 0D;
            this.dFieldEdit150.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit150.Name = "dFieldEdit150";
            this.dFieldEdit150.NoChangeInAuto = false;
            this.dFieldEdit150.Size = new System.Drawing.Size(281, 29);
            this.dFieldEdit150.StepValue = 0D;
            this.dFieldEdit150.TabIndex = 211;
            this.dFieldEdit150.Tag = "visionName";
            this.dFieldEdit150.Unit = "";
            this.dFieldEdit150.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit150.UnitWidth = 0;
            this.dFieldEdit150.ValueType = KCSDK.ValueDataType.String;
            // 
            // dFieldEdit151
            // 
            this.dFieldEdit151.AutoFocus = false;
            this.dFieldEdit151.Caption = "管理員密碼";
            this.dFieldEdit151.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit151.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit151.DataName = "AdministratorPassword";
            this.dFieldEdit151.DataSource = this.SetDS;
            this.dFieldEdit151.DefaultValue = null;
            this.dFieldEdit151.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit151.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit151.EditWidth = 180;
            this.dFieldEdit151.FieldValue = "";
            this.dFieldEdit151.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit151.IsModified = false;
            this.dFieldEdit151.Location = new System.Drawing.Point(24, 376);
            this.dFieldEdit151.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit151.MaxValue = 0D;
            this.dFieldEdit151.MinValue = 0D;
            this.dFieldEdit151.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit151.Name = "dFieldEdit151";
            this.dFieldEdit151.NoChangeInAuto = false;
            this.dFieldEdit151.Size = new System.Drawing.Size(281, 29);
            this.dFieldEdit151.StepValue = 0D;
            this.dFieldEdit151.TabIndex = 212;
            this.dFieldEdit151.Tag = "visionName";
            this.dFieldEdit151.Unit = "";
            this.dFieldEdit151.UnitFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit151.UnitWidth = 0;
            this.dFieldEdit151.ValueType = KCSDK.ValueDataType.String;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.dFieldEdit2);
            this.groupBox9.Controls.Add(this.dFieldEdit3);
            this.groupBox9.Location = new System.Drawing.Point(14, 123);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(349, 101);
            this.groupBox9.TabIndex = 212;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "入力空壓閥值";
            // 
            // dFieldEdit2
            // 
            this.dFieldEdit2.AutoFocus = false;
            this.dFieldEdit2.Caption = "空壓最大閥值";
            this.dFieldEdit2.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit2.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.DataName = "Main_AirPressure_Max";
            this.dFieldEdit2.DataSource = this.SetDS;
            this.dFieldEdit2.DefaultValue = "0.7";
            this.dFieldEdit2.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit2.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit2.EditWidth = 100;
            this.dFieldEdit2.FieldValue = "0";
            this.dFieldEdit2.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.IsModified = false;
            this.dFieldEdit2.Location = new System.Drawing.Point(12, 62);
            this.dFieldEdit2.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit2.MaxValue = 1D;
            this.dFieldEdit2.MinValue = 0D;
            this.dFieldEdit2.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit2.Name = "dFieldEdit2";
            this.dFieldEdit2.NoChangeInAuto = false;
            this.dFieldEdit2.Size = new System.Drawing.Size(320, 29);
            this.dFieldEdit2.StepValue = 0D;
            this.dFieldEdit2.TabIndex = 43;
            this.dFieldEdit2.Unit = "MPa";
            this.dFieldEdit2.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit2.UnitWidth = 40;
            this.dFieldEdit2.ValueType = KCSDK.ValueDataType.Double;
            // 
            // dFieldEdit3
            // 
            this.dFieldEdit3.AutoFocus = false;
            this.dFieldEdit3.Caption = "空壓最小閥值";
            this.dFieldEdit3.CaptionColor = System.Drawing.Color.Black;
            this.dFieldEdit3.CaptionFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.DataName = "Main_AirPressure_Min";
            this.dFieldEdit3.DataSource = this.SetDS;
            this.dFieldEdit3.DefaultValue = "0.55";
            this.dFieldEdit3.EditColor = System.Drawing.Color.Black;
            this.dFieldEdit3.EditFont = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Bold);
            this.dFieldEdit3.EditWidth = 100;
            this.dFieldEdit3.FieldValue = "0";
            this.dFieldEdit3.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.IsModified = false;
            this.dFieldEdit3.Location = new System.Drawing.Point(12, 33);
            this.dFieldEdit3.Margin = new System.Windows.Forms.Padding(0);
            this.dFieldEdit3.MaxValue = 1D;
            this.dFieldEdit3.MinValue = 0D;
            this.dFieldEdit3.ModifiedColor = System.Drawing.Color.Aqua;
            this.dFieldEdit3.Name = "dFieldEdit3";
            this.dFieldEdit3.NoChangeInAuto = false;
            this.dFieldEdit3.Size = new System.Drawing.Size(320, 29);
            this.dFieldEdit3.StepValue = 0D;
            this.dFieldEdit3.TabIndex = 42;
            this.dFieldEdit3.Unit = "MPa";
            this.dFieldEdit3.UnitFont = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.dFieldEdit3.UnitWidth = 40;
            this.dFieldEdit3.ValueType = KCSDK.ValueDataType.Double;
            // 
            // MAA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 651);
            this.Name = "MAA";
            this.Text = "MAA";
            this.tabMain.ResumeLayout(false);
            this.tpControl.ResumeLayout(false);
            this.tpSetting.ResumeLayout(false);
            this.tpFlow.ResumeLayout(false);
            this.tpSuperSetting.ResumeLayout(false);
            this.tpSuperSetting.PerformLayout();
            this.TabFlow.ResumeLayout(false);
            this.groupBox11.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.tabControl_MAAIO.ResumeLayout(false);
            this.tabPage_MAA.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox11;
        private KCSDK.DFieldEdit dFieldEdit_vacuum_off_volt;
        private KCSDK.DFieldEdit dFieldEdit_vaccum_on_p;
        private KCSDK.DFieldEdit dFieldEdit_vacuum_on_volt;
        private System.Windows.Forms.GroupBox groupBox7;
        private KCSDK.DFieldEdit dFieldEdit_back_touchpanel;
        private KCSDK.DFieldEdit dFieldEdit_front_touchpanel;
        private KCSDK.DCheckBox dCheckBox_switch_mouse;
        private System.Windows.Forms.TabControl tabControl_MAAIO;
        private KCSDK.DCheckBox dCheckBox_SafeDoor;
        private System.Windows.Forms.TabPage tabPage_MAA;
        private ProVLib.OutBit outBit_SafeDoorNotify;
        private ProVLib.OutBit OB_FrontScreen;
        public ProVLib.OutBit OB_BackScreen;
        private ProVLib.InBit IB_BackScreen;
        private ProVLib.InBit IB_FrontScreen;
        private System.Windows.Forms.GroupBox groupBox6;
        private ProVLib.InBit EMGA_Back;
        private ProVLib.InBit EMGA_Front;
        private System.Windows.Forms.GroupBox groupBox5;
        private ProVLib.InBit SafeDoor_Back;
        private ProVLib.InBit SafeDoor_Front;
        private System.Windows.Forms.GroupBox groupBox4;
        private ProVLib.AnalogIn analogIn_入力空壓偵測;
        private System.Windows.Forms.GroupBox groupBox3;
        public ProVLib.OutBit Music4;
        public ProVLib.OutBit Music3;
        private ProVLib.OutBit Music1;
        public ProVLib.OutBit Music2;
        private System.Windows.Forms.GroupBox groupBox8;
        private ProVLib.OutBit outBit_日光燈;
        private System.Windows.Forms.GroupBox groupBox2;
        private ProVLib.OutBit RedLight;
        private ProVLib.OutBit YellowLight;
        private ProVLib.OutBit GreenLight;
        private System.Windows.Forms.GroupBox groupBox1;
        private ProVLib.OutBit OB_AlarmResetButton;
        private ProVLib.OutBit OB_StopButton;
        private ProVLib.OutBit OB_StartButton;
        private ProVLib.InBit IB_StartButton;
        private ProVLib.InBit IB_StopButton;
        private ProVLib.InBit IB_AlarmResetButton;
        private KCSDK.DCheckBox dCheckBox1;
        private KCSDK.DFieldEdit dFieldEdit150;
        private KCSDK.DFieldEdit dFieldEdit151;
        private System.Windows.Forms.GroupBox groupBox9;
        private KCSDK.DFieldEdit dFieldEdit2;
        private KCSDK.DFieldEdit dFieldEdit3;
    }
}