namespace Zergatul.Cryptocurrency
{
    /// <summary>
    /// Pay-to-pubkey
    /// </summary>
    public abstract class P2PKAddressBase : AddressBase
    {
        public void FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();

            _value = Base58Encoding.Encode(_prefix, hash);
        }
    }
}