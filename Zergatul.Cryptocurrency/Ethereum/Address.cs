using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Address
    {
        public Secp256k1PrivateKey PrivateKey { get; set; }
        public byte[] Hash { get; private set; }

        public string Value => Hash != null ? "0x" + BitHelper.BytesToHex(Hash) : null;

        public void FromPrivateKey(Secp256k1PrivateKey key)
        {
            PrivateKey = key;
            FromPublicKey(ToPublicKey());
        }

        public void FromPrivateKey(byte[] key)
        {
            if (key.Length != 32)
                throw new InvalidOperationException();

            FromPrivateKey(new Secp256k1PrivateKey(key));
        }

        public void FromPrivateKey(string hex) => FromPrivateKey(BitHelper.HexToBytes(hex));

        public void FromPublicKey(byte[] pubkeyData)
        {
            var keccak = new Keccak256();
            keccak.Update(pubkeyData);
            Hash = keccak.ComputeHash();

            Hash = ByteArray.SubArray(Hash, 12, 20);
        }

        public byte[] ToPublicKey()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            var point = PrivateKey.ToECPoint();
            byte[] pubKey = new byte[64];
            Array.Copy(point.ToUncompressed(), 1, pubKey, 0, 64);
            return pubKey;
        }

        public override string ToString() => Value;
    }
}