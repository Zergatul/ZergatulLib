using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http.Frames
{
    class Headers : Frame
    {
        public override FrameType Type => FrameType.HEADERS;

        public override void ReadPayload(Stream stream, int length)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}