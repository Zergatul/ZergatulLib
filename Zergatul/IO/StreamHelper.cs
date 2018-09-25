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
    }
}