using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public static class Base58Encoding
    {
        private static readonly char[] DefaultSymbols = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        public static string Encode(byte version, byte[] data) => Encode(version, data, DefaultSymbols);

        public static string Encode(byte version, byte[] data, char[] symbols)
        {
            var dsha256 = new DoubleSHA256();
            dsha256.Update(new byte[] { version });
            dsha256.Update(data);
            byte[] hash = dsha256.ComputeHash();

            byte[] bytes = new byte[data.Length + 5];
            bytes[0] = version;
            Array.Copy(data, 0, bytes, 1, data.Length);
            Array.Copy(hash, 0, bytes, data.Length + 1, 4);

            var bigint = new BigInteger(bytes, ByteOrder.BigEndian);
            string result = bigint.ToString(58, symbols);
            for (int i = 0; i < bytes.Length && bytes[i] == 0; i++)
                result = symbols[0] + result;
            return result;
        }

        public static byte[] Decode(string value) => Decode(value, DefaultSymbols);

        public static byte[] Decode(string value, char[] symbols)
        {
            var bi = BigInteger.Parse(value, 58, symbols);
            byte[] bytes = bi.ToBytes(ByteOrder.BigEndian);

            int leadingZeros = 0;
            while (leadingZeros < value.Length && value[leadingZeros] == symbols[0])
                leadingZeros++;

            if (leadingZeros > 0)
                bytes = ByteArray.Concat(new byte[leadingZeros], bytes);

            var dsha256 = new DoubleSHA256();
            dsha256.Update(bytes, 0, bytes.Length - 4);
            byte[] hash = dsha256.ComputeHash();

            for (int i = 0; i < 4; i++)
                if (bytes[bytes.Length - 4 + i] != hash[i])
                    throw new Base58InvalidCheckSumException();

            return ByteArray.SubArray(bytes, 0, bytes.Length - 4);
        }
    }

    public class Base58InvalidCheckSumException : Exception
    {

    }
}