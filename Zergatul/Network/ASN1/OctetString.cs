using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class OctetString : Asn1Element
    {
        public byte[] Data { get; private set; }

        public OctetString()
            : base(new Asn1Tag
            {
                Class = Asn1TagClass.Universal,
                ValueType = Asn1ValueType.Primitive,
                Number = Asn1TagNumber.OCTET_STRING
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