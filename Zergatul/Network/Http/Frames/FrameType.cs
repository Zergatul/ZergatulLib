namespace Zergatul.Network.Http.Frames
{
    public enum FrameType
    {
        DATA = 0x00,
        HEADERS = 0x01,
        PRIORITY = 0x02,
        RST_STREAM = 0x03,
        SETTINGS = 0x04,
        PUSH_PROMISE = 0x05,
        PING = 0x06,
        GOAWAY = 0x07,
        WINDOW_UPDATE = 0x08,
        CONTINUATION = 0x09
    }
}