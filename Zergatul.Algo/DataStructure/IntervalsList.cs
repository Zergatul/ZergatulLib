using System;
using System.Collections;
using System.Collections.Generic;

namespace Zergatul.Algo.DataStructure
{
    public class IntervalList : IEnumerable<IntervalList.Interval>
    {
        public int Count => _list.Count - 2;

        public Interval this[int index] => _list[index + 1];

        private readonly List<Interval> _list;
        private readonly double _epsilon;
        private readonly FromComparer _fromComparer;
        private readonly ToComparer _toComparer;

        public IntervalList(double epsilon)
        {
            _list = new List<Interval>();
            _epsilon = epsilon;
            _fromComparer = new FromComparer(epsilon);
            _toComparer = new ToComparer(epsilon);
            Clear();
        }

        public void Add(double from, double to)
        {
            var interval = new Interval(from, to);

            int index1 = _list.BinarySearch(interval, _fromComparer);
            int index2 = _list.BinarySearch(interval, _toComparer);

            if (index1 < 0)
            {
                index1 = ~index1;
                var prev = _list[index1 - 1];
                if (prev.Contains(from, _epsilon))
                {
                    from = prev.From;
                    index1--;
                }
            }

            if (index2 < 0)
            {
                index2 = ~index2;
                var next = _list[index2];
                if (next.Contains(to, _epsilon))
                {
                    to = next.To;
                    index2++;
                }
            }
            else
            {
                index2++;
            }

            if (index1 == index2)
            {
                interval = new Interval(from, to);
                if (!_list[index1].Equals(interval, _epsilon))
                    _list.Insert(index1, interval);
                return;
            }
            else
            {
                int remove = index2 - index1 - 1;
                _list.RemoveRange(index1, remove);
                _list[index1] = new Interval(from, to);
            }
        }

        public void Clear()
        {
            _list.Clear();
            _list.Add(new Interval(0.25 * double.MinValue, 0.25 * double.MinValue));
            _list.Add(new Interval(0.25 * double.MaxValue, 0.25 * double.MaxValue));
        }

        public Enumerator GetEnumerator() => new Enumerator(_list);

        #region Interfaces

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<Interval> IEnumerable<Interval>.GetEnumerator() => GetEnumerator();

        #endregion

        public readonly struct Interval : IEquatable<Interval>, IEquatable<(double, double)>
        {
            public double From { get; }
            public double To { get; }

            public Interval(double from, double to)
            {
                From = from;
                To = to;
            }

            public bool Contains(double value, double epsilon)
            {
                return From - value < epsilon && value - To < epsilon;
            }

            public bool Equals(Interval other, double epsilon)
            {
                return Math.Abs(From - other.From) < epsilon && Math.Abs(To - other.To) < epsilon;
            }

            #region Overrides

            public override string ToString() => $"{From}; {To}";

            #endregion

            #region Interfaces

            public bool Equals(Interval other)
            {
                return From == other.From && To == other.To;
            }

            public bool Equals((double, double) other)
            {
                return (From, To) == other;
            }

            #endregion

            #region Operators

            public static bool operator ==(Interval i1, (double, double) i2)
            {
                return (i1.From, i1.To) == i2;
            }

            public static bool operator !=(Interval i1, (double, double) i2)
            {
                return (i1.From, i1.To) != i2;
            }

            #endregion
        }

        private class FromComparer : IComparer<Interval>
        {
            private readonly double _epsilon;

            public  FromComparer(double epsilon)
            {
                _epsilon = epsilon;
            }

            public int Compare(Interval i1, Interval i2)
            {
                if (Math.Abs(i1.From - i2.From) < _epsilon)
                    return 0;
                else
                    return i1.From.CompareTo(i2.From);
            }
        }

        private class ToComparer : IComparer<Interval>
        {
            private readonly double _epsilon;

            public ToComparer(double epsilon)
            {
                _epsilon = epsilon;
            }

            public int Compare(Interval i1, Interval i2)
            {
                if (Math.Abs(i1.To - i2.To) < _epsilon)
                    return 0;
                else
                    return i1.To.CompareTo(i2.To);
            }
        }

        public struct Enumerator : IEnumerator<Interval>
        {
            public Interval Current => _list[_index];
            object IEnumerator.Current => Current;

            private readonly List<Interval> _list;
            private int _index;

            public Enumerator(List<Interval> list)
            {
                _list = list;
                _index = 0;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _list.Count - 1;
            }

            public void Dispose()
            {
                
            }

            public void Reset()
            {
                throw new System.NotSupportedException();
            }
        }
    }
}