using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Tests;
using Zergatul.Network.Tls;
using System.Threading;
using System.Linq;
using Zergatul.Security.Tls;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class TlsStaticTests
    {
        [TestMethod]
        public void NoExtendedMasterSecret()
        {
            var dual = new DualPeer();

            var tlsServer = new Network.Tls.TlsStream(dual.Peer1);
            tlsServer.Parameters.MinVersion = TlsVersion.Tls12;
            tlsServer.Parameters.MaxVersion = TlsVersion.Tls12;
            tlsServer.Parameters.CipherSuites = new[] { CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256 };
            tlsServer.Parameters.SecureRandom = new StaticSecureRandom(Enumerable.Repeat((byte)123, 10000));
            tlsServer.Parameters.Certificate = CipherSuiteTests.GetRSACert();
            tlsServer.Parameters.Host = "localhost";
            tlsServer.Parameters.ExtendedMasterSecret = false;

            var tlsClient = new Network.Tls.TlsStream(dual.Peer2);
            tlsClient.Parameters.MinVersion = TlsVersion.Tls12;
            tlsClient.Parameters.MaxVersion = TlsVersion.Tls12;
            tlsClient.Parameters.CipherSuites = new[] { CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256 };
            tlsClient.Parameters.SecureRandom = new StaticSecureRandom(Enumerable.Repeat((byte)159, 10000));
            tlsClient.Parameters.Host = "localhost";
            tlsClient.Parameters.ExtendedMasterSecret = false;

            var serverThread = new Thread(() =>
            {
                tlsServer.AuthenticateAsServer();
                tlsServer.Close();
            });
            serverThread.Start();

            tlsClient.AuthenticateAsClient();
            tlsClient.Close();

            serverThread.Join();

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.MasterSecret,
                BitHelper.HexToBytes("7d3b675e21665437f009a9d046e8222574d3a3dfcce6479b0e2e01e30125a25a411c2492a1ac8c889684154bf0e5ba8d")));
            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.MasterSecret,
                tlsClient.ConnectionInfo.MasterSecret));

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.Client.FinishedMessageHash,
                BitHelper.HexToBytes("07d3cf765b9237206575e3196319161152bd598944ec134546d7e44a97e4bd51")));

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.Server.FinishedMessageHash,
                BitHelper.HexToBytes("acfcf8ebf6c9de9881273f64b71d46e41a162cdda05303173761fed26b3550d9")));
        }

        [TestMethod]
        public void ExtendedMasterSecret()
        {
            var dual = new DualPeer();

            var tlsServer = new Network.Tls.TlsStream(dual.Peer1);
            tlsServer.Parameters.MinVersion = TlsVersion.Tls12;
            tlsServer.Parameters.MaxVersion = TlsVersion.Tls12;
            tlsServer.Parameters.CipherSuites = new[] { CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256 };
            tlsServer.Parameters.SecureRandom = new StaticSecureRandom(Enumerable.Repeat((byte)123, 10000));
            tlsServer.Parameters.Certificate = CipherSuiteTests.GetRSACert();
            tlsServer.Parameters.Host = "localhost";
            tlsServer.Parameters.ExtendedMasterSecret = true;

            var tlsClient = new Network.Tls.TlsStream(dual.Peer2);
            tlsClient.Parameters.MinVersion = TlsVersion.Tls12;
            tlsClient.Parameters.MaxVersion = TlsVersion.Tls12;
            tlsClient.Parameters.CipherSuites = new[] { CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256 };
            tlsClient.Parameters.SecureRandom = new StaticSecureRandom(Enumerable.Repeat((byte)159, 10000));
            tlsClient.Parameters.Host = "localhost";
            tlsClient.Parameters.ExtendedMasterSecret = true;

            var serverThread = new Thread(() =>
            {

                tlsServer.AuthenticateAsServer();
                tlsServer.Close();
            });
            serverThread.Start();

            tlsClient.AuthenticateAsClient();
            tlsClient.Close();

            serverThread.Join();

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.MasterSecret,
                BitHelper.HexToBytes("7a0a13999d21d89e6c7cb0562aed5b882cc85fc422c624a0fe85737a53d5690bd2ffd61937449457639e863e7dd588c9")));
            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.MasterSecret,
                tlsClient.ConnectionInfo.MasterSecret));

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.Client.FinishedMessageHash,
                BitHelper.HexToBytes("b292734e10606e16936a121b88dfc0fec471dbaf126dbfdb446380f60b1bc1f3")));

            Assert.IsTrue(ByteArray.Equals(
                tlsServer.ConnectionInfo.Server.FinishedMessageHash,
                BitHelper.HexToBytes("d66dc0984ccd91cecbae41e15e6fe6ddd3a0461eaa129a2748de612d0d29ddaa")));
        }
    }
}