namespace Zergatul.Cryptocurrency.Verge
{
    public class P2SHAddress : Cryptocurrency.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x21 };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.Verge)
        {

        }
    }
}