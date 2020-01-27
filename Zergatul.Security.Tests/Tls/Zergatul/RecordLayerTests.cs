using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zergatul.Security.Zergatul;
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
                var stream = new MemoryStream(new byte[] { 0x16, 0x01 });
                var rl = new RecordLayer(stream, new StateMachine());
                byte[] buffer = new byte[1024];
                rl.ReadNext(buffer, 1);
            }, e => e.ErrorCode == ErrorCodes.UnexpectedEndOfStream.Code);
        }

        [TestMethod]
        public void EndOfStreamDataTest()
        {
            Assert2.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var stream = new MemoryStream(new byte[] { 0x16, 0x03, 0x03, 0x02, 0x00, 0x00 });
                var rl = new RecordLayer(stream, new StateMachine());
                byte[] buffer = new byte[1024];
                rl.ReadNext(buffer, 2);
            }, e => e.ErrorCode == ErrorCodes.UnexpectedEndOfStream.Code);
        }

        [TestMethod]
        public void InvalidContentTypeTest()
        {
            Assert2.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var stream = new MemoryStream(new byte[] { 0x00, 0x03, 0x03, 0x01, 0x00 });
                var rl = new RecordLayer(stream, new StateMachine());
                byte[] buffer = new byte[1024];
                rl.ReadNext(buffer, 1);
            }, e => e.ErrorCode == ErrorCodes.InvalidContentType.Code);
        }

        [TestMethod]
        public void UnexpectedHandshakeMessageTest()
        {
            Assert2.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var stream = new MemoryStream(new byte[] { 0x16, 0x03, 0x03, 0x02, 0x00, 0x00 });
                var stateMachine = new StateMachine();
                stateMachine.HState = HandshakeState.Finished;
                var rl = new RecordLayer(stream, stateMachine);
                byte[] buffer = new byte[1024];
                rl.ReadNext(buffer, 2);
            }, e => e.ErrorCode == ErrorCodes.UnexpectedHandshakeMessage.Code);
        }

        [TestMethod]
        public void InvalidLengthTest()
        {
            Assert2.ThrowsException<TlsStreamRecordLayerException>(() =>
            {
                var stream = new MemoryStream(new byte[] { 0x16, 0x03, 0x03, 0x01, 0x40 });
                var rl = new RecordLayer(stream, new StateMachine());
                byte[] buffer = new byte[1024];
                rl.ReadNext(buffer, 1);
            }, e => e.ErrorCode == ErrorCodes.RecordLayerOverflow.Code);
        }
    }
}