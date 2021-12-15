using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVLib;
using KCSDK;

namespace ADSW11000
{
    public partial class LotEndForm : BaseLotEndForm
    {
        public LotEndForm()
        {
            InitializeComponent();
        }

        public override void LotEnd()
        {
            LotEndResult = KCSDK.ActionDT.ResultType.None;

            ProVMessage msg = new ProVMessage();
            msg.SetParameter("PackageName", SYSPara.PackageName);
            uiActor1.AddMessage(msg);
        }

        private ProVMessage uiActor1_OnActorRun(ProVMessage msg)
        {
            return msg;
        }

        private ProVMessage uiActor2_OnActorRun(ProVMessage msg)
        {
            LotEndResult = ActionDT.ResultType.OK;
            return default(ProVMessage);
        }
    }
}
