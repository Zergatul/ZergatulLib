using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.PKCS7
{
    public class Attribute
    {
        public OID Type { get; private set; }
        public Asn1Element[] Values { get; private set; }

        public static Attribute Parse(Asn1Element element)
        {
            var result = new Attribute();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oid = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oid);
            result.Type = oid.OID;

            var set = seq.Elements[1] as Set;
            ParseException.ThrowIfNull(set);
            result.Values = set.Elements.ToArray();

            return result;
        }
    }
}