using System;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.FileFormat.Qip
{
    public class ChatHistory
    {
        public string Uin { get; private set; }
        public string Nick { get; private set; }
        public IReadOnlyList<ChatMessage> Messages { get; private set; }

        public ChatHistory(string filename)
        {
            using (var stream = File.OpenRead(filename))
                ReadFromStream(stream);
        }

        public ChatHistory(Stream stream)
        {
            ReadFromStream(stream);
        }

        private void ReadFromStream(Stream stream)
        {
            var reader = new BinaryReader(stream);

            string sign = reader.ReadAnsiString(3); // QHF
            int version = reader.ReadByte(); // 3
            uint cdSize = reader.ReadDWord(); // cdSize history
            byte[] unknown1 = reader.ReadBytes(10);
            byte[] unknown2 = reader.ReadBytes(16);
            uint msgCount1 = reader.ReadDWord();
            uint msgCount2 = reader.ReadDWord();
            int reserved = reader.ReadWord();
            int uinLength = reader.ReadWord();
            Uin = reader.ReadUtf8String(uinLength);
            int nickLength = reader.ReadWord();
            Nick = reader.ReadUtf8String(nickLength);

            var messages = new ChatMessage[msgCount1];
            for (uint i = 0; i < msgCount1; i++)
                messages[i] = new ChatMessage(reader);

            Messages = messages;
        }
    }
}