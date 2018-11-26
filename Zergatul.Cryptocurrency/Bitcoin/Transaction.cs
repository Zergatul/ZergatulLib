using System;
using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class Transaction : TransactionBase<TxInput, TxOutput>
    {
        public decimal? FeeBTC => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.Bitcoin.Multiplier;

        public override void Sign()
        {
            throw new System.NotImplementedException();
        }
    }
}