using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Asn1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5480#section-2.1.1
    /// </summary>
    public class ECParameters
    {
        public OID NamedCurve { get; private set; }

        public static ECParameters Parse(Asn1Element element)
        {
            if (element is ObjectIdentifier)
                return new ECParameters
                {
                    NamedCurve = ((ObjectIdentifier)element).OID
                };
            else
                throw new NotImplementedException();
        }
    }
}