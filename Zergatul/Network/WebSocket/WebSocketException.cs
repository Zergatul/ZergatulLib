using System;

namespace Zergatul.Network.WebSocket
{
    public class WebSocketException : Exception
    {
        public WebSocketException(string message)
            : base(message)
        {

        }
    }
}