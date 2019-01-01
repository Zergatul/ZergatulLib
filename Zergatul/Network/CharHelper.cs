using System;

namespace Zergatul.Network
{
    internal static class CharHelper
    {
        public static bool IsTab(int value) => value == 0x09;
        public static bool IsCR(int value) => value == 0x0D;
        public static bool IsLF(int value) => value == 0x0A;
        public static bool IsWhitespace(int value) => value == 0x09 || value == ' ';
        public static bool IsDigit(int value) => '0' <= value && value <= '9';
        public static bool IsHex(int value) => ('0' <= value && value <= '9') || ('a' <= value && value <= 'f') || ('A' <= value && value <= 'F');
        public static bool IsVChar(int value) => 0x20 <= value && value <= 0x7F;
        public static bool IsObsText(int value) => 0x80 <= value && value <= 0xFF;

        public static bool IsTChar(int value)
        {
            if ('^' <= value && value <= 'z')
                return true;
            if ('A' <= value && value <= 'Z')
                return true;
            if ('0' <= value && value <= '9')
                return true;
            if ('#' <= value && value <= '\'')
                return true;
            if (value == '!')
                return true;
            if (value == '*')
                return true;
            if (value == '+')
                return true;
            if (value == '-')
                return true;
            if (value == '.')
                return true;
            if (value == '|')
                return true;
            if (value == '~')
                return true;
            return false;
        }

        public static int ParseHex(int value)
        {
            switch (value)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}