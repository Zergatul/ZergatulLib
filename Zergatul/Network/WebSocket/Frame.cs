using System;

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

        public static Frame Parse(byte[] buffer, int index, int length, ref int messageLength)
        {
            if (length < 2)
                return null;

            int initialIndex = index;

            bool fin = (buffer[index] & 0x80) != 0;
            Opcode opcode = (Opcode)(buffer[index] & 0x0F);
            index++;
            length--;
            bool isMasked = (buffer[index] & 0x80) != 0;
            int payload = buffer[index] & 0x7F;
            index++;
            length--;
            if (payload == 126)
            {
                if (length < 2)
                    return null;
                payload = BitHelper.ToUInt16(buffer, index, ByteOrder.BigEndian);
                index += 2;
                length -= 2;
            }
            if (payload == 127)
                throw new NotImplementedException();

            byte[] mask = null;
            if (isMasked)
            {
                if (length < 4)
                    return null;
                mask = ByteArray.SubArray(buffer, index, 4);
                index += 4;
                length -= 4;
            }

            if (length < payload)
                return null;

            messageLength = index - initialIndex + payload;
            return new Frame
            {
                Fin = fin,
                Opcode = opcode,
                IsMasked = isMasked,
                PayloadLength = (ulong)payload,
                MaskingKey = mask,
                ApplicationData = ByteArray.SubArray(buffer, index, payload)
            };
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