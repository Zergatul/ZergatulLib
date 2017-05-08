using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = System.IO.Stream;

namespace Zergatul.Net.Tls
{
    internal class RecordMessage
    {
        private TlsStream _stream;

        public ContentType RecordType;
        public ProtocolVersion Version;
        public ushort Length => (ushort)ContentMessages.Sum(m => m.Length);
        public List<ContentMessage> ContentMessages = new List<ContentMessage>();

        public RecordMessage(TlsStream stream)
        {
            this._stream = stream;
        }

        public void Read(BinaryReader reader)
        {
            RecordType = (ContentType)reader.ReadByte();
            Version = (ProtocolVersion)reader.ReadShort();

            var counter = reader.StartCounter(reader.ReadShort());
            while (counter.CanRead)
            {
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        ContentMessages.Add(new HandshakeMessage(_stream));
                        ContentMessages.Last().Read(reader);
                        _stream.OnContentMessage(ContentMessages.Last());
                        break;
                }
            }
        }

        public void Write(Stream stream)
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);

            writer.WriteByte((byte)RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort(Length);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(writer);

            var buffer = list.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}