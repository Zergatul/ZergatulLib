using System;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.FileFormat.Pdf.Token
{
    internal class ObjectStream : TokenBase
    {
        public override bool IsBasic => false;
        public byte[] Data { get; }
        public IReadOnlyList<ObjectStreamEntry> Objects { get; }

        public ObjectStream(Stream stream, long objectCount, long first)
        {
            byte[] raw;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                raw = ms.ToArray();
            }

            var objects = new ObjectStreamEntry[objectCount];
            Objects = objects;

            var parser = new NumberParser(raw, 0, first);
            long prevOffset = -1;
            for (int i = 0; i < objectCount; i++)
            {
                if (!parser.MoveNext())
                    throw new InvalidDataException("Cannot parse object stream.");
                long id = parser.Current;

                if (!parser.MoveNext())
                    throw new InvalidDataException("Cannot parse object stream.");
                long offset = parser.Current;

                if (offset <= prevOffset)
                    throw new InvalidDataException("Offsets must be in increasing order.");
                prevOffset = offset;

                objects[i] = new ObjectStreamEntry(id, offset);
            }

            Data = new byte[raw.Length - first];
            Array.Copy(raw, first, Data, 0, Data.Length);
        }

        public struct ObjectStreamEntry
        {
            public long Id { get; }
            public long Offset { get; }

            public ObjectStreamEntry(long id, long offset)
            {
                Id = id;
                Offset = offset;
            }
        }
    }
}