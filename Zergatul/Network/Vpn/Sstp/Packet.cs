using System;

namespace Zergatul.Network.Vpn.Sstp
{
    internal abstract class Packet
    {
        public byte Version { get; set; }
        public bool C { get; protected set; }
        public int Length { get; protected set; }
        public byte[] Data { get; protected set; }
    }
}