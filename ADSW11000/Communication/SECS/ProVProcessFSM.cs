using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Sanford.StateMachineToolkit;
using Sanford.Threading;
using ProVTool;

namespace ProVGEMProcessFSM
{
    public enum StateID
    {
        IDLE = 1,
        HOME,
        INITIAL,
        SETUP,
        EXECUTING,
        PAUSE,
        ALARM,
        PROCESS,
        NORMAL,
        Disposed
    }

    public enum EventID
    {
        Dispose,
        EVT_HOME,
        EVT_HOMEDONE,
        EVT_SETUP,
        EVT_INITIAL,
        EVT_START,
        EVT_PAUSE,
        EVT_RESUME,
        EVT_ALARMSET,
        EVT_ALARMRST,
        EVT_STOP,
        EVT_ABORT,
        EVT_LOTEND
    }

    public class ProVProcessFSM : PassiveStateMachine<StateID, EventID, EventArgs>, IDisposable
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

        public ProVProcessFSM()
        {
            SetupSubstates(StateID.NORMAL, HistoryType.Deep, StateID.IDLE, StateID.HOME, StateID.PAUSE, StateID.PROCESS);
            SetupSubstates(StateID.PROCESS, HistoryType.Shallow, StateID.SETUP, StateID.INITIAL, StateID.EXECUTING);

            //Alarm Occur
            AddTransition(StateID.NORMAL, EventID.EVT_ALARMSET, StateID.ALARM);
            //Alarm Clear
            AddTransition(StateID.ALARM, EventID.EVT_ALARMRST, StateID.NORMAL);
            //IDLE
            AddTransition(StateID.IDLE, EventID.EVT_HOME, StateID.HOME);
            AddTransition(StateID.IDLE, EventID.EVT_SETUP, StateID.SETUP);
            //Home
            AddTransition(StateID.HOME, EventID.EVT_PAUSE, StateID.PAUSE);
            AddTransition(StateID.HOME, EventID.EVT_HOMEDONE, StateID.IDLE);
            AddTransition(StateID.HOME, EventID.EVT_ABORT, StateID.IDLE);
            //Initial
            AddTransition(StateID.SETUP, EventID.EVT_INITIAL, StateID.INITIAL);
            //Abort Process Before Start Command
            AddTransition(StateID.INITIAL, EventID.EVT_STOP, StateID.IDLE);
            //Start Process
            AddTransition(StateID.INITIAL, EventID.EVT_START, StateID.EXECUTING);
            AddTransition(StateID.INITIAL, EventID.EVT_PAUSE, StateID.PAUSE);
            //Pause When Process
            AddTransition(StateID.PROCESS, EventID.EVT_PAUSE, StateID.PAUSE);
            //Abort When Process
            AddTransition(StateID.PROCESS, EventID.EVT_ABORT, StateID.IDLE);
            //Lotend
            AddTransition(StateID.EXECUTING, EventID.EVT_LOTEND, StateID.IDLE);
            //Abort Process When Processing
            AddTransition(StateID.EXECUTING, EventID.EVT_ABORT, StateID.IDLE);
            //RESUME
            AddTransition(StateID.PAUSE, EventID.EVT_RESUME, StateID.PROCESS);
            AddTransition(StateID.PAUSE, EventID.EVT_START, StateID.PROCESS);
            AddTransition(StateID.PAUSE, EventID.EVT_LOTEND, StateID.IDLE);
            AddTransition(StateID.PAUSE, EventID.EVT_HOME, StateID.HOME);
            AddTransition(StateID.PAUSE, EventID.EVT_ABORT, StateID.IDLE);

            Initialize(StateID.NORMAL);
        }
    }
}
