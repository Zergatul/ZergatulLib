using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zergatul.IO;

namespace Zergatul.Network.Http.Frames
{
    class HeadersFrame : Frame
    {
        public override FrameType Type => FrameType.HEADERS;

        #region Flags

        /// <summary>
        /// When set, indicates that the header block is the last that the endpoint will send for the identified stream
        /// </summary>
        public bool END_STREAM
        {
            get => (Flags & 0x01) != 0;
            set
            {
                if (value)
                    Flags |= 0x01;
                else
                    Flags &= 0xFF ^ 0x01;
            }
        }

        /// <summary>
        /// When set, indicates that this frame contains an entire header block and is not followed by any CONTINUATION frames
        /// </summary>
        public bool END_HEADERS
        {
            get => (Flags & 0x04) != 0;
            set
            {
                if (value)
                    Flags |= 0x04;
                else
                    Flags &= 0xFF ^ 0x04;
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
                    Flags &= 0xFF ^ 0x08;
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
                    Flags &= 0xFF ^ 0x20;
            }
        }

        #endregion

        public bool ExclusiveDependency;
        public uint DependencyStreamIdentifier;
        public int Weight;
        public byte[] Data;

        public override void ReadPayload(Stream stream, int length)
        {
            if (Id == 0)
            {
                GoAwayWith(ErrorCode.PROTOCOL_ERROR);
                return;
            }

            byte[] buffer = new byte[4];

            int padLength = 0;
            if (PADDED)
            {
                StreamHelper.ReadArray(stream, buffer, 1);
                padLength = buffer[0];
                length--;
            }

            if (PRIORITY)
            {
                StreamHelper.ReadArray(stream, buffer);
                DependencyStreamIdentifier = BitHelper.ToUInt32(buffer, ByteOrder.BigEndian);
                ExclusiveDependency = (DependencyStreamIdentifier & 0x80000000) != 0;
                DependencyStreamIdentifier &= 0x7FFFFFFF;
                length -= 4;

                StreamHelper.ReadArray(stream, buffer, 1);
                Weight = buffer[0] + 1;
                length--;
            }

            Data = new byte[length];
            StreamHelper.ReadArray(stream, Data);
        }

        public override byte[] GetPayload()
        {
            return Data;
        }
    }
}