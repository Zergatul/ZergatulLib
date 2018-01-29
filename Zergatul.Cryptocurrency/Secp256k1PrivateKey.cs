using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency
{
    public class Secp256k1PrivateKey
    {
        private byte[] _data;
        public bool Compressed { get; private set; }

        public Secp256k1PrivateKey(byte[] data)
        {
            if (data.Length != 32)
                throw new InvalidOperationException();

            this._data = data;
        }

        public Secp256k1PrivateKey(byte[] data, bool compressed)
            : this(data)
        {
            this.Compressed = compressed;
        }

        public static Secp256k1PrivateKey FromHex(string hex)
        {
            byte[] data = BitHelper.HexToBytes(hex);
            if (data.Length != 32)
                throw new InvalidOperationException();

            return new Secp256k1PrivateKey(data);
        }

        public static Secp256k1PrivateKey FromWIF(byte prefix, string value)
        {
            byte[] data = Base58Encoding.Decode(value);
            if (data[0] != prefix)
                throw new InvalidOperationException();

            if (data.Length == 33)
            {
                var key = new Secp256k1PrivateKey(ByteArray.SubArray(data, 1, 32));
                key.Compressed = false;
                return key;
            }
            else if (data.Length == 34 && data[33] == 0x01)
            {
                var key = new Secp256k1PrivateKey(ByteArray.SubArray(data, 1, 32));
                key.Compressed = true;
                return key;
            }
            else
                throw new InvalidOperationException();
        }

        public ECPoint ToECPoint()
        {
            return EllipticCurve.secp256k1.GeneratorMultiplication(new BigInteger(_data, ByteOrder.BigEndian));
        }

        public string ToWIF(byte prefix)
        {
            if (Compressed)
                return Base58Encoding.Encode(new byte[] { prefix }, ByteArray.Concat(_data, new byte[] { 0x01 }));
            else
                return Base58Encoding.Encode(new byte[] { prefix }, _data);
        }

        public Secp256k1PrivateKey Clone(bool compressed)
        {
            var key = new Secp256k1PrivateKey(_data);
            key.Compressed = compressed;
            return key;
        }
    }
}