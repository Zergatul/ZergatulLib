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
    // TODO: move http2connection here
    public class Http2Client : IDisposable
    {
        #region Static members

        private static readonly byte[] ClientPreface = new byte[]
        {
            0x50, 0x52, 0x49, 0x20, 0x2a, 0x20, 0x48, 0x54, 0x54, 0x50, 0x2f, 0x32,
            0x2e, 0x30, 0x0d, 0x0a, 0x0d, 0x0a, 0x53, 0x4d, 0x0d, 0x0a, 0x0d, 0x0a
        };

        #endregion

        #region Public properties

        private bool _enablePush = true;
        public bool EnablePush
        {
            get => _enablePush;
            set
            {
                if (_isOpened)
                    throw new NotImplementedException();
                _enablePush = value;
            }
        }

        #endregion

        private Uri _uri;
        private Proxy.ProxyBase _proxy;
        private Stream _stream;
        private Http2Connection _connection;
        private State _state;
        private bool _isOpened;
        private bool _disposed;

        private Hpack _clientHpack;
        private Hpack _serverHpack;
        private uint _streamIdCounter;

        private List<Frame> _frameBuffer;

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

            this._uri = uri;
            this._proxy = proxy;
        }

        #region Public methods

        public void Open()
        {
            ThrowIfOpened();
            ThrowIfDisposed();

            var client = TcpConnector.GetTcpClient(_uri.Host, _uri.Port, _proxy);
            Stream stream;
            switch (_uri.Scheme)
            {
                case "http":
                    stream = client.GetStream();
                    break;

                case "https":
                    var tls = new TlsStream(client.GetStream());
                    var alpnExtension = new Tls.Extensions.ApplicationLayerProtocolNegotiationExtension();
                    alpnExtension.ProtocolNames = new[] { "h2" };
                    tls.Settings.Extensions = new Tls.Extensions.TlsExtension[] { alpnExtension };
                    tls.AuthenticateAsClient(_uri.Host);
                    stream = new BufferedWriteStream(tls, Tls.Messages.RecordMessageStream.PlaintextLimit);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            Open(stream);
        }

        public void Open(Stream stream)
        {
            ThrowIfOpened();
            ThrowIfDisposed();

            _stream = stream;

            InitConnection();

            _clientHpack = new Hpack((int)_connection.Settings.HeaderTableSize);
            _serverHpack = new Hpack((int)_connection.PeerSettings.HeaderTableSize);

            _frameBuffer = new List<Frame>();
            _streamIdCounter = 1;

            _isOpened = true;
        }

        public Http2Response GetResponse(Http2Request request)
        {
            ThrowIfNotOpened();
            ThrowIfDisposed();

            bool valid = !string.IsNullOrEmpty(request.Method);
            if (valid)
            {
                if (request.Method != "CONNECT")
                {
                    valid &= !string.IsNullOrEmpty(request.Scheme);
                    valid &= !string.IsNullOrEmpty(request.Path);
                }
            }

            if (!valid)
                throw new InvalidOperationException("Invalid headers");

            uint streamId = _streamIdCounter;
            _streamIdCounter += 2;
            SendHeaders(streamId, request.Headers);
            _stream.Flush();

            return new Http2Response(this, streamId);
        }

        public Http2Response GetResponse(string path, IEnumerable<Header> headers)
        {
            var request = new Http2Request
            {
                Method = "GET",
                Scheme = _uri.Scheme,
                Authority = _uri.Host,
                Path = path,
            };
            if (headers != null)
                foreach (var header in headers)
                    request[header.Name] = header.Value;

            return GetResponse(request);
        }

        #endregion

        #region Internal methods

        internal Frame GetNextFrameForStream(uint streamId)
        {
            for (int i = 0; i < _frameBuffer.Count; i++)
                if (_frameBuffer[i].Id == streamId)
                {
                    var bufferedFrame = _frameBuffer[i];
                    _frameBuffer.RemoveAt(i);
                    return bufferedFrame;
                }

            while (true)
            {
                var frame = _connection.ReadFrame();
                if (frame.Id == streamId)
                    return frame;
                else
                {
                    if (!ProcessFrame(frame))
                        _frameBuffer.Add(frame);
                }
            }
        }

        internal List<Header> DecodeHeaders(byte[] data)
        {
            using (var ms = new MemoryStream(data))
                return _serverHpack.Decode(ms);
        }

        #endregion

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
                if (_isOpened)
                    SendGoAway(ErrorCode.NO_ERROR);
            }

            _disposed = true;
        }

        #endregion

        #region Private methods

        private void ThrowIfOpened()
        {
            if (_isOpened)
                throw new InvalidOperationException();
        }

        private void ThrowIfNotOpened()
        {
            if (!_isOpened)
                throw new InvalidOperationException();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Http2Client));
        }

        private void InitConnection()
        {
            _connection = new Http2Connection(_stream);
            _connection.Open();

            _state = State.Init;

            // Send SETTINGS
            _connection.SendFrame(new SettingsFrame
            {
                Parameters = new Dictionary<SettingParameter, uint>
                {
                    [SettingParameter.SETTINGS_ENABLE_PUSH] = _enablePush ? 1U : 0U
                }
            });
            _connection.Flush();
            _state |= State.SettingsSend;

            // wait until we send ACK and receive ACK from server
            while (!_state.HasFlag(State.Ready))
                GetAndProcessNextFrame();
        }

        private void ApplySettings(SettingsFrame frame)
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

        private void GetAndProcessNextFrame()
        {
            var frame = _connection.ReadFrame();
            if (!ProcessFrame(frame))
                _frameBuffer.Add(frame);
        }

        private bool ProcessFrame(Frame frame)
        {
            switch (frame)
            {
                case SettingsFrame settingsFrame:
                    ProcessSettingsFrame(settingsFrame);
                    return true;

                case WindowUpdateFrame wndUpdFrame:
                    if (_state == State.Init)
                    {
                        SendGoAway(ErrorCode.PROTOCOL_ERROR);
                        throw new InvalidOperationException();
                    }

                    if (wndUpdFrame.Id != 0)
                        throw new NotImplementedException();
                    _connection.PeerSettings.InitialWindowSize += wndUpdFrame.Increment;
                    return true;

                default:
                    return false;
            }
        }

        private void ProcessSettingsFrame(SettingsFrame frame)
        {
            if (frame.ACK)
            {
                if (_state.HasFlag(State.SettingsSend))
                {
                    _state &= ~State.SettingsSend;
                }
                else
                {
                    SendGoAway(ErrorCode.PROTOCOL_ERROR);
                    throw new InvalidOperationException();
                }
            }
            else
            {
                ApplySettings(frame);
                _connection.SendFrame(new SettingsFrame { ACK = true });
                _connection.Flush();

                _state &= ~State.Init;
            }

            if ((_state & (State.Init | State.SettingsSend)) == 0)
                _state |= State.Ready;
        }

        private void SendHeaders(uint streamId, IEnumerable<Header> headers)
        {
            using (var ms = new MemoryStream())
            {
                _clientHpack.Encode(ms, headers);
                _connection.SendFrame(new HeadersFrame
                {
                    END_HEADERS = true,
                    END_STREAM = true,
                    Id = streamId,
                    Data = ms.ToArray()
                });
            }
        }

        private void SendGoAway(ErrorCode errorCode)
        {
            _connection.SendFrame(new GoAwayFrame
            {
                ErrorCode = errorCode
            });
            _connection.Flush();
            _connection.Close();
        }

        #endregion

        #region Nested classes

        [Flags]
        private enum State
        {
            Init = 1,
            SettingsSend = 2,
            Ready = 4
        }

        #endregion
    }
}