using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.FileFormat.Qip
{
    public class ChatMessage
    {
        public byte[] MsgRaw { get; }
        public string Message { get; }

        static ChatMessage()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        internal ChatMessage(BinaryReader reader)
        {
            int sing = reader.ReadWord(); // 1
            uint cdSizeBlock = reader.ReadDWord();

            int type1 = reader.ReadWord();
            int size1 = reader.ReadWord();
            uint idmsg = reader.ReadDWord();

            int type2 = reader.ReadWord();
            int size2 = reader.ReadWord();
            uint time = reader.ReadDWord();

            int type3 = reader.ReadWord();
            int size3 = reader.ReadWord();
            int unknown = reader.ReadByte();

            int type4 = reader.ReadWord();
            int size4 = reader.ReadWord();
            uint msgSize = reader.ReadDWord();
            byte[] msg = reader.ReadBytes((int)msgSize);

            //for (int i = 0; i < msg.Length; i++)
            //    msg[i] = (byte)(0xFF - msg[i] - i);

            //string msg1 = Encoding.GetEncoding(20866).GetString(msg);
            //string msg2 = Encoding.GetEncoding(1251).GetString(msg);
            //string msg3 = Encoding.GetEncoding(866).GetString(msg);
            //string msg4 = Encoding.GetEncoding(28595).GetString(msg);
            // 

            MsgRaw = msg;

            //Message = Encoding.GetEncoding(1251).GetString(msg);
        }
    }
}