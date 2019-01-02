using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Ethereum
{
    public class Address
    {
        public Secp256k1PrivateKey PrivateKey { get; set; }
        public byte[] Hash { get; private set; }

        public string Value => Hash != null ? MixedCaseAddressEncoding.Encode("0x" + BitHelper.BytesToHex(Hash)) : null;

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

        public void FromPublicKey(ECPoint point)
        {
            byte[] pubKey = new byte[64];
            Array.Copy(point.ToUncompressed(), 1, pubKey, 0, 64);
            FromPublicKey(pubKey);
        }

        public void FromPublicKey(byte[] pubkeyData)
        {
            if (pubkeyData.Length != 64)
                throw new InvalidOperationException();

            var keccak = new Keccak256();
            keccak.Update(pubkeyData);
            Hash = keccak.ComputeHash();

            Hash = ByteArray.SubArray(Hash, 12, 20);
        }

        public void Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
            if (value.Length != 42 || !value.StartsWith("0x"))
                throw new ArgumentException();

            if (!MixedCaseAddressEncoding.Validate(value))
                throw new ArgumentException();

            Hash = BitHelper.HexToBytes(value.Substring(2));
        }

        public byte[] ToPublicKey()
        {
            if (PrivateKey == null)
                throw new InvalidOperationException();

            byte[] pubKey = PrivateKey.ToUncompressedPublicKey();
            return ByteArray.SubArray(pubKey, 1, 64);
        }

        public void FromHash(byte[] hash)
        {
            if (hash?.Length != 20)
                throw new ArgumentException();

            Hash = hash;
        }

        public override string ToString() => Value;
    }
}