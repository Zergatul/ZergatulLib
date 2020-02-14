using System;

namespace Zergatul.FileFormat.Pdf.Token
{
    using static Common;

    internal class BeginObjectToken : TokenBase
    {
        public override bool IsBasic => false;

        public long Id { get; }
        public int Generation { get; }

        public BeginObjectToken(long id, int generation)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id));
            if (generation < 0 || generation > MaxGeneration)
                throw new ArgumentOutOfRangeException(nameof(generation));

            Id = id;
            Generation = generation;
        }
    }
}