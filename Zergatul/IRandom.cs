using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public interface IRandom
    {
        void GetBytes(byte[] data);
        void GetBytes(byte[] data, int offset, int count);

        ulong GetUInt64();
    }
}