using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    public class MACData
    {
        public DigestInfo MAC { get; private set; }
        public byte[] MACSalt { get; private set; }
        public int Iterations { get; private set; }

        public static MACData Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 3);

            var mac = DigestInfo.Parse(seq.Elements[0]);

            var os = seq.Elements[1] as OctetString;
            ParseException.ThrowIfNull(os);

            Integer iters = null;
            if (seq.Elements.Count == 3)
            {
                iters = seq.Elements[2] as Integer;
                ParseException.ThrowIfNull(iters);
            }

            return new MACData
            {
                MAC = mac,
                MACSalt = os.Data,
                Iterations = iters != null ? (int)iters.Value : 1
            };
        }
    }
}