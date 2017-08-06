using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc2315#section-10.1
    /// </summary>
    public class EncryptedContentInfo
    {
        public OID Type { get; private set; }
        public AlgorithmIdentifier EncryptionAlgorithm { get; private set; }
        public byte[] Content { get; private set; }

        public static EncryptedContentInfo Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 2, 3);

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);

            var result = new EncryptedContentInfo
            {
                Type = oi.OID,
                EncryptionAlgorithm = AlgorithmIdentifier.Parse(seq.Elements[1])
            };

            if (seq.Elements.Count == 3)
            {
                var cs = seq.Elements[2] as ContextSpecific;
                ParseException.ThrowIfNull(cs);
                ParseException.ThrowIfNotEqual(cs.Tag.TagNumberEx, 0);
                result.Content = cs.As<OctetString>().Raw;
            }

            return result;
        }
    }
}