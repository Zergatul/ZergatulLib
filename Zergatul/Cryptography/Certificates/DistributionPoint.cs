using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.13
    /// </summary>
    public class DistributionPoint
    {
        public DistributionPointName Name { get; private set; }
        public ReasonFlags Reasons { get; private set; }
        public string CRLIssuer { get; private set; }

        internal static DistributionPoint Parse(ASN1Element element)
        {
            var result = new DistributionPoint();

            var seq = element as Sequence;
            if (seq != null)
                foreach (var cs in seq.Elements.Cast<ContextSpecific>())
                    switch (cs.Tag.TagNumberEx)
                    {
                        case 0:
                            result.Name = DistributionPointName.Parse(cs.Elements[0]);
                            break;
                        case 1:
                            result.Reasons = ReasonFlags.Parse(cs.Elements[1]);
                            break;
                        case 2:
                            result.
                            break;
                    }

            throw new InvalidOperationException();
        }
    }
}
