using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SHA_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sha3 = new SHA3(256);
            byte[] input = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
            foreach (byte b in input)
            {
                Console.Write($"{b} "); // Convert byte to hexadecimal string and print
            }
            Console.WriteLine();
            byte[] hash = sha3.ComputeHash(input);
            foreach (byte h in hash)
            {
                Console.Write($"{h:X2} "); // Convert byte to hexadecimal string and print
            }
            Console.WriteLine();

            string hexHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
            Console.WriteLine(hexHash);

            Console.Read();
        }
    }

    public class SHA3
    {
        private readonly int _outputLength;
        private readonly int _rate;
        private readonly int _capacity;
        private ulong[,] state = new ulong[5, 5];

        public SHA3(int outputLength)
        {
            _outputLength = outputLength;
            _rate = 1600 - outputLength * 2;
            _capacity = outputLength * 2;
        }


        public byte[] ComputeHash(byte[] message)
        {
            int blockSize = _rate / 8;
            byte[] paddedMessage = message.Concat(Pad(blockSize, message.Length)).ToArray();
            

            for (int i = 0; i < paddedMessage.Length; i += blockSize)
            {
                byte[] block = paddedMessage.Skip(i).Take(blockSize).ToArray();
                StringToState(block);

                KeccakF1600(state);
            }

            byte[] output = StateToString().Take(_outputLength / 8).ToArray();
            return output;
        }

        // String to state conversion
        private void StringToState(byte[] message)
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    state[x, y] = BitConverter.ToUInt64(message, (5 * y + x));
                }
            }
        }

        //// State to string conversion
        private byte[] StateToString()
        {
            byte[] output = new byte[256];
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Array.Copy(BitConverter.GetBytes(state[x, y]), 0, output, 8 * (5 * y + x), 8);
                }
            }
            return output;
        }

        private byte[] Pad(int x,int m)
        {
            int j = (2*x - m - 2) % x;
            byte[] padding = new byte[j+2];
            padding[0] = 1;
            padding[j] = 1;
            return padding;
        }


        //KECCAK-p[b, nr],Rnd(A, ir) = ι(χ(π(ρ(θ(A)))), ir).
        private void KeccakF1600(ulong[,] state)
        {
            int numberOfRounds = 24;
            for (int round = 0; round < numberOfRounds; round++)
            {
                KeccakPermutations.Theta(state);
                KeccakPermutations.Rho(state);
                KeccakPermutations.Pi(state);
                KeccakPermutations.Chi(state);
                KeccakPermutations.Iota(state, round);
            }
        }
    }

    public class KeccakPermutations
    {
        private const int StateWidth = 5; //
        private const int StateHeight = 5;//
        private const int LaneSize = 64; //z

        //Specification of θ
        public static void Theta(ulong[,] state)
        {
            ulong[] C = new ulong[StateWidth];
            ulong[] D = new ulong[StateWidth];

            //C[x,z]=A[x, 0,z] ⊕ A[x, 1,z] ⊕ A[x, 2,z] ⊕ A[x, 3,z] ⊕ A[x, 4,z].
            for (int x = 0; x < StateWidth; x++)
            {
                C[x] = state[x, 0] ^ state[x, 1] ^ state[x, 2] ^ state[x, 3] ^ state[x, 4];
            }
            //D[x,z]=C[(x+4) mod 5, z] ⊕ C[(x+1) mod 5, (z –1) mod w].
            //RotateLeft help to move element at z-axis
            for (int x = 0; x < StateWidth; x++)
            {
                D[x] = C[(x + 4) % StateWidth] ^ RotateLeft(C[(x + 1) % StateWidth], 1);
            }
            //A′[x, y,z] = A[x, y,z] ⊕ D[x,z].
            for (int x = 0; x < StateWidth; x++)
            {
                for (int y = 0; y < StateHeight; y++)
                {
                    state[x, y] ^= D[x];
                }
            }
        }

        //Specification of ρ
        public static void Rho(ulong[,] state)
        {
            int x = 1;
            int y = 0;
            ulong current = state[x, y];

            for (int t = 0; t < 24; t++)
            {
                int offset = ((t + 1) * (t + 2) / 2) % LaneSize;
                state[x, y] = RotateLeft(current, offset);

                x = y;
                y = (2 * x + 3 * y) % StateWidth;      
            }
        }

        //Specification of π
        public static void Pi(ulong[,] state)
        {
            ulong[,] newState = new ulong[StateWidth, StateHeight];

            for (int x = 0; x < StateWidth; x++)
            {
                for (int y = 0; y < StateHeight; y++)
                {
                    newState[x, y] = state[(x + 3 * y) % StateWidth, x];
                }
            }

            state = newState;
            //Array.Copy(newState, state, StateWidth * StateHeight);
        }

        //Specification of χ
        public static void Chi(ulong[,] state)
        {
            ulong[,] newState = new ulong[StateWidth, StateHeight];

            for (int x = 0; x < StateWidth; x++)
            {
                for (int y = 0; y < StateHeight; y++)
                {
                    //~ = XOR 1
                    newState[x, y] = state[x, y] ^ ((~state[(x + 1) % StateWidth, y]) & state[(x + 2) % StateWidth, y]);
                }
            }

            Array.Copy(newState, state, StateWidth * StateHeight);
        }

        //rc
        private static ulong rc(int t)
        {
            if (t % 255 == 0) return 1;

            ulong R = 0b10000000;
            for (int i = 1; i <= t % 255; i++)
            {
                // concatenating 0 to R
                R <<=  1;
                // Apply XOR on R[0], R[4], R[5], and R[6] with R[8]
                ulong R0 = (R >> 8) & 1;
                ulong R4 = (R >> 4) & 1;
                ulong R5 = (R >> 3) & 1;
                ulong R6 = (R >> 2) & 1;
                ulong R8 = (R >> 0) & 1;

                R ^= R8 << 8;
                R ^= R8 << 4;
                R ^= R8 << 3;
                R ^= R8 << 2;
                R >>= 1; // Trunc8[R]
            }

            return (R>>7);
        }

        //Specification of ι
        public static void Iota(ulong[,] state, int roundIndex)
        {
            ulong RC = 0;

            for (int j = 0; j <= 6; j++)
            {
                RC ^= rc(j + 7 * roundIndex) << ((1 << j) - 1);
            }

            state[0, 0] ^= RC;
        }

        private static ulong RotateLeft(ulong value, int count)
        {
            return (value << count) | (value >> (64 - count));
        }
    }


                /*public void checkSHA3(string message)
                {

                    if (message == null) return;

                    byte[] bytes = null;
                    byte[] r = null;
                    SHA3.SHA3Unmanaged hash = null;
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();


                    hash = new SHA3Unmanaged(224);
                    bytes = encoding.GetBytes(message);
                    r = hash.ComputeHash(bytes);
                    hash1 = Global.ByteToString(r);

                    hash = new SHA3Unmanaged(256);
                    bytes = encoding.GetBytes(message);
                    r = hash.ComputeHash(bytes);
                    hash2 = Global.ByteToString(r);

                    hash = new SHA3Unmanaged(384);
                    bytes = encoding.GetBytes(message);
                    r = hash.ComputeHash(bytes);
                    hash3 = Global.ByteToString(r);

                    hash = new SHA3Unmanaged(512);
                    bytes = encoding.GetBytes(message);
                    r = hash.ComputeHash(bytes);
                    hash4 = Global.ByteToString(r);




                }*/
            
}
