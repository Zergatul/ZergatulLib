using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.1
    /// </summary>
    public class AuthorityKeyIdentifier : X509Extension
    {
        public byte[] KeyIdentifier { get; private set; }
        public GeneralNames AuthorityCertIssuer { get; private set; }
        public byte[] AuthorityCertSerialNumber { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = Asn1Element.ReadFrom(data);

            var seq = element as Sequence;
            CertificateParseException.ThrowIfFalse(seq != null);
            CertificateParseException.ThrowIfTrue(seq.Elements.Any(e => !(e is ContextSpecific)));

            foreach (var cs in seq.Elements.Cast<ContextSpecific>())
                switch (cs.Tag.TagNumberEx)
                {
                    case 0:
                        KeyIdentifier = cs.As<OctetString>().Data;
                        break;
                    case 1:
                        AuthorityCertIssuer = new GeneralNames(cs.Elements[0]);
                        break;
                    case 2:
                        AuthorityCertSerialNumber = cs.As<Integer>().Data;
                        break;
                    default:
                        throw new CertificateParseException();
                }
        }
    }
}