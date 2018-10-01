using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Network.Http;

namespace Zergatul.Network.Tests.Http
{
    [TestClass]
    public class HpackTests
    {
        [TestMethod]
        public void RFC7541_C_2_4_DecodeTest()
        {
            var hpack = new Hpack(0);
            var headers = hpack.Decode(new MemoryStream(new byte[] { 0x82 }));

            Assert.IsTrue(headers.Count == 1);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");

            hpack = new Hpack(0);
        }

        [TestMethod]
        public void RFC7541_C_2_4_EncodeTest()
        {
            var ms = new MemoryStream();
            var hpack = new Hpack(0);
            hpack.Encode(ms, new List<Header>
            {
                new Header(":method", "GET")
            });

            Assert.IsTrue(BitHelper.BytesToHex(ms.ToArray()) == "82");
        }

        [TestMethod]
        public void RFC7541_C_3_Test()
        {
            var hpack = new Hpack(0x1000);

            string hex = "828684410f7777772e6578616d706c652e636f6d";
            var headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "http");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");

            hex = "828684be58086e6f2d6361636865";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "http");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "cache-control");
            Assert.IsTrue(headers[4].Value == "no-cache");

            hex = "828785bf400a637573746f6d2d6b65790c637573746f6d2d76616c7565";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "https");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/index.html");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "custom-key");
            Assert.IsTrue(headers[4].Value == "custom-value");
        }

        [TestMethod]
        public void RFC7541_C_4_DecodeTest()
        {
            var hpack = new Hpack(0x1000);

            string hex = "828684418cf1e3c2e5f23a6ba0ab90f4ff";
            var headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "http");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");

            hex = "828684be5886a8eb10649cbf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));
            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "http");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "cache-control");
            Assert.IsTrue(headers[4].Value == "no-cache");

            hex = "828785bf408825a849e95ba97d7f8925a849e95bb8e8b4bf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "https");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/index.html");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "custom-key");
            Assert.IsTrue(headers[4].Value == "custom-value");
        }

        [TestMethod]
        public void RFC7541_C_4_EncodeTest()
        {
            var ms = new MemoryStream();
            var hpack = new Hpack(0x1000);
            hpack.Encode(ms, new List<Header>
            {
                new Header(":method", "GET"),
                new Header(":scheme", "http"),
                new Header(":path", "/"),
                new Header(":authority", "www.example.com")
            });

            Assert.IsTrue(BitHelper.BytesToHex(ms.ToArray()) == "828684418cf1e3c2e5f23a6ba0ab90f4ff");

            /*
            hex = "828684be5886a8eb10649cbf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));
            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "http");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "cache-control");
            Assert.IsTrue(headers[4].Value == "no-cache");

            hex = "828785bf408825a849e95ba97d7f8925a849e95bb8e8b4bf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 5);
            Assert.IsTrue(headers[0].Name == ":method");
            Assert.IsTrue(headers[0].Value == "GET");
            Assert.IsTrue(headers[1].Name == ":scheme");
            Assert.IsTrue(headers[1].Value == "https");
            Assert.IsTrue(headers[2].Name == ":path");
            Assert.IsTrue(headers[2].Value == "/index.html");
            Assert.IsTrue(headers[3].Name == ":authority");
            Assert.IsTrue(headers[3].Value == "www.example.com");
            Assert.IsTrue(headers[4].Name == "custom-key");
            Assert.IsTrue(headers[4].Value == "custom-value");*/
        }

        [TestMethod]
        public void RFC7541_C_5_Test()
        {
            var hpack = new Hpack(256);

            string hex = "4803333032580770726976617465611d4d6f6e2c203231204f637420323031332032303a31333a323120474d546e1768747470733a2f2f7777772e6578616d706c652e636f6d";
            var headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "302");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:21 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(hpack.DynamicTableSize == 222);

            hex = "4803333037c1c0bf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "307");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:21 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(hpack.DynamicTableSize == 222);

            hex = "88c1611d4d6f6e2c203231204f637420323031332032303a31333a323220474d54c05a04677a69707738666f6f3d4153444a4b48514b425a584f5157454f50495541585157454f49553b206d61782d6167653d333630303b2076657273696f6e3d31";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 6);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "200");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:22 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(headers[4].Name == "content-encoding");
            Assert.IsTrue(headers[4].Value == "gzip");
            Assert.IsTrue(headers[5].Name == "set-cookie");
            Assert.IsTrue(headers[5].Value == "foo=ASDJKHQKBZXOQWEOPIUAXQWEOIU; max-age=3600; version=1");
            Assert.IsTrue(hpack.DynamicTableSize == 215);
        }

        [TestMethod]
        public void RFC7541_C_6_Test()
        {
            var hpack = new Hpack(256);

            string hex = "488264025885aec3771a4b6196d07abe941054d444a8200595040b8166e082a62d1bff6e919d29ad171863c78f0b97c8e9ae82ae43d3";
            var headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "302");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:21 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(hpack.DynamicTableSize == 222);

            hex = "4883640effc1c0bf";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 4);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "307");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:21 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(hpack.DynamicTableSize == 222);

            hex = "88c16196d07abe941054d444a8200595040b8166e084a62d1bffc05a839bd9ab77ad94e7821dd7f2e6c7b335dfdfcd5b3960d5af27087f3672c1ab270fb5291f9587316065c003ed4ee5b1063d5007";
            headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 6);
            Assert.IsTrue(headers[0].Name == ":status");
            Assert.IsTrue(headers[0].Value == "200");
            Assert.IsTrue(headers[1].Name == "cache-control");
            Assert.IsTrue(headers[1].Value == "private");
            Assert.IsTrue(headers[2].Name == "date");
            Assert.IsTrue(headers[2].Value == "Mon, 21 Oct 2013 20:13:22 GMT");
            Assert.IsTrue(headers[3].Name == "location");
            Assert.IsTrue(headers[3].Value == "https://www.example.com");
            Assert.IsTrue(headers[4].Name == "content-encoding");
            Assert.IsTrue(headers[4].Value == "gzip");
            Assert.IsTrue(headers[5].Name == "set-cookie");
            Assert.IsTrue(headers[5].Value == "foo=ASDJKHQKBZXOQWEOPIUAXQWEOIU; max-age=3600; version=1");
            Assert.IsTrue(hpack.DynamicTableSize == 215);
        }
    }
}