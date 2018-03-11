using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class Null : Asn1Element
    {
        public Null()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.NULL
            })
        {

        }

        protected override byte[] BodyToBytes()
        {
            return new byte[0];
        }

        protected override void ReadBody(Stream stream)
        {
            if (Length != 0)
                throw new NotImplementedException();
        }
    }
}