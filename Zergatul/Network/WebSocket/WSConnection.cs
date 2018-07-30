﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Zergatul.Network.Http;
using Zergatul.Security;

namespace Zergatul.Network.WebSocket
{
    public class WSConnection
    {
        public WSConnectionState State { get; private set; }

        private Uri _uri;
        private TcpClient _client;
        private Stream _stream;
        private byte[] _nonce;
        private byte[] _buffer = new byte[65536];
        private int _bufPosition = 0;
        private SecureRandom _random;

        public WSConnection(string uri)
            : this(new Uri(uri))
        {

        }

        public WSConnection(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException();
            if (uri.Scheme != "ws" && uri.Scheme != "wss")
                throw new ArgumentException();

            this._uri = uri;
            this.State = WSConnectionState.Uninitialized;

            this._nonce = new byte[16];
            this._random = Provider.GetSecureRandomInstance(SecureRandoms.Default);
        }

        public void Open()
        {
            this.State = WSConnectionState.Connecting;

            _client = new TcpClient();
            _client.Connect(_uri.Host, _uri.Port);

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

            var response = ReadResponse();
            if (response.StatusCode != 101)
                throw new InvalidOperationException();
            if (!string.Equals(response.Headers[HttpResponseHeader.Connection], "Upgrade", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();
            if (!string.Equals(response.Headers[HttpResponseHeader.Upgrade], "websocket", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException();

            var md = Provider.GetMessageDigestInstance(MessageDigests.SHA1);
            byte[] digest = md.Digest(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            if (response.Headers[HttpResponseHeader.SecWebSocketAccept] != Convert.ToBase64String(digest))
                throw new InvalidOperationException();

            State = WSConnectionState.Connected;
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
                Frame frame;
                int messageLength = 0;
                while ((frame = Frame.Parse(_buffer, 0, _bufPosition, ref messageLength)) == null)
                    ReadBuffer();

                ShiftBuffer(messageLength);

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

        private HttpResponseMessage ReadResponse()
        {
            int messageLength = 0;
            HttpResponseMessage response;
            while ((response = HttpResponseMessage.Parse(_buffer, _bufPosition, ref messageLength)) == null)
                ReadBuffer();

            ShiftBuffer(messageLength);

            return response;
        }

        private void ReadBuffer()
        {
            if (_buffer.Length - _bufPosition == 0)
                throw new InvalidOperationException("Insufficient buffer");

            int read = _stream.Read(_buffer, _bufPosition, _buffer.Length - _bufPosition);
            if (read == 0)
                throw new InvalidOperationException();
            _bufPosition += read;
        }

        private void ShiftBuffer(int count)
        {
            if (count > _bufPosition)
                throw new InvalidOperationException();
            if (count == _bufPosition)
            {
                _bufPosition = 0;
            }
            else
            {
                for (int i = 0; i < _bufPosition - count; i++)
                    _buffer[i] = _buffer[i + count];
                _bufPosition -= count;
            }
        }
    }
}