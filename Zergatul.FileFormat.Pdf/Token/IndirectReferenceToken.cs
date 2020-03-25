using System;

namespace Zergatul.FileFormat.Pdf.Token
{
    using static Common;

    internal class IndirectReferenceToken : TokenBase
    {
        public override bool IsBasic => true;

        public long Id { get; }
        public int Generation { get; }

        public IndirectReferenceToken(long id, int generation)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            if (generation < 0 || generation > MaxGeneration)
                throw new ArgumentOutOfRangeException(nameof(generation));

            Id = id;
            Generation = generation;
        }

#if DEBUG
        public override string ToString()
        {
            return $"{Id} {Generation} R";
        }
#endif
    }
}