using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Parsers;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;

namespace Zergatul.FileFormat.Pdf
{
    using static ExceptionHelper;

    internal class PdfFileReader
    {
        public const int BufferSize = 1024;

        public Stream Stream { get; }
        public byte[] Buffer { get; }
        public TokenParser Parser { get; }

        private int _bufOffset, _bufLength;
        private ParserFactory _factory;
        private XRefTable _xref;
        private Dictionary<long, ObjectStream> _objectStreamCache;

        public PdfFileReader(Stream stream, ParserFactory factory, XRefTable xref)
        {
            Stream = stream;
            Buffer = new byte[BufferSize];
            Parser = new TokenParser(NextByte);

            _factory = factory;
            _xref = xref;
            _objectStreamCache = new Dictionary<long, ObjectStream>();
        }

        #region Public methods

        public void ResetBuffer(long position)
        {
            Stream.Position = position;
            _bufOffset = _bufLength = 0;
            Parser.Reset(position);
        }

        public int ReadBuffer()
        {
            int read = (int)System.Math.Min(BufferSize, Stream.Length - Stream.Position);
            StreamHelper.ReadArray(Stream, Buffer, read);
            return read;
        }

        #endregion

        #region Private methods

        private int NextByte()
        {
            if (_bufOffset >= _bufLength)
            {
                if (Stream.Position == Stream.Length)
                    throw new EndOfStreamException();

                _bufOffset = 0;
                _bufLength = Stream.Read(Buffer, 0, BufferSize);

                if (_bufLength == 0)
                    throw new EndOfStreamException();
            }

            return Buffer[_bufOffset++];
        }

        #endregion
    }
}