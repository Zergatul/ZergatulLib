using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Math.EllipticCurves.PrimeField;

namespace Zergatul.Cryptocurrency.Bitcoin
{
    public class PrivateKey
    {
        private byte[] _data;

        public PrivateKey(byte[] data)
        {
            if (data.Length != 32)
                throw new InvalidOperationException();

            this._data = data;
        }

        public static PrivateKey FromHex(string hex)
        {
            byte[] data = BitHelper.HexToBytes(hex);
            if (data.Length != 32)
                throw new InvalidOperationException();

            return new PrivateKey(data);
        }

        public static PrivateKey FromWIF(string value)
        {
            byte[] data = Base58Encoding.Decode(value);
            if (data[0] != 0x80)
                throw new InvalidOperationException();

            if (data.Length != 33)
                throw new NotImplementedException();

            return new PrivateKey(ByteArray.SubArray(data, 1, 32));
        }

        public ECPoint ToECPoint()
        {
            var curve = EllipticCurve.secp256k1;
            return new BigInteger(_data, ByteOrder.BigEndian) * curve.g;
        }

        public string ToWIF()
        {
            return Base58Encoding.Encode(0x80, _data);
        }
    }
}