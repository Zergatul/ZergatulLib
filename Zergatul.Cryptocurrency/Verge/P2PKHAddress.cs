namespace Zergatul.Cryptocurrency.Verge
{
    public class P2PKHAddress : P2PKHAddressBase
    {
        private static readonly byte[] Prefix = new byte[] { 0x1E };

        protected override byte[] _prefix => Prefix;
        protected override byte _wifPrefix => 0;

        public P2PKHAddress()
            : base(BlockchainCryptoFactory.Verge)
        {

        }

        public P2PKHAddress(string address)
            : this()
        {
            Parse(address);
        }

        public static P2PKHAddress FromPrivateKey(byte[] data)
        {
            var addr = new P2PKHAddress();
            addr.FromPrivateKey(new Secp256k1PrivateKey(data, true));
            return addr;
        }
    }
}