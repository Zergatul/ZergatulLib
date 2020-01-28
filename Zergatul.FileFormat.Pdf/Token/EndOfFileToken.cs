namespace Zergatul.FileFormat.Pdf.Token
{
    internal class EndOfFileToken : TokenBase
    {
        public static EndOfFileToken Unexpected { get; } = new EndOfFileToken();
        public static EndOfFileToken Marker { get; } = new EndOfFileToken();

        public override bool IsBasic => false;

        private EndOfFileToken()
        {

        }
    }
}