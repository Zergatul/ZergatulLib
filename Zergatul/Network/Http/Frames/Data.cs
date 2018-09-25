using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Http.Frames
{
    class Data : Frame
    {
        private bool END_STREAM => (Flags & 0x01) != 0;
        private bool PADDED => (Flags & 0x08) != 0;

        public override void Read(Stream stream, int length)
        {
            if (PADDED)
                throw new NotImplementedException();
            throw new NotImplementedException();
        }
    }
}