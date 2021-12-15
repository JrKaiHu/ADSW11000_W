using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVLib;

namespace SAWLib
{
    /// <summary>
    /// NDS 類別
    /// 1. 建構NDS物件，參數需傳入待測高Z軸馬達與測高檢查類比輸入元件
    /// 2. 參數設定(往下接觸Sensor的速度HIGHSPD, 往上離開Sensor的速度LOWSPD, 卡控電壓ControlVoltage, Z軸移動極限點位)
    /// 3. 呼叫Reset 重置參數
    /// 4. 呼叫Run 開始測高流程 
    /// 5. 回傳值為(eNDSState.BUSY) 時為測高程序進行中
    /// 5. 回傳值為(eNDSState.DONE) 時讀取ContactPos 完成非接觸測高
    /// 6. 回傳值為(eNDSState.OUTOFLIMIT) 移動位置超出Z軸設定的極限位置
    /// </summary>
    public enum eNCSState
    {
        IDLE = 0,
        BUSY = 1,
        DONE = 2,
        OUTOFLIMIT = 3,
        INITVOLTAGEMISMATCH = 4,
        MOTORNOTASSIGN = 5,
        SENSORNOTASSIGN = 6
    }

    [Designer(typeof(ObjFormDesigner))]
    public partial class NCTester : UserControl
    {
        private string strText = string.Empty;
        private Motor ZMotor = null;
        private AnalogIn ZDetectSnr = null;
        private int OrgInitSpeed = 100;
        private int OrgSpeed = 1000;
        private int OrgAcc = 10000;
        private int OrgDec = 10000;
        private int m_contactPos = 0;
        private int StandbyPos = 0;
        private int iTask = 0;

        delegate void UpdatePosDelegate(int Pos);
        UpdatePosDelegate OnUpdatePosState;

        public NCTester()
        {
            InitializeComponent();

            HIGHSPD = 500;
            LOWSPD = 50;
            ControlVoltage = 4.5;
            m_contactPos = 0;
            MeasureVoltage = 4.0;

            OnUpdatePosState = new UpdatePosDelegate(OnUpdateLEDHandler);
        }

        void OnUpdateLEDHandler(int Pos)
        {
            this.lblPos.Text = Pos.ToString();
        }

        [Category("ProV")]
        public Motor NCTMotor
        {
            get { return ZMotor; }
            set { ZMotor = value; }
        }

        [Category("ProV")]
        public AnalogIn NCTSensor
        {
            get { return ZDetectSnr; }
            set { ZDetectSnr = value; }
        }

       [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        EditorBrowsable(EditorBrowsableState.Always), Bindable(true)]
        public override string Text
        {
            get { return this.strText; }
            set
            {

                //if (value == null || value.Length < 1)
                //{
                //    value = this.Name;
                //}

                this.strText = value;
                this.Invalidate();
            }
        }

        private Font captionFont = new Font("微軟正黑體", 12);

        public Font CaptionFont
        {
            get
            {
                return captionFont;
            }
            set
            {
                captionFont = value;
                this.Refresh();
            }
        }

        [Category("ProV")]
        public int HIGHSPD
        {
            get;
            set;
        }

        [Category("ProV")]
        public int LOWSPD
        {
            get;
            set;
        }

        [Category("ProV")]
        public int ZLimitPos
        {
            get;
            set;
        }

        [Category("ProV")]
        public double ControlVoltage
        {
            get;
            set;
        }

        [Category("ProV")]
        public double MeasureVoltage
        {
            get;
            set;
        }

