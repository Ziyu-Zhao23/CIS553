using System;

namespace RijndaelField
{
	public static class RijndaelField
	{
		static void Main(string[] args)
		{
			Console.WriteLine(Multiply((byte)0x30, (byte)0x82));
		}
		public const byte MUL_CONSTANT = 0x1B;



		public static byte Multiply(byte multiplier, int multiplicand)
		{
			return Multiply(multiplier, (byte)multiplicand);
		}

		public static byte Multiply(byte multiplier, byte multiplicand)
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

			return result;
		}
	}
}