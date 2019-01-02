using System;
using System.IO;
using Zergatul.Network.Http.Frames;

namespace Zergatul.Network.Http
{
    public class Http2Connection
    {
        #region Static members

        private static readonly byte[] ClientPreface = new byte[]
        {
            0x50, 0x52, 0x49, 0x20, 0x2a, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x32,
            0x2e, 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x53, 0x4d, 0x0d, 0x0a, 0x0d, 0x0a
        };

        #endregion

        private Stream _stream;

        public Http2ConnectionSettings Settings;
        public Http2ConnectionSettings PeerSettings;

        public Http2Connection(Stream stream)
        {
            this._stream = stream;

            this.Settings = new Http2ConnectionSettings();
            this.PeerSettings = new Http2ConnectionSettings();
        }

        public void Open()
        {
            _stream.Write(ClientPreface, 0, ClientPreface.Length);
        }

        public void Close()
        {
            _stream.Close();
        }

        public void SendFrame(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            frame.Write(_stream);
        }

        public Frame ReadFrame()
        {
            return Frame.Read(_stream);
        }

        public void Flush() => _stream.Flush();
    }
}