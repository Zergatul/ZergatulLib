namespace Zergatul.Cryptocurrency.Zcash
{
    public class P2SHAddress : Base.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x1C, 0xBD };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.Zcash)
        {

        }
    }
}