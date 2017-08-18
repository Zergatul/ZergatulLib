using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractSignatureAlgorithm<InputClass, SignatureClass>
    {
        public abstract SignatureClass Sign(InputClass data);

        public byte[] SignHash(AbstractHash hashAlgorithm, SignatureScheme<InputClass, SignatureClass> scheme)
        {
            byte[] hash = hashAlgorithm.ComputeHash();
            InputClass value = scheme.EncodeData(hash);
            SignatureClass signature = Sign(value);
            return scheme.SignatureToBytes(signature);
        }

        public abstract InputClass Verify(SignatureClass signature, ref InputClass data);

        public bool VerifyHash(AbstractHash hashAlgorithm, byte[] signature, SignatureScheme<InputClass, SignatureClass> scheme)
        {
            byte[] hash = hashAlgorithm.ComputeHash();
            SignatureClass signatureClass = scheme.BytesToSignature(signature);
            InputClass value = Verify(signatureClass, ref hash);
            byte[] data = scheme.DecodeData(value);
            return ByteArray.Equals(hash, data);
        }
    }
}