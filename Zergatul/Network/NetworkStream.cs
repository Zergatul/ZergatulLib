using System.Threading;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network
{
    public class NetworkStream : System.Net.Sockets.NetworkStream
    {
        public NetworkStream(System.Net.Sockets.Socket socket)
            : base(socket)
        {

        }

        public NetworkStream(System.Net.Sockets.Socket socket, bool ownsSocket)
            : base(socket, ownsSocket)
        {

        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return 0;

            return await SocketHelper.ReceiveAsync(Socket, buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (StreamHelper.ValidateReadWriteParameters(buffer, offset, count))
                return;

            await SocketHelper.SendAsync(Socket, buffer, offset, count);
        }
    }
}