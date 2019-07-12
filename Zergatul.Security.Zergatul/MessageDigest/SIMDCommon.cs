namespace Zergatul.Security.Zergatul.MessageDigest
{
    static class SIMDCommon
    {
        #region Constants

        public static readonly uint[] IV224 =
        {
            0x33586E9F, 0x12FFF033, 0xB2D9F64D, 0x6F8FEA53,
            0xDE943106, 0x2742E439, 0x4FBAB5AC, 0x62B9FF96,
            0x22E7B0AF, 0xC862B3A8, 0x33E00CDC, 0x236B86A6,
            0xF64AE77C, 0xFA373B76, 0x7DC1EE5B, 0x7FB29CE8
        };

        public static readonly uint[] IV256 =
        {
            0x4D567983, 0x07190BA9, 0x8474577B, 0x39D726E9,
            0xAAF3D925, 0x3EE20B03, 0xAFD5E751, 0xC96006D3,
            0xC2C2BA14, 0x49B3BCB4, 0xF67CAF46, 0x668626C9,
            0xE2EAA8D2, 0x1FF47833, 0xD0C661A5, 0x55693DE1
        };

        public static readonly uint[] IV384 =
        {
            0x8A36EEBC, 0x94A3BD90, 0xD1537B83, 0xB25B070B,
            0xF463F1B5, 0xB6F81E20, 0x0055C339, 0xB4D144D1,
            0x7360CA61, 0x18361A03, 0x17DCB4B9, 0x3414C45A,
            0xA699A9D2, 0xE39E9664, 0x468BFE77, 0x51D062F8,
            0xB9E3BFE8, 0x63BECE2A, 0x8FE506B9, 0xF8CC4AC2,
            0x7AE11542, 0xB1AADDA1, 0x64B06794, 0x28D2F462,
            0xE64071EC, 0x1DEB91A8, 0x8AC8DB23, 0x3F782AB5,
            0x039B5CB8, 0x71DDD962, 0xFADE2CEA, 0x1416DF71
        };

        public static readonly uint[] IV512 =
        {
            0x0BA16B95, 0x72F999AD, 0x9FECC2AE, 0xBA3264FC,
            0x5E894929, 0x8E9F30E5, 0x2F1DAA37, 0xF0F2C558,
            0xAC506643, 0xA90635A5, 0xE25B878B, 0xAAB7878F,
            0x88817F7A, 0x0A02892B, 0x559A7550, 0x598F657E,
            0x7EEF60A1, 0x6B70E3E8, 0x9C1714D1, 0xB958E2A8,
            0xAB02675E, 0xED1C014F, 0xCD8D65BB, 0xFDB7A257,
            0x09254899, 0xD699C7BC, 0x9019B6DC, 0x2B9022E4,
            0x8FA14956, 0x21BF9BD3, 0xB94D0943, 0x6FFDDC22
        };

        public static readonly int[] P8Xor = new[] { 1, 6, 2, 3, 5, 7, 4 };

        public static readonly int[][] P4 =
        {
            new int[] { 2, 34, 18, 50 },
            new int[] { 6, 38, 22, 54 },
            new int[] { 0, 32, 16, 48 },
            new int[] { 4, 36, 20, 52 },
            new int[] { 14, 46, 30, 62 },
            new int[] { 10, 42, 26, 58 },
            new int[] { 12, 44, 28, 60 },
            new int[] { 8, 40, 24, 56 },
            new int[] { 15, 47, 31, 63 },
            new int[] { 13, 45, 29, 61 },
            new int[] { 3, 35, 19, 51 },
            new int[] { 1, 33, 17, 49 },
            new int[] { 9, 41, 25, 57 },
            new int[] { 11, 43, 27, 59 },
            new int[] { 5, 37, 21, 53 },
            new int[] { 7, 39, 23, 55 },
            new int[] { 8, 40, 24, 56 },
            new int[] { 4, 36, 20, 52 },
            new int[] { 14, 46, 30, 62 },
            new int[] { 2, 34, 18, 50 },
            new int[] { 6, 38, 22, 54 },
            new int[] { 10, 42, 26, 58 },
            new int[] { 0, 32, 16, 48 },
            new int[] { 12, 44, 28, 60 },
            new int[] { 70, 102, 86, 118 },
            new int[] { 64, 96, 80, 112 },
            new int[] { 72, 104, 88, 120 },
            new int[] { 78, 110, 94, 126 },
            new int[] { 76, 108, 92, 124 },
            new int[] { 74, 106, 90, 122 },
            new int[] { 66, 98, 82, 114 },
            new int[] { 68, 100, 84, 116 }
        };

        public static readonly int[][] Q4 =
        {
            new int[] { 66, 98, 82, 114 },
            new int[] { 70, 102, 86, 118 },
            new int[] { 64, 96, 80, 112 },
            new int[] { 68, 100, 84, 116 },
            new int[] { 78, 110, 94, 126 },
            new int[] { 74, 106, 90, 122 },
            new int[] { 76, 108, 92, 124 },
            new int[] { 72, 104, 88, 120 },
            new int[] { 79, 111, 95, 127 },
            new int[] { 77, 109, 93, 125 },
            new int[] { 67, 99, 83, 115 },
            new int[] { 65, 97, 81, 113 },
            new int[] { 73, 105, 89, 121 },
            new int[] { 75, 107, 91, 123 },
            new int[] { 69, 101, 85, 117 },
            new int[] { 71, 103, 87, 119 },
            new int[] { 9, 41, 25, 57 },
            new int[] { 5, 37, 21, 53 },
            new int[] { 15, 47, 31, 63 },
            new int[] { 3, 35, 19, 51 },
            new int[] { 7, 39, 23, 55 },
            new int[] { 11, 43, 27, 59 },
            new int[] { 1, 33, 17, 49 },
            new int[] { 13, 45, 29, 61 },
            new int[] { 71, 103, 87, 119 },
            new int[] { 65, 97, 81, 113 },
            new int[] { 73, 105, 89, 121 },
            new int[] { 79, 111, 95, 127 },
            new int[] { 77, 109, 93, 125 },
            new int[] { 75, 107, 91, 123 },
            new int[] { 67, 99, 83, 115 },
            new int[] { 69, 101, 85, 117 }
        };

        public static readonly int[][] P8 =
        {
            new int[] { 2, 66, 34, 98, 18, 82, 50, 114 },
            new int[] { 6, 70, 38, 102, 22, 86, 54, 118 },
            new int[] { 0, 64, 32, 96, 16, 80, 48, 112 },
            new int[] { 4, 68, 36, 100, 20, 84, 52, 116 },
            new int[] { 14, 78, 46, 110, 30, 94, 62, 126 },
            new int[] { 10, 74, 42, 106, 26, 90, 58, 122 },
            new int[] { 12, 76, 44, 108, 28, 92, 60, 124 },
            new int[] { 8, 72, 40, 104, 24, 88, 56, 120 },
            new int[] { 15, 79, 47, 111, 31, 95, 63, 127 },
            new int[] { 13, 77, 45, 109, 29, 93, 61, 125 },
            new int[] { 3, 67, 35, 99, 19, 83, 51, 115 },
            new int[] { 1, 65, 33, 97, 17, 81, 49, 113 },
            new int[] { 9, 73, 41, 105, 25, 89, 57, 121 },
            new int[] { 11, 75, 43, 107, 27, 91, 59, 123 },
            new int[] { 5, 69, 37, 101, 21, 85, 53, 117 },
            new int[] { 7, 71, 39, 103, 23, 87, 55, 119 },
            new int[] { 8, 72, 40, 104, 24, 88, 56, 120 },
            new int[] { 4, 68, 36, 100, 20, 84, 52, 116 },
            new int[] { 14, 78, 46, 110, 30, 94, 62, 126 },
            new int[] { 2, 66, 34, 98, 18, 82, 50, 114 },
            new int[] { 6, 70, 38, 102, 22, 86, 54, 118 },
            new int[] { 10, 74, 42, 106, 26, 90, 58, 122 },
            new int[] { 0, 64, 32, 96, 16, 80, 48, 112 },
            new int[] { 12, 76, 44, 108, 28, 92, 60, 124 },
            new int[] { 134, 198, 166, 230, 150, 214, 182, 246 },
            new int[] { 128, 192, 160, 224, 144, 208, 176, 240 },
            new int[] { 136, 200, 168, 232, 152, 216, 184, 248 },
            new int[] { 142, 206, 174, 238, 158, 222, 190, 254 },
            new int[] { 140, 204, 172, 236, 156, 220, 188, 252 },
            new int[] { 138, 202, 170, 234, 154, 218, 186, 250 },
            new int[] { 130, 194, 162, 226, 146, 210, 178, 242 },
            new int[] { 132, 196, 164, 228, 148, 212, 180, 244 },
        };

        public static readonly int[][] Q8 =
        {
            new int[] { 130, 194, 162, 226, 146, 210, 178, 242 },
            new int[] { 134, 198, 166, 230, 150, 214, 182, 246 },
            new int[] { 128, 192, 160, 224, 144, 208, 176, 240 },
            new int[] { 132, 196, 164, 228, 148, 212, 180, 244 },
            new int[] { 142, 206, 174, 238, 158, 222, 190, 254 },
            new int[] { 138, 202, 170, 234, 154, 218, 186, 250 },
            new int[] { 140, 204, 172, 236, 156, 220, 188, 252 },
            new int[] { 136, 200, 168, 232, 152, 216, 184, 248 },
            new int[] { 143, 207, 175, 239, 159, 223, 191, 255 },
            new int[] { 141, 205, 173, 237, 157, 221, 189, 253 },
            new int[] { 131, 195, 163, 227, 147, 211, 179, 243 },
            new int[] { 129, 193, 161, 225, 145, 209, 177, 241 },
            new int[] { 137, 201, 169, 233, 153, 217, 185, 249 },
            new int[] { 139, 203, 171, 235, 155, 219, 187, 251 },
            new int[] { 133, 197, 165, 229, 149, 213, 181, 245 },
            new int[] { 135, 199, 167, 231, 151, 215, 183, 247 },
            new int[] { 9, 73, 41, 105, 25, 89, 57, 121 },
            new int[] { 5, 69, 37, 101, 21, 85, 53, 117 },
            new int[] { 15, 79, 47, 111, 31, 95, 63, 127 },
            new int[] { 3, 67, 35, 99, 19, 83, 51, 115 },
            new int[] { 7, 71, 39, 103, 23, 87, 55, 119 },
            new int[] { 11, 75, 43, 107, 27, 91, 59, 123 },
            new int[] { 1, 65, 33, 97, 17, 81, 49, 113 },
            new int[] { 13, 77, 45, 109, 29, 93, 61, 125 },
            new int[] { 135, 199, 167, 231, 151, 215, 183, 247 },
            new int[] { 129, 193, 161, 225, 145, 209, 177, 241 },
            new int[] { 137, 201, 169, 233, 153, 217, 185, 249 },
            new int[] { 143, 207, 175, 239, 159, 223, 191, 255 },
            new int[] { 141, 205, 173, 237, 157, 221, 189, 253 },
            new int[] { 139, 203, 171, 235, 155, 219, 187, 251 },
            new int[] { 131, 195, 163, 227, 147, 211, 179, 243 },
            new int[] { 133, 197, 165, 229, 149, 213, 181, 245 },
        };

        public static readonly int[] FFT64_8_8_Twiddle =
        {
            1,    1,    1,    1,    1,    1,    1,    1,
            1,    2,    4,    8,   16,   32,   64,  128,
            1,   60,    2,  120,    4,  -17,    8,  -34,
            1,  120,    8,  -68,   64,  -30,   -2,   17,
            1,   46,   60,  -67,    2,   92,  120,  123,
            1,   92,  -17,  -22,   32,  117,  -30,   67,
            1,  -67,  120,  -73,    8,  -22,  -68,  -70,
            1,  123,  -34,  -70,  128,   67,   17,   35,
        };

        public static readonly int[] FFT128_8_16_Twiddle =
        {
            1,    1,   1,    1,   1,    1,   1,   1,   1,   1,   1,   1,   1,    1,   1,    1,
            1,   60,   2,  120,   4,  -17,   8, -34,  16, -68,  32, 121,  64,  -15, 128,  -30,
            1,   46,  60,  -67,   2,   92, 120, 123,   4, -73, -17, -11,   8,  111, -34,  -22,
            1,  -67, 120,  -73,   8,  -22, -68, -70,  64,  81, -30, -46,  -2, -123,  17, -111,
            1, -118,  46,  -31,  60,  116, -67, -61,   2,  21,  92, -62, 120,  -25, 123, -122,
            1,  116,  92, -122, -17,   84, -22,  18,  32, 114, 117, -49, -30,  118,  67,   62,
            1,  -31, -67,   21, 120, -122, -73, -50,   8,   9, -22, -89, -68,   52, -70,  114,
            1,  -61, 123,  -50, -34,   18, -70, -99, 128, -98,  67,  25,  17,   -9,  35,  -79
        };

        public static readonly int[] FFT128_2_64_Twiddle =
        {
              1, -118,   46,  -31,   60,  116,  -67,  -61,
              2,   21,   92,  -62,  120,  -25,  123, -122,
              4,   42,  -73, -124,  -17,  -50,  -11,   13,
              8,   84,  111,    9,  -34, -100,  -22,   26,
             16,  -89,  -35,   18,  -68,   57,  -44,   52,
             32,   79,  -70,   36,  121,  114,  -88,  104,
             64,  -99,  117,   72,  -15,  -29,   81,  -49,
            128,   59,  -23, -113,  -30,  -58,  -95,  -98
        };

        public static readonly int[] FFT256_2_128_Twiddle =
        {
              1,   41, -118,   45,   46,   87,  -31,   14,
             60, -110,  116, -127,  -67,   80,  -61,   69,
              2,   82,   21,   90,   92,  -83,  -62,   28,
            120,   37,  -25,    3,  123,  -97, -122, -119,
              4,  -93,   42,  -77,  -73,   91, -124,   56,
            -17,   74,  -50,    6,  -11,   63,   13,   19,
              8,   71,   84,  103,  111,  -75,    9,  112,
            -34, -109, -100,   12,  -22,  126,   26,   38,
             16, -115,  -89,  -51,  -35,  107,   18,  -33,
            -68,   39,   57,   24,  -44,   -5,   52,   76,
             32,   27,   79, -102,  -70,  -43,   36,  -66,
            121,   78,  114,   48,  -88,  -10,  104, -105,
             64,   54,  -99,   53,  117,  -86,   72,  125,
            -15, -101,  -29,   96,   81,  -20,  -49,   47,
            128,  108,   59,  106,  -23,   85, -113,   -7,
            -30,   55,  -58,  -65,  -95,  -40,  -98,   94
        };

        #endregion

        #region Methods

        public static int Reduce(int value) => (value & 0xFF) - (value >> 8);

        public static uint IF(uint x, uint y, uint z) => ((y ^ z) & x) ^ z;

        public static uint MAJ(uint x, uint y, uint z) => (z & y) | ((z | y) & x);

        public static void FFT_8(int[] y, int index, int stripe)
        {
            int u, v;
            u = y[index + stripe * 0];
            v = y[index + stripe * 4];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 4] = (u - v) << 0;
            u = y[index + stripe * 1];
            v = y[index + stripe * 5];
            y[index + stripe * 1] = u + v;
            y[index + stripe * 5] = (u - v) << 2;
            u = y[index + stripe * 2];
            v = y[index + stripe * 6];
            y[index + stripe * 2] = u + v;
            y[index + stripe * 6] = (u - v) << 4;
            u = y[index + stripe * 3];
            v = y[index + stripe * 7];
            y[index + stripe * 3] = u + v;
            y[index + stripe * 7] = (u - v) << 6;
            y[index + stripe * 6] = Reduce(y[index + stripe * 6]);
            y[index + stripe * 7] = Reduce(y[index + stripe * 7]);
            u = y[index + stripe * 0];
            v = y[index + stripe * 2];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 2] = (u - v) << 0;
            u = y[index + stripe * 4];
            v = y[index + stripe * 6];
            y[index + stripe * 4] = u + v;
            y[index + stripe * 6] = (u - v) << 0;
            u = y[index + stripe * 1];
            v = y[index + stripe * 3];
            y[index + stripe * 1] = u + v;
            y[index + stripe * 3] = (u - v) << 4;
            u = y[index + stripe * 5];
            v = y[index + stripe * 7];
            y[index + stripe * 5] = u + v;
            y[index + stripe * 7] = (u - v) << 4;
            y[index + stripe * 7] = Reduce(y[index + stripe * 7]);
            u = y[index + stripe * 0];
            v = y[index + stripe * 1];
            y[index + stripe * 0] = u + v;
            y[index + stripe * 1] = (u - v) << 0;
            u = y[index + stripe * 2];
            v = y[index + stripe * 3];
            y[index + stripe * 2] = u + v;
            y[index + stripe * 3] = (u - v) << 0;
            u = y[index + stripe * 4];
            v = y[index + stripe * 5];
            y[index + stripe * 4] = u + v;
            y[index + stripe * 5] = (u - v) << 0;
            u = y[index + stripe * 6];
            v = y[index + stripe * 7];
            y[index + stripe * 6] = u + v;
            y[index + stripe * 7] = (u - v) << 0;
            y[index + stripe * 0] = Reduce(y[index + stripe * 0]);
            y[index + stripe * 0] = y[index + stripe * 0] <= 128 ? y[index + stripe * 0] : y[index + stripe * 0] - 257;
            y[index + stripe * 1] = Reduce(y[index + stripe * 1]);
            y[index + stripe * 1] = y[index + stripe * 1] <= 128 ? y[index + stripe * 1] : y[index + stripe * 1] - 257;
            y[index + stripe * 2] = Reduce(y[index + stripe * 2]);
            y[index + stripe * 2] = y[index + stripe * 2] <= 128 ? y[index + stripe * 2] : y[index + stripe * 2] - 257;
            y[index + stripe * 3] = Reduce(y[index + stripe * 3]);
            y[index + stripe * 3] = y[index + stripe * 3] <= 128 ? y[index + stripe * 3] : y[index + stripe * 3] - 257;
            y[index + stripe * 4] = Reduce(y[index + stripe * 4]);
            y[index + stripe * 4] = y[index + stripe * 4] <= 128 ? y[index + stripe * 4] : y[index + stripe * 4] - 257;
            y[index + stripe * 5] = Reduce(y[index + stripe * 5]);
            y[index + stripe * 5] = y[index + stripe * 5] <= 128 ? y[index + stripe * 5] : y[index + stripe * 5] - 257;
            y[index + stripe * 6] = Reduce(y[index + stripe * 6]);
            y[index + stripe * 6] = y[index + stripe * 6] <= 128 ? y[index + stripe * 6] : y[index + stripe * 6] - 257;
            y[index + stripe * 7] = Reduce(y[index + stripe * 7]);
            y[index + stripe * 7] = y[index + stripe * 7] <= 128 ? y[index + stripe * 7] : y[index + stripe * 7] - 257;
        }

        #endregion
    }
}