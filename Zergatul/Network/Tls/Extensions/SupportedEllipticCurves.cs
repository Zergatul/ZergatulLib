using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc4492#section-5.1.1
    /// </summary>
    public class SupportedEllipticCurves : TlsExtension
    {
        private NamedCurve[] _curves;
        public NamedCurve[] Curves
        {
            get
            {
                return _curves;
            }
            set
            {
                _curves = value;
                FormatData();
            }
        }

        public SupportedEllipticCurves(params NamedCurve[] curves)
            : base(ExtensionType.SupportedEllipticCurves)
        {
            this.Curves = curves;
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadShort());
            var list = new List<NamedCurve>();
            while (counter.CanRead)
                list.Add((NamedCurve)reader.ReadShort());

            _curves = list.ToArray();
        }

        protected override void FormatData()
        {
            var data = new List<byte>();
            var writer = new BinaryWriter(data);

            writer.WriteShort((ushort)(_curves.Length * 2));
            for (int i = 0; i < _curves.Length; i++)
                writer.WriteShort((ushort)_curves[i]);

            _data = data.ToArray();
        }
    }
}