using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class Headers : Frame
    {
        public override FrameType Type => FrameType.HEADERS;

        #region Flags

        public bool END_STREAM
        {
            get => (Flags & 0x01) != 0;
            set
            {
                if (value)
                    Flags |= 0x01;
                else
                    Flags &= 0xFE;
            }
        }

        public bool END_HEADERS
        {
            get => (Flags & 0x04) != 0;
            set
            {
                if (value)
                    Flags |= 0x04;
                else
                    Flags &= 0xFB;
            }
        }

        public bool PADDED
        {
            get => (Flags & 0x08) != 0;
            set
            {
                if (value)
                    Flags |= 0x08;
                else
                    Flags &= 0xF7;
            }
        }

        public bool PRIORITY
        {
            get => (Flags & 0x20) != 0;
            set
            {
                if (value)
                    Flags |= 0x20;
                else
                    Flags &= 0xDF;
            }
        }

        #endregion

        public bool ExclusiveDependency;
        public uint DependencyStreamIdentifier;
        public int Weight;

        public override void ReadPayload(Stream stream, int length)
        {
            byte[] buffer = new byte[4];

            int padLength = 0;
            if (PADDED)
            {
                StreamHelper.ReadArray(stream, buffer, 1);
                padLength = buffer[0];
            }

            if (PRIORITY)
            {
                StreamHelper.ReadArray(stream, buffer);
                DependencyStreamIdentifier = BitHelper.ToUInt32(buffer, ByteOrder.BigEndian);
                ExclusiveDependency = (DependencyStreamIdentifier & 0x80000000) != 0;
                DependencyStreamIdentifier &= 0x7FFFFFFF;

                StreamHelper.ReadArray(stream, buffer, 1);
                Weight = buffer[0] + 1;
            }
        }

        public override byte[] GetPayload()
        {
            throw new NotImplementedException();
        }
    }
}