using System;
using System.IO;
using System.Text;
using Zergatul.Network.Http;
using Zergatul.Security;

namespace Zergatul.Network.WebSocket
{
    public class WebSocketClient
    {
        public ConnectionState State { get; private set; }

        public string this[string header]
        {
            get => _request.GetHeader(header);
            set => _request.SetHeader(header, value);
        }

        private Uri _uri;
        private NetworkProvider _provider;
        private Stream _stream;
        private HttpRequest _request;
        private Random _random;

        public WebSocketClient(string uri)
            : this(new Uri(uri))
        {

        }

        public WebSocketClient(Uri uri)
            : this(uri, new DefaultNetworkProvider())
        {

        }

        public WebSocketClient(Uri uri, NetworkProvider provider)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (uri.Scheme != "ws" && uri.Scheme != "wss")
                throw new ArgumentException();

            _uri = uri;
            _provider = provider;
            _random = new Random();
            State = ConnectionState.Uninitialized;

            InitHttpRequestMessage();
        }

        public void Open()
        {
            this.State = ConnectionState.Connecting;

            _stream = _provider.GetTcpStream(_uri.Host, _uri.Port, null);
            switch (_uri.Scheme)
            {
                case "ws":
                    break;

                case "wss":
                    var tls = new Tls.TlsStream(_stream);
                    tls.AuthenticateAsClient(_uri.Host);
                    _stream = tls;
                    break;
            }

            byte[] nonce = new byte[16];
            using (var random = SecureRandom.GetInstance(SecureRandoms.Default))
                random.GetNextBytes(nonce);

            string key = Convert.ToBase64String(nonce);
            _request.SetHeader(HttpRequestHeaders.SecWebSocketKey, key);

            _request.WriteTo(_stream);
            _stream.Flush();

            var response = new HttpResponse();
            response.ReadFrom(_stream);

            if (response.Status != HttpStatusCode.SwitchingProtocols)
                ThrowWebSocketException("Invalid status code", response);

            if (!string.Equals(response[HttpResponseHeaders.Connection], "Upgrade", StringComparison.InvariantCultureIgnoreCase))
                ThrowWebSocketException("Invalid Connection header", response);

            if (!string.Equals(response[HttpResponseHeaders.Upgrade], "websocket", StringComparison.InvariantCultureIgnoreCase))
                ThrowWebSocketException("Invalid Upgrade header", response);

            byte[] digest;
            using (var md = MessageDigest.GetInstance(MessageDigests.SHA1))
                digest = md.Digest(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));

            if (response[HttpResponseHeaders.SecWebSocketAccept] != Convert.ToBase64String(digest))
                ThrowWebSocketException("Invalid Sec-WebSocket-Accept header", response);

            State = ConnectionState.Connected;
        }

        public void SendText(string text)
        {
            SendFrame(new Frame
            {
                Fin = true,
                IsMasked = true,
                Opcode = Opcode.Text,
                ApplicationData = Encoding.UTF8.GetBytes(text)
            });
            _stream.Flush();
        }

        public void SendBinary(byte[] data)
        {
            SendFrame(new Frame
            {
                Fin = true,
                IsMasked = true,
                Opcode = Opcode.Binary,
                ApplicationData = data
            });
            _stream.Flush();
        }

        public Message ReadNextMessage()
        {
            while (true)
            {
                Frame frame = new Frame();
                frame.Read(_stream);

                switch (frame.Opcode)
                {
                    case Opcode.Text:
                        return new Message
                        {
                            Text = Encoding.UTF8.GetString(frame.ApplicationData)
                        };
                    case Opcode.Binary:
                        return new Message
                        {
                            Binary = frame.ApplicationData
                        };
                    case Opcode.Ping:
                        SendFrame(new Frame
                        {
                            Fin = true,
                            Opcode = Opcode.Pong,
                            IsMasked = true
                        });
                        _stream.Flush();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Close()
        {
            SendFrame(new Frame
            {
                Fin = true,
                Opcode = Opcode.Close,
                IsMasked = true
            });
            _stream.Close();
        }

        #region Private methods

        private void InitHttpRequestMessage()
        {
            _request = new HttpRequest(_uri);
            _request.Method = HttpMethods.Get;
            _request.RemoveHeader(HttpRequestHeaders.AcceptEncoding);
            _request.SetHeader(HttpRequestHeaders.Connection, "Upgrade");
            _request.SetHeader(HttpRequestHeaders.Upgrade, "websocket");
            _request.SetHeader(HttpRequestHeaders.SecWebSocketVersion, "13");
        }

        private void ThrowWebSocketException(string message, HttpResponse response)
        {
            var ex = new WebSocketException(message);
            ex.Data.Add("Response", response);
            throw ex;
        }

        private void SendFrame(Frame frame)
        {
            if (frame.IsMasked && frame.MaskingKey == null)
            {
                frame.MaskingKey = new byte[4];
                _random.NextBytes(frame.MaskingKey);
            }
            frame.PayloadLength = (ulong)(frame.ApplicationData?.Length ?? 0);
            if (frame.IsMasked && frame.ApplicationData != null)
                for (int i = 0; i < frame.ApplicationData.Length; i++)
                    frame.ApplicationData[i] ^= frame.MaskingKey[i & 0x03];

            frame.WriteTo(_stream);
        }

        #endregion
    }
}