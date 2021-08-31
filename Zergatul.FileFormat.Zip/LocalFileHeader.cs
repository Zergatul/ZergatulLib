namespace Zergatul.FileFormat.Zip
{
    internal class LocalFileHeader
    {
        public bool IsEncrypted => (_flags & 0x01) != 0;
        public bool SizeInDataDescriptor => (_flags & 0x08) != 0;
        public CompressionMethod Method { get; private set; }
        public int CompressedSize { get; private set; }

        public string FileName { get; private set; }

        private int _flags;

        public static LocalFileHeader Parse(BinaryReader reader)
        {
            if (reader.ReadInt32() != 0x04034B50)
                throw new ZipDataFormatException();

            int version = reader.ReadInt16();
            int flags = reader.ReadInt16();
            int method = reader.ReadInt16();
            int lastModifiedTime = reader.ReadInt16();
            int lastModifiedDate = reader.ReadInt16();
            int crc32 = reader.ReadInt32();
            int compressedSize = reader.ReadInt32();
            int uncompressedSize = reader.ReadInt32();
            int filenameLen = reader.ReadInt16();
            int extraFieldLen = reader.ReadInt16();
            string filename = reader.ReadString(filenameLen);
            string extraField = reader.ReadString(extraFieldLen);

            return new LocalFileHeader
            {
                _flags = flags,
                Method = (CompressionMethod)method,
                CompressedSize = compressedSize,
                FileName = filename
            };
        }
    }
}