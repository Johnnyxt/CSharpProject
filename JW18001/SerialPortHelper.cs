using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace JW18001
{
    internal class SerialPortHelper
    {
        private readonly byte[] rxBytes = new byte[1024];
        private readonly byte[] info = new byte[1024];
        private int timeCnt;
        private int gCmd;
        private static readonly SerialPort Sp = new SerialPort();

        public static readonly Protocol ProtocolPars = new Protocol();
        public static byte[] txBytes = new byte[1024];
        public static bool ts;
        public static int gCnt;
        public static readonly DispatcherTimer DTimer = new DispatcherTimer();

        //public delegate void ReadValue(byte[] data);

        //public delegate void GetConnect(byte[] data);

        //public delegate void ConfirmHardWare(byte[] data);

        //public delegate void GetRefData(byte[] data);

        //public delegate void GetSetPara(byte[] data);

        //public delegate void GetOpmWaveList(byte[] data);

        //public delegate void GetLightWaveList(byte[] data);

        //public delegate void SwitchOpmWave(byte[] data);

        //public delegate void SwitchLight(byte[] data);

        //public delegate void SetInstruTestMode(byte[] data);

        //public delegate void SetIlSampMode(byte[] data);

        //public delegate void SetPdlSampTime(byte[] data);

        //public delegate void StartTest(byte[] data);

        public static event SerialPortEventArgs.ReadValue ReadValueReceived;

        public static event SerialPortEventArgs.GetConnect ConnectReceived;

        public static event SerialPortEventArgs.ConfirmHardWare ConfirmHardWare;

        public static event SerialPortEventArgs.GetRefData GetRefDataReceived;

        public static event SerialPortEventArgs.GetSetPara GetSetParaReceived;

        public static event SerialPortEventArgs.GetOpmWaveList GetOpmWaveListReceived;

        public static event SerialPortEventArgs.GetLightWaveList GetLightWaveListReceived;

        public static event SerialPortEventArgs.SwitchOpmWave SwitchOpmWaveReceived;

        public static event SerialPortEventArgs.SwitchLight SwitchLightReceived;

        public static event SerialPortEventArgs.SetInstruTestMode SetInstruTestModeReceived;

        public static event SerialPortEventArgs.SetIlSampMode SetIlSampModeReceived;

        public static event SerialPortEventArgs.SetPdlSampTime SetPdlSampTimeReceived;

        public static event SerialPortEventArgs.StartTest StartTestReceived;

        public SerialPortHelper()
        {
            Sp.BaudRate = 115200;
            Sp.DataReceived += Sp_DataReceived;
            InitTimer();
        }

        private void InitTimer()
        {
            //DTimer.IsEnabled = true;
            DTimer.Interval = TimeSpan.FromMilliseconds(100);
            DTimer.Tick += DTimer_Tick;
        }

        public void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(10);
                int num = Sp.BytesToRead;
                Sp.Read(rxBytes, 0, num);
                for (int i = 0; i < num; i++)
                {
                    int rxovert = ProtocolPars.Protcol_Parser_P(rxBytes[i]);
                    if (rxovert != 0)
                    {
                        gCmd = rxovert;
                        ProtocolPars.Protocol_Convert(info);
                        com1_pro_decode(gCmd, info);
                        Array.Clear(rxBytes, 0, rxBytes.Length);
                    }
                }
            }
            catch (InvalidOperationException ioException)
            {
                MessageBox.Show(ioException.Message);
            }
        }

        private static void com1_pro_decode(int cmd, byte[] data)
        {
            switch (cmd)
            {
                case Cmd.Connect:

                    if (ConnectReceived != null)
                    {
                        ConnectReceived.Invoke(data);
                    }

                    break;

                case Cmd.SendTestStatus:
                    if (ConfirmHardWare != null)
                    {
                        ConfirmHardWare.Invoke(data);
                    }
                    break;

                case Cmd.GetValue:
                    if (ReadValueReceived != null)
                    {
                        ReadValueReceived.Invoke(data);
                    }
                    break;

                case Cmd.SetRef:
                    if (GetRefDataReceived != null)
                    {
                        GetRefDataReceived.Invoke(data);
                    }
                    break;

                case Cmd.GetSetPara:
                    if (GetSetParaReceived != null)
                    {
                        GetSetParaReceived.Invoke(data);
                    }

                    break;

                case Cmd.GetOpmWaveList:
                    if (GetOpmWaveListReceived != null)
                    {
                        GetOpmWaveListReceived.Invoke(data);
                    }
                    break;

                case Cmd.GetLightWaveList:
                    if (GetLightWaveListReceived != null)
                    {
                        GetLightWaveListReceived.Invoke(data);
                    }
                    break;

                case Cmd.SwitchOpmWave:
                    if (SwitchOpmWaveReceived != null)
                    {
                        SwitchOpmWaveReceived.Invoke(data);
                    }
                    break;

                case Cmd.SwitchLightSource:
                    if (SwitchLightReceived != null)
                    {
                        SwitchLightReceived.Invoke(data);
                    }
                    break;

                case Cmd.SetPdlSampTime:
                    if (SetPdlSampTimeReceived != null)
                    {
                        SetPdlSampTimeReceived.Invoke(data);
                    }
                    break;

                case Cmd.SetIlSampMode:
                    if (SetIlSampModeReceived != null)
                    {
                        SetIlSampModeReceived.Invoke(data);
                    }
                    break;

                case Cmd.SetTestMode:
                    if (SetInstruTestModeReceived != null)
                    {
                        SetInstruTestModeReceived.Invoke(data);
                    }
                    break;
            }
        }

        private void DTimer_Tick(object sender, EventArgs e)
        {
            if (ts)
            {
                SerialPortWrite(txBytes, gCnt);
                Array.Clear(txBytes, 0, txBytes.Length);
                timeCnt = 0;
                ts = false;
            }
            else
            {
                timeCnt++;
                if (timeCnt >= 3)
                {
                    byte[] data = new byte[256];
                    data[0] = (byte)Person.Channel;
                    gCnt = ProtocolPars.Protocol_wr(txBytes, Cmd.GetValue, data, 1);
                    SerialPortWrite(txBytes, gCnt);
                    Array.Clear(txBytes, 0, txBytes.Length);
                    timeCnt = 0;
                }
            }
        }

        public int Baud
        {
            get { return Sp.BaudRate; }
            set { Sp.BaudRate = value; }
        }

        public string PortName
        {
            get { return Sp.PortName; }
            set
            {
                Sp.PortName = value;
            }
        }

        public bool IsOpen { get; set; }

        public void SerialPortWrite(byte[] data, int cnt)
        {
            if (!Sp.IsOpen) return;
            try
            {
                Sp.Write(data, 0, cnt);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (Sp.IsOpen)
            {
                Sp.Close();
            }

            IsOpen = false;
        }

        public void OpenPort()
        {
            if (Sp.IsOpen)
            {
                Sp.Close();
            }
            try
            {
                Sp.Open();
            }
            catch (Exception)
            {
                IsOpen = false;
            }
            IsOpen = true;
        }
    }
}