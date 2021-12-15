using System;
using System.Drawing;
using System.Data;

namespace Saw
{
    class MarkPos
    {
        public string strSide = "";// Top、Right、Left、Bottom 
        public int nStep = 0;
        public int nTempθ = 0;
        public int nLeftθ = 0;
        public int nRightθ = 0;
        public int nLineNo = 0;
        public int nTempX = 0;
        public int nCurrentPos = 0;
        public int nOffsetX = 0;
        public int nTempY = 0;
        public int nHighBaseFocus = 0;
        public int nFindMarkStep = 0;//尋靶用
        public int nSearchMarkStep = 0;//九宮格掃靶

        public Point ptLTMarkPos = new Point(0, 0);//學靶左上角
        public Point ptLBMarkPos = new Point(0, 0);//學靶左上角
        public Point ptRTMarkPos = new Point(0, 0);//學靶右上角
        public Point ptRBMarkPos = new Point(0, 0);//學靶右下角

        public DataTable dtScanData = new DataTable();

        public void Reset()
        {
            strSide = "";
            nStep = 0;
            nTempθ = 0;
            nLeftθ = 0;
            nRightθ = 0;
            nLineNo = 0;
            nTempX = 0;
            nTempY = 0;
            nHighBaseFocus = 0;
            nFindMarkStep = 0;//尋靶用
            nSearchMarkStep = 0;//九宮格掃靶

            ptLTMarkPos = new Point(0, 0);//學靶左上角
            ptLBMarkPos = new Point(0, 0);//學靶左上角
            ptRTMarkPos = new Point(0, 0);//學靶右上角
            ptRBMarkPos = new Point(0, 0);//學靶右下角

            dtScanData = new DataTable();

            DataColumn[] dc = new DataColumn[9];

            dc[0] = new DataColumn("Scan_LineNo",  System.Type.GetType("System.String"));
            dc[1] = new DataColumn("Scan_Side",    System.Type.GetType("System.String"));
            dc[2] = new DataColumn("Scan_X1",      System.Type.GetType("System.String"));
            dc[3] = new DataColumn("Scan_Y1",      System.Type.GetType("System.String"));
            dc[4] = new DataColumn("Scan_X2",      System.Type.GetType("System.String"));
            dc[5] = new DataColumn("Scan_Y2",      System.Type.GetType("System.String"));
            dc[6] = new DataColumn("Scan_Z",       System.Type.GetType("System.String"));
            dc[7] = new DataColumn("Scan_Angle",   System.Type.GetType("System.String"));
            dc[8] = new DataColumn("Scan_XOffset", System.Type.GetType("System.String"));

            dtScanData.Columns.AddRange(dc);

        }
    }
}
