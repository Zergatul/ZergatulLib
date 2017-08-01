using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc6962#section-3.3
    /// </summary>
    public class SignedCertificateTimestampList : X509Extension
    {
        public byte[] SerializedSCT { get; private set; }

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);

            var os = element as OctetString;
            CertificateParseException.ThrowIfNull(os);

            SerializedSCT = os.Raw;
        }
    }
}