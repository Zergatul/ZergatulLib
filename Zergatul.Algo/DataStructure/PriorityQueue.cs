using System;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure
{
    public class PriorityQueue<T>
    {
        private T[] _data;
        private int _capacity;
        private int _count;
        private IComparer<T> _comparer;

        public PriorityQueue(T[] data, IComparer<T> comparer)
        {
            if (data is null)
                throw new ArgumentNullException(nameof(data));
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));

            _capacity = System.Math.Max(8, data.Length);
            _data = new T[_capacity + 1];
            _count = data.Length;
            _comparer = comparer;

            Array.Copy(data, 0, _data, 1, data.Length);

            for (int i = _count >> 1; i > 0; i--)
                HeapifyDown(i);
        }

        public T Dequeue()
        {
            if (_count == 0)
                throw new InvalidOperationException("Queue is empty");

            T result = _data[1];
            _data[1] = _data[_count];
            _count--;
            HeapifyDown(1);

            return result;
        }

        private void HeapifyDown(int index)
        {
            int left = index << 1;
            int right = left | 1;
            int min = index;
            int count = _count;

            if (left <= count && _comparer.Compare(_data[left], _data[min]) < 0)
                min = left;

            if (right <= count && _comparer.Compare(_data[right], _data[min]) < 0)
                min = right;

            if (min != index)
            {
                T buffer = _data[index];
                _data[index] = _data[min];
                _data[min] = buffer;

                HeapifyDown(min);
            }
        }
    }
}