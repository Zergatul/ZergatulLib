using System;

namespace Zergatul.Network.JsonRpc
{
    public class WebSocketTransport : JsonRpcTransport
    {
        private WebSocket.WebSocketClient _client;

        public WebSocketTransport(WebSocket.WebSocketClient client)
        {
            this._client = client;
        }

        public override void Send(string data)
        {
            _client.SendText(data);
        }

        public override string Receive()
        {
            var message = _client.ReadNextMessage();
            if (message.Binary != null)
                throw new NotImplementedException();
            return message.Text;
        }
    }
}