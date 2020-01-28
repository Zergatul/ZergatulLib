namespace Zergatul.FileFormat.Pdf.Token
{
    internal class EndOfDictionaryToken : TokenBase
    {
        public static EndOfDictionaryToken Instance { get; } = new EndOfDictionaryToken();

        public override bool IsBasic => false;

        private EndOfDictionaryToken()
        {

        }
    }
}