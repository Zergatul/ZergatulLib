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
                throw new InvalidDataException();
            if (!Dictionary.Is<IntegerToken>(nameof(Length)))
                throw new InvalidDataException();

            Length = Dictionary.GetInteger(nameof(Length));

            var filter = Dictionary.GetTokenNullable("Filter");
            if (filter != null && !(filter is NameToken) && !(filter is ArrayToken))
                throw new InvalidDataException();

            var decodeParams = Dictionary.GetTokenNullable("DecodeParms");
            if (decodeParams != null && !(decodeParams is DictionaryToken) && !(decodeParams is ArrayToken))
                throw new InvalidDataException();

            token = parser.NextToken();
            var @static = token as StaticToken;
            if (@static?.Value != "stream")
                throw new InvalidDataException();

            parser.SkipStrongRuleEndOfLine();

            Data = CreateStream(stream, parser.Position, filter, decodeParams);
        }

        private Stream CreateStream(Stream stream, long position, TokenBase filter, TokenBase decodeParams)
        {
            byte[] raw = new byte[Length];
            stream.Position = position;
            StreamHelper.ReadArray(stream, raw);

            switch (filter)
            {
                case NameToken name:
                    switch (name.Value)
                    {
                        case "FlateDecode":
                            var zlib = new ZlibStream(new MemoryStream(raw), CompressionMode.Decompress);
                            using (var sr = new StreamReader(zlib))
                            {
                                string x = sr.ReadToEnd();
                            }
                            return zlib;

                        default:
                            throw new NotImplementedException($"Stream filter ${name.Value} not implemented.");
                    }
                    break;

                case ArrayToken array:
                    throw new NotImplementedException();

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}