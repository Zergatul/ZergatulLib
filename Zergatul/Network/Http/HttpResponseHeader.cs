namespace Zergatul.Network.Http
{
    public static class HttpResponseHeader
    {
        // General
        public const string CacheControl = "Cache-Control";
        public const string Connection = "Connection";
        public const string Date = "Date";
        public const string Pragma = "Pragma";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string Via = "Via";
        public const string Warning = "Warning";

        // Response
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLength = "Content-Length";
        public const string ContentType = "Content-Type";
        public const string KeepAlive = "Keep-Alive";
        public const string Server = "Server";

        // Web Socket
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
    }
}