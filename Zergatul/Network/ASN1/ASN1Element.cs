using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    // https://habrahabr.ru/post/150757/
    // https://habrahabr.ru/post/150888/
    public abstract class Asn1Element
    {
        public Asn1Tag Tag { get; protected set; }
        public long Length { get; protected set; }
        public int HeaderLength { get; protected set; }

        public byte[] Raw { get; protected set; }
        protected List<byte> _raw;

        public Asn1Element(Asn1Tag tag)
        {
            this.Tag = tag;
        }

        protected virtual void ReadBody(Stream stream)
        {
            byte[] data = ReadBuffer(stream, checked((int)Length));
            _raw.AddRange(data);
            ReadBody(data);
        }

        protected virtual void ReadBody(byte[] data)
        {
            throw new NotImplementedException();
        }

        protected abstract byte[] BodyToBytes();

        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                Tag.WriteTo(ms);
                byte[] body = BodyToBytes();
                WriteLength(ms, body.Length);
                ms.Write(body, 0, body.Length);
                return ms.ToArray();
            }
        }

        public static Asn1Element ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new EndOfStreamException();

            var raw = new List<byte>();
            raw.Add((byte)result);

            int tagLength;
            Asn1Tag tag = Asn1Tag.FromByte((byte)result, stream, raw, out tagLength);
            int lenLength;
            long length = ReadLength(stream, raw, out lenLength);

            if (length == long.MinValue)
                throw new NotImplementedException("Indefinite length not implemented");

            if (tag.Class == Asn1TagClass.Application)
                throw new NotImplementedException("Application tag class not implemented");

            if (tag.Class == Asn1TagClass.ContextSpecific)
            {
                var cs = new ContextSpecific();
                cs.Tag = tag;
                cs.HeaderLength = tagLength + lenLength;
                cs.Length = length;
                cs._raw = raw;
                cs.ReadBody(stream);
                cs.Raw = cs._raw.ToArray();
                return cs;
            }

            Asn1Element element = Resolve(tag.Number);
            element.Tag = tag;
            element.HeaderLength = tagLength + lenLength;
            element.Length = length;
            element._raw = raw;
            element.ReadBody(stream);
            element.Raw = element._raw.ToArray();

            return element;
        }

        public static Asn1Element ReadFrom(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return ReadFrom(ms);
        }

        protected static Asn1Element Resolve(Asn1TagNumber tag)
        {
            switch (tag)
            {
                case Asn1TagNumber.EOC:
                    throw new NotImplementedException();
                case Asn1TagNumber.BOOLEAN:
                    return new Boolean();
                case Asn1TagNumber.INTEGER:
                    return new Integer();
                case Asn1TagNumber.BIT_STRING:
                    return new BitString();
                case Asn1TagNumber.OCTET_STRING:
                    return new OctetString();
                case Asn1TagNumber.NULL:
                    return new Null();
                case Asn1TagNumber.OBJECT_IDENTIFIER:
                    return new ObjectIdentifier();
                case Asn1TagNumber.UTF8String:
                    return new UTF8String();
                case Asn1TagNumber.SEQUENCE:
                    return new Sequence();
                case Asn1TagNumber.SET:
                    return new Set();
                case Asn1TagNumber.PrintableString:
                    return new PrintableString();
                case Asn1TagNumber.IA5String:
                    return new IA5String();
                case Asn1TagNumber.UTCTime:
                    return new UTCTime();
                case Asn1TagNumber.VisibleString:
                    return new VisibleString();
                default:
                    throw new NotImplementedException();
            }
        }

        protected static void StaticReadBody(Asn1Element element, byte[] data)
        {
            element.ReadBody(data);
        }

        private static long ReadLength(Stream stream, List<byte> raw, out int read)
        {
            read = 0;

            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            raw.Add((byte)readResult);
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
                raw.Add((byte)readResult);
                read++;
                result = checked(result * 256 + readResult);
            }

            return result;
        }

        private static void WriteLength(Stream stream, long length)
        {
            if (length < 128)
            {
                stream.WriteByte((byte)length);
                return;
            }

            int octets = 0;
            long l = length;
            while (l != 0)
            {
                octets++;
                l >>= 8;
            }

            stream.WriteByte((byte)(octets | 0x80));

            for (int i = 0; i < octets; i++)
                stream.WriteByte((byte)(length >> (8 * (octets - 1 - i))));
        }

        protected static long GetElementsLength(IList<Asn1Element> elements)
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