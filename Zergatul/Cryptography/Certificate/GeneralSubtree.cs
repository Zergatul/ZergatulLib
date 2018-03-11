using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;
using Zergatul.Network.Asn1;

namespace Zergatul.Cryptography.Certificate
{
    public class GeneralSubtree
    {
        public GeneralName Base { get; private set; }
        public BigInteger Mininum { get; private set; }
        public BigInteger Maximum { get; private set; }

        internal GeneralSubtree(Asn1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 1, 3);

            Base = new GeneralName(seq.Elements[0]);

            if (seq.Elements.Count >= 2)
            {
                var integer = seq.Elements[1] as Integer;
                ParseException.ThrowIfNull(integer);
                Mininum = integer.Value;
            }
            else
                Mininum = BigInteger.Zero;

            if (seq.Elements.Count >= 3)
            {
                var integer = seq.Elements[2] as Integer;
                ParseException.ThrowIfNull(integer);
                Maximum = integer.Value;
            }
        }
    }
}