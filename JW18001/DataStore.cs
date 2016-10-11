using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW18001
{
    internal class DataStore
    {
        public string SerialNumber { get; set; }
        public string SubSerialNumber { get; set; }
        public string TestChannel { get; set; }
        public string TestResult { get; set; }
        public List<int> TestChannelNumber { get; set; }
        public List<string> TestWave { get; set; }
        public List<string> TestIl { get; set; }
        public List<string> TestPdl { get; set; }
        public List<string> TestRl { get; set; }
        public bool TestWave1Enable { get; set; }
        public bool TestWave2Enable { get; set; }
        public bool TestWave3Enable { get; set; }
        public bool TestWave4Enable { get; set; }
        public bool TestIlEnable { get; set; }
        public bool TestPdlEnable { get; set; }
        public bool TestRlEnable { get; set; }
    }
}