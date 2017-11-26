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
        public byte[] Data { get; private set; }

        public OctetString()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.OCTET_STRING
            })
        {

        }

        public OctetString(byte[] data)
            : this()
        {
            this.Data = data;
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