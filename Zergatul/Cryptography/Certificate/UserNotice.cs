using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc3280#page-32
    /// </summary>
    public class UserNotice
    {
        public NoticeReference NoticeRef { get; private set; }
        public string ExplicitText { get; private set; }

        internal UserNotice(Asn1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);

            for (int i = 0; i < seq.Elements.Count; i++)
            {
                if (seq.Elements[i] is Sequence)
                    NoticeRef = new NoticeReference(seq.Elements[i]);
                if (seq.Elements[i] is Asn1StringElement)
                    ExplicitText = ((Asn1StringElement)seq.Elements[i]).Value;
            }
        }
    }
}