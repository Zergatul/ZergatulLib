using System;
using System.Collections;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure.Hash
{
    public class CoalescedChainingHashSet<T> : ICollection<T>
        where T : IEquatable<T>
    {
        private const int DefaultCapacity = 8;

        private BucketItem[] _buckets;
        private int _capacity;
        private int _resizeWhen;
        private int _mask;
        private int _version;
        private int _count;

        public CoalescedChainingHashSet()
            : this(DefaultCapacity)
        {
            
        }

        public CoalescedChainingHashSet(int capacity)
        {
            SetCapacity(capacity);
        }

        public bool Add(T value)
        {
            if (_count == _resizeWhen)
                Resize(_capacity << 1);

            int hash = value.GetHashCode();
            int index = hash & _mask;
            var item = _buckets[index];
            while (item.Next != 0)
            {
                if (item.Value.Equals(value))
                    return false;

                if (item.Next < 0)
                    break;

                index = item.Next - 1;
                item = _buckets[index];
            }

            _version++;
            _count++;

            if (item.Next == 0)
            {
                // empty bucket
                _buckets[index] = new BucketItem(value);
            }
            else
            {
                // filled bucket
                int next = index;
                while (true)
                {
                    next++;
                    if (next >= _capacity)
                        next = 0;
                    if (_buckets[next].Next == 0)
                        break;
                }
                _buckets[index] = new BucketItem(item.Value, next + 1);
                _buckets[next] = new BucketItem(value);
            }

            return true;
        }

        public bool Contains(T value)
        {
            int hash = value.GetHashCode();
            int index = hash & _mask;
            var item = _buckets[index];
            while (item.Next != 0)
            {
                if (item.Value.Equals(value))
                    return true;

                if (item.Next < 0)
                    return false;

                index = item.Next - 1;
                item = _buckets[index];
            }

            return false;
        }

        public bool Remove(T value)
        {
            int hash = value.GetHashCode();
            int index = hash & _mask;
            int prevIndex = -1;
            var item = _buckets[index];
            while (item.Next != 0)
            {
                if (item.Value.Equals(value))
                {
                    RemoveBucket(prevIndex, index, item.Next - 1);
                    _version++;
                    _count--;
                    return true;
                }

                if (item.Next < 0)
                    return false;

                prevIndex = index;
                index = item.Next - 1;
                item = _buckets[index];
            }

            return false;
        }

        public Enumerator GetEnumerator()
        {
            ShrinkIfRequired();
            return new Enumerator(this);
        }

        #region ICollection<T>

        public int Count => _count;
        public bool IsReadOnly => false;

        void ICollection<T>.Add(T value) => Add(value);

        public void Clear()
        {
            _version++;
            _count = 0;
            Array.Fill(_buckets, default);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in _buckets)
            {
                if (item.Next == 0)
                    continue;

                array[arrayIndex++] = item.Value;
            }
        }

        #endregion

        #region IEnumerable<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        private void SetCapacity(int capacity)
        {
            int c = 8;
            while (c < capacity)
                c <<= 1;

            _capacity = c;
            _mask = c - 1;
            _resizeWhen = (c >> 1) | (c >> 3); // 62.5%
            _buckets = new BucketItem[_capacity];
        }

        private void Resize(int capacity)
        {
            var oldBuckets = _buckets;
            _version++;
            SetCapacity(capacity);

            foreach (var bucket in oldBuckets)
            {
                if (bucket.Next == 0)
                    continue;

                int hash = bucket.Value.GetHashCode();
                int index = hash & _mask;
                var item = _buckets[index];
                while (item.Next != 0)
                {
                    if (item.Next < 0)
                        break;

                    index = item.Next - 1;
                    item = _buckets[index];
                }

                if (item.Next == 0)
                {
                    // empty bucket
                    _buckets[index] = new BucketItem(bucket.Value);
                }
                else
                {
                    // filled bucket
                    int next = index;
                    while (true)
                    {
                        next++;
                        if (next >= _capacity)
                            next = 0;
                        if (_buckets[next].Next == 0)
                            break;
                    }
                    _buckets[index] = new BucketItem(item.Value, next + 1);
                    _buckets[next] = new BucketItem(bucket.Value);
                }
            }
        }

        private void RemoveBucket(int prevIndex, int index, int nextIndex)
        {
            if (prevIndex >= 0)
            {
                var prev = _buckets[prevIndex];
                _buckets[prevIndex] = new BucketItem(prev.Value, nextIndex);
                _buckets[index] = default;
            }
            else
            {
                if (nextIndex >= 0)
                {
                    _buckets[index] = _buckets[nextIndex];
                    _buckets[nextIndex] = default;
                }
                else
                {
                    _buckets[index] = default;
                }
            }
        }

        private void ShrinkIfRequired()
        {
            if (_count > 0 && _count < (_capacity >> 2))
            {
                int capacity = _capacity;
                while (_count < (capacity >> 1) && capacity > DefaultCapacity)
                    capacity >>= 1;

                Resize(capacity);
            }
        }

        #region Nested classes

        private struct BucketItem
        {
            public T Value { get; }
            public int Next { get; }

            public BucketItem(T value)
            {
                Value = value;
                Next = -1;
            }

            public BucketItem(T value, int next)
            {
                Value = value;
                Next = next;
            }

            public override string ToString()
            {
                if (Next == 0)
                    return "NULL";
                if (Next == -1)
                    return Value.ToString();
                return $"{Value} -> ({Next})";
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            public T Current => _current;
            object IEnumerator.Current => _current;

            private readonly CoalescedChainingHashSet<T> _set;
            private readonly BucketItem[] _buckets;
            private readonly int _version;
            private int _index;
            private T _current;

            internal Enumerator(CoalescedChainingHashSet<T> set)
            {
                _set = set;
                _buckets = set._buckets;
                _version = set._version;
                _index = set._count > 0 ? 0 : int.MinValue;
                _current = default;
            }

            public bool MoveNext()
            {
                if (_set._version != _version)
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                if (_index < 0)
                    return false;

                for (int i = _index; i < _buckets.Length; i++)
                {
                    var bucket = _set._buckets[i];
                    if (bucket.Next != 0)
                    {
                        _current = bucket.Value;
                        _index = i + 1;
                        return true;
                    }
                }

                _index = int.MaxValue;
                return false;
            }

            public void Dispose()
            {
                
            }

            public void Reset()
            {
                _index = 0;
                _current = default;
            }
        }

        #endregion
    }
}