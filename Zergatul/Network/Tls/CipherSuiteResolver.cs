using System;
using Zergatul.Cryptography.BlockCipher;
using Zergatul.Cryptography.Hash;

using Cipher = Zergatul.Network.Tls.CipherSuiteBuilder;
using DHE = Zergatul.Network.Tls.DHEKeyExchange;
using ECDHE = Zergatul.Network.Tls.ECDHEKeyExchange;
using RSA = Zergatul.Network.Tls.RSASignature;
using ECDSA = Zergatul.Network.Tls.ECDSASignature;
using CBC = Zergatul.Network.Tls.HMACCBCCipher;
using GCM = Zergatul.Network.Tls.GCMCipher;
using ChaCha20 = Zergatul.Network.Tls.ChaCha20Poly1305Cipher;

namespace Zergatul.Network.Tls
{
    internal partial class CipherSuiteBuilder
    {
        public static Cipher Resolve(CipherSuite cipherSuite)
        {
            switch (cipherSuite)
            {
                //case CipherSuite.TLS_NULL_WITH_NULL_NULL:
                //case CipherSuite.TLS_RSA_WITH_NULL_MD5:
                //case CipherSuite.TLS_RSA_WITH_NULL_SHA:
                //case CipherSuite.TLS_RSA_EXPORT_WITH_RC4_40_MD5:
                //case CipherSuite.TLS_RSA_WITH_RC4_128_MD5:
                //case CipherSuite.TLS_RSA_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5:
                //case CipherSuite.TLS_RSA_WITH_IDEA_CBC_SHA:
                //case CipherSuite.TLS_RSA_EXPORT_WITH_DES40_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_EXPORT_WITH_DES40_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_EXPORT_WITH_DES40_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_EXPORT_WITH_DES40_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_DHE_RSA_EXPORT_WITH_DES40_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new DES(), new SHA1()), new SHA256());
                case CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new TripleDESEDE(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_EXPORT_WITH_RC4_40_MD5:
                //case CipherSuite.TLS_DH_Anon_WITH_RC4_128_MD5:
                //case CipherSuite.TLS_DH_Anon_EXPORT_WITH_DES40_CBC_SHA:
                //case CipherSuite.TLS_DH_Anon_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_DH_Anon_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.SSL_FORTEZZA_KEA_WITH_NULL_SHA:
                //case CipherSuite.SSL_FORTEZZA_KEA_WITH_FORTEZZA_CBC_SHA:
                //case CipherSuite.TLS_KRB5_WITH_DES_CBC_SHA:
                //case CipherSuite.TLS_KRB5_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_KRB5_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_KRB5_WITH_IDEA_CBC_SHA:
                //case CipherSuite.TLS_KRB5_WITH_DES_CBC_MD5:
                //case CipherSuite.TLS_KRB5_WITH_3DES_EDE_CBC_MD5:
                //case CipherSuite.TLS_KRB5_WITH_RC4_128_MD5:
                //case CipherSuite.TLS_KRB5_WITH_IDEA_CBC_MD5:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_DES_CBC_40_SHA:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_RC2_CBC_40_SHA:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_RC4_40_SHA:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_DES_CBC_40_MD5:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_RC2_CBC_40_MD5:
                //case CipherSuite.TLS_KRB5_EXPORT_WITH_RC4_40_MD5:
                //case CipherSuite.TLS_PSK_WITH_NULL_SHA:
                //case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA:
                //case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA:
                //case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new AES128(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new AES256(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_NULL_SHA256:
                //case CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256:
                //case CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new Camellia128(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_128_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256:
                    return new Cipher(new DHE(new RSA()), new CBC(new AES128(), new SHA256()), new SHA256());
                //case CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256:
                    return new Cipher(new DHE(new RSA()), new CBC(new AES256(), new SHA256()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA256:
                //case CipherSuite.TLS_GOSTR341094_WITH_28147_CNT_IMIT:
                //case CipherSuite.TLS_GOSTR341001_WITH_28147_CNT_IMIT:
                //case CipherSuite.TLS_GOSTR341094_WITH_NULL_GOSTR3411:
                //case CipherSuite.TLS_GOSTR341001_WITH_NULL_GOSTR3411:
                //case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA:
                case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA:
                    return new Cipher(new DHE(new RSA()), new CBC(new Camellia256(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_256_CBC_SHA:
                //case CipherSuite.TLS_PSK_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_DH_Anon_WITH_SEED_CBC_SHA:
                //case CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256:
                    return new Cipher(new DHE(new RSA()), new GCM(new AES128()), new SHA256());
                case CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384:
                    return new Cipher(new DHE(new RSA()), new GCM(new AES256()), new SHA384());
                //case CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_Anon_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_Anon_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384:
                //case CipherSuite.TLS_PSK_WITH_NULL_SHA256:
                //case CipherSuite.TLS_PSK_WITH_NULL_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new AES128(), new SHA1()), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new AES256(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA:
                case CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new TripleDESEDE(), new SHA1()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new AES128(), new SHA1()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new AES256(), new SHA1()), new SHA256());
                //case CipherSuite.TLS_ECDH_Anon_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDH_Anon_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_ECDH_Anon_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_ECDH_Anon_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_ECDH_Anon_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_RSA_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_DSS_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_RSA_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_SRP_SHA_DSS_WITH_AES_256_CBC_SHA:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new AES128(), new SHA256()), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new AES256(), new SHA384()), new SHA384());
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new AES128(), new SHA256()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new AES256(), new SHA384()), new SHA384());
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256:
                    return new Cipher(new ECDHE(new ECDSA()), new GCM(new AES128()), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384:
                    return new Cipher(new ECDHE(new ECDSA()), new GCM(new AES256()), new SHA384());
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256:
                    return new Cipher(new ECDHE(new RSA()), new GCM(new AES128()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384:
                    return new Cipher(new ECDHE(new RSA()), new GCM(new AES256()), new SHA384());
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384:
                //case CipherSuite.TLS_RSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DH_DSS_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DH_DSS_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DH_RSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DHE_DSS_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DH_anon_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DH_anon_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDH_RSA_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_RSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_DSS_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_DSS_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_DSS_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_anon_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_anon_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDH_RSA_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_PSK_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_PSK_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_ARIA_256_CBC_SHA384:
                //case CipherSuite.TLS_PSK_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_PSK_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_ARIA_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_ARIA_256_GCM_SHA384:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_ARIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_ARIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new Camellia128(), new SHA256()), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
                    return new Cipher(new ECDHE(new ECDSA()), new CBC(new Camellia256(), new SHA384()), new SHA384());
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new Camellia128(), new SHA256()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384:
                    return new Cipher(new ECDHE(new RSA()), new CBC(new Camellia256(), new SHA384()), new SHA384());
                //case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
                    return new Cipher(new ECDHE(new ECDSA()), new GCM(new Camellia128()), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
                    return new Cipher(new ECDHE(new ECDSA()), new GCM(new Camellia256()), new SHA384());
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                    return new Cipher(new ECDHE(new RSA()), new GCM(new Camellia128()), new SHA256());
                case CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                    return new Cipher(new ECDHE(new RSA()), new GCM(new Camellia256()), new SHA384());
                //case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384:
                //case CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384:
                //case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384:
                //case CipherSuite.TLS_RSA_WITH_AES_128_CCM:
                //case CipherSuite.TLS_RSA_WITH_AES_256_CCM:
                //case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM:
                //case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM:
                //case CipherSuite.TLS_RSA_WITH_AES_128_CCM_8:
                //case CipherSuite.TLS_RSA_WITH_AES_256_CCM_8:
                //case CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8:
                //case CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8:
                //case CipherSuite.TLS_PSK_WITH_AES_128_CCM:
                //case CipherSuite.TLS_PSK_WITH_AES_256_CCM:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM:
                //case CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM:
                //case CipherSuite.TLS_PSK_WITH_AES_128_CCM_8:
                //case CipherSuite.TLS_PSK_WITH_AES_256_CCM_8:
                //case CipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8:
                //case CipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8:
                //case CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8:
                case CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                    return new Cipher(new ECDHE(new RSA()), new ChaCha20(), new SHA256());
                case CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256:
                    return new Cipher(new ECDHE(new ECDSA()), new ChaCha20(), new SHA256());
                case CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256:
                    return new Cipher(new DHE(new RSA()), new ChaCha20(), new SHA256());
                //case CipherSuite.TLS_PSK_WITH_CHACHA20_POLY1305_SHA256:
                //case CipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
                //case CipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256:
                //case CipherSuite.TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256:
                default:
                    throw new NotImplementedException();
            }
        }
    }
}