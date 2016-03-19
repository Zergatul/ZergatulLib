using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpDataStreamReader
    {
        private Stream _stream;
        private FtpTransferMode _mode;

        public FtpDataStreamReader(Stream stream, FtpTransferMode mode)
        {
            if (mode == FtpTransferMode.Compressed)
                throw new NotImplementedException("Compressed mode not implemented");

            this._stream = stream;
            this._mode = mode;
        }

        public void ReadAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    ReadStreamAsync(destination, cancellationToken, progress);
                    return;
                case FtpTransferMode.Block:
                    ReadBlockAsync(destination, cancellationToken, progress);
                    return;
                case FtpTransferMode.Compressed:
                    ReadCompressedAsync(destination, cancellationToken, progress);
                    return;
                default:
                    throw new FtpException("Unknown mode");
            }
        }

        public void Read(Stream destination)
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    ReadStream(destination);
                    break;
                case FtpTransferMode.Block:
                    ReadBlock(destination);
                    break;
                case FtpTransferMode.Compressed:
                    ReadCompressed(destination);
                    break;
                default:
                    throw new FtpException("Unknown mode");
            }
        }

        private void ReadStream(Stream destination)
        {
            _stream.CopyTo(destination);
        }

        private void ReadBlock(Stream destination)
        {
            var buffer = new byte[1024];
            while (true)
            {
                _stream.Read(buffer, 0, 3);
                byte descriptor = buffer[0];
                int blockLength = (buffer[1] << 8) | buffer[2];
                if ((descriptor & FtpBlockHeaderDescriptor.EOF) != 0 ||
                    (descriptor & FtpBlockHeaderDescriptor.EOR) != 0)
                    break;
                int totalRead = 0;
                while (totalRead < blockLength)
                {
                    int bytesRead = _stream.Read(buffer, 0, Math.Min(buffer.Length, blockLength - totalRead));
                    totalRead += bytesRead;
                    destination.Write(buffer, 0, bytesRead);
                }
            }
        }

        private List<byte> ReadCompressed(Stream destination)
        {
            throw new NotImplementedException();
        }

        private void ReadStreamAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
            int totalRead = 0;
            var buffer = new byte[1024];
            if (progress != null)
                progress.Report(0);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;
                destination.Write(buffer, 0, bytesRead);
                totalRead += bytesRead;
                if (progress != null)
                    progress.Report(totalRead);
            }
        }

        private void ReadBlockAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
            throw new NotImplementedException();
            /*var bytes = new List<byte>();
            var buffer = new byte[1024];
            while (true)
            {
                _stream.Read(buffer, 0, 3);
                byte descriptor = buffer[0];
                int blockLength = (buffer[1] << 8) | buffer[2];
                if ((descriptor & FtpBlockHeaderDescriptor.EOF) != 0 ||
                    (descriptor & FtpBlockHeaderDescriptor.EOR) != 0)
                    break;
                int totalRead = 0;
                while (totalRead < blockLength)
                {
                    int bytesRead = _stream.Read(buffer, 0, Math.Min(buffer.Length, blockLength - totalRead));
                    totalRead += bytesRead;
                    for (int i = 0; i < bytesRead; i++)
                        bytes.Add(buffer[i]);
                }
            }
            _stream.Close();
            return bytes;*/
        }

        private void ReadCompressedAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
        }
    }
}