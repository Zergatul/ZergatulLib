using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Hash
{
    // HMAC: Keyed-Hashing for Message Authentication
    // https://www.ietf.org/rfc/rfc2104.txt
    public class HMAC<T> : HMAC
        where T : AbstractHash, new()
    {
        public HMAC(byte[] secretKey)
            : base(new T(), secretKey)
        {
        }
    }

    public class HMAC
    {
        public int BlockSize => _hash.BlockSize;
        public int HashSize => _hash.HashSize;

        private byte[] _ipad, _opad;

        private AbstractHash _hash;

        public HMAC(AbstractHash hash, byte[] secretKey)
        {
            this._hash = hash;

            Init(secretKey);
        }

        public byte[] ComputeHash(byte[] data)
        {
            // RFC 2104 // Page 2
            _hash.Reset();
            _hash.Update(_ipad);
            _hash.Update(data);
            byte[] h = _hash.ComputeHash();

            _hash.Reset();
            _hash.Update(_opad);
            _hash.Update(h);
            return _hash.ComputeHash();
        }

        public byte[] ComputeHash(byte[] data, int index, int length)
        {
            // RFC 2104 // Page 2
            _hash.Reset();
            _hash.Update(_ipad);
            _hash.Update(data, index, length);
            byte[] h = _hash.ComputeHash();

            _hash.Reset();
            _hash.Update(_opad);
            _hash.Update(h);
            return _hash.ComputeHash();
        }

        private void Init(byte[] secretKey)
        {
            if (secretKey.Length > BlockSize)
            {
                _hash.Reset();
                _hash.Update(secretKey);
                secretKey = _hash.ComputeHash();
            }

            if (secretKey.Length < BlockSize)
            {
                byte[] extSecretkey = new byte[BlockSize];
                Array.Copy(secretKey, extSecretkey, secretKey.Length);
                secretKey = extSecretkey;
            }

            _ipad = new byte[BlockSize];
            for (int i = 0; i < _ipad.Length; i++)
                _ipad[i] = (byte)(secretKey[i] ^ 0x36);

            _opad = new byte[BlockSize];
            for (int i = 0; i < _opad.Length; i++)
                _opad[i] = (byte)(secretKey[i] ^ 0x5C);
        }
    }
}