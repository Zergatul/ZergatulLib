using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate.Extensions
{
    public class NameConstraints : X509Extension
    {
        public GeneralSubtree[] PermittedSubtrees { get; private set; }
        public GeneralSubtree[] ExcludedSubtrees { get; private set; }

        protected override void Parse(byte[] data)
        {
            var element = Asn1Element.ReadFrom(data);

            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 1, 2);

            foreach (var elem in seq.Elements)
            {
                var cs = elem as ContextSpecific;
                ParseException.ThrowIfNull(cs);

                if (cs.Tag.TagNumberEx == 0)
                {
                    if (PermittedSubtrees != null)
                        throw new ParseException();

                    var seq2 = cs.Elements[0] as Sequence;
                    ParseException.ThrowIfNull(seq2);
                    PermittedSubtrees = seq2.Elements.Select(e => new GeneralSubtree(e)).ToArray();
                }

                if (cs.Tag.TagNumberEx == 1)
                {
                    if (ExcludedSubtrees != null)
                        throw new ParseException();

                    var seq2 = cs.Elements[0] as Sequence;
                    ParseException.ThrowIfNull(seq2);
                    ExcludedSubtrees = seq2.Elements.Select(e => new GeneralSubtree(e)).ToArray();
                }
            }
        }
    }
}