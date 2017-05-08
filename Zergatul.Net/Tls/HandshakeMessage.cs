using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class HandshakeMessage : ContentMessage
    {
        private TlsStream _stream;

        public HandshakeType MessageType;
        public override ushort Length => (ushort)(Body.Length + 4);
        public HandshakeBody Body;

        public HandshakeMessage(TlsStream stream)
        {
            this._stream = stream;
        }

        public override void Read(BinaryReader reader)
        {
            MessageType = (HandshakeType)reader.ReadByte();
            var length = reader.ReadUInt24();

            switch (MessageType)
            {
                case HandshakeType.ServerHello:
                    Body = new ServerHello();
                    break;
                case HandshakeType.Certificate:
                    Body = new Certificate();
                    break;
                case HandshakeType.ServerKeyExchange:
                    Body = new ServerKeyExchange(_stream);
                    break;
                case HandshakeType.ServerHelloDone:
                    Body = new ServerHelloDone();
                    break;
            }

            Body.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.WriteByte((byte)MessageType);
            writer.WriteUInt24(Body.Length);
            Body.WriteTo(writer);
        }
    }
}
