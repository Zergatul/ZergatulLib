using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography
{
    public class DefaultSecureRandom : ISecureRandom
    {
        RNGCryptoServiceProvider _rng;

        public DefaultSecureRandom()
        {
            this._rng = new RNGCryptoServiceProvider();
        }

        public void GetBytes(byte[] data)
        {
            _rng.GetBytes(data);
        }
    }
}