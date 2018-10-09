using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class WindowUpdateFrame : Frame
    {
        public override FrameType Type => FrameType.WINDOW_UPDATE;

        public uint Increment;

        public override void ReadPayload(Stream stream, int length)
        {
            if (length != 4)
            {
                GoAwayWith(ErrorCode.FRAME_SIZE_ERROR);
                return;
            }

            byte[] buffer = new byte[4];
            StreamHelper.ReadArray(stream, buffer);
            Increment = BitHelper.ToUInt32(buffer, ByteOrder.BigEndian) & 0x7FFFFFFFU;
            if (Increment == 0)
            {
                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                return;
            }
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}