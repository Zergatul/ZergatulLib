namespace Zergatul.Network.Tls
{
    public class ConnectionInfo
    {
        public CipherSuite? CipherSuite { get; internal set; }
        public byte[] MasterSecret { get; internal set; }
        public bool? ReuseSession { get; internal set; }
        public bool? ExtendedMasterSecret { get; internal set; }
        public string ApplicationLevelProtocolName { get; internal set; }

        public ConnectionClientInfo Client { get; private set; } = new ConnectionClientInfo();
        public ConnectionServerInfo Server { get; private set; } = new ConnectionServerInfo();

        internal ConnectionInfo()
        {

        }

        public class ConnectionClientInfo
        {
            public bool? OfferedExtendedMasterSecret { get; internal set; }
            public byte[] FinishedMessageHash { get; internal set; }
        }

        public class ConnectionServerInfo
        {
            public bool? OfferedExtendedMasterSecret { get; internal set; }
            public byte[] FinishedMessageHash { get; internal set; }
        }
    }
}