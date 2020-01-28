namespace Zergatul.FileFormat.Pdf.Token
{
    internal class IntegerToken : NumberToken
    {
        public override bool IsBasic => true;
        public long Value { get; }

        public IntegerToken(long value)
        {
            Value = value;
        }
    }
}