using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class InternalStructure
    {
        public XRefTable XRef { get; internal set; }
        public List<Footer> Footers { get; } = new List<Footer>();
    }
}