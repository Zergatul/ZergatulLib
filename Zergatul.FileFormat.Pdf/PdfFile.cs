using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Parsers;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;
    using static StringConstants;

    public class PdfFile
    {
        private static readonly byte[] StartXRefMarker = new byte[] { 0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66 };

        public string Version { get; private set; }
        public byte[] OriginalDocumentId { get; private set; }
        public byte[] DocumentId { get; private set; }
        public List<Footer> Footers { get; } = new List<Footer>();
        public XRefTable XRef { get; } = new XRefTable();
        public DocumentCatalog Document { get; } = new DocumentCatalog();

        private ParserFactory _factory;
        private PdfFileReader _reader;

        public PdfFile(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException("stream.CanRead is false.");
            if (!stream.CanSeek)
                throw new ArgumentException("stream.CanSeek is false.");
            if (stream.Position != 0)
                throw new ArgumentException("stream.Position should be 0.");

            _factory = ParserFactory.Default;
            _reader = new PdfFileReader(stream, _factory, XRef);

            Parse();

            ParseDocumentCatalog();
        }

        private void Parse()
        {
            Version = _factory.GetVersionParser(_reader).Parse();

            var footer = _factory.GetMainFooterParser(_reader).Parse();
            Footers.Add(footer);
            while (footer.Trailer.Prev != null)
            {
                _reader.ResetBuffer(footer.Trailer.Prev.Value);
                footer = _factory.GetFooterParser(_reader).Parse();
                Footers.Add(footer);
            }

            ProcessFooters();
        }

        private void ProcessFooters()
        {
            // merge xref tables
            for (int i = Footers.Count - 1; i >= 0; i--)
                XRef.MergeWith(Footers[i].XRef);

            // Any object in a cross-reference section whose number is greater than Trailer.Size is ignored and considered missing
            var remove = new List<long>();
            foreach (long id in XRef.Keys)
                if (id > Footers[0].Trailer.Size)
                    remove.Add(id);

            foreach (long id in remove)
                XRef.Remove(id);

            // fill ids
            if (Footers[0].Trailer.OriginalId != null)
                OriginalDocumentId = Footers[0].Trailer.OriginalId;
            if (Footers[0].Trailer.Id != null)
                DocumentId = Footers[0].Trailer.Id;
        }

        private void ParseDocumentCatalog()
        {
            ParseCatalogDictionary();
        }

        private void ParseCatalogDictionary()
        {
            var root = Structure.ListTrailers[0].Root;
            var token = GetObject(root);
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw new InvalidDataException("Document root should be dictionary.");

            if (!dictionary.ValidateName(Type, "Catalog"))
                throw new InvalidDataException("Catalog dictionary should have /Type key with /Catalog value.");
            if (!dictionary.TryGet(Pages, out IndirectReferenceToken pages))
                throw new InvalidDataException("Catalog dictionary should have /Pages key with indirect reference.");

            ParsePages(pages);
        }

        private void ParsePages(IndirectReferenceToken indirectReference)
        {
            var token = GetObject(indirectReference);
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw new InvalidDataException("Pages indirect reference should refer to dictionary.");

            if (!dictionary.ValidateName(Type, "Pages"))
                throw new InvalidDataException("Pages dictionary should have /Type key with /Pages value.");

            if (!dictionary.TryGet(Kids, out ArrayToken kids))
                throw new InvalidDataException("Pages dictionary should have /Kids key with array value.");

            if (!kids.ValidateType<IndirectReferenceToken>())
                throw new InvalidDataException("/Kids array should contain indirect references.");

            if (!dictionary.TryGet(Count, out IntegerToken count))
                throw new InvalidDataException("Pages dictionary should have /Count key with integer value.");

            var pages = new Page[count.Value];
            var queue = new Queue<IndirectReferenceToken>();
            foreach (var kid in kids.Value)
                queue.Enqueue(kid as IndirectReferenceToken);
            int index = 0;
            while (queue.Count > 0)
            {
                var page = ParsePage(queue.Dequeue());
                pages[index++] = page;
            }


            Document.Pages = pages;
        }

        private Page ParsePage(IndirectReferenceToken indirectReference)
        {
            var token = GetObject(indirectReference);
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw new InvalidDataException("Page indirect reference should refer to dictionary.");

            if (!dictionary.ValidateName(Type, "Page"))
                throw new InvalidDataException("Page dictionary should have /Type key with /Page value.");
        }

        private TokenBase GetObject(IndirectReferenceToken indirectReferenceToken)
        {
            return GetObject(new IndirectObject(indirectReferenceToken));
        }

        private TokenBase GetObject(IndirectObject indirectObject)
        {
            var entry = Structure.XRef[indirectObject.Id];
            if (entry.Generation != indirectObject.Generation)
                throw new InvalidDataException("Invalid indirect object generation number.");

            if (entry.Free)
                return NullToken.Instance;

            if (entry.Compressed)
            {
                ObjectStream objStream;
                if (!_objectStreamCache.ContainsKey(entry.StreamObjectId))
                {
                    var comprEntry = Structure.XRef[entry.StreamObjectId];
                    objStream = ParseObjectStream(comprEntry);
                    _objectStreamCache.Add(entry.StreamObjectId, objStream);
                }
                else
                {
                    objStream = _objectStreamCache[entry.StreamObjectId];
                }

                long offset = objStream.Objects[(int)entry.ObjectIndex].Offset;
                long endOffset = entry.ObjectIndex + 1 == objStream.Objects.Count ? objStream.Data.Length : objStream.Objects[(int)entry.ObjectIndex + 1].Offset;
                var parser = new TokenParser(objStream.Data, offset, endOffset);
                return parser.NextToken();
            }

            _stream.Position = entry.Offset;
            ResetBuffer();

            throw new NotImplementedException();
        }

        

    }
}