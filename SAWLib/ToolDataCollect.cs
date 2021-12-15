using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;
using KCSDK;

namespace SAWLib
{
    public partial class ToolDataCollect : Component
    {
        public class SummaryType
        {
            public int ChannelNo;
            public int DataCount;
            public double AVGVolt;
            public double MaxVolt;
            public double MinVolt;
            public int OverCount;
            public bool IsBroken;
        }

        #region 私有變數
        private double[] m_dataScaled;
        private double m_xInc;
        private KCSDK.GetTickCountEx tick = new GetTickCountEx();
        private double starttm = 0;

        private List<ToolDetect> ToolList = new List<ToolDetect>();

        // Instantiate & Initialize the FFT class
        DSPLib.FFT fft = new DSPLib.FFT();
        #endregion

        #region 公有變數
        
        public bool DeviceConnected;
        public List<SummaryType> SummaryList = new List<SummaryType>();
        public double ScanTM = 0;
        #endregion

        #region 私有變數
        int sectionLength = 0;
        int channelCount = 0;
        #endregion

        #region 控制項屬性

        private string deviceName = "PCIE-1810,BID#0";
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; } 
        }


        private WaveformAiCtrl mAIDevice = null;
        //[Browsable(true), Category("#參數設定"), Description("裝置選擇")]
        //public WaveformAiCtrl AIDevice
        //{
        //    get { return mAIDevice; }
        //    set
        //    {
        //        mAIDevice = value;
        //        if (mAIDevice != null)
        //            DeviceName = mAIDevice.SelectedDevice.Description;
        //        else
        //        {
        //            DeviceConnected = false;
        //            DeviceName = "";
        //        }
        //    }
        //}

        private double mMaxVolt = 5;
        [Browsable(true), Category("#參數設定"), Description("最大電壓值")]
        public double MaxVolt
        {
            get { return mMaxVolt; }
            set { mMaxVolt = value; }
        }

        private double mMinVolt = 0;
        [Browsable(true), Category("#參數設定"), Description("最小電壓值")]
        public double MinVolt
        {
            get { return mMinVolt; }
            set { mMinVolt = value; }
        }

        private double mOverValue = 0.5;
        [Browsable(true), Category("#參數設定"), Description("判定破刀電壓門檻值(相對於平均值)")]
        public double OverValue
        {
            get { return mOverValue; }
            set { mOverValue = value; }
        }

        private int mOverCount = 10;
        [Browsable(true), Category("#參數設定"), Description("破刀訊號數量限制")]
        public int OverCount
        {
            get { return mOverCount; }
            set { mOverCount = value; }
        }

        #endregion

        public ToolDataCollect()
        {
            InitializeComponent();
            DeviceConnected = false;
            ToolList.Clear();
            SummaryList.Clear();
        }

        public ToolDataCollect(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public void AddDetect(ToolDetect obj)
        {
            ToolList.Add(obj);
            SummaryType sum = new SummaryType();
            sum.IsBroken = false;
            sum.ChannelNo = obj.AIChannel;
            SummaryList.Add(sum);
        }

        public void DelDetect(ToolDetect obj)
        {
            int index = ToolList.FindIndex(x => x==obj);
            SummaryList.RemoveAt(index);
            ToolList.Remove(obj);
        }

        //初始化 0:Initial OK 1:Initial Error 2:No Setting
        public int Initial(int IsSimulation)
        {
            if (IsSimulation != 0 && deviceName != "DemoDevice,BID#0")
            {
                DeviceConnected = false;
                return 0;
            }

            //動態建立DAQ物件
            mAIDevice = new WaveformAiCtrl();
            mAIDevice.SelectedDevice = new DeviceInformation(deviceName);
            Conversion conversion = mAIDevice.Conversion;
            conversion.ChannelStart = 0; //從Channel 0開始掃描
            conversion.ChannelCount = 2; //總共有2個Channel
            conversion.ClockRate = 262144; //取樣率設定為262144Hz

            Record record = mAIDevice.Record;
            record.SectionCount = 1; //0:連續取樣　1: 等待Start訊號，每次取一次資料
            record.SectionLength = 131072;　//一個Channel的資料長度設定為131072
            //以上設定做完FFT後，頻率解析度為2Hz

            DeviceConnected = false;
            if (mAIDevice == null)
                return 2;

            if (!mAIDevice.Initialized)
                return 1;

            DeviceName = mAIDevice.SelectedDevice.Description;

            channelCount = mAIDevice.Conversion.ChannelCount;
            sectionLength = mAIDevice.Record.SectionLength;
            m_dataScaled = new double[channelCount * sectionLength];

            DeviceConnected = true;
            mAIDevice.DataReady += AIDevice_DataReady;
            fft.Initialize((uint)mAIDevice.Record.SectionLength, 0); //No Zero Padding
            return 0;
        }

        //資料收集 & 分析
        void AIDevice_DataReady(object sender, BfdAiEventArgs e)
        {
            try
            {
                // Calculate the elapsed time, 此次DataReady至下一次DataReady的時間，目前設定約500ms
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(String.Format("Elasped :{0} ms", stopwatch.ElapsedMilliseconds / 1.0));
                //The WaveformAiCtrl has been disposed.
                if (mAIDevice.State == ControlState.Idle)
                    return;

                //理論上此if判斷是永遠都不會成立
                if (m_dataScaled.Length < e.Count)
                    m_dataScaled = new double[e.Count];

                ErrorCode err = ErrorCode.Success;
                err = mAIDevice.GetData(e.Count, m_dataScaled);
                if (err != ErrorCode.Success && err != ErrorCode.WarningRecordEnd)
                    return;

                System.Diagnostics.Debug.WriteLine(String.Format("Data Count:{0}",e.Count));
                try
                {
                    // Find the power of two for the total FFT size up to 2^32
                    bool foundIt = false;
                    for (int mLogN = 1; mLogN <= 32; mLogN++)
                    {
                        double n = Math.Pow(2.0, mLogN);
                        if (e.Count == n)
                        {
                            foundIt = true;
                            break;
                        }
                    }
                    if (foundIt)
                        DataAnalysisFFT();
                }
                catch (Exception)
                {
                }
            }
            catch (System.Exception) { }
        }

        Stopwatch stopwatch = new Stopwatch();
        public double[] magLog;
        public double[] fSpan;
        public double[] mData;
        public double[] mDataOrg;
        private void DataAnalysisFFT()
        {
            int channel;
            mData = new double[sectionLength]; //一個Channel長度資料131072
            for (int i = 0; i < ToolList.Count; i++)
            {
                channel = ToolList[i].AIChannel;
                SummaryList[i].DataCount = sectionLength;
                
                //Channel 資料收集
                for (int j = 0, index = channel; j < sectionLength; j++, index += channelCount)
                    mData[j] = m_dataScaled[index];

                //求平均準位
                SummaryList[i].AVGVolt = mData.Average();
                SummaryList[i].MaxVolt = mData.Max();
                SummaryList[i].MinVolt = mData.Min();

                mDataOrg = new double[3000];

                Array.Copy(mData, 0, mDataOrg, 0, 3000); 

                mData = DCOffset(mData, -SummaryList[i].AVGVolt);

                System.Numerics.Complex[] cpxResult = fft.Execute(mData);

                // Calculate the frequency span
                fSpan = fft.FrequencySpan((uint)mAIDevice.Conversion.ClockRate);

                // Convert Magnitude
                double[] mag = DSPLib.DSP.ConvertComplex.ToMagnitude(cpxResult);

                //將訊號乘以10倍
                mag = DSPLib.DSP.Math.Multiply(mag, 10);
                //再將訊號自己成自己，把差值凸顯出來
                magLog = DSPLib.DSP.ConvertMagnitude.ToMagnitudeSquared(mag);
                //1. FFT結果如果有某頻率（除了直流0Hz）的Magnitude大於0.5則判定破刀
                //2. 頻率檢查由100Hz（6000RPM）開始
                int spanIndex = FindSpanIndex(fSpan);
                int copyCount = magLog.Count() - spanIndex;
                mag = new double[copyCount];

                Array.Copy(magLog, spanIndex, mag, 0, copyCount);

                bool bBroken = mag.Any(x => x > 0.5);
                if (SummaryList[i].AVGVolt >= 2.8f) //Average Voltage Broken
                {
                    SummaryList[i].OverCount++;
                    if (SummaryList[i].OverCount > OverCount)
                    {
                        SummaryList[i].IsBroken = true;
                        ToolList[i].IsBroken = SummaryList[i].IsBroken;
                        ToolList[i].BrokenValue = SummaryList[i].AVGVolt;
                    }
                    ToolList[i].Value = SummaryList[i].AVGVolt;
                    double d = (SummaryList[i].AVGVolt - MinVolt) / (MaxVolt - MinVolt) * 100.0;
                    ToolList[i].Percentage = (int)d;
                }
                else if (bBroken) //FFT Detected Broken
                {
                    SummaryList[i].IsBroken = true;
                    ToolList[i].IsBroken = SummaryList[i].IsBroken;
                    double d = mag.Max();
                    if (d >= 2.8) d = 2.8f;
                    d = (d - MinVolt) / (MaxVolt - MinVolt) * 100.0;
                    ToolList[i].Percentage = (int)d;
                    ToolList[i].BrokenValue = 2.8;
                    ToolList[i].Value = 2.8;
                }
                else //Blade Fine
                {
                    SummaryList[i].OverCount = 0;
                    SummaryList[i].IsBroken = false;
                    ToolList[i].IsBroken = SummaryList[i].IsBroken;
                    ToolList[i].Value = SummaryList[i].AVGVolt;
                    double d = (SummaryList[i].AVGVolt - MinVolt) / (MaxVolt - MinVolt) * 100.0;
                    ToolList[i].Percentage = (int)d;
                }
            }

            if (starttm == 0)
                starttm = tick.Value;
            else
            {
                ScanTM = tick.Value - starttm;
                starttm = tick.Value;
            }
            //取下一筆資料
            Start();
            // Start a Stopwatch
            stopwatch.Restart();
        }

        private static Double[] DCOffset(Double[] a, double dcV)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + dcV;
            }
            return result;
        }

        private static int FindSpanIndex(double[] span)
        {
            int i = 0;
            int spanCount = span.Count();
            for (i = 0; i < spanCount; ++i)
            {
                if (span[i] >= 100)
                {
                    break;
                }
            }
            return i;
        }


        //private void DataAnalysis()
        //{
        //    int channel;
        //    double[] mData = new double[AIDevice.Record.SectionLength];
        //    int sectionLength = AIDevice.Record.SectionLength;
        //    for (int i = 0; i < ToolList.Count; i++)
        //    {
        //        channel = ToolList[i].AIChannel;
        //        SummaryList[i].DataCount = sectionLength;

        //        //Channel 資料收集
        //        for (int j = 0, index = channel; j < sectionLength; j++, index += AIDevice.Conversion.ChannelCount)
        //            mData[j] = m_dataScaled[index];

        //        //求平均準位
        //        SummaryList[i].AVGVolt = mData.Average();
        //        SummaryList[i].MaxVolt = mData.Max();
        //        SummaryList[i].MinVolt = mData.Min();

        //        //求超過破刀電壓門檻值的訊號數量
        //        SummaryList[i].OverCount = 0;
        //        for (int j = 0; j < mData.Length; j++)
        //            if ((mData[j] > (SummaryList[i].AVGVolt + OverValue)) || (mData[j] < (SummaryList[i].AVGVolt - OverValue)))
        //                SummaryList[i].OverCount++;
        //        if (SummaryList[i].OverCount > OverCount)
        //            SummaryList[i].IsBroken = true;

        //        double d = (SummaryList[i].AVGVolt - MinVolt) / (MaxVolt - MinVolt) * 100.0;
        //        ToolList[i].Percentage = (int)d;
        //    }

        //    if (starttm == 0)
        //        starttm = tick.Value;
        //    else
        //    {
        //        ScanTM = tick.Value - starttm;
        //        starttm = tick.Value;
        //    }
        //}

        //重設破刀訊號
        public void ResetChannel(int ChannelNo)
        {
            for (int i = 0; i < ToolList.Count; i++)
            {
                if (SummaryList[i].ChannelNo == ChannelNo)
                {
                    SummaryType sum = SummaryList[i];
                    sum.IsBroken = false;
                    ToolList[i].IsBroken = false;
                    ToolList[i].BrokenValue = 0;
                }
            }
        }

        //開始監控 0:OK 1:Error
        public int Start()
        {
            if (!DeviceConnected)
                return - 1;

            ErrorCode err = ErrorCode.Success;

            err = mAIDevice.Prepare();
            if (mAIDevice.Conversion == null)
                return 1;
           
            //m_xInc = 1.0 / AIDevice.Conversion.ClockRate;
            if (err == ErrorCode.Success)
                err = mAIDevice.Start();

            if (err != ErrorCode.Success)
                return 1;
            return 0;
        }

        //停止監控 0:OK 1:Error
        public int Stop()
        {
            if (!DeviceConnected)
                return -1;
            ErrorCode err = ErrorCode.Success;
            if (mAIDevice.Conversion == null)
                return 1;

            err = mAIDevice.Stop();
            if (err != ErrorCode.Success)
                return 1;
            Array.Clear(m_dataScaled, 0, m_dataScaled.Length);
            starttm = 0;
            return 0;
        }
    }
}
