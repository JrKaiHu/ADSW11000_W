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
using ProVTool;
using ProVIFM;

namespace ADSW11000
{
    //使用者修改 (產品設定參數放置處)
    public partial class PackageF : BasePackageF
    {
        public PackageF()
        {
            InitializeComponent();

            //定義表格名稱，會影響Log記錄內的DataSource名稱
            ModuleName = "Package";

            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";
        }

        #region 架構使用 (Public)

        //產品切換後觸發此事件
        public override void SelectChange()
        {
        }

        //產品儲存後觸發此事件
        public override void AfterSave()
        {
        }

        //關聯式資料庫使用
        public override void InitialSubPackage()
        {
            #region 使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)
            //tFieldCB1.EditForm = FormSet.mToolF;
            //FormSet.mToolF.Tag = this;
            //if (SYSPara.IsAutoMode)
            //    FormSet.mToolF.SetFileName(SYSPara.PReadValue("Tool").ToString(), "", true);
            //BladeZ1Data.EditForm = FormSet.mBladeZ1Data;
            //FormSet.mToolF.Tag = this;
            //if (SYSPara.IsAutoMode)
            //    FormSet.mToolF.SetFileName(SYSPara.PReadValue("Tool").ToString(), "", true);
            #endregion
        }

        #endregion

        #region 架構使用 (Private)

        #endregion

    }
}
