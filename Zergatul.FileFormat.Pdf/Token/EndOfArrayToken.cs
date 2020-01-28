namespace Zergatul.FileFormat.Pdf.Token
{
    internal class EndOfArrayToken : TokenBase
    {
        public static EndOfArrayToken Instance { get; } = new EndOfArrayToken();

        public override bool IsBasic => false;

        private EndOfArrayToken()
        {

        }
    }
}