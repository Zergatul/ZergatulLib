using System;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;

    public class XRefEntry
    {
        public long Id { get; }
        public long Offset { get; }
        public int Generation { get; }
        public bool Free { get; }
        public long StreamObjectId { get; }
        public long ObjectIndex { get; }
        public bool Compressed { get; }

        public XRefEntry(long id, long offset, int generation, bool free)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "id cannot be negative.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset cannot be negative.");
            if (offset > 9999999999)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset too big");
            if (generation < 0)
                throw new ArgumentOutOfRangeException(nameof(generation), "generation cannot be negative.");
            if (generation > MaxGeneration)
                throw new ArgumentOutOfRangeException(nameof(generation), "generation too big.");

            Id = id;
            Offset = offset;
            Generation = generation;
            Free = free;
        }

        public XRefEntry(long id, long streamObjectId, long objectIndex)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "id cannot be negative.");
            if (streamObjectId < 0)
                throw new ArgumentOutOfRangeException(nameof(streamObjectId), "streamObjectId cannot be negative.");
            if (objectIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(objectIndex), "objectIndex cannot be negative.");

            Id = id;
            StreamObjectId = streamObjectId;
            ObjectIndex = objectIndex;
            Compressed = true;
        }
    }
}