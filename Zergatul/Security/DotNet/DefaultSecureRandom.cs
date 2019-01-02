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

        public override void SetSeed(byte[] seed)
        {
            throw new NotSupportedException();
        }
    }
}