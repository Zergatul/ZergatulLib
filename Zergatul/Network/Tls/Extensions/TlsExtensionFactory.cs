using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    internal class TlsExtensionFactory
    {
        public TlsExtension Read(BinaryReader reader)
        {
            var type = (ExtensionType)reader.ReadShort();

            TlsExtension ext;
            switch (type)
            {
                case ExtensionType.ServerName:
                    ext = new ServerNameExtension();
                    break;
                case ExtensionType.SignatureAlgorithms:
                    ext = new SignatureAlgorithmsExtension();
                    break;
                default:
                    ext = new TlsExtension(type);
                    break;
            }

            ext.Read(reader);

            return ext;
        }
    }
}
