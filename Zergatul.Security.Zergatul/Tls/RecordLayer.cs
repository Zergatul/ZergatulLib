using System;
using System.IO;
using static Zergatul.Security.Zergatul.Tls.TlsConstants;

namespace Zergatul.Security.Zergatul.Tls
{
    class RecordLayer
    {
        public ContentType CurrentContentType { get; private set; }
        public ProtocolVersion ProtocolVersion { get; private set; }

        private readonly Stream _stream;
        private readonly StateMachine _stateMachine;
        private ReadState _readState;
        private int _recordLength;
        private byte[] _readBuffer;
        private int _readOffset;

        public RecordLayer(Stream stream, StateMachine stateMachine)
        {
            _stream = stream;
            _stateMachine = stateMachine;

            _readState = ReadState.ReadHeader;
            _readBuffer = new byte[RecordLayerHeaderLength];
        }

        public void FillContentType()
        {
            if (_readState == ReadState.ReadHeader)
                ReadHeader();
        }

        public void ReadNext(byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                if (_readState == ReadState.ReadHeader)
                    ReadHeader();

                if (_readState == ReadState.ReadBody)
                {
                    int read = _stream.Read(buffer, offset, System.Math.Min(count - offset, _recordLength - _readOffset));
                    if (read == 0)
                        throw new TlsStreamRecordLayerException(ErrorCodes.UnexpectedEndOfStream);

                    offset += read;
                    _readOffset += read;
                    if (_readOffset == _recordLength)
                        _readState = ReadState.ReadHeader;
                }
            }
        }

        private void ReadHeader()
        {
            int index = 0;
            while (index < RecordLayerHeaderLength)
            {
                int read = _stream.Read(_readBuffer, index, RecordLayerHeaderLength - index);
                if (read == 0)
                    throw new TlsStreamRecordLayerException(ErrorCodes.UnexpectedEndOfStream);
                index += read;
            }

            CurrentContentType = (ContentType)_readBuffer[0];
            ProtocolVersion = (ProtocolVersion)(_readBuffer[1] | (_readBuffer[2] << 8));
            _recordLength = _readBuffer[3] | (_readBuffer[4] << 8);

            ValidateContentType();
            ValidateLength();

            _readState = ReadState.ReadBody;
            _readOffset = 0;
        }

        private void ValidateContentType()
        {
            switch (CurrentContentType)
            {
                case ContentType.Handshake:
                    if (_stateMachine.HState == HandshakeState.Finished)
                        throw new TlsStreamRecordLayerException(ErrorCodes.UnexpectedHandshakeMessage);
                    return;

                case ContentType.ChangeCipherSpec:
                case ContentType.ApplicationData:
                case ContentType.Alert:
                    return;
            }

            var exception = new TlsStreamRecordLayerException(ErrorCodes.InvalidContentType);
            exception.Data.Add("ContentType", (int)CurrentContentType);
            throw exception;
        }

        private void ValidateLength()
        {
            if (_recordLength > RecordLayerLimit)
            {
                var exception = new TlsStreamRecordLayerException(ErrorCodes.RecordLayerOverflow);
                exception.Data.Add("Length", _recordLength);
                throw exception;
            }
        }
    }
}