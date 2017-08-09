using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class ECB : AbstractBlockCipherMode
    {
        public override Encryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new ECBEncryptor(cipher, processBlock);

        public override Decryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new ECBDecryptor(cipher, processBlock);

        private class ECBEncryptor : Encryptor
        {
            public ECBEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Encrypt(byte[] data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (data.Length == 0 || data.Length % _cipher.BlockSize != 0)
                    throw new ArgumentException("Invalid data size", nameof(data));

                byte[] result = new byte[data.Length];

                for (int index = 0; index < data.Length; index += _cipher.BlockSize)
                {
                    byte[] plainText = ByteArray.SubArray(data, index, _cipher.BlockSize);
                    byte[] cipherText = _processBlock(plainText);
                    Array.Copy(cipherText, 0, result, index, _cipher.BlockSize);
                }

                return result.ToArray();
            }

            public override byte[] Encrypt(byte[] IV, byte[] data)
            {
                throw new NotSupportedException("ECB doesn't support IV");
            }

            public override AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] authenticatedData)
            {
                throw new NotSupportedException("ECB is not AEAD cipher mode");
            }
        }

        internal class ECBDecryptor : Decryptor
        {
            public ECBDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Decrypt(byte[] data)
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (data.Length == 0 || data.Length % _cipher.BlockSize != 0)
                    throw new ArgumentException("Invalid data size", nameof(data));

                byte[] result = new byte[data.Length];

                for (int index = 0; index < data.Length; index += _cipher.BlockSize)
                {
                    byte[] cipherText = ByteArray.SubArray(data, index, _cipher.BlockSize);
                    byte[] plainText = _processBlock(cipherText);
                    Array.Copy(plainText, 0, result, index, _cipher.BlockSize);
                }

                return result;
            }

            public override byte[] Decrypt(byte[] IV, byte[] data)
            {
                throw new NotSupportedException("ECB doesn't support IV");
            }

            public override byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] authenticatedData)
            {
                throw new NotSupportedException("ECB is not AEAD cipher mode");
            }
        }
    }
}