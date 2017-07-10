using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Math
{
    public class ArithmeticException : Exception
    {
        public ArithmeticException(string message) : base(message)
        {

        }
    }
}