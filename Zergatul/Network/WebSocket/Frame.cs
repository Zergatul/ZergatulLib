using System;
using System.IO;
using Zergatul.IO;

namespace Zergatul.Network.WebSocket
{
    class Frame
    {
        public bool Fin;
        public Opcode Opcode;
        public bool IsMasked;
        public ulong PayloadLength;
        public byte[] MaskingKey;
        public byte[] ApplicationData;

        private enum ReadState
        {
            Byte1,
            Byte2,
            Payload16,
            Payload64,
            Mask,
        }

        public void Read(GenericMessageReadStream stream, byte[] buffer)
        {
            int length = 0;
            int index = 0;

            while (length - index < 2)
                stream.IncrementalRead(buffer, ref length);

            Fin = (buffer[index] & 0x80) != 0;
            Opcode = (Opcode)(buffer[index] & 0x0F);
            index++;
            IsMasked = (buffer[index] & 0x80) != 0;
            int payload = buffer[index] & 0x7F;
            index++;
            if (payload == 126)
            {
                while (length - index < 2)
                    stream.IncrementalRead(buffer, ref length);
                payload = BitHelper.ToUInt16(buffer, index, ByteOrder.BigEndian);
                index += 2;
            }
            if (payload == 127)
            {
                while (length - index < 8)
                    stream.IncrementalRead(buffer, ref length);
                PayloadLength = BitHelper.ToUInt64(buffer, index, ByteOrder.BigEndian);
                payload = checked((int)PayloadLength);
                index += 8;
            }

            if (IsMasked)
            {
                while (length - index < 4)
                    stream.IncrementalRead(buffer, ref length);
                MaskingKey = ByteArray.SubArray(buffer, index, 4);
                index += 4;
            }

            while (length - index < payload)
                stream.IncrementalRead(buffer, ref length);

            ApplicationData = ByteArray.SubArray(buffer, index, payload);
            index += payload;

            if (index < length)
                stream.SendBackBuffer(buffer, index, length - index);
        }

        public byte[] ToBytes()
        {
            int length = 2;
            if (PayloadLength > 125)
                length += 2;
            if (PayloadLength >= 0x10000)
                length += 8;
            if (IsMasked)
                length += 4;
            length += ApplicationData?.Length ?? 0;

            byte[] data = new byte[length];

            int index = 2;
            data[0] = (byte)((Fin ? 0x80 : 0x00) | (int)Opcode);
            if (PayloadLength <= 125)
                data[1] = (byte)PayloadLength;
            else if (PayloadLength < 0x10000)
            {
                data[1] = 126;
                BitHelper.GetBytes((ushort)PayloadLength, ByteOrder.BigEndian, data, index);
                index += 2;
            }
            else
            {
                data[1] = 127;
                BitHelper.GetBytes(PayloadLength, ByteOrder.BigEndian, data, index);
                index += 8;
            }
            if (IsMasked)
            {
                data[1] |= 0x80;
                Array.Copy(MaskingKey, 0, data, index, 4);
                index += 4;
            }
            if (ApplicationData != null)
                Array.Copy(ApplicationData, 0, data, index, ApplicationData.Length);

            return data;
        }
    }
}