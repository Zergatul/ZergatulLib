using System;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class LitecoinFactory : BlockchainCryptoFactory
    {
        public override BlockBase GetBlock() => new Block();
        public override TransactionBase GetTransaction() => new Transaction();
        public override Cryptocurrency.P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override Cryptocurrency.P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}