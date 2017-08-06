using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public abstract class ASN1StringElement : ASN1Element
    {
        public ASN1StringElement(ASN1Tag tag)
            : base(tag)
        {

        }

        public string Value { get; protected set; }
    }
}