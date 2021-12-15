using System;
using System.Drawing;
using System.Data;

namespace Saw
{
    class FixturePos
    {
        public string strSide = "";// Top、Right、Left、Bottom  
        public int nTempθ = 0;
        public int nLineNo = 0;
        public int nHighBaseFocus = 0;

        public Point ptLTPos = new Point(0, 0);//學膠墊左上角
        public Point ptLBPos = new Point(0, 0);//學膠墊左下角

        public DataTable dtFixtureData = new DataTable();

        public void Reset()
        {
            strSide = "";
            nTempθ = 0;
            nLineNo = 0;
            nHighBaseFocus = 0;

            ptLTPos = new Point(0, 0);//學膠墊左上角
            ptLBPos = new Point(0, 0);//學膠墊左下角

            dtFixtureData = new DataTable();

            DataColumn[] FixtureData_dc = new DataColumn[5];

            FixtureData_dc[0] = new DataColumn("Fixture_LineNo" , System.Type.GetType("System.String"));
            FixtureData_dc[1] = new DataColumn("Fixture_Side"   , System.Type.GetType("System.String"));
            FixtureData_dc[2] = new DataColumn("Fixture_X"      , System.Type.GetType("System.String"));
            FixtureData_dc[3] = new DataColumn("Fixture_Angle"  , System.Type.GetType("System.String"));
            FixtureData_dc[4] = new DataColumn("Fixture_XOffset", System.Type.GetType("System.String"));

            dtFixtureData.Columns.AddRange(FixtureData_dc);
        }
    }
}
