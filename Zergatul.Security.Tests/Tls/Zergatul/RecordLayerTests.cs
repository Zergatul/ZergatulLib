using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul.Tls;
using Zergatul.Test.Common;

namespace Zergatul.Security.Tests.Tls.Zergatul
{
    [TestClass]
    public class RecordLayerTests
    {
        [TestMethod]
        public void EndOfStreamHeaderTest()
        {
            Assert2.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var stream = new Security.Zergatul.Tls.TlsStream(new MemoryStream(new byte[] { 0x16, 0x01 }));
                var rl = new RecordLayer();
                rl.Init(stream);
                rl.ReadNext();
            }, e => true);
        }

        [TestMethod]
        public void EndOfStreamDataTest()
        {
            Assert.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var rl = new RecordLayer();
                rl.Init();
                var ms = new MemoryStream(new byte[] { 0x16, 0x03, 0x03, 0x02, 0x00, 0x00 });
                while (true)
                {
                    rl.ReadNext(ms);
                    if (rl.EndOfRecordMessage)
                        return;
                    if (rl.HasFullHigherProtocolMessage)
                        return;
                }
            });
        }

        [TestMethod]
        public void InvalidContentTypeTest()
        {
            Assert.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var rl = new RecordLayer();
                rl.Init();
                rl.ReadNext(new MemoryStream(new byte[] { 0x00, 0x03, 0x03, 0x01, 0x00 }));
            });
        }

        [TestMethod]
        public void InvalidLengthTest()
        {
            Assert.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var rl = new RecordLayer();
                rl.Init();
                rl.ReadNext(new MemoryStream(new byte[] { 0x16, 0x03, 0x03, 0x01, 0x40 }));
            });
        }
    }
}