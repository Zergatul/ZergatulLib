namespace Zergatul.FileFormat.Pdf.Token
{
    internal class IntegerToken : NumberToken
    {
        public long Value { get; }

        public IntegerToken(long value)
        {
            Value = value;
        }
    }
}