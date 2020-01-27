namespace Zergatul.FileFormat.Pdf.Token
{
    internal class NullToken : TokenBase
    {
        public static NullToken Instance { get; } = new NullToken();

        private NullToken()
        {

        }
    }
}