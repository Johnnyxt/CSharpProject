using JW18001.Commands;
using JW18001.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;

namespace JW18001.ViewModels
{
    internal class MainLoginViewModel : NotificationObject
    {
        private Person.DataBase dataBase;
        private const string XmlPath = @".//Config.xml";
        private readonly DispatcherTimer dPort = new DispatcherTimer();
        private readonly byte[] txBytes = new byte[1024];
        private readonly XmlDocument xmlDoc = new XmlDocument();
        private string conState;
        private int gCnt;
        private bool isLink;
        private bool isMainLoginFailed = true;
        private string mainLoginName;
        private string mainLoginPsd;
        private int portCount;
        private string proType;
        private string selectedPort;
        private string title;
        private bool ts;
        private ObservableCollection<string> obsPort = new ObservableCollection<string>();
        private ObservableCollection<string> obsChannel = new ObservableCollection<string>();
        private readonly string[] channel = { "1x8", "2x8", "3x8", "4x8", "5x8", "6x8", "7x8", "8x8" };
        private int selChannel;
        private Thread thSendCmd;
        private int cmdStatus = 0;
        private bool isSend = true;

        private void Proc()
        {
            while (isSend)
            {
                if (cmdStatus == 1)
                {
                    SendStatus();
                    cmdStatus = 0;
                    Console.WriteLine("仪表状态确认");
                }
                if (cmdStatus == 2)
                {
                    SendCmd(Cmd.GetOpmWaveList);
                    cmdStatus = 0;
                    Console.WriteLine("获取OPM");
                }
                if (cmdStatus == 3)
                {
                    SendCmd(Cmd.GetLightWaveList);
                    cmdStatus = 0;
                    Console.WriteLine("获取光开关");
                }
                if (cmdStatus == 4)
                {
                    SerialPortHelper.DTimer.IsEnabled = true;
                    cmdStatus = 0;
                    isSend = false;
                }
            }
        }

        public MainLoginViewModel()
        {
            Title = Person.Vision;
            thSendCmd = new Thread(Proc);
            thSendCmd.Start();
            LoadXmlDoc();
            SerialPortHelper.ConnectReceived += SpHelper_ConnectReceived;
            SerialPortHelper.ConfirmHardWare += SerialPortHelper_ConfirmHardWare;
            SerialPortHelper.GetOpmWaveListReceived += SerialPortHelper_GetOpmWaveListReceived;
            SerialPortHelper.GetLightWaveListReceived += SerialPortHelper_GetLightWaveListReceived;
            ConState = "连接";
            InitPortTimer();
            InitCommand();
            foreach (var i in channel)
            {
                ObsChannel.Add(i);
            }
        }

        private void SerialPortHelper_GetLightWaveListReceived(byte[] data)
        {
            int waveSum = data[0];
            List<string> wave = new List<string>();
            for (int i = 0; i < waveSum; i++)
            {
                wave.Add(Person.DicWaveList[data[i + 1]]);
            }
            cmdStatus = 4;
            //Console.WriteLine(string.Join(",", wave.ToArray()));

            //  SerialPortHelper.DTimer.IsEnabled = true;
        }

        private void SerialPortHelper_GetOpmWaveListReceived(byte[] data)
        {
            int waveSum = data[0];
            List<string> wave = new List<string>();
            for (int i = 0; i < waveSum; i++)
            {
                wave.Add(Person.DicWaveList[data[i + 1]]);
            }
            //Console.WriteLine(string.Join(",", wave.ToArray()));
            //读取光源列表

            cmdStatus = 3;
            // SendCmd(Cmd.GetLightWaveList);
        }

        private void SerialPortHelper_ConfirmHardWare(byte[] data)
        {
            Person.Channel = data[1];
            if (Person.Channel > 64)
            {
            }
            cmdStatus = 2;
            //SendCmd(Cmd.GetOpmWaveList);
            //if (data[0] == 0)
            //{
            //    MessageBox.Show("仪表状态出错");
            //}
            //else
            //{
            //    //读取光功率计波长列表
            //    SendCmd(Cmd.GetOpmWaveList);
            //}
        }

