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

namespace ADSW11000
{
    public partial class LotStartForm : BaseLotStartForm
    {
        public LotStartForm()
        {
            InitializeComponent();
        }

        public override void LotStart()
        {
            LotStartResult = ActionDT.ResultType.None;

            ProVMessage msg = new ProVMessage();

            msg.SetParameter("RESULT", true);
            uiActor1.AddMessage(msg);
        }

        private ProVMessage uiActor1_OnActorRun(ProVMessage msg)
        {
            PMSettingF pmSetForm = new PMSettingF(false);
            if (pmSetForm.HasPMItem)
            {
                pmSetForm.ShowDialog();
                if (pmSetForm.DialogResult == System.Windows.Forms.DialogResult.Abort)
                {
                    msg.SetParameter("RESULT", false);
                }
                else
                {
                    msg.SetParameter("RESULT", true);
                }
            }
            return msg;            
        }

        private ProVMessage uiActor2_OnActorRun(ProVMessage msg)
        {
            bool Result = (bool)msg.GetParameter("RESULT");
            LotStartResult = Result ? ActionDT.ResultType.OK : ActionDT.ResultType.NG;
            return default(ProVLib.ProVMessage);
        }
    }
}
