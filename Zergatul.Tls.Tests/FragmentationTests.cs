using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Tls;
using System.Threading;
using Zergatul.Cryptography.Certificate;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class FragmentationTests
    {
        [TestMethod]
        public void Send1MB()
        {
            byte[] serverSend = Enumerable.Repeat((byte)1, 1024 * 1024).ToArray();
            byte[] clientSend = Enumerable.Repeat((byte)2, 1024 * 1024).ToArray();

            TestServerClient(serverSend, clientSend);
        }

        [TestMethod]
        public void Send100MB()
        {
            byte[] serverSend = Enumerable.Repeat((byte)3, 100 * 1024 * 1024).ToArray();
            byte[] clientSend = Enumerable.Repeat((byte)4, 100 * 1024 * 1024).ToArray();

            TestServerClient(serverSend, clientSend);
        }

        private static void TestServerClient(byte[] serverSend, byte[] clientSend)
        {
            var tlsSettings = new TlsStreamSettings
            {
                SupportExtendedMasterSecret = true,
                CertificateValidationOverride = c => true,
                CipherSuites = new CipherSuite[]
                {
                    CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA
                }
            };

            var evt = new ManualResetEvent(false);

            byte[] response = null;
            var serverThread = new Thread(() =>
            {
                X509Certificate cert = new X509Certificate(Settings.RSA4096CertName, Settings.RSA4096CertPwd);

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
                        tlsServerStream.Write(serverSend);
                        response = new byte[clientSend.Length];
                        tlsServerStream.Read(response);
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

                buffer = new byte[serverSend.Length];
                tls.Read(buffer);

                tls.Write(clientSend);
                serverThread.Join();
            }
            finally
            {
                client.Close();
            }

            Assert.IsTrue(ByteArray.Equals(buffer, serverSend));
            Assert.IsTrue(ByteArray.Equals(response, clientSend));
        }
    }
}
