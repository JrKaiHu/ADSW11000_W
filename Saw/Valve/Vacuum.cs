using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using ProVLib;

namespace Saw.Valve
{
    public class Vacuum : Valve
    {
        protected Vacuum(OutBit OpenOut, InBit OpenIn, OutBit CloseOut, InBit CloseIn, int nTimeoutMilliSec) :
            base(OpenOut, OpenIn, CloseOut, CloseIn, nTimeoutMilliSec)
        {
        }

        protected override enValveStatus GetValveStatus(enValveActMode enMode, Func<bool> CheckInput)
        {
            if (enMode == enValveActMode.NOCHECK) return enValveStatus.COMPLETE;
            if (!m_Sw.IsRunning) m_Sw.Restart();

            if (CheckInput())
            {
                m_Sw.Stop();
                if (enMode == enValveActMode.AUTOOFF) m_CloseOut.Off();
                return enValveStatus.COMPLETE;
            }
            else
            {
                if (m_Sw.ElapsedMilliseconds > m_nTimeoutMilliSec)
                {
                    m_Sw.Stop();
                    return enValveStatus.TIMEOUT;
                }
                else return enValveStatus.RUNNING;
            }
        }
    }
}
