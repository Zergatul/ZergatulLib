using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public class ParseException : Exception
    {
        public ParseException()
            : base()
        {

        }

        public ParseException(string message)
            : base(message)
        {

        }

        public static void ThrowIfTrue(bool value)
        {
            if (value)
                throw new ParseException();
        }

        public static void ThrowIfNull(object value)
        {
            if (value == null)
                throw new ParseException();
        }

        public static void ThrowIfNotEqual(int value, int expected)
        {
            if (value != expected)
                throw new ParseException();
        }

        public static void ThrowIfNotInRange(int value, int from, int to)
        {
            if (from > value || value > to)
                throw new ParseException();
        }
    }
}