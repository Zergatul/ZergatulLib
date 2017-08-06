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

        private byte[] _raw;

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

            this._raw = raw.ToArray();
        }

        public byte[] ToBytes()
        {
            if (_raw == null)
                _raw = GetRaw();

            return _raw;
        }

        private byte[] GetRaw()
        {
            var list = new List<byte>();
            var writer = new BinaryWriter(list);

            writer.WriteByte((byte)CurveParams.CurveType);
            switch (CurveParams.CurveType)
            {
                case ECCurveType.NamedCurve:
                    writer.WriteShort((ushort)CurveParams.NamedCurve);
                    break;
                default:
                    throw new NotImplementedException();
            }

            writer.WriteByte((byte)Point.Length);
            writer.WriteBytes(Point);

            return list.ToArray();
        }
    }
}