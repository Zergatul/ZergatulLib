using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Zergatul.IO;
using Zergatul.Network.Http;
using Zergatul.Security;

namespace Zergatul.Network.WebSocket
{
    public class WebSocketClient
    {
        public ConnectionState State { get; private set; }

        private Uri _uri;
        private TcpClient _client;
        private Stream _stream;
        private GenericMessageReadStream _readStream;
        private byte[] _nonce;
        private byte[] _buffer = new byte[1024 * 1024];
        private SecureRandom _random;

        public WebSocketClient(string uri)
            : this(new Uri(uri))
        {

        }

        public WebSocketClient(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException();
            if (uri.Scheme != "ws" && uri.Scheme != "wss")
                throw new ArgumentException();

            this._uri = uri;
            this.State = ConnectionState.Uninitialized;

            this._nonce = new byte[16];
            this._random = Provider.GetSecureRandomInstance(SecureRandoms.Default);
        }

        public void Open()
        {
            this.State = ConnectionState.Connecting;

            _client = TcpConnector.GetTcpClient(_uri.Host, _uri.Port);

            switch (_uri.Scheme)
            {
                case "ws":
                    _stream = _client.GetStream();
                    break;
                case "wss":
                    var tls = new Tls.TlsStream(_client.GetStream());
                    tls.AuthenticateAsClient(_uri.Host);
                    _stream = tls;
                    break;
            }

            _readStream = new GenericMessageReadStream(_stream);

            _random.GetNextBytes(_nonce);
            string key = Convert.ToBase64String(_nonce);

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = _uri.PathAndQuery;
            request.Headers[HttpRequestHeader.Host] = _uri.Host + (_uri.IsDefaultPort ? "" : ":" + _uri.Port);
            request.Headers[HttpRequestHeader.Connection] = "Upgrade";
            request.Headers[HttpRequestHeader.Upgrade] = "websocket";
            request.Headers[HttpRequestHeader.SecWebSocketKey] = key;
            request.Headers[HttpRequestHeader.SecWebSocketVersion] = "13";

            byte[] buffer = request.ToBytes();
            _stream.Write(buffer, 0, buffer.Length);

            var response = new HttpResponseMessage();
            response.Read(_readStream, _buffer);
            if (response.StatusCode != 101)
                throw new InvalidOperationException();
            if (!string.Equals(response[HttpResponseHeader.Connection], "Upgrade", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();
            if (!string.Equals(response[HttpResponseHeader.Upgrade], "websocket", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();

            var md = Provider.GetMessageDigestInstance(MessageDigests.SHA1);
            byte[] digest = md.Digest(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            if (response[HttpResponseHeader.SecWebSocketAccept] != Convert.ToBase64String(digest))
                throw new InvalidOperationException();

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
        }

        public Message ReadNextMessage()
        {
            while (true)
            {
                Frame frame = new Frame();
                frame.Read(_readStream, _buffer);

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
            _client.Close();
        }

        private void SendFrame(Frame frame)
        {
            if (frame.IsMasked && frame.MaskingKey == null)
            {
                frame.MaskingKey = new byte[4];
                _random.GetNextBytes(frame.MaskingKey);
            }
            frame.PayloadLength = (ulong)(frame.ApplicationData?.Length ?? 0);
            if (frame.IsMasked && frame.ApplicationData != null)
                for (int i = 0; i < frame.ApplicationData.Length; i++)
                    frame.ApplicationData[i] ^= frame.MaskingKey[i & 0x03];

            byte[] data = frame.ToBytes();
            _stream.Write(data, 0, data.Length);
        }
    }
}