using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc4492#section-5.1.2
    /// </summary>
    public class SupportedPointFormats : TlsExtension
    {
        private ECPointFormat[] _formats;
        public ECPointFormat[] Formats
        {
            get
            {
                return _formats;
            }
            set
            {
                _formats = value;
                FormatData();
            }
        }

        public SupportedPointFormats(params ECPointFormat[] formats)
            : base(ExtensionType.ECPointFormats)
        {
            this.Formats = formats;
        }

        protected override void ParseData()
        {
            var reader = new BinaryReader(_data);
            var counter = reader.StartCounter(reader.ReadByte());
            var list = new List<ECPointFormat>();
            while (counter.CanRead)
                list.Add((ECPointFormat)reader.ReadByte());

            _formats = list.ToArray();
        }

        protected override void FormatData()
        {
            var data = new List<byte>();
            var writer = new BinaryWriter(data);

            writer.WriteByte((byte)(_formats.Length));
            for (int i = 0; i < _formats.Length; i++)
                writer.WriteByte((byte)_formats[i]);

            _data = data.ToArray();
        }
    }
}