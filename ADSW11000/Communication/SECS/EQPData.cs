using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProVTool;

namespace ADSW11000
{
    public class ECData : IEquatable<ECData>
    {
        public ushort ECID
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String Format
        {
            get;
            set;
        }

        public TableCode Code
        {
            get;
            set;
        }

        public String Unit
        {
            get;
            set;
        }

        public SECS_BASE Min
        {
            get;
            set;
        }

        public SECS_BASE Max
        {
            get;
            set;
        }

        public SECS_BASE Value
        {
            get;
            set;
        }

        public ECData(ushort ECID, String Name, String Format, TableCode Code, String Unit, SECS_BASE Val, SECS_BASE Min, SECS_BASE Max)
        {
            this.ECID = ECID;
            this.Name = Name;
            this.Format = Format;
            this.Code = Code;
            this.Unit = Unit;
            this.Min = Min;
            this.Max = Max;
            this.Value = Val;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ECData objAsPart = obj as ECData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return ECID;
        }

        public bool Equals(ECData other)
        {
            if (other == null) return false;
            return (this.ECID.Equals(other.ECID));
        }
    }

    public class SVData : IEquatable<SVData>
    {
        public ushort SVID
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String Format
        {
            get;
            set;
        }

        public TableCode Code
        {
            get;
            set;
        }

        public SECS_BASE Min
        {
            get;
            set;
        }

        public SECS_BASE Max
        {
            get;
            set;
        }

        public String Unit
        {
            get;
            set;
        }

        public SVData(ushort SVID, String Name, String Format, TableCode Code, String Unit, SECS_BASE Min, SECS_BASE Max)
        {
            this.SVID = SVID;
            this.Name = Name;
            this.Format = Format;
            this.Code = Code;
            this.Unit = Unit;
            this.Min = Min;
            this.Max = Max;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SVData objAsPart = obj as SVData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return SVID;
        }

        public bool Equals(SVData other)
        {
            if (other == null) return false;
            return (this.SVID.Equals(other.SVID));
        }
    }

    public class DVData : IEquatable<DVData>
    {
        public ushort DVID
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String Format
        {
            get;
            set;
        }

        public TableCode Code
        {
            get;
            set;
        }

        public SECS_BASE Min
        {
            get;
            set;
        }

        public SECS_BASE Max
        {
            get;
            set;
        }

        public String Unit
        {
            get;
            set;
        }

        public DVData(ushort DVID, String Name, String Format, TableCode Code, String Unit, SECS_BASE Min, SECS_BASE Max)
        {
            this.DVID = DVID;
            this.Name = Name;
            this.Format = Format;
            this.Code = Code;
            this.Unit = Unit;
            this.Min = Min;
            this.Max = Max;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DVData objAsPart = obj as DVData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return DVID;
        }

        public bool Equals(DVData other)
        {
            if (other == null) return false;
            return (this.DVID.Equals(other.DVID));
        }
    }

    public class ALMData : IEquatable<ALMData>
    {
        public byte ALCD
        {
            get;
            set;
        }

        public String ALTX
        {
            get;
            set;
        }

        public ushort ALID
        {
            get;
            set;
        }

        public byte ALED
        {
            get;
            set;
        }

        public ALMData(byte ALCD, String ALTX, ushort ALID, byte ALED)
        {
            this.ALCD = ALCD;
            this.ALTX = ALTX;
            this.ALID = ALID;
            this.ALED = ALED;
        }

        public override string ToString()
        {
            return String.Format("E{0}", ALID);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ALMData objAsPart = obj as ALMData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return ALID;
        }

        public bool Equals(ALMData other)
        {
            if (other == null) return false;
            return (this.ALID.Equals(other.ALID));
        }
    }

    public class TraceDataCollection : IEquatable<TraceDataCollection>
    {
        public delegate SECS_BASE QuerySVDelegate(object sender, ushort SVID); //SVID = 0 --> Report to Host
        public event QuerySVDelegate QuerySVEvent;

        private System.Timers.Timer TRTimer;

        public TraceDataCollection()
        {
            TRTimer = new System.Timers.Timer(dsPeriod * 1000);
            TRTimer.Elapsed += TRTimer_Elapsed;
        }

        public ushort TRID //Trace Request ID
        {
            get;
            set;
        }

        //hhmmss
        private ushort dsPeriod = 1;
        public ushort DSPER //Data Samplng Period
        {
            get
            {
                return dsPeriod;
            }
            set
            {
                dsPeriod = value;
                TRTimer.Interval = dsPeriod * 1000;
            }
        }

        void TRTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TRTimer.Stop();
            if (QuerySVEvent != null)
            {
                SMPLN++;
                SECS_BASE sv = null;
                foreach (ushort svid in SVDataList)
                {
                    sv = QuerySVEvent(this, svid);
                    TraceReportList.Add(sv);
                }

                if (SMPLN % REPGSZ == 0)
                {
                    QuerySVEvent(this, 0);
                    TraceReportList.Clear();
                }

                if (SMPLN == TOTSMP)
                {
                    Stop();
                    SVDataList.Clear();
                }
                else
                {
                    TRTimer.Start();
                }
            }
        }

        public ushort TOTSMP //Total Samples to be Made
        {
            get;
            set;
        }

        public ushort REPGSZ //Report Group Size
        {
            get;
            set;
        }

        public ushort SMPLN //Sample Number
        {
            get;
            set;
        }

        public String STIME //Sample Time
        {
            get;
            set;
        }

        //Note:
        //The number of trace reports sent to the host is determined by 
        //(TOTSMP)/(REPGSZ)


