using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.IO
{
    public static class StreamHelper
    {
        public static Task CopyToAsync(Stream source, Stream destination, CancellationToken cancellationToken)
        {
            return source.CopyToAsync(destination, 81920, cancellationToken);
        }

        public static void ReadArray(Stream stream, byte[] data)
        {
            int index = 0;
            while (index < data.Length)
            {
                int read = stream.Read(data, index, data.Length - index);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        public static async Task ReadArray(Stream stream, byte[] data, CancellationToken cancellationToken)
        {
            int index = 0;
            while (index < data.Length)
            {
                int read = await stream.ReadAsync(data, index, data.Length - index, cancellationToken);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        public static void ReadArray(Stream stream, byte[] data, int length)
        {
            int index = 0;
            while (index < length)
            {
                int read = stream.Read(data, index, length - index);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        public static bool ValidateReadWriteParameters(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (count == 0)
                return true;

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            return false;
        }

        public static IAsyncResult BeginReadFromTapToApm(Stream stream, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<int>(state);
            stream.ReadAsync(buffer, offset, count).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                callback?.Invoke(tcs.Task);
            });

            return tcs.Task;
        }

        public static int EndReadFromTapToApm(IAsyncResult asyncResult)
        {
            return ((Task<int>)asyncResult).Result;
        }

        public static IAsyncResult BeginWriteFromTapToApm(Stream stream, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<int>(state);
            stream.WriteAsync(buffer, offset, count).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(0);

                callback?.Invoke(tcs.Task);
            });

            return tcs.Task;
        }

        public static void EndWriteFromTapToApm(IAsyncResult asyncResult)
        {
            ((Task)asyncResult).Wait();
        }
    }
}