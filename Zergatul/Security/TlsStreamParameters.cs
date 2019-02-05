using System;

namespace Zergatul.Security
{
    public class TlsStreamParameters
    {
        public TlsVersion? Version { get; set; }
        public bool? RequestClientCertificate { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ClientCertificateValidateCallback { get; set; }
        public Func<Cryptography.Certificate.X509Certificate, bool> ServerCertificateValidateCallback { get; set; }
        public bool LeaveOpen { get; set; }
    }
}