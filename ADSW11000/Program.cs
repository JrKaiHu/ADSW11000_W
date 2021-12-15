using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProVIFM;
using ProVLib;
using KCSDK;
using System.Diagnostics;
using System.Text;
using CommonObj;

namespace ADSW11000
{
    static class Program
    {
        // 記錄未處理之例外次數 ..
        static int UnHandledCount = 0;

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            #region 檢查SQL Local DB是否有安裝
            //By Noke 20200925 For PostgreSQL
            if (!SYSPara.logDB.IsInstalled())
            {
                MessageBox.Show("尚未安裝PostgreSQL12或無法判定，\n請下載並執行 \\dataserver\\EGROUP_SDK\\ProvAutomationSDK\\ProVSampleProject_C# Version\\3. 第三方元件_工具\\PostgreSQL資料\\postgresql-12.4-1-windows-x64.exe 進行安裝！\n 如確認已安裝，可將此檢查機制移除。", "資料庫安裝", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion

            #region 例外處理委派
            //// 定義系統離開前的 Handler 
            Application.ApplicationExit += OnProcessExit;

            ////將 例外狀況 交由 ThreadExcpetion處理 ,忽略程式組態檔 
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            ////處理 UI Thread 線程異常   
            Application.ThreadException += Application_ThreadException;
            ////其它非UI THREAD 的線程異常   
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            #endregion
        
            #region 讀取 setup.ini 資料

            SYSPara.SysDir = System.IO.Directory.GetCurrentDirectory();
            string sFileDir = SYSPara.SysDir + @"\Localdata";
            if (!System.IO.Directory.Exists(sFileDir))
                System.IO.Directory.CreateDirectory(sFileDir);
            TINIFile iFile = new TINIFile(sFileDir + @"\Setup.ini");
            SYSPara.Simulation = iFile.ReadInteger("System", "Simulation", 0); //是否為模擬
            SYSPara.OPPW = iFile.ReadString("System", "OPPW", ""); //作業員密碼
            SYSPara.ENGPW = iFile.ReadString("System", "ENGPW", ""); //工程師密碼
            SYSPara.Designer = "w";  //v4.0.1.40 新增設計師密碼
            SYSPara.EQID = iFile.ReadString("System", "EQID", "EQ"); //設備代號
            SYSPara.LightMode = iFile.ReadInteger("System", "LightMode", 0); //燈號顯示模式
            SYSPara.ScreenMode = iFile.ReadInteger("System", "ScreenMode", 1); //螢幕顯示模式
            SYSPara.LoginMode = iFile.ReadInteger("System", "LoginMode", 0); //使用者登入模式
            SYSPara.DebugMode = iFile.ReadBoolen("System", "DebugMode", false); //debug模式開關
            SYSPara.LogMaxLine = iFile.ReadInteger("System", "LogMaxLine", 2000); //歷史記錄最大行數           
            SYSPara.RunStartUpTM = DateTime.Now.ToString("yyyyMMddHHmmssFFF");
            SYSPara.LogPath = iFile.ReadString("System", "LogPath", @"D:\Log");
            SYSPara.UseDoubleScreen = iFile.ReadBoolen("System", "UseDoubleScreen", false); //使用雙螢幕模式 kent
            SYSPara.MachineID = iFile.ReadString("System", "MachineID", "ADSWS");  //v0.0.7.17 By Sanxiu 增加的機台編號
            SYSPara.XMotorLinearMode = iFile.ReadInteger("System", "XMotorLinearMode", 0);   //v0.5.7.28 By Sanxiu 馬達X軸加入絕對型光學尺功能

            iFile.Dispose();

            #endregion

            #region 避免程式重覆執行
            Boolean bCreatedNew;
            //Create a new mutex using specific mutex name
            Mutex m = new Mutex(false, "myUniqueName", out bCreatedNew);
            if (!bCreatedNew)
            {
                if (SYSPara.Simulation == 0)
                {
                    MessageBox.Show("Program has been run", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            #endregion

            #region 建立 Splash Form
            FormSet.mBanner = new BannerForm();
            FormSet.mBanner.Show();
            FormSet.mBanner.mCaption = "System Start";
            FormSet.mBanner.Start();
            #endregion

            //+ By Max 20200204 For LogAnalyzer
            FormSet.mBanner.mCaption = "資料庫初始化...";
            CreateLogDB();

            //創建 History Logger Form
            FormSet.mLogF = new HistoryLogger();
            FormSet.mLogF.Show();

            //創建 Option Form
            FormSet.m內參設定 = new 內參設定();

            //創建 Option Form
            FormSet.mOption = new OptionF();

            MMTimerAPI.TimeBeginPeriod(1);



            #region 讀取員工資料
            if (SYSPara.LoginMode == 1)
            {
                string fname = SYSPara.SysDir + @"\LocalData\User";
                SYSPara.UserList.Clear();
                UserDT data = new UserDT();
                data.UserID = "ProV";
                data.UserPW = "89543914";
                data.UserRight =  UserType.utProV;
                SYSPara.UserList.Add(data);
                if (File.Exists(fname))
                {
                    StreamReader sr = new StreamReader(fname, Encoding.GetEncoding(65001));
                    String line;
                    string[] strlist = new string[3];
                    while ((line = sr.ReadLine()) != null)
                    {
                        strlist = line.Split(',');

                        data = new UserDT();
                        data.UserID = strlist[0];
                        data.UserPW = strlist[1];
                        data.UserRight = (UserType)Convert.ToInt32(strlist[2]);
                        SYSPara.UserList.Add(data);
                    }
                    //關閉流
                    sr.Close();
                }
            }
            #endregion

            #region 創建 MiddleStage Server
            FormSet.mBanner.mCaption = "載入 MSS 模組";
            FormSet.mMSS = new MSS();
            FormSet.mMSS.Show();
            #endregion

            #region 載入 Alarm Table
            FormSet.mBanner.mCaption = "載入系統 Alarm Table";
            if (!SYSPara.Alarm.LoadFromFile(SYSPara.SysDir + @"\Localdata\alarm.xls"))
            {
                MessageBox.Show("Alarm Table Load Fail!");
                FormSet.mBanner.DisposTH();
                FormSet.mMSS.DisposeTH();
                return;
            }
            #endregion

            List<string> Errlist = new List<string>();
            Errlist.Clear();

            #region 載入 IO Table
            IOMapping io = new IOMapping();
            FormSet.mBanner.mCaption = "載入 IO Table";
            string iofile = SYSPara.SetupReadValue("IO描述檔").ToString();
            if (string.IsNullOrEmpty(iofile))
                iofile = SYSPara.SysDir + @"\Localdata\IOTable.xls";
            switch (io.LoadIOTableFromFile(iofile))
            {
                case 1: //1)無IOTable.xls
                    Errlist.Add("找不到 IOTable.xls 檔案");
                    break;
                case 2: //2)讀取檔案錯誤 
                    Errlist.Add("讀取 IOTable.xls 檔案錯誤");
                    break;
                case 3: //3)檔案內容錯誤
                    Errlist.Add("讀取 IOTable.xls 檔案內容錯誤");
                    break;
                case 4: //4)其他錯誤
                    Errlist.Add("讀取 IOTable.xls 檔案發生其他錯誤");
                    break;
                case 5: //5)暫存檔複製錯誤
                    Errlist.Add("IOTable.xls 暫存檔複製錯誤");
                    break;
                case 6: //6)授權異常
                    MessageBox.Show("授權異常!\nLincese Fail!");
                    FormSet.mBanner.DisposTH();
                    FormSet.mMSS.DisposeTH();
                    return;
                case 7: //7超過授權日期
                    MessageBox.Show("超過試用日期!\nExceed trial date!");
                    FormSet.mBanner.DisposTH();
                    FormSet.mMSS.DisposeTH();
                    return;

                case 0: //0)正常
                    break;
            }
            //SYSPara.LicenseMode = io.LicenseMode;
            //SYSPara.LicenseDate = io.LicenseDate;
            //SYSPara.Licenser = io.Licenser;
            #endregion

            #region 載入 Language Table
            FormSet.mBanner.mCaption = "載入語系檔案";
            SYSPara.Lang = new Language(SYSPara.SysDir + @"\Localdata\language.xls", Properties.Resources.ResourceManager);
            switch (SYSPara.Lang.LoadFromFile())
            {
                case 1: //1)無Language.xls
                    Errlist.Add("找不到系統語系檔案");
                    break;
                case 2: //2)讀取檔案錯誤 
                    Errlist.Add("讀取系統語系檔案錯誤");
                    break;
                case 3: //3)檔案內容錯誤
                    Errlist.Add("讀取語系檔案內容錯誤");
                    break;
                case 4: //4)其他錯誤
                    Errlist.Add("讀取語系檔案發生其他錯誤");
                    break;
                case 0: //0)正常
                    break;
            }
            #endregion

            #region 載入各模組 DLL
            string ModuleName = "";
            try
            {
                DirectoryInfo di = new DirectoryInfo(SYSPara.SysDir + @"\Module\");
                DirectoryInfo[] diArr = di.GetDirectories();

                foreach (DirectoryInfo dri in diArr)
                {
                    ModuleName = dri.Name;
                    FileInfo[] files = dri.GetFiles("*.dll");
                    String ModulePath = "";
                    if (files.Count() == 0)
                    {
                        Errlist.Add(string.Format("{0} 資料夾下不含DLL檔案，DLL未載入", dri.FullName));
                        continue;
                    }
                    else if (files.Count() > 1)
                    {
                        //多份DLL檔案的話，就要找出屬於該模組的DLL
                        string modulename;
                        bool findit = false;
                        foreach (FileInfo file in files)
                        {
                            modulename = file.Name.Replace(file.Extension, "");
                            if (dri.Name.IndexOf(modulename) != -1)
                            {
                                ModulePath = dri.FullName + "\\" + modulename + ".dll";
                                findit = true;
                                break;
                            }
                        }
                        if (!findit)
                        {
                            Errlist.Add(string.Format("{0} 資料夾下含有多份DLL檔案卻未找到模組DLL", dri.FullName));
                            continue;
                        }
                    }
                    else
                        ModulePath = dri.FullName + "\\" + files[0].Name;
                    
                    if (File.Exists(ModulePath) == true)
                    {
                        Byte[] asmdata = System.IO.File.ReadAllBytes(ModulePath);
                        Assembly assembly = Assembly.Load(asmdata);

                        string AssemName = assembly.GetName().Name;
                        string ClassName = AssemName + "." + AssemName;

                        Type type = assembly.GetType(ClassName);
                        
                        object obj = assembly.CreateInstance(type.FullName);

                        //驗證DLL是否繼承至 BaseModuleInterface
                        BaseModuleInterface mModule = obj as BaseModuleInterface;
                        if (mModule != null)
                        {
                            FormSet.mBanner.mCaption = string.Format("載入 {0}", ModuleName);

                            ((BaseModuleInterface)obj).SysDir = SYSPara.SysDir;
                            ((BaseModuleInterface)obj).Text = mModule.Text;
                            ((BaseModuleInterface)obj).FormBorderStyle = FormBorderStyle.None; //將右上角工具視窗控制項取消
                            ((BaseModuleInterface)obj).TopLevel = false; //取消視窗最上層
                            ((BaseModuleInterface)obj).Name = ModuleName; 
                            FormSet.ModuleList.Add(obj);

                            //模組語系Initial
                            int langresult = ((BaseModuleInterface)obj).InitialLanguage(dri.FullName + @"\language.xls");
                            switch (langresult)
                            {
                                case 1: //1)無Language.xls
                                    Errlist.Add(string.Format("[{0}] 無此語系檔案", ModuleName));
                                    break;
                                case 2: //2)讀取檔案錯誤 
                                    Errlist.Add(string.Format("[{0}] 讀取語系檔案錯誤", ModuleName));
                                    break;
                                case 3: //3)檔案內容錯誤
                                    Errlist.Add(string.Format("[{0}] 讀取語系檔案內容錯誤", ModuleName));
                                    break;
                                case 4: //4)其他錯誤
                                    Errlist.Add(string.Format("[{0}] 讀取語系檔案發生其他錯誤", ModuleName));
                                    break;
                                case 0: //0)正常
                                    break;
                            }

                            //讀入模組設定 XML
                            string sFilePath = dri.FullName + "\\" + ModuleName + ".xml";
                            ((BaseModuleInterface)obj).SetDS.Initial(sFilePath,ModuleName);

                            //讀入模組 Alarm Table
                            int ModuleID = ((BaseModuleInterface)obj).GetModuleID();
                            if (!SYSPara.Alarm.AppendFromFile(dri.FullName + "\\Alarm.xls", ModuleID))
                                Errlist.Add(string.Format("{0}'s Alarm Table 載入失敗", ModuleName));

                            //設定各模組內軸控元件IO Port
                            int setcount = 0;
                            foreach (Motor pMotor in mModule.MotorList)
                            {
                                pMotor.Running = false;

                                IOMapping.IOTableDT iodata = io.FindIO(ModuleName, pMotor);
                                if (iodata!=null)
                                {
                                    setcount++;
                                    bool r1 = string.IsNullOrEmpty(iodata.IOPort);
                                    bool r2 = !iodata.Enabled;
                                    if (r1 || r2)
                                    {
                                        pMotor.Enabled = false;
                                        pMotor.Visible = false;
                                        continue;
                                    }

                                    pMotor.MotionCard = iodata.MotionCard;
                                    pMotor.MotorParameter.IOPort = iodata.IOPort;
                                    pMotor.AlarmPolarity = iodata.AlarmPolarity;
                                    pMotor.ServoOnPolarity = iodata.ServoOnPolarity;
                                    pMotor.Enabled = iodata.Enabled;
                                    pMotor.LockUI = iodata.LockUI;
                                    pMotor.MotorParameter.Acceleration = iodata.Acc;
                                    pMotor.MotorParameter.Deceleration = iodata.Dec;
                                    pMotor.MotorParameter.Direction = iodata.Direction;
                                    pMotor.MotorParameter.EncGearRatio = iodata.EncGearRatio;
                                    pMotor.MotorParameter.GearRatio = iodata.GearRatio;
                                    pMotor.MotorParameter.HomeDirection = iodata.HomeDirection;
                                    pMotor.MotorParameter.HomePos = iodata.HomeOffsetPos;
                                    pMotor.MotorParameter.InPosOn = iodata.InPosOn;
                                    pMotor.MotorParameter.IsSensorB = iodata.IsSensorB;
                                    pMotor.MotorParameter.IsUseSoftLimit = iodata.UseSoftLimit;
                                    pMotor.MotorParameter.JogHighSpeed = iodata.JogHighSpeed;
                                    pMotor.MotorParameter.JogLowSpeed = iodata.JogLowSpeed;
                                    pMotor.MotorParameter.PulseCtrlMode = iodata.PulseOutMode;
                                    pMotor.MotorParameter.ServoAlarmOn = iodata.ServoAlarmOn;
                                    pMotor.MotorParameter.SoftLimitN = iodata.SoftLimitN;
                                    pMotor.MotorParameter.SoftLimitP = iodata.SoftLimitP;
                                    pMotor.MotorParameter.HomeMode = iodata.HomeMode;
                                    pMotor.MotorParameter.MotorType = iodata.MotorType;
                                }
                                else
                                    Errlist.Add(string.Format("[{1}] Motor [{0}] 的 IOPort 錯誤", pMotor.Name, ModuleName));
                            }

                            foreach (InBit pIn in mModule.InBitList)
                            {
                                pIn.Running = false;

                                IOMapping.IOTableDT iodata = io.FindIO(ModuleName, pIn);
                                if (iodata != null)
                                {
                                    setcount++;
                                    bool r1 = string.IsNullOrEmpty(iodata.IOPort);
                                    bool r2 = !iodata.Enabled;
                                    if (r1 || r2)
                                    {
                                        pIn.Enabled = false;
                                        pIn.Visible = false;
                                        continue;
                                    }

                                    pIn.IOPort = iodata.IOPort;
                                    pIn.IOType = iodata.IOType;
                                    pIn.HSLSpeed = iodata.HSLSpeed;
                                    pIn.Enabled = iodata.Enabled;
                                    pIn.LockUI = iodata.LockUI;
                                }
                                else
                                    Errlist.Add(string.Format("[{1}] InBit [{0}] 的 IOPort 錯誤", pIn.Name, ModuleName));
                            }
                            foreach (OutBit pOut in mModule.OutBitList)
                            {
                                pOut.Running = false;

                                IOMapping.IOTableDT iodata = io.FindIO(ModuleName, pOut);
                                if (iodata != null)
                                {
                                    setcount++;
                                    bool r1 = string.IsNullOrEmpty(iodata.IOPort);
                                    bool r2 = !iodata.Enabled;
                                    if (r1 || r2)
                                    {
                                        pOut.Enabled = false;
                                        pOut.Visible = false;
                                        continue;
                                    }

                                    pOut.IOPort = iodata.IOPort;
                                    pOut.IOType = iodata.IOType;
                                    pOut.HSLSpeed = iodata.HSLSpeed;
                                    pOut.Enabled = iodata.Enabled;
                                    pOut.LockUI = iodata.LockUI;
                                }
                                else
                                    Errlist.Add(string.Format("[{1}] InBit [{0}] 的 IOPort 錯誤", pOut.Name, ModuleName));
                            }
                            foreach (AnalogIn pAI in mModule.AIList)
                            {
                                pAI.Running = false;

                                IOMapping.IOTableDT iodata = io.FindIO(ModuleName, pAI);
                                if (iodata != null)
                                {
                                    setcount++;
                                    bool r1 = string.IsNullOrEmpty(iodata.IOPort);
                                    bool r2 = !iodata.Enabled;
                                    if (r1 || r2)
                                    {
                                        pAI.Enabled = false;
                                        pAI.Visible = false;
                                        continue;
                                    }

                                    pAI.IOPort = iodata.IOPort;
                                    pAI.IOType = iodata.IOType;
                                    pAI.HSLSpeed = iodata.HSLSpeed;
                                    pAI.Enabled = iodata.Enabled;
                                    pAI.LockUI = iodata.LockUI;
                                }
                                else
                                    Errlist.Add(string.Format("[{1}] Analog In [{0}] 的 IOPort 錯誤", pAI.Name, ModuleName));
                            }
                            foreach (AnalogOut pAO in mModule.AOList)
                            {
                                pAO.Running = false;

                                IOMapping.IOTableDT iodata = io.FindIO(ModuleName, pAO);
                                if (iodata != null)
                                {
                                    setcount++;
                                    bool r1 = string.IsNullOrEmpty(iodata.IOPort);
                                    bool r2 = !iodata.Enabled;
                                    if (r1 || r2)
                                    {
                                        pAO.Enabled = false;
                                        pAO.Visible = false;
                                        continue;
                                    }

                                    pAO.IOPort = iodata.IOPort;
                                    pAO.IOType = iodata.IOType;
                                    pAO.HSLSpeed = iodata.HSLSpeed;
                                    pAO.Enabled = iodata.Enabled;
                                    pAO.LockUI = iodata.LockUI;
                                }
                                else
                                    Errlist.Add(string.Format("[{1}] Analog Out [{0}] 的 IOPort 錯誤", pAO.Name, ModuleName));
                            }

                            int iotablecount = io.IOList.Count(x => x.ModuleName == ModuleName);
                            if (iotablecount != setcount)
                            {
                                Errlist.Add(string.Format("[{1}] IOTable內含IO數量：{0}", iotablecount, ModuleName));
                                Errlist.Add(string.Format("[{1}] 實際設定IO數量：{0}", setcount, ModuleName));
                            }

                            //設定模擬模式
                            mModule.SetSimulation(SYSPara.Simulation);
                        }
                    }
                }
                //FormSet.ModuleList.Sort((x, y) => { return -((BaseModuleInterface)x).SortIndex.CompareTo(((BaseModuleInterface)y).SortIndex); });
            }
            catch (Exception)
            {
                Errlist.Add(string.Format("載入模組 [{0}] 發生例外，請檢查!", ModuleName));
            }
            #endregion

            //創建 主控流程 視窗
            FormSet.mMainFlow = new MainFlowF();
            FormSet.mMainFlow.Show();

            //ProV元件載入
            TObjForm.ObjFormInit(SYSPara.Simulation);
            FlowChart.ObjFormDebugMode = SYSPara.DebugMode;

            #region 使用者修改 (關聯式資料庫/其他使用者定義的視窗)
            //Woody 2020/9/4
            FormSet.mBladeZ1Data = new BladeZ1Data();
            FormSet.mBladeZ2Data = new BladeZ2Data();
            //FormSet.mToolF = new ToolF();

            #endregion

            #region 載入通用視窗

            FormSet.mBanner.mCaption = "載入通用模組視窗";

            //創建 Initial Form
            FormSet.mInitialF = new InitialForm();

            //創建 Package Select Form 視窗
            //2020/09/01 Woody 修改，產品設定視窗(資料夾版本)
            FormSet.mPackageSelF = new PackageSelectExF();

            //創建 Package Form
            //2020/09/01 Woody 修改，產品設定視窗(資料夾版本)

            FormSet.mPackage = new PackageExF();
            FormSet.mPackage.Show();

            //創建 Module Container 視窗
            FormSet.mModuleContainer = new  ModuleContainerForm();

            //創建 使用者登入 視窗
            if (SYSPara.LoginMode==0)
                FormSet.mUserLogin = new UserLoginF();
            else if (SYSPara.LoginMode ==1)
                FormSet.mUserLoginEx = new UserLoginEx();

            //創建開/結批視窗
            FormSet.mLotStartF = new LotStartForm();
            FormSet.mLotEndF = new LotEndForm();

            //+ By Max For SECS
            //創建通訊視窗
            int idx = SYSPara.OReadValue("CommProtocol").ToInt();
            if (idx == 1) //SECS
            {
                FormSet.mGemF = new ProVGemF();
                FormSet.mGemF.Show();
            }

            //創建 主頁 視窗
            switch (SYSPara.ScreenMode)
            {
                case 0:
                case 1:
                    FormSet.mMainF1 = new MainF1();
                    break;
                case 2:
                    FormSet.mMainF2 = new MainF2();
                    break;
                case 3:
                    FormSet.mMainF3 = new MainF3();
                    break;
            }

            foreach (BaseModuleInterface module in FormSet.ModuleList)
            {
                //將資料元件送至各模組
                module.SetDataManagement(FormSet.mOption.OptionDS, FormSet.m內參設定.OptionDS, FormSet.mPackage.PackageDS);

                //自動掃描模組內的按鈕，並記錄Log
                SYSPara.ScanButtonForLogCallBack(module);
            }

            #endregion

            #region 將視窗加入語系控管

            switch (SYSPara.ScreenMode)
            {
                case 0:
                case 1:
                    SYSPara.Lang.AddControl(FormSet.mMainF1);
                    break;
                case 2:
                    SYSPara.Lang.AddControl(FormSet.mMainF2);
                    break;
                case 3:
                    SYSPara.Lang.AddControl(FormSet.mMainF3);
                    break;
            }

            if (SYSPara.LoginMode == 0)
                SYSPara.Lang.AddControl(FormSet.mUserLogin);
            else if (SYSPara.LoginMode == 1)
                SYSPara.Lang.AddControl(FormSet.mUserLoginEx);
            SYSPara.Lang.AddControl(FormSet.mInitialF);
            SYSPara.Lang.AddControl(FormSet.mLogF);
            SYSPara.Lang.AddControl(FormSet.mModuleContainer);
            SYSPara.Lang.AddControl(FormSet.mOption);
            SYSPara.Lang.AddControl(FormSet.mPackage);
            SYSPara.Lang.AddControl(FormSet.mPackageSelF);
            SYSPara.Lang.AddControl(KCSDK.MotorJogForm.MotorJog);

            #endregion

            #region 使用者修改 (關聯式資料庫/其他使用者定義的視窗 加入語系控管)
            //Woody 2020/9/4
            SYSPara.Lang.AddControl(FormSet.mBladeZ1Data);
            SYSPara.Lang.AddControl(FormSet.mBladeZ2Data);

            //SYSPara.Lang.AddControl(FormSet.mToolF);

            #endregion

            //創建核心流程執行緒
            SYSPara.thState = new StateThread();

            #region 關閉 Splash Form
            FormSet.mBanner.mCaption = "載入完成，進入系統";
            FormSet.mBanner.DisposTH();
            FormSet.mBanner.Dispose();
            #endregion

            //顯示錯誤列表
            if (Errlist.Count > 0)
            {
                System.IO.File.WriteAllLines(SYSPara.SysDir + @"\SystemErrorlist.txt", Errlist);
                System.Diagnostics.Process.Start(SYSPara.SysDir + @"\SystemErrorlist.txt");
            }

            switch (SYSPara.ScreenMode)
            {
                case 0: //自動縮放
                    FormSet.mMainF1.WindowState = FormWindowState.Maximized;
                    Application.Run(FormSet.mMainF1);
                    break;
                case 1: //1440x900
                    Application.Run(FormSet.mMainF1);
                    break;
                case 2: //1024x768
                    Application.Run(FormSet.mMainF2);
                    break;
                case 3: //1366x768
                    Application.Run(FormSet.mMainF3);
                    break;
            }

            #region 釋放資源

            SYSPara.thState.DisposTH();
            FormSet.mMSS.DisposeTH();

            #endregion
            MMTimerAPI.TimeEndPeriod(1);
        }

        #region 例外處理

        static void OnProcessExit(object sender, EventArgs e)
        {
            if (UnHandledCount <= 0)
                return;

            string str = "";
            str = string.Format("系統發生未處理的例外錯誤,共計: {0} 次，請將LOG.提供給開發者分析！", UnHandledCount);

            // Tips : 若掛在 AppDomain.CurrentDomain.ProcessExit, 底下Msgbox會自動關掉...
            MessageBox.Show(str, "系統離開", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // 檔案總管,定位反白選到檔案
            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", @"Log\ErrLog\ErrLog.txt"));

            /**  Tips : 自動開啟 ..        
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = @"Log\ErrLog\ErrLog.txt",
                UseShellExecute = true,
                WindowsStyle  = System.Diagnostics.ProcessWindowStyle.Hidden,
                CreateNoWindow = false;
                Verb = "open",
                Arguments = "我是參數"
            });
            **/
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = "";
            string strDateInfo = "出現UI線程未處理的異常：" + DateTime.Now.ToString() + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "異常類型：{0}\r\n異常消息：{1}\r\n異常堆棧追蹤：{2}\r\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("應程程序線程錯誤:{0}", e);
            }
            writeLog(str);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "發生應用線程未處理的異常：" + DateTime.Now.ToString() + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆棧追蹤:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }
            writeLog(str);
        }

        // log 文件記錄 - 採 Append 模式
        static void writeLog(string str)
        {
            if (!Directory.Exists("Log\\ErrLog"))
            {
                Directory.CreateDirectory("Log\\ErrLog");
            }

            using (StreamWriter sw = new StreamWriter(@"Log\ErrLog\ErrLog.txt", true /*appends*/ ))
            {
                sw.WriteLine(str);
                sw.WriteLine("---------------------------------------------------------");
                sw.Close();
                UnHandledCount++;
            }
        }

        #endregion

        //+ By Max 20200204 For LogAnalyzer
        //Modified By Noke 20200925 For PostgreSQL
        private static void CreateLogDB()
        {
            var check = false;
            try
            {
                SYSPara.logDB.RunBeforeStartup();
            }
            catch (InvalidOperationException)
            {
                check = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    MessageBox.Show(ex.Message, "訊息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(ex.InnerException.Message, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (check)
            {
                BackupDbAndClose();
            }
        }
        //Modified By Noke 20200925 For PostgreSQL
        private static async void BackupDbAndClose()
        {
            SYSPara.logDB.BackupAndDeleteDb();
            await Task.Delay(2000);
            FormSet.mUserLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
            switch (SYSPara.ScreenMode)
            {
                case 0: //自動縮放 
                case 1:
                    FormSet.mMainF1.Close();
                    break;
                case 2: //1024x768
                    FormSet.mMainF2.Close();
                    break;
                case 3: //1366x768
                    FormSet.mMainF3.Close();
                    break;
            }
        }

    }
}
