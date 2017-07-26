using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public abstract class ASN1Element
    {
        public ASN1Tag Tag { get; protected set; }
        public ulong Length { get; protected set; }

        protected abstract void ReadBody(Stream stream);

        public static ASN1Element ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new EndOfStreamException();

            ASN1Tag tag = ASN1Tag.FromByte((byte)result);
            ulong length = ReadLength(stream);
            ASN1Element element;
            switch (tag.Number)
            {
                case ASN1TagNumber.EOC:
                    if (tag.Class == ASN1TagClass.ContextSpecific)
                        return ReadFrom(stream);
                    else
                        throw new NotImplementedException();
                case ASN1TagNumber.INTEGER:
                    element = new Integer();
                    break;
                case ASN1TagNumber.OBJECT_IDENTIFIER:
                    element = new ObjectIdentifier();
                    break;
                case ASN1TagNumber.SEQUENCE:
                    element = new Sequence();
                    break;
                default:
                    throw new NotImplementedException();
            }

            element.Tag = tag;
            element.Length = length;
            element.ReadBody(stream);

            return element;
        }

        private static ulong ReadLength(Stream stream)
        {
            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            byte value = (byte)readResult;

            if (value < 128)
                return value;
            if (value == 128)
                return ulong.MaxValue;
            if (value == 255)
                throw new NotImplementedException("Reserved");

            int octets = value & 0x7F;
            if (octets > 8)
                throw new NotImplementedException("Value too big for UInt64");
            ulong result = 0;
            for (int i = 0; i < octets; i++)
            {
                readResult = stream.ReadByte();
                if (readResult == -1)
                    throw new EndOfStreamException();
                result = (result << 8) + (byte)readResult;
            }
            return result;
        }
    }
}