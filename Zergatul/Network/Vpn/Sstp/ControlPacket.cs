using System;

namespace Zergatul.Network.Vpn.Sstp
{
    internal class ControlPacket : Packet
    {
        public MessageType MessageType { get; set; }
        public Attribute[] Attributes { get; set; }

        public ControlPacket()
        {
            C = true;
        }
    }
}