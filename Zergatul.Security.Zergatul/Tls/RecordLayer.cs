using System;
using System.IO;
using static Zergatul.Security.Zergatul.Tls.TlsConstants;

namespace Zergatul.Security.Zergatul.Tls
{
    struct RecordLayer
    {
        public ContentType Type;
        public ProtocolVersion Version;
        public int Length;
        public ReadState RState;
        public byte[] ReadBuffer;
        public int BufferOffset;
        public int BufferLength;

        public bool EndOfRecordMessage => BufferLength != 0 && BufferOffset == BufferLength;
        public bool HasFullHigherProtocolMessage
        {
            get
            {
                switch (Type)
                {
                    case ContentType.Handshake:
                        bool hasHeader = BufferLength >= BufferOffset + 4;
                        if (!hasHeader)
                            return false;
                        int messageLength =
                            ReadBuffer[BufferOffset] |
                            (ReadBuffer[BufferOffset + 1] << 8) |
                            (ReadBuffer[BufferOffset + 2] << 16);
                        return BufferLength >= BufferOffset + 4 + messageLength;

                    case ContentType.ChangeCipherSpec:
                        throw new NotImplementedException();

                    case ContentType.ApplicationData:
                        throw new NotImplementedException();

                    case ContentType.Alert:
                        throw new NotImplementedException();

                    default:
                        throw new TlsStreamInternalErrorException("Invalid content type");
                }
            }
        }

        public void Init()
        {
            ReadBuffer = new byte[RecordLayerLimit];
        }

        public void ReadNext(Stream stream)
        {
            if (RState == ReadState.ReadHeader)
            {
                int index = 0;
                while (index < RecordLayerHeaderLength)
                {
                    int read = stream.Read(ReadBuffer, index, RecordLayerHeaderLength - index);
                    if (read == 0)
                        throw new TlsStreamRecordLayerException("Unexpected end of stream");
                    index += read;
                }

                Type = (ContentType)ReadBuffer[0];
                Version = (ProtocolVersion)(ReadBuffer[1] | (ReadBuffer[2] << 8));
                Length = ReadBuffer[3] | (ReadBuffer[4] << 8);

                ValidateContentType();
                ValidateLength();

                RState = ReadState.ReadBody;
                BufferOffset = 0;
                BufferLength = 0;
            }

            if (RState == ReadState.ReadBody)
            {
                int read = stream.Read(ReadBuffer, BufferLength, Length - BufferLength);
                if (read == 0)
                    throw new TlsStreamRecordLayerException("Unexpected end of stream");

                BufferLength += read;
                if (BufferLength == Length)
                    RState = ReadState.ReadHeader;
            }
        }

        private void ValidateContentType()
        {
            switch (Type)
            {
                case ContentType.Handshake:
                case ContentType.ChangeCipherSpec:
                case ContentType.ApplicationData:
                case ContentType.Alert:
                    return;
            }

            var exception = new TlsStreamRecordLayerException("Invalid content type");
            exception.Data.Add("ContentType", (int)Type);
            throw exception;
        }

        private void ValidateLength()
        {
            if (Length > RecordLayerLimit)
            {
                var exception = new TlsStreamRecordLayerException("Record layer overflow");
                exception.Data.Add("Length", Length);
                throw exception;
            }
        }
    }
}