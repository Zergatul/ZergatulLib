using System;
using System.IO;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class GoAway : Frame
    {
        public override FrameType Type => FrameType.GOAWAY;

        public uint LastStreamId;
        public ErrorCode ErrorCode;

        public override void ReadPayload(Stream stream, int length)
        {
            if (Id != 0)
            {
                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                return;
            }

            byte[] buffer = new byte[8];
            StreamHelper.ReadArray(stream, buffer);

            LastStreamId = BitHelper.ToUInt32(buffer, 0, ByteOrder.BigEndian) & 0x7FFFFFFF;
            ErrorCode = (ErrorCode)BitHelper.ToUInt32(buffer, 4, ByteOrder.BigEndian);
        }

        public override byte[] GetPayload()
        {
            byte[] payload = new byte[8];
            BitHelper.GetBytes((uint)ErrorCode, ByteOrder.BigEndian, payload, 4);
            return payload;
        }
    }
}