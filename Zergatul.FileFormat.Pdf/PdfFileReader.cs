using System.Collections.Generic;
using System.IO;
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

        public string Version { get; set; }
        public byte[] OriginalDocumentId { get; set; }
        public byte[] DocumentId { get; set; }
        public XRefTable XRef { get; set; }
        public List<Footer> Footers { get; set; }
        public DocumentCatalog Catalog { get; set; }
        public Dictionary<long, ObjectStream> ObjectStreamCache { get; set; }

        private int _bufOffset, _bufLength;
        private ParserFactory _factory;

        public PdfFileReader(Stream stream, ParserFactory factory)
        {
            _factory = factory;

            Stream = stream;
            Buffer = new byte[BufferSize];
            Parser = new TokenParser(NextByte);
            Footers = new List<Footer>();
            XRef = new XRefTable();
            Catalog = new DocumentCatalog();
            ObjectStreamCache = new Dictionary<long, ObjectStream>();
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