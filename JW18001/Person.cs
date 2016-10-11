using System;
using System.Collections.Generic;
using System.Windows;

namespace JW18001
{
    internal class Person
    {
        internal struct DataBase
        {
            public string ServerName { get; set; } //服务器名称
            public string DbLoginName { get; set; } //登录名
            public string DbLoginPsd { get; set; } //登录密码
            public string DbName { get; set; } //数据库名称
        }

        public static List<float> IlUpperThre = new List<float>();
        public static List<float> IlLowerThre = new List<float>();
        public static List<float> PdlUpperThre = new List<float>();
        public static List<float> PdlLowerThre = new List<float>();
        public static List<float> RlUpperThre = new List<float>();
        public static List<float> RlLowerThre = new List<float>();

        public static int Channel = 32;
        public static string DeviceName = "JW18001";
        public static SqlHelper SqlHelper;
        public static SerialPortHelper SpHelper = new SerialPortHelper();
        public static bool IsBasicInfoViewEnable;
        public static bool IsUserSetViewEnable;
        public static string[] TestWave = { "1310nm", "1490nm", "1550nm", "1625nm" };

        public static string Vision = string.Format("{0} {1}.{2}.{3}", DeviceName, DateTime.Now.Year, DateTime.Now.Month,
            DateTime.Now.Day);

        public static Dictionary<int, string> DicWaveList = new Dictionary<int, string>
        {
            {0x01, "850nm"},
            {0x02, "1300nm"},
            {0x03, "1310nm"},
            {0x04, "1490nm"},
            {0x05, "1550nm"},
            {0x06, "1625nm"},
            {0x07, "980nm"},
            {0x08, "1270nm"},
            {0x09, "1650nm"}
        };

        public static string[] StrUserItem = { "测试日期", "测试时间", "SN号", "子SN号", "判定", "操作员", "工号" };
        public static bool IsThreadFlag = true;

        public static void MyMessageBox(string strText)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageBox.Show(strText);
            }));
        }

        public static double[,] CabValue = {
            // {9.56, 6.22, 4.9, 4.72, 1.28, 4.51},
            //{9.67, 6.4, 5.25, 6.05, 1.64, -9.68},
            //{9.48, 6.29, 4.93, 6.79, 1.37, 4.53},
            //{9.38, 5.93, 4.54, 7.28, 0.91, 4.2},
            //{9.89, 6.31, 4.98, 8.79, 1.26, 4.58},
            //{9.98, 6.12, 4.94, 9.74, 1.28, 4.57},
            //{9.37, 8.09, 4.87, 10.7, 1.28, 4.85},
            //{9.44, 8.49, 5.16, 12.03, 1.63, -0.33},

            //{6.63, 2.4, 2.32, 7.56, -3.83, -0.39},
            //{6.67, 2.19, 2.31, 8.55, -3.85, -0.31},
            //{6.59, 2.26, 2.34, 9.53, -3.82, -0.39},
            //{6.58, 2.28, 2.34, 10.54, -3.89, -0.26},
            //{7, 2.34, 2.14, 11.65, -3.76, -0.24},
            //{6.78, 2.29, 2.3, 12.62, -3.82, -0.34},
            //{6.75, 2.25, 2.33, 13.53, -3.84, -0.35},
            //{6.63, 2.2, 2.35, 14.52, -3.89, -0.39},

            {10.09,5.9, 5.4,  5.41, 5.2, 6.24},
            {9.97, 6.25, 5.55, 5.46, 5.28, 6.18},
            {9.87, 6, 5.5, 5.11, 4.95,6},
            {9.73,5.55, 4.92, 4.88, 4.74, 5.7},
            {10.11, 6, 5.53, 5.55, 5.33, 6.27},
            {10.07, 6.03, 5.29, 5.23, 5.07, 6},
            {9.63, 6.36, 5.45, 5.34, 5.22, 6.14},
            {9.63, 6.15, 5.11, 5.1, 4.92, 5.9},
            {6.77, 0.25, 0.08, -0.17, -0.19, 0.43},
            {6.84, 0.3, 0, -0.3, -0.45, 0.13},
            {6.72,0.42, -0.22, -0.41, -0.48, 0.11},
            {6.67, 0.68, 0, -0.39, -0.44, 0.12},
            {6.94, 0.7, -1, -0.24, -0.35, 0.15},
            {6.85, 0.67, 0, -0.31, -0.34, 0.27},
            {6.8, 0.68, 0, -0.37, -0.42, 0.2},
            {6.66, 0.66, 0, -0.32, -0.37, 0.21}
        };
    }
}