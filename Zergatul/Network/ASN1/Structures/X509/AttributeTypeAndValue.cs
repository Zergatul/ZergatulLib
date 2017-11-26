using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures.X509
{
    class AttributeTypeAndValue
    {
        public OID Type { get; private set; }
        public ASN1StringElement Value { get; private set; }

        public static AttributeTypeAndValue Parse(ASN1Element element)
        {
            var result = new AttributeTypeAndValue();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oid = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oid);
            result.Type = oid.OID;

            var str = seq.Elements[1] as ASN1StringElement;
            ParseException.ThrowIfNull(str);
            result.Value = str;

            return result;
        }
    }
}