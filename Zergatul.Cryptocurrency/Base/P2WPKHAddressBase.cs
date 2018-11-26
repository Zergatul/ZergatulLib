using System;

namespace Zergatul.Cryptocurrency.Base
{
    public abstract class P2WPKHAddressBase : AddressBase
    {
        public void FromPublicKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            FromHash(RIPE160SHA256.Hash(key));
        }

        public void FromHash(byte[] hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));
            if (hash.Length != 20)
                throw new InvalidOperationException();

            _value = Bech32Encoding.Encode("bc", 0, hash);
        }

        public override Script CreateRedeemScript()
        {
            throw new NotImplementedException();
        }

        public override void Sign(TxInputBase input)
        {
            throw new NotImplementedException();
        }
    }
}