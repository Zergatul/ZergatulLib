using System;
using System.IO;

namespace Zergatul.Network.Http.Frames
{
    class Data : Frame
    {
        public override FrameType Type => FrameType.DATA;

        public bool END_STREAM => (Flags & 0x01) != 0;
        public bool PADDED => (Flags & 0x08) != 0;

        public override void ReadPayload(Stream stream, int length)
        {
            if (PADDED)
                throw new NotImplementedException();
            throw new NotImplementedException();
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}