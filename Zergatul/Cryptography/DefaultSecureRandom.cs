using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography
{
    public class DefaultSecureRandom : AbstractRandom, ISecureRandom
    {
        RNGCryptoServiceProvider _rng;

        public DefaultSecureRandom()
        {
            this._rng = new RNGCryptoServiceProvider();
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            _rng.GetBytes(data, offset, count);
        }
    }
}