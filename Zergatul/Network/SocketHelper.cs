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

        public static async Task<int> ReceiveAsync(Socket socket, byte[] buffer, int offset, int count)
        {
            var tcs = new TaskCompletionSource<int>();

            var args = new SocketAsyncEventArgs();
            args.SetBuffer(buffer, offset, count);
            args.UserToken = tcs;
            args.Completed += ReceiveAsyncCompleted;

            if (socket.ReceiveAsync(args))
                return await tcs.Task;
            else
                return args.BytesTransferred;
        }

        public static async Task SendAsync(Socket socket, byte[] buffer, int offset, int count)
        {
            var tcs = new TaskCompletionSource<bool>();

            var args = new SocketAsyncEventArgs();
            args.SetBuffer(buffer, offset, count);
            args.UserToken = tcs;
            args.Completed += SendAsyncCompleted;

            if (socket.SendAsync(args))
                await tcs.Task;
        }

        private static void ConnectAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                throw new SocketException((int)args.SocketError);

            var tcs = (TaskCompletionSource<bool>)args.UserToken;
            tcs.SetResult(true);
        }

        private static void ReceiveAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                throw new SocketException((int)args.SocketError);

            var tcs = (TaskCompletionSource<int>)args.UserToken;
            tcs.SetResult(args.BytesTransferred);
        }

        private static void SendAsyncCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
                throw new SocketException((int)args.SocketError);

            var tcs = (TaskCompletionSource<bool>)args.UserToken;
            tcs.SetResult(true);
        }
    }
}