using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class Null : ASN1Element
    {
        public Null()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.NULL
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