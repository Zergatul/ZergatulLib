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

        public void Read(Stream stream)
        {
            int @byte = ReadNextByte(stream);
            Fin = (@byte & 0x80) != 0;
            Opcode = (Opcode)(@byte & 0x0F);

            @byte = ReadNextByte(stream);
            IsMasked = (@byte & 0x80) != 0;
            int payload = @byte & 0x7F;

            if (payload == 126)
            {
                payload = (ReadNextByte(stream) << 8) | ReadNextByte(stream);
            }
            if (payload == 127)
            {
                byte[] data = new byte[8];
                StreamHelper.ReadArray(stream, data);
                PayloadLength = BitHelper.ToUInt64(data, 0, ByteOrder.BigEndian);

                payload = checked((int)PayloadLength);
            }

            if (IsMasked)
            {
                MaskingKey = new byte[4];
                StreamHelper.ReadArray(stream, MaskingKey);
            }

            ApplicationData = new byte[payload];
            StreamHelper.ReadArray(stream, ApplicationData);

            if (IsMasked)
            {
                for (int i = 0; i < ApplicationData.Length; i++)
                    ApplicationData[i] ^= MaskingKey[i & 0x03];
            }
        }

        public void WriteTo(Stream stream)
        {
            int length = 2;
            if (PayloadLength > 125)
                length += 2;
            if (PayloadLength >= 0x10000)
                length += 8;
            if (IsMasked)
                length += 4;

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

            stream.Write(data, 0, data.Length);

            if (ApplicationData != null)
                stream.Write(ApplicationData, 0, ApplicationData.Length);
        }

        #region Private methods

        private int ReadNextByte(Stream stream)
        {
            int result = stream.ReadByte();
            if (result == -1)
                throw new WebSocketException("Cannot read frame. Unexpected end of stream.");
            return result;
        }

        #endregion
    }
}