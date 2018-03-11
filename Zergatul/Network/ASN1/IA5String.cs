using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class IA5String : Asn1StringElement
    {
        public IA5String()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.IA5String
            })
        {

        }

        protected override byte[] BodyToBytes()
        {
            throw new NotImplementedException();
        }

        protected override void ReadBody(Stream stream)
        {
            byte[] buffer = ReadBuffer(stream, checked((int)Length));
            _raw.AddRange(buffer);
            ReadBody(buffer);
        }

        protected override void ReadBody(byte[] data)
        {
            Value = Encoding.ASCII.GetString(data);
        }
    }
}