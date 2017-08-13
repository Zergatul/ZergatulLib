using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Extensions
{
    /// <summary>
    /// https://tools.ietf.org/html/rfc7627
    /// </summary>
    public class ExtendedMasterSecret : TlsExtension
    {
        public override ushort Length => 0;

        public override byte[] Data
        {
            get
            {
                return new byte[0];
            }
            set
            {
                if (value.Length != 0)
                    throw new InvalidOperationException();
            }
        }

        public ExtendedMasterSecret()
            : base(ExtensionType.ExtendedMasterSecret)
        {
        }
    }
}