using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class BitString : Asn1Element
    {
        public int PadBits { get; private set; }
        public byte[] Data { get; private set; }

        public BitString()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.BIT_STRING
            })
        {

        }

        protected override byte[] BodyToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ReadBody(Stream stream)
        {
            if (Length < 1)
                throw new InvalidOperationException();

            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            PadBits = readResult;
            _raw.Add((byte)readResult);
            Data = ReadBuffer(stream, checked((int)Length - 1));
            _raw.AddRange(Data);
        }
    }
}