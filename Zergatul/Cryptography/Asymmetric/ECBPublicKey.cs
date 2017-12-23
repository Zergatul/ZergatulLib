using System;
using Zergatul.Math.EllipticCurves.BinaryField;

namespace Zergatul.Cryptography.Asymmetric
{
    /// <summary>
    /// Elliptic Curve Binary Field public key
    /// </summary>
    public class ECBPublicKey : AbstractPublicKey
    {
        public override int KeySize => Point.Curve.BitSize;

        public ECPoint Point { get; private set; }

        public ECBPublicKey(ECPoint point)
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