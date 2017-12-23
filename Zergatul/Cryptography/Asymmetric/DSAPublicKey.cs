using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptography.Asymmetric
{
    public class DSAPublicKey : AbstractPublicKey
    {
        public override int KeySize => 0;

        public BigInteger Value { get; private set; }

        public DSAPublicKey(BigInteger value)
        {
            this.Value = value;
        }

        public override AbstractEncryption ResolveEncryption()
        {
            throw new NotImplementedException();
        }

        public override AbstractSignature ResolveSignature()
        {
            throw new NotImplementedException();
        }

        public override AbstractKeyExchange ResolveKeyExchange()
        {
            throw new NotImplementedException();
        }
    }
}