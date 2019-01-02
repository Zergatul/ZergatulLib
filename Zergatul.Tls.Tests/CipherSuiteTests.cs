using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;
using Zergatul.Network.Tls;
using System.Threading;
using System.Text;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class CipherSuiteTests
    {
        private static string MessageToSend = "Hello World!!!";
        private static string MessageResponse = "OK";

        #region Certificates

        public static X509Certificate GetRSACert()
        {
            return new X509Certificate(Settings.RSA4096CertName, Settings.RSA4096CertPwd);
        }

        private static X509Certificate GetDSSCert()
        {
            return new X509Certificate(Settings.DSA3072CertName, Settings.DSA3072CertPwd);
        }

        private static X509Certificate GetECDSACert()
        {
            return new X509Certificate(Settings.ECDSAp521r1CertName, Settings.ECDSAp521r1CertPwd);
        }

        private static X509Certificate GetDHCert()
        {
            return new X509Certificate(Settings.DHCertName, Settings.DHCertPwd);
        }

        #endregion

        #region DH_Anon

        #region AES

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_AES_256_GCM_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_DH_anon_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_anon_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DH_anon_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_anon_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_DH_Anon_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_anon_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_DH_Anon_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_DES_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_RC4_128_MD5()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_RC4_128_MD5);
        }

        [TestMethod]
        public void TLS_DH_Anon_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_Anon_WITH_SEED_CBC_SHA);
        }

        #endregion

        #endregion

        #region DH

        #region AES

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_AES_256_GCM_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_DH_RSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_ARIA_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_DH_DSS_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_DES_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_DES_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_DSS_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_DSS_WITH_SEED_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DH_RSA_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DH_RSA_WITH_SEED_CBC_SHA);
        }

        #endregion

        #endregion

        #region DHE_RSA

        #region DHE_RSA_AES

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_256_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_256_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8);
        }

        #endregion

        #region DHE_RSA_ARIA

        [TestMethod]
        public void TLS_DHE_RSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region DHE_RSA_CAMELLIA

        [TestMethod]
        public void TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region DHE_RSA_CHACHA20

        [TestMethod]
        public void TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #region DHE_RSA_DES

        [TestMethod]
        public void TLS_DHE_RSA_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA);
        }

        #endregion

        #region DHE_RSA others

        [TestMethod]
        public void TLS_DHE_RSA_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_SEED_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #endregion

        #region DHE_DSS

        #region AES

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_DHE_DSS_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_DHE_DSS_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_SEED_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_DES_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_DSS_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #endregion

        #region ECDH

        #region AES

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_AES_256_GCM_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_ARIA_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_NULL_SHA);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_RSA_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDH_ECDSA_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #endregion

        #region ECDHE_RSA

        #region AES

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_256_CBC_SHA384);
        }


        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_NULL_SHA);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #endregion

        #region ECDHE_ECDSA

        #region AES

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_256_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region CHACHA20

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_NULL_SHA);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #endregion

        #region RSA

        #region AES

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_256_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_128_CCM);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_128_CCM_8);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_256_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_256_CCM);
        }

        [TestMethod]
        public void TLS_RSA_WITH_AES_256_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_AES_256_CCM_8);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_RSA_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_RSA_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_RSA_WITH_CAMELLIA_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_CAMELLIA_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_WITH_CAMELLIA_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_CAMELLIA_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region SEED

        [TestMethod]
        public void TLS_RSA_WITH_SEED_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_SEED_CBC_SHA);
        }

        #endregion

        #region 3DES

        [TestMethod]
        public void TLS_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_3DES_EDE_CBC_SHA);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_RSA_WITH_NULL_MD5()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_NULL_MD5);
        }

        [TestMethod]
        public void TLS_RSA_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_RSA_WITH_NULL_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_NULL_SHA256);
        }

        #endregion

        #region RC4

        [TestMethod]
        public void TLS_RSA_EXPORT_WITH_RC4_40_MD5()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_EXPORT_WITH_RC4_40_MD5);
        }

        [TestMethod]
        public void TLS_RSA_WITH_RC4_128_MD5()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_RC4_128_MD5);
        }

        [TestMethod]
        public void TLS_RSA_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_RC4_128_SHA);
        }

        #endregion

        #region RC2

        [TestMethod]
        public void TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_EXPORT_WITH_RC2_CBC_40_MD5);
        }

        #endregion

        #region DES

        [TestMethod]
        public void TLS_RSA_WITH_DES_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_WITH_DES_CBC_SHA);
        }

        #endregion

        #endregion

        #region PSK

        #region NULL

        [TestMethod]
        public void TLS_PSK_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_PSK_WITH_NULL_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_NULL_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_NULL_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_NULL_SHA384);
        }

        #endregion

        #region AES

        [TestMethod]
        public void TLS_PSK_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_128_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_128_CCM);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_128_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_128_CCM_8);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_256_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_256_CCM);
        }

        [TestMethod]
        public void TLS_PSK_WITH_AES_256_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_AES_256_CCM_8);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_PSK_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_PSK_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region RC4

        [TestMethod]
        public void TLS_PSK_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_RC4_128_SHA);
        }

        #endregion

        #region CHACHA20

        [TestMethod]
        public void TLS_PSK_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_PSK_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #endregion

        #region DHE_PSK

        #region AES

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_128_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_256_CCM()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_128_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM_8);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_AES_256_CCM_8()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM_8);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_DHE_PSK_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_DHE_PSK_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_NULL_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA256);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_NULL_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_NULL_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_DHE_PSK_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #endregion

        #region ECDHE_PSK

        #region AES

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_AES_256_CBC_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_ARIA_256_CBC_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_CAMELLIA_256_CBC_SHA384);
        }

        #endregion

        #region NULL

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_NULL_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_NULL_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_NULL_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #endregion

        #region RSA_PSK

        #region NULL

        [TestMethod]
        public void TLS_RSA_PSK_WITH_NULL_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_NULL_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_NULL_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_NULL_SHA384);
        }

        #endregion

        #region AES

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_256_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_256_GCM_SHA384);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_AES_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_AES_256_CBC_SHA384);
        }

        #endregion

        #region ARIA

        [TestMethod]
        public void TLS_RSA_PSK_WITH_ARIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_ARIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_ARIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_ARIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_ARIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_ARIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_ARIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_ARIA_256_GCM_SHA384);
        }

        #endregion

        #region CAMELLIA

        [TestMethod]
        public void TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_CBC_SHA384);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_128_GCM_SHA256);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_CAMELLIA_256_GCM_SHA384);
        }

        #endregion

        #region others

        [TestMethod]
        public void TLS_RSA_PSK_WITH_RC4_128_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_RC4_128_SHA);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_3DES_EDE_CBC_SHA);
        }

        [TestMethod]
        public void TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_RSA_PSK_WITH_CHACHA20_POLY1305_SHA256);
        }

        #endregion

        #endregion

        private static void TestCipherSuite(CipherSuite cipher)
        {
            var rnd = new DefaultRandom();
            string pskHint = "My secret key!";
            string pskId = "super-secret-key";
            var psk = new PreSharedKey
            {
                Identity = Encoding.ASCII.GetBytes(pskId),
                Secret = new byte[64]
            };
            rnd.GetBytes(psk.Secret);

            var tlsSettings = new TlsStreamSettings
            {
                CipherSuites = new CipherSuite[]
                {
                    cipher
                },
                SupportedCurves = new NamedGroup[]
                {
                    NamedGroup.secp256r1,
                    NamedGroup.secp384r1,
                    NamedGroup.secp521r1
                },
                PSKIdentityHint = Encoding.ASCII.GetBytes(pskHint),
                GetPSKByHint = (hint) =>
                {
                    string hintStr = Encoding.ASCII.GetString(hint);
                    if (hintStr == pskHint)
                        return psk;
                    else
                        throw new InvalidOperationException();
                },
                GetPSKByIdentity = (id) =>
                {
                    string idStr = Encoding.ASCII.GetString(id);
                    if (idStr == pskId)
                        return psk;
                    else
                        throw new InvalidOperationException();
                },
                ServerCertificateValidationOverride = c => true
            };

            var dual = new DualPeer();

            byte[] response = null;
            var serverThread = new Thread(() =>
            {
                X509Certificate cert;
                if (cipher.ToString().Contains("TLS_PSK_") || cipher.ToString().Contains("TLS_DHE_PSK_") || cipher.ToString().Contains("TLS_ECDHE_PSK_"))
                    cert = null;
                else if (cipher.ToString().StartsWith("TLS_DH_"))
                    cert = GetDHCert();
                else if (cipher.ToString().Contains("ECDSA") || cipher.ToString().Contains("_ECDH_"))
                    cert = GetECDSACert();
                else if (cipher.ToString().Contains("RSA"))
                    cert = GetRSACert();
                else if (cipher.ToString().Contains("DSS"))
                    cert = GetDSSCert();
                else
                    throw new NotImplementedException();

                var tlsServer = new TlsStream(dual.Peer1);
                tlsServer.Settings = tlsSettings;
                tlsServer.AuthenticateAsServer("localhost", cert);
                tlsServer.Write(Encoding.ASCII.GetBytes(MessageToSend));
                response = new byte[MessageResponse.Length];
                tlsServer.Read(response, 0, response.Length);
                tlsServer.Close();
            });
            serverThread.Start();

            byte[] buffer = null;
            var tlsClient = new TlsStream(dual.Peer2);
            tlsClient.Settings = tlsSettings;
            tlsClient.AuthenticateAsClient("localhost");

            buffer = new byte[MessageToSend.Length];
            tlsClient.Read(buffer, 0, buffer.Length);

            tlsClient.Write(Encoding.ASCII.GetBytes(MessageResponse));
            tlsClient.Close();

            serverThread.Join();

            Assert.IsTrue(Encoding.ASCII.GetString(buffer) == MessageToSend);
            Assert.IsTrue(Encoding.ASCII.GetString(response) == MessageResponse);
        }

        private static void TestCipherSuiteNetworkStream(CipherSuite cipher)
        {
            var rnd = new DefaultRandom();
            string pskHint = "My secret key!";
            string pskId = "super-secret-key";
            var psk = new PreSharedKey
            {
                Identity = Encoding.ASCII.GetBytes(pskId),
                Secret = new byte[64]
            };
            rnd.GetBytes(psk.Secret);

            var tlsSettings = new TlsStreamSettings
            {
                CipherSuites = new CipherSuite[]
                {
                    cipher
                },
                SupportedCurves = new NamedGroup[]
                {
                    NamedGroup.secp256r1,
                    NamedGroup.secp384r1,
                    NamedGroup.secp521r1
                },
                PSKIdentityHint = Encoding.ASCII.GetBytes(pskHint),
                GetPSKByHint = (hint) =>
                {
                    string hintStr = Encoding.ASCII.GetString(hint);
                    if (hintStr == pskHint)
                        return psk;
                    else
                        throw new InvalidOperationException();
                },
                GetPSKByIdentity = (id) =>
                {
                    string idStr = Encoding.ASCII.GetString(id);
                    if (idStr == pskId)
                        return psk;
                    else
                        throw new InvalidOperationException();
                },
                ServerCertificateValidationOverride = c => true
            };

            var evt = new ManualResetEvent(false);

            byte[] response = null;
            var serverThread = new Thread(() =>
            {
                X509Certificate cert;
                if (cipher.ToString().Contains("TLS_PSK_") || cipher.ToString().Contains("TLS_DHE_PSK_") || cipher.ToString().Contains("TLS_ECDHE_PSK_"))
                    cert = null;
                else if (cipher.ToString().StartsWith("TLS_DH_"))
                    cert = GetDHCert();
                else if (cipher.ToString().Contains("ECDSA") || cipher.ToString().Contains("_ECDH_"))
                    cert = GetECDSACert();
                else if (cipher.ToString().Contains("RSA"))
                    cert = GetRSACert();
                else if (cipher.ToString().Contains("DSS"))
                    cert = GetDSSCert();
                else
                    throw new NotImplementedException();

                var listener = new TcpListener(IPAddress.Any, Settings.Port);
                listener.Start();
                evt.Set();
                try
                {
                    var serverClient = listener.AcceptTcpClient();
                    try
                    {
                        var tlsServerStream = new TlsStream(serverClient.GetStream());
                        tlsServerStream.Settings = tlsSettings;
                        tlsServerStream.AuthenticateAsServer("localhost", cert);
                        tlsServerStream.Write(Encoding.ASCII.GetBytes(MessageToSend));
                        response = new byte[MessageResponse.Length];
                        tlsServerStream.Read(response, 0, response.Length);
                    }
                    finally
                    {
                        serverClient.Close();
                    }
                }
                finally
                {
                    listener.Stop();
                }
            });
            serverThread.Start();

            evt.WaitOne();
            var client = new TcpClient("localhost", Settings.Port);
            byte[] buffer = null;
            try
            {
                var tls = new TlsStream(client.GetStream());
                tls.Settings = tlsSettings;
                tls.AuthenticateAsClient("localhost");

                buffer = new byte[MessageToSend.Length];
                tls.Read(buffer, 0, buffer.Length);

                tls.Write(Encoding.ASCII.GetBytes(MessageResponse));
                serverThread.Join();
            }
            finally
            {
                client.Close();
            }

            Assert.IsTrue(Encoding.ASCII.GetString(buffer) == MessageToSend);
            Assert.IsTrue(Encoding.ASCII.GetString(response) == MessageResponse);
        }
    }
}