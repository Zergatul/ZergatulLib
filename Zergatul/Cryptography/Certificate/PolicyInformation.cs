using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class PolicyInformation
    {
        public OID PolicyIdentifier { get; private set; }

        public IReadOnlyCollection<PolicyQualifierInfo> PolicyQualifiers => _policyQualifiers?.AsReadOnly();

        private List<PolicyQualifierInfo> _policyQualifiers;

        internal PolicyInformation(ASN1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);
            CertificateParseException.ThrowIfTrue(seq.Length == 0);

            CertificateParseException.ThrowIfFalse(seq.Elements[0] is ObjectIdentifier);
            PolicyIdentifier = ((ObjectIdentifier)seq.Elements[0]).OID;

            if (seq.Elements.Count > 1)
            {
                CertificateParseException.ThrowIfFalse(seq.Elements[1] is Sequence);
                _policyQualifiers = ((Sequence)seq.Elements[1]).Elements.Select(e => new PolicyQualifierInfo(e)).ToList();
            }
        }
    }
}