namespace Zergatul.Cryptocurrency.Zcash
{
    public class TxOutput : Base.TxOutputBase
    {
        public decimal AmountZEC => 1m * Amount / _factory.Multiplier;

        public TxOutput()
            : base(BlockchainCryptoFactory.Zcash)
        {

        }
    }
}