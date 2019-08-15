using System;
using System.IO;
using static Zergatul.Security.Zergatul.Tls.TlsConstants;

namespace Zergatul.Security.Zergatul.Tls
{
    struct RecordLayer
    {
        private TlsStream _tlsStream;
        private ReadState _readState;
        private ContentType _contentType;
        private ProtocolVersion _protocolVersion;
        private int _recordLength;
        private byte[] _readBuffer;
        private int _readOffset;

        public void Init(TlsStream tlsStream)
        {
            _tlsStream = tlsStream;
            _readState = ReadState.ReadHeader;
            _readBuffer = new byte[RecordLayerHeaderLength];
        }

        public void ReadNext(byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                if (_readState == ReadState.ReadHeader)
                {
                    int index = 0;
                    while (index < RecordLayerHeaderLength)
                    {
                        int read = _tlsStream.InnerStream.Read(_readBuffer, index, RecordLayerHeaderLength - index);
                        if (read == 0)
                            throw new TlsStreamRecordLayerException("Unexpected end of stream");
                        index += read;
                    }

                    _contentType = (ContentType)_readBuffer[0];
                    _protocolVersion = (ProtocolVersion)(_readBuffer[1] | (_readBuffer[2] << 8));
                    _recordLength = _readBuffer[3] | (_readBuffer[4] << 8);

                    ValidateContentType();
                    ValidateLength();

                    _readState = ReadState.ReadBody;
                    _readOffset = 0;
                }

                if (_readState == ReadState.ReadBody)
                {
                    int read = _tlsStream.InnerStream.Read(buffer, offset, System.Math.Min(count - offset, _recordLength - _readOffset));
                    if (read == 0)
                        throw new TlsStreamRecordLayerException("Unexpected end of stream");

                    offset += read;
                    _readOffset += read;
                    if (_readOffset == _recordLength)
                        _readState = ReadState.ReadHeader;
                }
            }
        }

        private void ValidateContentType()
        {
            switch (_contentType)
            {
                case ContentType.Handshake:
                    if (_tlsStream.StateMachine.HState == HandshakeState.Finished)
                        throw new TlsStreamRecordLayerException("Received handshake type record, but handshake is finished");
                    break;

                case ContentType.ChangeCipherSpec:
                case ContentType.ApplicationData:
                case ContentType.Alert:
                    return;
            }

            var exception = new TlsStreamRecordLayerException("Invalid content type");
            exception.Data.Add("ContentType", (int)_contentType);
            throw exception;
        }

        private void ValidateLength()
        {
            if (_recordLength > RecordLayerLimit)
            {
                var exception = new TlsStreamRecordLayerException("Record layer overflow");
                exception.Data.Add("Length", _recordLength);
                throw exception;
            }
        }
    }
}