using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5208#section-5
    /// </summary>
    public class PrivateKeyInfo
    {
        public int Version { get; private set; }
        public AlgorithmIdentifier Algorithm { get; private set; }
        public byte[] PrivateKey { get; private set; }
        public object Attributes { get; private set; }

        public RSAPrivateKey RSA { get; private set; }
        public ECPrivateKey EC { get; private set; }

        public static PrivateKeyInfo Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 3); // can be 4 with attributes

            var @int = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(@int);

            var os = seq.Elements[2] as OctetString;
            ParseException.ThrowIfNull(os);

            var result = new PrivateKeyInfo
            {
                Version = (int)@int.Value,
                Algorithm = AlgorithmIdentifier.Parse(seq.Elements[1]),
                PrivateKey = os.Raw
            };

            if (result.Algorithm.Algorithm == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS1.RSA)
                result.RSA = RSAPrivateKey.Parse(ASN1Element.ReadFrom(result.PrivateKey));
            else if (result.Algorithm.Algorithm == OID.ISO.MemberBody.US.ANSI_X962.KeyType.ECPublicKey)
                result.EC = ECPrivateKey.Parse(ASN1Element.ReadFrom(result.PrivateKey));
            else
                throw new NotImplementedException();

            return result;
        }
    }
}