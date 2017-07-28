using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public abstract class ASN1TimeElement : ASN1Element
    {
        public DateTime Date { get; protected set; }
    }
}