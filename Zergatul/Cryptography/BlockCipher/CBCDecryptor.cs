﻿using System;
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

            byte[] result = new byte[data.Length];

            for (int index = 0; index < data.Length; index += Cipher.BlockSize)
            {
                byte[] cipherText = ByteArray.SubArray(data, index, Cipher.BlockSize);
                byte[] plainText = ProcessBlock(cipherText);
                ByteArray.Xor(plainText, IV);
                Array.Copy(plainText, 0, result, index, Cipher.BlockSize);
                IV = cipherText;
            }

            return result;
        }
    }
}