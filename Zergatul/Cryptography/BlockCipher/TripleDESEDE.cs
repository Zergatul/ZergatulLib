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
        public override int KeySize => 24;

        public override Func<byte[], byte[]> CreateEncryptor(byte[] key)
        {
            byte[] key1 = new byte[8];
            byte[] key2 = new byte[8];
            byte[] key3 = new byte[8];

            Array.Copy(key, 0, key1, 0, 8);
            Array.Copy(key, 8, key2, 0, 8);
            Array.Copy(key, 16, key3, 0, 8);

            var des = new DES();
            var enc1 = des.CreateEncryptor(key1);
            var dec = des.CreateDecryptor(key2);
            var enc2 = des.CreateEncryptor(key3);

            return block => enc2(dec(enc1(block)));
        }

        public override Func<byte[], byte[]> CreateDecryptor(byte[] key)
        {
            byte[] key1 = new byte[8];
            byte[] key2 = new byte[8];
            byte[] key3 = new byte[8];

            Array.Copy(key, 0, key1, 0, 8);
            Array.Copy(key, 8, key2, 0, 8);
            Array.Copy(key, 16, key3, 0, 8);

            var des = new DES();
            var dec1 = des.CreateDecryptor(key1);
            var enc = des.CreateEncryptor(key2);
            var dec2 = des.CreateDecryptor(key3);

            return block => dec1(enc(dec2(block)));
        }
    }
}