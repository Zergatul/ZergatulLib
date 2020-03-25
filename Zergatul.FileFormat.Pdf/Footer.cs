namespace Zergatul.FileFormat.Pdf
{
    public class Footer
    {
        public XRefTable XRef { get; }
        public TrailerDictionary Trailer { get; }

        public Footer(XRefTable xref, TrailerDictionary trailer)
        {
            XRef = xref;
            Trailer = trailer;
        }
    }
}