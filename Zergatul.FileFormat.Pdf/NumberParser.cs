using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Zergatul.FileFormat.Pdf
{
    using static Common;

    internal struct NumberParser : IEnumerator<long>
    {
        public long Current => _hasNext ? _current : throw new InvalidOperationException();
        object IEnumerator.Current => Current;

        private readonly byte[] _data;
        private long _offset;
        private long _count;

        private long _current;
        private bool _hasNext;

        public NumberParser(byte[] data, long offset, long count)
        {
            _data = data;
            _offset = offset;
            _count = count;

            _current = 0;
            _hasNext = false;
        }

        public bool MoveNext()
        {
            while (_count > 0 && IsWhiteSpace(_data[_offset]))
            {
                _offset++;
                _count--;
            }

            if (_count == 0)
            {
                _hasNext = false;
                return false;
            }

            if (!IsDigit(_data[_offset]))
                throw new InvalidDataException("Digit character expected.");

            _current = 0;
            int len = 0;
            while (IsDigit(_data[_offset]))
            {
                if (len == 15)
                    throw new InvalidDataException("Number parser overflow.");

                _current = _current * 10 + _data[_offset] - '0';
                _offset++;
                _count--;
                len++;
            }

            _hasNext = true;
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {

        }
    }
}