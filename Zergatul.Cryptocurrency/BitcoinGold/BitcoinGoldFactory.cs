using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class BitcoinGoldFactory : BlockchainCryptoFactory
    {
        public override ulong Multiplier => 100000000;

        public override BlockBase GetBlock() => null;
        public override TransactionBase GetTransaction() => null;
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}