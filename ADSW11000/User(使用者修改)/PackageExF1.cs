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

namespace ProVSimpleProject
{
    //使用者修改 (產品設定參數放置處)
    public partial class PackageExF1 : BasePackageExF1
    {
        private List<DataManagement> TableList = new List<DataManagement>(); //PSetValue 修改加入

        public PackageExF1()
        {
            InitializeComponent();
            TableList.Clear();

            //使用者修改注意，如有新增關聯式表格，需在建構函數加入這行，並命名
            //ModuleName = "Package";
            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";

            #region 使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)

            #endregion
        }

        public void Initial()
        {
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

        public void ShowUpdate(bool bShow)
        {
            btnUpdate.Visible = bShow;
        }

        //取得指定欄位值 (關聯式不受表格不同而影響)
        public DataTransfer ReadValue(string FieldName)
        {
            return PackageData.ReadValue(FieldName);
        }

        //設定產品的最後一次使用日期
        public void SetLastTime()
        {
            PackageDS.SetLastTime("Package");
        }

        //PSetValue 修改加入
        public void SetValue(string TableName, string DataName, string FieldName, object Value, bool IsAuto, ValueDataType ValueType)
        {
            if (TableName == "")
            {
                PackageDS.ChangeData(DataManagement.DataType.Package, FieldName, Value, IsAuto, PackageDS.SelectPackage, ValueType);
                PackageDS.SaveFile();
            }
            else
            {
                string tablekey = PackageDS.ReadValue(DataManagement.DataType.Package, DataName).ToString();
                DataManagement pDS = (DataManagement)TableList.Find(x => x.SourceName == TableName);
                pDS.ChangeData(DataManagement.DataType.Package, FieldName, Value, IsAuto, tablekey, ValueType);
                pDS.SaveFile();
            }
        }

        public void PSetValue(string FieldName, object value, bool IsAuto, ValueDataType valuetype)
        {
            PackageDS.ChangeData(DataManagement.DataType.Package, FieldName, value, IsAuto, "Package", valuetype);
            PackageDS.SaveFile();

            foreach (BaseModuleInterface pmodule in FormSet.ModuleList)
                pmodule.PackageData.SetValue("", FieldName, value);
            FormSet.mPackage.PackageData.SetValue("", FieldName, value);
        }

        #endregion

        #region 架構使用 (Private)

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            FormSet.mMSS.PushPackageData();
            MessageBox.Show("更新資料完成");
        }

        private void AddTable(TFieldCB mFieldObj, BasePackageExF TargetForm)
        {
            TargetForm.ParentObj = this;
            mFieldObj.EditForm = TargetForm;
            mFieldObj.TableSource = TargetForm.PackageDS;
            TableList.Add(mFieldObj.TableSource); //PSetValue 修改加入
            TargetForm.PackageDS.SourceName = TargetForm.ModuleName;
            TargetForm.RefreshView();
            mFieldObj.TableLV = TargetForm.lvPackage;
        }

        #endregion

    }
}
