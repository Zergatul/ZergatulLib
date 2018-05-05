﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream = System.IO.Stream;

namespace Zergatul.Network.Tls.Messages
{
    internal class RecordMessage
    {
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

        public ContentType RecordType;
        public ProtocolVersion Version;

        public List<ContentMessage> ContentMessages;

        private TlsStream _tlsStream;
        private ReadCounter _counter;
        private BinaryReader _decodedReader;

        public RecordMessage(TlsStream tlsStream)
        {
            this._tlsStream = tlsStream;
        }

        public ContentMessage ReadNext(BinaryReader reader)
        {
            if (_tlsStream.ReadEncrypted)
                return ReadEncrypted(reader);
            else
                return ReadRaw(reader);
        }

        private ContentMessage ReadRaw(BinaryReader reader)
        {
            if (_counter == null)
            {
                RecordType = (ContentType)reader.ReadByte();
                Version = (ProtocolVersion)reader.ReadShort();

                ushort length = reader.ReadShort();
                if (length > PlaintextLimit)
                    throw new RecordOverflowException();

                _counter = reader.StartCounter(length);

                if (RecordType == ContentType.Handshake)
                    reader.StartTracking(_tlsStream.SecurityParameters.HandshakeData);
            }

            if (_counter.CanRead)
            {
                ContentMessage message;
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        message = new HandshakeMessage(_tlsStream);
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

                if (!_counter.CanRead)
                {
                    if (RecordType == ContentType.Handshake)
                        reader.StopTracking();
                    _counter = null;
                }

                return message;
            }
            else
                throw new InvalidOperationException();
        }

        private ContentMessage ReadEncrypted(BinaryReader reader)
        {
            if (_counter == null)
            {
                RecordType = (ContentType)reader.ReadByte();
                Version = (ProtocolVersion)reader.ReadShort();

                ushort length = reader.ReadShort();
                if (length > CiphertextLimit)
                    throw new RecordOverflowException();

                var data = reader.ReadBytes(length);

                var plaintext = _tlsStream.SelectedCipher.SymmetricCipher.Decrypt(RecordType, Version, _tlsStream.DecodingSequenceNum, data);
                if (plaintext.Length > PlaintextLimit)
                    throw new RecordOverflowException();

                _tlsStream.IncDecodingSequenceNum();

                _decodedReader = new BinaryReader(plaintext);
                _counter = _decodedReader.StartCounter(plaintext.Length);
                _decodedReader.SetReadLimit(plaintext.Length);

                if (RecordType == ContentType.Handshake)
                    _decodedReader.StartTracking(_tlsStream.SecurityParameters.HandshakeData);
            }

            if (_counter.CanRead)
            {
                ContentMessage message;
                switch (RecordType)
                {
                    case ContentType.Handshake:
                        message = new HandshakeMessage(_tlsStream);
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

                message.Read(_decodedReader);

                if (!_counter.CanRead)
                {
                    if (RecordType == ContentType.Handshake)
                        _decodedReader.StopTracking();
                    _counter = null;
                }

                return message;
            }
            else
                throw new InvalidOperationException();
        }

        public void Write(Stream stream)
        {
            if (_tlsStream.WriteEncrypted && RecordType != ContentType.ChangeCipherSpec)
                WriteEncrypted(stream);
            else
                WriteRaw(stream);
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
                contentWriter.StartTracking(_tlsStream.SecurityParameters.HandshakeData);

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
                rawWriter.StartTracking(_tlsStream.SecurityParameters.HandshakeData);

            for (int i = 0; i < ContentMessages.Count; i++)
                ContentMessages[i].Write(rawWriter);

            rawWriter.StopTracking();

            var ciphertext = _tlsStream.SelectedCipher.SymmetricCipher.Encrypt(RecordType, Version, _tlsStream.EncodingSequenceNum, rawList.ToArray());
            _tlsStream.IncEncodingSequenceNum();

            writer.WriteByte((byte)RecordType);
            writer.WriteShort((ushort)Version);
            writer.WriteShort((ushort)ciphertext.Length);
            list.AddRange(ciphertext);

            var buffer = list.ToArray();
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}