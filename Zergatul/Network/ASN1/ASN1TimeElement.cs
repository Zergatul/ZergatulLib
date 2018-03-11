using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public abstract class Asn1TimeElement : Asn1Element
    {
        public DateTime Date { get; protected set; }

        public Asn1TimeElement(Asn1Tag tag)
            : base(tag)
        {

        }
    }
}