        private void InitPortTimer()
        {
            dPort.IsEnabled = true;
            dPort.Interval = TimeSpan.FromMilliseconds(100);
            dPort.Tick += DPort_Tick;
        }

        private void InitCommand()
        {
            ConnPortCommand = new DelegateCommand(ConnPortCmd);
            MainLoginCommand = new DelegateCommand(MainLoginCmd);
            CloseCommand = new DelegateCommand(CloseCmd);
        }

        public string LoginName { get; set; }
        public string LoginPsd { get; set; }
        public ICommand ConnPortCommand { get; set; }
        public ICommand MainLoginCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public string SelectedPort
        {
            get { return selectedPort; }
            set
            {
                selectedPort = value;
                RaisePropertyChanged("SelectedPort");
            }
        }

        public ObservableCollection<string> ObsPort
        {
            get { return obsPort; }
            set
            {
                obsPort = value;
                RaisePropertyChanged("ObsPort");
            }
        }

        public string MainLoginName
        {
            get { return mainLoginName; }
            set
            {
                mainLoginName = value;
                RaisePropertyChanged("MainLoginName");
            }
        }

        public string MainLoginPsd
        {
            get { return mainLoginPsd; }
            set
            {
                mainLoginPsd = value;
                RaisePropertyChanged("MainLoginPsd");
            }
        }

        public string ConState
        {
            get { return conState; }
            set
            {
                conState = value;
                RaisePropertyChanged("ConState");
            }
        }

        public string ProType
        {
            get { return proType; }
            set
            {
                proType = value;
                RaisePropertyChanged("ProType");
            }
        }

        public bool IsMainLoginFailed
        {
            get { return isMainLoginFailed; }
            set
            {
                isMainLoginFailed = value;
                RaisePropertyChanged("IsMainLoginFailed");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged("Title");
            }
        }

        public ObservableCollection<string> ObsChannel
        {
            get { return obsChannel; }
            set
            {
                obsChannel = value;

                RaisePropertyChanged("ObsChannel");
            }
        }

        public int SelChannel
        {
            get { return selChannel; }
            set
            {
                selChannel = value;
                RaisePropertyChanged("SelChannel");
            }
        }

