using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptocurrency.Bitcoin;
using Zergatul.Cryptography.Asymmetric;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Ripple
{
    // https://ripple.com/build/accounts/#address-encoding
    public class Account
    {
        public static readonly Account ACCOUNT_ZERO = FromHash(BitHelper.HexToBytes("0000000000000000000000000000000000000000"));
        public static readonly Account ACCOUNT_ONE = FromHash(BitHelper.HexToBytes("0000000000000000000000000000000000000001"));
        public static readonly Account Genesis = FromHash(BitHelper.HexToBytes("b5f762798a53d543a014caf8b297cff8f2f937e8"));
        public static readonly Account NameReservation = FromHash(BitHelper.HexToBytes("00000000000000000000000000000000016fc69d"));
        public static readonly Account NaN = FromHash(BitHelper.HexToBytes("0000000000000000000000000000000000000977"));

        public string Value { get; private set; }
        public BigInteger PrivateKey { get; private set; }

        public static Account FromPublicKey(ECPPublicKey key)
        {
            if (key.Point.Curve != EllipticCurve.secp256k1)
                throw new InvalidOperationException();

            byte[] keyBytes = key.Point.ToCompressed();
            if (keyBytes.Length != 33)
                throw new InvalidOperationException();

            return FromKeyBytes(keyBytes);
        }

        public static Account FromPublicKey(EdDSAPublicKey key)
        {
            if (key.Value.Length != 32)
                throw new InvalidOperationException();

            byte[] keyBytes = new byte[33];
            keyBytes[0] = 0xED;
            Array.Copy(key.Value, 0, keyBytes, 1, 32);

            return FromKeyBytes(keyBytes);
        }

        private static Account FromKeyBytes(byte[] key)
        {
            if (key.Length != 33)
                throw new InvalidOperationException();

            AbstractHash hash = new RIPE160SHA256();
            hash.Update(key);
            byte[] accID = hash.ComputeHash();

            return FromHash(accID);
        }

        public static Account FromHash(byte[] hash)
        {
            if (hash.Length != 20)
                throw new InvalidOperationException();

            return new Account
            {
                Value = Base58Encoding.Encode(new byte[] { 0 }, hash, Constants.Dictionary)
            };
        }

        public static Account FromPassphrase(string password)
        {
            var sha512 = new SHA512();
            sha512.Update(Encoding.ASCII.GetBytes(password));
            byte[] hash = sha512.ComputeHash();
            return FromSecretSeed(ByteArray.SubArray(hash, 0, 16));
        }

        public static Account FromSecretKey(string value)
        {
            byte[] data = Base58Encoding.Decode(value, Constants.Dictionary);

            if (data.Length != 17)
                throw new InvalidOperationException();

            if (data[0] != 0x21)
                throw new InvalidOperationException();

            byte[] seed = ByteArray.SubArray(data, 1, 16);
            return FromSecretSeed(seed);
        }

        public static Account FromSecretSeed(byte[] seed)
        {
            BigInteger generatorPrivateKey = PrivateKeyFromGenerator(seed, 0);
            ECPoint publicKey = generatorPrivateKey * Constants.Curve.g;
            byte[] familyGenerator = publicKey.ToCompressed();

            int indexNumber = 0;
            seed = ByteArray.Concat(familyGenerator, BitHelper.GetBytes(indexNumber, ByteOrder.BigEndian));
            BigInteger additionalKey = PrivateKeyFromGenerator(seed, 0);
            BigInteger accountPrivateKey = (generatorPrivateKey + additionalKey) % Constants.Curve.n;
            publicKey = accountPrivateKey * Constants.Curve.g;

            var account = FromKeyBytes(publicKey.ToCompressed());
            account.PrivateKey = accountPrivateKey;
            return account;
        }

        private static BigInteger PrivateKeyFromGenerator(byte[] seed, int counter)
        {
            while (true)
            {
                var sha512 = new SHA512();
                sha512.Update(seed);
                sha512.Update(BitHelper.GetBytes(counter, ByteOrder.BigEndian));
                byte[] hash = sha512.ComputeHash();
                BigInteger privateKey = new BigInteger(hash, 0, 32, ByteOrder.BigEndian);
                if (!privateKey.IsZero && privateKey <= Constants.Curve.n)
                    return privateKey;
            }
        }

        public override string ToString() => Value;
    }
}