        private List<SECS_BASE> traceReportList = new List<SECS_BASE>();
        public List<SECS_BASE> TraceReportList
        {
            get
            {
                return traceReportList;
            }
        }

        private List<ushort> svDataList = new List<ushort>();
        public List<ushort> SVDataList
        {
            get
            {
                return svDataList;
            }
        }

        public void Start()
        {
            SMPLN = 0;
            TRTimer.Start();
        }

        public void Stop()
        {
            SMPLN = 0;
            TRTimer.Stop();
            QuerySVEvent = null;
        }

        public override string ToString()
        {
            return String.Format("E{0}", TRID);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            TraceDataCollection objAsPart = obj as TraceDataCollection;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return TRID;
        }

        public bool Equals(TraceDataCollection other)
        {
            if (other == null) return false;
            return (this.TRID.Equals(other.TRID));
        }
    }

    public class SnFnData : IEquatable<SnFnData>
    {
        private byte mSType;
        private byte mFType;
        private bool bEnableSpool = false;


        public SnFnData(byte S, byte F)
        {
            mSType = S;
            mFType = F;
        }

        public byte SType
        {
            get
            {
                return mSType;
            }
        }

        public byte FType
        {
            get
            {
                return mFType;
            }
        }

        public bool EnableSpool
        {
            get
            {
                return bEnableSpool;
            }
            set
            {
                bEnableSpool = value;
            }
        }

        public override string ToString()
        {
            return String.Format("S{0}F{1}", SType, FType);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SnFnData objAsPart = obj as SnFnData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return SType * 100 + FType;
        }

        public bool Equals(SnFnData other)
        {
            if (other == null) return false;
            return (this.SType == other.SType && this.FType == other.FType);
        }
    }

    public class SpoolData : IEquatable<SpoolData>
    {
        private byte mSType;
        private byte mFType;
        private byte[] bData;
        private bool mW_Bit = true;
        private uint ulSystemBytes;

        public Byte[] Data
        {
            get
            {
                return bData;
            }
        }

        public byte SType
        {
            get
            {
                return mSType;
            }
        }

        public byte FType
        {
            get
            {
                return mFType;
            }
        }

        public bool W_Bit
        {
            get
            {
                return mW_Bit;
            }
        }

        public uint SystemBytes
        {
            get
            {
                return ulSystemBytes;
            }
            set
            {
                ulSystemBytes = value;
            }
        }

        public SpoolData(byte S, byte F, bool W_Bit, byte[] Data, uint sysbyte)
        {
            mSType = S;
            mFType = F;
            this.mW_Bit = W_Bit;
            this.bData = Data;
            this.ulSystemBytes = sysbyte;
        }

        public override string ToString()
        {
            return String.Format("S{0}F{1}", SType, FType);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SpoolData objAsPart = obj as SpoolData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return SType * 100 + FType;
        }

        public bool Equals(SpoolData other)
        {
            if (other == null) return false;
            return (this.SType.Equals(other.SType) && this.FType.Equals(other.FType));
        }
    }

    public class SpoolDataCollection
    {
        private object oLock = new object();
        private List<SpoolData> SpoolList = new List<SpoolData>();

        public SpoolData this[int idx]
        {
            get
            {
                SpoolData SpoolData = null;
                lock(oLock)
                {
                    SpoolData = SpoolList[idx];
                }
                return SpoolData;
            }
        }

        public String SpoolStartTime
        {
            get;
            set;
        }

        public int SpoolCountTotal
        {
            get;
            set;
        }

        public int Count
        {
            get
            {
                return SpoolList.Count;
            }
        }

        public void Clear()
        {
            SpoolList.Clear();
            SpoolCountTotal = 0;
        }

        public void Add(SpoolData data)
        {
            lock (oLock)
            {
                SpoolList.Add(data);
                SpoolCountTotal++;
            }
        }

        public void RemoveAt(int idx)
        {
            lock (oLock)
            {
                SpoolList.RemoveAt(idx);
            }
        }

        public bool WriteSpoolData(SpoolData sData)
        {
            String strData = string.Join(",", sData.Data);
            return true;
        }

        public bool ReadSpoolData()
        {
            String strData = "1,2,3,4,5,6,7,8,9";
            Byte[] bArray = strData.Split(',').Select(h => Byte.Parse(h)).ToArray();
            return true;
        }

    }

    //+ By Max 20190503
    public class PKGItem
    {
        public PKGItem(String Name, String QueryName, String Format)
        {
            this.Name = Name;
            this.QueryName = QueryName;
            this.Format = Format;
        }

        public String Name
        {
            get;
            set;
        }

        public String QueryName
        {
            get;
            set;
        }

        public String Format
        {
            get;
            set;
        }

        public SECS_BASE Data
        {
            get;
            set;
        }

        public string sData
        {
            get
            {
                return ((SECS_ASCII)Data).Data;
            }
        }

        public bool bData
        {
            get
            {
                return ((SECS_BOOLEAN)Data).Data;
            }
        }

        public int iData
        {
            get
            {
                return ((SECS_I4)Data).Data;
            }
        }

        public double fData
        {
            get
            {
                return ((SECS_F8)Data).Data;
            }
        }
    }

    public class PackageSchema
    {
        private List<PKGItem> ItemList = new List<PKGItem>();

        public void Add(PKGItem Item)
        {
            ItemList.Add(Item);
        }

        public int Count
        {
            get { return ItemList.Count; }
        }

        public PKGItem this[int idx]
        {
            get
            {
                return ItemList[idx];
            }
        }
    }
}
