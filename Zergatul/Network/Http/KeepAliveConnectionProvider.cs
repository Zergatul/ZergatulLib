using System;
using System.IO;

namespace Zergatul.Network.Http
{
    public abstract class KeepAliveConnectionProvider
    {
        public abstract KeepAliveConnection GetConnection(Uri uri);
    }
}