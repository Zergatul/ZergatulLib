using System;
using System.Collections.Generic;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;

    internal class ObjectParser : ParserBase<TokenBase>
    {
        private readonly long _id;
        private readonly int _generation;
        private readonly XRefTable _xref;
        private readonly Dictionary<long, ObjectStream> _cache;

        public ObjectParser(PdfFileReader reader, ParserFactory factory, XRefTable xref, Dictionary<long, ObjectStream> cache, long id, int generation)
            : base(reader, factory)
        {
            _xref = xref;
            _cache = cache;
            _id = id;
            _generation = generation;
        }

        public override TokenBase Parse()
        {
            var entry = _xref[_id];
            if (entry.Generation != _generation)
                throw InvalidDataExceptionByCode(ErrorCodes.GenerationNumberMismatch);

            if (entry.Free)
                return NullToken.Instance;

            if (entry.Compressed)
            {
                if (!_cache.TryGetValue(entry.StreamObjectId, out ObjectStream objStream))
                {
                    objStream = _factory.GetObjectStreamParser(_reader, _xref[entry.StreamObjectId]).Parse();
                    _cache.Add(entry.StreamObjectId, objStream);
                }

                long offset = objStream.Objects[(int)entry.ObjectIndex].Offset;
                long endOffset = entry.ObjectIndex + 1 == objStream.Objects.Count ? objStream.Data.Length : objStream.Objects[(int)entry.ObjectIndex + 1].Offset;
                var parser = new TokenParser(objStream.Data, offset, endOffset);
                return parser.NextToken();
            }

            _reader.ResetBuffer(entry.Offset);

            throw new NotImplementedException();
        }
    }
}