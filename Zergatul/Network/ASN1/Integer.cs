using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.Asn1
{
    public class Integer : Asn1Element
    {
        public byte[] Data { get; private set; }
        public BigInteger Value => GetValue();

        public Integer()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.INTEGER
            })
        {

        }

        public Integer(BigInteger value)
            : this()
        {
            Data = value.ToBytes(ByteOrder.BigEndian);
            if (value < 0)
                throw new NotImplementedException();

            if ((Data[0] & 0x80) != 0)
            {
                byte[] bytes = new byte[Data.Length + 1];
                Array.Copy(Data, 0, bytes, 1, Data.Length);
                Data = bytes;
            }
        }

        private BigInteger GetValue()
        {
            bool negative = (Data[0] & 0x80) != 0;
            byte[] bytes = new byte[Data.Length];
            Array.Copy(Data, bytes, Data.Length);
            if (negative)
                Data[0] &= 0x7F;
            BigInteger result = new BigInteger(Data, ByteOrder.BigEndian);
            if (negative)
                result = result.AdditiveInverse();
            return result;
        }

        protected override byte[] BodyToBytes()
        {
            return Data;
        }

        protected override void ReadBody(byte[] data)
        {
            Data = data;
        }
    }
}