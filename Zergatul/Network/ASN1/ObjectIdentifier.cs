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

            OID = new OID(sb.ToString());
        }
    }
}