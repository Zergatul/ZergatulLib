using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class ASN1Tag
    {
        public ASN1TagClass Class;
        public ASN1ValueType ValueType;
        public ASN1TagNumber Number;

        public static ASN1Tag FromByte(byte value)
        {
            return new ASN1Tag
            {
                Class = (ASN1TagClass)(value >> 6),
                ValueType = (ASN1ValueType)((value >> 5) & 1),
                Number = (ASN1TagNumber)(value & 0x1F)
            };
        }
    }
}