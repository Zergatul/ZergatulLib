using System.IO;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf.Parsers
{
    using static ExceptionHelper;
    using static StringConstants;

    internal class XRefStreamParser : ParserBase<Footer>
    {
        public XRefStreamParser(PdfFileReader reader, ParserFactory factory)
            : base(reader, factory)
        {

        }

        public override Footer Parse()
        {
            var streamObj = new StreamObject(_reader.Stream, _reader.Parser);
            var dictionary = streamObj.Dictionary;

            if (!dictionary.ContainsKey(Type))
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamTypeExpected);
            if (!dictionary.Is<NameToken>(Type))
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamTypeInvalidToken);
            if (dictionary.GetName(Type) != "XRef")
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamTypeInvalidValue);

            if (!dictionary.ContainsKey(W))
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamWExpected);
            if (!dictionary.Is<ArrayToken>(W))
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamWInvalidToken);
            var array = dictionary.GetArray(W);
            if (!array.Is<IntegerToken>(3))
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamWInvalidValue);

            var trailer = new TrailerDictionary(dictionary);

            int field1Len = (int)((IntegerToken)array.Value[0]).Value;
            int field2Len = (int)((IntegerToken)array.Value[1]).Value;
            int field3Len = (int)((IntegerToken)array.Value[2]).Value;
            int total = field1Len + field2Len + field3Len;

            if (field1Len < 0 || field2Len < 0 || field3Len < 0)
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamWNegativeValue);
            if (field1Len > 7 || field2Len > 7 || field3Len > 7)
                throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamWFieldOverflow);

            var xref = new XRefTable();

            using (var ms = new MemoryStream())
            {
                streamObj.Data.CopyTo(ms);
                if (ms.Length % total != 0)
                    throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamInvalidSize);

                long entriesCount = ms.Length / total;
                byte[] buffer = ms.GetBuffer();

                long index = 0;
                long id = 0;
                for (long i = 0; i < entriesCount; i++)
                {
                    long field1Val = 0, field2Val = 0, field3Val = 0;

                    if (field1Len == 0)
                    {
                        field1Val = 1;
                    }
                    else
                    {
                        for (int j = 0; j < field1Len; j++)
                            field1Val = (field1Val << 8) | buffer[index++];
                    }

                    for (int j = 0; j < field2Len; j++)
                        field2Val = (field2Val << 8) | buffer[index++];

                    for (int j = 0; j < field3Len; j++)
                        field3Val = (field3Val << 8) | buffer[index++];

                    switch (field1Val)
                    {
                        case 0:
                            // linked list of free objects
                            xref.Add(id, 0, (int)field3Val, true);
                            break;

                        case 1:
                            // objects that are in use but are not compressed
                            xref.Add(id, field2Val, (int)field3Val, false);
                            break;

                        case 2:
                            // compressed objects
                            xref.Add(id, new XRefEntry(id, field2Val, field3Val));
                            break;

                        default:
                            throw InvalidDataExceptionByCode(ErrorCodes.XRefStreamInvalidEntryType);
                    }

                    id++;
                }
            }

            return new Footer(xref, trailer);
        }
    }
}