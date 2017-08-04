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

        public static DigestInfo TryParse(ASN1Element element)
        {
            var seq = element as Sequence;
            if (seq == null || seq.Elements.Count != 2)
                return null;

            var ai = AlgorithmIdentifier.TryParse(seq.Elements[0]);
            var os = seq.Elements[1] as OctetString;

            if (ai == null || os == null)
                return null;

            return new DigestInfo
            {
                Algorithm = ai,
                Digest = os.Raw
            };
        }
    }
}
