namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class P2SHAddress : Base.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x05 };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}