using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1.Structures
{
    // https://tools.ietf.org/html/rfc3279
    public class DSAPrivateKey
    {
        public BigInteger p { get; private set; }
        public BigInteger q { get; private set; }
        public BigInteger g { get; private set; }
        public BigInteger x { get; private set; }
        public BigInteger y { get; private set; }

        public static DSAPrivateKey Parse(ASN1Element element)
        {
            return new DSAPrivateKey
            {
                x = (element as Integer).Value
            };

            /*var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotEqual(seq.Elements.Count, 6);

            if (seq.Elements.OfType<Integer>().Count() != 6)
                throw new ParseException();

            int version = (int)((Integer)seq.Elements[0]).Value;

            return new DSAPrivateKey
            {
                p = ((Integer)seq.Elements[1]).Value,
                q = ((Integer)seq.Elements[2]).Value,
                g = ((Integer)seq.Elements[3]).Value,
                y = ((Integer)seq.Elements[4]).Value,
                x = ((Integer)seq.Elements[5]).Value,
            };*/
        }
    }
}