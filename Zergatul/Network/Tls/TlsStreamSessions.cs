using System;
using System.Collections.Generic;

namespace Zergatul.Network.Tls
{
    public class TlsStreamSession
    {
        public byte[] ID { get; }
        public byte[] MasterSecret { get; }

        public TlsStreamSession(byte[] id, byte[] secret)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (secret == null)
                throw new ArgumentNullException(nameof(secret));

            ID = (byte[])id.Clone();
            MasterSecret = (byte[])secret.Clone();
        }
    }

    public class TlsStreamSessions
    {
        public static TlsStreamSessions Instance { get; } = new TlsStreamSessions();

        public event EventHandler<TlsStreamSessionEventArgs> SessionAdded;
        public event EventHandler<TlsStreamSessionEventArgs> SessionRemoved;

        private Dictionary<string, TlsStreamSession> _sessions;

        private TlsStreamSessions()
        {
            _sessions = new Dictionary<string, TlsStreamSession>();
        }

        public void Add(string host, TlsStreamSession session)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            lock (_sessions)
            {
                if (_sessions.ContainsKey(host))
                    _sessions[host] = session;
                else
                    _sessions.Add(host, session);
            }

            SessionAdded?.Invoke(this, new TlsStreamSessionEventArgs(host, session));
        }

        public TlsStreamSession Get(string host)
        {
            lock (_sessions)
            {
                if (_sessions.TryGetValue(host, out TlsStreamSession session))
                    return session;
                else
                    return null;
            }
        }

        public void Remove(string host)
        {
            TlsStreamSession session = null;
            lock (_sessions)
            {
                if (_sessions.TryGetValue(host, out session))
                    _sessions.Remove(host);
            }

            if (session != null)
                SessionRemoved?.Invoke(this, new TlsStreamSessionEventArgs(host, session));
        }
    }

    public class TlsStreamSessionEventArgs : EventArgs
    {
        public string Host { get; }
        public TlsStreamSession Session { get; }

        public TlsStreamSessionEventArgs(string host, TlsStreamSession session)
        {
            Host = host;
            Session = session;
        }
    }
}