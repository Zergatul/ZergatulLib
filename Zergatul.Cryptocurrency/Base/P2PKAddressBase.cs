namespace Zergatul.Cryptocurrency.Base
{
    /// <summary>
    /// Pay-to-pubkey
    /// </summary>
    public abstract class P2PKAddressBase : Base58AddressBase
    {
        public void FromPublicKey(byte[] pubkeyData)
        {
            byte[] hash = RIPE160SHA256.Hash(pubkeyData);
            _value = Base58Encoding.Encode(_prefix, hash);
        }

        public override Script CreateRedeemScript()
        {
            throw new System.NotImplementedException();
        }

        public override void Sign(TxInputBase input)
        {
            throw new System.NotImplementedException();
        }
    }
}