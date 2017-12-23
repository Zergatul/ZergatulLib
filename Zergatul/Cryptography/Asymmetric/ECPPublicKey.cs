using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Elliptic Curve Prime Field public key
    /// </summary>
    public class ECPPublicKey : AbstractPublicKey
    {
        public override int KeySize => Point.Curve.BitSize;

        public ECPoint Point { get; private set; }

        public ECPPublicKey(ECPoint point)
        {
            this.Point = point;
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