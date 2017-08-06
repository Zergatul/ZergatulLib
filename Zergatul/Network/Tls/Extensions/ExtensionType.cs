using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    public enum ExtensionType : ushort
    {
        /// <summary>
        /// RFC 6066
        /// </summary>
        ServerName = 0,

        /// <summary>
        /// RFC 6066
        /// </summary>
        MaxFragmentLength = 1,

        /// <summary>
        /// RFC 6066
        /// </summary>
        ClientCertificateUrl = 2,

        /// <summary>
        /// RFC 6066
        /// </summary>
        TrustedCAKeys = 3,

        /// <summary>
        /// RFC 6066
        /// </summary>
        TruncatedHmac = 4,

        /// <summary>
        /// RFC 6066
        /// </summary>
        StatusRequest = 5,

        /// <summary>
        /// RFC 4681
        /// </summary>
        UserMapping = 6,

        /// <summary>
        /// RFC 5878
        /// </summary>
        ClientAuthz = 7,

        /// <summary>
        /// RFC 5878
        /// </summary>
        ServerAuthz = 8,

        /// <summary>
        /// RFC 6091
        /// </summary>
        CertType = 9,

        /// <summary>
        /// RFC 4492, 7919
        /// </summary>
        SupportedEllipticCurves = 10,

        /// <summary>
        /// RFC 4492
        /// </summary>
        ECPointFormats = 11,

        /// <summary>
        /// RFC 5054
        /// </summary>
        SRP = 12,

        /// <summary>
        /// RFC 5246
        /// </summary>
        SignatureAlgorithms = 13,

        /// <summary>
        /// RFC 5764
        /// </summary>
        UseSRTP = 14,

        /// <summary>
        /// RFC 6520
        /// </summary>
        Heartbeat = 15,

        /// <summary>
        /// RFC 7301
        /// </summary>
        ApplicationLayerProtocolNegotiation = 16,

        /// <summary>
        /// RFC 6961
        /// </summary>
        StatusRequestV2 = 17,

        /// <summary>
        /// RFC 6962
        /// </summary>
        SignedCertificateTimestamp = 18,

        /// <summary>
        /// RFC 7250
        /// </summary>
        ClientCertificateType = 19,

        /// <summary>
        /// RFC 7250
        /// </summary>
        ServerCertificateType = 20,

        /// <summary>
        /// RFC 7685
        /// </summary>
        Padding = 21,

        /// <summary>
        /// RFC 7366
        /// </summary>
        EncryptThenMac = 22,

        /// <summary>
        /// RFC 7627
        /// </summary>
        ExtendedMasterSecret = 23,

        /// <summary>
        /// RFC 7924
        /// </summary>
        CachedInfo = 25,

        /// <summary>
        /// RFC 4507
        /// </summary>
        SessionTicketTLS = 35,

        /// <summary>
        /// RFC 5746
        /// </summary>
        RenegotiationInfo = 65281
    }
}
