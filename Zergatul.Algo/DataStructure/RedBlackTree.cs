using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zergatul.Algo.DataStructure
{
    public class RedBlackTree<T> : ICollection<T>
    {
        public IComparer<T> Comparer { get; }
        public Node Root { get; private set; }
        public int Version { get; private set; }

        private int _count;

        #region Constructors

        public RedBlackTree()
            : this(Comparer<T>.Default)
        {
            
        }

        public RedBlackTree(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            Comparer = comparer;
            Root = null;
            Version = 0;

            _count = 0;
        }

        public RedBlackTree(IEnumerable<T> data)
            : this(data, Comparer<T>.Default)
        {

        }

        public RedBlackTree(IEnumerable<T> data, IComparer<T> comparer)
            : this(data?.ToArray(), comparer)
        {

        }

        public RedBlackTree(T[] data, IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            Comparer = comparer;
            Version = 0;

            if (data == null || data.Length == 0)
            {
                Root = null;
                _count = 0;
                return;
            }

            int length = data.Length + 1;
            int height = 0;
            while (length > 0)
            {
                height++;
                length >>= 1;
            }

            Root = Build(data, 0, data.Length - 1, 1, height);
            _count = data.Length;
            Debug.Assert(IsValid());
        }

        #endregion

        #region Public methods

        public Node Add(T item)
        {
            Version++;

            if (Root == null)
            {
                Root = new Node(item, null, null, false);
                _count = 1;
                return Root;
            }

            Node parent = Root;
            Node newnode = new Node(item, null, null, true);
            while (true)
            {
                int compare = Comparer.Compare(item, parent.Value);
                if (compare < 0)
                {
                    if (parent.Left == null)
                    {
                        parent.SetLeft(newnode);
                        break;
                    }
                    else
                    {
                        parent = parent.Left;
                        continue;
                    }
                }
                if (compare > 0)
                {
                    if (parent.Right == null)
                    {
                        parent.SetRight(newnode);
                        break;
                    }
                    else
                    {
                        parent = parent.Right;
                        continue;
                    }
                }
                throw new InvalidOperationException("item already here");
            }

            _count++;

            var son = newnode;
            if (parent.IsRed)
            {
                while (parent.IsRed)
                {
                    var grandparent = parent.Parent;
                    var uncle = grandparent.Left == parent ? grandparent.Right : grandparent.Left;
                    if (uncle != null && uncle.IsRed)
                    {
                        parent.SetBlack();
                        uncle.SetBlack();
                        if (grandparent != Root)
                        {
                            grandparent.SetRed();
                            son = grandparent;
                            parent = son.Parent;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        bool left1 = grandparent.Left == parent;
                        bool left2 = parent.Left == son;

                        if (left1)
                        {
                            // LR case
                            if (!left2)
                                parent = parent.RotateLeft();

                            // LL case
                            var grandgrandparent = grandparent.Parent;
                            var rotated = grandparent.RotateRight();
                            grandparent.SetRed();
                            parent.SetBlack();
                            if (grandgrandparent == null)
                                Root = rotated;
                            break;
                        }
                        else
                        {
                            // RL case
                            if (left2)
                                parent = parent.RotateRight();

                            // RR case
                            var grandgrandparent = grandparent.Parent;
                            var rotated = grandparent.RotateLeft();
                            grandparent.SetRed();
                            parent.SetBlack();
                            if (grandgrandparent == null)
                                Root = rotated;
                            break;
                        }
                    }
                }
            }

            Debug.Assert(IsValid());
            return newnode;
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public Node Find(T item)
        {
            var node = Root;
            while (true)
            {
                if (node == null)
                    return null;

                int compare = Comparer.Compare(item, node.Value);
                if (compare == 0)
                    return node;

                node = compare < 0 ? node.Left : node.Right;
            }
        }

        public Node FindMin()
        {
            if (_count == 0)
                return null;

            var node = Root;
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        public Node FindMax()
        {
            if (_count == 0)
                return null;

            var node = Root;
            while (node.Right != null)
                node = node.Right;
            return node;
        }

        public void Remove(Node v)
        {
            Node u = BSTReplace(v);

            // True when u and v are both black
            bool uvBlack = ((u == null || u.IsBlack) && v.IsBlack);
            Node parent = v.Parent;

            if (u == null)
            {
                // u is NULL therefore v is leaf
                if (v == Root)
                {
                    // v is root, making root null
                    Root = null;
                }
                else
                {
                    if (uvBlack)
                    {
                        // u and v both black
                        // v is leaf, fix double black at v
                        fixDoubleBlack(v);
                    }
                    else
                    {
                        // u or v is red
                        if (v.GetSibling() != null)
                            // sibling is not null, make it red"
                            v.GetSibling().SetRed();
                    }

                    // delete v from the tree
                    if (v.IsOnLeft())
                    {
                        parent.SetLeft(null);
                    }
                    else
                    {
                        parent.SetRight(null);
                    }
                }
                _count--;
                Version++;
                Debug.Assert(IsValid());
                return;
            }

            if (v.Left == null || v.Right == null) {
                // v has 1 child
                if (v == Root)
                {
                    // v is root, assign the value of u to v, and delete u
                    v.Value = u.Value;
                    v.SetLeft(null);
                    v.SetRight(null);
                }
                else
                {
                    // Detach v from tree and move u up
                    if (v.IsOnLeft())
                    {
                        parent.SetLeft(u);
                    }
                    else
                    {
                        parent.SetRight(u);
                    }
                    u.Parent = parent;
                    if (uvBlack)
                    {
                        // u and v both black, fix double black at u
                        fixDoubleBlack(u);
                    }
                    else
                    {
                        // u or v red, color u black
                        u.SetBlack();
                    }
                }
                _count--;
                Version++;
                Debug.Assert(IsValid());
                return;
            }

            // v has 2 children, swap values with successor and recurse
            swapValues(u, v);
            Remove(u);
        }

        #endregion

        #region Private methods

        private void fixDoubleBlack(Node x)
        {
            if (x == Root)
                return;

            Node sibling = x.GetSibling();
            Node parent = x.Parent;
            if (sibling == null)
            {
                // No sibiling, double black pushed up
                fixDoubleBlack(parent);
            }
            else
            {
                if (sibling.IsRed)
                {
                    // Sibling red
                    parent.SetRed();
                    sibling.SetBlack();
                    if (sibling.IsOnLeft())
                    {
                        // left case
                        rightRotate(parent);
                    }
                    else
                    {
                        // right case
                        leftRotate(parent);
                    }
                    fixDoubleBlack(x);
                }
                else
                {
                    // Sibling black
                    if (sibling.hasRedChild())
                    {
                        // at least 1 red children
                        if (sibling.Left?.IsRed == true)
                        {
                            if (sibling.IsOnLeft())
                            {
                                // left left
                                sibling.Left.SetColor(sibling);
                                sibling.SetColor(parent);
                                rightRotate(parent);
                            }
                            else
                            {
                                // right left
                                sibling.Left.SetColor(parent);
                                rightRotate(sibling);
                                leftRotate(parent);
                            }
                        } else
                        {
                            if (sibling.IsOnLeft())
                            {
                                // left right
                                sibling.Right.SetColor(parent);
                                leftRotate(sibling);
                                rightRotate(parent);
                            }
                            else
                            {
                                // right right
                                sibling.Right.SetColor(sibling);
                                sibling.SetColor(parent);
                                leftRotate(parent);
                            }
                        }
                        parent.SetBlack();
                    }
                    else
                    {
                        // 2 black children
                        sibling.SetRed();
                        if (parent.IsBlack)
                            fixDoubleBlack(parent);
                        else
                            parent.SetBlack();
                    }
                }
            }
        }

        // left rotates the given node
        private void leftRotate(Node x)
        {
            // new parent will be node's right child
            Node nParent = x.Right;

            // update root if current node is root
            if (x == Root)
                Root = nParent;

            x.moveDown(nParent);

            // connect x with new parent's left element
            x.SetRight(nParent.Left);
            // connect new parent's left element with node
            // if it is not null
            if (nParent.Left != null)
                nParent.Left.Parent = x;

            // connect new parent with x
            nParent.SetLeft(x);
        }

        void rightRotate(Node x)
        {
            // new parent will be node's left child
            Node nParent = x.Left;

            // update root if current node is root
            if (x == Root)
                Root = nParent;

            x.moveDown(nParent);

            // connect x with new parent's right element
            x.SetLeft(nParent.Right);
            // connect new parent's right element with node
            // if it is not null
            if (nParent.Right != null)
                nParent.Right.Parent = x;

            // connect new parent with x
            nParent.SetRight(x);
        }

        private Node BSTReplace(Node x)
        {
            // when node have 2 children
            if (x.Left != null && x.Right != null)
                return Successor(x.Right);

            // when leaf
            if (x.Left == null && x.Right == null)
                return null;

            // when single child
            if (x.Left != null)
                return x.Left;
            else
                return x.Right;
        }

        private Node Successor(Node x)
        {
            Node temp = x;
            while (temp.Left != null)
                temp = temp.Left;
            return temp;
        }

        private void swapValues(Node u, Node v)
        {
            T temp;
            temp = u.Value;
            u.Value = v.Value;
            v.Value = temp;
        }

        #endregion

        #region ICollection<T>

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            Root = null;
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
                array[arrayIndex++] = item;
        }

        public int Count => _count;

        public bool IsReadOnly => false;

        public bool Remove(T item)
        {
            var node = Find(item);
            if (node == null)
                return false;

            Version++;

            Remove(node);
            return true;
            /**/

            if (_count == 0)
            {
                Root = null;
                return true;
            }

            #region BST deletion

            if (node.Left == null && node.Right == null)
            {
                // Node to be deleted is leaf
                if (node.Parent.Left == node)
                    node.Parent.SetLeft(null);
                else
                    node.Parent.SetRight(null);
            }
            else if (node.Left == null || node.Right == null)
            {
                // Node to be deleted has only one child
                if (node.Parent != null)
                {
                    if (node.Left != null)
                    {
                        if (node.Parent.Left == node)
                            node.Parent.SetLeft(node.Left);
                        else
                            node.Parent.SetRight(node.Left);
                    }
                    else
                    {
                        if (node.Parent.Left == node)
                            node.Parent.SetLeft(node.Right);
                        else
                            node.Parent.SetRight(node.Right);
                    }
                }
                else
                {
                    if (node.Left != null)
                        Root = node.Left;
                    else
                        Root = node.Right;

                    Root.Parent = null;
                }
            }
            else
            {
                // Node to be deleted has two children
                var replace = node.FindNext();
                node.Value = replace.Value;

                if (replace.Parent.Left == replace)
                    replace.Parent.SetLeft(null);
                else
                    replace.Parent.SetRight(null);

            }

            #endregion

            return true;
        }

        #endregion

        #region IEnumerable<T>

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(this);

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        #endregion

        private Node Build(T[] data, int from, int to, int height, int redheight)
        {
            if (from == to)
            {
                return new Node(data[from], null, null, height == redheight);
            }

            int mid = (from + to) / 2;
            var left = mid > from ? Build(data, from, mid - 1, height + 1, redheight) : null;
            var right = mid < to ? Build(data, mid + 1, to, height + 1, redheight) : null;
            return new Node(data[mid], left, right, false);
        }

        private bool IsValid()
        {
            if (Root == null)
                return true;

            if (Root.IsRed)
                return false;

            if (!CheckRedAdjRule(Root))
                return false;

            if (!CheckBlackNodesNumber(Root))
                return false;

            if (Root.Parent != null)
                return false;

            if (!CheckParentLinks(Root))
                return false;

            return true;
        }

        private bool CheckRedAdjRule(Node node)
        {
            if (node.IsRed)
            {
                if (node.Left?.IsRed == true)
                    return false;
                if (node.Right?.IsRed == true)
                    return false;
            }

            if (node.Left != null && !CheckRedAdjRule(node.Left))
                return false;
            if (node.Right != null && !CheckRedAdjRule(node.Right))
                return false;

            return true;
        }

        private bool CheckBlackNodesNumber(Node node)
        {
            int count = GetPaths(node, 0).Distinct().Count();
            if (count != 1)
                return false;

            if (node.Left != null && !CheckBlackNodesNumber(node.Left))
                return false;
            if (node.Right != null && !CheckBlackNodesNumber(node.Right))
                return false;

            return true;
        }

        private bool CheckParentLinks(Node node)
        {
            if (node.Left != null)
            {
                if (node.Left.Parent != node)
                    return false;
                if (!CheckParentLinks(node.Left))
                    return false;
            }

            if (node.Right != null)
            {
                if (node.Right.Parent != node)
                    return false;
                if (!CheckParentLinks(node.Right))
                    return false;
            }

            return true;
        }

        private IEnumerable<int> GetPaths(Node node, int depth)
        {
            if (!node.IsRed)
                depth++;

            if (node.Left == null)
            {
                yield return depth;
            }
            else
            {
                foreach (var path in GetPaths(node.Left, depth))
                    yield return path;
            }

            if (node.Right == null)
            {
                yield return depth;
            }
            else
            {
                foreach (var path in GetPaths(node.Right, depth))
                    yield return path;
            }
        }

        #region Nested classes

        public class Node
        {
            public T Value { get; internal set; }
            public bool IsRed { get; private set; }
            public bool IsBlack => !IsRed;
            public Node Parent { get; internal set; }
            public Node Left { get; private set; }
            public Node Right { get; private set; }

            public Node(T value, Node left, Node right, bool isred)
            {
                Value = value;
                Left = left;
                Right = right;
                IsRed = isred;

                if (left != null)
                    left.Parent = this;
                if (right != null)
                    right.Parent = this;
            }

            internal void SetLeft(Node node)
            {
                Left = node;
                if (node != null)
                    node.Parent = this;
            }

            internal void SetRight(Node node)
            {
                Right = node;
                if (node != null)
                    node.Parent = this;
            }

            internal void SetBlack()
            {
                IsRed = false;
            }

            internal void SetRed()
            {
                IsRed = true;
            }

            internal Node RotateLeft()
            {
                /*
                        this                  q
                       /    \                / \
                      a      q     ->    this   c
                            / \         /    \
                           b   c       a      b
                */
                var parent = Parent;
                var q = Right;
                var b = q.Left;

                SetRight(b);
                q.SetLeft(this);
                if (parent != null)
                {
                    if (parent.Left == this)
                        parent.SetLeft(q);
                    else
                        parent.SetRight(q);
                }
                else
                {
                    q.Parent = null;
                }
                return q;
            }

            internal Node RotateRight()
            {
                /*
                        this              p
                       /    \            / \
                      p      c    ->    a   this
                     / \                   /    \
                    a   b                 b      c
                */
                var parent = Parent;
                var p = Left;
                var b = p.Right;

                SetLeft(b);
                p.SetRight(this);
                if (parent != null)
                {
                    if (parent.Left == this)
                        parent.SetLeft(p);
                    else
                        parent.SetRight(p);
                }
                else
                {
                    p.Parent = null;
                }
                return p;
            }

            internal Node FindPrev()
            {
                var next = Left;
                if (next != null)
                {
                    while (next?.Right != null)
                        next = next.Right;
                    return next;
                }
                else
                {
                    var prev = this;
                    var node = Parent;
                    while (true)
                    {
                        if (node == null)
                            return null;

                        if (node.Right == prev)
                            return node;

                        prev = node;
                        node = node.Parent;
                    }
                }
            }

            internal Node FindNext()
            {
                var next = Right;
                if (next != null)
                {
                    while (next?.Left != null)
                        next = next.Left;
                    return next;
                }
                else
                {
                    var prev = this;
                    var node = Parent;
                    while (true)
                    {
                        if (node == null)
                            return null;

                        if (node.Left == prev)
                            return node;

                        prev = node;
                        node = node.Parent;
                    }
                }
            }

            internal bool IsOnLeft()
            {
                return this == Parent.Left;
            }

            internal Node GetSibling()
            {
                // sibling null if no parent
                if (Parent == null)
                    return null;

                if (IsOnLeft())
                    return Parent.Right;
                else
                    return Parent.Left;
            }

            // moves node down and moves given node in its place
            internal void moveDown(Node nParent)
            {
                if (Parent != null)
                {
                    if (IsOnLeft())
                    {
                        Parent.SetLeft(nParent);
                    }
                    else
                    {
                        Parent.SetRight(nParent);
                    }
                }
                nParent.Parent = Parent;
                Parent = nParent;
            }

            internal bool hasRedChild()
            {
                return Left?.IsRed == true || Right?.IsRed == true;
            }

            public void SetColor(Node node)
            {
                IsRed = node.IsRed;
            }

            public override string ToString()
            {
                string color = IsRed ? "RED" : "BLACK";
                return $"{Value} - {color}";
            }
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly RedBlackTree<T> _tree;
            private readonly int _version;
            private bool _isInitialized;
            private Node _current;

            public Enumerator(RedBlackTree<T> tree)
            {
                _tree = tree;
                _version = tree.Version;
                _isInitialized = false;
                _current = null;
            }

            #region IEnumerator<T>

            public T Current
            {
                get
                {
                    if (_current != null)
                        return _current.Value;
                    else
                        return default;
                }
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (_version != _tree.Version)
                    throw new InvalidOperationException();

                if (!_isInitialized)
                {
                    _isInitialized = true;

                    _current = _tree.Root;
                    while (_current?.Left != null)
                        _current = _current.Left;
                }
                else
                {
                    _current = _current?.FindNext();
                }

                return _current != null;
            }

            public void Reset() => throw new NotSupportedException();

            #endregion

            #region IEnumerator

            object IEnumerator.Current => Current;

            #endregion
        }

        #endregion
    }
}