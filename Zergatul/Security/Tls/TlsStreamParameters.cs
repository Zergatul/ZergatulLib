using System;

namespace Zergatul.Security.Tls
{
    public class TlsStreamParameters
    {
        #region Version

        public TlsVersion? MinVersion { get; set; }
        public TlsVersion? MaxVersion { get; set; }

        #endregion

        public CipherSuite[] CipherSuites { get; set; }
        public bool? RequestClientCertificate { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ClientCertificateValidateCallback { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ServerCertificateValidateCallback { get; set; }
        public bool BidirectionalShutdown { get; set; } = true;
        public bool LeaveOpen { get; set; }
    }
}