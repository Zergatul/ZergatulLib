using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    public class TlsExtension
    {
        public ExtensionType Type;
        public ushort Length => (ushort)(Data?.Length ?? 0);

        protected byte[] _data;
        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                ParseData();
            }
        }

        public TlsExtension()
        {

        }

        public TlsExtension(ExtensionType type)
        {
            Type = type;
        }

        internal void Read(BinaryReader reader)
        {
            Data = reader.ReadBytes(reader.ReadShort());
        }

        protected virtual void ParseData()
        {

        }

        protected virtual void FormatData()
        {

        }
    }
}