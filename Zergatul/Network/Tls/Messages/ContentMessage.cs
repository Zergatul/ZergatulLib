using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls.Messages
{
    internal abstract class ContentMessage
    {
        public abstract void Read(BinaryReader reader);
        //public abstract Task ReadAsync(BinaryReader reader, CancellationToken cancellationToken);
        public abstract void Write(BinaryWriter writer);
    }
}