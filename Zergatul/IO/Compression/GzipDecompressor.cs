using System;

namespace Zergatul.IO.Compression
{
    // https://tools.ietf.org/html/rfc1952
    class GzipDecompressor : IByteProcessor
    {
        public int Available { get; private set; }
        public bool IsFinished { get; private set; }
        public bool CanFinishNow => _state == State.ReadBlockHeader && _input.TotalBits == 0;
        public int ReadBufferSize { get; }

        private State _state;
        private InputBuffer _input;
        private CRC32 _crc32;
        private uint _memberLength;
        private DeflateDecompressor _deflate;

        public GzipDecompressor(int readBufferSize)
        {
            ReadBufferSize = readBufferSize;
            _state = State.ReadBlockHeader;
            _input = new InputBuffer(readBufferSize);
            _crc32 = new CRC32(CRC32Parameters.IEEE8023);
        }

        public void Decode()
        {
            DecodeInternal();
        }

        public void Get(byte[] buffer, int offset, int count)
        {
            if (count > Available)
                throw new InvalidOperationException();

            _deflate.Get(buffer, offset, count);
            _crc32.Update(buffer, offset, count);
            _memberLength += (uint)count;
            Available = _deflate.Available;
        }

        public void Add(byte[] buffer, int offset, int count)
        {
            if (Available != 0)
                throw new InvalidOperationException();

            _input.Add(buffer, offset, count);
        }

        private void DecodeInternal()
        {
            while (true)
            {
                switch (_state)
                {
                    case State.ReadBlockHeader:
                        if (_input.TotalBits < 80)
                            return;
                        int id = _input.ReadRawInt16();
                        if (id != 0x8B1F)
                            throw new GzipDataFormatException();
                        var cm = (CompressionMethod)_input.ReadRawByte();
                        if (cm != CompressionMethod.Deflate)
                            throw new NotImplementedException();
                        var flags = (MemberFlags)_input.ReadRawByte();
                        int mtime = _input.ReadRawInt32();
                        byte xfl = _input.ReadRawByte();
                        byte os = _input.ReadRawByte();
                        if (flags.HasFlag(MemberFlags.FEXTRA))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FNAME))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FCOMMENT))
                            throw new NotImplementedException();
                        if (flags.HasFlag(MemberFlags.FHCRC))
                            throw new NotImplementedException();
                        _deflate = new DeflateDecompressor(_input);
                        _crc32.Reset();
                        _memberLength = 0;
                        _state = State.ReadDeflateBlock;
                        goto case State.ReadDeflateBlock;

                    case State.ReadDeflateBlock:
                        if (_deflate.Available == 0 && _deflate.IsFinished)
                        {
                            _input.SkipTillByteBoundary();
                            if (_input.TotalBits < 64)
                                return;
                            uint crc = (uint)_input.ReadRawInt32();
                            uint iSize = (uint)_input.ReadRawInt32();
                            if (_crc32.GetCheckSum() != crc)
                                throw new GzipDataFormatException();
                            if (_memberLength != iSize)
                                throw new GzipDataFormatException();
                            _state = State.ReadBlockHeader;
                            goto case State.ReadBlockHeader;
                        }
                        _deflate.Decode();
                        Available = _deflate.Available;
                        return;
                }
            }
        }

        #region Nested classes

        private enum State
        {
            ReadBlockHeader,
            ReadDeflateBlock
        }

        private enum CompressionMethod
        {
            Deflate = 8
        }

        [Flags]
        private enum MemberFlags
        {
            FTEXT = 0x01,
            FHCRC = 0x02,
            FEXTRA = 0x04,
            FNAME = 0x08,
            FCOMMENT = 0x10
        }

        #endregion
    }
}