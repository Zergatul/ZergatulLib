using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class PublicKey
    {
        public OID Algorithm { get; private set; }
        public byte[] Key { get; private set; }

        internal PublicKey(ASN1CertificateSyntax.SubjectPublicKeyInfo keyInfo)
        {
            if (!(keyInfo.Algorithm.Parameters is Null))
                throw new NotImplementedException();

            this.Algorithm = keyInfo.Algorithm.Algorithm;
            this.Key = keyInfo.SubjectPublicKey.Data;
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
                    rsa.Parameters = new RSAParameters
                    {
                        BitSize = rsa.PublicKey.n.BitSize
                    };
                    return rsa;
                }
                else
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}
