namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class P2SHAddress : Base.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0xC4 };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.BitcoinTestnet)
        {

        }
    }
}