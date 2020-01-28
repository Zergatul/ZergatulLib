using System;

namespace Zergatul.FileFormat.Pdf
{
    internal static class Common
    {
        public const int MaxGeneration = 65535;

        public static bool IsEndOfLine(int value) => value == 0x0A || value == 0x0D;

        public static bool IsWhiteSpace(int value) =>
            value == 0x20 || value == 0x0A || value == 0x0D || value == 0x09 || value == 0x00 || value == 0x0C;

        public static bool IsDigit(int value) => '0' <= value && value <= '9';

        public static bool IsHexDigit(int value) =>
            ('0' <= value && value <= '9') || ('A' <= value && value <= 'F') || ('a' <= value && value <= 'f');

        public static bool IsNumberToken(byte value) =>
            ('0' <= value && value <= '9') || value == '-' || value == '+' || value == '.';

        public static bool IsZeroString(string value)
        {
            for (int i = 0; i < value.Length; i++)
                if (value[i] != '0')
                    return false;
            return true;
        }

        public static int ParseHex(int value)
        {
            if ('0' <= value && value <= '9')
                return value - '0';
            if ('A' <= value && value <= 'F')
                return 10 + value - 'A';
            if ('a' <= value && value <= 'f')
                return 10 + value - 'a';
            throw new ArgumentOutOfRangeException();
        }

        public static bool IsSpecial(int value) =>
            value == '/' || value == '[' || value == ']' || value == '<' || value == '>' || value == '(' || value == ')';
    }
}