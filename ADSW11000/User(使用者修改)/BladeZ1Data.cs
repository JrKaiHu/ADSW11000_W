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
    public partial class BladeZ1Data : BasePackageF
    {
        public BladeZ1Data()
        {
            InitializeComponent();
            ModuleName = "BladeZ1Data";
            FolderPath = SYSPara.SysDir + @"\LocalData\SubPackage\BladeZ1Data\";
        }
    }
}
