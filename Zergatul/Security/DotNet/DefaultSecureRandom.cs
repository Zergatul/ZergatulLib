using System;
using System.Security.Cryptography;

namespace Zergatul.Security.DotNet
{
    class DefaultSecureRandom : SecureRandom
    {
        private RNGCryptoServiceProvider _rng;

        public DefaultSecureRandom()
        {
            this._rng = new RNGCryptoServiceProvider();
        }

        public override void GetNextBytes(byte[] bytes)
        {
            _rng.GetBytes(bytes);
        }

        public override void GetNextBytes(byte[] bytes, int offset, int count)
        {
            _rng.GetBytes(bytes, offset, count);
        }

        public override void SetSeed(byte[] seed)
        {
            throw new NotSupportedException();
        }
    }
}