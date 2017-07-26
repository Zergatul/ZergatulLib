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
        public long Length { get; protected set; }
        public int HeaderLength { get; protected set; }

        protected abstract void ReadBody(Stream stream);

        public static ASN1Element ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new EndOfStreamException();

            int headerLength = 1;

            ASN1Tag tag = ASN1Tag.FromByte((byte)result);
            int read;
            long length = ReadLength(stream, out read);
            headerLength += read;

            if (tag.Class == ASN1TagClass.ContextSpecific)
                return ReadFrom(stream);

            ASN1Element element;
            switch (tag.Number)
            {
                case ASN1TagNumber.EOC:
                    throw new NotImplementedException();
                case ASN1TagNumber.INTEGER:
                    element = new Integer();
                    break;
                case ASN1TagNumber.BIT_STRING:
                    element = new BitString();
                    break;
                case ASN1TagNumber.NULL:
                    element = new Null();
                    break;
                case ASN1TagNumber.OBJECT_IDENTIFIER:
                    element = new ObjectIdentifier();
                    break;
                case ASN1TagNumber.SEQUENCE:
                    element = new Sequence();
                    break;
                case ASN1TagNumber.SET:
                    element = new Set();
                    break;
                case ASN1TagNumber.PrintableString:
                    element = new PrintableString();
                    break;
                case ASN1TagNumber.UTCTime:
                    element = new UTCTime();
                    break;
                default:
                    throw new NotImplementedException();
            }

            element.Tag = tag;
            element.HeaderLength = headerLength;
            element.Length = length;
            element.ReadBody(stream);

            return element;
        }

        private static long ReadLength(Stream stream, out int read)
        {
            read = 0;

            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            read++;

            byte b = (byte)readResult;

            if (b < 128)
                return b;
            if (b == 128)
                return long.MinValue;
            if (b == 255)
                throw new NotImplementedException("Reserved");

            int octets = b & 0x7F;
            if (octets > 8)
                throw new NotImplementedException("Value too big for UInt64");
            long result = 0;
            for (int i = 0; i < octets; i++)
            {
                readResult = stream.ReadByte();
                if (readResult == -1)
                    throw new EndOfStreamException();
                read++;
                result = checked(result * 256 + readResult);
            }

            return result;
        }

        protected static long GetElementsLength(IList<ASN1Element> elements)
        {
            long length = 0;
            for (int i = 0; i < elements.Count; i++)
                length += elements[i].Length + elements[i].HeaderLength;
            return length;
        }

        protected static byte[] ReadBuffer(Stream stream, int totalBytes)
        {
            int totalRead = 0;
            byte[] buffer = new byte[totalBytes];
            while (totalRead < totalBytes)
                totalRead += stream.Read(buffer, totalRead, totalBytes - totalRead);
            return buffer;
        }
    }
}