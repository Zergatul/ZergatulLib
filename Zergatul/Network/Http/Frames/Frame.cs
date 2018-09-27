using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    public abstract class Frame
    {
        public abstract FrameType Type { get; }
        public byte Flags;
        public uint Id;

        public bool ShouldSendGoAway;
        public ErrorCode GoAwayErrorCode;

        public abstract void ReadPayload(Stream stream, int length);
        public abstract byte[] GetPayload();

        public void Write(Stream stream)
        {
            if ((Id & 0x80000000) != 0)
                throw new InvalidOperationException("High bit should be 0");
            byte[] payload = GetPayload();
            int length = payload?.Length ?? 0;
            if (length > 0x4000)
                throw new InvalidOperationException("Frame too big");

            byte[] header = new byte[9];
            header[1] = (byte)(length >> 8);
            header[2] = (byte)(length & 0xFF);
            header[3] = (byte)Type;
            BitHelper.GetBytes(Id, ByteOrder.BigEndian, header, 5);

            stream.Write(header, 0, 9);
            if (length > 0)
                stream.Write(payload, 0, length);
        }

        protected void GoAwayWith(ErrorCode errorCode)
        {
            ShouldSendGoAway = true;
            GoAwayErrorCode = errorCode;
        }

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
            frame.ReadPayload(stream, length);

            return frame;
        }
    }
}