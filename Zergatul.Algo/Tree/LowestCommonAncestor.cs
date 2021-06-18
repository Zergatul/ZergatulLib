using System;
using System.Collections.Generic;
using System.Numerics;
using Zergatul.Algo.DataStructure.SegmentTree;

namespace Zergatul.Algo.Tree
{
    public static class LowestCommonAncestor
    {
        /// <summary>
        /// <br>Preprocessing - O(n)</br>
        /// <br>Query - O(log(n))</br>
        /// </summary>
        public static SegmentTreeStruct SegmentTreeAlgo(int root, int[][] adj)
        {
            BuildOrderArray(root, adj, out int[] order, out int[] first, out int[] height);

            var segmentTree = new ReadOnlyMinimumIndexSegmentTree();
            segmentTree.Build(height);

            return new SegmentTreeStruct(order, first, height, segmentTree);
        }

        /// <summary>
        /// <br>Preprocessing - O(n)</br>
        /// <br>Query - O(1)</br>
        /// </summary>
        public static FarachColtonBenderStruct FarachColtonBenderAlgo(int root, int[][] adj)
        {
            BuildOrderArray2(root, adj, out int[] order, out int[] first, out int[] height);

            int orderSize = order.Length;
            int[] log2 = new int[orderSize + 1];
            log2[0] = -1;
            for (int i = 1; i < log2.Length; i++)
                log2[i] = log2[i >> 1] + 1;

            int blockSize = System.Math.Max(1, log2[orderSize] >> 1);
            int blockCount = (orderSize + blockSize - 1) / blockSize;

            int[][] sparseTable = new int[blockCount][];
            int rowSize = log2[blockCount] + 1;
            for (int i = 0; i < blockCount; i++)
                sparseTable[i] = new int[rowSize];

            int[] blockMask = new int[blockCount];

            int possibilities = 1 << (blockSize - 1);
            int[][][] blocks = new int[possibilities][][];

            var fcb = new FarachColtonBenderStruct(order, first, height, blockSize, log2, sparseTable, blockMask, blocks);

            for (int i = 0, j = 0, b = 0; i < orderSize; i++, j++)
            {
                if (j == blockSize)
                {
                    j = 0;
                    b++;
                }
                if (j == 0 || fcb.MinByHeight(i, sparseTable[b][0]) == i)
                    sparseTable[b][0] = i;
            }

            int limit = log2[blockCount];
            for (int l = 1; l <= limit; l++)
            {
                for (int i = 0; i < blockCount; i++)
                {
                    int ni = i + (1 << (l - 1));
                    if (ni >= blockCount)
                        sparseTable[i][l] = sparseTable[i][l - 1];
                    else
                        sparseTable[i][l] = fcb.MinByHeight(sparseTable[i][l - 1], sparseTable[ni][l - 1]);
                }
            }

            for (int i = 0, j = 0, b = 0; i < orderSize; i++, j++)
            {
                if (j == blockSize)
                {
                    j = 0;
                    b++;
                }
                if (j > 0 && (i >= orderSize || fcb.MinByHeight(i - 1, i) == i - 1))
                    blockMask[b] += 1 << (j - 1);
            }

            for (int b = 0; b < blockCount; b++)
            {
                int mask = blockMask[b];
                if (blocks[mask] != null)
                    continue;

                blocks[mask] = new int[blockSize][];
                for (int i = 0; i < blockSize; i++)
                    blocks[mask][i] = new int[blockSize];

                for (int l = 0; l < blockSize; l++)
                {
                    blocks[mask][l][l] = l;
                    for (int r = l + 1; r < blockSize; r++)
                    {
                        blocks[mask][l][r] = blocks[mask][l][r - 1];
                        if (b * blockSize + r < orderSize)
                            blocks[mask][l][r] = fcb.MinByHeight(b * blockSize + blocks[mask][l][r], b * blockSize + r) - b * blockSize;
                    }
                }
            }

            return fcb;
        }

        public static SparseTableStruct SparseTableAlgo(int root, int[][] adj)
        {
            BuildOrderArray(root, adj, out int[] order, out int[] first, out int[] height);

            int orderLength = order.Length;
            int tableHeight = BitOperations.Log2((uint)orderLength);
            int[][] table = new int[tableHeight][];

            if (tableHeight > 0)
            {
                int[] row = new int[orderLength - 1];
                table[0] = row;

                for (int j = 0; j < row.Length; j++)
                    row[j] = height[j] < height[j + 1] ? j : j + 1;
            }

            for (int i = 1; i < tableHeight; i++)
            {
                int blockLength = 2 << i;
                int prevBlockLength = 1 << i;
                int[] row = new int[orderLength - blockLength + 1];
                table[i] = row;
                int[] prevRow = table[i - 1];

                for (int j = 0; j < row.Length; j++)
                {
                    int i1 = prevRow[j];
                    int i2 = prevRow[j + prevBlockLength];
                    row[j] = height[i1] < height[i2] ? i1 : i2;
                }
            }

            return new SparseTableStruct(order, first, height, table);
        }

