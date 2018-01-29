namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class P2PKAddress : P2PKAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x00 };

        protected override byte[] _prefix => Prefix;
    }
}