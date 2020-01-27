namespace Zergatul.FileFormat.Pdf.Token
{
    internal class NameToken : TokenBase
    {
        public string Value { get; }

        public NameToken(string value)
        {
            Value = value;
        }
    }
}