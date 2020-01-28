namespace Zergatul.FileFormat.Pdf.Token
{
    internal class NullToken : TokenBase
    {
        public static NullToken Instance { get; } = new NullToken();

        public override bool IsBasic => true;

        private NullToken()
        {

        }
    }
}