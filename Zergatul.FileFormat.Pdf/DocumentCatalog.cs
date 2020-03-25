using System.Collections.Generic;

namespace Zergatul.FileFormat.Pdf
{
    public class DocumentCatalog
    {
        public IReadOnlyList<Page> Pages { get; internal set; }
    }
}