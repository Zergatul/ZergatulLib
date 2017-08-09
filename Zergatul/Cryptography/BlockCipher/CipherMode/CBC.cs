using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class CBC : AbstractBlockCipherMode
    {
        public override Encryptor CreateEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new CBCEncryptor(cipher, processBlock);

        public override Decryptor CreateDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock) => new CBCDecryptor(cipher, processBlock);

        private class CBCEncryptor : Encryptor
        {
            public CBCEncryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Encrypt(byte[] IV, byte[] data)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (IV.Length != _cipher.BlockSize)
                    throw new ArgumentException("Invalid IV size", nameof(IV));
                if (data.Length == 0 || data.Length % _cipher.BlockSize != 0)
                    throw new ArgumentException("Invalid data size", nameof(data));

                byte[] result = new byte[data.Length];

                for (int index = 0; index < data.Length; index += _cipher.BlockSize)
                {
                    byte[] plainText = ByteArray.SubArray(data, index, _cipher.BlockSize);
                    ByteArray.Xor(plainText, IV);
                    byte[] cipherText = _processBlock(plainText);
                    Array.Copy(cipherText, 0, result, index, _cipher.BlockSize);
                    IV = cipherText;
                }

                return result;
            }

            public override byte[] Encrypt(byte[] data)
            {
                throw new NotSupportedException("CBC requires IV");
            }

            public override AEADCipherData Encrypt(byte[] IV, byte[] data, byte[] authenticatedData)
            {
                throw new NotSupportedException("CBC is not AEAD cipher mode");
            }
        }

        private class CBCDecryptor : Decryptor
        {
            public CBCDecryptor(AbstractBlockCipher cipher, Func<byte[], byte[]> processBlock)
                : base(cipher, processBlock)
            { }

            public override byte[] Decrypt(byte[] IV, byte[] data)
            {
                if (IV == null)
                    throw new ArgumentNullException(nameof(IV));
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                if (IV.Length != _cipher.BlockSize)
                    throw new ArgumentException("Invalid IV size", nameof(IV));
                if (data.Length == 0 || data.Length % _cipher.BlockSize != 0)
                    throw new ArgumentException("Invalid data size", nameof(data));

                byte[] result = new byte[data.Length];

                for (int index = 0; index < data.Length; index += _cipher.BlockSize)
                {
                    byte[] cipherText = ByteArray.SubArray(data, index, _cipher.BlockSize);
                    byte[] plainText = _processBlock(cipherText);
                    ByteArray.Xor(plainText, IV);
                    Array.Copy(plainText, 0, result, index, _cipher.BlockSize);
                    IV = cipherText;
                }

                return result;
            }

            public override byte[] Decrypt(byte[] data)
            {
                throw new NotSupportedException("CBC requires IV");
            }

            public override byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] authenticatedData)
            {
                throw new NotSupportedException("CBC is not AEAD cipher mode");
            }
        }
    }
}