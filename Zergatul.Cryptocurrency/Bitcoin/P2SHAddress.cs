namespace Zergatul.Cryptocurrency.Bitcoin
{
    /// <summary>
    /// Pay-to-script-hash
    /// </summary>
    public class P2SHAddress : Address
    {
        protected override byte _prefix => 5;

        public P2SHAddress()
        {

        }

        public P2SHAddress(string address)
        {
            _value = address;
        }

        /// <summary>
        /// Converts Bitcoin Gold address to Bitcoin address
        /// </summary>
        public P2SHAddress(BitcoinGold.P2SHAddress address)
        {
            FromScriptHash(address.Hash);
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
    }
}