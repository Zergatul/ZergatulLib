namespace Zergatul.FileFormat.Pdf.Token
{
    internal class StaticToken : TokenBase
    {
        public string Value { get; }

        public StaticToken(string value)
        {
            Value = value;
        }
    }
}