using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Cryptocurrency.P2P
{
    public class InvVect
    {
        public VectorType Type;
        public byte[] Hash;
    }

    public enum VectorType : uint
    {
        ERROR = 0,
        MSG_TX = 1,
        MSG_BLOCK = 2,
        MSG_FILTERED_BLOCK = 3,
        MSG_CMPCT_BLOCK = 4
    }
}