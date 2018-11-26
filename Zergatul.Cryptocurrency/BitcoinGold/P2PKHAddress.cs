namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class P2PKHAddress : Base.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 38 };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.BitcoinGold)
        {

        }
    }
}