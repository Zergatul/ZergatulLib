using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1
{
    public class Integer : ASN1Element
    {
        public byte[] Raw { get; private set; }
        public BigInteger Value => new BigInteger(Raw, ByteOrder.BigEndian);

        protected override void ReadBody(byte[] data)
        {
            Raw = data;
        }
    }
}