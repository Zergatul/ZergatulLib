using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin.Testnet
{
    public class BitcoinTestnetFactory : BitcoinFactory
    {
        public override P2PKHAddressBase GetP2PKHAddress() => new P2PKHAddress();
        public override P2SHAddressBase GetP2SHAddress() => new P2SHAddress();
        public override P2SHP2WPKHAddressBase GetP2SHP2WPKHAddress() => new P2SHP2WPKHAddress();
    }
}