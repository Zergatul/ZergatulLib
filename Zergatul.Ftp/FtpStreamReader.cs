using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public class FtpStreamReader
    {
        private Stream _stream;
        private FtpTransferMode _mode;

        public FtpStreamReader(Stream stream, FtpTransferMode mode)
        {
            if (mode == FtpTransferMode.Compressed)
                throw new NotImplementedException("Compressed mode not implemented");

            this._stream = stream;
            this._mode = mode;
        }

        public void ReadToStreamAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    ReadStreamToStreamAsync(destination, cancellationToken, progress);
                    return;
                case FtpTransferMode.Block:
                    ReadBlockToStreamAsync(destination, cancellationToken, progress);
                    return;
                case FtpTransferMode.Compressed:
                    ReadCompressedToStreamAsync(destination, cancellationToken, progress);
                    return;
                default:
                    throw new FtpException("Unknown mode");
            }
        }

        public List<byte> ReadToEnd()
        {
            switch (_mode)
            {
                case FtpTransferMode.Stream:
                    return ReadStreamToEnd();
                case FtpTransferMode.Block:
                    return ReadBlockToEnd();
                case FtpTransferMode.Compressed:
                    return ReadCompressedToEnd();
                default:
                    throw new FtpException("Unknown mode");
            }
        }

        private List<byte> ReadStreamToEnd()
        {
            var bytes = new List<byte>();
            var buffer = new byte[1024];
            while (true)
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;
                if (bytesRead == buffer.Length)
                    bytes.AddRange(buffer);
                else
                    for (int i = 0; i < bytesRead; i++)
                        bytes.Add(buffer[i]);
            }
            _stream.Close();
            return bytes;
        }

        private List<byte> ReadBlockToEnd()
        {
            var bytes = new List<byte>();
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
            return bytes;
        }

        private List<byte> ReadCompressedToEnd()
        {
            throw new NotImplementedException();
        }

        private void ReadStreamToStreamAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
            int totalRead = 0;
            var buffer = new byte[1024];
            progress.Report(0);
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break;
                destination.Write(buffer, 0, bytesRead);
                totalRead += bytesRead;
                progress.Report(totalRead);
            }
        }

        private void ReadBlockToStreamAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
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

        private void ReadCompressedToStreamAsync(Stream destination, CancellationToken cancellationToken, IProgress<long> progress)
        {
        }
    }
}