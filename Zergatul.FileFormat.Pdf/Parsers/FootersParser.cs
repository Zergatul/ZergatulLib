using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    internal class FootersParser : ParserBase<List<Footer>>
    {
        public FootersParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override List<Footer> Parse()
        {
            var footer = _factory.GetMainFooterParser(_reader).Parse();
            _reader.Footers.Add(footer);

            while (footer.Trailer.Prev != null)
            {
                _reader.ResetBuffer(footer.Trailer.Prev.Value);
                footer = _factory.GetFooterParser(_reader).Parse();
                _reader.Footers.Add(footer);
            }

            ProcessFooters();

            return _reader.Footers;
        }

        private void ProcessFooters()
        {
            // merge xref tables
            for (int i = _reader.Footers.Count - 1; i >= 0; i--)
                _reader.XRef.MergeWith(_reader.Footers[i].XRef);

            // Any object in a cross-reference section whose number is greater than Trailer.Size is ignored and considered missing
            var remove = new List<long>();
            foreach (long id in _reader.XRef.Keys)
                if (id > _reader.Footers[0].Trailer.Size)
                    remove.Add(id);

            foreach (long id in remove)
                _reader.XRef.Remove(id);

            // fill ids
            if (_reader.Footers[0].Trailer.OriginalId != null)
                _reader.OriginalDocumentId = _reader.Footers[0].Trailer.OriginalId;
            if (_reader.Footers[0].Trailer.Id != null)
                _reader.DocumentId = _reader.Footers[0].Trailer.Id;
        }
    }
}