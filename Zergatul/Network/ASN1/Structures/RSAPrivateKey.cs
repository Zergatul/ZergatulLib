using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Math;

namespace Zergatul.Network.ASN1.Structures
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc3447#appendix-A.1.2
    /// </summary>
    public class RSAPrivateKey
    {
        public int Version { get; private set; }

        /// <summary>
        /// n
        /// </summary>
        public BigInteger Modulus { get; private set; }

        /// <summary>
        /// e
        /// </summary>
        public BigInteger PublicExponent { get; private set; }

        /// <summary>
        /// d
        /// </summary>
        public BigInteger PrivateExponent { get; private set; }

        /// <summary>
        /// p
        /// </summary>
        public BigInteger Prime1 { get; private set; }

        /// <summary>
        /// q
        /// </summary>
        public BigInteger Prime2 { get; private set; }

        /// <summary>
        /// d mod (p - 1)
        /// </summary>
        public BigInteger Exponent1 { get; private set; }

        /// <summary>
        /// d mod (q - 1)
        /// </summary>
        public BigInteger Exponent2 { get; private set; }

        /// <summary>
        /// (inverse of q) mod p
        /// </summary>
        public BigInteger Coefficient { get; private set; }

        public static RSAPrivateKey Parse(ASN1Element element)
        {
            var seq = element as Sequence;
            ParseException.ThrowIfNull(seq);
            ParseException.ThrowIfNotInRange(seq.Elements.Count, 9, 10);

            for (int i = 0; i < 9; i++)
                if (!(seq.Elements[i] is Integer))
                    throw new ParseException();

            var result = new RSAPrivateKey();

            result.Version = (int)((Integer)seq.Elements[0]).Value;
            result.Modulus = ((Integer)seq.Elements[1]).Value;
            result.PublicExponent = ((Integer)seq.Elements[2]).Value;
            result.PrivateExponent = ((Integer)seq.Elements[3]).Value;
            result.Prime1 = ((Integer)seq.Elements[4]).Value;
            result.Prime2 = ((Integer)seq.Elements[5]).Value;
            result.Exponent1 = ((Integer)seq.Elements[6]).Value;
            result.Exponent2 = ((Integer)seq.Elements[7]).Value;
            result.Coefficient = ((Integer)seq.Elements[8]).Value;

            return result;
        }
    }
}