using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Cryptography.Certificate;
using Zergatul.IO;
using Zergatul.Security;
using Zergatul.Security.Tls;
using Zergatul.Tests;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class OpenSslTests
    {
        static SecurityProvider _openssl = new OpenSslProvider();
        static SecurityProvider _default = new DefaultSecurityProvider();

        [TestMethod]
        public void TLS_RSA_WITH_AES_128_CBC_SHA() => Test(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA);

        private static void Test(CipherSuite cs)
        {
            var pair = new DualPeer();

            var rnd = new Random();
            byte[] serverData = new byte[1024];
            rnd.NextBytes(serverData);
            byte[] clientData = new byte[1024];
            rnd.NextBytes(clientData);

            Exception serverException = null;
            Exception clientException = null;

            var serverEvent = new AutoResetEvent(false);
            var clientEvent = new AutoResetEvent(false);

            var serverThread = new Thread(() =>
            {
                try
                {
                    using (var tls = _default.GetTlsStream(pair.Peer1))
                    {
                        tls.Parameters.Certificate = GetRSACert();
                        tls.Parameters.CipherSuites = new[] { cs };
                        tls.Parameters.MinVersion = TlsVersion.Tls12;
                        tls.Parameters.MaxVersion = TlsVersion.Tls12;
                        tls.AuthenticateAsServer();
                        tls.Write(serverData, 0, serverData.Length);
                        tls.Flush();
                        byte[] buff = new byte[clientData.Length];
                        StreamHelper.ReadArray(tls, buff);
                        Assert.IsTrue(ByteArray.Equals(buff, clientData));
                    }
                }
                catch (Exception ex)
                {
                    serverException = ex;
                }
                finally
                {
                    serverEvent.Set();
                }
            });

            var clientThread = new Thread(() =>
            {
                try
                {
                    using (var tls = _openssl.GetTlsStream(pair.Peer2))
                    {
                        tls.Parameters.CipherSuites = new[] { cs };
                        tls.Parameters.MinVersion = TlsVersion.Tls12;
                        tls.Parameters.MaxVersion = TlsVersion.Tls12;
                        tls.Parameters.Host = "localhost";
                        tls.AuthenticateAsClient();
                        tls.Write(clientData, 0, clientData.Length);
                        tls.Flush();
                    }
                }
                catch (Exception ex)
                {
                    clientException = ex;
                }
                finally
                {
                    clientEvent.Set();
                }
            });

            serverThread.Start();
            clientThread.Start();

            TestHelper.WaitAll(serverEvent, clientEvent);

            if (serverException != null)
                throw new Exception("Server exception", serverException);
            if (clientException != null)
                throw new Exception("Client exception", clientException);
        }

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
    }
}