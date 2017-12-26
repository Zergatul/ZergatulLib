using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Cryptography.Hash;
using Zergatul.Math;
using Zergatul.Math.EdwardsCurves;

namespace Zergatul.Cryptography.Asymmetric
{
    public class EdDSAParameters : AbstractParameters
    {
        public EdCurve Curve { get; set; }

        /// <summary>
        /// An odd prime power p. EdDSA uses an elliptic curve over the finite field GF(p).
        /// </summary>
        public BigInteger p { get; protected set; }

        /// <summary>
        /// 2^(b-1) > p
        /// </summary>
        public int b { get; protected set; }

        /// <summary>
        /// A (b-1)-bit encoding of elements of the finite field GF(p).
        /// </summary>
        //public abstract byte[] Encoding(BigInteger x, BigInteger y);

        /// <summary>
        /// A cryptographic hash function H producing 2*b-bit output.
        /// </summary>
        //public abstract byte[] Hash(byte[] data);

        /// <summary>
        /// An integer c that is 2 or 3.  Secret EdDSA scalars are multiples of 2^c.The integer c is the base-2 logarithm of the so-called cofactor.
        /// </summary>
        public int c { get; protected set; }

        /// <summary>
        /// An integer n with c &lt;= n &lt; b. Secret EdDSA scalars have exactly n + 1 bits, with the top bit(the 2^n position) always set and the bottom c bits always cleared.
        /// </summary>
        public int n { get; protected set; }

        /// <summary>
        /// A non-square element d of GF(p).  The usual recommendation is to take it as the value nearest to zero that gives an acceptable curve.
        /// </summary>
        public object d { get; protected set; }

        /// <summary>
        /// A non-zero square element a of GF(p).  The usual recommendation for best performance is a = -1 if p mod 4 = 1, and a = 1 if p mod 4 = 3.
        /// </summary>
        public BigInteger a { get; protected set; }

        public BigInteger Bx { get; protected set; }
        public BigInteger By { get; protected set; }

        public BigInteger L { get; protected set; }

        //public static Ed25519 Ed25519 => new Ed25519();
    }

    //public class Ed25519 : EdDSAParameters
    //{
    //    private static readonly BigInteger _p = BigInteger.ShiftLeft(BigInteger.One, 255) - 19;
    //    private static readonly BigInteger _d = BigInteger.Parse("37095705934669439343138083508754565189542113879843219016388785533085940283555");
    //    private static readonly BigInteger _a = _p - 1;
    //    private static readonly BigInteger _bx = BigInteger.Parse("15112221349535400772501151409588531511454012693041857206046113283949847762202");
    //    private static readonly BigInteger _by = BigInteger.Parse("46316835694926478169428394003475163141307993866256225615783033603165251855960");
    //    private static readonly BigInteger _L = BigInteger.ShiftLeft(BigInteger.One, 252) + BigInteger.Parse("27742317777372353535851937790883648493");

    //    private SHA512 _hash = new SHA512();

    //    public Ed25519()
    //    {
    //        this.p = _p;
    //        this.b = 256;
    //        this.c = 3;
    //        this.n = 254;
    //        this.a = _a;
    //        this.Bx = _bx;
    //        this.By = _by;
    //        this.L = _L;
    //    }

    //    public override byte[] Hash(byte[] data)
    //    {
    //        _hash.Reset();
    //        _hash.Update(data);
    //        return _hash.ComputeHash();
    //    }

    //    public override byte[] Encoding(BigInteger x, BigInteger y)
    //    {
    //        return ByteArray.Concat(x.ToBytes(ByteOrder.LittleEndian, 32), y.ToBytes(ByteOrder.LittleEndian, 32));
    //    }
    //}
}