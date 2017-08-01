using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    public class UserNotice
    {
        public NoticeReference NoticeRef { get; private set; }
        public string ExplicitText { get; private set; }

        internal UserNotice(ASN1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);

            if (seq.Elements.Count >= 1)
                NoticeRef = new NoticeReference(seq.Elements[0]);
            if (seq.Elements.Count >= 2)
            {
                CertificateParseException.ThrowIfFalse(seq.Elements[1] is ASN1StringElement);
                ExplicitText = ((ASN1StringElement)seq.Elements[1]).Value;
            }
        }
    }
}