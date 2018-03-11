using System.IO;

namespace Zergatul.Network.Tls
{
    public class TlsProvider : ITlsProvider
    {
        public TlsStreamSettings Settings { get; set; }

        public Stream AuthenticateAsClient(Stream innerStream, string host)
        {
            var tls = new TlsStream(innerStream);
            if (Settings != null)
                tls.Settings = Settings;
            tls.AuthenticateAsClient(host);
            return tls;
        }
    }
}