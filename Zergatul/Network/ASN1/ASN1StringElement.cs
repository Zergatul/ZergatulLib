using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1
{
    public abstract class Asn1StringElement : Asn1Element
    {
        public Asn1StringElement(Asn1Tag tag)
            : base(tag)
        {

        }

        public string Value { get; protected set; }
    }
}