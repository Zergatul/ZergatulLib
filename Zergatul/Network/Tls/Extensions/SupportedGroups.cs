using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc4492#section-5.1.1
    /// https://tools.ietf.org/html/rfc7919
    /// </summary>
    public class SupportedGroups : TlsExtension
    {
        private NamedGroup[] _curves;
        public NamedGroup[] Curves
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

        public SupportedGroups(params NamedGroup[] curves)
            : base(ExtensionType.SupportedGroups)
        {
            this.Curves = curves;
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadShort());
            var list = new List<NamedGroup>();
            while (counter.CanRead)
                list.Add((NamedGroup)reader.ReadShort());

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