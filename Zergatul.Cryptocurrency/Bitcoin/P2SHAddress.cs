using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class P2SHAddress : Cryptocurrency.P2SHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x05 };

        protected override byte[] _prefix => Prefix;

        public P2SHAddress()
            : base(BlockchainCryptoFactory.Bitcoin)
        {

        }
    }
}