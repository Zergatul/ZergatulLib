using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class SignatureScheme<InputClass, SignatureClass>
    {
        public abstract InputClass EncodeData(byte[] data);
        public abstract byte[] DecodeData(InputClass data);
        public abstract byte[] SignatureToBytes(SignatureClass signature);
        public abstract SignatureClass BytesToSignature(byte[] signature);
    }
}