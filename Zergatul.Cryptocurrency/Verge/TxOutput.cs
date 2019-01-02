namespace Zergatul.Cryptocurrency.Verge
{
    public class TxOutput : Base.TxOutputBase
    {
        public decimal AmountXVG => 1m * Amount / _factory.Multiplier;

        public TxOutput()
            : base(BlockchainCryptoFactory.Verge)
        {

        }
    }
}