namespace Zergatul.Cryptocurrency.Zcash
{
    public class TxOutput : TxOutputBase
    {
        public decimal AmountZEC => 1m * Amount / 100000000;

        public TxOutput()
            : base(BlockchainCryptoFactory.Zcash)
        {

        }
    }
}