using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.2
    /// </summary>
    public class SubjectKeyIdentifier : X509Extension
    {
        public byte[] KeyIdentifier { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = Asn1Element.ReadFrom(data);

            var os = element as OctetString;
            CertificateParseException.ThrowIfNull(os);

            KeyIdentifier = os.Data;
        }
    }
}