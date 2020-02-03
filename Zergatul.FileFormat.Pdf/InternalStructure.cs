using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class InternalStructure
    {
        public XRefTable XRef { get; internal set; }

        public List<XRefTable> ListXRefs { get; } = new List<XRefTable>();
        public List<TrailerDictionary> ListTrailers { get; } = new List<TrailerDictionary>();
    }
}