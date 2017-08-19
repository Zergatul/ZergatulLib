using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class SignatureScheme
    {
        public abstract void SetParameter(object parameter);
        public abstract byte[] Sign(byte[] data);
        public abstract bool Verify(byte[] signature, byte[] data);
    }
}