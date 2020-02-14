using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.IO.Compression
{
    class ZlibDecompressor : IByteProcessor
    {
        public int Available { get; private set; }
        public bool IsFinished { get; private set; }

        private State _state;
        private InputBuffer _input;
        private Adler32 _adler32;
        private DeflateDecompressor _deflate;

        public ZlibDecompressor(int readBufferSize)
        {
            _state = State.ReadBlockHeader;
            _input = new InputBuffer(readBufferSize);
            _adler32 = new Adler32();
        }

        public void Add(byte[] buffer, int offset, int count)
        {
            if (Available != 0)
                throw new InvalidOperationException();

            _input.Add(buffer, offset, count);
        }

        public void Decode()
        {
            while (true)
            {
                switch (_state)
                {
                    case State.ReadBlockHeader:
                        if (_input.TotalBits < 16)
                            return;

                        int cm = _input.ReadBits(4);
                        int cinfo = _input.ReadBits(4);
                        int fcheck = _input.ReadBits(5);
                        int fdict = _input.ReadBits(1);
                        int flevel = _input.ReadBits(2);

                        if ((CompressionMethod)cm != CompressionMethod.Deflate)
                            throw new ZlibDataFormatException();
                        if (fdict == 1)
                            throw new NotImplementedException();

                        int check = (cinfo << 12) | (cm << 8) | (flevel << 6) | (fdict << 5) | fcheck;
                        if (check % 31 != 0)
                            throw new ZlibDataFormatException();

                        _deflate = new DeflateDecompressor(_input);
                        _adler32.Reset();
                        _state = State.ReadDeflateBlock;
                        goto case State.ReadDeflateBlock;

                    case State.ReadDeflateBlock:
                        if (_deflate.Available == 0 && _deflate.IsFinished)
                        {
                            if (_input.TotalBits < 32)
                                return;

                            uint sum = (uint)_input.ReadRawInt32BE();
                            if (sum != _adler32.GetCheckSum())
                                throw new ZlibDataFormatException();

                            IsFinished = true;
                            _state = State.End;
                            return;
                        }
                        _deflate.Decode();
                        Available = _deflate.Available;
                        return;

                    case State.End:
                        return;
                }
            }
        }

        public void Get(byte[] buffer, int offset, int count)
        {
            if (count > Available)
                throw new InvalidOperationException();

            _deflate.Get(buffer, offset, count);
            _adler32.Update(buffer, offset, count);
            Available = _deflate.Available;
        }

        #region Nested classes

        private enum State
        {
            ReadBlockHeader,
            ReadDeflateBlock,
            End
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