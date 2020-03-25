﻿namespace Zergatul.Network.Http.Frames
{
    public enum FrameErrorCode : uint
    {
        NO_ERROR = 0x00,
        PROTOCOL_ERROR = 0x01,
        INTERNAL_ERROR = 0x02,
        FLOW_CONTROL_ERROR = 0x03,
        SETTINGS_TIMEOUT = 0x04,
        STREAM_CLOSED = 0x05,
        FRAME_SIZE_ERROR = 0x06,
        REFUSED_STREAM = 0x07,
        CANCEL = 0x08,
        COMPRESSION_ERROR = 0x09,
        CONNECT_ERROR = 0x0A,
        ENHANCE_YOUR_CALM = 0x0B,
        INADEQUATE_SECURITY = 0x0C,
        HTTP_1_1_REQUIRED = 0x0D
    }
}