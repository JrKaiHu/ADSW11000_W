using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using ProVLib;

namespace Saw.Valve
{
    public abstract class Valve
    {
        protected OutBit m_OpenOut;
        protected InBit m_OpenIn;
        protected OutBit m_CloseOut;
        protected InBit m_CloseIn;
        protected int m_nTimeoutMilliSec;
        protected Stopwatch m_Sw = new Stopwatch();

        protected Valve(OutBit OpenOut, InBit OpenIn, OutBit CloseOut, InBit CloseIn, int nTimeoutMilliSec)
        {
            m_OpenOut = OpenOut;
            m_OpenIn = OpenIn;
            m_CloseOut = CloseOut;
            m_CloseIn = CloseIn;
            m_nTimeoutMilliSec = nTimeoutMilliSec;
        }

        protected enValveStatus Open(enValveActMode enMode)
        {
            if (m_OpenOut != null) m_OpenOut.On();
            if (m_CloseOut != null) m_CloseOut.Off();

            return GetValveStatus(enMode, IsOpenSR);
        }

        protected enValveStatus Close(enValveActMode enMode)
        {
            if (m_CloseOut != null) m_CloseOut.On();
            if (m_OpenOut != null) m_OpenOut.Off();

            return GetValveStatus(enMode, IsOpenSR);
        }

        protected abstract enValveStatus GetValveStatus(enValveActMode enMode, Func<bool> CheckInput);

        public void Off()
        {
            m_OpenOut.Off();
            m_CloseOut.Off();
        }

        public bool IsOpenSR()
        {
            bool bRet = true;

            if (m_OpenIn != null)
            {
                bRet &= m_OpenIn.Value;
            }

            if (m_CloseIn != null)
            {
                bRet &= (!m_CloseIn.Value);
            }

            return bRet;
        }

        public bool IsCloseSR()
        {
            bool bRet = true;

            if (m_CloseIn != null)
            {
                bRet &= m_CloseIn.Value;
            }

            if (m_OpenIn != null)
            {
                bRet &= (!m_OpenIn.Value);
            }

            return bRet;
        }
    }
}
