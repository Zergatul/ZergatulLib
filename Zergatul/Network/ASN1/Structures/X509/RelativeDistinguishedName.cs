using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.X509
{
    class RelativeDistinguishedName
    {
        public AttributeTypeAndValue[] Attributes { get; private set; }

        public static RelativeDistinguishedName Parse(Asn1Element element)
        {
            var result = new RelativeDistinguishedName();

            var set = element as Set;
            ParseException.ThrowIfNull(set);
            result.Attributes = set.Elements.Select(e => AttributeTypeAndValue.Parse(e)).ToArray();

            return result;
        }
    }
}