using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    public class CertificatePolicies : X509Extension
    {
        public IReadOnlyList<PolicyInformation> List => _list.AsReadOnly();

        private List<PolicyInformation> _list;

        protected override void Parse(byte[] data)
        {
            var element = Asn1Element.ReadFrom(data);

            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);

            _list = seq.Elements.Select(e => new PolicyInformation(e)).ToList();
        }
    }
}