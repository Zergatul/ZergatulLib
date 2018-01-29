using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Zergatul.Network;

namespace Zergatul.Cryptocurrency.P2P
{
    public class Peer
    {
        private ProtocolSpecification _spec;
        private TcpClient _client;
        private NetworkStream _stream;
        private PeerStreamReader _reader;

        private Dictionary<string, byte[]> _blocks = new Dictionary<string, byte[]>();
        public ReadOnlyDictionary<string, byte[]> Blocks => new ReadOnlyDictionary<string, byte[]>(_blocks);

        public void Connect(IPAddress addr, ProtocolSpecification spec)
        {
            _spec = spec;

            _client = new TcpClient();
            _client.ReceiveTimeout = _client.SendTimeout = 7000;
            _client.Connect(addr, spec.Port);
            _stream = _client.GetStream();

            var buffer = new List<byte>();

            SendMessage(new VersionMessage
            {
                Version = 170002,
                Services = 1,
                Timestamp = (ulong)(DateTime.UtcNow - Constants.UnixTimeStart).TotalSeconds,
                AddrRecv = new ShortNetworkAddress
                {
                    Services = 1,
                    IP = addr,
                    Port = spec.Port
                },
                AddrFrom = new ShortNetworkAddress
                {
                    Services = 1,
                    IP = IPAddress.Parse("176.36.233.101"),
                    Port = spec.Port
                },
                Nonce = 0x1234567890,
                UserAgent = "/MagicBean:1.0.81/",
                StartHeight = 0,
                Relay = false
            });

            _reader = new PeerStreamReader(_stream, spec);
            _reader.OnMessage += OnMessage;
            _reader.Start(TimeSpan.FromSeconds(1));
        }

        public void RequestBlock(string id)
        {
            SendMessage(new GetDataMessage
            {
                Inventory = new[]
                {
                    new InvVect
                    {
                        Type = VectorType.MSG_BLOCK,
                        Hash = BitHelper.HexToBytes(id)
                    }
                }
            });

            _reader.Start(TimeSpan.FromSeconds(10));
        }

        public void Close()
        {
            _client.Close();
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message is VersionMessage)
            {
                SendMessage(new VerAckMessage());
            }

            if (e.Message is PingMessage)
            {
                SendMessage(new PongMessage
                {
                    Nonce = ((PingMessage)e.Message).Nonce
                });
            }

            if (e.Message is GetHeadersMessage)
            {
                SendMessage(new HeadersMessage());
            }

            if (e.Message is BlockMessage)
            {
                var block = e.Message as BlockMessage;
                _blocks.Add("block1", block.RawBlock);
            }
        }

        private void SendMessage(Message message)
        {
            List<byte> buffer = new List<byte>();
            message.Serialize(buffer, _spec);
            _stream.Write(buffer.ToArray(), 0, buffer.Count);
        }
    }
}