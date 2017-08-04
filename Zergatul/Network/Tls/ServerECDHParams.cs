using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class ServerECDHParams
    {
        public ECParameters CurveParams;
        public byte[] Point;

        public byte[] Raw;

        public void Read(BinaryReader reader)
        {
            var raw = new List<byte>();
            reader.StartTracking(raw);

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

            Point = reader.ReadBytes(reader.ReadByte());

            reader.StopTracking();
            this.Raw = raw.ToArray();
        }
    }
}