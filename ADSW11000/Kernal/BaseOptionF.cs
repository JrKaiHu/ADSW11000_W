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

namespace ADSW11000
{
    //使用者修改 (設備通用參數放置處)
    public partial class BaseOptionF : Form
    {
        public BaseOptionF()
        {
            InitializeComponent();

            #region 架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            #endregion
        }

        public BaseOptionF(string TableName)
        {
            InitializeComponent();

            #region 架構使用

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            RefreshAllData(TableName);

            #endregion
        }

        public virtual void Initial()
        {
        }

        public void RefreshAllData(string TableName)
        {
            OptionDS.Initial(string.Format(@"{0}\LocalData\{1}.xml", SYSPara.SysDir, TableName), TableName);
            DataSnippet.DataGetAll(this);
            DataSnippet.ControlEnable(this, false);
        }

        //使用者權限切換後的頁面變化控制
        public virtual void ChangeUser()
        {
        }

        //關聯式子表格相關設定
        public virtual void InitialSubPackage()
        {
        }

        #region 架構使用

        public bool IsLogExist()
        {
            return OptionDS.IsLogExist();
        }

        public string GetLog()
        {
            return OptionDS.GetLog();
        }

        public void ExitClick()
        {
            DataSnippet.ControlEnable(this, false);
            Parent = null;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「結束」-[通用設定]");
            DataSnippet.ControlEnable(this, false);
            Parent = null;
        }

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「編輯」-[通用設定]");
            DataSnippet.ControlEnable(this, true, SYSPara.IsAutoMode);
        }

        private void btnSave2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「儲存」-[通用設定]");

            DataSnippet.ControlEnable(this, false);
            DataSnippet.DataSetAll(this);
            OptionDS.SaveFile();
        }

        private void btnCancel2_Click(object sender, EventArgs e)
        {
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下「取消」-[通用設定]");

            DataSnippet.DataGetAll(this);
            DataSnippet.ControlEnable(this, false);
        }

        #endregion
    }
}
