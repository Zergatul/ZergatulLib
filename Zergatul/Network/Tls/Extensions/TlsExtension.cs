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
        public virtual ushort Length => (ushort)(Data?.Length ?? 0);

        protected byte[] _data;
        public virtual byte[] Data
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

        public static TlsExtension Resolve(ExtensionType type)
        {
            switch (type)
            {
                case ExtensionType.ExtendedMasterSecret:
                    return new ExtendedMasterSecret();
                case ExtensionType.SupportedGroups:
                    return new SupportedGroups();
                case ExtensionType.ECPointFormats:
                    return new SupportedPointFormats();
                default:
                    return new TlsExtension(type);
            }
        }
    }
}