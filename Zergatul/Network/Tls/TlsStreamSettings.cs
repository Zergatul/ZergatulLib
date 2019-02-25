using System;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Certificate;
using Zergatul.Security.Tls;

namespace Zergatul.Network.Tls
{
    [Obsolete]
    public class TlsStreamSettings
    {
        public CipherSuite[] CipherSuites;
        public NamedGroup[] SupportedCurves;
        public bool SupportExtendedMasterSecret;
        public DiffieHellmanParameters DHParameters;
        public bool ReuseSessions;

        public Func<byte[]> GetRandom;

        public Extensions.TlsExtension[] Extensions;

        public byte[] PSKIdentityHint;
        public Func<byte[], PreSharedKey> GetPSKByHint;
        public Func<byte[], PreSharedKey> GetPSKByIdentity;

        public Func<X509Certificate, bool> ServerCertificateValidationOverride;
        public bool RequestClientCertificate;
        public Func<X509Certificate, bool> ClientCertificateValidate;

        public TlsStreamSettings Clone()
        {
            var result = (TlsStreamSettings)MemberwiseClone();

            if (CipherSuites != null)
                result.CipherSuites = (CipherSuite[])CipherSuites.Clone();

            if (SupportedCurves != null)
                result.SupportedCurves = (NamedGroup[])SupportedCurves.Clone();

            if (Extensions != null)
                result.Extensions = (Extensions.TlsExtension[])Extensions.Clone();

            if (PSKIdentityHint != null)
                result.PSKIdentityHint = (byte[])PSKIdentityHint.Clone();

            return result;
        }

        public static TlsStreamSettings Default => new TlsStreamSettings
        {
            SupportExtendedMasterSecret = true,
            ReuseSessions = true,
            DHParameters = DiffieHellmanParameters.Group14,
            CipherSuites = new CipherSuite[]
            {
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,

                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM,

                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384,

                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,

                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,

                CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,

                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM,
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM,

                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256
            },
            SupportedCurves = new NamedGroup[]
            {
                NamedGroup.sect163k1,
                NamedGroup.sect163r1,
                NamedGroup.sect163r2,
                NamedGroup.sect193r1,
                NamedGroup.sect193r2,
                NamedGroup.sect233k1,
                NamedGroup.sect233r1,
                NamedGroup.sect239k1,
                NamedGroup.sect283k1,
                NamedGroup.sect283r1,
                NamedGroup.sect409k1,
                NamedGroup.sect409r1,
                NamedGroup.sect571k1,
                NamedGroup.sect571r1,
                NamedGroup.secp160k1,
                NamedGroup.secp160r1,
                NamedGroup.secp160r2,
                NamedGroup.secp192k1,
                NamedGroup.secp192r1,
                NamedGroup.secp224k1,
                NamedGroup.secp224r1,
                NamedGroup.secp256k1,
                NamedGroup.secp256r1,
                NamedGroup.secp384r1,
                NamedGroup.secp521r1,
            }
        };
    }

    public class PreSharedKey
    {
        public byte[] Identity;
        public byte[] Secret;
    }
}