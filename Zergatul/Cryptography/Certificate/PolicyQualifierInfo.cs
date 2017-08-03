using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class PolicyQualifierInfo
    {
        public OID PolicyQualifierId { get; private set; }
        public Qualifier Qualifier { get; private set; }

        internal PolicyQualifierInfo(ASN1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);
            CertificateParseException.ThrowIfFalse(seq.Elements.Count == 2);

            CertificateParseException.ThrowIfFalse(seq.Elements[0] is ObjectIdentifier);
            PolicyQualifierId = ((ObjectIdentifier)seq.Elements[0]).OID;

            Qualifier = new Qualifier(PolicyQualifierId, seq.Elements[1]);
        }
    }
}