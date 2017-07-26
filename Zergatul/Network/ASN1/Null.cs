using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public class Null : ASN1Element
    {
        protected override void ReadBody(Stream stream)
        {
            if (Length != 0)
                throw new NotImplementedException();
        }
    }
}