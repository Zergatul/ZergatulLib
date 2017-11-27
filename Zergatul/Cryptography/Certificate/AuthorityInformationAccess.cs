using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.2.1
    /// </summary>
    public class AuthorityInformationAccess : X509Extension
    {
        public IReadOnlyList<AccessDescription> Descriptions => _descriptions.AsReadOnly();

        private List<AccessDescription> _descriptions;

        protected override void Parse(byte[] data)
        {
            var element = ASN1Element.ReadFrom(data);

            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);

            _descriptions = seq.Elements.Select(e => new AccessDescription(e)).ToList();
        }
    }
}