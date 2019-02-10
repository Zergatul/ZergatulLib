using System;

namespace Zergatul.Security.Tls
{
    internal static class TlsHelper
    {
        public static int ToProtocolVersion(TlsVersion version)
        {
            switch (version)
            {
                case TlsVersion.Tls10: return 0x0301;
                case TlsVersion.Tls11: return 0x0302;
                case TlsVersion.Tls12: return 0x0303;
                case TlsVersion.Tls13: return 0x0304;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}