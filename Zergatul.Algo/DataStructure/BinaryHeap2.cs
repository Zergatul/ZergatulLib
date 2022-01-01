using System;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure
{
    /// <summary>
    /// Supports removing/updating arbitrary elements.
    /// </summary>
    public class BinaryHeap2<TKey, TValue>
    {
        public int Count => _count;

        private Node[] _data;
        private int _capacity;
        private int _count;
        private IComparer<TKey> _comparer;

        public BinaryHeap2()
            : this(Comparer<TKey>.Default)
        {

        }

        public BinaryHeap2(IComparer<TKey> comparer)
        {
#if !NOCHECKS
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
#endif

            _capacity = 8;
            _data = new Node[_capacity + 1];
            _count = 0;
            _comparer = comparer;
        }

        // TODO change and expose Nodes
        public BinaryHeap2(TKey[] keys, TValue[] values, IComparer<TKey> comparer)
        {
            if (keys is null)
                throw new ArgumentNullException(nameof(keys));
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            if (comparer is null)
                throw new ArgumentNullException(nameof(comparer));
            if (keys.Length != values.Length)
                throw new ArgumentException("keys.Length != values.Length");

            _capacity = System.Math.Max(8, keys.Length);
            _data = new Node[_capacity + 1];
            _count = keys.Length;
            _comparer = comparer;

            for (int i = 0; i < _count; i++)
                _data[i + 1] = new Node(i + 1, keys[i], values[i]);

            for (int i = _count >> 1; i > 0; i--)
                HeapifyDown(i);
        }

        public Node Push(TKey key, TValue value)
        {
            if (_count == _capacity)
            {
                _capacity <<= 1;
                Node[] data = new Node[_capacity | 1];
                Array.Copy(_data, 1, data, 1, _count);
                _data = data;
            }

            _count++;
            Node result = _data[_count] = new Node(_count, key, value);
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
            if (_comparer.Compare(_data[2].Key, _data[3].Key) < 0)
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

        public void Update(Node node, TKey key)
        {
            int compare = _comparer.Compare(key, node.Key);
            if (compare == 0)
                return;

            node.Key = key;
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

                TKey oldValue = node.Key;
                TKey newValue = last.Key;
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

                if (left <= count && _comparer.Compare(_data[left].Key, _data[min].Key) < 0)
                    min = left;

                if (right <= count && _comparer.Compare(_data[right].Key, _data[min].Key) < 0)
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
                if (_comparer.Compare(_data[index].Key, _data[parent].Key) < 0)
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

                if (left <= _count && _comparer.Compare(_data[i].Key, _data[left].Key) > 0)
                    throw new InvalidOperationException();

                if (right <= _count && _comparer.Compare(_data[i].Key, _data[right].Key) > 0)
                    throw new InvalidOperationException();
            }
        }
#endif

        public class Node
        {
            internal int Index { get; set; }
            public TKey Key { get; internal set; }
            public TValue Value { get; }

            internal Node(int index, TKey key, TValue value)
            {
                Index = index;
                Key = key;
                Value = value;
            }

#if DEBUG
            public override string ToString()
            {
                return $"Key={Key} Value={Value}";
            }
#endif
        }
    }
}