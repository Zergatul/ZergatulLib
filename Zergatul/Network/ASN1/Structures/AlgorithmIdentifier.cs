using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.1.1.2
    /// </summary>
    public class AlgorithmIdentifier
    {
        public OID Algorithm { get; private set; }
        public ASN1Element Parameters { get; private set; }

        public static AlgorithmIdentifier TryParse(ASN1Element element)
        {
            var seq = element as Sequence;
            if (seq == null || seq.Elements.Count != 2)
                return null;

            var oi = seq.Elements[0] as ObjectIdentifier;
            if (oi == null)
                return null;

            return new AlgorithmIdentifier
            {
                Algorithm = oi.OID,
                Parameters = seq.Elements[1]
            };
        }
    }
}