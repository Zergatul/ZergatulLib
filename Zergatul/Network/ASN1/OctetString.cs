using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class OctetString : ASN1Element
    {
        public byte[] Raw { get; private set; }

        public OctetString()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.OCTET_STRING
            })
        {

        }

        public OctetString(byte[] raw)
            : this()
        {
            this.Raw = raw;
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