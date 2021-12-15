using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Sanford.StateMachineToolkit;
using Sanford.Threading;
using ProVTool;

namespace ProVGEMControlFSM
{
    public enum StateID
    {
        OFFLINE,
        ONLINE,
        EQPOFFLINE,
        HOSTOFFLINE,
        ATTEMPTONLINE,
        ONLINELOCAL,
        ONLINEREMOTE,
        Disposed
    }

    public enum EventID
    {
        Dispose,
        EVT_TURNONLINE,
        EVT_TURNOFFLINE,
        EVT_RCVS1F0,
        EVT_RCVS1F2,
        EVT_TOLOCAL,
        EVT_TOREMOTE,
        //Host Set to Off-Line
        EVT_RCVS1F15,
        //Host Set to On-Line
        EVT_RCVS1F17,
    }

    public class ProVControlFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
    {
        private bool isDisposed = false;

        public event EventHandler<ControlFSMEventArgs> ControlFSMEvent;

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

        public ProVControlFSM(ushort OnLineSubstate, StateID InitCtrlState = StateID.OFFLINE)
        {
            SetupSubstates(StateID.OFFLINE, HistoryType.None, StateID.EQPOFFLINE, StateID.HOSTOFFLINE, StateID.ATTEMPTONLINE);
            if (OnLineSubstate == 0)
            {
                SetupSubstates(StateID.ONLINE, HistoryType.None, StateID.ONLINELOCAL, StateID.ONLINEREMOTE);
            }
            else
            {
                SetupSubstates(StateID.ONLINE, HistoryType.None, StateID.ONLINEREMOTE, StateID.ONLINELOCAL);
            }

            AddTransition(StateID.EQPOFFLINE, EventID.EVT_TURNONLINE, StateID.ATTEMPTONLINE, SendS1F1);
            AddTransition(StateID.ATTEMPTONLINE, EventID.EVT_RCVS1F0, StateID.HOSTOFFLINE);
            AddTransition(StateID.ATTEMPTONLINE, EventID.EVT_RCVS1F2, StateID.ONLINE);
            AddTransition(StateID.ONLINE, EventID.EVT_TURNOFFLINE, StateID.EQPOFFLINE);
            AddTransition(StateID.ONLINE, EventID.EVT_RCVS1F15, StateID.HOSTOFFLINE, SendS1F16);
            AddTransition(StateID.HOSTOFFLINE, EventID.EVT_TURNOFFLINE, StateID.EQPOFFLINE);
            AddTransition(StateID.HOSTOFFLINE, EventID.EVT_RCVS1F17, StateID.ONLINE, SendS1F18);
            AddTransition(StateID.ONLINELOCAL, EventID.EVT_TOREMOTE, StateID.ONLINEREMOTE);
            AddTransition(StateID.ONLINEREMOTE, EventID.EVT_TOLOCAL, StateID.ONLINELOCAL);

            Initialize(InitCtrlState);
        }

        private void SendS1F1(object sender, EventArgs args)
        {
            if (ControlFSMEvent != null)
            {
                ControlFSMEventArgs arg = new ControlFSMEventArgs();
                arg.ActionName = "SendS1F1";
                ControlFSMEvent(this, arg);
            }
        }

        private void SendS1F16(object sender, EventArgs args)
        {
            if (ControlFSMEvent != null)
            {
                ControlFSMEventArgs arg = new ControlFSMEventArgs();
                arg.ActionName = "SendS1F16";
                ControlFSMEvent(this, arg);
            }
        }

        private void SendS1F18(object sender, EventArgs args)
        {
            if (ControlFSMEvent != null)
            {
                ControlFSMEventArgs arg = new ControlFSMEventArgs();
                arg.ActionName = "SendS1F18";
                ControlFSMEvent(this, arg);
            }
        }
    }

    public class ControlFSMEventArgs : EventArgs
    {
        public String ActionName { get; set; }
    }

}
