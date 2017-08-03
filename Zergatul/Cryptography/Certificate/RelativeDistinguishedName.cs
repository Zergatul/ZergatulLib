using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class RelativeDistinguishedName
    {
        public IReadOnlyCollection<AttributeTypeAndValue> List => _list.AsReadOnly();

        private List<AttributeTypeAndValue> _list;

        internal RelativeDistinguishedName(ASN1Element element)
        {
            var set = element as Set;

            CertificateParseException.ThrowIfFalse(set != null);

            _list = set.Elements.Select(e => new AttributeTypeAndValue(e)).ToList();
        }
    }
}