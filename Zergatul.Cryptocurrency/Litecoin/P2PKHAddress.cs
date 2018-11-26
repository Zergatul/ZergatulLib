namespace Zergatul.Cryptocurrency.Litecoin
{
    public class P2PKHAddress : Base.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x30 };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0xB0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Litecoin)
        {

        }
    }
}