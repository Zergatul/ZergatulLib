namespace Zergatul.Network.JsonRpc
{
    public abstract class JsonRpcTransport
    {
        public abstract void Send(string data);
        public abstract string Receive();
    }
}