namespace Zergatul.Cryptocurrency.Verge
{
    public class P2PKHAddress : P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x1E };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Verge)
        {

        }
    }
}