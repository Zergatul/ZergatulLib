using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    // https://info.isl.ntt.co.jp/crypt/eng/camellia/dl/01espec.pdf
    // https://tools.ietf.org/html/rfc3713
    public abstract class Camellia : AbstractBlockCipher
    {
        public override int BlockSize => 16;
        public override int KeySize => Nk / 8;

        private const ulong Σ1 = 0xA09E667F3BCC908B;
        private const ulong Σ2 = 0xB67AE8584CAA73B2;
        private const ulong Σ3 = 0xC6EF372FE94F82BE;
        private const ulong Σ4 = 0x54FF53A5F1D36F1C;
        private const ulong Σ5 = 0x10E527FADE682D1D;
        private const ulong Σ6 = 0xB05688C2B3E6C1FD;

        private static readonly byte[] SBox1 = new byte[]
        {
            112, 130,  44, 236, 179,  39, 192, 229, 228, 133,  87,  53, 234,  12, 174,  65,
             35, 239, 107, 147,  69,  25, 165,  33, 237,  14,  79,  78,  29, 101, 146, 189,
            134, 184, 175, 143, 124, 235,  31, 206,  62,  48, 220,  95,  94, 197,  11,  26,
            166, 225,  57, 202, 213,  71,  93,  61, 217,   1,  90, 214,  81,  86, 108,  77,
            139,  13, 154, 102, 251, 204, 176,  45, 116,  18,  43,  32, 240, 177, 132, 153,
            223,  76, 203, 194,  52, 126, 118,   5, 109, 183, 169,  49, 209,  23,   4, 215,
             20,  88,  58,  97, 222,  27,  17,  28,  50,  15, 156,  22,  83,  24, 242,  34,
            254,  68, 207, 178, 195, 181, 122, 145,  36,   8, 232, 168,  96, 252, 105,  80,
            170, 208, 160, 125, 161, 137,  98, 151,  84,  91,  30, 149, 224, 255, 100, 210,
             16, 196,   0,  72, 163, 247, 117, 219, 138,   3, 230, 218,   9,  63, 221, 148,
            135,  92, 131,   2, 205,  74, 144,  51, 115, 103, 246, 243, 157, 127, 191, 226,
             82, 155, 216,  38, 200,  55, 198,  59, 129, 150, 111,  75,  19, 190,  99,  46,
            233, 121, 167, 140, 159, 110, 188, 142,  41, 245, 249, 182,  47, 253, 180,  89,
            120, 152,   6, 106, 231,  70, 113, 186, 212,  37, 171,  66, 136, 162, 141, 250,
            114,   7, 185,  85, 248, 238, 172,  10,  54,  73,  42, 104,  60,  56, 241, 164,
             64,  40, 211, 123, 187, 201,  67, 193,  21, 227, 173, 244, 119, 199, 128, 158
        };

        private static readonly byte[] SBox2 = new byte[]
        {
            224,   5,  88, 217, 103,  78, 129, 203, 201,  11, 174, 106, 213,  24,  93, 130,
             70, 223, 214,  39, 138,  50,  75,  66, 219,  28, 158, 156,  58, 202,  37, 123,
             13, 113,  95,  31, 248, 215,  62, 157, 124,  96, 185, 190, 188, 139,  22,  52,
             77, 195, 114, 149, 171, 142, 186, 122, 179,   2, 180, 173, 162, 172, 216, 154,
             23,  26,  53, 204, 247, 153,  97,  90, 232,  36,  86,  64, 225,  99,   9,  51,
            191, 152, 151, 133, 104, 252, 236,  10, 218, 111,  83,  98, 163,  46,   8, 175,
             40, 176, 116, 194, 189,  54,  34,  56, 100,  30,  57,  44, 166,  48, 229,  68,
            253, 136, 159, 101, 135, 107, 244,  35,  72,  16, 209,  81, 192, 249, 210, 160,
             85, 161,  65, 250,  67,  19, 196,  47, 168, 182,  60,  43, 193, 255, 200, 165,
             32, 137,   0, 144,  71, 239, 234, 183,  21,   6, 205, 181,  18, 126, 187,  41,
             15, 184,   7,   4, 155, 148,  33, 102, 230, 206, 237, 231,  59, 254, 127, 197,
            164,  55, 177,  76, 145, 110, 141, 118,   3,  45, 222, 150,  38, 125, 198,  92,
            211, 242,  79,  25,  63, 220, 121,  29,  82, 235, 243, 109,  94, 251, 105, 178,
            240,  49,  12, 212, 207, 140, 226, 117, 169,  74,  87, 132,  17,  69,  27, 245,
            228,  14, 115, 170, 241, 221,  89,  20, 108, 146,  84, 208, 120, 112, 227,  73,
            128,  80, 167, 246, 119, 147, 134, 131,  42, 199,  91, 233, 238, 143,   1,  61
        };

        private static readonly byte[] SBox3 = new byte[]
        {
             56,  65,  22, 118, 217, 147,  96, 242, 114, 194, 171, 154, 117,   6,  87, 160,
            145, 247, 181, 201, 162, 140, 210, 144, 246,   7, 167,  39, 142, 178,  73, 222,
             67,  92, 215, 199,  62, 245, 143, 103,  31,  24, 110, 175,  47, 226, 133,  13,
             83, 240, 156, 101, 234, 163, 174, 158, 236, 128,  45, 107, 168,  43,  54, 166,
            197, 134,  77,  51, 253, 102,  88, 150,  58,   9, 149,  16, 120, 216,  66, 204,
            239,  38, 229,  97,  26,  63,  59, 130, 182, 219, 212, 152, 232, 139,   2, 235,
             10,  44,  29, 176, 111, 141, 136,  14,  25, 135,  78,  11, 169,  12, 121,  17,
            127,  34, 231,  89, 225, 218,  61, 200,  18,   4, 116,  84,  48, 126, 180,  40,
             85, 104,  80, 190, 208, 196,  49, 203,  42, 173,  15, 202, 112, 255,  50, 105,
              8,  98,   0,  36, 209, 251, 186, 237,  69, 129, 115, 109, 132, 159, 238,  74,
            195,  46, 193,   1, 230,  37,  72, 153, 185, 179, 123, 249, 206, 191, 223, 113,
             41, 205, 108,  19, 100, 155,  99, 157, 192,  75, 183, 165, 137,  95, 177,  23,
            244, 188, 211,  70, 207,  55,  94,  71, 148, 250, 252,  91, 151, 254,  90, 172,
             60,  76,   3,  53, 243,  35, 184,  93, 106, 146, 213,  33,  68,  81, 198, 125,
             57, 131, 220, 170, 124, 119,  86,   5,  27, 164,  21,  52,  30,  28, 248,  82,
             32,  20, 233, 189, 221, 228, 161, 224, 138, 241, 214, 122, 187, 227,  64,  79
        };

        private static readonly byte[] SBox4 = new byte[]
        {
            112,  44, 179, 192, 228,  87, 234, 174,  35, 107,  69, 165, 237,  79,  29, 146,
            134, 175, 124,  31,  62, 220,  94,  11, 166,  57, 213,  93, 217,  90,  81, 108,
            139, 154, 251, 176, 116,  43, 240, 132, 223, 203,  52, 118, 109, 169, 209,   4,
             20,  58, 222,  17,  50, 156,  83, 242, 254, 207, 195, 122,  36, 232,  96, 105,
            170, 160, 161,  98,  84,  30, 224, 100,  16,   0, 163, 117, 138, 230,   9, 221,
            135, 131, 205, 144, 115, 246, 157, 191,  82, 216, 200, 198, 129, 111,  19,  99,
            233, 167, 159, 188,  41, 249,  47, 180, 120,   6, 231, 113, 212, 171, 136, 141,
            114, 185, 248, 172,  54,  42,  60, 241,  64, 211, 187,  67,  21, 173, 119, 128,
            130, 236,  39, 229, 133,  53,  12,  65, 239, 147,  25,  33,  14,  78, 101, 189,
            184, 143, 235, 206,  48,  95, 197,  26, 225, 202,  71,  61,   1, 214,  86,  77,
             13, 102, 204,  45,  18,  32, 177, 153,  76, 194, 126,   5, 183,  49,  23, 215,
             88,  97,  27,  28,  15,  22,  24,  34,  68, 178, 181, 145,   8, 168, 252,  80,
            208, 125, 137, 151,  91, 149, 255, 210, 196,  72, 247, 219,   3, 218,  63, 148,
             92,   2,  74,  51, 103, 243, 127, 226, 155,  38,  55,  59, 150,  75, 190,  46,
            121, 140, 110, 142, 245, 182, 253,  89, 152, 106,  70, 186,  37,  66, 162, 250,
              7,  85, 238,  10,  73, 104,  56, 164,  40, 123, 201, 193, 227, 244, 199, 158
        };

        int Nk, Nr;

        public Camellia(int Nk, int Nr)
        {
            this.Nk = Nk;
            this.Nr = Nr;
        }

        private static ulong F(ulong fin, ulong ke)
        {
            ulong x = fin ^ ke;
            byte t1 = (byte)(x >> 56);
            byte t2 = (byte)(x >> 48);
            byte t3 = (byte)(x >> 40);
            byte t4 = (byte)(x >> 32);
            byte t5 = (byte)(x >> 24);
            byte t6 = (byte)(x >> 16);
            byte t7 = (byte)(x >> 8);
            byte t8 = (byte)x;
            t1 = SBox1[t1];
            t2 = SBox2[t2];
            t3 = SBox3[t3];
            t4 = SBox4[t4];
            t5 = SBox2[t5];
            t6 = SBox3[t6];
            t7 = SBox4[t7];
            t8 = SBox1[t8];
            byte y1 = (byte)(t1 ^ t3 ^ t4 ^ t6 ^ t7 ^ t8);
            byte y2 = (byte)(t1 ^ t2 ^ t4 ^ t5 ^ t7 ^ t8);
            byte y3 = (byte)(t1 ^ t2 ^ t3 ^ t5 ^ t6 ^ t8);
            byte y4 = (byte)(t2 ^ t3 ^ t4 ^ t5 ^ t6 ^ t7);
            byte y5 = (byte)(t1 ^ t2 ^ t6 ^ t7 ^ t8);
            byte y6 = (byte)(t2 ^ t3 ^ t5 ^ t7 ^ t8);
            byte y7 = (byte)(t3 ^ t4 ^ t5 ^ t6 ^ t8);
            byte y8 = (byte)(t1 ^ t4 ^ t5 ^ t6 ^ t7);
            return
                ((ulong)y1 << 56) |
                ((ulong)y2 << 48) |
                ((ulong)y3 << 40) |
                ((ulong)y4 << 32) |
                ((ulong)y5 << 24) |
                ((ulong)y6 << 16) |
                ((ulong)y7 << 8) | y8;
        }

        private static ulong FL(ulong flin, ulong ke)
        {
            uint x1 = (uint)(flin >> 32);
            uint x2 = (uint)flin;
            uint k1 = (uint)(ke >> 32);
            uint k2 = (uint)ke;
            x2 ^= BitHelper.RotateLeft(x1 & k1, 1);
            x1 ^= x2 | k2;
            return ((ulong)x1 << 32) | x2;
        }

        private static ulong FLInv(ulong flin, ulong ke)
        {
            uint y1 = (uint)(flin >> 32);
            uint y2 = (uint)flin;
            uint k1 = (uint)(ke >> 32);
            uint k2 = (uint)ke;
            y1 ^= y2 | k2;
            y2 ^= BitHelper.RotateLeft(y1 & k1, 1);
            return ((ulong)y1 << 32) | y2;
        }

        private static void KeySchedule(byte[] key, int Nk, out ulong[] kw, out ulong[] k, out ulong[] ke)
        {
            byte[] KL, KR;

            switch (Nk)
            {
                case 128:
                    KL = key;
                    KR = new byte[16];
                    break;
                case 192:
                    KL = new byte[16];
                    KR = new byte[16];
                    Array.Copy(key, 0, KL, 0, 16);
                    Array.Copy(key, 16, KR, 0, 8);
                    for (int i = 8; i < 16; i++)
                        KR[i] = (byte)(~KR[i - 8]);
                    break;
                case 256:
                    KL = new byte[16];
                    KR = new byte[16];
                    Array.Copy(key, 0, KL, 0, 16);
                    Array.Copy(key, 16, KR, 0, 16);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            ulong KLl = BitHelper.ToUInt64(KL, 0, ByteOrder.BigEndian);
            ulong KLr = BitHelper.ToUInt64(KL, 8, ByteOrder.BigEndian);
            ulong KRl = BitHelper.ToUInt64(KR, 0, ByteOrder.BigEndian);
            ulong KRr = BitHelper.ToUInt64(KR, 8, ByteOrder.BigEndian);

            // D1 = (KL ^ KR) >> 64
            ulong D1 = KLl ^ KRl;

            // D2 = (KL ^ KR) & MASK64
            ulong D2 = KLr ^ KRr;

            D2 ^= F(D1, Σ1);
            D1 ^= F(D2, Σ2);
            D1 ^= KLl;
            D2 ^= KLr;
            D2 ^= F(D1, Σ3);
            D1 ^= F(D2, Σ4);

            // KA = (D1 << 64) | D2;
            ulong KAl = D1;
            ulong KAr = D2;

            kw = new ulong[4];
            k = new ulong[Nk == 128 ? 18 : 24];
            ke = new ulong[Nk == 128 ? 4 : 6];

            if (Nk == 128)
            {
                // kw1 = (KL <<<   0) >> 64;
                kw[0] = KLl;

                // kw2 = (KL <<<   0) & MASK64;
                kw[1] = KLr;

                // k1  = (KA <<<   0) >> 64;
                k[0] = KAl;

                // k2  = (KA <<<   0) & MASK64;
                k[1] = KAr;

                // k3  = (KL <<<  15) >> 64;
                k[2] = (KLl << 15) | (KLr >> 49);

                // k4  = (KL <<<  15) & MASK64;
                k[3] = (KLr << 15) | (KLl >> 49);

                // k5  = (KA <<<  15) >> 64;
                k[4] = (KAl << 15) | (KAr >> 49);

                // k6  = (KA <<<  15) & MASK64;
                k[5] = (KAr << 15) | (KAl >> 49);

                // ke1 = (KA <<<  30) >> 64;
                ke[0] = (KAl << 30) | (KAr >> 34);

                // ke2 = (KA <<<  30) & MASK64;
                ke[1] = (KAr << 30) | (KAl >> 34);

                // k7  = (KL <<<  45) >> 64;
                k[6] = (KLl << 45) | (KLr >> 19);

                // k8  = (KL <<<  45) & MASK64;
                k[7] = (KLr << 45) | (KLl >> 19);

                // k9  = (KA <<<  45) >> 64;
                k[8] = (KAl << 45) | (KAr >> 19);

                // k10 = (KL <<<  60) & MASK64;
                k[9] = (KLr << 60) | (KLl >> 4);

                // k11 = (KA <<<  60) >> 64;
                k[10] = (KAl << 60) | (KAr >> 4);

                // k12 = (KA <<<  60) & MASK64;
                k[11] = (KAr << 60) | (KAl >> 4);

                // ke3 = (KL <<<  77) >> 64;
                ke[2] = (KLr << 13) | (KLl >> 51);

                // ke4 = (KL <<<  77) & MASK64;
                ke[3] = (KLl << 13) | (KLr >> 51);

                // k13 = (KL <<<  94) >> 64;
                k[12] = (KLr << 30) | (KLl >> 34);

                // k14 = (KL <<<  94) & MASK64;
                k[13] = (KLl << 30) | (KLr >> 34);

                // k15 = (KA <<<  94) >> 64;
                k[14] = (KAr << 30) | (KAl >> 34);

                // k16 = (KA <<<  94) & MASK64;
                k[15] = (KAl << 30) | (KAr >> 34);

                // k17 = (KL <<< 111) >> 64;
                k[16] = (KLr << 47) | (KLl >> 17);

                // k18 = (KL <<< 111) & MASK64;
                k[17] = (KLl << 47) | (KLr >> 17);

                // kw3 = (KA <<< 111) >> 64;
                kw[2] = (KAr << 47) | (KAl >> 17);

                // kw4 = (KA <<< 111) & MASK64;
                kw[3] = (KAl << 47) | (KAr >> 17);
            }
            else
            {
                // D1 = (KA ^ KR) >> 64;
                D1 = KAl ^ KRl;

                // D2 = (KA ^ KR) & MASK64;
                D2 = KAr ^ KRr;

                // D2 = D2 ^ F(D1, Sigma5);
                D2 ^= F(D1, Σ5);

                // D1 = D1 ^ F(D2, Sigma6);
                D1 ^= F(D2, Σ6);

                // KB = (D1 << 64) | D2;
                ulong KBl = D1;
                ulong KBr = D2;

                // kw1 = (KL <<<   0) >> 64;
                kw[0] = KLl;

                // kw2 = (KL <<<   0) & MASK64;
                kw[1] = KLr;

                // k1  = (KB <<<   0) >> 64;
                k[0] = KBl;

                // k2  = (KB <<<   0) & MASK64;
                k[1] = KBr;

                // k3  = (KR <<<  15) >> 64;
                k[2] = (KRl << 15) | (KRr >> 49);

                // k4  = (KR <<<  15) & MASK64;
                k[3] = (KRr << 15) | (KRl >> 49);

                // k5  = (KA <<<  15) >> 64;
                k[4] = (KAl << 15) | (KAr >> 49);

                // k6  = (KA <<<  15) & MASK64;
                k[5] = (KAr << 15) | (KAl >> 49);

                // ke1 = (KR <<<  30) >> 64;
                ke[0] = (KRl << 30) | (KRr >> 34);

                // ke2 = (KR <<<  30) & MASK64;
                ke[1] = (KRr << 30) | (KRl >> 34);

                // k7  = (KB <<<  30) >> 64;
                k[6] = (KBl << 30) | (KBr >> 34);

                // k8  = (KB <<<  30) & MASK64;
                k[7] = (KBr << 30) | (KBl >> 34);

                // k9  = (KL <<<  45) >> 64;
                k[8] = (KLl << 45) | (KLr >> 19);

                // k10 = (KL <<<  45) & MASK64;
                k[9] = (KLr << 45) | (KLl >> 19);

                // k11 = (KA <<<  45) >> 64;
                k[10] = (KAl << 45) | (KAr >> 19);

                // k12 = (KA <<<  45) & MASK64;
                k[11] = (KAr << 45) | (KAl >> 19);

                // ke3 = (KL <<<  60) >> 64;
                ke[2] = (KLl << 60) | (KLr >> 4);

                // ke4 = (KL <<<  60) & MASK64;
                ke[3] = (KLr << 60) | (KLl >> 4);

                // k13 = (KR <<<  60) >> 64;
                k[12] = (KRl << 60) | (KRr >> 4);

                // k14 = (KR <<<  60) & MASK64;
                k[13] = (KRr << 60) | (KRl >> 4);

                // k15 = (KB <<<  60) >> 64;
                k[14] = (KBl << 60) | (KBr >> 4);

                // k16 = (KB <<<  60) & MASK64;
                k[15] = (KBr << 60) | (KBl >> 4);

                // k17 = (KL <<<  77) >> 64;
                k[16] = (KLr << 13) | (KLl >> 51);

                // k18 = (KL <<<  77) & MASK64;
                k[17] = (KLl << 13) | (KLr >> 51);

                // ke5 = (KA <<<  77) >> 64;
                ke[4] = (KAr << 13) | (KAl >> 51);

                // ke6 = (KA <<<  77) & MASK64;
                ke[5] = (KAl << 13) | (KAr >> 51);

                // k19 = (KR <<<  94) >> 64;
                k[18] = (KRr << 30) | (KRl >> 34);

                // k20 = (KR <<<  94) & MASK64;
                k[19] = (KRl << 30) | (KRr >> 34);

                // k21 = (KA <<<  94) >> 64;
                k[20] = (KAr << 30) | (KAl >> 34);

                // k22 = (KA <<<  94) & MASK64;
                k[21] = (KAl << 30) | (KAr >> 34);

                // k23 = (KL <<< 111) >> 64;
                k[22] = (KLr << 47) | (KLl >> 17);

                // k24 = (KL <<< 111) & MASK64;
                k[23] = (KLl << 47) | (KLr >> 17);

                // kw3 = (KB <<< 111) >> 64;
                kw[2] = (KBr << 47) | (KBl >> 17);

                // kw4 = (KB <<< 111) & MASK64;
                kw[3] = (KBl << 47) | (KBr >> 17);
            }
        }

        private static void ReverseKeys(ulong[] kw, ulong[] k, ulong[] ke)
        {
            // kw1 <-> kw3
            ulong buf = kw[0];
            kw[0] = kw[2];
            kw[2] = buf;

            // kw2 <-> kw4
            buf = kw[1];
            kw[1] = kw[3];
            kw[3] = buf;

            for (int i = k.Length / 2 - 1; i >= 0; i--)
            {
                int j = k.Length - 1 - i;
                buf = k[i];
                k[i] = k[j];
                k[j] = buf;
            }

            for (int i = ke.Length / 2 - 1; i >= 0; i--)
            {
                int j = ke.Length - 1 - i;
                buf = ke[i];
                ke[i] = ke[j];
                ke[j] = buf;
            }
        }

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            ulong[] kw, k, ke;

            KeySchedule(key, Nk, out kw, out k, out ke);

            return ProcessBlock(Nr, kw, k, ke);
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            ulong[] kw, k, ke;

            KeySchedule(key, Nk, out kw, out k, out ke);
            ReverseKeys(kw, k, ke);

            return ProcessBlock(Nr, kw, k, ke);
        }

        private static Func<byte[], byte[]> ProcessBlock(int Nr, ulong[] kw, ulong[] k, ulong[] ke)
        {
            byte[] buffer = new byte[16];
            return (block) =>
            {
                ulong D1 = BitHelper.ToUInt64(block, 0, ByteOrder.BigEndian);
                ulong D2 = BitHelper.ToUInt64(block, 8, ByteOrder.BigEndian);

                D1 ^= kw[0];
                D2 ^= kw[1];

                int keindex = 0;

                for (int r = 0; r < Nr; r++)
                {
                    if (r == 6 || r == 12 || r == 18)
                    {
                        D1 = FL(D1, ke[keindex++]);
                        D2 = FLInv(D2, ke[keindex++]);
                    }

                    if (r % 2 == 0)
                        D2 ^= F(D1, k[r]);
                    else
                        D1 ^= F(D2, k[r]);
                }

                D1 ^= kw[3];
                D2 ^= kw[2];

                BitHelper.GetBytes(D2, ByteOrder.BigEndian, buffer, 0);
                BitHelper.GetBytes(D1, ByteOrder.BigEndian, buffer, 8);

                return buffer;
            };
        }
    }
}