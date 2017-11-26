using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures.PKCS7
{
    class EncapsulatedContentInfo
    {
        public OID ContentType { get; private set; }
        public byte[] Content { get; private set; }

        public static EncapsulatedContentInfo Parse(ASN1Element element)
        {
            var result = new EncapsulatedContentInfo();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 1, 2);

            var oid = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oid);
            result.ContentType = oid.OID;

            if (seq.Elements.Count > 1)
            {
                var cs = seq.Elements[1] as ContextSpecific;
                ParseException.ThrowIfNull(cs);
                ParseException.ThrowIfTrue(cs.IsImplicit);

                var octet = cs.Elements[0] as OctetString;
                ParseException.ThrowIfNull(octet);
                result.Content = octet.Data;
            }

            return result;
        }
    }
}