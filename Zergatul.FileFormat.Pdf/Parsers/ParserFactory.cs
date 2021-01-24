using System.Collections.Generic;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    internal class ParserFactory
    {
        public static ParserFactory Default { get; } = new ParserFactory();

        public virtual ParserBase<Footer> GetFooterParser(PdfFileReader reader) =>
            new FooterParser(reader, this);

        public virtual ParserBase<Footer> GetMainFooterParser(PdfFileReader reader) =>
            new MainFooterParser(reader, this);

        public virtual ParserBase<List<Footer>> GetFootersParser(PdfFileReader reader) =>
            new FootersParser(reader, this);

        public virtual ParserBase<TokenBase> GetObjectParser(PdfFileReader reader, long id, int generation) =>
            new ObjectParser(reader, this, id, generation);

        public virtual ParserBase<ObjectStream> GetObjectStreamParser(PdfFileReader reader, XRefEntry entry) =>
            new ObjectStreamParser(reader, this, entry);

        public virtual ParserBase<TrailerDictionary> GetTrailerParser(PdfFileReader reader) =>
            new TrailerParser(reader, this);

        public virtual ParserBase<string> GetVersionParser(PdfFileReader reader) =>
            new VersionParser(reader, this);

        public virtual ParserBase<Footer> GetXRefStreamParser(PdfFileReader reader) =>
            new XRefStreamParser(reader, this);

        public virtual ParserBase<XRefTable> GetXRefTableParser(PdfFileReader reader) =>
            new XRefTableParser(reader, this);

        public virtual ParserBase<DocumentCatalog> GetDocumentCatalogParser(PdfFileReader reader) =>
            new DocumentCatalogParser(reader, this);
    }
}