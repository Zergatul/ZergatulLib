using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    /// <summary>
    /// Pay-to-pubkey-hash
    /// </summary>
    public class P2PKHAddress : Address
    {
        public P2PKHAddress Another { get; set; }
        public PrivateKey PrivateKey { get; set; }

        protected override byte _prefix => 0;

        public P2PKHAddress()
        {

        }

        public P2PKHAddress(string address)
        {
            _value = address;
            if (!Validate())
                throw new InvalidOperationException("Invalid address");
        }

        /// <summary>
        /// Converts Bitcoin Gold address to Bitcoin
        /// </summary>
        /// <param name="address"></param>
        public P2PKHAddress(BitcoinGold.P2PKHAddress address)
        {
            FromPublicKeyHash(address.Hash);
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

        public void FromWIF(string value)
        {
            var key = PrivateKey.FromWIF(value);
            var point = key.ToECPoint();

            FromPublicKey(point.ToUncompressed());
            PrivateKey = key;
            Another = new P2PKHAddress();
            Another.FromPublicKey(point.ToCompressed());
            Another.PrivateKey = key;
        }
    }
}