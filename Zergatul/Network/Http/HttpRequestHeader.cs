namespace Zergatul.Network.Http
{
    public static class HttpRequestHeader
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

        // Request
        public const string AcceptEncoding = "Accept-Encoding";
        public const string Host = "Host";

        // Web Socket
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
    }
}