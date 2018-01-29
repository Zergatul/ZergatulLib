using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Zergatul.Cryptocurrency.Bitcoin;

namespace Zergatul.Cryptocurrency.P2P
{
    public class PeerStreamReader
    {
        private NetworkStream _stream;
        private ProtocolSpecification _spec;
        private List<byte> _buffer;
        private byte[] _readBuffer;
        private int _length;

        public event EventHandler<MessageEventArgs> OnMessage;

        public PeerStreamReader(NetworkStream stream, ProtocolSpecification spec)
        {
            this._stream = stream;
            this._spec = spec;
            this._buffer = new List<byte>();
            this._readBuffer = new byte[1024];
        }

        public void Start(TimeSpan timeout)
        {
            DateTime start = DateTime.Now;

            while (true)
            {
                if (IsFullMessage())
                {
                    var ea = new MessageEventArgs(ReadMessage());
                    OnMessage?.Invoke(this, ea);
                    if (ea.Stop)
                        return;

                    start = DateTime.Now;
                }

                if (_stream.DataAvailable)
                {
                    int read = _stream.Read(_readBuffer, 0, _readBuffer.Length);
                    _buffer.AddRange(_readBuffer.Take(read));
                }
                else
                    Thread.Sleep(50);

                if (DateTime.Now - start > timeout)
                    return;
            }
        }

        private bool IsFullMessage()
        {
            if (_buffer.Count < 24)
                return false;

            _length = BitHelper.ToInt32(_buffer.Skip(16).Take(4).ToArray(), 0, ByteOrder.LittleEndian);
            return _buffer.Count >= 24 + _length;
        }

        private Message ReadMessage()
        {
            byte[] message = _buffer.Take(24 + _length).ToArray();
            _buffer.RemoveRange(0, 24 + _length);

            uint magic = BitHelper.ToUInt32(message, 0, ByteOrder.LittleEndian);
            if (magic != _spec.Magic)
                throw new InvalidOperationException();

            var dsha256 = new DoubleSHA256();
            dsha256.Update(message, 24, _length);
            byte[] hash = dsha256.ComputeHash();

            if (!hash.Take(4).SequenceEqual(message.Skip(20).Take(4)))
                throw new InvalidOperationException();

            string cmd = Encoding.ASCII.GetString(message, 4, 12).TrimEnd('\0');

            Message result;
            switch (cmd)
            {
                case "version":
                    result = new VersionMessage();
                    break;
                case "verack":
                    result = new VerAckMessage();
                    break;
                case "inv":
                    result = new InvMessage();
                    break;
                case "getheaders":
                    result = new GetHeadersMessage();
                    break;
                case "block":
                    result = new BlockMessage();
                    break;
                case "addr":
                    result = new AddrMessage();
                    break;
                case "ping":
                    result = new PingMessage();
                    break;
                case "pong":
                    result = new PongMessage();
                    break;
                default:
                    throw new NotImplementedException();
            }

            result.DeserializePayload(ByteArray.SubArray(message, 24, _length));

            return result;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public Message Message { get; private set; }
        public bool Stop { get; set; }

        public MessageEventArgs(Message message)
        {
            this.Message = message;
        }
    }
}