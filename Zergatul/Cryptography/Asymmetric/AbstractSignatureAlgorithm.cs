using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractSignatureAlgorithm
    {
        public abstract byte[] SignHash(byte[] data);

        public virtual byte[] SignData(byte[] data, AbstractHash hashAlgorithm)
        {
            hashAlgorithm.Update(data);
            return SignHash(hashAlgorithm.ComputeHash());
        }

        public abstract bool VerifyHash(byte[] data);

        public virtual bool VerifyData(byte[] data, AbstractHash hashAlgorithm)
        {
            hashAlgorithm.Update(data);
            return VerifyHash(hashAlgorithm.ComputeHash());
        }
    }
}
