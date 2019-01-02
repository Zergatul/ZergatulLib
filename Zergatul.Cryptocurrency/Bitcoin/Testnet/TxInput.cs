namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class TxInput : Base.TxInputBase
    {
        public TxInput()
            : base(BlockchainCryptoFactory.BitcoinTestnet)
        {

        }
    }
}