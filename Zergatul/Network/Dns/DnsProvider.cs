using System.Net;

namespace Zergatul.Network.Dns
{
    public abstract class DnsProvider
    {
        public static DnsProvider Instance;

        public abstract IPHostEntry Resolve(string hostName);
    }
}