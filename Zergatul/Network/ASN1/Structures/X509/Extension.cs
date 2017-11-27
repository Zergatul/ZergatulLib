using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures.X509
{
    class Extension
    {
        public OID ID { get; private set; }
        public bool Critical { get; private set; }
        public byte[] Value { get; private set; }

        public static Extension Parse(ASN1Element element)
        {
            var result = new Extension();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 3);

            var oid = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oid);
            result.ID = oid.OID;

            int index = 1;
            if (seq.Elements.Count == 3)
            {
                var @bool = seq.Elements[1] as Boolean;
                ParseException.ThrowIfNull(@bool);
                result.Critical = @bool.Value;

                index = 2;
            }

            var octetstr = seq.Elements[index] as OctetString;
            ParseException.ThrowIfNull(octetstr);
            result.Value = octetstr.Data;

            return result;
        }
    }
}