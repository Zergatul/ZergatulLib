using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zergatul.Network.Mime
{
    static class EncodingHelper
    {
        private static readonly Regex _headerValueRegex = new Regex(@"^[a-zA-Z0-9\-.%/<>@_]+$");
        private static readonly Regex _headerSpaceValueRegex = new Regex(@"^[a-zA-Z0-9\-.%/<>@_ ]+$");

        public static string EncodedWordsEncode(string value, Encoding encoding)
        {
            string prefix = "=?" + encoding.BodyName + "?B?";
            string suffix = "?=";
            int length = 75 - prefix.Length - suffix.Length;
            int maxBytes = ((4 * length / 3) + 3) & ~3;
            var sb = new StringBuilder();

            byte[] bytes = encoding.GetBytes(value);
            for (int i = 0; i < (bytes.Length + maxBytes - 1) / maxBytes; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Constants.TelnetEndOfLine);
                    sb.Append(' ');
                }

                byte[] part = ByteArray.SubArray(bytes, i * maxBytes, System.Math.Min(maxBytes, bytes.Length - i * maxBytes));
                sb.Append(prefix);
                sb.Append(Convert.ToBase64String(part));
                sb.Append(suffix);
            }
            return sb.ToString();
        }

        public static bool IsValidHeaderValueString(string value, bool allowSpace)
        {
            if (allowSpace)
                return _headerSpaceValueRegex.IsMatch(value);
            else
                return _headerValueRegex.IsMatch(value);
        }
    }
}