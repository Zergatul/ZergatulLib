using System;

namespace Zergatul.Network.Http
{
    public class HttpResponse : IDisposable
    {
        private KeepAliveConnection _connection;

        public HttpResponse(KeepAliveConnection connection)
        {
            this._connection = connection;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}