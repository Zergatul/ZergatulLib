using System;
using System.Collections;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure
{
    public class Deque<T> : IReadOnlyCollection<T>
    {
        private T[] _buffer;
        private int _capacity;
        private int _offset;
        private int _length;

        public Deque(int capacity)
        {
            _capacity = capacity;
            _buffer = new T[capacity];
        }

        #region Public methods

        public void PushFirst(T value)
        {
            EnsureCapacity();

            _offset = GetIndexDec(_offset - 1);
            _buffer[_offset] = value;
            _length++;
        }

        public void PushLast(T value)
        {
            EnsureCapacity();

            _buffer[GetIndexInc(_offset + _length)] = value;
            _length++;
        }

        public T PeekFirst()
        {
            if (_length == 0)
                throw new InvalidOperationException();

            return _buffer[_offset];
        }

        public T PeekLast()
        {
            if (_length == 0)
                throw new InvalidOperationException();

            return _buffer[GetIndexInc(_offset + _length - 1)];
        }

        public T PopFirst()
        {
            if (_length == 0)
                throw new InvalidOperationException();

            T result = _buffer[_offset];
            _offset = GetIndexInc(_offset + 1);
            _length--;
            return result;
        }

        public T PopLast()
        {
            if (_length == 0)
                throw new InvalidOperationException();

            T result = _buffer[GetIndexInc(_offset + _length - 1)];
            _length--;
            return result;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        #endregion

        #region Private methods

        private void EnsureCapacity()
        {
            if (_capacity == _length)
            {
                T[] newBuffer = new T[_capacity << 1];
                Array.Copy(_buffer, _offset, newBuffer, 0, _capacity - _offset);
                Array.Copy(_buffer, 0, newBuffer, _capacity - _offset, _offset);
                _offset = 0;
                _capacity <<= 1;
                _buffer = newBuffer;
            }
        }

        private int GetIndexDec(int position)
        {
            if (position < 0)
                return position + _capacity;
            else
                return position;
        }

        private int GetIndexInc(int position)
        {
            if (position >= _capacity)
                return position - _capacity;
            else
                return position;
        }

        #endregion

        #region Interfaces

        public int Count => _length;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Nested classes

        public struct Enumerator : IEnumerator<T>
        {
            public T Current => _deque._buffer[_deque.GetIndexInc(_deque._offset + _position)];
            object IEnumerator.Current => Current;

            private Deque<T> _deque;
            private int _position;

            public Enumerator(Deque<T> deque)
            {
                _deque = deque;
                _position = -1;
            }

            public bool MoveNext()
            {
                _position++;
                return _position < _deque.Count;
            }

            public void Dispose()
            {
                
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }
        }

        #endregion
    }
}