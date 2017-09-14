using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Symmetric.CipherMode
{
    public class ECB : AbstractBlockCipherMode
    {
        protected override BlockCipherEncryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            return new ECBEncryptor(cipher, processBlock);
        }

        protected override BlockCipherDecryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
        {
            return new ECBDecryptor(cipher, processBlock);
        }

        private class ECBEncryptor : BlockCipherEncryptor
        {
            public ECBEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Encrypt(byte[] IV, byte[] data)
            {
                if (IV != null)
                    throw new ArgumentException("ECB doesn't support IV", nameof(IV));

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
        }

        internal class ECBDecryptor : BlockCipherDecryptor
        {
            public ECBDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Decrypt(byte[] IV, byte[] data)
            {
                if (IV != null)
                    throw new ArgumentException("ECB doesn't support IV", nameof(IV));

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
        }
    }
}