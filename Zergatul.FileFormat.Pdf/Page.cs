using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class Page
    {
        public PageActions Actions { get; }
        public IReadOnlyList<IndirectObject> Kids { get; }
        public IndirectObject Parent { get; }
    }
}