using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public decimal? FeeBTC => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Bitcoin.Multiplier;
    }
}