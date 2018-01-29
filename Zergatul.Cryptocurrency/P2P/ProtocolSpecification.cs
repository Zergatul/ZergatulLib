using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class ProtocolSpecification
    {
        public uint Magic { get; private set; }
        public ushort Port { get; private set; }

        public static readonly ProtocolSpecification Bitcoin = new ProtocolSpecification
        {
            Magic = 0xD9B4BEF9
        };

        public static readonly ProtocolSpecification Zcash = new ProtocolSpecification
        {
            Magic = 0x6427E924,
            Port = 8233
        };
    }
}