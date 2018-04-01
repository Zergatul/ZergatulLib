using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class BitcoinGoldFactory : BlockchainCryptoFactory
    {
        public override ulong Multiplier => 100000000;

        public override BlockBase GetBlock() => null;
        public override TransactionBase GetTransaction() => null;
        public override Cryptocurrency.P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override Cryptocurrency.P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
    }
}