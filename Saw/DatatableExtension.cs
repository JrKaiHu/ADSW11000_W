using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace Saw
{
    public static class DatatableExtension
    {
        public static DataTable Sort(this DataTable dt, string strCondition)
        {
            DataView dv = dt.DefaultView;
            dv.Sort = strCondition;
            return dv.ToTable();
        }

        public static void MoveSpecificRowToLast(this DataTable dt, string strCondition)
        {
            DataRow row = dt.Select(strCondition)[0];
            DataRow newRow = dt.NewRow();
            newRow.ItemArray = row.ItemArray;
            dt.Rows.Add(newRow);
            row.Delete();
        }
    }
}
