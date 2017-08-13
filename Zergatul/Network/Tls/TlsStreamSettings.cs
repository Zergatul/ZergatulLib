using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    public class TlsStreamSettings
    {
        public CipherSuite[] CipherSuites;
        public NamedCurve[] SupportedCurves;
        public bool SupportExtendedMasterSecret;

        public static TlsStreamSettings Default = new TlsStreamSettings
        {
            SupportExtendedMasterSecret = true,
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
            SupportedCurves = new NamedCurve[]
            {
                NamedCurve.sect163k1,
                NamedCurve.sect163r1,
                NamedCurve.sect163r2,
                NamedCurve.sect193r1,
                NamedCurve.sect193r2,
                NamedCurve.sect233k1,
                NamedCurve.sect233r1,
                NamedCurve.sect239k1,
                NamedCurve.sect283k1,
                NamedCurve.sect283r1,
                NamedCurve.sect409k1,
                NamedCurve.sect409r1,
                NamedCurve.sect571k1,
                NamedCurve.sect571r1,
                NamedCurve.secp160k1,
                NamedCurve.secp160r1,
                NamedCurve.secp160r2,
                NamedCurve.secp192k1,
                NamedCurve.secp192r1,
                NamedCurve.secp224k1,
                NamedCurve.secp224r1,
                NamedCurve.secp256k1,
                NamedCurve.secp256r1,
                NamedCurve.secp384r1,
                NamedCurve.secp521r1,
            }
        };
    }
}