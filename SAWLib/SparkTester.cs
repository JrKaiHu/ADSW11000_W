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
    public enum eSTState
    {
        IDLE = 0,
        BUSY = 1,
        DONE = 2,
        OUT_OF_LIMIT = 3,
        INIT_VOLTAGE_MISMATCH = 4,
        MOTOR_NOT_ASSIGN = 5,
        SENSOR_NOT_ASSIGN = 6,
        SENSOR_SIGNAL_ABNORMAL = 7,
        SWITCH_PIN_ABNORMAL = 8
    }

    [Designer(typeof(ObjFormDesigner))]
    public partial class SparkTester : UserControl
    {
        private string strText = string.Empty;
        private Motor _m_STMotor = null;
        private InBit _m_STSensor = null;
        private OutBit _m_STSwitch = null;
        private int OrgInitSpeed = 100;
        private int OrgSpeed = 1000;
        private int OrgAcc = 10000;
        private int OrgDec = 10000;
        private int m_contactPos = 0;
        private int StandbyPos = 0;
        private int iTask = 0;

        private MyTimer T1 = new MyTimer(true);

        delegate void UpdatePosDelegate(int Pos);
        UpdatePosDelegate OnUpdatePosState;

        public SparkTester()
        {
            InitializeComponent();

            FEEDSPD = 50;
            m_contactPos = 0;

            OnUpdatePosState = new UpdatePosDelegate(OnUpdateLEDHandler);
        }

        void OnUpdateLEDHandler(int Pos)
        {
            this.lblPos.Text = Pos.ToString();
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         EditorBrowsable(EditorBrowsableState.Always), Bindable(true)]
        public override string Text
        {
            get { return this.strText; }
            set
            {
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
        public Motor m_STMotor
        {
            get { return _m_STMotor; }
            set { _m_STMotor = value; }
        }

        [Category("ProV")]
        public InBit m_STSensor
        {
            get { return _m_STSensor; }
            set { _m_STSensor = value; }
        }

        [Category("ProV")]
        public OutBit m_STSwitch
        {
            get { return _m_STSwitch; }
            set { _m_STSwitch = value; }
        }

        [Category("ProV")]
        public int FEEDSPD
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

        private eSTState Ret = eSTState.IDLE;
        public void Reset()
        {
            //將原始參數保存下來
            OrgInitSpeed = _m_STMotor.InitSpeed;
            OrgSpeed = _m_STMotor.Speed;
            OrgAcc = _m_STMotor.Acceleration;
            OrgDec = _m_STMotor.Deceleration;
            StandbyPos = _m_STMotor.ReadPos();

            iTask = 0;
            Ret = eSTState.IDLE;

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
        private int nResetRetry = 0;

        public eSTState Run(bool simu)
        {
            Ret = eSTState.BUSY;

            switch (iTask)
            {
                case 0:
                    {
                        if (_m_STMotor == null)
                        {
                            Ret = eSTState.MOTOR_NOT_ASSIGN;
                            return Ret;
                        }

                        if (_m_STSensor == null || _m_STSwitch == null)
                        {
                            Ret = eSTState.SENSOR_NOT_ASSIGN;
                            return Ret;
                        }

                        _m_STMotor.InitSpeed = 0;
                        _m_STMotor.SetSpeed(FEEDSPD);
                        _m_STMotor.SetAcceleration(FEEDSPD * 10);
                        _m_STMotor.SetDeceleration(FEEDSPD * 10);

                        if (_m_STSwitch.On(100))
                        {
                            iTask = 1;
                            nResetRetry = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        if (_m_STSensor.Value)
                        {
                            Ret = eSTState.SENSOR_SIGNAL_ABNORMAL;
                            return Ret;
                        }
                        iTask = 5;
                    }
                    break;
                case 5: // Check Whether Out of Limit Bound or Not
                    {
                        NextPos = _m_STMotor.ReadPos() - 50;
                        if (NextPos <= ZLimitPos)
                        {
                            Ret = eSTState.OUT_OF_LIMIT;
                            return Ret;
                        }
                        iTask = 10;
                    }
                    break;
                case 10:
                    {
                        bool On = _m_STSensor.Value;
                        if (simu == true)
                        {
                            On = true;
                        }

                        if (!_m_STSwitch.Value) return eSTState.SWITCH_PIN_ABNORMAL;

                        if (On)
                        {
                            iTask = 50;
                            _m_STMotor.FastStop();
                        }
                        else
                        {
                            if (InvokeRequired)
                            {
                                this.BeginInvoke(OnUpdatePosState, _m_STMotor.ReadEncPos());
                            }
                            else
                            {
                                lblPos.Text = _m_STMotor.ReadEncPos().ToString();
                            }

                            if (_m_STMotor.G00(NextPos)) iTask = 5;
                        }
                    }
                    break;
                case 50:
                    {
                        //Read Encoder Pos For Non Contact Check Final Pos
                        m_contactPos = _m_STMotor.ReadEncPos();
                        _m_STSwitch.Off();

                        //Restore Ogiginal Motor Parameter
                        _m_STMotor.InitSpeed = OrgInitSpeed;
                        _m_STMotor.SetSpeed(OrgSpeed);
                        _m_STMotor.SetAcceleration(OrgAcc);
                        _m_STMotor.SetDeceleration(OrgDec);

                        T1.Restart();
                        iTask = 52;
                    }
                    break;
                case 52:
                    {
                        if (_m_STSwitch.On(500))
                        {
                            T1.Restart();
                            iTask = 53;
                        }
                        //if (T1.On(200))
                        //{
                        //    _m_STSwitch.On();

                        //    T1.Restart();
                        //    iTask = 54;
                        //}
                    }
                    break;

                case 53:
                    {
                        if (_m_STSwitch.Off(500))
                        {
                            T1.Restart();
                            iTask = 54;
                        }
                    }
                    break;

                case 54:

                    if (!_m_STSensor.Value)
                    {
                        iTask = 55;
                        T1.Restart();
                    }
                    else
                    {
                        if (T1.On(500))
                        {
                            if (nResetRetry < 10)
                            {
                                nResetRetry++;
                                iTask = 52;
                            }
                        }
                    }

                    //_m_STSwitch.Off();
                    //if (_m_STSensor.Off(500))
                    //{
                    //    iTask = 55;
                    //    T1.Restart();
                    //}
                    break;
                case 55:
                    {
                        if (T1.On(500))
                        {
                            _m_STMotor.SetEncoderPos(m_contactPos);
                            _m_STMotor.SetPos(m_contactPos);
                            iTask = 60;
                        }
                    }
                    break;
                case 60:
                    {
                        if (_m_STMotor.G00(StandbyPos))
                        {
                            Ret = eSTState.DONE;
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
