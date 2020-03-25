using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class MainFooterParser : ParserBase<Footer>
    {
        private static readonly byte[] StartXRefMarker = new byte[] { 0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66 };

        public MainFooterParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override Footer Parse()
        {
            GoToStartXRef();
            GoToMainXref();

            return _factory.GetFooterParser(_reader).Parse();
        }

        #region Private methods

        private void GoToStartXRef()
        {
            bool found = false;

            int markerLength = StartXRefMarker.Length;
            long position = _reader.Stream.Length - markerLength;
            while (position > 0)
            {
                position = System.Math.Max(position - PdfFileReader.BufferSize + markerLength, 0);
                _reader.Stream.Position = position;
                int read = _reader.ReadBuffer();

                for (int i = read - 1; i >= markerLength; i--)
                {
                    found = true;
                    for (int j = 0; j < markerLength; j++)
                        if (_reader.Buffer[i - j] != StartXRefMarker[markerLength - j - 1])
                        {
                            found = false;
                            break;
                        }

                    if (!found)
                        continue;

                    _reader.ResetBuffer(position + i + 1 - markerLength);
                    break;
                }

                if (found)
                    break;
            }

            if (!found)
                throw InvalidDataExceptionByCode(ErrorCodes.CannotLocateXRef);
        }

        private void GoToMainXref()
        {
            var token = _reader.Parser.NextToken();
            if ((token as StaticToken)?.Value != "startxref")
                throw InvalidDataExceptionByCode(ErrorCodes.StartXRefExpected);

            token = _reader.Parser.NextToken();
            var integer = token as IntegerToken;
            if (integer == null)
                throw InvalidDataExceptionByCode(ErrorCodes.InvalidStartXRefValue);

            _reader.ResetBuffer(integer.Value);
        }

        #endregion
    }
}