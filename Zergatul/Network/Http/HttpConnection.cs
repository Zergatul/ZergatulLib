using System.IO;

namespace Zergatul.Network.Http
{
    public abstract class HttpConnection
    {
        public abstract Stream Stream { get; }
        public abstract void Close();
        public abstract void CloseUnderlyingStream();
    }
}