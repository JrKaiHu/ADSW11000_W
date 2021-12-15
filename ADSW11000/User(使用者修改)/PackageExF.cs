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
using CommonObj;


#region copy用
#endregion copy用
namespace ADSW11000
{   //使用者修改 (產品設定參數放置處)
    public partial class PackageExF : BasePackageExF
    {
        #region 基本

        public PackageExF()
        {
            InitializeComponent();
            //定義表格名稱，會影響Log記錄內的DataSource名稱
            ModuleName = "Package";

            FolderPath = SYSPara.SysDir + @"\LocalData\Package\";
        }

        #endregion 基本

        #region 公用函數

        public override void InitialSubPackage()
        {
            #region 公用函數_使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)

            //Woody 2020/9/4
            BladeZ1Data.EditForm = FormSet.mBladeZ1Data;
            FormSet.mBladeZ1Data.Tag = this;
            if (SYSPara.IsAutoMode)
                FormSet.mBladeZ1Data.SetFileName(SYSPara.PReadValue("BladeZ1Data").ToString(), "", true);

            BladeZ2Data.EditForm = FormSet.mBladeZ2Data;
            FormSet.mBladeZ2Data.Tag = this;
            if (SYSPara.IsAutoMode)
                FormSet.mBladeZ2Data.SetFileName(SYSPara.PReadValue("BladeZ2Data").ToString(), "", true);

            //tFieldCB1.EditForm = FormSet.mToolF;
            //FormSet.mToolF.Tag = this;
            //if (SYSPara.IsAutoMode)
            //    FormSet.mToolF.SetFileName(SYSPara.PReadValue("Tool").ToString(), "", true);   

            //+By Max 20210317
            KCSDK.DataSnippet.DataGetAll(FormSet.mBladeZ1Data.pnlControl);
            KCSDK.DataSnippet.DataGetAll(FormSet.mBladeZ2Data.pnlControl);
            #endregion 公用函數_使用者修改 (關聯式資料庫，將相關表格加入這個主表格內)
        }

        //+ By Max 20210312, v4.0.1.53 資料重讀回UI
        public override void AfterSave()
        {
            base.AfterSave();
            DataSnippet.DataGetAll(pnlControl);
        }

        #endregion 公用函數

        #region 事件

