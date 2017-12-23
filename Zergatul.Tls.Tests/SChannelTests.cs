using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using Zergatul.Network.Tls;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Zergatul.Cryptography.Asymmetric;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class SChannelTests
    {
        /*[TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA);
        }

        [Ignore]
        [TestMethod]
        public void TLS_DHE_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA);
        }

        [TestMethod]
        public void TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_RSA_WITH_AES_128_CBC_SHA256);
        }

        [TestMethod]
        public void TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA);
        }

        /*[TestMethod]
        public void TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA()
        {
            TestCipherSuite(CipherSuite.TLS_DHE_RSA_WITH_3DES_EDE_CBC_SHA);
        }*/

        private static void TestCipherSuite(CipherSuite cs)
        {
            using (PutCipherSuiteToTop(cs))
            {
                TestClient(cs);
                TestServer(cs);
            }
        }

        private static void TestServer(CipherSuite cs)
        {
            bool ECcert = cs.ToString().Contains("ECDSA");

            string serverWrite = "Hello World!";
            string clientRead = null;

            var clientReadEvent = new ManualResetEvent(false);

            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();
                try
                {
                    var client = listener.AcceptTcpClient();
                    try
                    {
                        var tlsStream = new TlsStream(client.GetStream());
                        tlsStream.Settings = new TlsStreamSettings
                        {
                            SupportExtendedMasterSecret = true,
                            DHParameters = DiffieHellmanParameters.Group2, // 1024 bit
                            CipherSuites = new CipherSuite[] { cs },
                            SupportedCurves = new NamedGroup[]
                            {
                                NamedGroup.secp256r1,
                                NamedGroup.secp384r1,
                                NamedGroup.secp521r1
                            }
                        };

                        var cert = ECcert ?
                            new Cryptography.Certificate.X509Certificate("ecdsa-cert.pfx", @"\7FoIK*f1{\q") :
                            new Cryptography.Certificate.X509Certificate("rsa-cert.pfx", "hh87$-Jqo");

                        tlsStream.AuthenticateAsServer("localhost", cert);
                        tlsStream.Write(Encoding.ASCII.GetBytes(serverWrite));

                        clientReadEvent.WaitOne();
                    }
                    finally
                    {
                        client.Close();
                    }
                }
                finally
                {
                    listener.Stop();
                }
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);
                var ssl = new System.Net.Security.SslStream(tcp.GetStream(), true, (p1, p2, p3, p4) => true);
                try
                {
                    ssl.AuthenticateAsClient("localhost");

                    byte[] buffer = new byte[serverWrite.Length];
                    ssl.Read(buffer, 0, buffer.Length);

                    clientRead = Encoding.ASCII.GetString(buffer);
                }
                finally
                {
                    clientReadEvent.Set();
                    ssl.Close();
                    tcp.Close();
                }
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Assert.IsTrue(serverWrite == clientRead);
        }

        private static void TestClient(CipherSuite cs)
        {
            bool ECcert = cs.ToString().Contains("ECDSA");

            string serverWrite = "Hello World!";
            string clientRead = null;

            var clientReadEvent = new ManualResetEvent(false);

            var serverThread = new Thread(() =>
            {
                var listener = new TcpListener(IPAddress.Any, 32028);
                listener.Start();
                try
                {
                    var client = listener.AcceptTcpClient();
                    var sslStream = new System.Net.Security.SslStream(client.GetStream(), false);
                    try
                    {
                        var cert = ECcert ?
                            new System.Security.Cryptography.X509Certificates.X509Certificate2("ecdsa-cert.pfx", @"\7FoIK*f1{\q") :
                            new System.Security.Cryptography.X509Certificates.X509Certificate2("rsa-cert.pfx", "hh87$-Jqo");

                        sslStream.AuthenticateAsServer(cert, false, System.Security.Authentication.SslProtocols.Tls12, false);

                        sslStream.Write(Encoding.ASCII.GetBytes(serverWrite));

                        clientReadEvent.WaitOne();
                    }
                    finally
                    {
                        sslStream.Close();
                        client.Close();
                    }
                }
                finally
                {
                    listener.Stop();
                }
            });

            var clientThread = new Thread(() =>
            {
                var tcp = new TcpClient("localhost", 32028);
                var tlsStream = new TlsStream(tcp.GetStream());
                tlsStream.Settings = new TlsStreamSettings
                {
                    SupportExtendedMasterSecret = true,
                    CipherSuites = new CipherSuite[] { cs },
                    SupportedCurves = new NamedGroup[]
                    {
                        NamedGroup.secp256r1,
                        NamedGroup.secp384r1,
                        NamedGroup.secp521r1
                    }
                };
                try
                {
                    tlsStream.AuthenticateAsClient("localhost");

                    byte[] buffer = new byte[serverWrite.Length];
                    tlsStream.Read(buffer, 0, buffer.Length);

                    clientRead = Encoding.ASCII.GetString(buffer);
                }
                finally
                {
                    clientReadEvent.Set();
                    tcp.Close();
                }
            });

            serverThread.Start();
            clientThread.Start();

            serverThread.Join();
            clientThread.Join();

            Assert.IsTrue(serverWrite == clientRead);
        }

        private static List<string> GetBCryptCipherSuites()
        {
            uint cbBuffer = 0;
            IntPtr ppBuffer = IntPtr.Zero;
            uint status = BCrypt.EnumContextFunctions(
                BCrypt.CRYPT_LOCAL,
                "SSL",
                BCrypt.NCRYPT_SCHANNEL_INTERFACE,
                ref cbBuffer,
                ref ppBuffer);

            var ciphers = new List<string>();
            if (status == 0)
            {
                BCrypt.CRYPT_CONTEXT_FUNCTIONS functions = (BCrypt.CRYPT_CONTEXT_FUNCTIONS)Marshal.PtrToStructure(ppBuffer, typeof(BCrypt.CRYPT_CONTEXT_FUNCTIONS));

                IntPtr pStr = functions.rgpszFunctions;
                for (int i = 0; i < functions.cFunctions; i++)
                {
                    ciphers.Add(Marshal.PtrToStringUni(Marshal.ReadIntPtr(pStr)));
                    pStr += IntPtr.Size;
                }
                BCrypt.FreeBuffer(ppBuffer);
            }
            else
                throw new InvalidOperationException();

            return ciphers;
        }

        private static IDisposable PutCipherSuiteToTop(CipherSuite cs)
        {
            var list = GetBCryptCipherSuites();
            if (list.Contains(cs.ToString()))
            {
                int position = list.IndexOf(cs.ToString());
                BCrypt.AddContextFunction(
                    BCrypt.CRYPT_LOCAL,
                    "SSL",
                    BCrypt.NCRYPT_SCHANNEL_INTERFACE,
                    cs.ToString(),
                    0);

                return new SimpleDispose(() =>
                {
                    BCrypt.AddContextFunction(
                        BCrypt.CRYPT_LOCAL,
                        "SSL",
                        BCrypt.NCRYPT_SCHANNEL_INTERFACE,
                        cs.ToString(),
                        (uint)position);
                });
            }
            else
            {
                BCrypt.AddContextFunction(
                    BCrypt.CRYPT_LOCAL,
                    "SSL",
                    BCrypt.NCRYPT_SCHANNEL_INTERFACE,
                    cs.ToString(),
                    0);

                return new SimpleDispose(() =>
                {
                    BCrypt.RemoveContextFunction(
                        BCrypt.CRYPT_LOCAL,
                        "SSL",
                        BCrypt.NCRYPT_SCHANNEL_INTERFACE,
                        cs.ToString());
                });
            }
        }

        private class SimpleDispose : IDisposable
        {
            private Action _action;

            public SimpleDispose(Action action)
            {
                this._action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}