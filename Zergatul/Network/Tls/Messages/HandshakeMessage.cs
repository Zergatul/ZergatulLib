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

        private TlsStream _tlsStream;

        public HandshakeMessage(TlsStream stream)
        {
            this._tlsStream = stream;
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
            if (Body.Type == HandshakeType.ServerKeyExchange)
            {
                _tlsStream.SelectedCipher.KeyExchange.WriteServerKeyExchange((ServerKeyExchange)Body, contentWriter);
            }
            else if (Body.Type == HandshakeType.ClientKeyExchange)
            {
                _tlsStream.SelectedCipher.KeyExchange.WriteClientKeyExchange((ClientKeyExchange)Body, contentWriter);
            }
            else
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
                    Body = _tlsStream.SelectedCipher.KeyExchange.ReadServerKeyExchange(reader);
                    return;
                case HandshakeType.ServerHelloDone:
                    Body = new ServerHelloDone();
                    break;
                case HandshakeType.ClientKeyExchange:
                    Body = _tlsStream.SelectedCipher.KeyExchange.ReadClientKeyExchange(reader);
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