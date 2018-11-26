namespace Zergatul.Cryptocurrency.P2P
{
    public class ProtocolSpecification
    {
        public uint Magic { get; private set; }
        public ushort Port { get; private set; }

        public static readonly ProtocolSpecification Bitcoin = new ProtocolSpecification
        {
            Magic = 0xD9B4BEF9,
            Port = 8333
        };

        public static readonly ProtocolSpecification BitcoinTestnet = new ProtocolSpecification
        {
            Magic = 0x0709110B,
            Port = 18333
        };

        public static readonly ProtocolSpecification Zcash = new ProtocolSpecification
        {
            Magic = 0x6427E924,
            Port = 8233
        };
    }
}