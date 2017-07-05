using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;

namespace Zergatul.Network.Tls
{
    internal class TlsUtils
    {
        private ISecureRandom _random;

        public TlsUtils(ISecureRandom random)
        {
            this._random = random;
        }

        public uint GetGMTUnixTime()
        {
            return (uint)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public byte[] GetRandomBytes(int count)
        {
            var result = new byte[count];
            _random.GetBytes(result);
            return result;
        }

        public byte[] HexToBytes(string value)
        {
            return value.Split(' ', '-').Select(h => Convert.ToByte(h, 16)).ToArray();
        }
    }
}
