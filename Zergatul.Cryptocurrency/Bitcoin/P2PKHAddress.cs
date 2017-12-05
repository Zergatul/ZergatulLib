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

        public static P2PKHAddress FromPublicKey(byte[] pubkeyData)
        {
            var ripesha = new RIPE160SHA256();
            ripesha.Update(pubkeyData);
            byte[] hash = ripesha.ComputeHash();
            return FromPublicKeyHash(hash);
        }

        public static P2PKHAddress FromPublicKeyHash(byte[] hash)
        {
            return new P2PKHAddress
            {
                _value = "1" + Base58Encoding.Encode(0, hash)
            };
        }

        public static P2PKHAddress FromWIF(string value)
        {
            var key = PrivateKey.FromWIF(value);
            var point = key.ToECPoint();

            var addr = FromPublicKey(point.ToUncompressed());
            addr.PrivateKey = key;
            addr.Another = FromPublicKey(point.ToCompressed());
            addr.Another.PrivateKey = key;
            return addr;
        }
    }
}