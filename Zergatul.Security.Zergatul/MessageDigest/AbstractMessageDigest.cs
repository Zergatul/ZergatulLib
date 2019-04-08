using System;

namespace Zergatul.Security.Zergatul.MessageDigest
{
    abstract class AbstractMessageDigest : Security.MessageDigest
    {
        protected byte[] buffer;
        protected int bufOffset;

        public override void Update(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            while (length > 0)
            {
                int copy = System.Math.Min(buffer.Length - bufOffset, length);
                Buffer.BlockCopy(data, offset, buffer, bufOffset, copy);

                offset += copy;
                length -= copy;
                bufOffset += copy;

                if (bufOffset == buffer.Length)
                {
                    bufOffset = 0;
                    ProcessBlock();
                }
            }
        }

        protected abstract void ProcessBlock();
    }
}