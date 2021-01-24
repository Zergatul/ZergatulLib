using System.Collections.Generic;
using Zergatul.FileFormat.Pdf.Token;

namespace Zergatul.FileFormat.Pdf
{
    using static ExceptionHelper;
    using static StringConstants;

    public class Page
    {
        public PageActions Actions { get; }
        public IndirectObject Parent { get; }

        internal Page(DictionaryToken dictionary)
        {
            if (!dictionary.ValidateName(Type, "Page"))
                throw InvalidDataExceptionByCode(ErrorCodes.PageTypeInvalidValue);

            if (!dictionary.TryGet("Parent", out IndirectReferenceToken parent))
                throw InvalidDataExceptionByCode(ErrorCodes.PageParentExpected);
            Parent = new IndirectObject(parent);

            //LastModified
            //Resources
            //MediaBox
            //CropBox
            //BleedBox
            //TrimBox
            //ArtBox
            //BoxColorInfo
        }
    }
}