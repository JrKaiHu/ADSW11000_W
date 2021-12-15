using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Sanford.StateMachineToolkit;
using Sanford.Threading;
using ProVTool;

namespace ProVGEMLimitFSM
{
    public enum StateID
    {
        DISABLED,
        ENABLED,
        BELOWLIMIT,
        ABOVELIMIT,
        NOZONE,
        Disposed
    }

    public enum EventID
    {
        Dispose,
        EVT_DEFINELIMIT,
        EVT_UNDEFINELIMIT,
        EVT_UPPERDB,
        EVT_LOWERDB,
        EVT_TIMER_EXPIRED
    }

    public class ProVLimitFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
    {
        private System.Timers.Timer QueryTimer = null;
        private int iQueryInterval = 1; //Every 1 sec
        public delegate SECS_BASE QueryDataValueDelegate(ushort VID);
        public event QueryDataValueDelegate QueryDataValue;

        private bool isDisposed = false;

        #region IDisposable Members

        public void Dispose()
        {
            #region Guard

            if (isDisposed)
            {
                return;
            }

            #endregion

            Send(EventID.Dispose);
        }

        #endregion

        public int QueryInterval
        {
            get
            {
                return iQueryInterval;
            }
            set
            {
                iQueryInterval = value;
            }
        }

        

        public ushort VID
        {
            get;
            set;
        }

        public ushort LIMITID
        {
            get;
            set;
        }

        public String Format
        {
            get;
            set;
        }

        public SECS_BASE CurValue
        {
            get;
            set;
        }

        public SECS_BASE UPPERDB
        {
            get;
            set;
        }

        public SECS_BASE LOWERDB
        {
            get;
            set;
        }

        public ushort TransitionType
        {
            get;
            set;
        }

        public ProVLimitFSM(ushort VID, ushort LIMITID, String Format, SECS_BASE UpperLimit, SECS_BASE LowerLimit)
        {
            this.VID = VID;
            this.LIMITID = LIMITID;
            this.Format = Format;
            this.UPPERDB = UpperLimit;
            this.LOWERDB = LowerLimit;

            SetupSubstates(StateID.ENABLED, HistoryType.None, StateID.NOZONE, StateID.ABOVELIMIT, StateID.BELOWLIMIT);

            AddTransition(StateID.DISABLED, EventID.EVT_DEFINELIMIT, StateID.ENABLED);
            AddTransition(StateID.ENABLED, EventID.EVT_UNDEFINELIMIT, StateID.DISABLED);
            AddTransition(StateID.BELOWLIMIT, EventID.EVT_UPPERDB, StateID.ABOVELIMIT);
            AddTransition(StateID.ABOVELIMIT, EventID.EVT_LOWERDB, StateID.BELOWLIMIT);
            AddTransition(StateID.NOZONE, EventID.EVT_LOWERDB, StateID.BELOWLIMIT);
            AddTransition(StateID.NOZONE, EventID.EVT_UPPERDB, StateID.ABOVELIMIT);

            Initialize(StateID.DISABLED);

            QueryTimer = new System.Timers.Timer(iQueryInterval * 1000);
            QueryTimer.Elapsed += QueryTimer_Elapsed;
        }

        public void StopMonitor()
        {
            QueryTimer.Stop();
        }

        public void StartMonitor()
        {
            QueryTimer.Start();
        }

        void QueryTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            QueryTimer.Stop();
            if (QueryDataValue != null)
            {
                CurValue = QueryDataValue(VID);

                if (CurValue != null)
                {
                    switch (Format)
                    {
                        case "B":
                            {
                                SECS_BINARY val = CurValue as SECS_BINARY;
                                if (val != null)
                                {
                                    SECS_BINARY UpperData = UPPERDB as SECS_BINARY;
                                    SECS_BINARY LowerData = LOWERDB as SECS_BINARY;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "U1":
                            {
                                SECS_U1 val = CurValue as SECS_U1;
                                if (val != null)
                                {
                                    SECS_U1 UpperData = UPPERDB as SECS_U1;
                                    SECS_U1 LowerData = LOWERDB as SECS_U1;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "U2":
                            {
                                SECS_U2 val = CurValue as SECS_U2;
                                if (val != null)
                                {
                                    SECS_U2 UpperData = UPPERDB as SECS_U2;
                                    SECS_U2 LowerData = LOWERDB as SECS_U2;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "U4":
                            {
                                SECS_U4 val = CurValue as SECS_U4;
                                if (val != null)
                                {
                                    SECS_U4 UpperData = UPPERDB as SECS_U4;
                                    SECS_U4 LowerData = LOWERDB as SECS_U4;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "I1":
                            {
                                SECS_I1 val = CurValue as SECS_I1;
                                if (val != null)
                                {
                                    SECS_I1 UpperData = UPPERDB as SECS_I1;
                                    SECS_I1 LowerData = LOWERDB as SECS_I1;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "I2":
                            {
                                SECS_I2 val = CurValue as SECS_I2;
                                if (val != null)
                                {
                                    SECS_I2 UpperData = UPPERDB as SECS_I2;
                                    SECS_I2 LowerData = LOWERDB as SECS_I2;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "I4":
                            {
                                SECS_I4 val = CurValue as SECS_I4;
                                if (val != null)
                                {
                                    SECS_I4 UpperData = UPPERDB as SECS_I4;
                                    SECS_I4 LowerData = LOWERDB as SECS_I4;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "F4":
                            {
                                SECS_F4 val = CurValue as SECS_F4;
                                if (val != null)
                                {
                                    SECS_F4 UpperData = UPPERDB as SECS_F4;
                                    SECS_F4 LowerData = LOWERDB as SECS_F4;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                        case "F8":
                            {
                                SECS_F8 val = CurValue as SECS_F8;
                                if (val != null)
                                {
                                    SECS_F8 UpperData = UPPERDB as SECS_F8;
                                    SECS_F8 LowerData = LOWERDB as SECS_F8;
                                    if (val.Data >= UpperData.Data)
                                    {
                                        TransitionType = 0;
                                        Send(EventID.EVT_UPPERDB);
                                        Execute();
                                    }
                                    else if (val.Data <= LowerData.Data)
                                    {
                                        TransitionType = 1;
                                        Send(EventID.EVT_LOWERDB);
                                        Execute();
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            QueryTimer.Start();
        }
    }
}
