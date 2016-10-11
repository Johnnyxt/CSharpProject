using System;

namespace JW18001
{
    internal class Protocol
    {
        private enum Protcol_Parser
        {
            T_Check_SOP = 0,
            T_Check_ID,
            T_Check_Length,
            T_Check_Cmd,
            T_Check_Data,
            T_Check_CheckSum,
            T_Check_EOP,
            T_SUCCESS,
        };

        private const int MAX_SENDBUFF = 255;
        private const byte SOP = 0x3c;
        public byte Id = 0x81;
        private const Byte EOP = 0x3e;
        private volatile Byte Snd_Len = 0;
        private volatile Byte[] Cmd;
        private volatile Byte[] txdata = new Byte[MAX_SENDBUFF];
        private volatile Byte[] pdata = new Byte[MAX_SENDBUFF];

        private const Byte MAXSIZE = 255;
        private const Byte MINSIZE = 5;
        private volatile Byte DataIndex;
        private volatile byte cmdIndex;
        private volatile Byte CheckSum;
        private volatile Byte P_DataLen = 0;
        private volatile byte[] P_Cmd = new byte[2];

        private Protcol_Parser pp = Protcol_Parser.T_Check_SOP;

        private Byte Calculation_Sum_Check(Byte[] data, int num)
        {
            Byte tmp = 0;

            for (int i = 0; i < num; i++)
            {
                tmp += data[i];
            }

            tmp = (Byte)(~tmp + 0x01);

            return tmp;
        }

        private Protcol_Parser Check_SOP(Byte input)
        {
            if (input == SOP)
            {
                CheckSum = input;
                P_DataLen = 0;
                DataIndex = 0;
                cmdIndex = 0;
                return Protcol_Parser.T_Check_ID;
            }

            return Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_ID(Byte input)
        {
            if (input == Id)
            {
                return Protcol_Parser.T_Check_Length;
            }

            return Protcol_Parser.T_Check_ID;
        }

        private Protcol_Parser Check_Length(byte input)
        {
            if ((input >= MINSIZE))
            {
                P_DataLen = (Byte)(input - MINSIZE);
                return Protcol_Parser.T_Check_Cmd;
            }

            return Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_Cmd(Byte input)
        {
            P_Cmd[cmdIndex++] = input;

            if (cmdIndex != 2)
            {
                return Protcol_Parser.T_Check_Cmd;
            }
            if (P_DataLen > 0)
            {
                return Protcol_Parser.T_Check_Data;
            }
            return Protcol_Parser.T_Check_CheckSum;
        }

        private Protcol_Parser Check_Data(Byte input)
        {
            pdata[DataIndex++] = input;
            if (DataIndex != P_DataLen)
            {
                return Protcol_Parser.T_Check_Data;
            }

            return Protcol_Parser.T_Check_CheckSum;
        }

        private Protcol_Parser Check_CheckSum()
        {
            return (CheckSum == 0) ? Protcol_Parser.T_Check_EOP : Protcol_Parser.T_Check_SOP;
        }

        private Protcol_Parser Check_EOP(Byte input)
        {
            return (input == EOP) ? Protcol_Parser.T_SUCCESS : Protcol_Parser.T_Check_SOP;
        }

        public int Protcol_Parser_P(Byte input)
        {
            CheckSum += input;

            switch (pp)
            {
                case Protcol_Parser.T_Check_SOP:
                    pp = Check_SOP(input);
                    break;

                case Protcol_Parser.T_Check_ID:
                    pp = Check_ID(input);
                    break;

                case Protcol_Parser.T_Check_Length:
                    pp = Check_Length(input);
                    break;

                case Protcol_Parser.T_Check_Cmd:
                    pp = Check_Cmd(input);
                    break;

                case Protcol_Parser.T_Check_Data:
                    pp = Check_Data(input);
                    break;

                case Protcol_Parser.T_Check_CheckSum:
                    pp = Check_CheckSum();
                    break;

                case Protcol_Parser.T_Check_EOP:
                    pp = Check_EOP(input);
                    if (pp == Protcol_Parser.T_SUCCESS)
                    {
                        pp = Protcol_Parser.T_Check_SOP;

                        int retCmd = (Cmd[0] << 8) + Cmd[1];
                        return retCmd;
                    }
                    break;

                default:
                    pp = Protcol_Parser.T_Check_SOP;
                    break;
            }
            return 0;
        }

        public void Cmd_Action(byte[] cmd)
        {
            txdata[0] = SOP;
            txdata[1] = Id;
            Array.Copy(cmd, 0, txdata, 3, 2);
            Snd_Len += 5;
            txdata[2] = Snd_Len;
            txdata[Snd_Len] = Calculation_Sum_Check(txdata, Snd_Len);
            Snd_Len++;
            txdata[Snd_Len++] = EOP;
        }

        public void Cmd_Action(byte cmd)
        {
            txdata[0] = SOP;
            txdata[1] = Id;
            txdata[3] = 0x01;
            txdata[4] = cmd;
            Snd_Len += 5;
            txdata[2] = Snd_Len;
            txdata[Snd_Len] = Calculation_Sum_Check(txdata, Snd_Len);
            Snd_Len++;
            txdata[Snd_Len++] = EOP;
        }

        public int Protocol_wr(byte[] tx, ushort cmd)
        {
            Cmd = BitConverter.GetBytes(cmd);
            Array.Reverse(Cmd);
            Cmd_Action(Cmd);
            int len = Snd_Len;

            for (int i = 0; i < len; i++)
            {
                tx[i] = txdata[i];
            }
            Snd_Len = 0;

            return len;
        }

        public int Protocol_wr(byte[] tx, ushort cmd, byte[] data, int cnt)
        {
            Cmd = BitConverter.GetBytes(cmd);
            Array.Reverse(Cmd);
            Snd_Len = 0;

            for (int i = 0; i < cnt; i++)
            {
                txdata[5 + Snd_Len++] = data[i];
            }

            Cmd_Action(Cmd);
            int len = Snd_Len;

            for (int i = 0; i < len; i++)
            {
                tx[i] = txdata[i];
            }
            Snd_Len = 0;

            return len;
        }

        public int Protocol_Convert(byte[] dst)
        {
            for (int i = 0; i < DataIndex; i++)
            {
                dst[i] = pdata[i];
            }

            return DataIndex;
        }
    }
}