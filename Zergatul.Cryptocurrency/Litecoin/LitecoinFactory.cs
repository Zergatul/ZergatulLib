using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class LitecoinFactory : BlockchainCryptoFactory
    {
        public override ulong Multiplier => 100000000;

        public override BlockBase GetBlock() => new Block();
        public override TransactionBase GetTransaction() => new Transaction();
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}