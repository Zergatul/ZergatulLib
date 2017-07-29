using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.ASN1;

namespace Zergatul.Cryptography.Certificates
{
    public class GeneralNames
    {
        public IReadOnlyList<GeneralName> List => _list.AsReadOnly();

        private List<GeneralName> _list;

        internal GeneralNames(ASN1Element element)
        {
            var seq = element as Sequence;

            CertificateParseException.ThrowIfFalse(seq != null);

            _list = seq.Elements.Select(e => new GeneralName(e)).ToList();
        }
    }
}