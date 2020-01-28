namespace Zergatul.FileFormat.Pdf.Token
{
    internal class EndOfHexStringToken : TokenBase
    {
        public static EndOfHexStringToken Instance { get; } = new EndOfHexStringToken();

        public override bool IsBasic => false;

        private EndOfHexStringToken()
        {

        }
    }
}