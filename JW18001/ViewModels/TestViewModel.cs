using JW18001.Commands;
using JW18001.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace JW18001.ViewModels
{
    internal class TestViewModel : NotificationObject
    {
        private Brush refBtn;
        private int gCnt;
        private byte refCh;
        private const string IniDirPath = @".//UserSet";
        private string IniFileName = "Set.ini";
        private bool isWave1TestEnable;
        private bool isWave2TestEnable;
        private bool isWave3TestEnable;
        private bool isWave4TestEnable;
        private byte opmWaveIndex = 1;
        private int pdlSampTime;
        private bool isIlTestEnable;
        private bool isPdlTestEnable;
        private bool isRlTestEnable;
        private bool isPdlSampleTime1Check;
        private bool isPdlSampleTime2Check;
        private bool isPdlSampleTime3Check;
        private bool isMaxSampleCheck;
        private bool isMinSampleCheck;
        private bool isAverSampleCheck;
        private bool isIlTestCheck;
        private bool isPdlTestCheck;
        private string testResult;
        private Thread thSwitchRef;
        private Thread thSwitchWave;
        private bool isSwitchedOpm;
        private bool isSwitchedLight;
        private int lightRefIndex = 0x01;
        private int lightIndex = 0x01;
        private int index = 0x01;
        private bool isRefed;
        private string sn;
        private string subSn;
        private string oldProductCode;
        private string pdlSampleTimeIndex;
        private string ilSampleModeIndex;
        private string ilTestMode;
        private string pdlTestMode;
        private bool isEndTest;
        private readonly List<string> lstUserItem;
        private bool isRunThread = true;
        private Brush testResColor;
        private readonly DataTable dataTable1 = new DataTable();
        private readonly DataTable dataTable2 = new DataTable();
        private readonly string startupPath = AppDomain.CurrentDomain.BaseDirectory;
        public ObservableCollection<DisplayDataModel> displayData;
        public ObservableCollection<TestItemModel> testItem;
        public ObservableCollection<RefSetModel> refSet;
        public ObservableCollection<TestDataDetailModel> testDataDetail;
        public ObservableCollection<ThreSetModel> threSet;
        private string saveExcelPath;
        private BasicInfoViewModel basicInfo = new BasicInfoViewModel();
        private DataStore dataStore = new DataStore();
        private List<string> testTime = new List<string>();
        private string saveExcelName;
        public ICommand RefWaveCommand { get; set; }
        public ICommand SwitchWaveCommand { get; set; }
        public ICommand SetPdLSampTimeCommand { get; set; }
        public ICommand SetIlSampModeCommand { get; set; }
        public ICommand SetTestModeCommand { get; set; }
        public ICommand TestCommand { get; set; }
        public ICommand TestCompleteCommand { get; set; }
        public ICommand SetExcelDataPathCommand { get; set; }
        public ICommand SaveUserSetConfigCommand { get; set; }
        public ICommand ReadUserSetConfigCommand { get; set; }

        public TestViewModel()
        {
            lstUserItem = new List<string>(Person.Channel);
            foreach (string t in Person.StrUserItem)
            {
                lstUserItem.Add(t);
            }
            for (int i = 0; i < Person.Channel - Person.StrUserItem.Length; i++)
            {
                lstUserItem.Add(string.Empty);
            }
            DisplayData = new ObservableCollection<DisplayDataModel>();
            TestItem = new ObservableCollection<TestItemModel>();
            RefSet = new ObservableCollection<RefSetModel>();
            TestDataDetail = new ObservableCollection<TestDataDetailModel>();
            ThreSet = new ObservableCollection<ThreSetModel>();
            LoadSpEventArgs();
            LoadDelegateCmd();
            InitDisplay();
            InitDataStore();
            InitDataTable();
            LoadUserSetConfig(Path.Combine(IniDirPath, IniFileName));
        }

        private void LoadSpEventArgs()
        {
            SerialPortHelper.ReadValueReceived += SerialPortHelper_ReadValueReceived;
            SerialPortHelper.GetRefDataReceived += SerialPortHelper_GetRefDataReceived;
            SerialPortHelper.SwitchOpmWaveReceived += SerialPortHelper_SwitchOpmWaveReceived;
            SerialPortHelper.SwitchLightReceived += SerialPortHelper_SwitchLightReceived;
            SerialPortHelper.SetIlSampModeReceived += SerialPortHelper_SetIlSampModeReceived;
            SerialPortHelper.SetInstruTestModeReceived += SerialPortHelper_SetInstruTestModeReceived;
            SerialPortHelper.SetPdlSampTimeReceived += SerialPortHelper_SetPdlSampTimeReceived;
            SerialPortHelper.StartTestReceived += SerialPortHelper_StartTestReceived;
        }

        private void LoadDelegateCmd()
        {
            RefWaveCommand = new DelegateCommand(RefWaveCmd);
            SwitchWaveCommand = new DelegateCommand(SwitchWaveCmd);
            SetIlSampModeCommand = new DelegateCommand(SetIlSampModeCmd);
            SetPdLSampTimeCommand = new DelegateCommand(SetPdlSampTimeCmd);
            SetTestModeCommand = new DelegateCommand(SetTestModeCmd);
            TestCommand = new DelegateCommand(TestCmd);
            TestCompleteCommand = new DelegateCommand(TestCompleteCmd);
            SaveUserSetConfigCommand = new DelegateCommand(SaveUserSetConfigCmd);
            SetExcelDataPathCommand = new DelegateCommand(SetExcelDataPathCmd);
            ReadUserSetConfigCommand = new DelegateCommand(ReadUserSetConfigCmd);
        }

        private void ReadUserSetConfigCmd(object obj)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                InitialDirectory = Path.Combine(startupPath, "UserSet"),
                Filter = "IniFiles(*.ini)|*.ini"
            };

            var res = dlg.ShowDialog();
            if (res == true)
            {
                var selFile = dlg.FileName;
                LoadUserSetConfig(selFile);
            }
        }

        private void InitDataStore()
        {
            dataStore.TestWave = new List<string>();
            dataStore.TestIl = new List<string>();
            dataStore.TestPdl = new List<string>();
            dataStore.TestRl = new List<string>();
            dataStore.TestChannelNumber = new List<int>();
        }

        private void SetExcelDataPathCmd(object obj)
        {
            var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                SaveExcelPath = fbd.SelectedPath;
            }
        }

        private void SaveRef()
        {
            string time = refSet[0].Time;
            string iniPath = Path.Combine(startupPath, "UserSet", IniFileName);
            IniFileHelper.IniWriteValue(iniPath, "Ref", "Time", time);
            string[] strRef = new string[Person.Channel];
            for (int i = 0; i < Person.Channel; i++)
            {
                strRef[i] = refSet[i].Wave1Ref;
            }
            var tempRef = string.Join(",", strRef);
            IniFileHelper.IniWriteValue(iniPath, "Ref", "1310nm", tempRef);
            for (int i = 0; i < Person.Channel; i++)
            {
                strRef[i] = refSet[i].Wave2Ref;
            }
            tempRef = string.Join(",", strRef);
            IniFileHelper.IniWriteValue(iniPath, "Ref", "1490nm", tempRef);
            for (int i = 0; i < Person.Channel; i++)
            {
                strRef[i] = refSet[i].Wave3Ref;
            }
            tempRef = string.Join(",", strRef);
            IniFileHelper.IniWriteValue(iniPath, "Ref", "1550nm", tempRef);
            for (int i = 0; i < Person.Channel; i++)
            {
                strRef[i] = refSet[i].Wave4Ref;
            }
            tempRef = string.Join(",", strRef);
            IniFileHelper.IniWriteValue(iniPath, "Ref", "1625nm", tempRef);
        }

        private void SaveUserSetConfigCmd(object obj)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
            {
                Filter = @"|*.ini",
                InitialDirectory = Path.Combine(startupPath, "UserSet"),
                FileName = Path.GetFileNameWithoutExtension(IniFileName),
                CreatePrompt = true,
                OverwritePrompt = true,
                AddExtension = true
            };
            var res = saveFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                var saveFileName = saveFileDialog.FileName;
                string path = Path.Combine(startupPath, "UserSet", saveFileName);
                IniFileName = saveFileName;
                if (saveFileName != string.Empty)
                {
                    IniFileHelper.IniWriteValue(path, "UserSet", "ExcelPath", SaveExcelPath);
                }
                if (IsPdlSampleTime1Check)
                {
                    pdlSampleTimeIndex = "1";
                }
                if (IsPdlSampleTime2Check)
                {
                    pdlSampleTimeIndex = "2";
                }
                if (IsPdlSampleTime3Check)
                {
                    pdlSampleTimeIndex = "3";
                }
                IniFileHelper.IniWriteValue(path, "UserSet", "PdlSampleTime", pdlSampleTimeIndex);
                if (IsMaxSampleCheck)
                {
                    ilSampleModeIndex = "1";
                }
                if (IsMinSampleCheck)
                {
                    ilSampleModeIndex = "2";
                }
                if (IsAverSampleCheck)
                {
                    ilSampleModeIndex = "3";
                }
                IniFileHelper.IniWriteValue(path, "UserSet", "IlSampleMode", ilSampleModeIndex);
                ilTestMode = IsIlTestEnable ? "1" : "0";
                pdlTestMode = IsPdlTestEnable ? "1" : "0";
                List<bool> lstTestWave = new List<bool>
                {
                    IsWave1TestEnable,
                    IsWave2TestEnable,
                    IsWave3TestEnable,
                    IsWave4TestEnable
                };
                IniFileHelper.IniWriteValue(path, "UserSet", "TestWave", string.Join(",", lstTestWave.ToArray()));
                Person.IlLowerThre.Clear();
                Person.IlUpperThre.Clear();
                Person.PdlLowerThre.Clear();
                Person.PdlUpperThre.Clear();
                Person.RlLowerThre.Clear();
                Person.RlUpperThre.Clear();
                for (int i = 0; i < Person.TestWave.Length; i++)
                {
                    List<string> lst = new List<string>
                    {
                        threSet[i].IlLowerLimit.ToString(),
                        threSet[i].IlUpperLimit.ToString(),
                        threSet[i].PdlLowerLimit.ToString(),
                        threSet[i].PdlUpperLimit.ToString(),
                        threSet[i].RlLowerLimit.ToString(),
                        threSet[i].RlUpperLimit.ToString()
                    };

                    Person.IlLowerThre.Add(threSet[i].IlLowerLimit);
                    Person.IlUpperThre.Add(threSet[i].IlUpperLimit);
                    Person.PdlLowerThre.Add(threSet[i].PdlLowerLimit);
                    Person.PdlUpperThre.Add(threSet[i].PdlUpperLimit);
                    Person.RlLowerThre.Add(threSet[i].RlLowerLimit);
                    Person.RlUpperThre.Add(threSet[i].RlUpperLimit);
                    var temp = string.Join(",", lst.ToArray());
                    IniFileHelper.IniWriteValue(path, "Threshold", Person.TestWave[i], temp);
                }
            }
        }

        private void TestCompleteCmd(object obj)
        {
            FillDataTable();
            SqlHelper.WriteToServer(dataTable2, SqlHelper.TableName);
            DbHelper.WriteToServer(dataTable2, DbHelper.DataSource, DbHelper.TableName);
            WriteToExcel();
            TestItem.Clear();
            TestDataDetail.Clear();
            dataTable2.Clear();

            for (int i = 0; i < Person.Channel; i++)
            {
                TestItemModel testItemModel = new TestItemModel { Channel = string.Concat("CH", i + 1) };
                testItem.Add(testItemModel);
                TestDataDetailModel testDataDetailModel = new TestDataDetailModel { UserItem = lstUserItem[i], Channel = string.Concat("CH", i + 1) };
                TestDataDetail.Add(testDataDetailModel);
            }
            TestResColor = Brushes.White;
            TestResult = string.Empty;
        }

        private void SerialPortHelper_StartTestReceived(byte[] data)
        {
            //lightIndex++;
        }

        private void ThreadRefSwitch()
        {
            while (lightRefIndex <= 4 && Person.IsThreadFlag)
            {
                SwitchLight(lightRefIndex);
                while (isSwitchedLight)
                {
                    SwitchOpmWave(refCh, lightRefIndex + 2);
                    Thread.Sleep(200);
                    while (isSwitchedOpm)
                    {
                        RefWaveCmd(refCh);
                        Thread.Sleep(300);
                        isSwitchedLight = false;

                        if (isRefed)
                        {
                            index = lightRefIndex++;
                            isRefed = false;
                        }
                        break;
                    }
                    Thread.Sleep(10);
                }
                Thread.Sleep(200);
            }
        }

        public ObservableCollection<DisplayDataModel> DisplayData

        {
            get { return displayData; }
            set
            {
                displayData = value;
                RaisePropertyChanged("DisplayData");
            }
        }

        public ObservableCollection<TestItemModel> TestItem
        {
            get { return testItem; }
            set
            {
                testItem = value;
                RaisePropertyChanged("TestItem");
            }
        }

        public ObservableCollection<RefSetModel> RefSet
        {
            get { return refSet; }
            set
            {
                refSet = value;
                RaisePropertyChanged("RefSet");
            }
        }

        public Brush RefBtn
        {
            get { return refBtn; }
            set
            {
                refBtn = value;
                RaisePropertyChanged("RefBtn");
            }
        }

        public int PdlSampTime
        {
            get { return pdlSampTime; }
            set
            {
                pdlSampTime = value;
                RaisePropertyChanged("PdlSampTime");
            }
        }

        public bool IsIlTestEnable
        {
            get { return isIlTestEnable; }
            set
            {
                isIlTestEnable = value;
                RaisePropertyChanged("IsIlTestEnable");
            }
        }

        public bool IsPdlTestEnable
        {
            get { return isPdlTestEnable; }
            set
            {
                isPdlTestEnable = value;
                RaisePropertyChanged("IsPdlTestEnable");
            }
        }

        public bool IsPdlSampleTime1Check
        {
            get { return isPdlSampleTime1Check; }
            set
            {
                isPdlSampleTime1Check = value;
                RaisePropertyChanged("IsPdlSampleTime1Check");
            }
        }

        public bool IsPdlSampleTime2Check
        {
            get { return isPdlSampleTime2Check; }
            set
            {
                isPdlSampleTime2Check = value;
                RaisePropertyChanged("IsPdlSampleTime2Check");
            }
        }

        public bool IsPdlSampleTime3Check
        {
            get { return isPdlSampleTime3Check; }
            set
            {
                isPdlSampleTime3Check = value;
                RaisePropertyChanged("IsPdlSampleTime3Check");
            }
        }

        public bool IsMaxSampleCheck
        {
            get { return isMaxSampleCheck; }
            set
            {
                isMaxSampleCheck = value;
                RaisePropertyChanged("IsMaxSampleCheck");
            }
        }

        public bool IsMinSampleCheck
        {
            get { return isMinSampleCheck; }
            set
            {
                isMinSampleCheck = value;
                RaisePropertyChanged("IsMinSampleCheck");
            }
        }

        public bool IsAverSampleCheck
        {
            get { return isAverSampleCheck; }
            set
            {
                isAverSampleCheck = value;
                RaisePropertyChanged("IsAverSampleCheck");
            }
        }

        public string Sn
        {
            get { return sn; }
            set
            {
                sn = value;
                RaisePropertyChanged("Sn");
            }
        }

        public string SubSn
        {
            get { return subSn; }
            set
            {
                subSn = value;
                RaisePropertyChanged("SubSn");
            }
        }

        public ObservableCollection<TestDataDetailModel> TestDataDetail
        {
            get { return testDataDetail; }
            set
            {
                testDataDetail = value;
                RaisePropertyChanged("TestDataDetail");
            }
        }

        public ObservableCollection<ThreSetModel> ThreSet
        {
            get { return threSet; }
            set
            {
                threSet = value;
                RaisePropertyChanged("ThreSetModel");
            }
        }

        public bool IsWave1TestEnable
        {
            get { return isWave1TestEnable; }
            set
            {
                isWave1TestEnable = value;
                RaisePropertyChanged("IsWave1TestEnable");
            }
        }

        public bool IsWave2TestEnable
        {
            get { return isWave2TestEnable; }
            set
            {
                isWave2TestEnable = value;
                RaisePropertyChanged("IsWave2TestEnable");
            }
        }

        public bool IsWave3TestEnable
        {
            get { return isWave3TestEnable; }
            set
            {
                isWave3TestEnable = value;
                RaisePropertyChanged("IsWave3TestEnable");
            }
        }

        public bool IsWave4TestEnable
        {
            get { return isWave4TestEnable; }
            set
            {
                isWave4TestEnable = value;
                RaisePropertyChanged("IsWave4TestEnable");
            }
        }

        public string SaveExcelPath
        {
            get { return saveExcelPath; }
            set
            {
                saveExcelPath = value;
                RaisePropertyChanged("SaveExcelPath");
            }
        }

        public bool IsRlTestEnable
        {
            get { return isRlTestEnable; }
            set
            {
                isRlTestEnable = value;
                RaisePropertyChanged("IsRlTestEnable");
            }
        }

        public string TestResult
        {
            get { return testResult; }
            set
            {
                testResult = value;
                RaisePropertyChanged("TestResult");
            }
        }

        public Brush TestResColor
        {
            get { return testResColor; }
            set
            {
                testResColor = value;
                RaisePropertyChanged("TestResColor");
            }
        }

        public string SaveExcelName
        {
            get { return saveExcelName; }
            set
            {
                saveExcelName = value;
                RaisePropertyChanged("SaveExcelName");
            }
        }

        private void TestCmd(object obj)
        {
            Person.IsThreadFlag = true;
            //  lightIndex = 0x01;
            // RunTest(0x01);

            thSwitchWave = new Thread(RunTestThread);
            thSwitchWave.Start();
            //thSwitchWave = new Thread(ThreadSwitchWave);
            //thSwitchWave.Start();

            //SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.StartTest);
            //SerialPortHelper.ts = true;
        }

        private void SendTestMode()
        {
            byte[] data = new byte[256];
            data[0] = Convert.ToByte(IsIlTestEnable);
            data[1] = Convert.ToByte(IsPdlTestEnable);
            data[2] = 0;
            SendCmd(Cmd.SetTestMode, data, 3);
        }

        private void RunTest(int ch)
        {
            while (isRunThread)
            {
                SwitchLight(ch);
                while (isSwitchedLight)
                {
                    SwitchFullOpm(ch + 2);
                    Thread.Sleep(200);
                    while (isSwitchedOpm)
                    {
                        SendTestMode();
                        Thread.Sleep(300);
                        SendCmd(Cmd.StartTest);
                        Thread.Sleep(3000);
                        isSwitchedLight = false;
                        if (isEndTest)
                        {
                            isEndTest = false;
                            isRunThread = false;
                            testTime.Add(DateTime.Now.ToString("yyyy-M-d hh-mm-ss"));
                        }

                        break;
                    }
                    Thread.Sleep(10);
                }
            }
        }

        private void GetTestReuslt()
        {
            for (int i = 0; i < Person.Channel; i++)
            {
                //Wave1
                if (TestDataDetail[i].Wave1Il != null)
                {
                    float wave1Il = Convert.ToSingle(TestDataDetail[i].Wave1Il);
                    if (wave1Il < Person.IlLowerThre[0] || wave1Il >= Person.IlUpperThre[0])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                if (TestDataDetail[i].Wave1Pdl != null)
                {
                    float wave1Pdl = Convert.ToSingle(TestDataDetail[i].Wave1Pdl);
                    if (wave1Pdl < Person.PdlLowerThre[0] || wave1Pdl >= Person.PdlUpperThre[0])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                //Wave2
                if (TestDataDetail[i].Wave2Il != null)
                {
                    float wave2Il = Convert.ToSingle(TestDataDetail[i].Wave2Il);
                    if (wave2Il < Person.IlLowerThre[1] || wave2Il >= Person.IlUpperThre[1])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                if (TestDataDetail[i].Wave2Pdl != null)
                {
                    float wave2Pdl = Convert.ToSingle(TestDataDetail[i].Wave2Pdl);
                    if (wave2Pdl < Person.PdlLowerThre[1] || wave2Pdl >= Person.PdlUpperThre[1])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                //Wave3
                if (TestDataDetail[i].Wave3Il != null)
                {
                    float wave3Il = Convert.ToSingle(TestDataDetail[i].Wave3Il);
                    if (wave3Il < Person.IlLowerThre[2] || wave3Il >= Person.IlUpperThre[2])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                if (TestDataDetail[i].Wave3Pdl != null)
                {
                    float wave3Pdl = Convert.ToSingle(TestDataDetail[i].Wave3Pdl);
                    if (wave3Pdl < Person.PdlLowerThre[2] || wave3Pdl >= Person.PdlUpperThre[2])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                //Wave4
                if (TestDataDetail[i].Wave4Il != null)
                {
                    float wave4Il = Convert.ToSingle(TestDataDetail[i].Wave4Il);
                    if (wave4Il < Person.IlLowerThre[3] || wave4Il >= Person.IlUpperThre[3])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }

                if (TestDataDetail[i].Wave4Pdl != null)
                {
                    float wave4Pdl = Convert.ToSingle(TestDataDetail[i].Wave4Pdl);
                    if (wave4Pdl < Person.PdlLowerThre[3] || wave4Pdl >= Person.PdlUpperThre[3])
                    {
                        TestResult = "FAIL";
                        TestResColor = Brushes.Red;
                        break;
                    }
                }
                TestResult = "PASS";
                TestResColor = Brushes.Green;
            }
        }

        private void RunTestThread()
        {
            while (Person.IsThreadFlag)
            {
                if (IsWave1TestEnable)
                {
                    isRunThread = true;
                    lightIndex = 0x01;
                    RunTest(lightIndex);
                }
                if (IsWave2TestEnable)
                {
                    isRunThread = true;
                    lightIndex = 0x02;
                    RunTest(lightIndex);
                }
                if (IsWave3TestEnable)
                {
                    isRunThread = true;
                    lightIndex = 0x03;
                    RunTest(lightIndex);
                }
                if (IsWave4TestEnable)
                {
                    isRunThread = true;
                    lightIndex = 0x04;
                    RunTest(lightIndex);
                }

                GetTestReuslt();
                TestDataDetail[0].UserInfo = DateTime.Now.ToString("yyyy-MM-dd");
                TestDataDetail[1].UserInfo = DateTime.Now.ToString("HH:mm:ss");
                TestDataDetail[2].UserInfo = Sn;
                TestDataDetail[3].UserInfo = SubSn;
                TestDataDetail[4].UserInfo = TestResult;
                TestDataDetail[5].UserInfo = basicInfo.OpName;
                TestDataDetail[6].UserInfo = basicInfo.OpWorkId;

                Person.IsThreadFlag = false;

                Thread.Sleep(10);
            }
        }

        private void SendCmd(ushort cmd)
        {
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, cmd);
            SerialPortHelper.ts = true;
        }

        private void SendCmd(ushort cmd, byte[] data, int len)
        {
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, cmd, data, len);
            SerialPortHelper.ts = true;
        }

        private void SetTestModeCmd(object obj)
        {
            byte[] data = new byte[256];
            data[0] = Convert.ToByte(IsIlTestEnable);
            data[1] = Convert.ToByte(IsPdlTestEnable);
            data[2] = 0;

            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SetTestMode, data, 3);
            SerialPortHelper.ts = true;
        }

        private static void SetPdlSampTimeCmd(object obj)
        {
            byte[] data = new byte[256];
            data[0] = Convert.ToByte(obj);
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SetPdlSampTime, data, 1);
            SerialPortHelper.ts = true;
        }

        private static void SetIlSampModeCmd(object obj)
        {
            byte[] data = new byte[256];
            data[0] = 0;
            data[1] = Convert.ToByte(obj);
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SetIlSampMode, data, 2);
            SerialPortHelper.ts = true;
        }

        private void SerialPortHelper_SetPdlSampTimeReceived(byte[] data)
        {
            Person.MyMessageBox("Pdl采样时间设置ok");
        }

        private void SerialPortHelper_SetInstruTestModeReceived(byte[] data)
        {
            //  Person.MyMessageBox("仪表测试模式设置OK");
        }

        private void SerialPortHelper_SetIlSampModeReceived(byte[] data)
        {
            Person.MyMessageBox("IL采样模式设置OK");
        }

        private void SwitchWaveCmd(object obj)
        {
            SwitchFullOpm(opmWaveIndex);
            opmWaveIndex++;
            if (opmWaveIndex > 6)
            {
                opmWaveIndex = 1;
            }
        }

        private void SwitchFullOpm(int wave)
        {
            byte[] data = new byte[256];
            data[0] = 0;
            data[1] = (byte)wave;

            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SwitchOpmWave, data, 2);
            SerialPortHelper.ts = true;
        }

        private void SerialPortHelper_SwitchLightReceived(byte[] data)
        {
            isSwitchedLight = true;
        }

        private void SerialPortHelper_SwitchOpmWaveReceived(byte[] data)
        {
            isSwitchedOpm = true;
        }

        private void SerialPortHelper_GetRefDataReceived(byte[] data)
        {
            isRefed = true;
            if (refCh == 0)
            {
                for (int i = 0; i < Person.Channel; i++)
                {
                    string refData = ((float)BitConverter.ToInt16(data, 2 * i) / 100).ToString("F2");

                    switch (lightRefIndex)
                    {
                        case 1:

                            refSet[i].Wave1Ref = refData;
                            break;

                        case 2:
                            refSet[i].Wave2Ref = refData;
                            break;

                        case 3:
                            refSet[i].Wave3Ref = refData;
                            break;

                        case 4:
                            refSet[i].Wave4Ref = refData;
                            refSet[i].Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            SaveRef();
                            break;
                    }
                }
            }
            else
            {
                string refData = ((float)BitConverter.ToInt16(data, 0) / 100).ToString("F2");
                //refData = ((Convert.ToDouble(refData) + Person.CabValue[refCh - 1, lightRefIndex - 1])).ToString("F2");
                switch (lightRefIndex)
                {
                    case 1:
                        Console.WriteLine(1310);
                        refSet[refCh - 1].Wave1Ref = refData;

                        break;

                    case 2:
                        Console.WriteLine(1490);
                        refSet[refCh - 1].Wave2Ref = refData;

                        break;

                    case 3:
                        Console.WriteLine(1550);
                        refSet[refCh - 1].Wave3Ref = refData;

                        break;

                    case 4:
                        Console.WriteLine(1625);
                        refSet[refCh - 1].Wave4Ref = refData;
                        refSet[refCh - 1].Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        SaveRef();
                        break;
                }
                //displayData[refCh - 1].Db = (Convert.ToSingle(refData) - Convert.ToSingle(displayData[refCh - 1].Dbm)).ToString("F2");
            }
        }

        private void RefWaveCmd(object obj)
        {
            lightRefIndex = 1;
            Person.IsThreadFlag = true;
            refCh = (byte)(Convert.ToInt32(obj) + 1);

            thSwitchRef = new Thread(ThreadRefSwitch);
            thSwitchRef.Start();
            isSwitchedLight = false;
            isSwitchedOpm = false;
        }

        private void RefWaveCmd(int ch)
        {
            byte[] data = new byte[256];
            data[0] = (byte)ch;
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SetRef, data, 1);
            SerialPortHelper.ts = true;
        }

        /// <summary>
        /// 切换光功率计
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="wavelengh"></param>
        private void SwitchOpmWave(byte ch, int wavelengh)
        {
            byte[] data = new byte[256];
            data[0] = ch;
            data[1] = (byte)wavelengh;

            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SwitchOpmWave, data, 2);
            SerialPortHelper.ts = true;
        }

        /// <summary>
        /// 切换光开关
        /// </summary>
        /// <param name="ch"></param>
        private void SwitchLight(int ch)
        {
            byte[] data = new byte[256];
            data[0] = (byte)ch;
            SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SwitchLightSource, data, 1);
            SerialPortHelper.ts = true;
        }

        private void SerialPortHelper_ReadValueReceived(byte[] data)
        {
            int mode = data[0];
            switch (mode)
            {
                case 0:

                    for (int i = 0; i < Person.Channel; i++)
                    {
                        string opmWave = Person.DicWaveList[data[3 * i + 1]];
                        // Console.WriteLine(opmWave);
                        string dbm = ((float)BitConverter.ToInt16(data, 3 * i + 2) / 100).ToString("F2");
                        displayData[i].OpmWave = opmWave;
                        displayData[i].Dbm = dbm;

                        //   displayData[i].Dbm = ((Convert.ToDouble(dbm) + Person.CabValue[i, data[3 * i + 1] - 1])).ToString("F2");

                        if (index == 1)
                        {
                            Console.WriteLine("1310nm");
                            displayData[i].Db = ((Convert.ToSingle(refSet[i].Wave1Ref) - Convert.ToSingle(displayData[i].Dbm))).ToString("F2");
                        }
                        if (index == 2)
                        {
                            Console.WriteLine("1490nm");
                            displayData[i].Db = ((Convert.ToSingle(refSet[i].Wave2Ref) - Convert.ToSingle(displayData[i].Dbm))).ToString("F2");
                        }
                        if (index == 3)
                        {
                            Console.WriteLine("1550nm");
                            displayData[i].Db = ((Convert.ToSingle(refSet[i].Wave3Ref) - Convert.ToSingle(displayData[i].Dbm))).ToString("F2");
                        }
                        if (index == 4)
                        {
                            Console.WriteLine("1625nm");
                            displayData[i].Db = ((Convert.ToSingle(refSet[i].Wave4Ref) - Convert.ToSingle(displayData[i].Dbm))).ToString("F2");
                        }
                    }
                    break;

                //IL
                case 1:
                    for (int i = 0; i < Person.Channel; i++)
                    {
                        string wave = Person.DicWaveList[data[3 * i + 1]];
                        string il = ((float)BitConverter.ToInt16(data, 3 * i + 2) / 100).ToString("F2");
                        displayData[i].OpmWave = wave;
                        testItem[i].IlValue = il;
                        switch (lightIndex)
                        {
                            case 1:
                                testDataDetail[i].Wave1Il = il;
                                testDataDetail[i].Wave1TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                break;

                            case 2:
                                testDataDetail[i].Wave2Il = il;
                                testDataDetail[i].Wave2TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                break;

                            case 3:
                                testDataDetail[i].Wave3Il = il;
                                testDataDetail[i].Wave3TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                break;

                            case 4:
                                testDataDetail[i].Wave4Il = il;
                                testDataDetail[i].Wave4TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                break;
                        }
                    }

                    isEndTest = true;

                    break;

                //PDL
                case 2:

                    for (int i = 0; i < Person.Channel; i++)
                    {
                        string wave = Person.DicWaveList[data[3 * i + 1]];
                        string pdl = ((float)BitConverter.ToInt16(data, 2 + 3 * i) / 100).ToString("F2");
                        displayData[i].OpmWave = wave;

                        testItem[i].PdlValue = pdl;
                        if (lightIndex == 1)
                        {
                            testDataDetail[i].Wave1Pdl = pdl;
                            testDataDetail[i].Wave1TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (lightIndex == 2)
                        {
                            testDataDetail[i].Wave2Pdl = pdl;
                            testDataDetail[i].Wave2TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (lightIndex == 3)
                        {
                            testDataDetail[i].Wave3Pdl = pdl;
                            testDataDetail[i].Wave3TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        if (lightIndex == 4)
                        {
                            testDataDetail[i].Wave4Pdl = pdl;
                            testDataDetail[i].Wave4TestTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }

                    isEndTest = true;
                    break;
            }
        }

        private void InitDisplay()
        {
            for (int i = 0; i < Person.Channel; i++)
            {
                DisplayDataModel displayDataModel = new DisplayDataModel { Channel = string.Concat("CH", i + 1) };
                displayData.Add(displayDataModel);
                TestItemModel testItemModel = new TestItemModel { Channel = string.Concat("CH", i + 1) };
                testItem.Add(testItemModel);
                TestDataDetailModel testDataDetailModel = new TestDataDetailModel { UserItem = lstUserItem[i], Channel = string.Concat("CH", i + 1) };
                TestDataDetail.Add(testDataDetailModel);
                RefSetModel refSetModel = new RefSetModel { Channel = string.Concat("CH", i + 1) };
                refSet.Add(refSetModel);
            }
        }

        private string GetIniPath()
        {
            if (IsSnInput())
            {
                string newProductCode = Sn.Substring(2, 8);
                string fileName = string.Format("{0}.ini", newProductCode);
                if (!string.IsNullOrEmpty(oldProductCode))
                {
                    if (!oldProductCode.Equals(newProductCode))
                    {
                        MessageBox.Show("产品编码不同!");
                    }
                }
                oldProductCode = newProductCode;
                string iniPath = Path.Combine(IniDirPath, fileName);
                if (File.Exists(iniPath))
                {
                    return iniPath;
                }
                MessageBox.Show("无该产品对应的编码配置信息");
            }
            return null;
        }

        private bool IsSnInput()
        {
            Sn = Sn.Trim();
            if (!string.IsNullOrEmpty(Sn))
            {
                if (Sn.Length == 28)
                {
                    string s1 = Sn.Substring(10, 1);
                    string s2 = Sn.Substring(21, 1);
                    if (s1.Equals("/") && s2.Equals("S"))
                    {
                        return true;
                    }
                }
            }
            MessageBox.Show("SN错误，请输入正确的SN");
            return false;
        }

        private void LoadUserSetConfig(string filePath)
        {
            //Excel文件名
            SaveExcelName = IniFileHelper.IniGetStringValue(filePath, "UserSet", "ExcelName", null);

            //Excel路径

            SaveExcelPath = IniFileHelper.IniGetStringValue(filePath, "UserSet", "ExcelPath", null);
            //pdl采样时间
            pdlSampleTimeIndex = IniFileHelper.IniGetStringValue(filePath, "UserSet", "PdlSampleTime", null);
            if (pdlSampleTimeIndex == "1")
            {
                IsPdlSampleTime1Check = true;
            }
            if (pdlSampleTimeIndex == "2")
            {
                IsPdlSampleTime2Check = true;
            }
            if (pdlSampleTimeIndex == "3")
            {
                IsPdlSampleTime3Check = true;
            }
            //插损采样模式设置
            ilSampleModeIndex = IniFileHelper.IniGetStringValue(filePath, "UserSet", "IlSampleMode", null);
            if (ilSampleModeIndex == "1")
            {
                IsMaxSampleCheck = true;
            }
            if (ilSampleModeIndex == "2")
            {
                IsMinSampleCheck = true;
            }
            if (ilSampleModeIndex == "3")
            {
                IsAverSampleCheck = true;
            }

            //插损测试模式
            ilTestMode = IniFileHelper.IniGetStringValue(filePath, "UserSet", "IlTestMode", null);
            if (ilTestMode == "1")
            {
                IsIlTestEnable = true;
            }
            else
            {
                IsIlTestEnable = false;
            }

            //pdl测试模式
            pdlTestMode = IniFileHelper.IniGetStringValue(filePath, "UserSet", "PdlTestMode", null);
            if (pdlTestMode == "1")
            {
                IsPdlTestEnable = true;
            }
            else
            {
                IsPdlTestEnable = false;
            }

            string[] testWave = IniFileHelper.IniGetStringValue(filePath, "UserSet", "TestWave", null).Split(',');
            IsWave1TestEnable = Convert.ToBoolean(testWave[0]);
            IsWave2TestEnable = Convert.ToBoolean(testWave[1]);
            IsWave3TestEnable = Convert.ToBoolean(testWave[2]);
            IsWave4TestEnable = Convert.ToBoolean(testWave[3]);

            threSet.Clear();
            string[] threkey = IniFileHelper.IniGetAllItemKeys(filePath, "Threshold");
            foreach (string str in threkey)
            {
                ThreSetModel t = new ThreSetModel();
                var value = IniFileHelper.IniGetStringValue(filePath, "Threshold", str, null);
                var arrayValue = value.Split(',');
                t.Wave = str;
                t.IlLowerLimit = Convert.ToSingle(arrayValue[0]);
                t.IlUpperLimit = Convert.ToSingle(arrayValue[1]);
                t.PdlLowerLimit = Convert.ToSingle(arrayValue[2]);
                t.PdlUpperLimit = Convert.ToSingle(arrayValue[3]);
                t.RlLowerLimit = Convert.ToSingle(arrayValue[4]);
                t.RlUpperLimit = Convert.ToSingle(arrayValue[5]);

                Person.IlLowerThre.Add(t.IlLowerLimit);
                Person.IlUpperThre.Add(t.IlUpperLimit);
                Person.PdlLowerThre.Add(t.PdlLowerLimit);
                Person.PdlUpperThre.Add(t.PdlUpperLimit);
                Person.RlLowerThre.Add(t.RlLowerLimit);
                Person.RlUpperThre.Add(t.RlUpperLimit);
                threSet.Add(t);
            }

            //Ref

            string[] refKey = IniFileHelper.IniGetAllItemKeys(filePath, "Ref");
            var refTime = IniFileHelper.IniGetStringValue(filePath, "Ref", refKey[0], null);
            var arrayRefValue = IniFileHelper.IniGetStringValue(filePath, "Ref", refKey[1], null).Split(',');
            for (int i = 0; i < Person.Channel; i++)
            {
                refSet[i].Wave1Ref = arrayRefValue[i];
                refSet[i].Time = refTime;
            }

            arrayRefValue = IniFileHelper.IniGetStringValue(filePath, "Ref", refKey[2], null).Split(',');
            for (int i = 0; i < Person.Channel; i++)
            {
                refSet[i].Wave2Ref = arrayRefValue[i];
            }
            arrayRefValue = IniFileHelper.IniGetStringValue(filePath, "Ref", refKey[3], null).Split(',');
            for (int i = 0; i < Person.Channel; i++)
            {
                refSet[i].Wave3Ref = arrayRefValue[i];
            }
            arrayRefValue = IniFileHelper.IniGetStringValue(filePath, "Ref", refKey[4], null).Split(',');
            for (int i = 0; i < Person.Channel; i++)
            {
                refSet[i].Wave4Ref = arrayRefValue[i];
            }
        }

        private void WriteToExcel()
        {
            if (!Directory.Exists(SaveExcelPath))
            {
                Directory.CreateDirectory(SaveExcelPath);
            }
            string dir = Path.Combine(SaveExcelPath, string.Format("{0}.xlsx", SaveExcelName));

            if (!File.Exists(dir))
            {
                ExcelHelper.CreateExcelFile(dir);
            }

            string filePath = dir;
            ExcelHelper.DataTabletoExcel(dataTable2, filePath);
        }

        private void FillDataTable()
        {
            dataStore.SerialNumber = Sn;
            dataStore.SubSerialNumber = SubSn;
            dataStore.TestChannel = Person.Channel.ToString();
            dataStore.TestWave1Enable = IsWave1TestEnable;
            dataStore.TestWave2Enable = IsWave2TestEnable;
            dataStore.TestWave3Enable = IsWave3TestEnable;
            dataStore.TestWave4Enable = IsWave4TestEnable;
            dataStore.TestResult = "PASS";

            DataRow newRow = dataTable1.NewRow();
            newRow["TestSerialNumber"] = dataStore.SerialNumber;
            newRow["TestSubSerialNumber"] = dataStore.SubSerialNumber;
            newRow["TestChannel"] = dataStore.TestChannel;
            newRow["TestWave1Enable"] = dataStore.TestWave1Enable;
            newRow["TestWave2Enable"] = dataStore.TestWave2Enable;
            newRow["TestWave3Enable"] = dataStore.TestWave3Enable;
            newRow["TestWave4Enable"] = dataStore.TestWave4Enable;
            newRow["TestIlEnable"] = dataStore.TestIlEnable;
            newRow["TestPdlEnable"] = dataStore.TestPdlEnable;
            newRow["TestRlEnable"] = dataStore.TestRlEnable;
            newRow["TestResult"] = dataStore.TestResult;

            dataTable1.Rows.Add(newRow);
            //       SqlHelper.WriteToServer(dataTable1, "dbo.TSerialNumber");

            {
                for (int i = 0; i < Person.Channel; i++)
                {
                    DataRow row = dataTable2.NewRow();
                    row["TestSerialNumber"] = dataStore.SerialNumber;
                    row["TestChannelNumber"] = i + 1;
                    row["TestIl1"] = testDataDetail[i].Wave1Il;
                    row["TestPdl1"] = testDataDetail[i].Wave1Pdl;
                    row["TestRl1"] = testDataDetail[i].Wave1Rl;
                    row["TestTime1"] = testDataDetail[i].Wave1TestTime;

                    row["TestIl2"] = testDataDetail[i].Wave2Il;
                    row["TestPdl2"] = testDataDetail[i].Wave2Pdl;
                    row["TestRl2"] = testDataDetail[i].Wave2Rl;
                    row["TestTime2"] = testDataDetail[i].Wave2TestTime;

                    row["TestIl3"] = testDataDetail[i].Wave3Il;
                    row["TestPdl3"] = testDataDetail[i].Wave3Pdl;
                    row["TestRl3"] = testDataDetail[i].Wave3Rl;
                    row["TestTime3"] = testDataDetail[i].Wave3TestTime;

                    row["TestIl4"] = testDataDetail[i].Wave4Il;
                    row["TestPdl4"] = testDataDetail[i].Wave4Pdl;
                    row["TestRl4"] = testDataDetail[i].Wave4Rl;
                    row["TestTime4"] = testDataDetail[i].Wave4TestTime;
                    dataTable2.Rows.Add(row);
                }
            }
        }

        private void InsertTestData(int channel, string wave, string il, string pdl, string rl)
        {
            for (int i = 0; i < Person.Channel; i++)
            {
                //DataRow row = dataTable2.NewRow();
                //dataStore.SerialNumber = Sn;
                //dataStore.TestChannelNumber.Add(i);
                //dataStore.TestWave.Add(wave);
                //dataStore.TestIl.Add(il);
                //dataStore.TestPdl.Add(pdl);
                //dataStore.TestRl.Add(rl);
                //row["TestSerialNumber"] = dataStore.SerialNumber;
                //row["TestChannelNumber"] = dataStore.TestChannelNumber[i] + 1;
                //row["TestWave"] = dataStore.TestWave[i];
                //row["TestIl"] = dataStore.TestIl[i];
                //row["TestPdl"] = dataStore.TestPdl[i];
                //row["TestRl"] = dataStore.TestRl[i];
                //dataTable2.Rows.Add(row);
            }
        }

        private void InitDataTable()
        {
            dataTable1.Columns.Add("TestSerialNumber", typeof(string));
            dataTable1.Columns.Add("TestSubSerialNumber", typeof(string));
            dataTable1.Columns.Add("TestChannel", typeof(int));
            dataTable1.Columns.Add("TestWave1Enable", typeof(bool));
            dataTable1.Columns.Add("TestWave2Enable", typeof(bool));
            dataTable1.Columns.Add("TestWave3Enable", typeof(bool));
            dataTable1.Columns.Add("TestWave4Enable", typeof(bool));
            dataTable1.Columns.Add("TestIlEnable", typeof(bool));
            dataTable1.Columns.Add("TestPdlEnable", typeof(bool));
            dataTable1.Columns.Add("TestRlEnable", typeof(bool));
            dataTable1.Columns.Add("TestResult", typeof(string));

            //dataTable2.Columns.Add("TestSerialNumber", typeof(string));
            //dataTable2.Columns.Add("TestChannelNumber", typeof(int));
            //dataTable2.Columns.Add("TestWave", typeof(string));
            //dataTable2.Columns.Add("TestIl", typeof(string));
            //dataTable2.Columns.Add("TestPdl", typeof(string));
            //dataTable2.Columns.Add("TestRl", typeof(string));

            dataTable2.Columns.Add("TestSerialNumber", typeof(string));
            dataTable2.Columns.Add("TestChannelNumber", typeof(int));
            dataTable2.Columns.Add("TestIl1", typeof(string));
            dataTable2.Columns.Add("TestPdl1", typeof(string));
            dataTable2.Columns.Add("TestRl1", typeof(string));
            dataTable2.Columns.Add("TestTime1", typeof(string));
            dataTable2.Columns.Add("TestIl2", typeof(string));
            dataTable2.Columns.Add("TestPdl2", typeof(string));
            dataTable2.Columns.Add("TestRl2", typeof(string));
            dataTable2.Columns.Add("TestTime2", typeof(string));
            dataTable2.Columns.Add("TestIl3", typeof(string));
            dataTable2.Columns.Add("TestPdl3", typeof(string));
            dataTable2.Columns.Add("TestRl3", typeof(string));
            dataTable2.Columns.Add("TestTime3", typeof(string));
            dataTable2.Columns.Add("TestIl4", typeof(string));
            dataTable2.Columns.Add("TestPdl4", typeof(string));
            dataTable2.Columns.Add("TestRl4", typeof(string));
            dataTable2.Columns.Add("TestTime4", typeof(string));
        }
    }
}