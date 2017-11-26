using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7292#section-4.2.3
    /// </summary>
    public class CertBag
    {
        public OID Id { get; private set; }
        public X509Certificate X509Certificate { get; private set; }

        public static CertBag Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var oi = seq.Elements[0] as ObjectIdentifier;
            ParseException.ThrowIfNull(oi);

            var cs = seq.Elements[1] as ContextSpecific;
            ParseException.ThrowIfNull(cs);
            ParseException.ThrowIfTrue(cs.IsImplicit);
            ParseException.ThrowIfNotEqual(cs.Elements.Count, 1);

            var result = new CertBag { Id = oi.OID };

            if (result.Id == OID.ISO.MemberBody.US.RSADSI.PKCS.PKCS9.CertTypes.X509Certificate)
            {
                var os = cs.Elements[0] as OctetString;
                ParseException.ThrowIfNull(os);
                result.X509Certificate = new X509Certificate(os.Data);
            }
            else
                throw new NotImplementedException();

            return result;
        }
    }
}