        [Category("ProV")]
        public int ContactPos
        {
            get { return m_contactPos; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SizeF strSize = e.Graphics.MeasureString(this.Text, CaptionFont);
            int yPos = (this.Height - (int)strSize.Height) / 2;
            e.Graphics.DrawString(this.Text, CaptionFont, Brushes.White, new PointF(1, yPos));
        }

        private eNCSState Ret = eNCSState.IDLE;
        public void Reset()
        {
            //將原始參數保存下來
            OrgInitSpeed = ZMotor.InitSpeed;
            OrgSpeed = ZMotor.Speed;
            OrgAcc = ZMotor.Acceleration;
            OrgDec = ZMotor.Deceleration;
            StandbyPos = ZMotor.ReadPos();
            iTask = 0;
            Ret = eNCSState.IDLE;

            m_contactPos = 0;

            if (InvokeRequired)
            {
                this.BeginInvoke(OnUpdatePosState, m_contactPos);
            }
            else
            {
                lblPos.Text = m_contactPos.ToString();
            }
        }
        private int NextPos = 0;
        public eNCSState Run(bool SimuSensorOn,bool SimulationMode)
        {
            Ret = eNCSState.BUSY;
            switch (iTask)
            {
                case 0:
                    {
                        if (ZMotor == null)
                        {
                            Ret = eNCSState.MOTORNOTASSIGN;
                            return Ret; 
                        }
                        if (ZDetectSnr == null)
                        {
                            Ret = eNCSState.SENSORNOTASSIGN;
                            return Ret;
                        }


                        if (ZDetectSnr.RealValue < ControlVoltage && !SimulationMode)
                        {
                            Ret = eNCSState.INITVOLTAGEMISMATCH;
                            return Ret;
                        }

                        MeasureVoltage = ZDetectSnr.RealValue - 1; //量測電壓為初始電壓減一伏特

                        ZMotor.InitSpeed = 0;
                        ZMotor.SetSpeed(HIGHSPD);
                        ZMotor.SetAcceleration(HIGHSPD * 10);
                        ZMotor.SetDeceleration(HIGHSPD * 10);

                        iTask = 5;
                    }
                    break;
                case 5: // Check Whether Out of Limit Bound or Not
                    {
                        NextPos = ZMotor.ReadPos() - 50;
                        if (NextPos <= ZLimitPos)
                        {
                            Ret = eNCSState.OUTOFLIMIT;
                            return Ret;
                        }
                        iTask = 10;
                    }
                    break;
                case 10: //Delay and Check Analog Input Value with Measure Voltage
                    {
                        bool OK;
                        if (SimuSensorOn)
                        {
                            OK = true;
                        }
                        else
                            OK = ZDetectSnr.RealValue <= MeasureVoltage;


                        if (ZMotor.G00(NextPos))
                        {
                            if (OK)
                            {
                                iTask = 20;
                            }
                            else
                            {
                                iTask = 5;
                            }
                        }
                    }
                    break;
                case 20: //Change to Lower Speed
                    {
                        ZMotor.SetSpeed(LOWSPD);
                        ZMotor.SetAcceleration(LOWSPD * 10);
                        ZMotor.SetDeceleration(LOWSPD * 10);
                        iTask = 30;
                    }
                    break;
                case 30: //Calculate Next Pos
                    {
                        NextPos = ZMotor.ReadPos() + 10;
                        iTask = 35;
                    }
                    break;
                case 35: // Move to Next Pos
                    {
                        if (ZMotor.G00(NextPos))
                        {
                            //T1.Restart();
                            //Reset Analog Input IsUpToDate Flag
                            ZDetectSnr.IsUpToDate = false;
                            iTask = 36;
                        }
                    }
                    break;
                case 36: // Move to Next Pos
                    {
                        bool OK;

                        if (SimulationMode)
                            OK = true;
                        else
                            OK = ZDetectSnr.RealValue >= MeasureVoltage;
                        if (ZDetectSnr.IsUpToDate || SimulationMode) //Check Analog Input IsUpToDate Flag
                        {
                            //Check Analog Input Value with Measure Voltage
                            if (OK)
                            {
                                iTask = 37;
                            }
                            else
                            {
                                iTask = 30;
                            }
                        }
                    }
                    break;
                case 37: // Check Whether Out of Limit Bound or Not
                    {
                        NextPos = ZMotor.ReadPos() - 3;
                        if (NextPos <= ZLimitPos)
                        {
                            Ret = eNCSState.OUTOFLIMIT;
                            return Ret;
                        }
                        iTask = 38;
                    }
                    break;
                case 38: //Delay and Check Analog Input Value with Measure Voltage
                    {
                        bool OK;

                        if (SimulationMode)
                            OK = true;
                        else
                            OK = ZDetectSnr.RealValue <= MeasureVoltage;

                        if (ZMotor.G00(NextPos))
                        {
                            if (OK)
                            {
                                iTask = 39;
                            }
                            else
                            {
                                iTask = 37;
                            }
                        }
                    }
                    break;
                case 39: //Calculate Next Pos
                    {
                        NextPos = ZMotor.ReadPos() + 1;
                        iTask = 40;
                    }
                    break;
                case 40: // Move to Next Pos
                    {
                        if (ZMotor.G00(NextPos))
                        {
                            //T1.Restart();
                            //Reset Analog Input IsUpToDate Flag
                            ZDetectSnr.IsUpToDate = false;
                            iTask = 41;
                        }
                    }
                    break;
                case 41: // Move to Next Pos
                    {
                        bool OK;

                        if (SimulationMode)
                            OK = true;
                        else
                            OK = ZDetectSnr.RealValue >= MeasureVoltage;

                        if (ZDetectSnr.IsUpToDate || SimulationMode) //Check Analog Input IsUpToDate Flag
                        {
                            //Check Analog Input Value with Measure Voltage
                            if (OK)
                            {
                                iTask = 50;
                            }
                            else
                            {
                                iTask = 39;
                            }
                        }
                    }
                    break;
                case 50:
                    {
                        //Read Encoder Pos For Non Contact Check Final Pos
                        m_contactPos = ZMotor.ReadEncPos();
                        
                        //Restore Ogiginal Motor Parameter
                        ZMotor.InitSpeed = OrgInitSpeed;
                        ZMotor.SetSpeed(OrgSpeed);
                        ZMotor.SetAcceleration(OrgAcc);
                        ZMotor.SetDeceleration(OrgDec);
                        iTask = 60;
                    }
                    break;
                case 60:
                    {
                        if (ZMotor.G00(StandbyPos))
                        {
                            Ret = eNCSState.DONE;
                            iTask = 0;

                            if (InvokeRequired)
                            {
                                this.BeginInvoke(OnUpdatePosState, m_contactPos);
                            }
                            else
                            {
                                lblPos.Text = m_contactPos.ToString();
                            }
                        }
                    }
                    break;
            }
            return Ret;
        }
    }
}
