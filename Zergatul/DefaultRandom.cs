using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul
{
    public class DefaultRandom : AbstractRandom
    {
        private Random _rnd;

        public DefaultRandom()
        {
            _rnd = new Random();
        }

        public override void GetBytes(byte[] data, int offset, int count)
        {
            for (int i = 0; i < count; i++)
                data[offset + i] = (byte)_rnd.Next(256);
        }
    }
}
