namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class P2PKHAddress : Base.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x6F };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0xEF;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.BitcoinTestnet)
        {

        }
    }
}