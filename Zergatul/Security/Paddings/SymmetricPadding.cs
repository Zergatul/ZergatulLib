namespace Zergatul.Security.Paddings
{
    abstract class SymmetricPadding
    {
        public abstract byte[] GetPadding(int position, int blockSize);
        public abstract int RemovePadding(byte[] block, int offset, int length);
    }
}