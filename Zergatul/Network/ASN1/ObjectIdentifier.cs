using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography;

namespace Zergatul.Network.ASN1
{
    public class ObjectIdentifier : ASN1Element
    {
        public OID OID { get; private set; }

        public ObjectIdentifier()
            : base(new ASN1Tag
            {
                Class = ASN1TagClass.Universal,
                ValueType = ASN1ValueType.Primitive,
                Number = ASN1TagNumber.OBJECT_IDENTIFIER
            })
        {

        }

        public ObjectIdentifier(OID oid)
            : this()
        {
            this.OID = oid;
        }

        protected override byte[] BodyToBytes()
        {
            var array = this.OID.DotNotation.Split('.').Select(p => int.Parse(p)).ToArray();
            if (array.Length < 2)
                throw new NotImplementedException();

            int first;
            switch (array[0])
            {
                case 0: first = array[1]; break;
                case 1: first = 40 + array[1]; break;
                case 2: first = 80 + array[1]; break;
                default: throw new InvalidOperationException();
            }

            using (var ms = new MemoryStream())
            {
                if (array.Length == 2)
                    first |= 0x80;
                ms.WriteByte((byte)first);
                for (int i = 2; i < array.Length; i++)
                {
                    int value = array[i];
                    byte[] parts = Enumerable.Range(0, 4).Reverse().Select(p => (byte)((value >> (7 * p)) & 0x7F)).SkipWhile(p => p == 0).ToArray();
                    for (int p = 0; p < parts.Length - 1; p++)
                        parts[p] |= 0x80;
                    ms.Write(parts, 0, parts.Length);
                }
                return ms.ToArray();
            }
        }

        protected override void ReadBody(Stream stream)
        {
            long totalRead = 0;
            ulong number = 0;
            bool first = true;
            var sb = new StringBuilder();

            while (totalRead < Length)
            {
                int readResult = stream.ReadByte();
                if (readResult == -1)
                    throw new EndOfStreamException();
                totalRead++;

                byte b = (byte)readResult;
                number += (byte)(b & 0x7F);
                if ((b & 0x80) == 0)
                {
                    if (first)
                    {
                        if (number < 40)
                            sb.Append('0');
                        else if (number < 80)
                        {
                            sb.Append('1');
                            number -= 40;
                        }
                        else
                        {
                            sb.Append('2');
                            number -= 80;
                        }
                        first = false;
                    }
                    sb.Append('.');
                    sb.Append(number);
                    number = 0;
                }
                else
                    checked
                    {
                        number *= 128;
                    }
            }

            string oidString = sb.ToString();
            var oid = OID.Find(oidString);
            if (oid != null)
                this.OID = oid;
            else
                this.OID = new OID(oidString);
        }
    }
}