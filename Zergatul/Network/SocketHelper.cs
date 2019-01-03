using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Zergatul.Network
{
    public static class SocketHelper
    {
        public static async Task ConnectAsync(Socket socket, IPEndPoint endPoint)
        {
            var tcs = new TaskCompletionSource<bool>();

            var args = new SocketAsyncEventArgs();
            args.RemoteEndPoint = endPoint;
            args.UserToken = tcs;
            args.Completed += ConnectAsyncCompleted;

            if (socket.ConnectAsync(args))
                await tcs.Task;
        }

        private static void ConnectAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            var tcs = (TaskCompletionSource<bool>)args.UserToken;
            tcs.SetResult(true);
        }
    }
}