using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractSignatureAlgorithm<SignatureClass>
    {
        public abstract SignatureClass SignHash(byte[] hash);

        public SignatureClass SignHash(AbstractHash hashAlgorithm)
        {
            return SignHash(hashAlgorithm.ComputeHash());
        }

        public SignatureClass SignData(AbstractHash hashAlgorithm, byte[] data)
        {
            hashAlgorithm.Reset();
            hashAlgorithm.Update(data);
            return SignHash(hashAlgorithm.ComputeHash());
        }

        public abstract bool VerifyHash(byte[] hash, SignatureClass signature);

        public bool VerifyHash(AbstractHash hashAlgorithm, SignatureClass signature)
        {
            return VerifyHash(hashAlgorithm.ComputeHash(), signature);
        }

        public bool VerifyData(AbstractHash hashAlgorithm, byte[] data, SignatureClass signature)
        {
            hashAlgorithm.Reset();
            hashAlgorithm.Update(data);
            return VerifyHash(hashAlgorithm.ComputeHash(), signature);
        }
    }
}