        /// <summary>
        /// height - height of items in order array
        /// </summary>
        public static void BuildOrderArray(int root, int[][] adj, out int[] order, out int[] first, out int[] height)
        {
            const int Leave = 0x40000000;
            const int ReEnter = 0x20000000;

            var stack = new Stack<int>();
            stack.Push(root | Leave);
            stack.Push(root);

            int n = adj.Length;
            int index = 0;
            int currHeight = 0;
            order = new int[2 * n - 1];
            first = new int[n];
            height = new int[2 * n - 1];

            while (stack.Count > 0)
            {
                int top = stack.Pop();
                if (top >= Leave)
                {
                    top ^= Leave;
                    currHeight--;
                    if (order[index - 1] != top)
                    {
                        height[index] = currHeight;
                        order[index++] = top;
                    }
                    continue;
                }

                if (top >= ReEnter)
                {
                    height[index] = currHeight - 1;
                    order[index++] = top ^ ReEnter;
                    continue;
                }

                first[top] = index;
                height[index] = currHeight++;
                order[index++] = top;

                int[] curAdj = adj[top];
                for (int i = curAdj.Length - 1; i >= 0; i--)
                {
                    int child = curAdj[i];
                    stack.Push(child | Leave);
                    stack.Push(child);
                    if (i != 0)
                        stack.Push(top | ReEnter);
                }
            }
        }

        /// <summary>
        /// height - height of vertices
        /// </summary>
        public static void BuildOrderArray2(int root, int[][] adj, out int[] order, out int[] first, out int[] height)
        {
            const int Leave = 0x40000000;
            const int ReEnter = 0x20000000;

            var stack = new Stack<int>();
            stack.Push(root | Leave);
            stack.Push(root);

            int n = adj.Length;
            int index = 0;
            int currHeight = 0;
            order = new int[2 * n - 1];
            first = new int[n];
            height = new int[n];

            while (stack.Count > 0)
            {
                int top = stack.Pop();
                if (top >= Leave)
                {
                    top ^= Leave;
                    currHeight--;
                    if (order[index - 1] != top)
                    {
                        order[index++] = top;
                    }
                    continue;
                }

                if (top >= ReEnter)
                {
                    order[index++] = top ^ ReEnter;
                    continue;
                }

                first[top] = index;
                height[top] = currHeight++;
                order[index++] = top;

                int[] curAdj = adj[top];
                for (int i = curAdj.Length - 1; i >= 0; i--)
                {
                    int child = curAdj[i];
                    stack.Push(child | Leave);
                    stack.Push(child);
                    if (i != 0)
                        stack.Push(top | ReEnter);
                }
            }
        }

        public readonly struct SegmentTreeStruct
        {
            public int[] Order { get; }
            public int[] First { get; }
            public int[] Height { get; }
            public ReadOnlyMinimumIndexSegmentTree SegmentTree { get; }

            public SegmentTreeStruct(int[] order, int[] first, int[] height, ReadOnlyMinimumIndexSegmentTree segmentTree)
            {
                Order = order;
                First = first;
                Height = height;
                SegmentTree = segmentTree;
            }

            public int Query(int v1, int v2)
            {
                int i1 = First[v1];
                int i2 = First[v2];
                if (i1 > i2)
                    (i1, i2) = (i2, i1);

                return Order[SegmentTree.Query(i1, i2 + 1)];
            }
        }

        public readonly struct FarachColtonBenderStruct
        {
            public int[] Order { get; }
            public int[] First { get; }
            public int[] Height { get; }
            public int BlockSize { get; }
            public int[] Log2 { get; }
            public int[][] SparseTable { get; }
            public int[] BlockMask { get; }
            public int[][][] Blocks { get; }

            public FarachColtonBenderStruct(
                int[] order,
                int[] first,
                int[] height,
                int blockSize,
                int[] log2,
                int[][] sparseTable,
                int[] blockMask,
                int[][][] blocks)
            {
                Order = order;
                First = first;
                Height = height;
                BlockSize = blockSize;
                Log2 = log2;
                SparseTable = sparseTable;
                BlockMask = blockMask;
                Blocks = blocks;
            }

            public int Query(int v, int u)
            {
                int l = First[v];
                int r = First[u];
                if (l > r)
                    (l, r) = (r, l);

                int bl = l / BlockSize;
                int br = r / BlockSize;
                if (bl == br)
                    return Order[InBlock(bl, l % BlockSize, r % BlockSize)];

                int ans1 = InBlock(bl, l % BlockSize, BlockSize - 1);
                int ans2 = InBlock(br, 0, r % BlockSize);
                int ans = MinByHeight(ans1, ans2);
                if (bl + 1 < br)
                {
                    int lq = Log2[br - bl - 1];
                    int ans3 = SparseTable[bl + 1][lq];
                    int ans4 = SparseTable[br - (1 << lq)][lq];
                    ans = MinByHeight(ans, MinByHeight(ans3, ans4));
                }
                return Order[ans];
            }

            public int InBlock(int b, int l, int r)
            {
                return Blocks[BlockMask[b]][l][r] + b * BlockSize;
            }

            public int MinByHeight(int i1, int i2)
            {
                return Height[Order[i1]] < Height[Order[i2]] ? i1 : i2;
            }
        }

        public readonly struct SparseTableStruct
        {
            public int[] Order { get; }
            public int[] First { get; }
            public int[] Height { get; }
            public int[][] Table { get; }

            public SparseTableStruct(int[] order, int[] first, int[] height, int[][] table)
            {
                Order = order;
                First = first;
                Height = height;
                Table = table;
            }

            public int Query(int v1, int v2)
            {
                if (v1 == v2)
                    return v1;

                int i1 = First[v1];
                int i2 = First[v2];

                if (i1 > i2)
                    (i1, i2) = (i2, i1);

                if (i2 - i1 == 1)
                    return Order[Height[i1] < Height[i2] ? i1 : i2];

                int log = BitOperations.Log2((uint)(i2 - i1));
                int[] row = Table[log - 1];
                int pow2 = 1 << log;

                int min1 = row[i1];
                int min2 = row[i2 - pow2 + 1];
                return Order[Height[min1] < Height[min2] ? min1 : min2];
            }
        }
    }
}