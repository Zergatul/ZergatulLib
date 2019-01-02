using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Zergatul.IO;

namespace Zergatul.Network.Tls.Messages
{
    class RecordMessageStream
    {
        #region Constants

        // https://tools.ietf.org/html/rfc5246#section-6.2.1
        // Fragmentation
        /*
            The length (in bytes) of the following TLSPlaintext.fragment.
            The length MUST NOT exceed 2^14.
        */
        public const int PlaintextLimit = 16384;

        // https://tools.ietf.org/html/rfc5246#section-6.2.2
        // Record Compression and Decompression
        /*
            The length (in bytes) of the following TLSCompressed.fragment.
            The length MUST NOT exceed 2^14 + 1024.
        */
        public const int CompressedLimit = 16384 + 1024;

        // https://tools.ietf.org/html/rfc5246#section-6.2.3
        // Record Payload Protection
        /*
            The length (in bytes) of the following TLSCiphertext.fragment.
            The length MUST NOT exceed 2^14 + 2048.
        */
        public const int CiphertextLimit = 16384 + 2048;

        #endregion

        public ProtocolVersion Version;
        public bool ReadEncrypted;
        public bool WriteEncrypted;
        public CipherSuiteBuilder SelectedCipher;
        public ulong DecodingSequenceNum;
        public ulong EncodingSequenceNum;

        private Stream _stream;
        private List<byte> _handshakeBuffer;
        private BinaryReader _reader;

        private List<byte> _handshakeWriteBuffer;

        private ContentType _readContentType;
        private Stream _readStream;

        public RecordMessageStream(Stream stream, List<byte> handshakeBuffer)
        {
            this._stream = stream;
            this._handshakeBuffer = handshakeBuffer;

            _reader = new BinaryReader(_stream);
            DecodingSequenceNum = 0;
            EncodingSequenceNum = 0;
            _handshakeWriteBuffer = new List<byte>();
        }

        public ContentMessage ReadMessage()
        {
            if (_readStream == null)
            {
                _readContentType = (ContentType)_reader.ReadByte();
                var version = (ProtocolVersion)_reader.ReadShort();

                if (version != Version)
                    throw new TlsStreamException("Invalid TLS version");

                ushort length = _reader.ReadShort();


                if (ReadEncrypted) // TODO: optimize to use stream
                {
                    if (length > CiphertextLimit)
                        throw new RecordOverflowException();

                    byte[] buffer = SelectedCipher.SymmetricCipher.Decrypt(_readContentType, version, DecodingSequenceNum++, _reader.ReadBytes(length));
                    _readStream = new LimitedReadStream(new MemoryStream(buffer), buffer.Length);
                }
                else
                {
                    if (length > PlaintextLimit)
                        throw new RecordOverflowException();

                    _readStream = new LimitedReadStream(_stream, length);
                }
            }

            if (_readStream.CanRead)
            {
                var reader = new BinaryReader(_readStream);
                var tracking = new List<byte>();

                ContentMessage message;
                switch (_readContentType)
                {
                    case ContentType.Handshake:
                        message = new HandshakeMessage(SelectedCipher);
                        reader.StartTracking(tracking);
                        break;
                    case ContentType.ChangeCipherSpec:
                        message = new ChangeCipherSpec();
                        break;
                    case ContentType.Alert:
                        message = new Alert();
                        break;
                    case ContentType.ApplicationData:
                        message = new ApplicationData();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                message.Read(reader);
                if (_readContentType == ContentType.Handshake)
                {
                    _handshakeBuffer.AddRange(tracking);
                    reader.StopTracking();
                }

                if (!_readStream.CanRead)
                    _readStream = null;

                return message;
            }
            else
                throw new InvalidOperationException();
        }

        public void ReleaseHandshakeBuffer()
        {
            if (_handshakeWriteBuffer.Count > 0)
            {
                WriteMessage(new RecordMessage
                {
                    RecordType = ContentType.Handshake,
                    Version = Version,
                    ContentMessagesRaw = _handshakeWriteBuffer
                });

                _handshakeWriteBuffer.Clear();
            }
        }

        public void WriteHandshake(HandshakeMessage message)
        {
            var buffer = new List<byte>();
            var writer = new BinaryWriter(buffer);
            message.Write(writer);
            _handshakeWriteBuffer.AddRange(buffer);

            _handshakeBuffer.AddRange(buffer);
        }

        public void WriteChangeCipherSpec()
        {
            ReleaseHandshakeBuffer();

            WriteMessage(new RecordMessage
            {
                RecordType = ContentType.ChangeCipherSpec,
                Version = Version,
                ContentMessages = new List<ContentMessage>
                {
                    new ChangeCipherSpec()
                }
            });
        }

        public void WriteAlert(AlertLevel level, AlertDescription desc)
        {
            WriteMessage(new RecordMessage
            {
                RecordType = ContentType.Alert,
                Version = Version,
                ContentMessages = new List<ContentMessage>
                {
                    new Alert
                    {
                        Level = level,
                        Description = desc
                    }
                }
            });
        }

        public void WriteApplicationData(byte[] data)
        {
            WriteMessage(new RecordMessage
            {
                RecordType = ContentType.ApplicationData,
                Version = Version,
                ContentMessages = new List<ContentMessage>
                {
                    new ApplicationData
                    {
                        Data = data
                    }
                }
            });
        }

        private void WriteMessage(RecordMessage message)
        {
            List<byte> buffer;
            BinaryWriter writer;

            if (message.ContentMessagesRaw == null)
            {
                buffer = new List<byte>();
                writer = new BinaryWriter(buffer);
                foreach (var msg in message.ContentMessages)
                    msg.Write(writer);
                message.ContentMessagesRaw = buffer;
            }

            if (WriteEncrypted)
            {
                message.ContentMessagesRaw = SelectedCipher.SymmetricCipher.Encrypt(
                    message.RecordType,
                    Version,
                    EncodingSequenceNum++,
                    message.ContentMessagesRaw.ToArray()).ToList();
            }

            buffer = new List<byte>();
            writer = new BinaryWriter(buffer);

            writer.WriteByte((byte)message.RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort((ushort)message.ContentMessagesRaw.Count);
            buffer.AddRange(message.ContentMessagesRaw);

            _stream.Write(buffer.ToArray(), 0, buffer.Count);
            _stream.Flush();
        }
    }
}