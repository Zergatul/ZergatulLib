using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls.CipherSuites;

namespace Zergatul.Network.Tls
{
    internal class HandshakeMessage : ContentMessage
    {
        private AbstractCipherSuite _cipher;

        public HandshakeBody Body;

        public HandshakeMessage(HandshakeBody body, AbstractCipherSuite cipher = null)
        {
            this.Body = body;
            this._cipher = cipher;
        }

        public override void Read(BinaryReader reader)
        {
            var type = (HandshakeType)reader.ReadByte();
            var length = reader.ReadUInt24();

            using (reader.SetReadLimit(length))
            {
                ResolveHandshakeBody(type);
                Body.Read(reader);
            }
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

        private void ResolveHandshakeBody(HandshakeType type)
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
                    Body = new ServerKeyExchange(_cipher);
                    break;
                case HandshakeType.ServerHelloDone:
                    Body = new ServerHelloDone();
                    break;
                case HandshakeType.ClientKeyExchange:
                    Body = new ClientKeyExchange(_cipher);
                    break;
                case HandshakeType.Finished:
                    Body = new Finished();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
