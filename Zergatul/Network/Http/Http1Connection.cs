using System.Diagnostics;
using System.IO;

namespace Zergatul.Network.Http
{
    public abstract class Http1Connection
    {
        public abstract Stream Stream { get; }
        public Stopwatch Timer { get; set; }
        public int Timeout { get; set; }

        protected Http1Connection()
        {
            Timer = new Stopwatch();
            Timeout = 0;
        }

        public abstract void Close();
        public abstract void CloseUnderlyingStream();
    }
}