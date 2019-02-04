namespace Zergatul.Network.Tls
{
    public enum ProtocolVersion : ushort
    {
        Ssl30 = 0x0300,
        Tls10 = 0x0301,
        Tls11 = 0x0302,
        Tls12 = 0x0303,
        Tls13 = 0x0304
    }
}