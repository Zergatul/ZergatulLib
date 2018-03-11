using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class Boolean : Asn1Element
    {
        public bool Value { get; private set; }

        public Boolean()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.BOOLEAN
            })
        {

        }

        public Boolean(bool value)
            : this()
        {
            this.Value = value;
        }

        protected override byte[] BodyToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ReadBody(Stream stream)
        {
            if (Length != 1)
                throw new NotImplementedException();

            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            _raw.Add((byte)readResult);
            Value = readResult != 0;
        }
    }
}