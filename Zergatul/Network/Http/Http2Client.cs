using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Http
{
    public class Http2Client : IDisposable
    {
        #region Static members

        private static readonly byte[] ClientPreface = new byte[]
        {
            0x50, 0x52, 0x49, 0x20, 0x2a, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x32,
            0x2e, 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x53, 0x4d, 0x0d, 0x0a, 0x0d, 0x0a
        };

        #endregion

        private bool _disposed;
        private Stream _stream;

        public Http2Client(string uri)
            : this(new Uri(uri), null)
        {
            
        }

        public Http2Client(Uri uri, Proxy.ProxyBase proxy)
        {
            if (uri.Query != "")
                throw new ArgumentException(nameof(uri));
            if (uri.AbsolutePath != "/")
                throw new ArgumentException(nameof(uri));

            var client = TcpConnector.GetTcpClient(uri.Host, uri.Port, proxy);
            switch (uri.Scheme)
            {
                case "http":
                    _stream = client.GetStream();
                    break;
                case "https":
                    var tls = new TlsStream(client.GetStream());
                    tls.AuthenticateAsClient(uri.Host);
                    _stream = new BufferedStream(tls, Tls.Messages.RecordMessageStream.PlaintextLimit);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            Init();
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _stream.Dispose();

            _disposed = true;
        }

        #endregion

        private void Init()
        {
            _stream.Write(ClientPreface, 0, ClientPreface.Length);
            //SendFrame(FrameType.SETTINGS, 0, null);
            _stream.Flush();

            ReadFrame(out FrameType type, out uint id, out byte[] payload);
            if (type != FrameType.SETTINGS)
                throw new InvalidOperationException();
        }

        private void SendFrame(FrameType type, uint id, byte[] payload)
        {
            if ((id & 0x80000000) != 0)
                throw new InvalidOperationException("High bit should be 0");
            int length = payload?.Length ?? 0;
            if (length > 0x4000)
                throw new InvalidOperationException("Frame too big");

            byte[] header = new byte[9];
            header[1] = (byte)(length >> 8);
            header[2] = (byte)(length & 0xFF);
            header[3] = (byte)type;
            BitHelper.GetBytes(id, ByteOrder.BigEndian, header, 5);

            _stream.Write(header, 0, 9);
            if (length > 0)
                _stream.Write(payload, 0, length);
        }

        private void ReadFrame(out FrameType type, out uint id, out byte[] payload)
        {
            byte[] header = new byte[9];
            ReadArray(header);
            if (header[0] != 0)
                throw new InvalidOperationException("Frame too big");
            int length = (header[1] << 8) | header[2];
            type = (FrameType)header[3];
            if (header[4] != 0)
                throw new InvalidOperationException("Flags should be zero");
            id = BitHelper.ToUInt32(header, 5, ByteOrder.BigEndian);
            id &= 0x7FFFFFFF;

            if (length == 0)
                payload = null;
            else
            {
                payload = new byte[length];
                ReadArray(payload);
            }
        }

        private void ReadArray(byte[] data)
        {
            int index = 0;
            while (index < data.Length)
            {
                int read = _stream.Read(data, index, data.Length - index);
                if (read == 0)
                    throw new EndOfStreamException();
                index += read;
            }
        }

        #region Nested classes

        private enum FrameType
        {
            SETTINGS = 0x04
        }

        #endregion
    }
}