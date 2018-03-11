using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.Asn1.Structures.PKCS7
{
    class SignedData
    {
        public int Version { get; private set; }
        public AlgorithmIdentifier[] DigestAlgorithms { get; private set; }
        public EncapsulatedContentInfo EncapContentInfo { get; private set; }
        public X509Certificate[] Certificates { get; private set; }
        public SignerInfo[] SignerInfos { get; private set; }

        public static SignedData Parse(Asn1Element element)
        {
            var result = new SignedData();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 4, 6);

            var @int = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(@int);
            result.Version = (int)@int.Value;

            var set = seq.Elements[1] as Set;
            ParseException.ThrowIfNull(set);
            result.DigestAlgorithms = set.Elements.Select(e => AlgorithmIdentifier.Parse(e)).ToArray();

            result.EncapContentInfo = EncapsulatedContentInfo.Parse(seq.Elements[2]);

            int index = 3;

            if (index < seq.Elements.Count - 1)
            {
                var cs = seq.Elements[index] as ContextSpecific;
                if (cs != null && !cs.IsImplicit)
                {
                    // Single certificate
                    if (cs.Elements[0] is Sequence)
                    {
                        result.Certificates = new[]
                        {
                        new X509Certificate(cs.Elements[0])
                    };
                    }
                    else
                        throw new NotImplementedException();

                    index++;
                }
            }

            set = seq.Elements[index++] as Set;
            ParseException.ThrowIfNull(set);
            result.SignerInfos = set.Elements.Select(e => SignerInfo.Parse(e)).ToArray();

            return result;
        }
    }
}