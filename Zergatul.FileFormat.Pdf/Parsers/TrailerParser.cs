using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class TrailerParser : ParserBase<TrailerDictionary>
    {
        public TrailerParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override TrailerDictionary Parse()
        {
            if ((_reader.Parser.LastToken as StaticToken)?.Value != "trailer")
                throw InvalidDataExceptionByCode(ErrorCodes.TrailerExpected);

            var token = _reader.Parser.NextToken();
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw InvalidDataExceptionByCode(ErrorCodes.TrailerInvalidToken);

            return new TrailerDictionary(dictionary);
        }
    }
}