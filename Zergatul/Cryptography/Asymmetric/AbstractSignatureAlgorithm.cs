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
        public abstract bool Verify(SignatureClass signature, InputClass data);

        public abstract SignatureScheme GetScheme(string name);
    }
}