        private static void CloseCmd(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SpHelper_ConnectReceived(byte[] data)
        {
            isLink = true;
            SerialPortHelper.ConnectReceived -= SpHelper_ConnectReceived;
            Person.MyMessageBox("连接成功");

            var revision = BitConverter.ToUInt32(data, 0);
            ProType = revision.ToString("X");
            cmdStatus = 1;
            //Array.Clear(data, 0, data.Length);
            //SendStatus();
        }

        private void SendCmd(ushort cmd)
        {
            //SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, cmd);
            //SerialPortHelper.ts = true;

            gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(txBytes, cmd);
            Person.SpHelper.SerialPortWrite(txBytes, gCnt);

            //SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, cmd);
            //SerialPortHelper.ts = true;

            // Person.SpHelper.SerialPortWrite(txBytes, gCnt);
        }

        private void SendStatus()
        {
            //byte[] data = new byte[256];
            //data[0] = 0x04;
            //data[1] = (Convert.ToByte(Person.Channel));
            //data[2] = 0x01;
            //data[3] = 0;
            //data[4] = 0;
            //gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(txBytes, Cmd.SendTestStatus, data, 5);
            //Person.SpHelper.SerialPortWrite(txBytes, gCnt);

            gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(txBytes, Cmd.SendTestStatus);
            Person.SpHelper.SerialPortWrite(txBytes, gCnt);

            //SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.SendTestStatus, data, 5);
            //SerialPortHelper.ts = true;
        }

        private void MainLoginCmd(object obj)
        {
            if (!isLink)
            {
                MessageBox.Show("串口未连接", "JW8307A", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (LoginName == MainLoginName && LoginPsd == mainLoginPsd)
            {
                IsMainLoginFailed = false;
                Person.IsBasicInfoViewEnable = true;
                Person.IsUserSetViewEnable = true;

                //跳转到数据库登录界面
                var view = new DbLoginView();
                view.ShowDialog();
            }
            else
            {
                Person.SqlHelper = new SqlHelper(dataBase.ServerName, dataBase.DbName, dataBase.DbLoginName, dataBase.DbLoginPsd);
                if (Person.SqlHelper.IsConnect)
                {
                    IsMainLoginFailed = false;
                    Person.IsBasicInfoViewEnable = false;
                    Person.IsUserSetViewEnable = false;
                    DbLoginViewModel.SqlHelper = Person.SqlHelper;
                    var view = new BasicInfoView();

                    view.ShowDialog();
                }
                else
                {
                    MessageBox.Show("数据库登录失败,请联系管理员");
                }
            }

            //IsMainLoginFailed = false;
            //Person.IsBasicInfoViewEnable = false;
            //Person.IsUserSetViewEnable = false;
            //DbLoginViewModel.SqlHelper = Person.SqlHelper;
            //var view = new BasicInfoView();

            //view.ShowDialog();
        }

        private void LoadXmlDoc()
        {
            try
            {
                xmlDoc.Load(XmlPath);
                var selectSingleNode = xmlDoc.SelectSingleNode("Setting/MainLogin");
                if (selectSingleNode == null)
                    return;
                var nodeList = selectSingleNode.ChildNodes;
                var xNode = nodeList.Item(0);
                if (xNode != null)
                {
                    LoginName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(1);
                if (xNode != null)
                {
                    LoginPsd = xNode.InnerText.Trim();
                }

                selectSingleNode = xmlDoc.SelectSingleNode("Setting/ServerLogin");
                if (selectSingleNode == null)
                    return;
                nodeList = selectSingleNode.ChildNodes;
                xNode = nodeList.Item(0);
                if (xNode != null)
                {
                    dataBase.ServerName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(1);
                if (xNode != null)
                {
                    dataBase.DbName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(2);
                if (xNode != null)
                {
                    dataBase.DbLoginName = xNode.InnerText.Trim();
                }
                xNode = nodeList.Item(3);
                if (xNode != null)
                {
                    dataBase.DbLoginPsd = xNode.InnerText.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DPort_Tick(object sender, EventArgs e)
        {
            var keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            if (keyCom == null) return;
            var subKeys = keyCom.GetValueNames();
            if (portCount == subKeys.Length) return;
            obsPort.Clear();
            foreach (var name in subKeys)
            {
                var port = (string)keyCom.GetValue(name);
                obsPort.Add(port);
                portCount = subKeys.Length;
                SelectedPort = port;
            }
        }

        private void ConnPortCmd(object obj)
        {
            if (isLink)
            {
                Person.SpHelper.ClosePort();
                isLink = false;
                ConState = "连接";
                return;
            }

            if (!Person.SpHelper.IsOpen)
            {
                try
                {
                    Person.SpHelper.PortName = SelectedPort;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            try
            {
                Person.SpHelper.OpenPort();
                //发送连接指令

                gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(txBytes, Cmd.Connect);
                Person.SpHelper.SerialPortWrite(txBytes, gCnt);
                Array.Clear(txBytes, 0, txBytes.Length);

                //SerialPortHelper.gCnt = SerialPortHelper.ProtocolPars.Protocol_wr(SerialPortHelper.txBytes, Cmd.Connect);
                //SerialPortHelper.ts = true;
                //Array.Clear(SerialPortHelper.txBytes, 0, SerialPortHelper.txBytes.Length);

                Thread.Sleep(200);
                if (!isLink)
                {
                    Person.SpHelper.ClosePort();
                    return;
                }
                ConState = "断开";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}