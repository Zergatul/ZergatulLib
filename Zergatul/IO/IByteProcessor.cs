namespace Zergatul.IO
{
    internal interface IByteProcessor
    {
        int Available { get; }
        bool IsFinished { get; }
        int ReadBufferSize { get; }
        void Decode();
        void Get(byte[] buffer, int offset, int count);
        void Process(byte[] buffer, int offset, int count);
    }
}