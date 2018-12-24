namespace Zergatul.Network.Vpn.Sstp
{
    internal enum AttributeId : byte
    {
        SSTP_ATTRIB_ENCAPSULATED_PROTOCOL_ID = 0x01,
        SSTP_ATTRIB_STATUS_INFO = 0x02,
        SSTP_ATTRIB_CRYPTO_BINDING = 0x03,
        SSTP_ATTRIB_CRYPTO_BINDING_REQ = 0x04
    }
}