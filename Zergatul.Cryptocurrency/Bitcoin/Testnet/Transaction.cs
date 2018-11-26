using System;

namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class Transaction : Base.TransactionBase<TxInput, TxOutput>
    {
        public decimal? FeeBTC => Fee == null ? 0 : 1m * Fee.Value / BlockchainCryptoFactory.BitcoinTestnet.Multiplier;
    }
}