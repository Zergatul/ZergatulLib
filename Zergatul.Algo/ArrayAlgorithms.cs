using System;

namespace Zergatul.Algo
{
    public static class ArrayAlgorithms
    {
        public static T FindKth<T>(T[] array, int k, Random rnd, Comparison<T> comparison)
        {
            return FindKth(array, k, 0, array.Length - 1, rnd, comparison);
        }

        public static void Shuffle<T>(T[] array, Random rnd)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        private static T FindKth<T>(T[] array, int k, int left, int right, Random rnd, Comparison<T> comparison)
        {
            if (left == right)
                return array[left];

            int pivotIndex = rnd.Next(left, right + 1);
            pivotIndex = Partition(array, left, right, pivotIndex, comparison);

            if (k == pivotIndex)
                return array[k];
            else if (k < pivotIndex)
                return FindKth(array, k, left, pivotIndex - 1, rnd, comparison);
            else
                return FindKth(array, k, pivotIndex + 1, right, rnd, comparison);
        }

        private static int Partition<T>(T[] array, int left, int right, int pivotIndex, Comparison<T> comparison)
        {
            var pivotValue = array[pivotIndex];
            (array[pivotIndex], array[right]) = (array[right], array[pivotIndex]);
            int storeIndex = left;
            for (int i = left; i < right; i++)
            {
                if (comparison(array[i], pivotValue) < 0)
                {
                    (array[storeIndex], array[i]) = (array[i], array[storeIndex]);
                    storeIndex++;
                }
            }
            (array[right], array[storeIndex]) = (array[storeIndex], array[right]);
            return storeIndex;
        }
    }
}