using System;
using System.Collections.Generic;
using System.IO;
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

        public int TagNumberEx;

        public void WriteTo(Stream stream)
        {
            if ((int)Number == 0x1F)
                throw new NotImplementedException();

            int value = ((byte)Class << 6) | ((byte)ValueType << 5) | (byte)Number;
            stream.WriteByte((byte)value);
        }

        public static ASN1Tag FromByte(byte value, Stream stream, List<byte> raw, out int length)
        {
            length = 1;
            ASN1Tag result = new ASN1Tag
            {
                Class = (ASN1TagClass)(value >> 6),
                ValueType = (ASN1ValueType)((value >> 5) & 1),
                Number = (ASN1TagNumber)(value & 0x1F)
            };

            if ((int)result.Number == 0x1F)
            {
                result.TagNumberEx = 0;
                while (true)
                {
                    int readResult = stream.ReadByte();
                    if (readResult == -1)
                        throw new EndOfStreamException();
                    raw.Add((byte)readResult);
                    length++;
                    result.TagNumberEx = checked(result.TagNumberEx * 128 + (readResult & 0x7F));
                    if ((readResult & 0x80) == 0)
                        break;
                }
            }
            else
                result.TagNumberEx = (byte)result.Number;

            return result;
        }
    }
}