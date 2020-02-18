using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;
using Zergatul.IO.Compression;

namespace Zergatul.FileFormat.Pdf
{
    internal class StreamObject
    {
        public long RawOffset { get; }
        public long Length { get; }
        public Stream Data { get; }
        public DictionaryToken Dictionary { get; }

        public StreamObject(Stream stream, Parser parser)
        {
            var token = parser.NextToken();
            Dictionary = token as DictionaryToken;
            if (Dictionary == null)
                throw new InvalidDataException();

            if (!Dictionary.ContainsKey(nameof(Length)))
                throw new InvalidDataException("Stream dictionary must contain Length key.");
            if (!Dictionary.Is<IntegerToken>(nameof(Length)))
                throw new InvalidDataException("Stream dictionary Length key should be interger.");

            Length = Dictionary.GetInteger(nameof(Length));

            var filter = Dictionary.GetTokenNullable("Filter");
            if (filter != null && !(filter is NameToken) && !(filter is ArrayToken))
                throw new InvalidDataException("Stream dictionary Filter key should be name or array.");

            var decodeParams = Dictionary.GetTokenNullable("DecodeParms");
            if (decodeParams != null && !(decodeParams is DictionaryToken) && !(decodeParams is ArrayToken))
                throw new InvalidDataException("Stream dictionary DecodeParms key should be dictionary or array.");

            token = parser.NextToken();
            var @static = token as StaticToken;
            if (@static?.Value != "stream")
                throw new InvalidDataException();

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
                        case "FlateDecode":
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

        public void SkipEndOfStreamToken(Parser parser)
        {
            var token = parser.NextToken();
            var @static = token as StaticToken;
            if (@static?.Value != "endstream")
                throw new InvalidDataException("endstream token expected.");

            token = parser.NextToken();
            @static = token as StaticToken;
            if (@static?.Value != "endobj")
                throw new InvalidDataException("endobj token expected.");
        }
    }
}