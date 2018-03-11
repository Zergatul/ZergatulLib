using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc6962#section-3.3
    /// </summary>
    public class SignedCertificateTimestampList : X509Extension
    {
        public byte[] SerializedSCT { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = Asn1Element.ReadFrom(data);

            var os = element as OctetString;
            CertificateParseException.ThrowIfNull(os);

            SerializedSCT = os.Data;
        }
    }
}