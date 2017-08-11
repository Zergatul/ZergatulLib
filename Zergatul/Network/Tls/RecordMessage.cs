using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = System.IO.Stream;

namespace Zergatul.Network.Tls
{
    internal class RecordMessage
    {
        // https://tools.ietf.org/html/rfc5246#section-6.2.1
        // Fragmentation
        /*
            The length (in bytes) of the following TLSPlaintext.fragment.
            The length MUST NOT exceed 2^14.
        */
        private const int PlaintextLimit = 16384;

        // https://tools.ietf.org/html/rfc5246#section-6.2.2
        // Record Compression and Decompression
        /*
            The length (in bytes) of the following TLSCompressed.fragment.
            The length MUST NOT exceed 2^14 + 1024.
        */
        private const int CompressedLimit = 16384 + 1024;

        // https://tools.ietf.org/html/rfc5246#section-6.2.3
        // Record Payload Protection
        /*
            The length (in bytes) of the following TLSCiphertext.fragment.
            The length MUST NOT exceed 2^14 + 2048.
        */
        private const int CiphertextLimit = 16384 + 2048;

        public ContentType RecordType;
        public ProtocolVersion Version;
        public List<ContentMessage> ContentMessages = new List<ContentMessage>();

        private TlsStream _tlsStream;

        public delegate void ContentMessageEventHandler(object sender, ContentMessageEventArgs e);
        public event ContentMessageEventHandler OnContentMessage;

        public RecordMessage(TlsStream tlsStream)
        {
            this._tlsStream = tlsStream;
        }

        public void Read(BinaryReader reader)
        {
            if (_tlsStream.ReadEncrypted)
                ReadEncrypted(reader);
            else
                ReadRaw(reader);
        }

        private void ReadRaw(BinaryReader reader)
        {
            RecordType = (ContentType)reader.ReadByte();
            Version = (ProtocolVersion)reader.ReadShort();

            var counter = reader.StartCounter(reader.ReadShort());

            if (RecordType == ContentType.Handshake)
                reader.StartTracking(_tlsStream.HandshakeData);

            while (counter.CanRead)
            {
                ContentMessage message;
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        message = new HandshakeMessage(null, _tlsStream.SelectedCipher);
                        break;
                    case ContentType.ChangeCipherSpec:
                        message = new ChangeCipherSpec();
                        break;
                    case ContentType.Alert:
                        message = new Alert();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                message.Read(reader);
                ContentMessages.Add(message);
                OnContentMessage?.Invoke(this, new ContentMessageEventArgs(message, true, _tlsStream.Role == Role.Client));
            }

            if (RecordType == ContentType.Handshake)
                reader.StopTracking();
        }

        private void ReadEncrypted(BinaryReader reader)
        {
            RecordType = (ContentType)reader.ReadByte();
            Version = (ProtocolVersion)reader.ReadShort();

            var data = reader.ReadBytes(reader.ReadShort());

            var plaintext = _tlsStream.SelectedCipher.Decode(new ByteArray(data), RecordType, Version, _tlsStream.DecodingSequenceNum);
            _tlsStream.IncDecodingSequenceNum();

            var decodedReader = new BinaryReader(plaintext.Array);
            var counter = decodedReader.StartCounter(plaintext.Length);
            decodedReader.SetReadLimit(plaintext.Length);

            if (RecordType == ContentType.Handshake)
                decodedReader.StartTracking(_tlsStream.HandshakeData);

            while (counter.CanRead)
            {
                ContentMessage message;
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        message = new HandshakeMessage(null, _tlsStream.SelectedCipher);
                        break;
                    case ContentType.ChangeCipherSpec:
                        message = new ChangeCipherSpec();
                        break;
                    case ContentType.ApplicationData:
                        message = new ApplicationData();
                        break;
                    case ContentType.Alert:
                        message = new Alert();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                message.Read(decodedReader);
                ContentMessages.Add(message);
                OnContentMessage?.Invoke(this, new ContentMessageEventArgs(message, true, _tlsStream.Role == Role.Client));
            }

            if (RecordType == ContentType.Handshake)
                decodedReader.StopTracking();
        }

        public void Write(Stream stream)
        {
            if (_tlsStream.WriteEncrypted && RecordType != ContentType.ChangeCipherSpec)
                WriteEncrypted(stream);
            else
                WriteRaw(stream);

            foreach (var message in ContentMessages)
                OnContentMessage?.Invoke(this, new ContentMessageEventArgs(message, false, _tlsStream.Role == Role.Server));
        }

        private void WriteRaw(Stream stream)
        {
            var header = new List<byte>();
            var headerWriter = new BinaryWriter(header);

            headerWriter.WriteByte((byte)RecordType);
            headerWriter.WriteShort((ushort)Version);

            // write to separate writer, so we can send Length
            var content = new List<byte>();
            var contentWriter = new BinaryWriter(content);

            // RFC 5246 // Page 63
            /*
                Note: ChangeCipherSpec messages, alerts, and any other record types
                are not handshake messages and are not included in the hash
                computations.
            */
            if (RecordType == ContentType.Handshake)
                contentWriter.StartTracking(_tlsStream.HandshakeData);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(contentWriter);

            contentWriter.StopTracking();

            headerWriter.WriteShort((ushort)content.Count);

            var buffer = header.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            buffer = content.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        private void WriteEncrypted(Stream stream)
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);

            var rawList = new List<byte>();
            var rawWriter = new BinaryWriter(rawList);

            if (RecordType == ContentType.Handshake)
                rawWriter.StartTracking(_tlsStream.HandshakeData);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(rawWriter);

            rawWriter.StopTracking();

            var ciphertext = _tlsStream.SelectedCipher.Encode(new ByteArray(rawList.ToArray()), RecordType, Version, _tlsStream.EncodingSequenceNum);
            _tlsStream.IncEncodingSequenceNum();

            writer.WriteByte((byte)RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort((ushort)ciphertext.Length);
            list.AddRange(ciphertext.Array);

            var buffer = list.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}