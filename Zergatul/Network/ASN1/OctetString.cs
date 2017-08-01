using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class OctetString : ASN1Element
    {
        public byte[] Raw { get; private set; }

        protected override void ReadBody(byte[] data)
        {
            Raw = data;
        }
    }
}