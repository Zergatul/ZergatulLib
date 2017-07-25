using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using Zergatul.Network.Tls;
using System.Threading;
using System.Text;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class CipherSuiteTests
    {
        private static string MessageToSend = "Hello World!!!";
        private static string MessageResponse = "OK";

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

        private static void TestCipherSuite(CipherSuite cipher)
        {
            var tlsSettings = new TlsStreamSettings
            {
                CipherSuites = new CipherSuite[]
                {
                    cipher
                }
            };

            byte[] response = null;
            var serverThread = new Thread(() =>
            {
                var cert = new X509Certificate2(Settings.CertName, Settings.CertPassword);

                var listener = new TcpListener(IPAddress.Any, Settings.Port);
                listener.Start();
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
