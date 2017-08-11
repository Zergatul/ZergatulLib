using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Network.Tls
{
    internal abstract class AbstractSignature
    {
        public abstract void SetAlgorithm(object algorithm);

        public abstract byte[] Sign(AbstractHash hash);
        public abstract bool Verify(AbstractHash hash, byte[] signature);
    }
}