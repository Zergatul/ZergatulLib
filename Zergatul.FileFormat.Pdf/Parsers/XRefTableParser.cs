using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class XRefTableParser : ParserBase<XRefTable>
    {
        public XRefTableParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override XRefTable Parse()
        {
            if ((_reader.Parser.LastToken as StaticToken)?.Value != "xref")
                throw InvalidDataExceptionByCode(ErrorCodes.XRefExpected);

            var xref = new XRefTable();

            while (true)
            {
                var token = _reader.Parser.NextToken();
                var integer = token as IntegerToken;
                if (integer == null)
                {
                    if (xref.Count > 0)
                        return xref;
                    else
                        throw InvalidDataExceptionByCode(ErrorCodes.XRefEmpty);
                }
                long objectNumber = integer.Value;

                token = _reader.Parser.NextToken();
                integer = token as IntegerToken;
                if (integer == null)
                    throw InvalidDataExceptionByCode(ErrorCodes.XRefObjectCountInvalidToken);
                long objectCount = integer.Value;

                for (long i = 0; i < objectCount; i++)
                {
                    token = _reader.Parser.NextToken();
                    integer = token as IntegerToken;
                    if (integer == null)
                        throw InvalidDataExceptionByCode(ErrorCodes.XRefEntryOffsetInvalidToken);
                    long offset = integer.Value;

                    token = _reader.Parser.NextToken();
                    integer = token as IntegerToken;
                    if (integer == null)
                        throw InvalidDataExceptionByCode(ErrorCodes.XRefEntryGenNumberInvalidToken);
                    int generation = checked((int)integer.Value);

                    token = _reader.Parser.NextToken();
                    var @static = token as StaticToken;
                    if (@static?.Value != "n" && @static?.Value != "f")
                        throw InvalidDataExceptionByCode(ErrorCodes.XRefEntryInUseMarkerInvalid);
                    bool free = @static.Value == "f";

                    xref.Add(objectNumber + i, offset, generation, free);
                }
            }
        }
    }
}