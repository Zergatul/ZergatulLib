using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpDataStreamWriter
    {
        private Stream _stream;
        private FtpTransferMode _mode;

        public FtpDataStreamWriter(Stream stream, FtpTransferMode mode)
        {
            if (mode != FtpTransferMode.Stream)
                throw new NotImplementedException(mode + " mode not implemented");

            this._stream = stream;
            this._mode = mode;
        }

        public void Write(Stream data)
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    WriteStream(data);
                    break;
            }
        }

        private void WriteStream(Stream data)
        {
            data.CopyTo(_stream);
        }
    }
}