        #region 切割設定分頁
        private void OnButtonClicked2(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == btn_AutoCutLine)
            {
                #region Open cut line generator dialog
                CutLineSetting cutLineSetting = new CutLineSetting();
                cutLineSetting.iMode = 0;

                if (new CutLineGenerator(ref cutLineSetting).ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    AutoCutLineXML(cutLineSetting);
                }
                #endregion
            }
            else if (btn == btn_SimulateCutLine)
            {
                #region Open cut simulator dialog
                if (dCutProg.RowCount != 0)
                {
                    CutLineSimulate frmCutLineSimulate = new CutLineSimulate(dCutProg);
                    frmCutLineSimulate.Show();
                }
                #endregion
            }
            else if (btn == btnAddOneCut)
            {
                #region Add
                AddRow(dCutProg);
                //+ By Max 20210216
                for (int i = 1; i < dCutProg.Rows[dCutProg.Rows.Count - 1].Cells.Count; ++i)
                {
                    DataGridViewCell cell = dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[i];
                    cell.Value = dCutProg.Columns[i].Name == "Cut_高度偏移" ? 3000 : dCutProg.Rows[0].Cells[i].Value;
                }

                if (dCutProg.Rows.Count == 1)
                {
                    dCutProg.Rows[0].Cells[1].Value = false;
                    dCutProg.Rows[0].Cells[2].Value = 1;
                    dCutProg.Rows[0].Cells[3].Value = false;
                    //dCutProg.Rows[0].Cells[4].Value = i - 1; //不設定，新增完，將焦點設定在此Cell並進入編輯
                    dCutProg.Rows[0].Cells[5].Value = 3000;
                    dCutProg.Rows[0].Cells[6].Value = "Type1";
                    dCutProg.Rows[0].Cells[7].Value = "CH2";
                    dCutProg.Rows[0].Cells[8].Value = 170;
                    dCutProg.Rows[0].Cells[9].Value = 85;
                }

                dCutProg.Focus();
                dCutProg.CurrentCell = dCutProg[4, dCutProg.Rows.Count - 1];
                dCutProg.BeginEdit(true);
                #endregion
            }
            else if (btn == btnInsertOneCut || btn == btnDelSelCut || 
                     btn == btnMoveUpCut || btn == btnMoveDownCut)
            {
                if (btn == btnInsertOneCut)
                {
                    #region Insert
                    InsertRow(dCutProg);

                    if (dCutProg.Rows.Count == 0)
                    {
                        if (SYSPara.Lang.GetNowLanguage() == "tw")
                        {
                            dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Message", "請使用新增或自動刀序產生設定功能");
                        }
                        else
                        {
                            dia_Message.Instance.ShowDialog(enMsgDialogType.NOTIFY, "Message", "Please use new or auto creation for cut");
                        }
                        //MessageBox.Show("請使用新增或自動刀序產生設定功能", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //+ By Max 20210216
                    if (dCutProg.Rows.Count > 1)
                    {
                        for (int i = 1; i < dCutProg.Rows[dCutProg.SelectedCells[0].RowIndex - 1].Cells.Count; ++i)
                        {
                            DataGridViewCell cell = dCutProg.Rows[dCutProg.SelectedCells[0].RowIndex - 1].Cells[i];
                            cell.Value = dCutProg.Columns[i].Name == "Cut_高度偏移" ? 3000 : dCutProg.Rows[0].Cells[i].Value;
                        }

                        dCutProg.Focus();
                        dCutProg.CurrentCell = dCutProg[4, dCutProg.SelectedCells[0].RowIndex - 1];
                        dCutProg.BeginEdit(true);
                    }

                    if (dCutProg.Rows.Count == 1)
                    {
                        dCutProg.Rows[0].Cells[1].Value = false;
                        dCutProg.Rows[0].Cells[2].Value = 1;
                        dCutProg.Rows[0].Cells[3].Value = false;
                        //dCutProg.Rows[0].Cells[4].Value = i - 1; //不設定，新增完，將焦點設定在此Cell並進入編輯
                        dCutProg.Rows[0].Cells[5].Value = 3000;
                        dCutProg.Rows[0].Cells[6].Value = "Type1";
                        dCutProg.Rows[0].Cells[7].Value = "CH2";
                        dCutProg.Rows[0].Cells[8].Value = 170;
                        dCutProg.Rows[0].Cells[9].Value = 85;

                        dCutProg.Focus();
                        dCutProg.CurrentCell = dCutProg[4, 0];
                        dCutProg.BeginEdit(true);
                    }
                    #endregion
                }
                else if (btn == btnDelSelCut)
                {
                    #region Delete
                    DelRow(dCutProg);
                    #endregion
                }
                else if (btn == btnMoveUpCut)
                {
                    #region Move up
                    UpMove(dCutProg);
                    #endregion
                }
                else if (btn == btnMoveDownCut)
                {
                    #region Move down
                    DownMove(dCutProg);
                    #endregion
                }

                #region Reset all index
                for (int i = 0; i < dCutProg.Rows.Count; i++)
                {
                    dCutProg.Rows[i].Cells[0].Value = (i + 1);
                }
                #endregion
            }
            else if (btn == btnClearAllCut)
            {
                #region  Clear all
                ClearAll(dCutProg);
                #endregion
            }
        }

