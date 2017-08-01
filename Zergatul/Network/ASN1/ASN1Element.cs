using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    // https://habrahabr.ru/post/150757/
    // https://habrahabr.ru/post/150888/
    public abstract class ASN1Element
    {
        public ASN1Tag Tag { get; protected set; }
        public long Length { get; protected set; }
        public int HeaderLength { get; protected set; }

        protected virtual void ReadBody(Stream stream)
        {
            byte[] data = ReadBuffer(stream, checked((int)Length));
            ReadBody(data);
        }

        protected virtual void ReadBody(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static ASN1Element ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new EndOfStreamException();

            int tagLength;
            ASN1Tag tag = ASN1Tag.FromByte((byte)result, stream, out tagLength);
            int lenLength;
            long length = ReadLength(stream, out lenLength);

            if (length == long.MinValue)
                throw new NotImplementedException("Indefinite length not implemented");

            if (tag.Class == ASN1TagClass.Application)
                throw new NotImplementedException("Application tag class not implemented");

            if (tag.Class == ASN1TagClass.ContextSpecific)
            {
                var cs = new ContextSpecific();
                cs.Tag = tag;
                cs.HeaderLength = tagLength + lenLength;
                cs.Length = length;
                cs.ReadBody(stream);
                return cs;
            }

            ASN1Element element = Resolve(tag.Number);
            element.Tag = tag;
            element.HeaderLength = tagLength + lenLength;
            element.Length = length;
            element.ReadBody(stream);

            return element;
        }

        public static ASN1Element ReadFrom(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return ReadFrom(ms);
        }

        protected static ASN1Element Resolve(ASN1TagNumber tag)
        {
            switch (tag)
            {
                case ASN1TagNumber.EOC:
                    throw new NotImplementedException();
                case ASN1TagNumber.BOOLEAN:
                    return new Boolean();
                case ASN1TagNumber.INTEGER:
                    return new Integer();
                case ASN1TagNumber.BIT_STRING:
                    return new BitString();
                case ASN1TagNumber.OCTET_STRING:
                    return new OctetString();
                case ASN1TagNumber.NULL:
                    return new Null();
                case ASN1TagNumber.OBJECT_IDENTIFIER:
                    return new ObjectIdentifier();
                case ASN1TagNumber.UTF8String:
                    return new UTF8String();
                case ASN1TagNumber.SEQUENCE:
                    return new Sequence();
                case ASN1TagNumber.SET:
                    return new Set();
                case ASN1TagNumber.PrintableString:
                    return new PrintableString();
                case ASN1TagNumber.IA5String:
                    return new IA5String();
                case ASN1TagNumber.UTCTime:
                    return new UTCTime();
                default:
                    throw new NotImplementedException();
            }
        }

        protected static void StaticReadBody(ASN1Element element, byte[] data)
        {
            element.ReadBody(data);
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