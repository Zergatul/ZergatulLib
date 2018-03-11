using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class PublicKey
    {
        public OID Algorithm { get; private set; }
        public byte[] Key { get; private set; }
        public Asn1Element Parameters { get; private set; }

        internal PublicKey(Network.Asn1.Structures.X509.SubjectPublicKeyInfo keyInfo)
        {
            this.Algorithm = keyInfo.Algorithm.Algorithm;
            this.Key = keyInfo.SubjectPublicKey;
            this.Parameters = keyInfo.Algorithm.Parameters;
        }

        internal IEllipticCurve ResolveCurve()
        {
            var oid = (Parameters as ObjectIdentifier)?.OID;
            if (oid != null)
            {
                var curve = OIDInfoResolver.GetCurve(oid);
                if (curve == null)
                    throw new NotImplementedException();
                else
                    return curve;
            }

            return null;
        }

        public AbstractAsymmetricAlgorithm ResolveAlgorithm()
        {
            if (Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA)
            {
                var element = Asn1Element.ReadFrom(Key);
                var seq = element as Sequence;
                if (seq != null && seq.Elements.Count == 2 && seq.Elements[0] is Integer && seq.Elements[1] is Integer)
                {
                    var rsa = new RSAEncryption();
                    rsa.PublicKey = new RSAPublicKey(((Integer)seq.Elements[0]).Value, ((Integer)seq.Elements[1]).Value);
                    return rsa;
                }
                else
                    throw new NotImplementedException();
            }
            else if (Algorithm == OID.ISO.MemberBody.US.ANSI_X962.KeyType.ECPublicKey)
            {
                var curve = ResolveCurve();
                if (curve == null)
                    throw new InvalidOperationException();

                var ecdsa = new ECPDSA();
                ecdsa.Parameters = new ECPDSAParameters((Math.EllipticCurves.PrimeField.EllipticCurve)curve);

                if (curve is Math.EllipticCurves.PrimeField.EllipticCurve)
                {
                    ecdsa.PublicKey = new ECPPublicKey(ECPointGeneric.Parse(Key, curve).PFECPoint);
                    return ecdsa;
                }
                else
                    throw new NotImplementedException();
            }
            else if (Algorithm == OID.ISO.MemberBody.US.X957.X9Algorithm.DSA)
            {
                var element = Asn1Element.ReadFrom(Key);
                var integer = element as Integer;
                if (integer == null)
                    throw new InvalidOperationException();

                var dsa = new DSA();
                dsa.PublicKey = new DSAPublicKey(integer.Value);

                var seq = Parameters as Sequence;
                if (seq == null)
                    throw new InvalidOperationException();
                if (seq.Elements.Count != 3)
                    throw new InvalidOperationException();
                if (seq.Elements.Any(e => !(e is Integer)))
                    throw new InvalidOperationException();

                dsa.Parameters = new DSAParameters(
                    p: ((Integer)seq.Elements[0]).Value,
                    q: ((Integer)seq.Elements[1]).Value,
                    g: ((Integer)seq.Elements[2]).Value
                );

                return dsa;
            }
            else if (Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS3.DHKeyAgreement)
            {
                var element = Asn1Element.ReadFrom(Key);
                var integer = element as Integer;
                if (integer == null)
                    throw new InvalidOperationException();

                var dh = new DiffieHellman();
                dh.PublicKey = new DiffieHellmanPublicKey(integer.Value);

                var seq = Parameters as Sequence;
                if (seq == null)
                    throw new InvalidOperationException();
                if (seq.Elements.Count != 2)
                    throw new InvalidOperationException();
                if (seq.Elements.Any(e => !(e is Integer)))
                    throw new InvalidOperationException();

                BigInteger g = ((Integer)seq.Elements[1]).Value;
                BigInteger p = ((Integer)seq.Elements[0]).Value;
                dh.Parameters = new DiffieHellmanParameters(g, p);

                return dh;
            }

            throw new NotImplementedException();
        }
    }
}