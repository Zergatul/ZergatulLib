using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public enum ASN1ValueType : byte
    {
        Primitive = 0,
        Constructed = 1
    }
}