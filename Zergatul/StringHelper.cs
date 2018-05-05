using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    internal static class StringHelper
    {
        public static bool IsASCII(string str)
        {
            if (string.IsNullOrEmpty(str))
                return true;
            for (int i = 0; i < str.Length; i++)
                if (str[i] >= 128)
                    return false;
            return true;
        }
    }
}