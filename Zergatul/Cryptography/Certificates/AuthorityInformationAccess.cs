using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc5280#section-4.2.2.1
    /// </summary>
    public class AuthorityInformationAccess : X509Extension
    {
        public OID OID { get; private set; }
        public string Value { get; private set; }

        protected override void Parse(OctetString data)
        {
            var element = ASN1Element.ReadFrom(data.Raw);
            var seq = element as Sequence;
            if (seq != null && seq.Elements.Count == 1)
            {
                seq = seq.Elements[0] as Sequence;
                if (seq != null && seq.Elements.Count == 2)
                {
                    var oi = seq.Elements[0] as ObjectIdentifier;
                    var value = seq.Elements[1].ApplyRules(ASN1ContextRules.GeneralName) as ASN1StringElement;
                    if (oi != null && value != null)
                    {
                        OID = oi.OID;
                        Value = value.Value;
                        return;
                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}