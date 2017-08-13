using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public class Poly1305KeyGenerator
    {
        public byte[] GenerateKey(byte[] key, byte[] nonce)
        {
            var chacha = new ChaCha20();
            var keyStream = chacha.InitKeyStream(key, nonce, 0);
            byte[] result = new byte[32];
            keyStream.Read(result, 0, 32);
            return result;
        }
    }
}