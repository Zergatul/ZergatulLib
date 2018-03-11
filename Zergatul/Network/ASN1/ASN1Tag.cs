using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public class Asn1Tag
    {
        public Asn1TagClass Class;
        public Asn1ValueType ValueType;
        public Asn1TagNumber Number;

        public int TagNumberEx;

        public void WriteTo(Stream stream)
        {
            if ((int)Number == 0x1F)
                throw new NotImplementedException();

            int value = ((byte)Class << 6) | ((byte)ValueType << 5) | (byte)Number;
            stream.WriteByte((byte)value);
        }

        public static Asn1Tag FromByte(byte value, Stream stream, List<byte> raw, out int length)
        {
            length = 1;
            Asn1Tag result = new Asn1Tag
            {
                Class = (Asn1TagClass)(value >> 6),
                ValueType = (Asn1ValueType)((value >> 5) & 1),
                Number = (Asn1TagNumber)(value & 0x1F)
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