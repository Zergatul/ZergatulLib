namespace Zergatul.Network.Vpn.Sstp
{
    internal enum MessageType : short
    {
        SSTP_MSG_CALL_CONNECT_REQUEST = 0x0001,
        SSTP_MSG_CALL_CONNECT_ACK = 0x0002,
        SSTP_MSG_CALL_CONNECT_NAK = 0x0003,
        SSTP_MSG_CALL_CONNECTED = 0x0004,
        SSTP_MSG_CALL_ABORT = 0x0005,
        SSTP_MSG_CALL_DISCONNECT = 0x0006,
        SSTP_MSG_CALL_DISCONNECT_ACK = 0x0007,
        SSTP_MSG_ECHO_REQUEST = 0x0008,
        SSTP_MSG_ECHO_RESPONSE = 0x0009
    }
}