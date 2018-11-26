using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class BitcoinFactory : BlockchainCryptoFactory
    {
        public override ulong Multiplier => 100000000;

        public override BlockBase GetBlock() => new Block();
        public override TransactionBase GetTransaction() => new Transaction();
        public override P2PKAddressBase GetP2PKAddress() => new P2PKAddress();
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
        public override P2SHP2WPKHAddressBase GetP2SHP2WPKHAddress() => new P2SHP2WPKHAddress();
        public override P2WPKHAddressBase GetP2WPKHAddress() => new P2WPKHAddress();
    }
}