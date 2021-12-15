using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProVIFM;

namespace ADSW11000
{
    public partial class ModuleContainerForm : Form
    {
        private object LastBtn;
        private object ExitBtn;
        public ModuleContainerForm()
        {
            InitializeComponent();

            this.TopLevel = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            LastBtn = null;
            ExitBtn = button1;
        }

        public void Initial()
        {
            //建立 離開按鈕
            //CreateExitBtn();
            //建立 模組 Button
            CreateBtnByModule(typeof(BaseModuleInterface), 4);
            ////建立 主控流程 按鈕
            //CreateMainFlowBtn();
        }

        private void CreateExitBtn()
        {
            //動態產生按鈕
            Button btn = new Button();
            Font font = new Font("微軟正黑體", 16, FontStyle.Bold);
            btn.Font = font;
            btn.Dock = DockStyle.Top;
            btn.ForeColor = Color.Black;
            btn.BackColor = Color.FromArgb(243, 243, 243);
            btn.Height = 80;
            btn.Text = "結 束";
            btn.UseVisualStyleBackColor = false;
            btn.ImageList = btnImgList;
            btn.ImageIndex = 0;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Name = "btnExit";
            ExitBtn = btn;

            //加入事件
            btn.Click += new System.EventHandler(btnClose_Click);

            //顯示在Panel上
            //splitContainer1.Panel1.Controls.Add(btn);
            panel1.Controls.Add(btn);
        }

        private void CreateMainFlowBtn()
        {
            //根據模組數量,動態產生按鈕
            Button btn = new Button();
            Font font = new Font("微軟正黑體", 16, FontStyle.Bold);
            btn.Font = font;
            btn.Dock = DockStyle.Top;
            btn.ForeColor = Color.Black;
            btn.BackColor = Color.FromArgb(243, 243, 243);
            btn.Height = 80;
            btn.Text = "主控流程";
            btn.UseVisualStyleBackColor = false;
            btn.ImageList = imgList;
            btn.ImageIndex = 0;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleCenter;

            //加入事件
            btn.Click += new System.EventHandler(btnMainFlow_Click);

            //顯示在Panel上
            splitContainer1.Panel1.Controls.Add(btn);
        }

        private void CreateBtnByModule(Type mType,int ImgIndex)
        {
            foreach (object obj in FormSet.ModuleList)
            {
                Type t = obj.GetType();
                if (t.IsSubclassOf(mType))
                {
                    //根據模組數量,動態產生按鈕
                    Button btn = new Button();
                    Font font = new Font("微軟正黑體", 16, FontStyle.Bold);
                    btn.Font = font;
                    btn.Dock = DockStyle.Top;
                    btn.ForeColor = Color.Black;
                    btn.BackColor = Color.FromArgb(243, 243, 243);
                    btn.Height = 80;
                    btn.Text = ((BaseModuleInterface)obj).Text;
                    btn.UseVisualStyleBackColor = false;
                    btn.Tag = obj;
                    btn.ImageList = imgList;
                    btn.ImageIndex = ImgIndex;
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.Name = string.Format("btnModule_{0}", ((BaseModuleInterface)obj).Name);
                    LastBtn = btn;

                    //加入事件
                    btn.Click += new System.EventHandler(btnChooseModlue_Click);

                    //顯示在Panel上
                    //splitContainer1.Panel1.Controls.Add(btn);
                    panel1.Controls.Add(btn);
                }
            }
        }

        //結束按鈕
        private void btnClose_Click(object sender, EventArgs e)
        {
            SYSPara.SysState = StateMode.SM_ABORT;
            this.Parent = null;
        }

        //主控流程按鈕
        private void btnMainFlow_Click(object sender, EventArgs e)
        {
            //解除所有嵌在Panel上的Form
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.Parent = null;

            //解除所有嵌在Panel上的Form
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.Parent = null;

            //將按鈕顏色恢復為Control顏色
            foreach (Control control in splitContainer1.Panel1.Controls)
                if (control is Button)
                {
                    ((Button)control).ForeColor = Color.Black;
                    ((Button)control).BackColor = Color.FromArgb(243, 243, 243);
                }

            //按下按鈕顯示Window顏色
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).BackColor = Color.FromArgb(92, 92, 92);

            //顯示Main Flow Form
            pnlContainer.Controls.Add(FormSet.mMainFlow);
            (FormSet.mMainFlow).WindowState = FormWindowState.Maximized;
            (FormSet.mMainFlow).Show();
        }

        private void btnChooseModlue_Click(object sender, EventArgs e)
        {
            //儲存操作按鈕資訊
            SYSPara.LogSay(EnLoggerType.EnLog_OP, "使用者按下 Module -『 " + ((Button)sender).Text + " 』鈕");

            //解除所有嵌在Panel上的Form
            foreach (BaseModuleInterface module in FormSet.ModuleList)
                module.Parent = null;

            //將按鈕顏色恢復為Control顏色
            foreach (Control control in panel1.Controls)
                if (control is Button)
                {
                    ((Button)control).ForeColor = Color.Black;
                    ((Button)control).BackColor = Color.FromArgb(243, 243, 243);
                }

            //按下按鈕顯示Window顏色
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).BackColor = Color.FromArgb(92, 92, 92);

            //顯示ModuleForm
            object obj = ((Button)sender).Tag;
            pnlContainer.Controls.Add((BaseModuleInterface)obj);
            ((BaseModuleInterface)obj).WindowState = FormWindowState.Maximized;
            //+ By Max 20210217, 更新模組UI上資料元件的資料
            string sFilePath = string.Format(@"{0}\Module\{1}\{2}.xml", SYSPara.SysDir, ((BaseModuleInterface)obj).Name, ((BaseModuleInterface)obj).Name);
            ((BaseModuleInterface)obj).SetDS.Initial(sFilePath, ((BaseModuleInterface)obj).Name);
            ((BaseModuleInterface)obj).ShowPage((ProVIFM.UserType)SYSPara.LoginUser);
            ((BaseModuleInterface)obj).Show();

            #region 使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)
            //當模組內有需要用到子表格時，才需要定義
            //((BaseModuleInterface)obj).InitialSubPackage(FormSet.mToolF);
            #endregion
        }

        public void PerformClick()
        {
            if (LastBtn != null)
            {
                ((Button)LastBtn).Focus();
                ((Button)LastBtn).PerformClick();
            }
        }

        public void ExitClick()
        {
            if (ExitBtn != null)
            {
                ((Button)ExitBtn).Focus();
                ((Button)ExitBtn).PerformClick();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SYSPara.SysState = StateMode.SM_CANCEL;
            this.Parent = null;
        }
    }
}
