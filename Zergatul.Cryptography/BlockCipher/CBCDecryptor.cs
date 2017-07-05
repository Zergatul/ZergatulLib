using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    internal class CBCDecryptor : Decryptor
    {
        public override byte[] Decrypt(byte[] IV, byte[] data)
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
                var cipherText = new ByteArray(data).SubArray(i * Cipher.BlockSize, Cipher.BlockSize);
                var plainText = ProcessBlock(cipherText.ToArray());
                result += new ByteArray(plainText) ^ new ByteArray(IV);
                IV = cipherText.ToArray();
            }

            return result.ToArray();
        }
    }
}