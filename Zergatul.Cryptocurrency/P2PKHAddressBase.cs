using System;

namespace Zergatul.Cryptocurrency
{
    /// <summary>
    /// Pay-to-pubkey-hash
    /// </summary>
    public abstract class P2PKHAddressBase : AddressBase
    {
        public Secp256k1PrivateKey PrivateKey { get; set; }
        public bool? IsCompressed { get; set; }

        protected abstract byte _wifPrefix { get; }

        protected BlockchainCryptoFactory _factory;

        protected P2PKHAddressBase(BlockchainCryptoFactory factory)
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

        public void FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();
            FromPublicKeyHash(hash);
        }

        public void FromPublicKeyHash(byte[] hash)
        {
            _value = Base58Encoding.Encode(_prefix, hash);
        }

        public P2PKHAddressBase Convert(BlockchainCryptoFactory factory)
        {
            var addr = factory.GetP2PKHAddress();
            addr.FromPublicKeyHash(Hash);
            return addr;
        }

        public void FromWIF(string value)
        {
            var key = Secp256k1PrivateKey.FromWIF(_wifPrefix, value);
            FromPrivateKey(key);
        }

        public void FromPrivateKey(Secp256k1PrivateKey key)
        {
            var point = key.ToECPoint();

            if (key.Compressed)
                FromPublicKey(point.ToCompressed());
            else
                FromPublicKey(point.ToUncompressed());
            IsCompressed = key.Compressed;
            PrivateKey = key;
        }

        public string ToWIF()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            return PrivateKey.ToWIF(_wifPrefix);
        }

        public P2PKHAddressBase ToUncompressed()
        {
            if (IsCompressed == null)
                throw new InvalidOperationException();

            if (IsCompressed.Value)
            {
                var key = PrivateKey.Clone(false);
                var addr = _factory.GetP2PKHAddress();
                addr.FromPrivateKey(key);
                return addr;
            }
            else
                return this;
        }

        public P2PKHAddressBase ToCompressed()
        {
            if (IsCompressed == null)
                throw new InvalidOperationException();

            if (IsCompressed.Value)
                return this;
            else
            {
                var key = PrivateKey.Clone(true);
                var addr = _factory.GetP2PKHAddress();
                addr.FromPrivateKey(key);
                return addr;
            }
        }
    }
}