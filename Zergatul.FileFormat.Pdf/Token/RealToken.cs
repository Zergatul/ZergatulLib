namespace Zergatul.FileFormat.Pdf.Token
{
    internal class RealToken : NumberToken
    {
        public decimal Value { get; }

        public RealToken(decimal value)
        {
            Value = value;
        }
    }
}