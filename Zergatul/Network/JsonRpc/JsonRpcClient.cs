namespace Zergatul.Network.JsonRpc
{
    /// <summary>
    /// Version 2.0
    /// </summary>
    public class JsonRpcClient
    {
        private JsonRpcTransport _transport;
        private int _id;

        public JsonRpcClient(JsonRpcTransport transport)
        {
            this._transport = transport;
            this._id = 1;
        }

        public JsonRpcClient(WebSocket.WebSocketClient client)
            : this(new WebSocketTransport(client))
        {

        }

        public void Call(string method, string parameters)
        {
            string request;
            if (string.IsNullOrEmpty(parameters))
                request = $@"{{""jsonrpc"":""2.0"",""method"":""{method}"",""id"":{_id++}}}";
            else
                request = $@"{{""jsonrpc"":""2.0"",""method"":""{method}"",""params"":{parameters},""id"":{_id++}}}";
            _transport.Send(request);
        }

        public void Notify(string method, string parameters)
        {
            string request;
            if (string.IsNullOrEmpty(parameters))
                request = $@"{{""jsonrpc"":""2.0"",""method"":""{method}""}}";
            else
                request = $@"{{""jsonrpc"":""2.0"",""method"":""{method}"",""params"":{parameters}}}";
            _transport.Send(request);
        }

        public string ReadNextMessage()
        {
            return _transport.Receive();
        }
    }
}