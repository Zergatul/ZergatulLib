using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class AttributeTypeAndValue
    {
        public OID Type { get; private set; }
        public object Value { get; private set; }

        internal AttributeTypeAndValue(Asn1Element element)
        {
            var seq = element as Sequence;

            CertificateParseException.ThrowIfFalse(seq != null);
            CertificateParseException.ThrowIfFalse(seq.Elements.Count == 2);


        }
    }
}