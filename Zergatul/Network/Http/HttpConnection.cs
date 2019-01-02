using System.IO;

namespace Zergatul.Network.Http
{
    public abstract class HttpConnection
    {
        public Stream Stream { get; protected set; }

        public abstract void WriteHeader(byte[] data);
        public abstract void WriteBody(byte[] data);
        public abstract void WriteBody(Stream stream);
    }
}