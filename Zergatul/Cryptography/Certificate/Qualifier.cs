using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class Qualifier
    {
        public string CPSUri { get; private set; }

        public UserNotice UserNotice { get; private set; }

        internal Qualifier(OID policyQualifierId, Asn1Element element)
        {
            if (policyQualifierId == OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.QT.CPS)
            {
                var ia5 = element as IA5String;
                CertificateParseException.ThrowIfNull(ia5);
                CPSUri = ia5.Value;
            }
            else if (policyQualifierId == OID.ISO.IdentifiedOrganization.DOD.Internet.Security.Mechanisms.PKIX.QT.UNotice)
            {
                UserNotice = new UserNotice(element);
            }
            else
                throw new NotImplementedException();
        }
    }
}