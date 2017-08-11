using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc4492#page-21
    /// </summary>
    public class ECDSASignatureValue
    {
        public BigInteger r { get; private set; }
        public BigInteger s { get; private set; }

        private ECDSASignatureValue()
        {

        }

        public ECDSASignatureValue(BigInteger r, BigInteger s)
        {
            this.r = r;
            this.s = s;
        }

        public byte[] ToBytes()
        {
            var seq = new Sequence(new Integer(r), new Integer(s));
            return seq.ToBytes();
        }

        public static ECDSASignatureValue Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 2);

            var int1 = seq.Elements[0] as Integer;
            ParseException.ThrowIfNull(int1);

            var int2 = seq.Elements[1] as Integer;
            ParseException.ThrowIfNull(int2);

            return new ECDSASignatureValue { r = int1.Value, s = int2.Value };
        }
    }
}