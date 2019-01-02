using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Zergatul.IO;
using Zergatul.Network.WebSocket;
using Zergatul.Tests;

namespace Zergatul.Network.Tests.WebSocket
{
    [TestClass]
    public class WebSocketClientTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            var tcp = new DualPeer();
            var server = tcp.Peer1;
            var client = tcp.Peer2;

            string response =
                "HTTP/1.1 101 Switching protocols\r\n" +
                "Connection: Upgrade\r\n" +
                "Upgrade: websocket\r\n" +
                "Sec-WebSocket-Accept: cW0HMpChSOllUrDZnf5AIF3ENuY=\r\n" +
                "\r\n";
            byte[] buffer = Encoding.ASCII.GetBytes(response);
            server.Write(buffer, 0, buffer.Length);

            string frame = "8186ABCDEF12" + "f8a89d64cebf";
            buffer = BitHelper.HexToBytes(frame);
            server.Write(buffer, 0, buffer.Length);

            var mock = new Mock<NetworkProvider>();
            mock.Setup(p => p.GetTcpStream("example.com", 80, null)).Returns(client);

            var ws = new WebSocketClient(new Uri("ws://example.com/service"), mock.Object);
            using (StaticRandomTestProvider.Use("101112131415161718191A1B1C1D1E1F"))
                ws.Open();
            ws.SendText("Client");
            var msg = ws.ReadNextMessage();
            Assert.IsTrue(msg.Text == "Server");

            var sr = new StreamReader(new LimitedReadStream(server, 157), Encoding.ASCII);
            string line = sr.ReadLine();
            Assert.IsTrue(line == "GET /service HTTP/1.1");

            var headers = new List<string>();
            while (true)
            {
                line = sr.ReadLine();
                if (line == null)
                    Assert.Fail();
                if (line == "")
                    break;
                headers.Add(line);
            }
            headers.Sort();
            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0] == "Connection: Upgrade");
            Assert.IsTrue(headers[1] == "Host: example.com");
            Assert.IsTrue(headers[2] == "Sec-WebSocket-Key: EBESExQVFhcYGRobHB0eHw==");
            Assert.IsTrue(headers[3] == "Sec-WebSocket-Version: 13");
            Assert.IsTrue(headers[4] == "Upgrade: websocket");

            var list = new List<byte>();
            int @byte;
            while ((@byte = server.ReadByte()) != -1)
                list.Add((byte)@byte);

            Assert.IsTrue(list.Count == 12);
            for (int i = 0; i < 6; i++)
                list[6 + i] ^= list[2 + (i & 0x03)];
            Assert.IsTrue(list[0] == 0x81);
            Assert.IsTrue(list[1] == 0x86);
            Assert.IsTrue(Encoding.ASCII.GetString(list.ToArray(), 6, 6) == "Client");

            mock.Verify(p => p.GetTcpStream("example.com", 80, null), Times.Once());
        }
    }
}