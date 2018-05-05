using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Mime
{
    public static class ContentType
    {
        public static class Text
        {
            public const string Plain = "text/plain";
            public const string Html = "text/html";
        }

        public static class Image
        {
            public const string Jpeg = "image/jpeg";
            public const string Png = "image/png";
        }

        public static class Multipart
        {
            public const string Mixed = "multipart/mixed";
        }
    }
}