using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KCSDK;
using System.Runtime.InteropServices;
using System.Security;

namespace ADSW11000
{
    public class StateThread
    {
        private Thread ThreadCtrl = null;
        private bool StopThread = false;
        private SPCCounter spcCounter = new SPCCounter(true);

        public StateThread()
        {
            ThreadCtrl = new Thread(Execute);
            ThreadCtrl.Priority = ThreadPriority.Highest;
            ThreadCtrl.IsBackground = true;
            ThreadCtrl.Start();
        }

        private void Execute()
        {
            long RunTick = 0;
            long ScanTick = 0;
            double LastSecond = 0;
            double TempSecond = 0;
            double tm;
            int SleepTM = 1;
            ulong TotalRunTM = 0;

            SYSPara.IdleTM = 0;
            SYSPara.HomeTM = 0;
            SYSPara.ManualTM = 0;
            SYSPara.MaxScanTime = 0;


            GetTickCountEx tick = new GetTickCountEx();
            //bool mFirst = true;

            while (!StopThread)
            {
                tm = tick.Value;
                //常態執行
                try
                {
                    FormSet.mMSS.AlwaysRun();
                }
                catch (Exception)
                {
                    SYSPara.Alarm.Say("E0502");
                }

                //主流程
                try
                {
                    FormSet.mMSS.MainRun();
                }
                catch (Exception)
                {
                    SYSPara.Alarm.Say("E0503");
                }


                if (RunTick > 0)
                {
                    Thread.Sleep(SleepTM);
                    RunTick = 0;
                }

                RunTick++;
                ScanTick++;
                TempSecond = tick.Value;

                //系統時間統計
                if ((TempSecond - LastSecond) > 999)
                {
                    LastSecond = TempSecond;

                    switch (SYSPara.RunMode)
                    {
                        case RunModeDT.IDLE:
                            SYSPara.IdleTM++;
                            spcCounter.SPCIdleTM++;
                            break;
                        case RunModeDT.HOME:
                            SYSPara.HomeTM++;
                            spcCounter.SPCHomeTM++;
                            break;
                        case RunModeDT.MANUAL:
                            SYSPara.ManualTM++;
                            spcCounter.SPCManualTM++;
                            break;
                        case RunModeDT.AUTO:
                            SYSPara.OperationSecond++;
                            if (SYSPara.SysRun)
                            {
                                SYSPara.RunSecond++;
                                spcCounter.SPCRunSecond++;
                            }
                            else
                            {
                                SYSPara.StopSecond++;
                                spcCounter.SPCStopSecond++;
                            }
                            break;
                    }

                    SYSPara.ScanTime = ScanTick;
                    ScanTick = 0;

                    if (TotalRunTM++ % 60 == 0) //每隔60秒記錄一次
                    {
                        //+ By Max 20210113 For LogAnalyzer
                        try
                        {
                            SYSPara.logDB.LogSayDb(spcCounter.SPCRunSecond, spcCounter.SPCHomeTM, spcCounter.SPCIdleTM, spcCounter.SPCManualTM, spcCounter.SPCStopSecond);
                            spcCounter.ResetCounter();
                        }
                        catch (Exception) { }
                    }
                }

                SYSPara.ScanTimeEx = tick.Value - tm;
                //if (mFirst)
                //{
                //    SYSPara.ScanTimeEx = tick.Value - tm;
                //    mFirst = false;
                //}
                //else
                //    SYSPara.ScanTimeEx = (SYSPara.ScanTimeEx + (tick.Value - tm)) / 2;

                if (SYSPara.ScanTimeEx > SYSPara.MaxScanTime)
                    SYSPara.MaxScanTime = SYSPara.ScanTimeEx;
            }
        }

        public void DisposTH()
        {
            StopThread = true;
            ThreadCtrl.Join();
        }

        internal struct SPCCounter
        {
            public uint SPCIdleTM;
            public uint SPCHomeTM;
            public uint SPCManualTM;
            public uint SPCRunSecond;
            public uint SPCStopSecond;

            public SPCCounter(bool bReset = true)
            {
                SPCIdleTM = 0;
                SPCHomeTM = 0;
                SPCManualTM = 0;
                SPCRunSecond = 0;
                SPCStopSecond = 0;
            }

            public void ResetCounter()
            {
                SPCIdleTM = 0;
                SPCHomeTM = 0;
                SPCManualTM = 0;
                SPCRunSecond = 0;
                SPCStopSecond = 0;
            }
        }
    }
}
