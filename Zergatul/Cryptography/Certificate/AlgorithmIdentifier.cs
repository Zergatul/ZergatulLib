using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.1.1.2
    /// </summary>
    public class AlgorithmIdentifier
    {
        public OID Algorithm { get; private set; }
        public ASN1Element Parameters { get; private set; }

        internal AlgorithmIdentifier(ASN1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);
            CertificateParseException.ThrowIfFalse(seq.Elements.Count == 2);

            CertificateParseException.ThrowIfFalse(seq.Elements[0] is ObjectIdentifier);
            Algorithm = ((ObjectIdentifier)seq.Elements[0]).OID;

            Parameters = seq.Elements[1];
        }
    }
}