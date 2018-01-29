using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Litecoin
{
    public class P2PKHAddress : Cryptocurrency.P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x30 };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0xB0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Litecoin)
        {

        }
    }
}