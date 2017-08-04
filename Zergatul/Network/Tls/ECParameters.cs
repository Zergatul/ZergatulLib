using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class ECParameters
    {
        public ECCurveType CurveType;

        public byte[] PrimeP;
        public ECCurve Curve;
        public byte[] Base;
        public byte[] Order;
        public byte[] Cofactor;

        public ushort M;
        public ECBasisType Basis;
        public byte[] K;
        public byte[] K1;
        public byte[] K2;
        public byte[] K3;

        public NamedCurve NamedCurve;
    }
}
