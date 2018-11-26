namespace Zergatul.Cryptocurrency.Base
{
    public abstract class AddressBase
    {
        protected string _value = "[Invalid address]";

        public string Value => _value;

        public abstract Script CreateRedeemScript();

        public abstract void Sign(TxInputBase input);

        public override string ToString() => _value;
    }
}