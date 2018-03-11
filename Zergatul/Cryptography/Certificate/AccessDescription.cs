using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class AccessDescription
    {
        public OID AccessMethod { get; private set; }
        public GeneralName AccessLocation { get; private set; }

        internal AccessDescription(Asn1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);
            CertificateParseException.ThrowIfFalse(seq.Elements.Count == 2);

            CertificateParseException.ThrowIfFalse(seq.Elements[0] is ObjectIdentifier);
            AccessMethod = ((ObjectIdentifier)seq.Elements[0]).OID;

            AccessLocation = new GeneralName(seq.Elements[1]);
        }
    }
}