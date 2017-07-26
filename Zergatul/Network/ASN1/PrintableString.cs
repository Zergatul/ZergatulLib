using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class PrintableString : ASN1Element
    {
        public string Value { get; private set; }

        protected override void ReadBody(Stream stream)
        {
            byte[] buffer = ReadBuffer(stream, checked((int)Length));
            Value = Encoding.ASCII.GetString(buffer);
        }
    }
}