using System;
using System.Text;

namespace Zergatul.FileFormat.Csv
{
    internal static class CellParser
    {
        public static int GetInt(Memory<byte> raw)
        {
            return int.Parse(Encoding.ASCII.GetString(raw.Span));
        }

        public static double? GetDoubleNullable(Memory<byte> raw)
        {
            if (raw.IsEmpty)
                return null;
            return double.Parse(Encoding.ASCII.GetString(raw.Span));
        }
    }
}