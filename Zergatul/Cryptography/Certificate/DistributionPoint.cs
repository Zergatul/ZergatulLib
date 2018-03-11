using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.1.13
    /// </summary>
    public class DistributionPoint
    {
        public DistributionPointName Name { get; private set; }
        public ReasonFlags Reasons { get; private set; }
        public GeneralNames CRLIssuer { get; private set; }

        internal DistributionPoint(Asn1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfFalse(seq != null);

            foreach (var cs in seq.Elements.Cast<ContextSpecific>())
                switch (cs.Tag.TagNumberEx)
                {
                    case 0:
                        Name = new DistributionPointName(cs.Elements[0]);
                        break;
                    case 1:
                        Reasons = new ReasonFlags(cs.Elements[0]);
                        break;
                    case 2:
                        CRLIssuer = new GeneralNames(cs.Elements[0]);
                        break;
                    default:
                        throw new CertificateParseException();
                }
        }
    }
}