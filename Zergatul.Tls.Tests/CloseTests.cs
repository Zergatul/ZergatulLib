using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Net;
using Zergatul.Network.Tls;
using System.Threading;
using Zergatul.Cryptography.Certificate;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class CloseTests
    {
        [TestMethod]
        public void Test()
        {
            var tlsSettings = new TlsStreamSettings
            {
                SupportExtendedMasterSecret = true,
                CipherSuites = new CipherSuite[]
                {
                    CipherSuite.TLS_DH_Anon_WITH_AES_128_CBC_SHA
                }
            };

            var evt = new ManualResetEvent(false);

            ConnectionState serverState = ConnectionState.NoConnection;
            ConnectionState clientState;

            byte[] response = null;
            var serverThread = new Thread(() =>
            {
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
                        tlsServerStream.AuthenticateAsServer("localhost");
                        tlsServerStream.Write(new byte[100]);
                        response = new byte[100];
                        tlsServerStream.Read(response);
                        tlsServerStream.Close();
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

                buffer = new byte[100];
                tls.Read(buffer);

                tls.Write(new byte[100]);
                tls.Close();

                serverThread.Join();
            }
            finally
            {
                client.Close();
            }

            Assert.Fail();

            //Assert.IsTrue(serverState == ConnectionState.Closed);
            //Assert.IsTrue(clientState == ConnectionState.Closed);
        }
    }
}