namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class TxOutput : Base.TxOutputBase
    {
        public decimal AmountBTC
        {
            get => 1m * Amount / _factory.Multiplier;
            set => Amount = (ulong)(value * _factory.Multiplier);
        }

        public TxOutput()
            : base(BlockchainCryptoFactory.BitcoinTestnet)
        {

        }
    }
}