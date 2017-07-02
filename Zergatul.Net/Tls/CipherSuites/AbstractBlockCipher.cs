using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal abstract class AbstractBlockCipher
    {
        protected BlockCipherMode _mode;
        protected int _keySizeBits;
        protected int _keySizeBytes;
        protected int _blockSizeBits;
        protected int _blockSizeBytes;

        public AbstractBlockCipher(BlockCipherMode mode, int blockSizeBits, int keySizeBits)
        {
            this._mode = mode;
            this._blockSizeBits = blockSizeBits;
            this._blockSizeBytes = blockSizeBits / 8;
            this._keySizeBits = keySizeBits;
            this._keySizeBytes = keySizeBits / 8;
        }

        protected abstract ByteArray EncryptBlock(ByteArray block, ByteArray key);
        protected abstract ByteArray DecryptBlock(ByteArray block, ByteArray key);

        public ByteArray Encrypt(ByteArray IV, ByteArray data, ByteArray key)
        {
            if (IV.Length % _blockSizeBytes != 0)
                throw new ArgumentException("Invalid IV size");
            if (data.Length % _blockSizeBytes != 0)
                throw new ArgumentException("Invalid data size");
            if (key.Length % _keySizeBytes != 0)
                throw new ArgumentException("Invalid key size");

            var result = new ByteArray();
            switch (_mode)
            {
                case BlockCipherMode.CBC:
                    for (int i = 0; i < data.Length / _blockSizeBytes; i++)
                    {
                        var plainText = data.SubArray(i * _blockSizeBytes, _blockSizeBytes);
                        var cipherText = EncryptBlock(IV ^ plainText, key);
                        result += cipherText;
                        IV = cipherText;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }

        public ByteArray Decrypt(ByteArray IV, ByteArray data, ByteArray key)
        {
            if (IV.Length % _blockSizeBytes != 0)
                throw new ArgumentException("Invalid IV size");
            if (data.Length % _blockSizeBytes != 0)
                throw new ArgumentException("Invalid data size");
            if (key.Length % _keySizeBytes != 0)
                throw new ArgumentException("Invalid key size");

            var result = new ByteArray();
            switch (_mode)
            {
                case BlockCipherMode.CBC:
                    for (int i = 0; i < data.Length / _blockSizeBytes; i++)
                    {
                        var cipherText = data.SubArray(i * _blockSizeBytes, _blockSizeBytes);
                        var plainText = DecryptBlock(cipherText, key);
                        result += plainText ^ IV;
                        IV = cipherText;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result;
        }
    }
}
