using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificate
{
    public class NoticeReference
    {
        public string Organization { get; private set; }

        public IReadOnlyCollection<int> NoticeNumbers => _noticeNumbers.AsReadOnly();

        private List<int> _noticeNumbers;

        internal NoticeReference(ASN1Element element)
        {
            var seq = element as Sequence;
            CertificateParseException.ThrowIfNull(seq);
            CertificateParseException.ThrowIfFalse(seq.Elements.Count == 2);

            CertificateParseException.ThrowIfFalse(seq.Elements[0] is ASN1StringElement);
            Organization = ((ASN1StringElement)seq.Elements[0]).Value;

            CertificateParseException.ThrowIfFalse(seq.Elements[1] is Sequence);
            CertificateParseException.ThrowIfTrue(((Sequence)seq.Elements[1]).Elements.Any(e => !(e is Integer)));
            _noticeNumbers = ((Sequence)seq.Elements[1]).Elements.Cast<Integer>().Select(e => (int)e.Value).ToList();
        }
    }
}