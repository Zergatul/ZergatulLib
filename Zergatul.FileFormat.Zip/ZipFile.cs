using System;
using System.IO;

namespace Zergatul.FileFormat.Zip
{
    public class ZipFile
    {
        public Stream InnerStream { get; }

        private BinaryReader _reader;

        public ZipFile(Stream stream)
        {
            InnerStream = stream;

            _reader = new BinaryReader(stream);
        }

        public (string, byte[]) GetCompressedData()
        {
            var header = LocalFileHeader.Parse(_reader);
            return (header.FileName, _reader.ReadBytes(header.CompressedSize));
        }
    }
}