using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.13
    /// </summary>
    public class CRLDistributionPoints : X509Extension
    {
        public DistributionPoint[] Points { get; private set; }

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);
            var seq = element as Sequence;
            if (seq != null)
            {
                Points = seq.Elements.Select(e => new DistributionPoint(e)).ToArray();
            }
        }
    }
}