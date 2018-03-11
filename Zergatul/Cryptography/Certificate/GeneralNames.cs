using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class GeneralNames
    {
        public IReadOnlyList<GeneralName> List => _list.AsReadOnly();

        private List<GeneralName> _list;

        internal GeneralNames(Asn1Element element)
        {
            if (element is Sequence)
                _list = ((Sequence)element).Elements.Select(e => new GeneralName(e)).ToList();
            else if (element is ContextSpecific)
                _list = new List<GeneralName> { new GeneralName(element) };
            else
                throw new CertificateParseException();
        }
    }
}