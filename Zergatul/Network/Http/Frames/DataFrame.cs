using System;
using System.IO;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class DataFrame : Frame
    {
        public override FrameType Type => FrameType.DATA;

        public bool END_STREAM => (Flags & 0x01) != 0;
        public bool PADDED => (Flags & 0x08) != 0;

        public byte[] Data;

        public override void ReadPayload(Stream stream, int length)
        {
            if (PADDED)
                throw new NotImplementedException();

            Data = new byte[length];
            StreamHelper.ReadArray(stream, Data);
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}