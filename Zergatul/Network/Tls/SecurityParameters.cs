using System.Collections.Generic;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Network.Tls
{
    internal class SecurityParameters
    {
        public ProtocolVersion Version;
        public CipherType CipherType;
        public byte EncKeyLength;
        public byte BlockLength;
        public byte FixedIVLength;
        public byte RecordIVLength;
        public byte MACLength;
        public byte[] MasterSecret;
        public byte[] ClientRandom;
        public byte[] ServerRandom;

        public X509Certificate ServerCertificate;
        public bool CertificateRequested;

        public List<byte> HandshakeBuffer;
        public byte[] ClientFinishedHash;
        public byte[] ServerFinishedHash;

        public bool ExtendedMasterSecret;
    }
}