using System;
using System.IO;
using System.Net.Security;

namespace Zergatul.Network
{
    public class DefaultTlsProvider : ITlsProvider
    {
        public Stream AuthenticateAsClient(Stream innerStream, string host)
        {
            var ssl = new SslStream(innerStream, true);
            ssl.AuthenticateAsClient(host, null, System.Security.Authentication.SslProtocols.Tls12, true);
            return ssl;
        }
    }
}