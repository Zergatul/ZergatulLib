using System.Collections.Generic;

namespace Zergatul.Cryptocurrency.P2P
{
    public class VerAckMessage : Message
    {
        public override string Command => "verack";

        protected override void SerializePayload(List<byte> buffer)
        {
            
        }

        public override void DeserializePayload(byte[] buffer)
        {
            
        }
    }
}