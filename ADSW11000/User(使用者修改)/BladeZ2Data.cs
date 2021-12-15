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
    public partial class BladeZ2Data : BasePackageF
    {
        public BladeZ2Data()
        {
            InitializeComponent();
            ModuleName = "BladeZ2Data";
            FolderPath = SYSPara.SysDir + @"\LocalData\SubPackage\BladeZ2Data\";
        }
    }
}
