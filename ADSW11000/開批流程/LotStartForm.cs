using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KCSDK;
using ProVLib;

namespace ProVSimpleProject
{
    public partial class LotStartForm : BaseLotStartForm
    {
        public LotStartForm()
        {
            InitializeComponent();
        }

        private void uiActor2_OnUpdateUI(ProVLib.ProVMessage msg)
        {
            LotStartResult = ActionDT.ResultType.OK;
        }

        public override void LotStart()
        {
            LotStartResult = ActionDT.ResultType.None;

            ProVMessage msg = new ProVMessage();
            msg.SetParameter("PackageName", SYSPara.PackageName);
            uiActor1.AddMessage(msg);
        }
    }
}
