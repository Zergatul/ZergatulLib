using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    internal class FooterParser : ParserBase<Footer>
    {
        public FooterParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override Footer Parse()
        {
            var token = _reader.Parser.NextToken(allowObj: true);
            if (token is BeginObjectToken)
            {
                return _factory.GetXRefStreamParser(_reader).Parse();
            }
            else
            {
                var xref = _factory.GetXRefTableParser(_reader).Parse();
                var trailer = _factory.GetTrailerParser(_reader).Parse();
                return new Footer(xref, trailer);
            }
        }
    }
}