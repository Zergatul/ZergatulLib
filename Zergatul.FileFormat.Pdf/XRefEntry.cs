namespace Zergatul.FileFormat.Pdf
{
    using static Common;
    using static ExceptionHelper;

    public class XRefEntry
    {
        public long Id { get; }
        public long Offset { get; }
        public int Generation { get; }
        public bool Free { get; }
        public long StreamObjectId { get; }
        public long ObjectIndex { get; }
        public bool Compressed { get; }

        public XRefEntry(long id, long offset, int generation, bool free)
        {
            if (id < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(id), ErrorCodes.XRefEntryNegativeId);
            if (offset < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(offset), ErrorCodes.XRefEntryNegativeOffset);
            if (offset > 9999999999)
                throw ArgumentOutOfRangeExceptionByCode(nameof(offset), ErrorCodes.XRefEntryOffsetOverflow);
            if (generation < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(generation), ErrorCodes.XRefEntryNegativeGeneration);
            if (generation > MaxGeneration)
                throw ArgumentOutOfRangeExceptionByCode(nameof(generation), ErrorCodes.XRefEntryGenerationOverflow);

            Id = id;
            Offset = offset;
            Generation = generation;
            Free = free;
        }

        public XRefEntry(long id, long streamObjectId, long objectIndex)
        {
            if (id < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(id), ErrorCodes.XRefEntryNegativeId);
            if (streamObjectId < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(streamObjectId), ErrorCodes.XRefEntryNegativeStreamObjectId);
            if (objectIndex < 0)
                throw ArgumentOutOfRangeExceptionByCode(nameof(objectIndex), ErrorCodes.XRefEntryNegativeObjectIndex);

            Id = id;
            StreamObjectId = streamObjectId;
            ObjectIndex = objectIndex;
            Compressed = true;
        }
    }
}