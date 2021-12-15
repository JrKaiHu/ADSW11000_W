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
    public partial class PMItemF : Form
    {
        public PMItem m_Item;
        private bool m_Reset = false;

        public PMItemF()
        {
            InitializeComponent();
        }

        public PMItemF(ref PMItem Item, bool bReset = false)
        {
            InitializeComponent();
            m_Item = Item;
            cboPMCycle.SelectedIndex = 0;
            m_Reset = bReset;
            if (m_Reset)
            {
                this.Size = new System.Drawing.Size(330, 157);
                btnAdd.Text = "確定";
                btnCancel.Visible = false;
                btnAdd.Location = new Point(100, 94);
                txtItem.Text = m_Item.ItemName;

            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (m_Reset)
            {
                m_Item.ItemName = txtItem.Text;
                m_Item.PMCycle = (ePMCycle)cboPMCycle.SelectedIndex;
                m_Item.PMDate = dtPickerPM.Value;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;

            }
            else
            {
                if (!String.IsNullOrEmpty(txtItem.Text) && m_Item != null)
                {
                    m_Item.ItemName = txtItem.Text;
                    m_Item.PMCycle = (ePMCycle)cboPMCycle.SelectedIndex;
                    m_Item.PMDate = dtPickerPM.Value;
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
            }
        }
    }
}
