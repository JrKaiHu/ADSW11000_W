using System;
using System.Threading.Tasks;
using System.Data;

namespace Saw
{
    class CutPos
    {
        public int YCutEndPos = 0;
        public int YCutStartPos = 0;
        public int CutZ1 = 0;
        public int CutZ2 = 0;
        public int SafeZ = 0;
        public int Step = 0;
        public int TempX1 = 0;
        public int TempX2 = 0;
        public int TempU = 0;
        public DataTable CutData = new DataTable();

        public void Reset()
        {
            YCutStartPos = 0;
            YCutEndPos = 0;
            CutZ1 = 0;
            CutZ2 = 0;
            SafeZ = 0;
            Step = 0;
            TempX1 = 0;
            TempX2 = 0;
            TempU = 0;
            CutData = new DataTable();

            DataColumn[] CutData_dc = new DataColumn[10];

            CutData_dc[0] = new DataColumn("Cut_Index", System.Type.GetType("System.String"));
            CutData_dc[1] = new DataColumn("Cut_Z1使用", System.Type.GetType("System.String"));
            CutData_dc[2] = new DataColumn("Cut_Z1刀順", System.Type.GetType("System.String"));
            CutData_dc[3] = new DataColumn("Cut_Z2使用", System.Type.GetType("System.String"));
            CutData_dc[4] = new DataColumn("Cut_Z2刀順", System.Type.GetType("System.String"));
            CutData_dc[5] = new DataColumn("Cut_高度偏移", System.Type.GetType("System.String"));
            CutData_dc[6] = new DataColumn("Cut_切割方向", System.Type.GetType("System.String"));
            CutData_dc[7] = new DataColumn("Cut_料片方向", System.Type.GetType("System.String"));
            CutData_dc[8] = new DataColumn("Cut_速度", System.Type.GetType("System.String"));
            CutData_dc[9] = new DataColumn("Cut_入板速度", System.Type.GetType("System.String"));

            CutData.Columns.AddRange(CutData_dc);

        }
    }
}
