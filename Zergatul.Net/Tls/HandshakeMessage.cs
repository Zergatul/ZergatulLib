using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Net.Tls.CipherSuites;

namespace Zergatul.Net.Tls
{
    internal class HandshakeMessage : ContentMessage
    {
        private CipherSuite _cipher;

        public HandshakeType MessageType;
        public override ushort Length => (ushort)(Body.Length + 4);
        public HandshakeBody Body;

        public HandshakeMessage(CipherSuite cipher = null)
        {
            this._cipher = cipher;
        }

        public override void Read(BinaryReader reader)
        {
            MessageType = (HandshakeType)reader.ReadByte();
            var length = reader.ReadUInt24();

            using (reader.SetReadLimit(length))
            {
                ResolveHandshakeBody();
                Body.Read(reader);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteByte((byte)MessageType);
            writer.WriteUInt24(Body.Length);
            Body.WriteTo(writer);
        }

        private void ResolveHandshakeBody()
        {
            switch (MessageType)
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
