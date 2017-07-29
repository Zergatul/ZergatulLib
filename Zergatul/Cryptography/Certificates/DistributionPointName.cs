using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#page-47
    /// </summary>
    public class DistributionPointName
    {
        public GeneralNames FullName { get; private set; }
        public string NameRelativeToCRLIssuer { get; private set; }

        internal DistributionPointName(ASN1Element element)
        {

        }
    }
}