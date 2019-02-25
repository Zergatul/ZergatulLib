using System;

namespace Zergatul.Security.Tls
{
    public class TlsStreamParameters
    {
        #region Version

        public TlsVersion? MinVersion { get; set; }
        public TlsVersion? MaxVersion { get; set; }

        #endregion

        public SecurityProvider Provider { get; set; }
        public SecureRandom SecureRandom { get; set; }

        public string Host { get; set; }
        public CipherSuite[] CipherSuites { get; set; }
        public string[] AppProtocols { get; set; }
        public bool? ExtendedMasterSecret { get; set; }
        public Cryptography.Certificate.X509Certificate Certificate { get; set; }
        public bool? RequestClientCertificate { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ClientCertificateValidateCallback { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ServerCertificateValidateCallback { get; set; }
        public bool ReuseSessions { get; set; }
        public bool BidirectionalShutdown { get; set; } = true;
        public bool KeepOpen { get; set; }

        public byte[] PSKIdentityHint { get; set; }
        public Func<byte[], PreSharedKey> GetPSKByHint;
        public Func<byte[], PreSharedKey> GetPSKByIdentity;
    }
}