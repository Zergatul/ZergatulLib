namespace Zergatul.Cryptocurrency.Litecoin
{
    public class P2PKHAddress : Bitcoin.P2PKHAddress
    {
        protected override byte _prefix => 0x30;

        public P2PKHAddress()
        {

        }

        public P2PKHAddress(string address)
            : base(address)
        {

        }
    }

    public class P2SHAddress : Bitcoin.P2SHAddress
    {
        protected override byte _prefix => 0x05;

        public P2SHAddress()
        {

        }

        public P2SHAddress(string address)
            : base(address)
        {

        }
    }
}