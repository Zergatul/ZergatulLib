using System;

namespace Zergatul.Algo.DataStructure.SegmentTree
{
    public struct ReadOnlyMinimumIndexSegmentTree
    {
        private int[] values;
        private int[] indexes;
        private int length;

        public void Build(int[] array)
        {
            length = array.Length;

            values = new int[length << 1];
            indexes = new int[length << 1];

            Array.Copy(array, 0, values, length, length);
            for (int i = 0; i < length; i++)
                indexes[length + i] = i;

            for (int i = length - 1; i > 0; i--)
            {
                int i1 = i << 1;
                int i2 = i1 | 1;
                int vi1 = values[i1];
                int vi2 = values[i2];
                if (vi1 < vi2)
                {
                    values[i] = vi1;
                    indexes[i] = indexes[i1];
                }
                else
                {
                    values[i] = vi2;
                    indexes[i] = indexes[i2];
                }
            }
        }

        public int Query(int left, int right)
        {
            int result = int.MaxValue;
            int index = -1;
            for (left += length, right += length; left < right; left >>= 1, right >>= 1)
            {
                if ((left & 1) != 0)
                {
                    int val = values[left];
                    if (val < result)
                    {
                        result = val;
                        index = indexes[left];
                    }
                    left++;
                }

                if ((right & 1) != 0)
                {
                    int val = values[--right];
                    if (val < result)
                    {
                        result = val;
                        index = indexes[right];
                    }
                }
            }
            return index;
        }
    }
}