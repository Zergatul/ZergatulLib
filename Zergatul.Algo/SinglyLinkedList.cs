using System;
using System.Collections;
using System.Collections.Generic;

namespace Zergatul.Algo
{
    public class SinglyLinkedList<T> : IEnumerable<T>
    {
        public Node First;
        public Node Last;
        public int Count;

        public void AddFirst(T value)
        {
            First = new Node(value, First);
            if (First.Next == null)
                Last = First;
            Count++;
        }

        public void AddLast(T value)
        {
            if (Last == null)
            {
                AddFirst(value);
            }
            else
            {
                Last = Last.Next = new Node(value, null);
                Count++;
            }
        }

        public void AddAfter(Node prev, T value)
        {
            if (prev == null)
            {
                AddFirst(value);
            }
            else
            {
                var newNode = prev.Next = new Node(value, prev.Next);
                if (newNode.Next == null)
                    Last = newNode;
                Count++;
            }
        }

        public void RemoveFirst()
        {
            if (First == null)
                throw new InvalidOperationException();

            First = First.Next;
            if (First == null)
                Last = null;

            Count--;
        }

        public void RemoveAfter(Node prev)
        {
            if (prev == null)
            {
                RemoveFirst();
            }
            else
            {
                if (prev.Next == null)
                    throw new InvalidOperationException();

                var next = prev.Next = prev.Next.Next;
                if (next == null)
                    Last = prev;

                Count--;
            }
        }

        #region Interfaces

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Enumerator GetEnumerator() => new Enumerator(this);

        #endregion

        public class Node
        {
            public T Value;
            public Node Next;

            public Node(T value, Node next)
            {
                Value = value;
                Next = next;
            }
        }

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private SinglyLinkedList<T> _list;
            private T _current;
            private int _index;
            private Node _node;

            public Enumerator(SinglyLinkedList<T> list)
            {
                _list = list;
                _current = default;
                _index = -1;
                _node = null;
            }

            public T Current => _current;

            object IEnumerator.Current => _current;

            public void Dispose()
            {
                
            }

            public bool MoveNext()
            {
                if (_index == -1)
                    _node = _list.First;

                _index++;

                if (_node == null)
                {
                    _current = default;
                    return false;
                }
                else
                {
                    _current = _node.Value;
                    _node = _node.Next;
                    return true;
                }
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}