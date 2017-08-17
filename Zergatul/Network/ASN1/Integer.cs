using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1
{
    public class Integer : ASN1Element
    {
        public byte[] Raw { get; private set; }
        public BigInteger Value => GetValue();

        public Integer()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.INTEGER
            })
        {

        }

        public Integer(BigInteger value)
            : this()
        {
            Raw = value.ToBytes(ByteOrder.BigEndian);
            if (value < 0)
                throw new NotImplementedException();

            if ((Raw[0] & 0x80) != 0)
            {
                byte[] bytes = new byte[Raw.Length + 1];
                Array.Copy(Raw, 0, bytes, 1, Raw.Length);
                Raw = bytes;
            }
        }

        private BigInteger GetValue()
        {
            bool negative = (Raw[0] & 0x80) != 0;
            byte[] bytes = new byte[Raw.Length];
            Array.Copy(Raw, bytes, Raw.Length);
            if (negative)
                Raw[0] &= 0x7F;
            BigInteger result = new BigInteger(Raw, ByteOrder.BigEndian);
            if (negative)
                result = result.AdditiveInverse();
            return result;
        }

        protected override byte[] BodyToBytes()
        {
            return Raw;
        }

        protected override void ReadBody(byte[] data)
        {
            Raw = data;
        }
    }
}