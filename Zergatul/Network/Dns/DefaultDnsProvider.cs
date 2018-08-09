using System.Net;

namespace Zergatul.Network.Dns
{
    public class DefaultDnsProvider : DnsProvider
    {
        public override IPHostEntry Resolve(string hostName) => System.Net.Dns.GetHostEntry(hostName);
    }
}