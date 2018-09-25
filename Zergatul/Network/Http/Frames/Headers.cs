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
        public override void Read(Stream stream, int length)
        {
            throw new NotImplementedException();
        }
    }
}