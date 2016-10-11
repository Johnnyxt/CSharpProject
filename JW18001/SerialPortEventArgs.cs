using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW18001
{
    internal class SerialPortEventArgs
    {
        public delegate void ReadValue(byte[] data);

        public delegate void GetConnect(byte[] data);

        public delegate void ConfirmHardWare(byte[] data);

        public delegate void GetRefData(byte[] data);

        public delegate void GetSetPara(byte[] data);

        public delegate void GetOpmWaveList(byte[] data);

        public delegate void GetLightWaveList(byte[] data);

        public delegate void SwitchOpmWave(byte[] data);

        public delegate void SwitchLight(byte[] data);

        public delegate void SetInstruTestMode(byte[] data);

        public delegate void SetIlSampMode(byte[] data);

        public delegate void SetPdlSampTime(byte[] data);

        public delegate void StartTest(byte[] data);
    }
}