namespace Zergatul.Network.Http.Frames
{
    public enum SettingParameter : ushort
    {
        SETTINGS_HEADER_TABLE_SIZE = 0x01,
        SETTINGS_ENABLE_PUSH = 0x02,
        SETTINGS_MAX_CONCURRENT_STREAMS = 0x03,
        SETTINGS_INITIAL_WINDOW_SIZE = 0x04,
        SETTINGS_MAX_FRAME_SIZE = 0x05,
        SETTINGS_MAX_HEADER_LIST_SIZE = 0x06
    }
}