        private void AutoCutLineXML(CutLineSetting cutLineSetting)
        {
            dCutProg.Rows.Clear();

            if (cutLineSetting.cutSeqSet == CutSeqSet.Default) //預設（如Z2刀則由左切到右，Z1刀則由右切到左）
            {
                if (cutLineSetting.toolSelect != ToolSelect.Z1Z2)
                {
                    #region 只用單刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {
                        foreach (int nCutIdx in cutLineSetting.CutIdxLstForCh1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        foreach (int nCutIdx in cutLineSetting.CutIdxLstForCh2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    else //CH2 先切
                    {
                        foreach (int nCutIdx in cutLineSetting.CutIdxLstForCh2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        foreach (int nCutIdx in cutLineSetting.CutIdxLstForCh1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    #endregion
                }
                else //Both Z1 and Z2
                {
                    #region 雙刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {

                        AppendOneChannel(cutLineSetting);
                        cutLineSetting.chSelect = ChannelSelect.CH2;
                        AppendOneChannel(cutLineSetting);
                    }
                    else //CH2 先切
                    {
                        AppendOneChannel(cutLineSetting);
                        cutLineSetting.chSelect = ChannelSelect.CH1;
                        AppendOneChannel(cutLineSetting);
                    }
                    #endregion
                }

            }
            else if (cutLineSetting.cutSeqSet == CutSeqSet.OneSideCut) //單方向邊料先切除
            {
                if (cutLineSetting.toolSelect == ToolSelect.Z1Z2)
                {
                    #region 只用單刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH1 最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);

                        foreach (int nCutIdx in lst1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        //CH2 最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);

                        foreach (int nCutIdx in lst2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    else //CH2 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH2 最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);

                        foreach (int nCutIdx in lst2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        //CH1 最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);

                        foreach (int nCutIdx in lst1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    #endregion
                }
                else //Both Z1 and Z2
                {
                    #region 雙刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH1 第一道與最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);
                        lst1.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        //CH2 第一道與最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);
                        lst2.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);
                    }
                    else //CH2 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH2 第一道與最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);
                        lst2.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        //CH1 第一道與最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);
                        lst1.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);
                    }
                    #endregion
                }
            }
            else //雙方向邊料先切除 TwoSideCut
            {
                if (cutLineSetting.toolSelect != ToolSelect.Z1Z2)
                {
                    #region 只用單刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH1 最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);

                        //CH1 第一道邊料
                        AppendOneRow(true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(0);

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        //CH2 最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);

                        //CH2 第一道邊料
                        AppendOneRow(true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(0);

                        foreach (int nCutIdx in lst2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        foreach (int nCutIdx in lst1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    else //CH2 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH2 最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);

                        //CH2 第一道邊料
                        AppendOneRow(true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(0);

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        //CH1 最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);

                        //CH1 第一道邊料
                        AppendOneRow(true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(0);

                        foreach (int nCutIdx in lst1)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        foreach (int nCutIdx in lst2)
                        {
                            AppendOneRow(true, nCutIdx, cutLineSetting);
                        }
                    }
                    #endregion
                }
                else //Both Z1 and Z2
                {
                    #region 雙刀
                    if (cutLineSetting.chSelect == ChannelSelect.CH1) //CH1 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH1 第一道與最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);
                        lst1.RemoveAt(0);

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        //CH2 第一道與最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);
                        lst2.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        AppendOneChannel(cutLineSetting);
                    }
                    else //CH2 先切
                    {
                        List<int> lst1 = cutLineSetting.CutIdxLstForCh1;
                        List<int> lst2 = cutLineSetting.CutIdxLstForCh2;

                        //CH2 第一道與最後一道邊料
                        AppendOneRow(true, lst2[lst2.Count - 1], true, lst2[0], cutLineSetting);
                        lst2.RemoveAt(lst2.Count - 1);
                        lst2.RemoveAt(0);

                        cutLineSetting.chSelect = ChannelSelect.CH1;

                        //CH1 第一道與最後一道邊料
                        AppendOneRow(true, lst1[lst1.Count - 1], true, lst1[0], cutLineSetting);
                        lst1.RemoveAt(lst1.Count - 1);
                        lst1.RemoveAt(0);

                        AppendOneChannel(cutLineSetting);

                        cutLineSetting.chSelect = ChannelSelect.CH2;

                        AppendOneChannel(cutLineSetting);
                    }
                    #endregion
                }
            }
            
        }

        private void AppendOneChannel(CutLineSetting setting)
        {
            List<int> CutIdxLst = setting.chSelect == ChannelSelect.CH1 ? setting.CutIdxLstForCh1 : setting.CutIdxLstForCh2;

            List<int> Z2CutIdxLst = CutIdxLst.Take(
                            CutIdxLst.Count() / 2 + CutIdxLst.Count() % 2).ToList();
            List<int> Z1CutIdxLst = CutIdxLst.Skip(
                CutIdxLst.Count() / 2 + CutIdxLst.Count() % 2).ToList();

            while (Z1CutIdxLst.Count > 0)
            {
                AppendOneRow(true, Z1CutIdxLst[0], true, Z2CutIdxLst[0], setting);
                Z1CutIdxLst.RemoveAt(0);
                Z2CutIdxLst.RemoveAt(0);
            }

            if (Z2CutIdxLst.Count > 0)
            {
                AppendOneRow(false, 0, true, Z2CutIdxLst[0], setting);
                Z2CutIdxLst.RemoveAt(0);
            }
        }

        private void AppendOneRow(
            bool bIsZ1Checked, int nCutIdxZ1, 
            bool bIsZ2Checked, int nCutIdxZ2, 
            CutLineSetting cls)
        {
            dCutProg.Rows.Add();
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[0].Value = dCutProg.Rows.Count;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[1].Value = bIsZ1Checked;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[2].Value = nCutIdxZ1;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[3].Value = bIsZ2Checked;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[4].Value = nCutIdxZ2;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[5].Value = cls.CutOffset;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[6].Value = cls.typeSelect == TypeSelect.Type1 ? "Type1" : "Type2";
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[7].Value = cls.chSelect == ChannelSelect.CH1 ? "CH1" : "CH2";
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[8].Value = cls.CutSpeed;

            if (cls.EnterSpeed > cls.CutSpeed) cls.EnterSpeed = cls.CutSpeed;
            if (cls.EnterSpeed < cls.CutSpeed / 2) cls.EnterSpeed = cls.CutSpeed / 2;

            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[9].Value = cls.EnterSpeed;
        }

        private void AppendOneRow(
            bool bIsChecked, int nCutIdx,
            CutLineSetting cls)
        {
            dCutProg.Rows.Add();
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[0].Value = dCutProg.Rows.Count;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[1].Value = cls.toolSelect == ToolSelect.Z1 ? bIsChecked : false;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[2].Value = cls.toolSelect == ToolSelect.Z1 ? nCutIdx : 0;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[3].Value = cls.toolSelect == ToolSelect.Z2 ? bIsChecked : false;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[4].Value = cls.toolSelect == ToolSelect.Z2 ? nCutIdx : 0;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[5].Value = cls.CutOffset;
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[6].Value = cls.typeSelect == TypeSelect.Type1 ? "Type1" : "Type2";
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[7].Value = cls.chSelect == ChannelSelect.CH1 ? "CH1" : "CH2";
            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[8].Value = cls.CutSpeed;

            if (cls.EnterSpeed > cls.CutSpeed) cls.EnterSpeed = cls.CutSpeed;
            if (cls.EnterSpeed < cls.CutSpeed / 2) cls.EnterSpeed = cls.CutSpeed / 2;

            dCutProg.Rows[dCutProg.Rows.Count - 1].Cells[9].Value = cls.EnterSpeed;
        }
        #endregion

        #region 入料前與切割後清洗分頁
        private void OnButtonClicked1(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Control ctrl = btn.Parent;
            DDataGridView dgvTemp = dCleanBeforeLoad;

            if (ctrl.Name != "tabPage_Package_入料前清洗")
            {
                dgvTemp = dCleanAfterCut;
            }

            if (btn == btnAddOfBefore || btn == btnAddOfAfter)
            {
                AddRow(dgvTemp);

                //+ By Max 20210216
                for (int i = 1; i < dgvTemp.Rows[dgvTemp.Rows.Count - 1].Cells.Count; ++i)
                {
                    dgvTemp.Rows[dgvTemp.Rows.Count - 1].Cells[i].Value = dgvTemp.Rows[0].Cells[i].Value;
                }
            }
            else if (btn == btnDeleteOfBefore || btn == btnDelOfAfter)
            {
                DelRow(dgvTemp);
            }
            else if (btn == btnMoveUpOfBefore || btn ==btnMoveUpOfAfter)
            {
                UpMove(dgvTemp);
            }
            else if (btn == btnMoveDownOfBefore || btn == btnMoveDownOfAfter)
            {
                DownMove(dgvTemp);
            }
            else if (btn == btnInsertOfBefore || btn == btnInsertOfAfter)
            {
                InsertRow(dgvTemp);
            }
            else if (btn == btnClearAllOfBefore || btn == btnClearAllOfAfter)
            {
                ClearAll(dgvTemp);
            }
        }
        #endregion

        #region 預切割
        private void button_新增_Click(object sender, EventArgs e)
        {
            AddRow(dPreCutData);
        }

        private void button_刪除_Click(object sender, EventArgs e)
        {
            DelRow(dPreCutData);
        }

        private void button_上移_Click(object sender, EventArgs e)
        {
            UpMove(dPreCutData);
        }

        private void button_下移_Click(object sender, EventArgs e)
        {
            DownMove(dPreCutData);
        }

        private void button_插入_Click(object sender, EventArgs e)
        {
            InsertRow(dPreCutData);
        }

        private void button_清除_Click(object sender, EventArgs e)
        {
            ClearAll(dPreCutData);
        }
        #endregion

        #endregion 事件

        private void RefreshUI_timer_Tick(object sender, EventArgs e)
        {
            //是否開啟預切割功能
            if (FormSet.mMainFlow.mSaw.SReadValue("bEnableSharpen").ToBoolean())
            {
                tabPage_Package_預切割.Parent = tabControl1;
            }
            else
            {
                tabPage_Package_預切割.Parent = null;
            }
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == btnEdit)
            {
                if (SYSPara.RunMode == RunModeDT.AUTO)
                {
                    SYSPara.PackageStop = false;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    SYSPara.SysRun = false;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                    FormSet.mMSS.M_Stop();   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                }

                //+ By Max 20210205
                SetCutSeqEditability(true);

                #region By Wolf 20211103 v4.0.97.0 非admin權限登入無法修改是否掃靶的設定
                chkScanTarget.Enabled = SYSPara.LoginUser == UserType.utProV || SYSPara.LoginUser == UserType.utAdministrator;
                #endregion
            }
            else if (btn == btnSave || btn == btnCancel || btn == btnClose)
            {
                if (btn == btnSave)
                {
                    //+ By Max 20210308, 切割刀序合理性檢查
                    CutSequenceRationalityCheck();
                }

                SYSPara.PackageStop = true;   //v0.0.7.19 By Sanxiu 通用設定，產品設定進入時不暫停，編輯時才暫停
                //+ By Max 20210205
                SetCutSeqEditability(false);
            }
        }

        //+ By Max 20210308, 切割刀序合理性檢查
        private bool CutSequenceRationalityCheck()
        {
            HashSet<int> CH1Set = new HashSet<int>();
            HashSet<int> CH2Set = new HashSet<int>();
            HashSet<int> CH1OutOfRangeSet = new HashSet<int>();
            HashSet<int> CH2OutOfRangeSet = new HashSet<int>();
            string sCH = "CH1";
            int Z1Line = 1;
            int Z2Line = 1;
            int iCH1LineNum = Convert.ToInt32(FormSet.mPackage.dFieldEdit16.FieldValue) + 1; //CH1 切割道數量
            int iCH2LineNum = Convert.ToInt32(FormSet.mPackage.dFieldEdit17.FieldValue) + 1; //CH2 切割道數量

            int nCheckTotalNum = 0;

            foreach (DataGridViewRow r in dCutProg.Rows)
            {
                try
                {
                    //+ By Max 20210316, v4.0.1.55, Bugs修正
                    sCH = r.Cells[7].Value.ToString();
                    if (sCH == "CH1")
                    {
                        Z1Line = Convert.ToInt32(r.Cells[2].Value);
                        if (Z1Line > iCH1LineNum)
                            CH1OutOfRangeSet.Add(Z1Line);
                        if(Z1Line != 0)
                            CH1Set.Add(Z1Line);
                        Z2Line = Convert.ToInt32(r.Cells[4].Value);
                        if (Z2Line > iCH1LineNum)
                            CH1OutOfRangeSet.Add(Z2Line);
                        if (Z2Line != 0)
                            CH1Set.Add(Z2Line);
                    }
                    else
                    {
                        Z1Line = Convert.ToInt32(r.Cells[2].Value);
                        if (Z1Line > iCH2LineNum)
                            CH2OutOfRangeSet.Add(Z1Line);
                        if (Z1Line != 0)
                            CH2Set.Add(Z1Line);
                        Z2Line = Convert.ToInt32(r.Cells[4].Value);
                        if (Z2Line > iCH2LineNum)
                            CH2OutOfRangeSet.Add(Z2Line);
                        if (Z2Line != 0)
                            CH2Set.Add(Z2Line);
                    }

                    // 檢查是否有刀續為0卻被打勾
                    DataGridViewCheckBoxCell chkZ1 = r.Cells[1] as DataGridViewCheckBoxCell;
                    DataGridViewCheckBoxCell chkZ2 = r.Cells[3] as DataGridViewCheckBoxCell;

                    if (Convert.ToBoolean(chkZ1.Value))
                    {
                        if (r.Cells[2].Value.ToString() == "0") nCheckTotalNum--;
                        else nCheckTotalNum++;
                    }
                    else
                    {
                        int nChannel = r.Cells[7].Value.ToString() == "CH1" ? 1 : 2;

                        if (r.Cells[2].Value.ToString() != "0")
                        {
                            SYSPara.Alarm.Say("w0505", nChannel, r.Cells[2].Value.ToString());
                        }
                    }

                    if (Convert.ToBoolean(chkZ2.Value))
                    {
                        if (r.Cells[4].Value.ToString() == "0") nCheckTotalNum--;
                        else nCheckTotalNum++;
                    }
                    else
                    {
                        int nChannel = r.Cells[7].Value.ToString() == "CH1" ? 1 : 2;

                        if (r.Cells[4].Value.ToString() != "0")
                        {
                            SYSPara.Alarm.Say("w0505", nChannel, r.Cells[4].Value.ToString());
                        }
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            //Console.WriteLine(nCheckTotalNum);

            if (nCheckTotalNum < (iCH1LineNum + iCH2LineNum))
            {
                return false;
            }

            if (CH1OutOfRangeSet.Count > 0 || CH2OutOfRangeSet.Count > 0)
            {
                for (int i = 0; i < CH1OutOfRangeSet.Count; ++i)
                {
                    SYSPara.Alarm.Say("w0504", 1, CH1OutOfRangeSet.Skip(i).First(), iCH1LineNum);
                }
                for (int i = 0; i < CH2OutOfRangeSet.Count; ++i)
                {
                    SYSPara.Alarm.Say("w0504", 2, CH2OutOfRangeSet.Skip(i).First(), iCH2LineNum);
                }
                return false;
            }

            if (CH1Set.Count != iCH1LineNum || CH2Set.Count != iCH2LineNum)
            {
                for (int i = 1; i <= iCH1LineNum; ++i)
                {
                    if (!CH1Set.Contains(i))
                    {
                        SYSPara.Alarm.Say("w0503", 1, i);
                    }
                }
                for (int i = 1; i <= iCH2LineNum; ++i)
                {
                    if (!CH2Set.Contains(i))
                    {
                        SYSPara.Alarm.Say("w0503", 2, i);
                    }
                }
                return false;
            }
            else
                return true;
        }

        //+ By Max 20210205
        private void SetCutSeqEditability(bool bEdit = true)
        {
            //Cut Sequence
            btn_AutoCutLine.Enabled = bEdit;
            btn_SimulateCutLine.Enabled = bEdit;
            btnMoveDownCut.Enabled = bEdit;
            btnClearAllCut.Enabled = bEdit;
            btnMoveUpCut.Enabled = bEdit;
            btnDelSelCut.Enabled = bEdit;
            btnAddOneCut.Enabled = bEdit;
            btnInsertOneCut.Enabled = bEdit;

            //PreClean
            btnMoveDownOfBefore.Enabled = bEdit;
            btnClearAllOfBefore.Enabled = bEdit;
            btnMoveUpOfBefore.Enabled = bEdit;
            btnDeleteOfBefore.Enabled = bEdit;
            btnAddOfBefore.Enabled = bEdit;
            btnInsertOfBefore.Enabled = bEdit;

            //PostClean
            btnMoveDownOfAfter.Enabled = bEdit;
            btnClearAllOfAfter.Enabled = bEdit;
            btnMoveUpOfAfter.Enabled = bEdit;
            btnDelOfAfter.Enabled = bEdit;
            btnAddOfAfter.Enabled = bEdit;
            btnInsertOfAfter.Enabled = bEdit;
        }

        //+ By Max 20210216
        private void dCutProg_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            dCutProg.Rows[e.RowIndex].ErrorText = String.Empty;

            if (e.ColumnIndex == 9)
            {
                int nEnterSpd = Convert.ToInt32(dCutProg.Rows[e.RowIndex].Cells["Cut_入板速度"].Value.ToString());
                int nCutSpd = Convert.ToInt32(dCutProg.Rows[e.RowIndex].Cells["Cut_速度"].Value.ToString());
                if (nEnterSpd > nCutSpd)
                {
                    dCutProg.Rows[e.RowIndex].Cells["Cut_入板速度"].Value = nCutSpd.ToString();
                }
                if (nEnterSpd < nCutSpd / 2)
                {
                    dCutProg.Rows[e.RowIndex].Cells["Cut_入板速度"].Value = (nCutSpd / 2).ToString();
                }
            }
        }

        //+ By Max 20210308, Cell合理性檢查
        private void dCutProg_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell cell = dCutProg.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!cell.IsInEditMode) return;

            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                dCutProg.Rows[e.RowIndex].ErrorText = "Cell must not be empty";
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", "Cell must not be empty!");
                //MessageBox.Show("Cell must not be empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (e.ColumnIndex == 0 || e.ColumnIndex == 2 || 
                e.ColumnIndex == 4 || e.ColumnIndex == 5 || 
                e.ColumnIndex == 8 || e.ColumnIndex == 9)
            {
                int nVal;
                if (!Int32.TryParse(e.FormattedValue.ToString(), out nVal))
                {
                    dCutProg.Rows[e.RowIndex].ErrorText = "Cell must be digital";
                    dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", "Cell must be digital!");
                    //MessageBox.Show("Cell must be digital!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void dCleanBeforeLoad_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            dCleanBeforeLoad.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void dCleanBeforeLoad_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell cell = dCleanBeforeLoad.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!cell.IsInEditMode) return;

            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                dCleanBeforeLoad.Rows[e.RowIndex].ErrorText = "Cell must not be empty";
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", "Cell must not be empty!");
                //MessageBox.Show("Cell must not be empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void dCleanAfterCut_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            dCleanAfterCut.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        private void dCleanAfterCut_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridViewCell cell = dCleanAfterCut.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (!cell.IsInEditMode) return;

            // Confirm that the cell is not empty.
            if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
            {
                dCleanAfterCut.Rows[e.RowIndex].ErrorText = "Cell must not be empty";
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", "Cell must not be empty!");
                //MessageBox.Show("Cell must not be empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void PackageExF_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                dCutProg.Columns["Cut_入板速度"].Visible = SYSPara.LoginUser == UserType.utProV;
            }
        }

        private void dCutProg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Avoid checking the checkbox with zero cut line

            dCutProg.EndEdit();

            if (e.ColumnIndex == 1 || e.ColumnIndex == 3)
            {
                DataGridViewCheckBoxCell chk = dCutProg.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
                if (chk.Value == chk.TrueValue)
                {
                    if (dCutProg.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString() == "0")
                    {
                        chk.Value = chk.FalseValue;
                        dCutProg.CommitEdit(DataGridViewDataErrorContexts.Commit);
                    }
                }
            }
        }

        private void BladeZ1Data_OnBeforeShow()
        {
            FormSet.mBladeZ1Data.FileName = BladeZ1Data.FieldValue;
        }

        private void BladeZ2Data_OnBeforeShow()
        {
            FormSet.mBladeZ2Data.FileName = BladeZ2Data.FieldValue;
        }

        private void label9_MouseUp(object sender, MouseEventArgs e)
        {
            //+ By Max, 20210804
            if (Control.ModifierKeys == Keys.Control && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                dCheckBox2.Visible = !dCheckBox2.Visible;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            chkScanTarget.CheckState = CheckState.Checked;
            btnSave.PerformClick();
        }
    }
}
