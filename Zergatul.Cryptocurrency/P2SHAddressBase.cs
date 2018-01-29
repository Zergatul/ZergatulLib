using System;

namespace Zergatul.Cryptocurrency
{
    /// <summary>
    /// Pay-to-script-hash
    /// </summary>
    public abstract class P2SHAddressBase : AddressBase
    {
        protected BlockchainCryptoFactory _factory;

        protected P2SHAddressBase(BlockchainCryptoFactory factory)
        {
            this._factory = factory;
        }

        public void Parse(string address)
        {
            string old = _value;
            _value = address;
            if (!Validate())
            {
                _value = old;
                throw new InvalidOperationException("Invalid address");
            }
        }

        public void FromScript(byte[] scriptData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(scriptData);
            byte[] hash = ripesha.ComputeHash();
            FromScriptHash(hash);
        }

        public void FromScriptHash(byte[] hash)
        {
            _value = Base58Encoding.Encode(_prefix, hash);
        }

        public P2SHAddressBase Convert(BlockchainCryptoFactory factory)
        {
            var addr = factory.GetP2SHAddress();
            addr.FromScriptHash(Hash);
            return addr;
        }
    }
}