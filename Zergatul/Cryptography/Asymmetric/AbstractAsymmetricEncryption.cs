using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractAsymmetricEncryption
    {
        public abstract byte[] Encrypt(byte[] data);
        public abstract byte[] Decrypt(byte[] data);
    }
}