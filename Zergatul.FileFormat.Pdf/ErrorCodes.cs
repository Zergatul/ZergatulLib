namespace Zergatul.FileFormat.Pdf
{
    public static class ErrorCodes
    {
        private static int _index = 1;

        public static readonly ErrorCode InvalidPdfHeader = new ErrorCode(_index++, "Invalid PDF header.");
        public static readonly ErrorCode InvalidVersionCharacter = new ErrorCode(_index++, "Non-ASCII character in PDF version.");
        public static readonly ErrorCode InvalidVersion = new ErrorCode(_index++, "Invalid PDF version.");

        public static readonly ErrorCode CannotLocateXRef = new ErrorCode(_index++, "Cannot locate startxref section.");
        public static readonly ErrorCode StartXRefExpected = new ErrorCode(_index++, "\"startxref\" expected.");
        public static readonly ErrorCode InvalidStartXRefValue = new ErrorCode(_index++, "Invalid \"startxref\" value.");
        public static readonly ErrorCode XRefExpected = new ErrorCode(_index++, "\"xref\" expected.");
        public static readonly ErrorCode XRefEmpty = new ErrorCode(_index++, "XRef table has no records.");
        public static readonly ErrorCode XRefObjectCountInvalidToken = new ErrorCode(_index++, "XRef object count should be integer.");
        public static readonly ErrorCode XRefEntryOffsetInvalidToken = new ErrorCode(_index++, "XRef entry offset should be integer.");
        public static readonly ErrorCode XRefEntryGenNumberInvalidToken = new ErrorCode(_index++, "XRef entry generation number should be integer.");
        public static readonly ErrorCode XRefEntryInUseMarkerInvalid = new ErrorCode(_index++, "XRef entry in-use marker should be \"n\" or \"f\".");
        public static readonly ErrorCode XRefEntryNegativeId = new ErrorCode(_index++, "XRefEntry Id cannot be negative.");
        public static readonly ErrorCode XRefEntryNegativeOffset = new ErrorCode(_index++, "XRefEntry Offset cannot be negative.");
        public static readonly ErrorCode XRefEntryOffsetOverflow = new ErrorCode(_index++, "XRefEntry Offset too big.");
        public static readonly ErrorCode XRefEntryNegativeGeneration = new ErrorCode(_index++, "XRefEntry Generation cannot be negative.");
        public static readonly ErrorCode XRefEntryGenerationOverflow = new ErrorCode(_index++, "XRefEntry Generation too big.");
        public static readonly ErrorCode XRefEntryNegativeStreamObjectId = new ErrorCode(_index++, "XRefEntry StreamObjectId cannot be negative.");
        public static readonly ErrorCode XRefEntryNegativeObjectIndex = new ErrorCode(_index++, "XRefEntry ObjectIndex cannot be negative.");

        public static readonly ErrorCode XRefStreamTypeExpected = new ErrorCode(_index++, "XRef stream dictionary must contain /Type key.");
        public static readonly ErrorCode XRefStreamTypeInvalidToken = new ErrorCode(_index++, "XRef stream dictionary /Type key should be name.");
        public static readonly ErrorCode XRefStreamTypeInvalidValue = new ErrorCode(_index++, "XRef stream dictionary /Type key should have /XRef value.");
        public static readonly ErrorCode XRefStreamSizeExpected = new ErrorCode(_index++, "XRef stream dictionary must contain /Size key.");
        public static readonly ErrorCode XRefStreamSizeInvalidToken = new ErrorCode(_index++, "XRef stream dictionary /Size key should be integer.");
        public static readonly ErrorCode XRefStreamPrevInvalidToken = new ErrorCode(_index++, "XRef stream dictionary /Prev key should be integer.");
        public static readonly ErrorCode XRefStreamWExpected = new ErrorCode(_index++, "XRef stream dictionary must contain /W key.");
        public static readonly ErrorCode XRefStreamWInvalidToken = new ErrorCode(_index++, "XRef stream dictionary /W key should be array.");
        public static readonly ErrorCode XRefStreamWInvalidValue = new ErrorCode(_index++, "XRef stream dictionary /W key should be array with 3 integers.");
        public static readonly ErrorCode XRefStreamWNegativeValue = new ErrorCode(_index++, "XRef stream dictionary /W key should be array with 3 non-negative integers.");
        public static readonly ErrorCode XRefStreamWFieldOverflow = new ErrorCode(_index++, "XRef stream dictionary /W key field length overflow.");
        public static readonly ErrorCode XRefStreamInvalidSize = new ErrorCode(_index++, "XRef stream size is not multiple of sum of field lengths.");
        public static readonly ErrorCode XRefStreamInvalidEntryType = new ErrorCode(_index++, "XRef stream invalid entry type.");
        //public static readonly ErrorCode XRefStream = new ErrorCode(_index++, "XRef stream");
        //public static readonly ErrorCode XRefStream = new ErrorCode(_index++, "XRef stream");
        //public static readonly ErrorCode XRefStream = new ErrorCode(_index++, "XRef stream");
        //public static readonly ErrorCode XRefStream = new ErrorCode(_index++, "XRef stream");
        //public static readonly ErrorCode XRefStream = new ErrorCode(_index++, "XRef stream");

        public static readonly ErrorCode TrailerExpected = new ErrorCode(_index++, "\"trailer\" expected.");
        public static readonly ErrorCode TrailerInvalidToken = new ErrorCode(_index++, "\"trailer\" section should have dictionary inside.");

        public static readonly ErrorCode StreamExpected = new ErrorCode(_index++, "\"stream\" expected.");
        public static readonly ErrorCode StreamDictionaryLengthExpected = new ErrorCode(_index++, "Stream dictionary must contain /Length key.");
        public static readonly ErrorCode StreamDictionaryLengthInvalidToken = new ErrorCode(_index++, "Stream dictionary /Length key should be integer.");
        public static readonly ErrorCode StreamDictionaryFilterInvalidToken = new ErrorCode(_index++, "Stream dictionary /Filter key should be name or array.");
        public static readonly ErrorCode StreamDictionaryDecodeParamsInvalidToken = new ErrorCode(_index++, "Stream dictionary /DecodeParms key should be dictionary or array.");

        public static readonly ErrorCode GenerationNumberMismatch = new ErrorCode(_index++, "Generation numbers don't match between reference and xref table.");
        public static readonly ErrorCode BeginObjectTokenExpected = new ErrorCode(_index++, "Begin object token expected.");

        public static readonly ErrorCode ObjectStreamCompressedEntry = new ErrorCode(_index++, "Compressed object cannot be inside another compressed object.");
        public static readonly ErrorCode ObjectStreamCompressedFree = new ErrorCode(_index++, "Compressed object references to free object.");
        public static readonly ErrorCode ObjectStreamBeginObjectTokenExpected = new ErrorCode(_index++, "BeginObjectToken expected.");
        public static readonly ErrorCode ObjectStreamObjectNumberMismatch = new ErrorCode(_index++, "Object numbers mismatch.");
        public static readonly ErrorCode ObjectStreamObjectGenerationMismatch = new ErrorCode(_index++, "Object generations mismatch.");
        public static readonly ErrorCode ObjectStreamTypeExpected = new ErrorCode(_index++, "Object stream dictionary must contain /Type key.");
        public static readonly ErrorCode ObjectStreamTypeInvalidToken = new ErrorCode(_index++, "Object stream dictionary /Type key should be name.");
        public static readonly ErrorCode ObjectStreamTypeInvalidValue = new ErrorCode(_index++, "Object stream dictionary /Type key should have value ObjStm.");
        public static readonly ErrorCode ObjectStreamNExpected = new ErrorCode(_index++, "Object stream dictionary must contain /N key.");
        public static readonly ErrorCode ObjectStreamNInvalidToken = new ErrorCode(_index++, "Object stream dictionary /N key should be integer.");
        public static readonly ErrorCode ObjectStreamFirstExpected = new ErrorCode(_index++, "Object stream dictionary must contain /First key.");
        public static readonly ErrorCode ObjectStreamFirstInvalidToken = new ErrorCode(_index++, "Object stream dictionary /First key should be integer.");


        public static readonly ErrorCode DocumentCatalogInvalidToken = new ErrorCode(_index++, "Document root should be dictionary.");
        public static readonly ErrorCode DocumentCatalogTypeInvalid = new ErrorCode(_index++, "Catalog dictionary should have /Type key with /Catalog value.");
        public static readonly ErrorCode DocumentCatalogPagesInvalid = new ErrorCode(_index++, "Catalog dictionary should have /Pages key with indirect reference.");

        public static readonly ErrorCode PagesInvalidToken = new ErrorCode(_index++, "Pages indirect reference should refer to dictionary.");
        public static readonly ErrorCode PagesTypeInvalid = new ErrorCode(_index++, "Pages dictionary should have /Type key with /Pages value.");
        public static readonly ErrorCode PagesKidsInvalid = new ErrorCode(_index++, "Pages dictionary should have /Kids key with array value.");
        public static readonly ErrorCode PagesKidsInvalidItems = new ErrorCode(_index++, "/Kids array should contain indirect references.");
        public static readonly ErrorCode PagesCountInvalid = new ErrorCode(_index++, "Pages dictionary should have /Count key with integer value.");
        public static readonly ErrorCode PagesCountMismatch = new ErrorCode(_index++, "Pages dictionary count does not match the actual pages count.");

        public static readonly ErrorCode PageInvalidToken = new ErrorCode(_index++, "Page indirect reference should refer to dictionary.");
        public static readonly ErrorCode PageTypeInvalidToken = new ErrorCode(_index++, "Page dictionary should have /Type key with name.");
        public static readonly ErrorCode PageTypeInvalidValue = new ErrorCode(_index++, "Page dictionary should have /Type key /Page or /Pages value.");
        public static readonly ErrorCode PageParentExpected = new ErrorCode(_index++, "Page dictionary should have /Parent key with indirect reference.");
        //

        //"Invalid xref entry offset."
    }
}