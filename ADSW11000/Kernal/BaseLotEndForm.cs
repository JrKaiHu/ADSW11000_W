﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVIFM;
using ProVLib;
using KCSDK;
using CommonObj;

namespace ADSW11000
{
    //使用者修改 (主控流程)
    public partial class BaseLotEndForm : Form
    {
        public ActionDT.ResultType LotEndResult = ActionDT.ResultType.None; //開批結果
        public BaseLotEndForm()
        {
            InitializeComponent();

            TopLevel = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            IntPtr frmptr = this.Handle;
        }
       
        public virtual void LotEnd()
        {
        }
    }
}
