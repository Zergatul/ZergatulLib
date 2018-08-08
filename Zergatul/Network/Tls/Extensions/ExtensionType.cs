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
        ServerName = 0x00,

        /// <summary>
        /// RFC 6066
        /// </summary>
        MaxFragmentLength = 0x01,

        /// <summary>
        /// RFC 6066
        /// </summary>
        ClientCertificateUrl = 0x02,

        /// <summary>
        /// RFC 6066
        /// </summary>
        TrustedCAKeys = 0x03,

        /// <summary>
        /// RFC 6066
        /// </summary>
        TruncatedHmac = 0x04,

        /// <summary>
        /// RFC 6066
        /// </summary>
        StatusRequest = 0x05,

        /// <summary>
        /// RFC 4681
        /// </summary>
        UserMapping = 0x06,

        /// <summary>
        /// RFC 5878
        /// </summary>
        ClientAuthz = 0x07,

        /// <summary>
        /// RFC 5878
        /// </summary>
        ServerAuthz = 0x08,

        /// <summary>
        /// RFC 6091
        /// </summary>
        CertType = 0x09,

        /// <summary>
        /// RFC 4492, 7919
        /// </summary>
        SupportedGroups = 0x0A,

        /// <summary>
        /// RFC 4492
        /// </summary>
        ECPointFormats = 0x0B,

        /// <summary>
        /// RFC 5054
        /// </summary>
        SRP = 0x0C,

        /// <summary>
        /// RFC 5246
        /// </summary>
        SignatureAlgorithms = 0x0D,

        /// <summary>
        /// RFC 5764
        /// </summary>
        UseSRTP = 0x0E,

        /// <summary>
        /// RFC 6520
        /// </summary>
        Heartbeat = 0x0F,

        /// <summary>
        /// RFC 7301
        /// </summary>
        ApplicationLayerProtocolNegotiation = 0x10,

        /// <summary>
        /// RFC 6961
        /// </summary>
        StatusRequestV2 = 0x11,

        /// <summary>
        /// RFC 6962
        /// </summary>
        SignedCertificateTimestamp = 0x12,

        /// <summary>
        /// RFC 7250
        /// </summary>
        ClientCertificateType = 0x13,

        /// <summary>
        /// RFC 7250
        /// </summary>
        ServerCertificateType = 0x14,

        /// <summary>
        /// RFC 7685
        /// </summary>
        Padding = 0x15,

        /// <summary>
        /// RFC 7366
        /// </summary>
        EncryptThenMac = 0x16,

        /// <summary>
        /// RFC 7627
        /// </summary>
        ExtendedMasterSecret = 0x17,

        /// <summary>
        /// RFC 7924
        /// </summary>
        CachedInfo = 0x19,

        /// <summary>
        /// RFC 4507
        /// </summary>
        SessionTicketTLS = 0x23,

        KeyShare = 0x33,

        /// <summary>
        /// RFC 5746
        /// </summary>
        RenegotiationInfo = 0xFF01
    }
}
