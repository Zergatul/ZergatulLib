using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.X509
{
    class SubjectPublicKeyInfo
    {
        public AlgorithmIdentifier Algorithm { get; private set; }
        public byte[] SubjectPublicKey { get; private set; }

        public static SubjectPublicKeyInfo Parse(Asn1Element element)
        {
            var result = new SubjectPublicKeyInfo();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            result.Algorithm = AlgorithmIdentifier.Parse(seq.Elements[0]);

            var bitstr = seq.Elements[1] as BitString;
            ParseException.ThrowIfNull(bitstr);
            result.SubjectPublicKey = bitstr.Data;

            return result;
        }
    }
}