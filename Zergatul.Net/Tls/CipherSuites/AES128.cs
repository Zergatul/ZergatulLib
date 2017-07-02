using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.CipherSuites
{
    internal class AES128 : AbstractBlockCipher
    {
        public AES128(BlockCipherMode mode)
            : base(mode, 128, 128)
        {

        }

        protected override ByteArray EncryptBlock(ByteArray block, ByteArray key)
        {
            var rijndael = new System.Security.Cryptography.RijndaelManaged();
            rijndael.BlockSize = 128;
            rijndael.KeySize = 128;
            rijndael.Key = key.ToArray();
            rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
            rijndael.Padding = System.Security.Cryptography.PaddingMode.None;

            var result = new byte[_blockSizeBytes];
            rijndael.CreateEncryptor().TransformBlock(block.ToArray(), 0, _blockSizeBytes, result, 0);
            return new ByteArray(result);
        }

        protected override ByteArray DecryptBlock(ByteArray block, ByteArray key)
        {
            var rijndael = new System.Security.Cryptography.RijndaelManaged();
            rijndael.BlockSize = 128;
            rijndael.KeySize = 128;
            rijndael.Key = key.ToArray();
            rijndael.Mode = System.Security.Cryptography.CipherMode.ECB;
            rijndael.Padding = System.Security.Cryptography.PaddingMode.None;

            var result = new byte[_blockSizeBytes];
            rijndael.CreateDecryptor().TransformBlock(block.ToArray(), 0, _blockSizeBytes, result, 0);
            return new ByteArray(result);
        }
    }
}
