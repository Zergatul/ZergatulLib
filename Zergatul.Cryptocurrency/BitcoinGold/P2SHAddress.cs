using System;
namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class P2SHAddress : Base.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 23 };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.BitcoinGold)
        {

        }
    }
}