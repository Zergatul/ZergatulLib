using System.Collections.Generic;
using System.Text;

namespace Zergatul.Network.Http
{
    public static class KeepAliveParser
    {
        public static IEnumerable<KeyValuePair<string, string>> Parse(string value)
        {
            const int ParseKeyFirstChar = 1;
            const int ParseKey = 2;
            const int ParseValueFirstChar = 3;
            const int ParseValue = 4;
            const int ParseQuotedValue = 5;
            const int ParseWhitespace = 6;
            const int ParseComma = 7;

            if (string.IsNullOrEmpty(value))
                yield break;

            int status = ParseKeyFirstChar;
            var sb = new StringBuilder();
            string key = null;
            foreach (char ch in value)
            {
                switch (status)
                {
                    case ParseKeyFirstChar:
                        sb.Clear();
                        if (CharHelper.IsTChar(ch))
                        {
                            sb.Append(ch);
                            status = ParseKey;
                        }
                        else
                            yield break;
                        break;

                    case ParseKey:
                        if (ch == '=')
                        {
                            key = sb.ToString();
                            status = ParseValueFirstChar;
                            break;
                        }
                        if (CharHelper.IsTChar(ch))
                            sb.Append(ch);
                        else
                            yield break;
                        break;

                    case ParseValueFirstChar:
                        sb.Clear();
                        if (ch == '"')
                        {
                            status = ParseQuotedValue;
                        }
                        else
                        {
                            status = ParseValue;
                            if (CharHelper.IsTChar(ch))
                                sb.Append(ch);
                            else
                                yield break;
                        }
                        break;

                    case ParseValue:
                        if (ch == ',')
                        {
                            yield return new KeyValuePair<string, string>(key, sb.ToString());
                            status = ParseWhitespace;
                            key = null;
                            break;
                        }
                        if (CharHelper.IsTChar(ch))
                            sb.Append(ch);
                        else
                            yield break;
                        break;

                    case ParseQuotedValue:
                        if (ch == '"')
                        {
                            yield return new KeyValuePair<string, string>(key, sb.ToString());
                            status = ParseComma;
                            key = null;
                            break;
                        }
                        if (CharHelper.IsWhitespace(ch) || CharHelper.IsVChar(ch) || CharHelper.IsObsText(ch))
                            sb.Append(ch);
                        else
                            yield break;
                        break;

                    case ParseWhitespace:
                        if (!CharHelper.IsWhitespace(ch))
                        {
                            status = ParseKeyFirstChar;
                            goto case ParseKeyFirstChar;
                        }
                        break;

                    case ParseComma:
                        if (ch == ',')
                            status = ParseWhitespace;
                        else
                            yield break;
                        break;
                }
            }

            if (key != null)
                yield return new KeyValuePair<string, string>(key, sb.ToString());
        }
    }
}