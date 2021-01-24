using System;
using System.Collections.Generic;
using System.IO;
using Zergatul.FileFormat.Pdf.Parsers;

namespace Zergatul.FileFormat.Pdf
{
    public class PdfFile
    {
        public string Version => _reader.Version;
        public byte[] OriginalDocumentId => _reader.OriginalDocumentId;
        public byte[] DocumentId => _reader.DocumentId;
        public IReadOnlyList<Footer> Footers => _reader.Footers;
        public XRefTable XRef => _reader.XRef;
        public DocumentCatalog Catalog => _reader.Catalog;

        private ParserFactory _factory;
        private PdfFileReader _reader;

        public PdfFile(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("stream.CanRead is false.");
            if (!stream.CanSeek)
                throw new ArgumentException("stream.CanSeek is false.");
            if (stream.Position != 0)
                throw new ArgumentException("stream.Position should be 0.");

            _factory = ParserFactory.Default;
            _reader = new PdfFileReader(stream, _factory);

            Parse();
        }

        private void Parse()
        {
            _factory.GetVersionParser(_reader).Parse();
            _factory.GetFootersParser(_reader).Parse();
            _factory.GetDocumentCatalogParser(_reader).Parse();
        }
    }
}