using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.BlockCipher.CipherMode;

namespace Zergatul.Cryptography.BlockCipher
{
    public abstract class AbstractBlockCipherMode
    {
        public abstract Encryptor CreateEncryptor();
        public abstract Decryptor CreateDecryptor();

        public static AbstractBlockCipherMode Resolve(BlockCipherMode mode)
        {
            switch (mode)
            {
                case BlockCipherMode.ECB: return new ECB();
                case BlockCipherMode.CBC: return new CBC();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}