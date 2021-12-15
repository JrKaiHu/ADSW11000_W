using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProVTool;

namespace ADSW11000
{
    public class EventReport
    {
        private List<RPTData> RPTList = new List<RPTData>();
        private ProVSECSEngine SECSEngine = null;
        private bool bEnabled = true;

        public delegate SECS_BASE QueryValueDelegate(String Name, String Format, TableCode Code);
        public event QueryValueDelegate QueryValueEvent;

        public EventReport(ProVSECSEngine SECSEngine, ushort CEID, String EvtName)
        {
            this.SECSEngine = SECSEngine;
            this.CEID = CEID;
            this.EventName = EvtName;
        }

        public bool Enabled
        {
            get
            {
                return bEnabled;
            }
            set
            {
                bEnabled = value;
            }
        }
         
        public String EventName
        {
            get;
            set;
        }

        public ushort CEID
        {
            get;
            set;
        }

        public RPTData this[int idx]
        {
            get
            {
                return RPTList.Find(x => x.RPTID == (ushort)(idx));
            }
        }

        public void AddRPT(ushort RPTID)
        {
            RPTData rptData = new RPTData(RPTID);
            RPTList.Add(rptData);
        }

        public void AddRPT(RPTData rptData)
        {
            RPTList.Add(rptData);
        }

        public void RemoveRPT(RPTData rptData)
        {
            if(rptData != null)
                RPTList.RemoveAll(x => x.RPTID == (ushort)(rptData.RPTID));
            else
                RPTList.Clear();
        }

        public List<RPTData> GetRPTList()
        {
            return RPTList;
        }

        public List<byte> EncodeEventReport(bool bAnnotated = false)
        {
            List<byte> sb = new List<byte>();
            /*
            *  L<3>
            *      DATAID
            *      CEID
            *      L<a>
            *          L<2>
            *              RPTID1
            *              L<b>
            *                  VID1
            *                  VID2
            *          L<2>
            *              RPTID2
            *              L<c>
            *                  VID1
            *                  VID2
            *                  VID3
            */
            sb.Clear();
            SECS_LIST lstData = new SECS_LIST(3);
            SECSEngine.EncodeDataItem(ref sb, lstData);
            SECS_U2 u2Data = new SECS_U2(CEID); //DATAID
            SECSEngine.EncodeDataItem(ref sb, u2Data);
            SECS_U2 u4Data = new SECS_U2(CEID); //CEID
            SECSEngine.EncodeDataItem(ref sb, u4Data);
            List<RPTData> ReportNum = GetRPTList();
            lstData = new SECS_LIST(ReportNum.Count);
            SECSEngine.EncodeDataItem(ref sb, lstData);

            foreach (RPTData rpt in ReportNum)
            {
                lstData = new SECS_LIST(2);
                SECSEngine.EncodeDataItem(ref sb, lstData);
                u2Data = new SECS_U2(rpt.RPTID); //RPTID
                SECSEngine.EncodeDataItem(ref sb, u2Data);
                List<VIDData> vidSet = rpt.GetVIDList();
                int NodeNum = vidSet.Count;
                if (bAnnotated)
                    NodeNum = NodeNum * 2;
                lstData = new SECS_LIST(NodeNum);
                SECSEngine.EncodeDataItem(ref sb, lstData);
                foreach(VIDData vid in vidSet)
                {
                    if (bAnnotated)
                    {
                        u2Data = new SECS_U2(vid.VID);
                        SECSEngine.EncodeDataItem(ref sb, u2Data);
                    }
                    SECS_BASE secsData = QueryValueEvent(vid.Name, vid.Format, vid.Code);  
                    SECSEngine.EncodeDataItem(ref sb, secsData);
                }
            }
            return sb;
        }
        
    }

    public class VIDData : IEquatable<VIDData>
    {
        public ushort VID { get; set; }
        public String Name { get; set; }
        public String Format { get; set; }
        public TableCode Code { get; set; }
        public SECS_BASE Min { get; set; }
        public SECS_BASE Max { get; set; }
        public String Unit { get; set; }
            

        public VIDData(ushort ID, String Name, String Format, TableCode Code, SECS_BASE Min, SECS_BASE Max, String Unit)
        {
            this.VID = ID;
            this.Name = Name;
            this.Format = Format;
            this.Code = Code;
            this.Min = Min;
            this.Max = Max;
            this.Unit = Unit;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            VIDData objAsPart = obj as VIDData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return VID;
        }

        public bool Equals(VIDData other)
        {
            if (other == null) return false;
            return (this.VID.Equals(other.VID));
        }
    }

    public class RPTData : IEquatable<RPTData>
    {
        private List<VIDData> VIDList = new List<VIDData>();
        public ushort RPTID { get; set; }
        public String Name { get; set; }

        public RPTData(ushort ID)
        {
            this.RPTID = ID;
            Name = String.Format("RPTID_{0}", RPTID);
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            RPTData objAsPart = obj as RPTData;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }

        public override int GetHashCode()
        {
            return RPTID;
        }

        public bool Equals(RPTData other)
        {
            if (other == null) return false;
            return (this.RPTID.Equals(other.RPTID));
        }

        public VIDData this[int idx]
        {
            get
            {
                return VIDList.Find(x => x.VID == (ushort)(idx));
            }
        }

        public void AddVID(VIDData vidData)
        {
            VIDList.Add(vidData);
        }

        public void RemoveVID(VIDData vidData)
        {
            if (vidData != null)
                VIDList.RemoveAll(x => x.VID == (ushort)(vidData.VID));
            else
                VIDList.Clear();
        }

        public List<VIDData> GetVIDList()
        {
            return VIDList;
        }
    }
}
