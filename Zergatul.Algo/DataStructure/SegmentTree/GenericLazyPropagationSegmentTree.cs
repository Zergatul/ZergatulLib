using System;

namespace Zergatul.Algo.DataStructure.SegmentTree
{
    public struct GenericLazyPropagationSegmentTree<T, K>
        where T : struct
        where K : struct
    {
        public int Length { get; }
        public Func<T, T, T> CombineData { get; }
        public Func<K, K, K> CombineModification { get; }
        public Func<T, K, T> ApplyModification { get; }

        private T[] _data;
        private K[] _modifications;

        public GenericLazyPropagationSegmentTree(
            T[] data,
            K zero,
            Func<T, T, T> combineData,
            Func<K, K, K> combineModification,
            Func<T, K, T> applyModification)
        {
            Length = data.Length;
            CombineData = combineData;
            CombineModification = combineModification;
            ApplyModification = applyModification;

            _data = new T[Length << 1];
            Array.Copy(data, 0, _data, Length, Length);

            _modifications = new K[Length << 1];
            Array.Fill(_modifications, zero);

            for (int i = Length - 1; i > 0; i--)
            {
                int i1 = i << 1;
                int i2 = i1 | 1;
                _data[i] = combineData(_data[i1], _data[i2]);
            }
        }

        public void Modify(int left, int right, K value)
        {
            //for (left += le, r += n; l < r; l >>= 1, r >>= 1)
            //{
            //    if (l & 1) t[l++] += value;
            //    if (r & 1) t[--r] += value;
            //}
        }
    }
}