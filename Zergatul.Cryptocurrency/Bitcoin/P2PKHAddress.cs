namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class P2PKHAddress : Base.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x00 };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0x80;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}