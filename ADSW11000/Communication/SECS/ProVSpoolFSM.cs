using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.StateMachineToolkit;
using Sanford.Threading;
using System.ComponentModel;

namespace ProVGEMSpoolFSM
{
    public enum StateID
    {
        POWEROFF,
        POWERON,
        SPOOLINACTIVE,
        SPOOLLOAD,
        SPOOLUNLOAD,
        SPOOLNOTFULL,
        SPOOLFULL,
        NOSPOOLPUTPUT,
        SPOOLOUTPUT,
        PURGESPOOL,
        TRANSMITSPOOL,
        Disposed
    }

    public enum EventID
    {
        Dispose,
        EVT_COMMBROKEN,
        EVT_SPOOLFULL,
        EVT_SPOOLEMPTY,
        EVT_PURGE,
        EVT_TRANSMIT,
        EVT_MAXSPOOLTRANSREACH,
        EVT_POWERDOWN,
        EVT_POWERUP,
    }

    public class SpoolLoadFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
    {
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

        public SpoolLoadFSM()
        {
            SetupSubstates(StateID.SPOOLLOAD, HistoryType.None, StateID.SPOOLNOTFULL, StateID.SPOOLFULL);

            AddTransition(StateID.SPOOLINACTIVE, EventID.EVT_COMMBROKEN, StateID.SPOOLLOAD, OnSpoolLoad);
            AddTransition(StateID.SPOOLNOTFULL, EventID.EVT_SPOOLFULL, StateID.SPOOLFULL, OnSpoolFull);

            Initialize(StateID.SPOOLINACTIVE);
        }

        private void OnSpoolLoad(object sender, EventArgs args)
        {
        }

        private void OnSpoolFull(object sender, EventArgs args)
        {
        }
    }

    public class SpoolUnloadFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
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

        public SpoolUnloadFSM()
        {
            SetupSubstates(StateID.SPOOLUNLOAD, HistoryType.None, StateID.NOSPOOLPUTPUT, StateID.SPOOLOUTPUT);
            SetupSubstates(StateID.SPOOLOUTPUT, HistoryType.None, StateID.TRANSMITSPOOL, StateID.PURGESPOOL);

            AddTransition(StateID.SPOOLINACTIVE, EventID.EVT_COMMBROKEN, StateID.SPOOLUNLOAD);
            AddTransition(StateID.SPOOLOUTPUT, EventID.EVT_SPOOLEMPTY, StateID.SPOOLINACTIVE, OnSpoolEmpty);
            AddTransition(StateID.NOSPOOLPUTPUT, EventID.EVT_PURGE, StateID.PURGESPOOL, OnPurgeSpool);
            AddTransition(StateID.NOSPOOLPUTPUT, EventID.EVT_TRANSMIT, StateID.TRANSMITSPOOL, OnTransmitSpool);
            AddTransition(StateID.TRANSMITSPOOL, EventID.EVT_MAXSPOOLTRANSREACH, StateID.NOSPOOLPUTPUT, OnMaxTransmitReach);

            Initialize(StateID.SPOOLINACTIVE);

        }

        private void OnSpoolEmpty(object sender, EventArgs args)
        {
        }

        private void OnPurgeSpool(object sender, EventArgs args)
        {
        }

        private void OnTransmitSpool(object sender, EventArgs args)
        {
        }

        private void OnMaxTransmitReach(object sender, EventArgs args)
        {
        }
    }
}
