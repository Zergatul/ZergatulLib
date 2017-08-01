using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class IA5String : ASN1StringElement
    {
        protected override void ReadBody(Stream stream)
        {
            byte[] buffer = ReadBuffer(stream, checked((int)Length));
            ReadBody(buffer);
        }

        protected override void ReadBody(byte[] data)
        {
            Value = Encoding.ASCII.GetString(data);
        }
    }
}