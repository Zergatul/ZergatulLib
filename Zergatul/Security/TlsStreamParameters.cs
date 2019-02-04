using System;

namespace Zergatul.Security
{
    public class TlsStreamParameters
    {
        public TlsVersion? Version { get; set; }
        public bool? RequestClientCertificate { get; set; }
        public Func<bool, Cryptography.Certificate.X509Certificate> ClientCertificateValidateCallback { get; set; }
        public Func<bool, Cryptography.Certificate.X509Certificate> ServerCertificateValidateCallback { get; set; }
        public bool LeaveOpen { get; set; }
    }
}