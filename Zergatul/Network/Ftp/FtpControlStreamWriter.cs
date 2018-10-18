using System;
using System.IO;
using System.Text;

namespace Zergatul.Network.Ftp
{
    public class FtpControlStreamWriter
    {
        private readonly Stream _stream;

        public FtpControlStreamWriter(Stream stream)
        {
            this._stream = stream;
        }

        public void WriteReply(FtpServerReply reply)
        {
            if (reply.Message != null)
            {
                for (int i = 0; i < reply.Message.Length; i++)
                {
                    int @char = reply.Message[i];
                    if (@char == 10 || @char == 13)
                        throw new NotImplementedException();
                }
            }

            int code = (int)reply.Code;
            if (code < 100 || code >= 1000)
                throw new InvalidOperationException();

            string line = code + " " + (string.IsNullOrEmpty(reply.Message) ? reply.Code.ToString() : reply.Message) + Constants.TelnetEndOfLine;
            byte[] buffer = Encoding.ASCII.GetBytes(line);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }
}