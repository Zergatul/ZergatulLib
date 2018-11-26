using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Cryptocurrency.P2P
{
    class RejectMessage : Message
    {
        public override string Command => "reject";

        public string Message;
        public CCode CCode;
        public string Reason;
        public byte[] Data;

        protected override void SerializePayload(List<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override void DeserializePayload(byte[] buffer)
        {
            int index = 0;

            int length = VarLengthInt.ParseInt32(buffer, ref index);
            Message = Encoding.ASCII.GetString(buffer, index, length);
            index += length;

            CCode = (CCode)buffer[index++];

            length = VarLengthInt.ParseInt32(buffer, ref index);
            Reason = Encoding.ASCII.GetString(buffer, index, length);
            index += length;

            if (index != buffer.Length)
                Data = ByteArray.SubArray(buffer, index, buffer.Length - index);
        }
    }
}