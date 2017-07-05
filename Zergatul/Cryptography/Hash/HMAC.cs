using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Hash
{
    public class HMAC<T>
        where T : AbstractHash, new()
    {
        public int BlockSize => _hash.BlockSize;
        public int HashSize => _hash.HashSize;

        private ByteArray _ipad, _opad;

        private AbstractHash _hash;

        public HMAC(ByteArray secretKey)
        {
            _hash = new T();

            Init(secretKey);
        }

        public ByteArray ComputeHash(ByteArray data)
        {
            // RFC 2104 // Page 2
            return Hash(_opad + Hash(_ipad + data));
        }

        private void Init(ByteArray secretKey)
        {
            if (secretKey.Length > BlockSize)
                secretKey = Hash(secretKey);

            if (secretKey.Length < BlockSize)
                secretKey = secretKey + new byte[BlockSize - secretKey.Length];

            _ipad = secretKey ^ 0x36;
            _opad = secretKey ^ 0x5C;
        }

        protected virtual ByteArray Hash(ByteArray data)
        {
            _hash.Reset();
            _hash.Update(data.Array);
            return new ByteArray(_hash.ComputeHash());
        }
    }
}