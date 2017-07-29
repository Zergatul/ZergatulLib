using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.ASN1
{
    public static class ASN1ContextRules
    {
        /// <summary>
        /// https://tools.ietf.org/html/rfc5280#page-38
        /// </summary>
        public static readonly Dictionary<int, Type> GeneralName = new Dictionary<int, Type>
        {
            [1] = typeof(IA5String),
            [2] = typeof(IA5String),
            [6] = typeof(IA5String),
            [7] = typeof(OctetString),
            [8] = typeof(ObjectIdentifier)
        };
    }
}