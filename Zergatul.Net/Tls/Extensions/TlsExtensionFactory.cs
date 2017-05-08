using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Net.Tls.Extensions
{
    internal class TlsExtensionFactory
    {
        public TlsExtension Read(BinaryReader reader)
        {
            var type = (ExtensionType)reader.ReadShort();
            switch (type)
            {
                case ExtensionType.SignatureAlgorithms:
                    return new SignatureAlgorithmsExtension(reader);
                default:
                    return new TlsExtension(type, reader);
            }
        }
    }
}
