using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class Boolean : ASN1Element
    {
        public bool Value { get; private set; }

        protected override void ReadBody(Stream stream)
        {
            if (Length != 1)
                throw new NotImplementedException();

            int readResult = stream.ReadByte();
            if (readResult == -1)
                throw new EndOfStreamException();
            Value = readResult != 0;
        }
    }
}