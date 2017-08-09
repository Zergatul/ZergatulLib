using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.BlockCipher.CipherMode
{
    public abstract class AEADDecryptor
    {
        public abstract byte[] Decrypt(byte[] IV, AEADCipherData data, byte[] authenticatedData);
    }
}