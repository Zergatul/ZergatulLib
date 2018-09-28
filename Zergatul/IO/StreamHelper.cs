using System;
using System.IO;

namespace Zergatul.IO
{
    public static class StreamHelper
    {
        public static void ReadArray(Stream stream, byte[] data)
        {
            int index = 0;
            while (index < data.Length)
            {
                int read = stream.Read(data, index, data.Length - index);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        public static void ReadArray(Stream stream, byte[] data, int length)
        {
            int index = 0;
            while (index < length)
            {
                int read = stream.Read(data, index, length - index);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        public static bool ValidateWriteParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (count == 0)
                return true;

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            return false;
        }
    }
}