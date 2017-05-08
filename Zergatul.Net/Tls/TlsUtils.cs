using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class TlsUtils
    {
        private static System.Random _rnd = new System.Random();

        public uint GetGMTUnixTime()
        {
            return (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public byte[] GetRandomBytes(int count)
        {
            var result = new byte[count];
            _rnd.NextBytes(result);
            return result;
        }

        public byte[] HexToBytes(string value)
        {
            return value.Split(' ', '-').Select(h => Convert.ToByte(h, 16)).ToArray();
        }
    }
}
