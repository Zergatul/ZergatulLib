using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class IA5String : ASN1StringElement
    {
        public IA5String()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.IA5String
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
            ReadBody(buffer);
        }

        protected override void ReadBody(byte[] data)
        {
            Value = Encoding.ASCII.GetString(data);
        }
    }
}