using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class TxOutput : TxOutputBase
    {
        public decimal AmountBTC
        {
            get => 1m * Amount / _factory.Multiplier;
            set => Amount = (ulong)(value * _factory.Multiplier);
        }

        public TxOutput()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}