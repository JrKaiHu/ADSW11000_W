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

namespace ProVGEMCommFSM
{
    public enum StateID
    {
        DISABLED,
        ENABLED,
        NOT_COMMUNICATING,
        COMMUNICATING,
        WAIT_CRA,
        WAIT_DELAY,
        WAIT_CR_FROM_HOST,
        Disposed
    }

    public enum EventID
    {
        Dispose,
        EVT_HSMS_CONNECTED,
        EVT_ENABLE,
        EVT_DISABLE,
        EVT_CONN_TRANSACTION_FAILURE,
        EVT_COMMDELAY_EXPIRED,
        EVT_RCV_OTHERTHAN_S1F13,
        EVT_COMMACK_0,
        EVT_COMM_FAILURE,
        EVT_RCV_S1F13
    }

    public class ProVEQPInitiatedFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
    {
        private readonly AsyncOperation operation = AsyncOperationManager.CreateOperation(null);

        private readonly DelegateScheduler scheduler = new DelegateScheduler();

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

        public ProVEQPInitiatedFSM()
        {
            this[StateID.DISABLED].EntryHandler += Disable_EntryHandler;
            this[StateID.ENABLED].EntryHandler += Enable_EntryHandler;
            this[StateID.WAIT_CRA].EntryHandler += WaitCRA_EntryHandler;
            this[StateID.NOT_COMMUNICATING].EntryHandler += Communicating_EntryHandler;

            SetupSubstates(StateID.ENABLED, HistoryType.None, StateID.NOT_COMMUNICATING, StateID.COMMUNICATING);
            SetupSubstates(StateID.NOT_COMMUNICATING, HistoryType.None, StateID.WAIT_CRA, StateID.WAIT_DELAY);

            AddTransition(StateID.DISABLED, EventID.EVT_ENABLE, StateID.ENABLED);
            AddTransition(StateID.ENABLED, EventID.EVT_DISABLE, StateID.DISABLED);

            AddTransition(StateID.WAIT_CRA, EventID.EVT_CONN_TRANSACTION_FAILURE, StateID.WAIT_DELAY, ScheduleCommDelayTimer);
            AddTransition(StateID.WAIT_DELAY, EventID.EVT_RCV_OTHERTHAN_S1F13, StateID.WAIT_CRA);
            AddTransition(StateID.WAIT_DELAY, EventID.EVT_COMMDELAY_EXPIRED, StateID.WAIT_CRA);
            AddTransition(StateID.WAIT_CRA, EventID.EVT_COMMACK_0, StateID.COMMUNICATING);
            AddTransition(StateID.WAIT_DELAY, EventID.EVT_COMMACK_0, StateID.COMMUNICATING);
            AddTransition(StateID.COMMUNICATING, EventID.EVT_COMM_FAILURE, StateID.NOT_COMMUNICATING);

            Initialize(StateID.DISABLED);
        }

        void Communicating_EntryHandler(object sender, TransitionEventArgs<StateID, EventID, EventArgs> e)
        {
            scheduler.Stop();
            scheduler.Clear();
        }

        void WaitCRA_EntryHandler(object sender, TransitionEventArgs<StateID, EventID, EventArgs> e)
        {
            scheduler.Start();
        }

        void Enable_EntryHandler(object sender, TransitionEventArgs<StateID, EventID, EventArgs> e)
        {
        }

        void Disable_EntryHandler(object sender, TransitionEventArgs<StateID, EventID, EventArgs> e)
        {
            scheduler.Stop();
            scheduler.Clear();
        }

        private void ScheduleCommDelayTimer(object sender, EventArgs args)
        {
            scheduler.Add(1, 5000, new SendTimerDelegate(SendTimerEvent));
        }

        private delegate void SendTimerDelegate();

        private void SendTimerEvent()
        {
            operation.Post(new SendOrPostCallback(delegate(object state)
            {
                Send(EventID.EVT_COMMDELAY_EXPIRED);
                Execute();
            }), null);
        }
    }

    public class ProVHostInitiatedFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
    {
        public EventHandler OnS1F14Send;
        public EventHandler OnCommEnable;
        public EventHandler OnCommDisable;

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

        public ProVHostInitiatedFSM()
        {
            SetupSubstates(StateID.ENABLED, HistoryType.None, StateID.NOT_COMMUNICATING, StateID.COMMUNICATING);
            SetupSubstates(StateID.NOT_COMMUNICATING, HistoryType.None, StateID.WAIT_CR_FROM_HOST);

            AddTransition(StateID.DISABLED, EventID.EVT_ENABLE, StateID.ENABLED, CommunicationEnable);
            AddTransition(StateID.ENABLED, EventID.EVT_DISABLE, StateID.DISABLED, CommunicationDisable);

            AddTransition(StateID.WAIT_CR_FROM_HOST, EventID.EVT_RCV_S1F13, StateID.COMMUNICATING, SendS1F14);
            AddTransition(StateID.WAIT_CR_FROM_HOST, EventID.EVT_COMMACK_0, StateID.COMMUNICATING);
            AddTransition(StateID.COMMUNICATING, EventID.EVT_COMM_FAILURE, StateID.NOT_COMMUNICATING);

            Initialize(StateID.DISABLED);
        }

        private void SendS1F14(object sender, EventArgs args)
        {
            if (OnS1F14Send != null)
                OnS1F14Send(this, null);
        }

        private void CommunicationEnable(object sender, EventArgs args)
        {
            if (OnCommEnable != null)
            {
                OnCommEnable(this, null);
            }
        }

        private void CommunicationDisable(object sender, EventArgs args)
        {
            if (OnCommDisable != null)
            {
                OnCommDisable(this, null);
            }
        }
    }
}
