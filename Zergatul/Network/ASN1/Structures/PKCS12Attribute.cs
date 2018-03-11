using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures
{
    public class PKCS12Attribute
    {
        public OID Id { get; private set; }
        public byte[] LocalKeyId { get; private set; }

        public static PKCS12Attribute Parse(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);

            var set = seq.Elements[1] as Set;
            ParseException.ThrowIfNull(set);

            var result = new PKCS12Attribute { Id = oi.OID };

            if (result.Id == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS9.LocalKeyID)
            {
                ParseException.ThrowIfNotEqual(set.Elements.Count, 1);
                var os = set.Elements[0] as OctetString;
                ParseException.ThrowIfNull(os);
                result.LocalKeyId = os.Data;
            }
            else
                throw new NotImplementedException();

            return result;
        }
    }
}