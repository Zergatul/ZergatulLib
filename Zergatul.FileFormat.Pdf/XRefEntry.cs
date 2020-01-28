using System;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;

    public class XRefEntry
    {
        public long Offset { get; }
        public int Generation { get; }
        public bool Free { get; }

        public XRefEntry(long offset, int generation, bool free)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset cannot be negative.");
            if (offset > 9999999999)
                throw new ArgumentOutOfRangeException(nameof(offset), "offset too big");
            if (generation < 0)
                throw new ArgumentOutOfRangeException(nameof(generation), "generation cannot be negative.");
            if (generation > MaxGeneration)
                throw new ArgumentOutOfRangeException(nameof(generation), "generation too big.");

            Offset = offset;
            Generation = generation;
            Free = free;
        }
    }
}