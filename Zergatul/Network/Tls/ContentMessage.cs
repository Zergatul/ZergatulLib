using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal abstract class ContentMessage
    {
        public abstract void Read(BinaryReader reader);

        public abstract void Write(BinaryWriter writer);
    }
}