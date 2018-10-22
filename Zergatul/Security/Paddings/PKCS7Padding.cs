using System;

namespace Zergatul.Security.Paddings
{
    class PKCS7Padding : SymmetricPadding
    {
        public override byte[] GetPadding(int position, int blockSize)
        {
            if (position < 0 || position >= blockSize)
                throw new InvalidOperationException();

            byte[] padding = new byte[blockSize - position];
            for (int i = 0; i < padding.Length; i++)
                padding[i] = (byte)padding.Length;

            return padding;
        }

        public override int RemovePadding(byte[] block, int offset, int length)
        {
            int fill = block[offset + length - 1];
            if (fill > length)
                return -1;

            for (int i = offset + length - fill; i > offset + length; i++)
                if (block[i] != fill)
                    return -1;

            return length - fill;
        }
    }
}