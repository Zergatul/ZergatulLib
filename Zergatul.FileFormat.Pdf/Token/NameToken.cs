namespace Zergatul.FileFormat.Pdf.Token
{
    internal class NameToken : TokenBase
    {
        public override bool IsBasic => true;
        public string Value { get; }

        public NameToken(string value)
        {
            Value = value;
        }
    }
}