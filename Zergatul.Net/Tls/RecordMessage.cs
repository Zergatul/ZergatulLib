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
        public ContentType RecordType;
        public ProtocolVersion Version;
        public ushort Length => (ushort)ContentMessages.Sum(m => m.Length);
        public List<ContentMessage> ContentMessages = new List<ContentMessage>();
        public bool Encrypted;

        private TlsStream _stream;

        public delegate void ContentMessageReadEventHandler(object sender, ContentMessageReadEventArgs e);
        public event ContentMessageReadEventHandler OnContentMessageRead;

        public RecordMessage(TlsStream stream)
        {
            this._stream = stream;
        }

        public void Read(BinaryReader reader)
        {
            RecordType = (ContentType)reader.ReadByte();
            Version = (ProtocolVersion)reader.ReadShort();

            var counter = reader.StartCounter(reader.ReadShort());

            if (RecordType == ContentType.Handshake)
                reader.StartTracking(_stream.HandshakeData);

            while (counter.CanRead)
            {
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        var message = new HandshakeMessage(_stream.SelectedCipher);
                        message.Read(reader);
                        ContentMessages.Add(message);
                        OnContentMessageRead?.Invoke(this, new ContentMessageReadEventArgs(message));
                        break;
                }
            }

            reader.StopTracking();
        }

        public void Write(Stream stream)
        {
            if (Encrypted)
                WriteEncrypted(stream);
            else
                WriteRaw(stream);
        }

        private void WriteRaw(Stream stream)
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);

            writer.WriteByte((byte)RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort(Length);

            if (RecordType == ContentType.Handshake)
                writer.StartTracking(_stream.HandshakeData);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(writer);

            writer.StopTracking();

            var buffer = list.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void WriteEncrypted(Stream stream)
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);

            var rawList = new List<byte>();
            var rawWriter = new BinaryWriter(rawList);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(rawWriter);
            var ciphertext = _stream.SelectedCipher.ProcessPlaintext(new ByteArray(rawList.ToArray()), _stream.EncodingSequenceNum);
            _stream.IncEncodingSequenceNum();

            writer.WriteByte((byte)RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort((ushort)ciphertext.Length);
            ciphertext.AddTo(list);

            var buffer = list.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}