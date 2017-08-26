using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using Zergatul.Network.Tls;
using System.Net;
using System.Text;
using Zergatul.Cryptography.Certificate;
using System.Diagnostics;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class BouncyCastleTests
    {
        private static CipherSuite[] NotSupportedByBC = new CipherSuite[]
        {
            CipherSuite.TLS_DHE_RSA_WITH_DES_CBC_SHA,
            CipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256,
            CipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
        };

        private bool FilterUnsupportedByBC(CipherSuite cs)
        {
            if (cs.ToString().Contains("ARIA"))
                return false;
            return !NotSupportedByBC.Contains(cs);
        }

        [TestMethod]
        public void TestAll()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites.Where(FilterUnsupportedByBC).ToList());
        }

        [TestMethod]
        public void TestOne()
        {
            TestServer(CipherSuite.TLS_RSA_WITH_DES_CBC_SHA);
        }

        [TestMethod]
        public void Test_ECDSA()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("ECDSA"))
                .ToList());
        }

        [TestMethod]
        public void Test_ECDH_ECDSA_AES()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("_ECDH_ECDSA_WITH_AES"))
                .ToList());
        }

        [TestMethod]
        public void Test_RSA_KeyExchange()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().StartsWith("TLS_RSA_"))
                .ToList());
        }

        [TestMethod]
        public void Test_RSA_CBC()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("RSA"))
                .Where(cs => cs.ToString().Contains("CBC"))
                .ToList());
        }

        [TestMethod]
        public void Test_GCM()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("GCM"))
                .ToList());
        }

        [TestMethod]
        public void Test_CCM()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("CCM"))
                .ToList());
        }

        [TestMethod]
        public void Test_CHACHA20()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("CHACHA20"))
                .ToList());
        }

        [TestMethod]
        public void Test_Camellia()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("CAMELLIA"))
                .ToList());
        }

        [TestMethod]
        public void Test_RSA_CCM()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("RSA"))
                .Where(cs => cs.ToString().Contains("CCM"))
                .ToList());
        }

        [TestMethod]
        public void Test_PSK_KeyExchange_AES()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("TLS_PSK_WITH_AES"))
                .ToList());
        }

        [TestMethod]
        public void Test_DHE_PSK_AES()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(FilterUnsupportedByBC)
                .Where(cs => cs.ToString().Contains("TLS_DHE_PSK_WITH_AES"))
                .ToList());
        }

        private static void TestCipherSuites(IReadOnlyList<CipherSuite> ciphers)
        {
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

            using (var evt = new CountdownEvent(ciphers.Count))
            {
                foreach (var cs in ciphers)
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        TestServer(cs);
                        evt.Signal();
                    });

                evt.Wait();
            }
        }

        private static int startPort = 35000;
        private static HashSet<int> ports = new HashSet<int>();

        private static int GetFreePort()
        {
            lock (ports)
            {
                int port = startPort;
                while (ports.Contains(port))
                    port++;
                ports.Add(port);
                return port;
            }
        }

        private static void ReleasePort(int port)
        {
            lock (ports)
                ports.Remove(port);
        }

        private static void TestServer(CipherSuite cs)
        {
            bool ECcert = cs.ToString().Contains("ECDSA");
            int port = GetFreePort();

            var serverStarted = new ManualResetEvent(false);
            bool serverFailed = false;
            var clientRead = new ManualResetEvent(false);

            string msg = "Hello World!";
            string readMsg = null;

            Thread serverThread = null;
            Thread clientThread = null;

            serverThread = new Thread(() =>
            {
                X509Certificate cert;
                if (cs.ToString().Contains("TLS_PSK_") || cs.ToString().Contains("TLS_DHE_PSK"))
                    cert = null;
                else if (cs.ToString().StartsWith("TLS_DH_"))
                    cert = GetDHCert();
                else if (cs.ToString().Contains("ECDSA"))
                    cert = GetECDSACert();
                else if (cs.ToString().Contains("RSA"))
                    cert = GetRSACert();
                else if (cs.ToString().Contains("DSS"))
                    cert = GetDSSCert();
                else
                    throw new NotImplementedException();

                var listener = new TcpListener(IPAddress.Any, port);
                try
                {
                    listener.Start();

                    serverStarted.Set();

                    using (var client = listener.AcceptTcpClient())
                    {
                        var tlsStream = new TlsStream(client.GetStream());
                        tlsStream.Settings = new TlsStreamSettings
                        {
                            SupportExtendedMasterSecret = true,
                            CipherSuites = new CipherSuite[] { cs },
                            SupportedCurves = new NamedCurve[] { NamedCurve.secp256r1, NamedCurve.secp521r1 },
                            GetPSKByHint = (hint) =>
                            {
                                var sha512 = new Cryptography.Hash.SHA512();
                                sha512.Update(Encoding.ASCII.GetBytes("Bouncy Castle PSK"));

                                return new PreSharedKey
                                {
                                    Identity = Encoding.ASCII.GetBytes("super-secret-key"),
                                    Secret = sha512.ComputeHash()
                                };
                            },
                            PSKIdentityHint = Encoding.ASCII.GetBytes("PSK hint"),
                            GetPSKByIdentity = (id) =>
                            {
                                var sha512 = new Cryptography.Hash.SHA512();
                                sha512.Update(Encoding.ASCII.GetBytes("Bouncy Castle PSK"));

                                return new PreSharedKey
                                {
                                    Identity = Encoding.ASCII.GetBytes("super-secret-key"),
                                    Secret = sha512.ComputeHash()
                                };
                            }
                        };

                        tlsStream.AuthenticateAsServer("localhost", cert);
                        tlsStream.Write(Encoding.ASCII.GetBytes(msg));

                        clientRead.WaitOne();

                        client.Close();
                    }
                }
                catch
                {
                    serverFailed = true;
                    serverStarted.Set();
                    if (clientThread != null)
                        clientThread.Abort();
                }
                finally
                {
                    listener.Stop();
                }
            });

            clientThread = new Thread(() =>
            {
                serverStarted.WaitOne();
                if (serverFailed)
                    return;

                using (var tcp = new TcpClient("localhost", port))
                {
                    var handler = new Org.BouncyCastle.Crypto.Tls.TlsClientProtocol(tcp.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
                    try
                    {
                        var tlsclient = cs.ToString().Contains("_PSK_") ?
                            (Org.BouncyCastle.Crypto.Tls.TlsClient)new MyBCPskClient(cs) :
                            new MyBCTlsClient(cs);
                        handler.Connect(tlsclient);

                        byte[] buffer = new byte[12];
                        handler.Stream.Read(buffer, 0, 12);
                        readMsg = Encoding.ASCII.GetString(buffer);
                    }
                    finally
                    {
                        clientRead.Set();
                        handler.Close();
                    }
                }
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            ReleasePort(port);

            Assert.IsTrue(msg == readMsg, cs.ToString() + " failed");

            Debug.WriteLine(cs.ToString() + " OK");
        }

        private static X509Certificate GetRSACert()
        {
            return new X509Certificate(Settings.RSA4096CertName, Settings.RSA4096CertPwd);
        }

        private static X509Certificate GetDSSCert()
        {
            return new X509Certificate(Settings.DSA3072CertName, Settings.DSA3072CertPwd);
        }

        private static X509Certificate GetECDSACert()
        {
            return new X509Certificate(Settings.ECDSAp256r1CertName, Settings.ECDSAp256r1CertPwd);
        }

        private static X509Certificate GetDHCert()
        {
            return new X509Certificate(Settings.DHCertName, Settings.DHCertPwd);
        }

        class MyBCTlsClient : Org.BouncyCastle.Crypto.Tls.DefaultTlsClient
        {
            private CipherSuite[] _suites;

            public MyBCTlsClient(params CipherSuite[] suites)
            {
                this._suites = suites;
            }

            public override Org.BouncyCastle.Crypto.Tls.TlsAuthentication GetAuthentication()
            {
                return new MyBCTlsAuthentication();
            }

            public override int[] GetCipherSuites()
            {
                return _suites.Select(cs => (int)cs).ToArray();
            }
        }

        class MyBCPskClient : Org.BouncyCastle.Crypto.Tls.PskTlsClient
        {
            private CipherSuite[] _suites;

            public MyBCPskClient(params CipherSuite[] suites)
                : base(new MyTlsPskIdentity())
            {
                this._suites = suites;
            }

            public override int[] GetCipherSuites()
            {
                return _suites.Select(cs => (int)cs).ToArray();
            }
        }

        class MyTlsPskIdentity : Org.BouncyCastle.Crypto.Tls.TlsPskIdentity
        {
            public byte[] GetPsk()
            {
                var sha512 = new Cryptography.Hash.SHA512();
                sha512.Update(Encoding.ASCII.GetBytes("Bouncy Castle PSK"));
                return sha512.ComputeHash();
            }

            public byte[] GetPskIdentity()
            {
                return Encoding.ASCII.GetBytes("super-secret-key");
            }

            public void NotifyIdentityHint(byte[] psk_identity_hint)
            {
                
            }

            public void SkipIdentityHint()
            {
                
            }
        }

        class MyBCTlsAuthentication : Org.BouncyCastle.Crypto.Tls.TlsAuthentication
        {
            public Org.BouncyCastle.Crypto.Tls.TlsCredentials GetClientCredentials(Org.BouncyCastle.Crypto.Tls.CertificateRequest certificateRequest)
            {
                // return client certificate
                return null;
            }

            public void NotifyServerCertificate(Org.BouncyCastle.Crypto.Tls.Certificate serverCertificate)
            {
                // validate server certificate
            }
        }
    }
}