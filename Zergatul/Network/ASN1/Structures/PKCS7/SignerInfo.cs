using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.PKCS7
{
    class SignerInfo
    {
        public CMSVersion Version { get; private set; }
        public SignerIdentifier SID { get; private set; }
        public AlgorithmIdentifier DigestAlgorithm { get; private set; }
        public Attribute[] SignedAttributes { get; private set; }
        public AlgorithmIdentifier SignatureAlgorithm { get; private set; }
        public byte[] Signature { get; private set; }
        public Attribute[] UnsignedAttributes { get; private set; }

        public byte[] SignedAttributesRaw { get; private set; }

        public static SignerInfo Parse(Asn1Element element)
        {
            var result = new SignerInfo();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 5, 7);

            var @int = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(@int);
            result.Version = (CMSVersion)(int)@int.Value;

            result.SID = SignerIdentifier.Parse(seq.Elements[1], result);
            result.DigestAlgorithm = AlgorithmIdentifier.Parse(seq.Elements[2]);

            int index = 3;

            var cs = seq.Elements[index] as ContextSpecific;
            if (cs != null)
            {
                result.SignedAttributes = cs.Elements.Select(e => Attribute.Parse(e)).ToArray();
                var set = new Set(cs.Elements.ToArray());
                result.SignedAttributesRaw = set.ToBytes();
                index++;
            }

            result.SignatureAlgorithm = AlgorithmIdentifier.Parse(seq.Elements[index++]);

            var octetstr = seq.Elements[index++] as OctetString;
            ParseException.ThrowIfNull(octetstr);
            result.Signature = octetstr.Data;

            if (index < seq.Elements.Count)
            {
                cs = seq.Elements[index] as ContextSpecific;
                if (cs != null)
                {
                    result.UnsignedAttributes = cs.Elements.Select(e => Attribute.Parse(e)).ToArray();
                    index++;
                }
            }

            return result;
        }
    }
}