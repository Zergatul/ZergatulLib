using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    internal class ECBDecryptor : Decryptor
    {
        public override byte[] Decrypt(byte[] IV, byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (IV != null)
                throw new ArgumentException("ECB block cipher mode doesn't use IV", nameof(IV));
            if (data.Length == 0 || data.Length % Cipher.BlockSize != 0)
                throw new ArgumentException("Invalid data size", nameof(data));

            byte[] result = new byte[data.Length];

            for (int index = 0; index < data.Length; index += Cipher.BlockSize)
            {
                byte[] cipherText = ByteArray.SubArray(data, index, Cipher.BlockSize);
                byte[] plainText = ProcessBlock(cipherText);
                Array.Copy(plainText, 0, result, index, Cipher.BlockSize);
            }

            return result;
        }
    }
}