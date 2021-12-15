using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADSW11000
{
    public partial class DateSelectF : Form
    {
        public DateSelectF()
        {
            InitializeComponent();
        }

        private int TypeNo = 0;
        private DateTime RestoreTime;
        private string TypeName;
        private string FieldName;
        public void Initial(int typeno, DateTime restoretime, string typename)
        {
            TypeNo = typeno;
            RestoreTime = restoretime;
            TypeName = typename;

            label1.Text = string.Format("將類型「{0}」的所有欄位值還原至", TypeName);
            dateTimePicker1.Value = RestoreTime;
        }
        public void Initial(int typeno, DateTime restoretime, string typename, string fieldname)
        {
            TypeNo = typeno;
            RestoreTime = restoretime;
            TypeName = typename;
            FieldName = fieldname;

            label1.Text = string.Format("將類型「{0}」的欄位「{1}」值還原至", TypeName, FieldName);
            dateTimePicker1.Value = RestoreTime;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
