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

            ms = new MemoryStream();
            hpack.Encode(ms, new List<Header>
            {
                new Header(":method", "GET"),
                new Header(":scheme", "http"),
                new Header(":path", "/"),
                new Header(":authority", "www.example.com"),
                new Header("cache-control", "no-cache")
            });
            Assert.IsTrue(BitHelper.BytesToHex(ms.ToArray()) == "828684be5886a8eb10649cbf");

            ms = new MemoryStream();
            hpack.Encode(ms, new List<Header>
            {
                new Header(":method", "GET"),
                new Header(":scheme", "https"),
                new Header(":path", "/index.html"),
                new Header(":authority", "www.example.com"),
                new Header("custom-key", "custom-value")
            });
            Assert.IsTrue(BitHelper.BytesToHex(ms.ToArray()) == "828785bf408825a849e95ba97d7f8925a849e95bb8e8b4bf");
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

        [TestMethod]
        public void RealTest1()
        {
            var hpack = new Hpack(4096);

            string hex = "188210011f278586b19272ff1f1091497ca589d34d1f649c7620a98386fc2b3d1f13a1fe5f038f10409424744ebc26df0bf2b6f4a165c2492c6f86e05b038db6d34d0bf91f299aa47e561cc58190b6cb800014fb50d5128b642eeb63be7a466a911f05012a109419085421621ea4d87a161d141fc2d495339e447f8cc5837fd63c10dffad7ab76ff109419085421621ea4d87a161d141fc2d3947216c47f012a109619085421621ea4d87a161d141fc2c4b0b216a498742384947420bf108f19085421621ea4d87a16a47e561cc58479c6800f108619085ad2b127a4bfaf6fd29c8d2267fa53898be2b3d895b91a44cff4a5f3f8aa8355d7e94dc3ee55af8d23108eaec3a4e43d1154598e930d263d5f0268321085a7d5761d27026832108725062d4988d5ff8979b5c42657101f5c42108a25062d498ac28e888d5f8979b5c42657101f5c42108b9a73a13120b677310b11ab8812e202e1644bb2ff10869a73a1311abf890bcd2ec817105f5c411082b1290237321083aed44f834d9697108ff2b0fa8e919964d83a9129eca4b27f8f7d4081f03ea014ae98e0a76f0d1f411f098aa47e561cc581a644007f1f1596df697e9403ea6a22541002f28066e005700d298b46ff1f1296d07abe9403ca6a22541002f2816ee005700d298b46ff1f0d84085f13bf";
            var headers = hpack.Decode(new MemoryStream(BitHelper.HexToBytes(hex)));

            Assert.IsTrue(headers.Count == 24);
            Assert.IsTrue(headers[0].ToString() == ":status: 200");
            Assert.IsTrue(headers[1].ToString() == "server: Apache");
            Assert.IsTrue(headers[2].ToString() == "content-type: text/html;charset=UTF-8");
            Assert.IsTrue(headers[3].ToString() == "etag: \"9068c20f1c727825919f58f136cdfb91:1506554442\"");
            Assert.IsTrue(headers[4].ToString() == "strict-transport-security: max-age=31536000 ; includeSubDomains");
            Assert.IsTrue(headers[5].ToString() == "access-control-allow-origin: *");
            Assert.IsTrue(headers[6].ToString() == "access-control-allow-methods: GET,HEAD,POST");
            Assert.IsTrue(headers[7].ToString() == "access-control-allow-headers: *");
            Assert.IsTrue(headers[8].ToString() == "access-control-allow-credentials: false");
            Assert.IsTrue(headers[9].ToString() == "access-control-max-age: 86400");
            Assert.IsTrue(headers[10].ToString() == "accept-ch: DPR, Width, Viewport-Width, Downlink, Save-Data");
            Assert.IsTrue(headers[11].ToString() == "protocol_negotiation: h2");
            Assert.IsTrue(headers[12].ToString() == "myproto: h2");
            Assert.IsTrue(headers[13].ToString() == "client_ip: 85.223.209.22");
            Assert.IsTrue(headers[14].ToString() == "client_real_ip: 85.223.209.22");
            Assert.IsTrue(headers[15].ToString() == "ghost_service_ip: 2.20.132.39");
            Assert.IsTrue(headers[16].ToString() == "ghost_ip: 184.30.219.21");
            Assert.IsTrue(headers[17].ToString() == "rtt: 72");
            Assert.IsTrue(headers[18].ToString() == "push: true");
            Assert.IsTrue(headers[19].ToString() == "x-akamai-transformed: 9 10909 0 pmb=mRUM,1");
            Assert.IsTrue(headers[20].ToString() == "cache-control: max-age=43200");
            Assert.IsTrue(headers[21].ToString() == "expires: Tue, 09 Oct 2018 03:02:04 GMT");
            Assert.IsTrue(headers[22].ToString() == "date: Mon, 08 Oct 2018 15:02:04 GMT");
            Assert.IsTrue(headers[23].ToString() == "content-length: 11927");
        }
    }
}