using System;
using System.IO;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;
using Zergatul.IO.Compression;

namespace Zergatul.FileFormat.Pdf
{
    using static ExceptionHelper;
    using S = StringConstants;

    internal class StreamObject
    {
        public long RawOffset { get; }
        public long Length { get; }
        public Stream Data { get; }
        public DictionaryToken Dictionary { get; }

        public StreamObject(Stream stream, TokenParser parser)
        {
            var token = parser.NextToken();
            Dictionary = token as DictionaryToken;
            if (Dictionary == null)
                throw new InvalidDataException();

            if (!Dictionary.ContainsKey(nameof(S.Length)))
                throw InvalidDataExceptionByCode(ErrorCodes.StreamDictionaryLengthExpected);
            if (!Dictionary.Is<IntegerToken>(nameof(S.Length)))
                throw InvalidDataExceptionByCode(ErrorCodes.StreamDictionaryLengthInvalidToken);

            Length = Dictionary.GetInteger(nameof(S.Length));

            var filter = Dictionary.GetTokenNullable(S.Filter);
            if (filter != null && !(filter is NameToken) && !(filter is ArrayToken))
                throw InvalidDataExceptionByCode(ErrorCodes.StreamDictionaryFilterInvalidToken);

            var decodeParams = Dictionary.GetTokenNullable(S.DecodeParms);
            if (decodeParams != null && !(decodeParams is DictionaryToken) && !(decodeParams is ArrayToken))
                throw InvalidDataExceptionByCode(ErrorCodes.StreamDictionaryDecodeParamsInvalidToken);

            token = parser.NextToken();
            var @static = token as StaticToken;
            if (@static?.Value != "stream")
                throw InvalidDataExceptionByCode(ErrorCodes.StreamExpected);

            parser.SkipStrongRuleEndOfLine();
            RawOffset = parser.Position;

            Data = CreateStream(stream, filter, decodeParams);
        }

        private Stream CreateStream(Stream stream, TokenBase filter, TokenBase decodeParams)
        {
            byte[] raw = new byte[Length];
            stream.Position = RawOffset;
            StreamHelper.ReadArray(stream, raw);

            switch (filter)
            {
                case NameToken name:
                    switch (name.Value)
                    {
                        case S.FlateDecode:
                            var zlib = new ZlibStream(new MemoryStream(raw), CompressionMode.Decompress);
                            return zlib;

                        default:
                            throw new NotImplementedException($"Stream filter ${name.Value} not implemented.");
                    }

                case ArrayToken array:
                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException();
            }
        }

        //public void SkipEndOfStreamToken(TokenParser parser)
        //{
        //    var token = parser.NextToken();
        //    var @static = token as StaticToken;
        //    if (@static?.Value != "endstream")
        //        throw new InvalidDataException("endstream token expected.");

        //    token = parser.NextToken();
        //    @static = token as StaticToken;
        //    if (@static?.Value != "endobj")
        //        throw new InvalidDataException("endobj token expected.");
        //}
    }
}