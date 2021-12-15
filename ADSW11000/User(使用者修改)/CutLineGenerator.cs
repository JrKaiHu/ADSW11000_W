using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonObj;

using System.Globalization;
using System.Resources;

namespace ADSW11000
{
    public partial class CutLineGenerator : Form
    {
        private CutLineSetting m_CutLineSetting;
        public CutLineGenerator()
        {
            InitializeComponent();
        }

        public CutLineGenerator(ref CutLineSetting cutLineSetting)
        {
            InitializeComponent();
            m_CutLineSetting = cutLineSetting;

            if (cutLineSetting.iMode == 1)   //v0.5.7.26 By Sanxiu 即看即切修正
            {
                groupBox1.Enabled = false;
                groupBox4.Enabled = false;
                cboChannel.Enabled = false;
                rdoZ2.Checked = true;
                rdoDefault.Checked = true;
            }

            cboChannel.Text = "CH2";
            cboType.Text = "Type1";
        }

        private void CutLineGenerator_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                m_CutLineSetting.CutIdxLstForCh1 = Enumerable.Range(1, Convert.ToInt32(txtCH1Num.Text) + 1).ToList();
                m_CutLineSetting.CutIdxLstForCh2 = Enumerable.Range(1, Convert.ToInt32(txtCH2Num.Text) + 1).ToList();

                m_CutLineSetting.CutOffset  = Convert.ToInt32(txtCutOffset.Text);
                m_CutLineSetting.CutSpeed   = Convert.ToInt32(txtCutSpeed.Text);
                m_CutLineSetting.EnterSpeed = Convert.ToInt32(txtEnterSpeed.Text);
                m_CutLineSetting.chSelect   = (ChannelSelect)cboChannel.SelectedIndex;
                m_CutLineSetting.typeSelect = (TypeSelect)cboType.SelectedIndex;

                if (rdoZ1.Checked)
                    m_CutLineSetting.toolSelect = ToolSelect.Z1;
                else if(rdoZ2.Checked)
                    m_CutLineSetting.toolSelect = ToolSelect.Z2;
                else
                    m_CutLineSetting.toolSelect = ToolSelect.Z1Z2;

                if (rdoOneSideCut.Checked)
                    m_CutLineSetting.cutSeqSet = CutSeqSet.OneSideCut;
                else if (rdoTwoSideCut.Checked)
                    m_CutLineSetting.cutSeqSet = CutSeqSet.TwoSideCut;
                else
                    m_CutLineSetting.cutSeqSet = CutSeqSet.Default;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAutoGen_MouseMove(object sender, MouseEventArgs e)
        {
            string sShow = "";
            bool bIsChkOk = true;
            int iCalculation = -1;

            string strNowLang = SYSPara.Lang.GetNowLanguage();

            try
            {
                iCalculation = int.Parse(txtCutSpeed.Text);

                if (iCalculation < 10 || iCalculation > 170)
                {
                    if (strNowLang == "tw")
                    {
                        sShow += "切割速度輸入" + iCalculation.ToString() + "，已超出設定範圍10~170\n";
                    }
                    else
                    {
                        sShow += "Cut speed input" + iCalculation.ToString() + ", over the range of 10~170\n";
                    }
                    
                    bIsChkOk = false;
                }
            }
            catch (Exception ex)
            {
                sShow += strNowLang == "tw" ? "切割速度輸入錯誤\n" : "Cut speen input error\n";
                sShow += ex.Message;
                bIsChkOk = false;
            }

            try
            {
                iCalculation = int.Parse(txtCutOffset.Text);
                if (iCalculation <= 0)
                {
                    sShow += strNowLang == "tw" ? "提刀高度需大於0\n" : "Height of cut must be greater than 0\n";
                    bIsChkOk = false;
                }
            }
            catch (Exception ex)
            {
                sShow += strNowLang == "tw" ? "提刀高度輸入錯誤\n" : "Height of cut input error\n";
                sShow += ex.Message;
                bIsChkOk = false;
            }

            try
            {
                iCalculation = int.Parse(txtCH2Num.Text);
                if (iCalculation <= 0)
                {
                    sShow += strNowLang == "tw" ? "CH2個數需大於0\n" : "The number of CH2 must be greater than 0\n";
                    bIsChkOk = false;
                }
            }
            catch (Exception ex)
            {
                sShow += strNowLang == "tw" ? "CH2個數錯誤\n" : "The number of CH2 is wrong\n";
                sShow += ex.Message;
                bIsChkOk = false;
            }

            try
            {
                iCalculation = int.Parse(txtCH1Num.Text);
                if (iCalculation <= 0)
                {
                    sShow += strNowLang == "tw" ? "CH1個數需大於0\n" : "The number of CH1 must be greater than 0\n";
                    bIsChkOk = false;
                }
            }
            catch (Exception ex)
            {
                sShow += strNowLang == "tw" ? "CH1個數錯誤\n" : "The number of CH1 is wrong\n";
                sShow += ex.Message;
                bIsChkOk = false;
            }

            if (bIsChkOk == false)
            {
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "", sShow);
                //MessageBox.Show(sShow);
            }
        }

        private string GetDialogMsgText(string strKey)
        {
            ResourceManager resourceManager = new ResourceManager(typeof(Properties.Resources));
            if (SYSPara.Lang.GetNowLanguage() == "tw")
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("zh-TW"));
            }
            else
            {
                return resourceManager.GetString(strKey, CultureInfo.CreateSpecificCulture("en-US"));
            }
        }

        private void CutLineGenerator_Load(object sender, EventArgs e)
        {
            int iCH2Num = Convert.ToInt32(FormSet.mPackage.dFieldEdit17.FieldValue);
            if (iCH2Num == 0)
            {
                //MessageBox.Show("無法帶入CH2面的料件個數，請先至料片規格進行設定!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", GetDialogMsgText("Text_CutLineGenerator_CH2_Number_Alarm"));
                Close();
            }
            else
            {
                txtCH2Num.Text = iCH2Num.ToString();
            }

            int iCH1Num = Convert.ToInt32(FormSet.mPackage.dFieldEdit16.FieldValue);
            if (iCH1Num == 0)
            {
                //MessageBox.Show("無法帶入CH1面的料件個數，請先至料片規格進行設定!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dia_Message.Instance.ShowDialog(enMsgDialogType.ERROR, "Error", GetDialogMsgText("Text_CutLineGenerator_CH1_Number_Alarm"));
                Close();
            }
            else
            {
                txtCH1Num.Text = iCH1Num.ToString();
            }

            grpEnterSpeed.Visible = SYSPara.LoginUser == UserType.utProV;
        }
    }

    public enum ToolSelect{Z1, Z2, Z1Z2};
    public enum ChannelSelect{CH1, CH2, CH3, CH4};
    public enum TypeSelect{Type1, Type2};
    public enum CutSeqSet{Default, OneSideCut, TwoSideCut};
    
    public class CutLineSetting
    {
        public int CutOffset;
        public int CutSpeed;
        public int EnterSpeed;
        public ToolSelect toolSelect;
        public ChannelSelect chSelect;
        public TypeSelect typeSelect;
        public CutSeqSet cutSeqSet;
        public int iMode;   //v0.5.7.26 By Sanxiu 即看即切修正

        public List<int> CutIdxLstForCh1;
        public List<int> CutIdxLstForCh2;
    }
}
