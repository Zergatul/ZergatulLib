namespace Zergatul.IO
{
    internal interface IByteProcessor
    {
        int Available { get; }
        bool IsFinished { get; }
        int ReadBufferSize { get; }
        void Add(byte[] buffer, int offset, int count);
        void Decode();
        void Get(byte[] buffer, int offset, int count);
    }
}