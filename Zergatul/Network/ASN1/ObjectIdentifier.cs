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
            ulong totalRead = 0;
            while (totalRead < Length)
            {
                int readResult = stream.ReadByte();
                if (readResult == -1)
                    throw new EndOfStreamException();

                byte b = (byte)readResult;
            }
        }
    }
}
