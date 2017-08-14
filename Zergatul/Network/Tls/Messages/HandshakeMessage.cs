using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal class HandshakeMessage : ContentMessage
    {
        public HandshakeBody Body;

        private CipherSuiteBuilder _cipherSuite;

        public HandshakeMessage()
        {

        }

        public HandshakeMessage(CipherSuiteBuilder cipherSuite)
        {
            this._cipherSuite = cipherSuite;
        }

        public HandshakeMessage(HandshakeBody body)
        {
            this.Body = body;
        }

        public override void Read(BinaryReader reader)
        {
            var type = (HandshakeType)reader.ReadByte();
            var length = reader.ReadUInt24();

            using (reader.SetReadLimit(length))
                ReadHandshakeBody(type, reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteByte((byte)Body.Type);

            var content = new List<byte>();
            var contentWriter = new BinaryWriter(content);
            Body.WriteTo(contentWriter);

            writer.WriteUInt24(content.Count);
            writer.WriteBytes(content.ToArray());
        }

        private void ReadHandshakeBody(HandshakeType type, BinaryReader reader)
        {
            switch (type)
            {
                case HandshakeType.ClientHello:
                    Body = new ClientHello();
                    break;
                case HandshakeType.ServerHello:
                    Body = new ServerHello();
                    break;
                case HandshakeType.Certificate:
                    Body = new Certificate();
                    break;
                case HandshakeType.ServerKeyExchange:
                    Body = _cipherSuite.KeyExchange.ReadServerKeyExchange(reader);
                    return;
                case HandshakeType.ServerHelloDone:
                    Body = new ServerHelloDone();
                    break;
                case HandshakeType.ClientKeyExchange:
                    Body = _cipherSuite.KeyExchange.ReadClientKeyExchange(reader);
                    return;
                case HandshakeType.Finished:
                    Body = new Finished();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Body.Read(reader);
        }
    }
}