using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptography.Asymmetric
{
    public abstract class AbstractKey
    {
        public abstract int KeySize { get; }
        public virtual int KeySizeBytes => (KeySize + 7) / 8;
        public abstract AbstractEncryption ResolveEncryption();
        public abstract AbstractSignature ResolveSignature();
        public abstract AbstractKeyExchange ResolveKeyExchange();
    }
}