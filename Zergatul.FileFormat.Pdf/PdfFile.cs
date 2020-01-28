using System;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;

    public class PdfFile
    {
        private const int BufferSize = 1024;
        private static readonly byte[] HeaderMarker = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };
        private static readonly byte[] StartXRefMarker = new byte[] { 0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66 };

        public string Version { get; private set; }
        public InternalStructure Structure { get; }

        private Stream _stream;
        private byte[] _buffer;
        private int _bufOffset, _bufLength;
        private Parser _parser;
        private StringBuilder _sb;

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

            _stream = stream;

            Structure = new InternalStructure();

            _buffer = new byte[BufferSize];
            _parser = new Parser(NextByte);
            _sb = new StringBuilder();

            ParseHeader();
            GoToStartXRef();
            GoToMainXref();
            ParseXRefTable();
            ParseIncrementalUpdates();
        }

        private void ParseHeader()
        {
            const int MaxHeaderLength = 15;

            StreamHelper.ReadArray(_stream, _buffer, MaxHeaderLength);
            for (int i = 0; i < HeaderMarker.Length; i++)
                if (_buffer[i] != HeaderMarker[i])
                    throw new InvalidDataException("Invalid PDF header.");

            for (int i = 0; i < MaxHeaderLength; i++)
            {
                if (_buffer[i] >= 0x80)
                    throw new InvalidDataException("Non-ASCII character in PDF version.");
                if (IsEndOfLine(_buffer[i]))
                {
                    Version = Encoding.ASCII.GetString(_buffer, HeaderMarker.Length, i - HeaderMarker.Length);
                    return;
                }
            }

            throw new InvalidDataException("Invalid PDF version.");
        }

        private void ParseXRefTable()
        {
            ResetBuffer();
            _parser.Reset();

            var token = _parser.NextToken();
            var @static = token as StaticToken;
            if (@static == null)
                throw new InvalidDataException("Invalid xref data.");

            if (@static.Value == "xref")
            {
                var xref = new XRefTable();
                Structure.ListXRefs.Add(xref);

                while (true)
                {
                    token = _parser.NextToken();
                    var integer = token as IntegerToken;
                    if (integer == null)
                    {
                        if (xref.Count == 0)
                        {
                            // table should have at least one record
                            throw new InvalidDataException("Invalid xref object number.");
                        }
                        else
                        {
                            ParseTrailer(token);
                            return;
                        }
                    }
                    long objectNumber = integer.Value;

                    token = _parser.NextToken();
                    integer = token as IntegerToken;
                    if (integer == null)
                        throw new InvalidDataException("Invalid xref object count.");
                    long objectCount = integer.Value;

                    for (long i = 0; i < objectCount; i++)
                    {
                        token = _parser.NextToken();
                        integer = token as IntegerToken;
                        if (integer == null)
                            throw new InvalidDataException("Invalid xref entry offset.");
                        long offset = integer.Value;

                        token = _parser.NextToken();
                        integer = token as IntegerToken;
                        if (integer == null)
                            throw new InvalidDataException("Invalid xref entry generation number.");
                        int generation = checked((int)integer.Value);

                        token = _parser.NextToken();
                        @static = token as StaticToken;
                        if (@static?.Value != "n" && @static?.Value != "f")
                            throw new InvalidDataException("Invalid xref entry in-use marker.");
                        bool free = @static.Value == "f";

                        xref.Add(objectNumber + i, offset, generation, free);
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ParseTrailer(TokenBase token)
        {
            var @static = token as StaticToken;
            if (@static?.Value != "trailer")
                throw new InvalidDataException("trailer expected.");

            token = _parser.NextToken();
            var dictionary = token as DictionaryToken;
            if (dictionary == null)
                throw new InvalidDataException("trailer should have dictionary token.");

            Structure.ListTrailers.Add(new TrailerDictionary(dictionary));
        }

        private void ParseIncrementalUpdates()
        {
            while (true)
            {
                var lastTrailer = Structure.ListTrailers[Structure.ListTrailers.Count - 1];
                if (lastTrailer.Prev == null)
                    break;

                throw new NotImplementedException();

                _stream.Position = lastTrailer.Prev.Value;
                ParseXRefTable();
            }
        }

        private void GoToStartXRef()
        {
            bool found = false;

            int markerLength = StartXRefMarker.Length;
            long position = _stream.Length - markerLength;
            while (position > 0)
            {
                position = System.Math.Max(position - BufferSize + markerLength, 0);
                _stream.Position = position;
                int read = ReadBuffer();

                for (int i = read - 1; i >= markerLength; i--)
                {
                    found = true;
                    for (int j = 0; j < markerLength; j++)
                        if (_buffer[i - j] != StartXRefMarker[markerLength - j - 1])
                        {
                            found = false;
                            break;
                        }

                    if (!found)
                        continue;

                    _stream.Position = position + i + 1 - markerLength;
                    break;
                }

                if (found)
                    break;
            }

            if (!found)
                throw new InvalidDataException("Cannot locate startxref section.");
        }

        private void GoToMainXref()
        {
            ResetBuffer();
            _parser.Reset();

            EnsureStaticToken("startxref", "startxref expected.");
            var token = _parser.NextToken();
            var integer = token as IntegerToken;
            if (integer == null)
                throw new InvalidDataException("Invalid startxref value.");

            _stream.Position = integer.Value;
        }

        private void EnsureStaticToken(string value, string message)
        {
            var token = _parser.NextToken();
            if ((token as StaticToken)?.Value != value)
            {
                throw new InvalidDataException(message);
            }
        }

        private void ResetBuffer()
        {
            _bufOffset = _bufLength = 0;
        }

        private int NextByte()
        {
            if (_bufOffset >= _bufLength)
            {
                if (_stream.Position == _stream.Length)
                    throw new EndOfStreamException();

                _bufOffset = 0;
                _bufLength = _stream.Read(_buffer, 0, BufferSize);

                if (_bufLength == 0)
                    throw new EndOfStreamException("Unexpected end of stream.");
            }

            return _buffer[_bufOffset++];
        }

        private int ReadBuffer()
        {
            int read = (int)System.Math.Min(BufferSize, _stream.Length - _stream.Position);
            StreamHelper.ReadArray(_stream, _buffer, read);
            return read;
        }
    }
}