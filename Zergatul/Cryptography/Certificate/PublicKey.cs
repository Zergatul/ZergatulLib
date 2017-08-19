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
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class PublicKey
    {
        public OID Algorithm { get; private set; }
        public byte[] Key { get; private set; }
        public ASN1Element Parameters { get; private set; }

        internal PublicKey(X509CertificateSyntax.SubjectPublicKeyInfo keyInfo)
        {
            this.Algorithm = keyInfo.Algorithm.Algorithm;
            this.Key = keyInfo.SubjectPublicKey.Data;
            this.Parameters = keyInfo.Algorithm.Parameters;
        }

        internal IEllipticCurve ResolveCurve()
        {
            var oid = (Parameters as ObjectIdentifier)?.OID;
            if (oid != null)
            {
                if (oid == OID.ISO.IdentifiedOrganization.Certicom.Curve.secp521r1)
                    return Math.EllipticCurves.PrimeField.EllipticCurve.secp521r1;
                else if (oid == OID.ISO.MemberBody.US.ANSI_X962.Curves.Prime.Prime256v1)
                    return Math.EllipticCurves.PrimeField.EllipticCurve.secp256r1;
                else
                    throw new NotImplementedException();
            }

            return null;
        }

        public AbstractAsymmetricAlgorithm ResolveAlgorithm()
        {
            if (Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA)
            {
                var element = ASN1Element.ReadFrom(Key);
                var seq = element as Sequence;
                if (seq != null && seq.Elements.Count == 2 && seq.Elements[0] is Integer && seq.Elements[1] is Integer)
                {
                    var rsa = new RSA();
                    rsa.PublicKey = new RSAPublicKey
                    {
                        n = ((Integer)seq.Elements[0]).Value,
                        e = ((Integer)seq.Elements[1]).Value
                    };
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

                var ecdsa = new ECDSA();
                ecdsa.Parameters = new ECDSAParameters { Curve = curve };

                if (curve is Math.EllipticCurves.PrimeField.EllipticCurve)
                {
                    ecdsa.PublicKey = ECPointGeneric.Parse(Key, curve);
                    return ecdsa;
                }
                else
                    throw new NotImplementedException();
            }
            else if (Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS3.DHKeyAgreement)
            {
                var element = ASN1Element.ReadFrom(Key);
                var integer = element as Integer;
                if (integer == null)
                    throw new InvalidOperationException();

                var dh = new DiffieHellman();
                dh.PublicKey = integer.Value;

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