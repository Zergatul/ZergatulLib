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
    class Certificate
    {
        public TBSCertificate TBSCertificate { get; private set; }
        public AlgorithmIdentifier SignatureAlgorithm { get; private set; }
        public byte[] SignatureValue { get; private set; }

        public byte[] TBSCertificateRaw { get; private set; }

        public static Certificate Parse(Asn1Element element)
        {
            var result = new Certificate();

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 3);

            result.TBSCertificate = TBSCertificate.Parse(seq.Elements[0]);
            result.TBSCertificateRaw = seq.Elements[0].Raw;
            result.SignatureAlgorithm = AlgorithmIdentifier.Parse(seq.Elements[1]);

            var bitstr = seq.Elements[2] as BitString;
            ParseException.ThrowIfNull(bitstr);
            result.SignatureValue = bitstr.Data;

            return result;
        }
    }
}