namespace Zergatul.Network.Http
{
    public static class Http2Headers
    {
        public static class Pseudo
        {
            public const string Method = ":method";
            public const string Scheme = ":scheme";
            public const string Authority = ":authority";
            public const string Path = ":path";
        }

        public const string AcceptEncoding = "accept-encoding";
        public const string ContentEncoding = "content-encoding";
        public const string Cookie = "cookie";
    }
}