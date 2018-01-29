using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency
{
    public class BlockParseException : Exception
    {
        public BlockParseException()
        {

        }

        public BlockParseException(Exception innerException)
            : base("", innerException)
        {

        }
    }
}