using System;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class ObjectParser : ParserBase<TokenBase>
    {
        private readonly long _id;
        private readonly int _generation;

        public ObjectParser(PdfFileReader reader, ParserFactory factory, long id, int generation)
            : base(reader, factory)
        {
            _id = id;
            _generation = generation;
        }

        public override TokenBase Parse()
        {
            var entry = _reader.XRef[_id];
            if (entry.Generation != _generation)
                throw InvalidDataExceptionByCode(ErrorCodes.GenerationNumberMismatch);

            if (entry.Free)
                return NullToken.Instance;

            ObjectStream objStream;

            if (entry.Compressed)
            {
                if (!_reader.ObjectStreamCache.TryGetValue(entry.StreamObjectId, out objStream))
                {
                    objStream = _factory.GetObjectStreamParser(_reader, _reader.XRef[entry.StreamObjectId]).Parse();
                    _reader.ObjectStreamCache.Add(entry.StreamObjectId, objStream);
                }

                long offset = objStream.Objects[(int)entry.ObjectIndex].Offset;
                long endOffset = entry.ObjectIndex + 1 == objStream.Objects.Count ? objStream.Data.Length : objStream.Objects[(int)entry.ObjectIndex + 1].Offset;
                var parser = new TokenParser(objStream.Data, offset, endOffset);
                return parser.NextToken();
            }

            _reader.ResetBuffer(entry.Offset);
            return _factory.GetObjectStreamParser(_reader, entry).Parse();
        }
    }
}