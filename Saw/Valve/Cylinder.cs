using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using ProVLib;

namespace Saw.Valve
{
    public class Cylinder : Valve
    {
        private int m_nWaitTimeMilliSec;
        private bool m_bIsWaitDetect = false;

        Cylinder(OutBit OpenOut, InBit OpenIn, OutBit CloseOut, InBit CloseIn, int nTimeoutMilliSec, int nWaitTimeMilliSec = 0):
            base(OpenOut, OpenIn, CloseOut, CloseIn, nTimeoutMilliSec)
        {
            m_nWaitTimeMilliSec = nWaitTimeMilliSec;
        }

        protected override enValveStatus GetValveStatus(enValveActMode enMode, Func<bool> CheckInput)
        {
            if (enMode == enValveActMode.NOCHECK) return enValveStatus.COMPLETE;
            if (!m_Sw.IsRunning) m_Sw.Restart();

            if (CheckInput())
            {
                if (!m_bIsWaitDetect)
                {
                    m_Sw.Stop();
                    m_bIsWaitDetect = true;
                    return enValveStatus.RUNNING;
                }
                else
                {
                    if (m_Sw.ElapsedMilliseconds < m_nWaitTimeMilliSec)
                    {
                        return enValveStatus.RUNNING;
                    }
                    else
                    {
                        m_Sw.Stop();
                        m_bIsWaitDetect = false;
                        return enValveStatus.COMPLETE;
                    }
                }
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
