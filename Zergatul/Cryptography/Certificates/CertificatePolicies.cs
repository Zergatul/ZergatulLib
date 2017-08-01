using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    public class CertificatePolicies : X509Extension
    {
        public IReadOnlyCollection<PolicyInformation> List => _list.AsReadOnly();

        private List<PolicyInformation> _list;

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);

            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);

            _list = seq.Elements.Select(e => new PolicyInformation(e)).ToList();
        }
    }
}