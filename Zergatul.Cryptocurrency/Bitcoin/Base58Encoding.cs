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
        private const byte Version = 0;
        private static readonly char[] Symbols = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        public static string Encode(byte[] data)
        {
            var dsha256 = new DoubleSHA256();
            dsha256.Update(new byte[] { Version });
            dsha256.Update(data);
            byte[] hash = dsha256.ComputeHash();

            byte[] bytes = new byte[data.Length + 4];
            Array.Copy(data, 0, bytes, 0, data.Length);
            Array.Copy(hash, 0, bytes, data.Length, 4);

            var bigint = new BigInteger(bytes, ByteOrder.BigEndian);
            return bigint.ToString(58, Symbols);
        }
    }
}