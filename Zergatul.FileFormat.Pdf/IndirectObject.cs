using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf
{
    public class IndirectObject
    {
        public long Id { get; }
        public int Generation { get; }

        internal IndirectObject(IndirectReferenceToken token)
        {
            Id = token.Id;
            Generation = token.Generation;
        }

        public override string ToString() => $"{Id} {Generation} R";
    }
}