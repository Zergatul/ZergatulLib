using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls
{
    internal class ServerECDHParams
    {
        public ECParameters CurveParams;
        public ECPoint Point;

        public void Read(BinaryReader reader)
        {
            CurveParams = new ECParameters
            {
                CurveType = (ECCurveType)reader.ReadByte()
            };

            switch (CurveParams.CurveType)
            {
                case ECCurveType.NamedCurve:
                    CurveParams.NamedCurve = (NamedCurve)reader.ReadShort();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Point = new ECPoint
            {
                Point = reader.ReadBytes(reader.ReadByte())
            };
        }
    }
}