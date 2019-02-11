using Zergatul.Security.Tls;

namespace Zergatul.Network.Tls
{
    public static class TlsDefaults
    {
        public static CipherSuite[] CipherSuites { get; set; } = new CipherSuite[]
        {
            CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384
        };
    }
}