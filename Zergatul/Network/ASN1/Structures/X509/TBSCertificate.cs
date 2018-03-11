using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures.X509
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.1
    /// </summary>
    class TBSCertificate
    {
        public int Version { get; private set; }
        public byte[] SerialNumber { get; private set; }
        public AlgorithmIdentifier Signature { get; private set; }
        public Name Issuer { get; private set; }
        public Validity Validity { get; private set; }
        public Name Subject { get; private set; }
        public SubjectPublicKeyInfo SubjectPublicKeyInfo { get; private set; }
        public byte[] IssuerUniqueID { get; private set; }
        public byte[] SubjectUniqueID { get; private set; }
        public Extension[] Extensions { get; private set; }

        public static TBSCertificate Parse(Asn1Element element)
        {
            var result = new TBSCertificate();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 6, 10);

            int index = 0;

            var cs = seq.Elements[index] as ContextSpecific;
            Integer @int;
            if (cs == null)
            {
                result.Version = 2;
            }
            else
            {
                ParseException.ThrowIfTrue(cs.IsImplicit);
                @int = cs.Elements[0] as Integer;
                ParseException.ThrowIfNull(@int);
                result.Version = (int)@int.Value;

                index++;
            }

            @int = seq.Elements[index++] as Integer;
            ParseException.ThrowIfNull(@int);
            result.SerialNumber = @int.Data;

            result.Signature = AlgorithmIdentifier.Parse(seq.Elements[index++]);
            result.Issuer = Name.Parse(seq.Elements[index++]);
            result.Validity = Validity.Parse(seq.Elements[index++]);
            result.Subject = Name.Parse(seq.Elements[index++]);
            result.SubjectPublicKeyInfo = SubjectPublicKeyInfo.Parse(seq.Elements[index++]);

            while (index < seq.Elements.Count)
            {
                cs = seq.Elements[index++] as ContextSpecific;
                ParseException.ThrowIfNull(cs);
                ParseException.ThrowIfTrue(cs.IsImplicit);

                switch (cs.Tag.TagNumberEx)
                {
                    case 1:
                        var bitstr = cs.Elements[0] as BitString;
                        ParseException.ThrowIfNull(bitstr);
                        result.IssuerUniqueID = bitstr.Data;
                        break;
                    case 2:
                        bitstr = cs.Elements[0] as BitString;
                        ParseException.ThrowIfNull(bitstr);
                        result.SubjectUniqueID = bitstr.Data;
                        break;
                    case 3:
                        var extensions = cs.Elements[0] as Sequence;
                        ParseException.ThrowIfNull(extensions);
                        result.Extensions = extensions.Elements.Select(e => Extension.Parse(e)).ToArray();
                        break;
                }
            }

            return result;
        }
    }
}