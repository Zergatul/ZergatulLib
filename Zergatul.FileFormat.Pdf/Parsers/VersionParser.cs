using System.Text;
using Zergatul.IO;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static Common;
    using static ExceptionHelper;

    internal class VersionParser : ParserBase<string>
    {
        private const int MaxHeaderLength = 15;
        private static readonly byte[] HeaderMarker = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };

        public VersionParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override string Parse()
        {
            byte[] buffer = new byte[MaxHeaderLength];
            StreamHelper.ReadArray(_reader.Stream, buffer, MaxHeaderLength);
            for (int i = 0; i < HeaderMarker.Length; i++)
                if (buffer[i] != HeaderMarker[i])
                    throw InvalidDataExceptionByCode(ErrorCodes.InvalidPdfHeader);

            for (int i = 0; i < MaxHeaderLength; i++)
            {
                if (buffer[i] >= 0x80)
                    throw InvalidDataExceptionByCode(ErrorCodes.InvalidVersionCharacter);

                if (IsEndOfLine(buffer[i]))
                {
                    _reader.Version = Encoding.ASCII.GetString(buffer, HeaderMarker.Length, i - HeaderMarker.Length);
                    return _reader.Version;
                }
            }

            throw InvalidDataExceptionByCode(ErrorCodes.InvalidVersion);
        }
    }
}