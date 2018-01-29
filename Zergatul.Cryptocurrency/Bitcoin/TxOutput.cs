using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class TxOutput : TxOutputBase
    {
        public decimal AmountBTC
        {
            get
            {
                return 1m * Amount / 100000000;
            }
            set
            {
                Amount = (ulong)(value * 100000000);
            }
        }

        public TxOutput()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}