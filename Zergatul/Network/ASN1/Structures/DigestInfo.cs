using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc2315#section-9.4
    /// </summary>
    public class DigestInfo
    {
        public AlgorithmIdentifier Algorithm { get; private set; }
        public byte[] Digest { get; private set; }

        public static DigestInfo Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var ai = AlgorithmIdentifier.Parse(seq.Elements[0]);
            var os = seq.Elements[1] as OctetString;
            ParseException.ThrowIfNull(os);

            return new DigestInfo
            {
                Algorithm = ai,
                Digest = os.Raw
            };
        }
    }
}
