using System.Collections.Generic;
using System.Net;

namespace Zergatul.Network.Dns
{
    public class StaticDnsProvider : DnsProvider
    {
        private DnsProvider _provider;
        private Dictionary<string, IPHostEntry> _dictionary;

        public StaticDnsProvider(DnsProvider fallbackProvider)
        {
            this._provider = fallbackProvider;
            this._dictionary = new Dictionary<string, IPHostEntry>();
        }

        public override IPHostEntry Resolve(string hostName)
        {
            if (_dictionary.TryGetValue(hostName.ToLower(), out IPHostEntry entry))
                return entry;
            else
                return _provider.Resolve(hostName);
        }

        public void AddEntry(string hostName, IPHostEntry entry)
        {
            _dictionary[hostName.ToLower()] = entry;
        }

        public void RemoveEntry(string hostName)
        {
            _dictionary.Remove(hostName.ToLower());
        }
    }
}