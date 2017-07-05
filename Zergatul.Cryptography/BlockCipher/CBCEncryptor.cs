using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    internal class CBCEncryptor : Encryptor
    {
        public override byte[] Encrypt(byte[] IV, byte[] data)
        {
            if (IV == null)
                throw new ArgumentNullException(nameof(IV));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (IV.Length != Cipher.BlockSize)
                throw new ArgumentException("Invalid IV size", nameof(IV));
            if (data.Length == 0 || data.Length % Cipher.BlockSize != 0)
                throw new ArgumentException("Invalid data size", nameof(data));

            ByteArray result = new ByteArray();

            for (int i = 0; i < data.Length / Cipher.BlockSize; i++)
            {
                var plainText = new ByteArray(data).SubArray(i * Cipher.BlockSize, Cipher.BlockSize);
                var cipherText = ProcessBlock((new ByteArray(IV) ^ plainText).ToArray());
                result += cipherText;
                IV = cipherText;
            }

            return result.ToArray();
        }
    }
}