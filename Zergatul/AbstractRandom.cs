using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public abstract class AbstractRandom : IRandom
    {
        public virtual void GetBytes(byte[] data)
        {
            GetBytes(data, 0, data.Length);
        }

        public abstract void GetBytes(byte[] data, int offset, int count);

        public ulong GetUInt64()
        {
            byte[] buffer = new byte[8];
            GetBytes(buffer);
            return BitHelper.ToUInt64(buffer, 0, ByteOrder.BigEndian);
        }
    }
}