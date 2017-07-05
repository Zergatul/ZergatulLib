using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Network.Tls
{
    internal class ReadCounter
    {
        private BinaryReader _reader;
        private int _totalBytes;
        private int _initialPosition;

        public ReadCounter(BinaryReader reader, int totalBytes)
        {
            this._reader = reader;
            this._totalBytes = totalBytes;
            this._initialPosition = reader.Position;
        }

        public bool CanRead => _initialPosition + _totalBytes > _reader.Position;
    }
}
