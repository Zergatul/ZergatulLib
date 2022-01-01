using System;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure
{
    public class BinaryHeap3<T>
    {
        public int Count => _count;

        private Node[] _data;
        private int _capacity;
        private int _count;
        private IComparer<T> _comparer;

        public BinaryHeap3()
            : this(Comparer<T>.Default)
        {

        }

        public BinaryHeap3(IComparer<T> comparer)
            : this(8, comparer)
        {

        }

        public BinaryHeap3(int capacity, IComparer<T> comparer)
        {
#if !NOCHECKS
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
#endif

            _capacity = capacity;
            _data = new Node[_capacity + 1];
            _count = 0;
            _comparer = comparer;
        }

        public void AddRange(T[] data)
        {
            _count = data.Length;
            for (int i = 0; i < _count; i++)
                _data[i + 1] = new Node(i + 1, data[i]);

            for (int i = _count >> 1; i > 0; i--)
                HeapifyDown(i);
        }

        public Node Push(T element)
        {
            if (_count == _capacity)
            {
                _capacity <<= 1;
                Node[] data = new Node[_capacity | 1];
                Array.Copy(_data, 1, data, 1, _count);
                _data = data;
            }

            _count++;
            Node result = _data[_count] = new Node(_count, element);
            HeapifyUp(_count);

            return result;
        }

        public Node Peek()
        {
#if !NOCHECKS
            if (_count == 0)
                throw new InvalidOperationException();
#endif

            return _data[1];
        }

        public void Peek2(out Node top1, out Node top2)
        {
#if !NOCHECKS
            if (_count < 2)
                throw new InvalidOperationException();
#endif

            top1 = _data[1];
            if (_comparer.Compare(_data[2].Value, _data[3].Value) < 0)
                top2 = _data[2];
            else
                top2 = _data[3];
        }

        public Node Pop()
        {
#if !NOCHECKS
            if (_count == 0)
                throw new InvalidOperationException("Heap is empty.");
#endif

            Node result = _data[1];
            _data[1] = _data[_count];
            _count--;
            HeapifyDown(1);

            return result;
        }

        public void Update(Node node, T element)
        {
            int compare = _comparer.Compare(element, node.Value);
            if (compare == 0)
                return;

            node.Value = element;
            if (compare < 0)
                HeapifyUp(node.Index); // decrease
            else
                HeapifyDown(node.Index); // increase
        }

        public void Remove(Node node)
        {
            int index = node.Index;
            if (index != _count)
            {
                Node last = _data[_count];
                last.Index = index;
                _data[index] = last;
                _data[_count--] = null;

                T oldValue = node.Value;
                T newValue = last.Value;
                int compare = _comparer.Compare(newValue, oldValue);
                if (compare < 0)
                    HeapifyUp(index); // decrease
                else
                    HeapifyDown(index); // increase
            }
            else
            {
                _data[_count--] = null;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = index << 1;
                int right = left | 1;
                int min = index;
                int count = _count;

                if (left <= count && _comparer.Compare(_data[left].Value, _data[min].Value) < 0)
                    min = left;

                if (right <= count && _comparer.Compare(_data[right].Value, _data[min].Value) < 0)
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
                if (_comparer.Compare(_data[index].Value, _data[parent].Value) < 0)
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
            Node buffer = _data[i1];
            _data[i1] = _data[i2];
            _data[i2] = buffer;

            _data[i1].Index = i1;
            _data[i2].Index = i2;
        }

#if DEBUG
        private void Validate()
        {
            for (int i = _count >> 1; i > 0; i--)
            {
                int left = i << 1;
                int right = left | 1;

                if (left <= _count && _comparer.Compare(_data[i].Value, _data[left].Value) > 0)
                    throw new InvalidOperationException();

                if (right <= _count && _comparer.Compare(_data[i].Value, _data[right].Value) > 0)
                    throw new InvalidOperationException();
            }
        }
#endif

        public class Node
        {
            internal int Index { get; set; }
            public T Value { get; internal set; }

            internal Node(int index, T value)
            {
                Index = index;
                Value = value;
            }

#if DEBUG
            public override string ToString()
            {
                return $"Value={Value}";
            }
#endif
        }
    }
}