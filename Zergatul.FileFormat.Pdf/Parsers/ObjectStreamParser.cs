using System;
using System.Collections.Generic;
using System.Text;
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

            var streamObj = new StreamObject(Stream, Parser);
            var dictionary = streamObj.Dictionary;

            const string Type = nameof(Type);
            const string N = nameof(N);
            const string First = nameof(First);

            if (!dictionary.ContainsKey(Type))
                throw new InvalidDataException("Object stream dictionary must contain [Type] key.");
            if (!dictionary.Is<NameToken>(Type))
                throw new InvalidDataException("Object stream dictionary [Type] key should be name.");
            if (dictionary.GetName(Type) != "ObjStm")
                throw new InvalidDataException("Object stream dictionary [Type] key should have value ObjStm.");

            if (!dictionary.ContainsKey(N))
                throw new InvalidDataException("Object stream dictionary must contain [N] key.");
            if (!dictionary.Is<IntegerToken>(N))
                throw new InvalidDataException("Object stream dictionary [N] key should be integer.");

            if (!dictionary.ContainsKey(First))
                throw new InvalidDataException("Object stream dictionary must contain [First] key.");
            if (!dictionary.Is<IntegerToken>(First))
                throw new InvalidDataException("Object stream dictionary [First] key should be integer.");

            return new ObjectStream(streamObj.Data, dictionary.GetInteger(N), dictionary.GetInteger(First));
        }
    }
}