using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class ObjectStreamParser : ParserBase<ObjectStream>
    {
        private readonly XRefEntry _entry;

        public ObjectStreamParser(PdfFileReader reader, ParserFactory factory, XRefEntry entry)
            : base(reader, factory)
        {
            _entry = entry;
        }

        public override ObjectStream Parse()
        {
            if (_entry.Compressed)
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamCompressedEntry);

            if (_entry.Free)
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamCompressedFree);

            _reader.ResetBuffer(_entry.Offset);

            var token = _reader.Parser.NextToken(allowObj: true);
            var beginObj = token as BeginObjectToken;
            if (beginObj == null)
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamBeginObjectTokenExpected);

            if (beginObj.Id != _entry.Id)
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamObjectNumberMismatch);
            if (beginObj.Generation != _entry.Generation)
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamObjectGenerationMismatch);

            var streamObj = new StreamObject(_reader.Stream, _reader.Parser);
            var dictionary = streamObj.Dictionary;

            const string Type = nameof(Type);
            const string N = nameof(N);
            const string First = nameof(First);

            if (!dictionary.ContainsKey(Type))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamTypeExpected);
            if (!dictionary.Is<NameToken>(Type))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamTypeInvalidToken);
            if (dictionary.GetName(Type) != "ObjStm")
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamTypeInvalidValue);

            if (!dictionary.ContainsKey(N))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamNExpected);
            if (!dictionary.Is<IntegerToken>(N))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamNInvalidToken);

            if (!dictionary.ContainsKey(First))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamFirstExpected);
            if (!dictionary.Is<IntegerToken>(First))
                throw InvalidDataExceptionByCode(ErrorCodes.ObjectStreamFirstInvalidToken);

            return new ObjectStream(streamObj.Data, dictionary.GetInteger(N), dictionary.GetInteger(First));
        }
    }
}