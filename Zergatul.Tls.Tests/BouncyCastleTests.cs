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

        [TestMethod]
        public void TestAll()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites.Except(NotSupportedByBC).ToList());
        }

        [TestMethod]
        public void TestOne()
        {
            TestServer(CipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void Test_ECDSA()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Except(NotSupportedByBC)
                .Where(cs => cs.ToString().Contains("ECDSA"))
                .ToList());
        }

        [TestMethod]
        public void Test_RSA_CBC()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(cs => cs.ToString().Contains("RSA"))
                .Where(cs => !cs.ToString().Contains("ARIA"))
                .Where(cs => cs.ToString().Contains("CBC"))
                .ToList());
        }

        [TestMethod]
        public void Test_GCM()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Except(NotSupportedByBC)
                .Where(cs => cs.ToString().Contains("GCM"))
                .ToList());
        }

        [TestMethod]
        public void Test_CHACHA20()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Except(NotSupportedByBC)
                .Where(cs => cs.ToString().Contains("CHACHA20"))
                .ToList());
        }

        [TestMethod]
        public void Test_Camellia()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Except(NotSupportedByBC)
                .Where(cs => cs.ToString().Contains("CAMELLIA"))
                .ToList());
        }

        [TestMethod]
        public void Test_RSA_CCM()
        {
            TestCipherSuites(TlsStream.SupportedCipherSuites
                .Where(cs => cs.ToString().Contains("RSA"))
                .Where(cs => !cs.ToString().Contains("ARIA"))
                .Where(cs => cs.ToString().Contains("CCM"))
                .ToList());
        }

        private static void TestCipherSuites(IReadOnlyList<CipherSuite> ciphers)
        {
            ThreadPool.SetMaxThreads(4, 4);

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
                            SupportedCurves = new NamedCurve[] { NamedCurve.secp256r1, NamedCurve.secp521r1 }
                        };

                        var cert = ECcert ?
                            new X509Certificate("ecdsa-secp256r1.pfx", @"l(*qqqJ;q30[]e") :
                            new X509Certificate("rsa-cert.pfx", "hh87$-Jqo");
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
                        handler.Connect(new MyBCTlsClient(cs));

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
                if (_suites.Length == 0)
                    return base.GetCipherSuites();
                else
                    return _suites.Select(cs => (int)cs).ToArray();
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