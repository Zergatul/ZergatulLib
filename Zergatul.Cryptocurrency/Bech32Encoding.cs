using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency
{
    public static class Bech32Encoding
    {
        private const string Characters = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";
        private static readonly int[] Generator = new[] { 0x3B6A57B2, 0x26508E6D, 0x1EA119FA, 0x3D4233DD, 0x2A1462B3 };

        public static string Encode(string hrp, byte version, byte[] data)
        {
            if (hrp == null)
                throw new ArgumentNullException(nameof(hrp));
            if (version >= 0x20)
                throw new ArgumentOutOfRangeException(nameof(version));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            byte[] rearranged = new byte[(8 * data.Length + 4) / 5 + 1];
            rearranged[0] = version;
            int index = 1;
            int bits = 0;
            int acc = 0;
            for (int i = 0; i < data.Length; i++)
            {
                acc = (acc << 8) | data[i];
                bits += 8;
                while (bits >= 5)
                {
                    bits -= 5;
                    rearranged[index++] = (byte)((acc >> bits) & 0x1F);
                }
            }

            while (bits > 0)
            {
                rearranged[index++] = (byte)((acc >> bits) & 0x1F);
                bits -= 5;
            }

            byte[] checkSum = CreateCheckSum(hrp, rearranged);

            var sb = new StringBuilder(hrp.Length + rearranged.Length + 1);
            sb.Append(hrp);
            sb.Append('1');
            for (int i = 0; i < index; i++)
                sb.Append(Characters[rearranged[i]]);
            for (int i = 0; i < 6; i++)
                sb.Append(Characters[checkSum[i]]);

            return sb.ToString();
        }

        public static bool TryDecode(string hrp, byte version, string value, out byte[] data)
        {
            if (hrp == null)
                throw new ArgumentNullException(hrp);
            if (value == null)
                throw new ArgumentNullException(value);

            data = null;
            value = value.ToLower();

            int position = value.LastIndexOf('1');
            if (position < 1 || position + 7 > value.Length || value.Length > 90)
                return false;

            if (hrp.Length != position || value.IndexOf(hrp) != 0)
                return false;

            for (int i = position + 1; i < value.Length; i++)
                if (Characters.IndexOf(value[i]) == -1)
                    return false;

            byte[] rearranged = new byte[value.Length - position - 1];
            for (int i = position + 1; i < value.Length; i++)
                rearranged[i - position - 1] = (byte)Characters.IndexOf(value[i]);

            if (rearranged[0] != version)
                return false;

            byte[] checkSum = CreateCheckSum(hrp, ByteArray.SubArray(rearranged, 0, rearranged.Length - 6));
            if (!ByteArray.IsSubArray(rearranged, checkSum, rearranged.Length - 6))
                return false;

            int acc = 0;
            int bits = 0;
            data = new byte[(5 * (rearranged.Length - 8) + 7) / 8];
            int index = 0;
            for (int i = 1; i < rearranged.Length - 6; i++)
            {
                acc = (acc << 5) | rearranged[i];
                bits += 5;
                if (bits >= 8)
                {
                    bits -= 8;
                    data[index++] = (byte)((acc >> bits) & 0xFF);
                }
            }

            return true;
        }

        private static byte[] CreateCheckSum(string hrp, byte[] data)
        {
            var values = new byte[hrp.Length * 2 + data.Length + 7];
            HrpExpand(hrp, values);
            Array.Copy(data, 0, values, hrp.Length * 2 + 1, data.Length);

            int mod = Polymod(values) ^ 1;
            byte[] sum = new byte[6];
            for (int i = 0; i < 6; i++)
                sum[i] = (byte)((mod >> 5 * (5 - i)) & 0x1F);
            return sum;
        }

        private static void HrpExpand(string hrp, byte[] values)
        {
            int length = hrp.Length;
            for (int i = 0; i < hrp.Length; i++)
            {
                int ch = hrp[i] & 0xFF;
                values[i] = (byte)(ch >> 5);
                values[length + i + 1] = (byte)(ch & 0x1F);
            }
        }

        private static int Polymod(byte[] values)
        {
            int chk = 1;
            for (int i = 0; i < values.Length; i++)
            {
                int b = chk >> 25;
                chk = (chk & 0x1FFFFFF) << 5 ^ values[i];
                for (int j = 0; j < Generator.Length; j++)
                    if (((b >> j) & 1) == 1)
                        chk ^= Generator[j];
            }
            return chk;
        }
    }
}