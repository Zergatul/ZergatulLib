using System;

namespace Zergatul.Cryptocurrency.Zcash
{
    public class P2PKHAddress : Base.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x1C, 0xB8 };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Zcash)
        {

        }
    }
}