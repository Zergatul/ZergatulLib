using Zergatul.Cryptocurrency.Base;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class P2SHP2WPKHAddress : P2SHP2WPKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x05 };

        protected override byte[] _prefix => Prefix;
    }
}