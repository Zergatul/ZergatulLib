using System.Collections.Generic;
using System.Linq;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;
    using static StringConstants;

    internal class DocumentCatalogParser : ParserBase<DocumentCatalog>
    {
        public DocumentCatalogParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {
            
        }

        public override DocumentCatalog Parse()
        {
            var root = _reader.Footers[0].Trailer.Root;
            var token = _factory.GetObjectParser(_reader, root.Id, root.Generation).Parse();
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw InvalidDataExceptionByCode(ErrorCodes.DocumentCatalogInvalidToken);

            if (!dictionary.ValidateName(Type, "Catalog"))
                throw InvalidDataExceptionByCode(ErrorCodes.DocumentCatalogTypeInvalid);
            if (!dictionary.TryGet(Pages, out IndirectReferenceToken pages))
                throw InvalidDataExceptionByCode(ErrorCodes.DocumentCatalogPagesInvalid);

            _reader.Catalog.Pages = ParsePages(pages).ToArray();

            return _reader.Catalog;
        }

        private IEnumerable<Page> ParsePages(IndirectReferenceToken reference)
        {
            var token = _factory.GetObjectParser(_reader, reference.Id, reference.Generation).Parse();
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw InvalidDataExceptionByCode(ErrorCodes.PagesInvalidToken);

            return ParsePages(dictionary);
        }

        private IEnumerable<Page> ParsePages(DictionaryToken dictionary)
        {
            if (!dictionary.ValidateName(Type, "Pages"))
                throw InvalidDataExceptionByCode(ErrorCodes.PagesTypeInvalid);

            if (!dictionary.TryGet(Kids, out ArrayToken kids))
                throw InvalidDataExceptionByCode(ErrorCodes.PagesKidsInvalid);

            if (!kids.ValidateType<IndirectReferenceToken>())
                throw InvalidDataExceptionByCode(ErrorCodes.PagesKidsInvalidItems);

            if (!dictionary.TryGet(Count, out IntegerToken count))
                throw InvalidDataExceptionByCode(ErrorCodes.PagesCountInvalid);

            int total = 0;
            foreach (IndirectReferenceToken kid in kids.Value)
            {
                var token = _factory.GetObjectParser(_reader, kid.Id, kid.Generation).Parse();

                dictionary = token as DictionaryToken;
                if (dictionary == null)
                    throw InvalidDataExceptionByCode(ErrorCodes.PageInvalidToken);

                if (!dictionary.TryGet(Type, out NameToken name))
                    throw InvalidDataExceptionByCode(ErrorCodes.PageTypeInvalidToken);

                switch (name.Value)
                {
                    case "Pages":
                        foreach (var page in ParsePages(dictionary))
                        {
                            total++;
                            yield return page;
                        }
                        break;

                    case "Page":
                        total++;
                        yield return new Page(dictionary);
                        break;

                    default:
                        throw InvalidDataExceptionByCode(ErrorCodes.PageTypeInvalidValue);
                }
            }

            if (total != count.Value)
                throw InvalidDataExceptionByCode(ErrorCodes.PagesCountMismatch);
        }
    }
}