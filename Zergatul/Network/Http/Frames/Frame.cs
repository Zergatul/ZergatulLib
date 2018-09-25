using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    internal abstract class Frame
    {
        public byte Flags;
        public uint Id;

        public abstract void Read(Stream stream, int length);

        public static Frame Read(Stream stream)
        {
            byte[] header = new byte[9];
            StreamHelper.ReadArray(stream, header);
            if (header[0] != 0)
                throw new InvalidOperationException("Frame too big");
            int length = (header[1] << 8) | header[2];
            var type = (FrameType)header[3];
            byte flags = header[4];
            uint id = BitHelper.ToUInt32(header, 5, ByteOrder.BigEndian);
            id &= 0x7FFFFFFF;

            Frame frame;
            switch (type)
            {
                case FrameType.DATA: frame = new Data(); break;
                default:
                    throw new NotImplementedException();
            }

            frame.Flags = header[4];
            frame.Id = id;
            frame.Read(stream, length);

            return frame;
        }
    }
}