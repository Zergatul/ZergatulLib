using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.DataStructure
{
    public class BinaryHeap1<T>
    {
        public int Count => _count;

        private T[] _data;
        private int _capacity;
        private int _count;
        private readonly IComparer<T> _comparer;

        public BinaryHeap1()
            : this(Comparer<T>.Default)
        {

        }

        public BinaryHeap1(int capacity)
            : this(capacity, Comparer<T>.Default)
        {

        }

        public BinaryHeap1(IComparer<T> comparer)
            : this(8, comparer)
        {

        }

        public BinaryHeap1(int capacity, IComparer<T> comparer)
        {
#if !NOCHECKS
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
#endif

            _capacity = capacity;
            _data = new T[_capacity + 1];
            _count = 0;
            _comparer = comparer;
        }

        public void AddRange(T[] data)
        {
            _count = data.Length;
            Array.Copy(data, 0, _data, 1, _count);

            for (int i = _count >> 1; i > 0; i--)
                HeapifyDown(i);
        }

        public void Push(T element)
        {
            if (_count == _capacity)
            {
                _capacity <<= 1;
                T[] data = new T[_capacity | 1];
                Array.Copy(_data, 1, data, 1, _count);
                _data = data;
            }

            _data[++_count] = element;
            HeapifyUp(_count);
        }

        public T Peek()
        {
#if !NOCHECKS
            if (_count == 0)
                throw new InvalidOperationException();
#endif

            return _data[1];
        }

        public T Pop()
        {
#if !NOCHECKS
            if (_count == 0)
                throw new InvalidOperationException("Heap is empty.");
#endif

            T result = _data[1];
            _data[1] = _data[_count];
            _count--;
            HeapifyDown(1);

            return result;
        }

        private void HeapifyDown(int index)
        {
            while (true)
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
                    Swap(index, min);
                    index = min;
                }
                else
                {
                    break;
                }
            }
        }

        private void HeapifyUp(int index)
        {
            while (index > 1)
            {
                int parent = index >> 1;
                if (_comparer.Compare(_data[index], _data[parent]) < 0)
                {
                    Swap(index, parent);
                    index = parent;
                }
                else
                {
                    break;
                }
            }
        }

        private void Swap(int i1, int i2)
        {
            T buffer = _data[i1];
            _data[i1] = _data[i2];
            _data[i2] = buffer;
        }

#if DEBUG
        private void Validate()
        {
            for (int i = _count >> 1; i > 0; i--)
            {
                int left = i << 1;
                int right = left | 1;

                if (left <= _count && _comparer.Compare(_data[i], _data[left]) > 0)
                    throw new InvalidOperationException();

                if (right <= _count && _comparer.Compare(_data[i], _data[right]) > 0)
                    throw new InvalidOperationException();
            }
        }
#endif
    }
}