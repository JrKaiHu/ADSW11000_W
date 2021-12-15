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

namespace ProVSimpleProject
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

        private void uiActor2_OnUpdateUI(ProVMessage msg)
        {
            LotEndResult = ActionDT.ResultType.OK;
        }

        private void uiActor1_OnUpdateUI(ProVMessage msg)
        {
            FormSet.mMainF1.LB_出料堆疉資訊.Items.Clear();
            FormSet.mMainF1.LB_出料堆疉資訊.Items.AddRange(FormSet.mMainFlow.出料列表.ToArray());
        }
    }
}
