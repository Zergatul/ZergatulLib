namespace Zergatul.Cryptocurrency.BitcoinGold
{
    public class P2PKAddress : Bitcoin.P2PKAddress
    {
        protected override byte _prefix => 38;
    }

    public class P2PKHAddress : Bitcoin.P2PKHAddress
    {
        protected override byte _prefix => 38;

        public P2PKHAddress()
        {

        }

        public P2PKHAddress(string address)
            : base(address)
        {

        }

        /// <summary>
        /// Converts Bitcoin address to Bitcoin Gold address
        /// </summary>
        public P2PKHAddress(Bitcoin.P2PKHAddress address)
        {
            FromPublicKeyHash(address.Hash);
        }
    }

    public class P2SHAddress : Bitcoin.P2SHAddress
    {
        protected override byte _prefix => 23;

        public P2SHAddress()
        {

        }

        public P2SHAddress(string address)
            : base(address)
        {

        }

        /// <summary>
        /// Converts Bitcoin adddress to Bitcoin Gold address
        /// </summary>
        public P2SHAddress(Bitcoin.P2SHAddress address)
        {
            FromScriptHash(address.Hash);
        }
    }
}