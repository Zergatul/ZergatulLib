using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void WriteAsync(Stream data, CancellationToken cancellationToken, IProgress<long> progress)
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    WriteStreamAsync(data, cancellationToken, progress);
                    break;
            }
        }

        private void WriteStream(Stream data)
        {
            data.CopyTo(_stream);
        }

        private void WriteStreamAsync(Stream data, CancellationToken cancellationToken, IProgress<long> progress)
        {
            int totalWrite = 0;
            byte[] buffer = new byte[1024];
            if (progress != null)
                progress.Report(0);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int bytesRead = data.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;
                cancellationToken.ThrowIfCancellationRequested();
                _stream.Write(buffer, 0, bytesRead);
                totalWrite += bytesRead;
                if (progress != null)
                    progress.Report(totalWrite);
            }
        }
    }
}