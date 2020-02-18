using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zergatul.FileFormat.Pdf.Token;
using Zergatul.IO;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;
    using static StringConstants;

    public class PdfFile
    {
        private const int BufferSize = 1024;
        private static readonly byte[] HeaderMarker = new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D };
        private static readonly byte[] StartXRefMarker = new byte[] { 0x73, 0x74, 0x61, 0x72, 0x74, 0x78, 0x72, 0x65, 0x66 };

        public string Version { get; private set; }
        public byte[] OriginalDocumentId { get; private set; }
        public byte[] DocumentId { get; private set; }
        public InternalStructure Structure { get; }
        public DocumentCatalog Document { get; }

        private Stream _stream;
        private byte[] _buffer;
        private int _bufOffset, _bufLength;
        private Parser _parser;
        private StringBuilder _sb;
        private Dictionary<long, ObjectStream> _objectStreamCache;

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
            _objectStreamCache = new Dictionary<long, ObjectStream>();

            ParseHeader();
            GoToStartXRef();
            GoToMainXref();
            ParseXRefTable();
            ParseIncrementalUpdates();

            ProcessTrailers();

            ParseDocumentCatalog();
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

            var token = _parser.NextToken(allowObj: true);

            var beginObj = token as BeginObjectToken;
            if (beginObj != null)
            {
                ParseXRefStream(new StreamObject(_stream, _parser));
                return;
            }

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

            throw new InvalidDataException("Cannot parse xref table.");
        }

        private void ParseXRefStream(StreamObject streamObj)
        {
            var dictionary = streamObj.Dictionary;

            if (!dictionary.ContainsKey(Type))
                throw new InvalidDataException("XRef stream dictionary must contain Type key.");
            if (!dictionary.Is<NameToken>(Type))
                throw new InvalidDataException("XRef stream dictionary Type key should be name.");
            if (dictionary.GetName(Type) != "XRef")
                throw new InvalidDataException("XRef stream dictionary Type key should have value XRef.");

            if (!dictionary.ContainsKey(Size))
                throw new InvalidDataException("XRef stream dictionary must contain Size key.");
            if (!dictionary.Is<IntegerToken>(Size))
                throw new InvalidDataException("XRef stream dictionary Size key should be integer.");

            if (dictionary.ContainsKey(Prev))
            {
                if (!dictionary.Is<IntegerToken>(Prev))
                    throw new InvalidDataException("XRef stream dictionary Prev key should be integer.");
            }

            if (!dictionary.ContainsKey(W))
                throw new InvalidDataException("XRef stream dictionary must contain W key.");
            if (!dictionary.Is<ArrayToken>(W))
                throw new InvalidDataException("XRef stream dictionary W key should be array.");
            var array = dictionary.GetArrayNullable(W);
            bool ok =
                array.Value.Count == 3 &&
                array.Value[0] is IntegerToken &&
                array.Value[1] is IntegerToken &&
                array.Value[2] is IntegerToken;
            if (!ok)
                throw new InvalidDataException("XRef stream dictionary W key should be array with 3 integers.");

            int field1Len = (int)((IntegerToken)array.Value[0]).Value;
            int field2Len = (int)((IntegerToken)array.Value[1]).Value;
            int field3Len = (int)((IntegerToken)array.Value[2]).Value;
            int total = field1Len + field2Len + field3Len;

            if (field1Len < 0 || field2Len < 0 || field3Len < 0)
                throw new InvalidDataException("XRef stream dictionary W key should be array with 3 non-negative integers.");
            if (field1Len > 7 || field2Len > 7 || field3Len > 7)
                throw new InvalidDataException("XRef stream dictionary W key field length overflow.");

            var xref = new XRefTable();
            Structure.ListXRefs.Add(xref);

            using (var ms = new MemoryStream())
            {
                streamObj.Data.CopyTo(ms);
                if (ms.Length % total != 0)
                    throw new InvalidDataException("XRef stream size is not multiple of size of fields.");

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
                            throw new InvalidDataException("Invalid entry type in XRef stream.");
                    }

                    id++;
                }
            }

            _stream.Position = streamObj.RawOffset + streamObj.Length;
            ResetBuffer();

            Structure.ListTrailers.Add(new TrailerDictionary(streamObj.Dictionary));

            if (dictionary.ContainsKey(Prev))
            {
                // parse previous xref
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

                _stream.Position = lastTrailer.Prev.Value;
                ParseXRefTable();
            }
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
                throw new InvalidDataException("")
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
                var parser = new Parser(objStream.Data, offset, endOffset);
                return parser.NextToken();
            }

            _stream.Position = entry.Offset;
            ResetBuffer();

            throw new NotImplementedException();
        }

        private ObjectStream ParseObjectStream(XRefEntry entry)
        {
            if (entry.Compressed)
                throw new InvalidDataException("Compressed object cannot be inside another compressed object.");

            if (entry.Free)
                throw new InvalidDataException("Compressed object references to free object.");

            _stream.Position = entry.Offset;
            ResetBuffer();

            var token = _parser.NextToken(allowObj: true);
            var beginObj = token as BeginObjectToken;
            if (beginObj == null)
                throw new InvalidDataException("BeginObjectToken expected.");

            if (beginObj.Id != entry.Id)
                throw new InvalidDataException("Object numbers mismatch.");
            if (beginObj.Generation != entry.Generation)
                throw new InvalidDataException("Object generation mismatch.");

            var streamObj = new StreamObject(_stream, _parser);
            var dictionary = streamObj.Dictionary;

            const string Type = nameof(Type);
            const string N = nameof(N);
            const string First = nameof(First);

            if (!dictionary.ContainsKey(Type))
                throw new InvalidDataException("Object stream dictionary must contain [Type] key.");
            if (!dictionary.Is<NameToken>(Type))
                throw new InvalidDataException("Object stream dictionary [Type] key should be name.");
            if (dictionary.GetName(Type) != "ObjStm")
                throw new InvalidDataException("Object stream dictionary [Type] key should have value ObjStm.");

            if (!dictionary.ContainsKey(N))
                throw new InvalidDataException("Object stream dictionary must contain [N] key.");
            if (!dictionary.Is<IntegerToken>(N))
                throw new InvalidDataException("Object stream dictionary [N] key should be integer.");

            if (!dictionary.ContainsKey(First))
                throw new InvalidDataException("Object stream dictionary must contain [First] key.");
            if (!dictionary.Is<IntegerToken>(First))
                throw new InvalidDataException("Object stream dictionary [First] key should be integer.");

            return new ObjectStream(streamObj.Data, dictionary.GetInteger(N), dictionary.GetInteger(First));
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

            EnsureStaticToken("startxref", "startxref expected.");
            var token = _parser.NextToken();
            var integer = token as IntegerToken;
            if (integer == null)
                throw new InvalidDataException("Invalid startxref value.");

            _stream.Position = integer.Value;
        }

        private void ProcessTrailers()
        {
            var trailer = Structure.ListTrailers[0];
            if (trailer.OriginalId != null)
                OriginalDocumentId = trailer.OriginalId;
            if (trailer.Id != null)
                DocumentId = trailer.Id;

            Structure.XRef = Structure.ListXRefs[Structure.ListXRefs.Count - 1].Clone();
            for (int i = Structure.ListXRefs.Count - 2; i >= 0; i--)
                Structure.XRef.MergeWith(Structure.ListXRefs[i]);

            // Any object in a cross-reference section whose number is greater than Trailer.Size is ignored and considered missing
            var remove = new List<long>();
            foreach (long id in Structure.XRef.Keys)
                if (id > trailer.Size)
                    remove.Add(id);

            foreach (long id in remove)
                Structure.XRef.Remove(id);
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
            _parser.Reset(_stream.Position);
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