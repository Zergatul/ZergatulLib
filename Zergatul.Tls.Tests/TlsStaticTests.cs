using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Tests;
using Zergatul.Network.Tls;
using System.Threading;
using System.Linq;

namespace Zergatul.Tls.Tests
{
    [TestClass]
    public class TlsStaticTests
    {
        [TestMethod]
        public void NoExtendedMasterSecret()
        {
            var tlsSettings = new TlsStreamSettings
            {
                SupportExtendedMasterSecret = false,
                CipherSuites = new CipherSuite[]
                {
                    CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256
                },
                ServerCertificateValidationOverride = c => true,
                GetRandom = () => new byte[32]
            };

            var dual = new DualPeer();

            TlsStream tlsServer = new TlsStream(dual.Peer1,
                ProtocolVersion.Tls12,
                new StaticRandom(Enumerable.Repeat((byte)123, 10000)));
            tlsServer.Settings = tlsSettings;

            var tlsClient = new TlsStream(dual.Peer2,
                ProtocolVersion.Tls12,
                new StaticRandom(Enumerable.Repeat((byte)159, 10000)));
            tlsClient.Settings = tlsSettings;

            var serverThread = new Thread(() =>
            {
                
                tlsServer.AuthenticateAsServer("localhost", CipherSuiteTests.GetRSACert());
                tlsServer.Close();
            });
            serverThread.Start();

            tlsClient.AuthenticateAsClient("localhost");
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
            var tlsSettings = new TlsStreamSettings
            {
                SupportExtendedMasterSecret = true,
                CipherSuites = new CipherSuite[]
                {
                    CipherSuite.TLS_DHE_RSA_WITH_AES_128_CBC_SHA256
                },
                ServerCertificateValidationOverride = c => true,
                GetRandom = () => new byte[32]
            };

            var dual = new DualPeer();

            TlsStream tlsServer = new TlsStream(dual.Peer1,
                ProtocolVersion.Tls12,
                new StaticRandom(Enumerable.Repeat((byte)123, 10000)));
            tlsServer.Settings = tlsSettings;

            var tlsClient = new TlsStream(dual.Peer2,
                ProtocolVersion.Tls12,
                new StaticRandom(Enumerable.Repeat((byte)159, 10000)));
            tlsClient.Settings = tlsSettings;

            var serverThread = new Thread(() =>
            {

                tlsServer.AuthenticateAsServer("localhost", CipherSuiteTests.GetRSACert());
                tlsServer.Close();
            });
            serverThread.Start();

            tlsClient.AuthenticateAsClient("localhost");
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