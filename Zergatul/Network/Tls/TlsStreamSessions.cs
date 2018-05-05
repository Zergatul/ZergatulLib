using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    public class TlsStreamSession
    {
        public byte[] ID { get; private set; }

        public TlsStreamSession(byte[] id)
        {
            this.ID = (byte[])id.Clone();
        }
    }

    public class TlsStreamSessions
    {
        public static TlsStreamSessions Instance { get; } = new TlsStreamSessions();

        private Dictionary<string, TlsStreamSession> _sessions;

        private TlsStreamSessions()
        {
            _sessions = new Dictionary<string, TlsStreamSession>();
        }

        public TlsStreamSession this[string host]
        {
            get
            {
                TlsStreamSession session;
                if (_sessions.TryGetValue(host, out session))
                    return session;
                else
                    return null;
            }
        }

        public TlsStreamSession AddSession(string host, byte[] id)
        {
            if (host == null)
                return null;

            var session = new TlsStreamSession(id);
            if (_sessions.ContainsKey(host))
                _sessions[host] = session;
            else
                _sessions.Add(host, session);
            return session;
        }
    }
}