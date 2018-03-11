using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class PrintableString : Asn1StringElement
    {
        public PrintableString()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.PrintableString
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
            Value = Encoding.ASCII.GetString(buffer);
        }
    }
}