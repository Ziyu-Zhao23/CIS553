using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixColumnAES
{
    public static class RijndaelField
    {

        static void Main(string[] args)
        {
            Console.WriteLine(Multiply((byte)0x30, (byte)0x82));
            //byte[] list0 = new byte[] { 0x63, 0x30, 0x63, 0x30 };
            byte[] list1 = new byte[] { 0xE5, 0xE4, 0x59, 0xEB };
            byte[] list2 = new byte[] { 0x34, 0x68, 0x84, 0x47 };
            byte[] list3 = new byte[] { 0x62, 0x8A, 0x59, 0x24 };
            byte[] list4 = new byte[] { 0x01, 0xF3, 0xB5, 0xC4 };


            //Console.WriteLine(MixColumn(list0));
            Console.WriteLine(MixColumn(list1));
            Console.WriteLine(MixColumn(list2));
            Console.WriteLine(MixColumn(list3));
            Console.WriteLine(MixColumn(list4));
            Console.Read();
        }
        public const byte MUL_CONSTANT = 0x1B;

        public static string MixColumn(byte[] vector)
        {
            string result;
            result = $"{Multiply(0x02, vector[0])} {Multiply(0x03, vector[1])}  {Multiply(0x01, vector[2])} {Multiply(0x01, vector[3])}" +
                    $"\n{Multiply(0x01, vector[0])} {Multiply(0x02, vector[1])} {Multiply(0x03, vector[2])} {Multiply(0x01, vector[3])}" +
                    $"\n{Multiply(0x01, vector[0])} {Multiply(0x01, vector[1])} {Multiply(0x02, vector[2])} {Multiply(0x03, vector[3])}" +
                    $"\n{Multiply(0x03, vector[0])} {Multiply(0x01, vector[1])} {Multiply(0x01, vector[2])} {Multiply(0x02, vector[3])}";
                return result;
        }


        /*public static byte Multiply(byte multiplier, int multiplicand)
        {
            return Multiply(multiplier, (byte)multiplicand);
        }*/

        public static string Multiply(byte multiplier, byte multiplicand)
        {
            byte result;

            result = ((multiplicand & 1) == 1) ? multiplier : (byte)0;

            for (int position = 1; position < 8; position++)
            {
                bool relevantBit = ((multiplicand >> position) & 1) == 1;

                if (relevantBit)
                {
                    byte intermediateByte = multiplier;

                    for (int iteration = 0; iteration < position; iteration++)
                    {
                        bool msb = (((intermediateByte >> 7) & 1) == 1);

                        intermediateByte = (byte)((intermediateByte << 1) & 0xFF);

                        if (msb)
                        {
                            intermediateByte ^= MUL_CONSTANT;
                        }
                    }

                    result ^= intermediateByte;
                }
                else
                {
                    continue;
                }
            }

            string hexValue = result.ToString("X");

            return hexValue;
        }
    }
    
}

