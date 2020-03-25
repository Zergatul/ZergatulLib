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

        public virtual ParserBase<TokenBase> GetObjectParser(
            PdfFileReader reader,
            XRefTable xref,
            Dictionary<long, ObjectStream> cache,
            long id,
            int generation) =>
            new ObjectParser(reader, this, xref, cache, id, generation);

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
    }
}