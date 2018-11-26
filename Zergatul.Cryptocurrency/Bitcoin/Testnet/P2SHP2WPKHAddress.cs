namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class P2SHP2WPKHAddress : Base.P2SHP2WPKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0xC4 };

        protected override byte[] _prefix => Prefix;
    }
}