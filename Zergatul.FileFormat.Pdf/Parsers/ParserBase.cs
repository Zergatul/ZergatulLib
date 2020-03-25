namespace Zergatul.FileFormat.Pdf.Parsers
{
    internal abstract class ParserBase<T>
    {
        protected PdfFileReader _reader;
        protected ParserFactory _factory;

        public ParserBase(PdfFileReader reader, ParserFactory factory)
        {
            _reader = reader;
            _factory = factory;
        }

        public abstract T Parse();
    }
}