namespace Zergatul.Cryptocurrency.Verge
{
    public class VergeFactory : BlockchainCryptoFactory
    {
        public override BlockBase GetBlock() => null;
        public override TransactionBase GetTransaction() => new Transaction();
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}