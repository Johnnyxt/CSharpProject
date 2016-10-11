using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JW18001
{
    internal class Cmd
    {
        /// <summary>
        /// 	联机指令
        /// </summary>
        public const int Connect = 0x0031;

        /// <summary>
        ///	下发所需测试状态
        /// </summary>
        public const int SendTestStatus = 0x0041;

        /// <summary>
        /// 切换光功率计波长
        /// </summary>
        public const int SwitchOpmWave = 0x0440;

        /// <summary>
        ///设置仪表测试模式
        /// </summary>
        public const int SetTestMode = 0x0448;

        /// <summary>
        ///读取仪表测试值和功率值
        /// </summary>
        public const int GetValue = 0x0442;

        /// <summary>
        ///对光功率计当前波长下进行REF操作
        /// </summary>
        public const int SetRef = 0x0444;

        /// <summary>
        /// 设置光功率计IL采样模式
        /// </summary>
        public const int SetIlSampMode = 0x0446;

        /// <summary>
        ///设置光功率平均时间
        /// </summary>
        public const int SetOpmAverTime = 0x044A;

        /// <summary>
        ///读取下位机光功率计部分设置参数
        /// </summary>
        public const int GetSetPara = 0x044E;

        /// <summary>
        ///获取光功率计波长列表
        /// </summary>
        public const int GetOpmWaveList = 0x0452;

        /// <summary>
        ///启动测试
        /// </summary>
        public const int StartTest = 0x0454;

        /// <summary>
        ///设置PDL采样时间
        /// </summary>
        public const int SetPdlSampTime = 0x0502;

        /// <summary>
        ///切换光开关（光源）
        /// </summary>
        public const int SwitchLightSource = 0x0600;

        /// <summary>
        ///上传光源波长列表
        /// </summary>
        public const int GetLightWaveList = 0x0652;

        /// <summary>
        ///设置使用波长（校准）
        /// </summary>
        public const int SetUsingWave = 0x00c1;
    }
}