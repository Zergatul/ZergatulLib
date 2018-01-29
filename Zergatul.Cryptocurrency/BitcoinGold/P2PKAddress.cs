namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class P2PKAddress : Bitcoin.P2PKAddress
    {
        private static readonly byte[] Prefix = new byte[] { 38 };

        protected override byte[] _prefix => Prefix;
    }
}