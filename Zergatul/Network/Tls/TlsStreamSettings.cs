using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Network.Tls
{
    public class TlsStreamSettings
    {
        public CipherSuite[] CipherSuites;
        public NamedGroup[] SupportedCurves;
        public bool SupportExtendedMasterSecret;
        public DiffieHellmanParameters DHParameters;

        public byte[] PSKIdentityHint;
        public Func<byte[], PreSharedKey> GetPSKByHint;
        public Func<byte[], PreSharedKey> GetPSKByIdentity;

        public static TlsStreamSettings Default = new TlsStreamSettings
        {
            SupportExtendedMasterSecret = true,
            DHParameters = DiffieHellmanParameters.Group14,
            CipherSuites = new CipherSuite[]
            {
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256,
                CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA,

                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA,
                CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384,
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