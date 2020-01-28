namespace Zergatul.FileFormat.Pdf.Token
{
    internal class StaticToken : TokenBase
    {
        public override bool IsBasic => false;
        public string Value { get; }

        public StaticToken(string value)
        {
            Value = value;
        }
    }
}