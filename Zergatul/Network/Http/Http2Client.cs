using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;
using Zergatul.Network.Http.Frames;
using Zergatul.Network.Tls;

namespace Zergatul.Network.Http
{
    // TODO
    // implement settings timeout https://tools.ietf.org/html/rfc7540#section-6.5.3
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
        private Http2Connection _connection;
        private bool _isClosed;

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
                    var alpnExtension = new Tls.Extensions.ApplicationLayerProtocolNegotiationExtension();
                    alpnExtension.ProtocolNames = new[] { "h2" };
                    tls.Settings.Extensions = new Tls.Extensions.TlsExtension[] { alpnExtension };
                    tls.AuthenticateAsClient(uri.Host);
                    _stream = new BufferedWriteStream(tls, Tls.Messages.RecordMessageStream.PlaintextLimit);
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
            {
                if (!_isClosed)
                    SendGoAway(ErrorCode.NO_ERROR);
            }

            _disposed = true;
        }

        #endregion

        private void Init()
        {
            _connection = new Http2Connection(_stream);
            _connection.Open();
            _connection.SendFrame(new Settings());
            _connection.Flush();

            var frame = _connection.ReadFrame();
            if (frame.Type != FrameType.SETTINGS)
            {
                SendGoAway(ErrorCode.PROTOCOL_ERROR);
                return;
            }

            var settings = (Settings)frame;
            if (settings.ACK)
            {
                SendGoAway(ErrorCode.PROTOCOL_ERROR);
                return;
            }
            ApplySettings(settings);

            _connection.SendFrame(new Settings
            {
                ACK = true
            });
            _connection.Flush();

            frame = _connection.ReadFrame();
            if (frame.Type != FrameType.SETTINGS)
            {
                SendGoAway(ErrorCode.PROTOCOL_ERROR);
                return;
            }

            settings = (Settings)frame;
            if (!settings.ACK)
            {
                SendGoAway(ErrorCode.PROTOCOL_ERROR);
                return;
            }
        }

        private void ApplySettings(Settings frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            foreach (var kv in frame.Parameters)
                switch (kv.Key)
                {
                    case SettingParameter.SETTINGS_HEADER_TABLE_SIZE:
                        _connection.PeerSettings.HeaderTableSize = kv.Value;
                        break;

                    case SettingParameter.SETTINGS_ENABLE_PUSH:
                        _connection.PeerSettings.EnablePush = kv.Value == 1 ? true : false;
                        break;

                    case SettingParameter.SETTINGS_MAX_CONCURRENT_STREAMS:
                        _connection.PeerSettings.MaxConcurrentStreams = kv.Value;
                        break;

                    case SettingParameter.SETTINGS_INITIAL_WINDOW_SIZE:
                        _connection.PeerSettings.InitialWindowSize = kv.Value;
                        break;

                    case SettingParameter.SETTINGS_MAX_FRAME_SIZE:
                        _connection.PeerSettings.MaxFrameSize = kv.Value;
                        break;

                    case SettingParameter.SETTINGS_MAX_HEADER_LIST_SIZE:
                        _connection.PeerSettings.MaxHeaderListSize = kv.Value;
                        break;
                }
        }

        private void SendGoAway(ErrorCode errorCode)
        {
            _isClosed = true;
            _connection.SendFrame(new GoAway
            {
                ErrorCode = errorCode
            });
            _connection.Flush();
            _connection.Close();
        }
    }
}