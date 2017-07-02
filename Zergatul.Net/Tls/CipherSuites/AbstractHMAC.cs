using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class AbstractHMAC
    {
        private ByteArray _secretKey;
        private int _blockLength;
        private int _hashLength;

        private ByteArray _ipad, _opad;

        protected AbstractHMAC(ByteArray secretKey, int blockLength, int hashLength)
        {
            this._secretKey = secretKey;
            this._blockLength = blockLength;
            this._hashLength = hashLength;

            Init();
        }

        private void Init()
        {
            if (_secretKey.Length > _blockLength)
                _secretKey = Hash(_secretKey);

            if (_secretKey.Length < _blockLength)
                _secretKey = _secretKey + new byte[_blockLength - _secretKey.Length];

            _ipad = _secretKey ^ 0x36;
            _opad = _secretKey ^ 0x5C;
        }

        public ByteArray Compute(ByteArray data)
        {
            // RFC 2104 // Page 2
            return Hash(_opad + Hash(_ipad + data));
        }

        protected abstract ByteArray Hash(ByteArray data);
    }
}
