using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Zcash
{
    public class ZcashFactory : BlockchainCryptoFactory
    {
        public override ulong Multiplier => 100000000;

        public override BlockBase GetBlock() => null;
        public override TransactionBase GetTransaction() => new Transaction();
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}