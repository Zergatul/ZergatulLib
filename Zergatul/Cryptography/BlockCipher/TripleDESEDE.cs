using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher
{
    public class TripleDESEDE : AbstractBlockCipher
    {
        public override int BlockSize => 8;
        public override int KeySize => 21;

        public override Encryptor CreateEncryptor(byte[] key, BlockCipherMode mode)
        {
            byte[] key1 = new byte[8];
            byte[] key2 = new byte[8];
            byte[] key3 = new byte[8];

            Array.Copy(key, 0, key1, 0, 8);
            Array.Copy(key, 8, key2, 0, 8);
            Array.Copy(key, 16, key3, 0, 8);

            var des = new DES();
            var enc1 = des.CreateEncryptor(key1, BlockCipherMode.ECB);
            var dec = des.CreateDecryptor(key2, BlockCipherMode.ECB);
            var enc2 = des.CreateEncryptor(key3, BlockCipherMode.ECB);

            var encryptor = ResolveEncryptor(mode);
            encryptor.Cipher = this;
            encryptor.ProcessBlock = (block) =>
            {
                return enc2.Encrypt(dec.Decrypt(enc1.Encrypt(block)));
            };

            return encryptor;
        }

        public override Decryptor CreateDecryptor(byte[] key, BlockCipherMode mode)
        {
            byte[] key1 = new byte[8];
            byte[] key2 = new byte[8];
            byte[] key3 = new byte[8];

            Array.Copy(key, 0, key1, 0, 8);
            Array.Copy(key, 8, key2, 0, 8);
            Array.Copy(key, 16, key3, 0, 8);

            var des = new DES();
            var dec1 = des.CreateDecryptor(key1, BlockCipherMode.ECB);
            var enc = des.CreateEncryptor(key2, BlockCipherMode.ECB);
            var dec2 = des.CreateDecryptor(key3, BlockCipherMode.ECB);

            var decryptor = ResolveDecryptor(mode);
            decryptor.Cipher = this;
            decryptor.ProcessBlock = (block) =>
            {
                return dec1.Decrypt(enc.Encrypt(dec2.Decrypt(block)));
            };

            return decryptor;
        }
    }
}