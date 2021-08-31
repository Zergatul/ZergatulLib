using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.Sorting
{
    public static class QuickSort
    {
        public static void Sort<T>(T[] array, IComparer<T> comparer)
        {
            Sort(array, comparer, 0, array.Length - 1, new Random());
        }

        private static void Sort<T>(T[] array, IComparer<T> comparer, int left, int right, Random rnd)
        {
            if (left < right)
            {
                int pivot = Partition(array, comparer, left, right, rnd);

                if (pivot > 1)
                    Sort(array, comparer, left, pivot - 1, rnd);

                if (pivot + 1 < right)
                    Sort(array, comparer, pivot + 1, right, rnd);
            }
        }

        private static int Partition<T>(T[] array, IComparer<T> comparer, int left, int right, Random rnd)
        {
            int index = rnd.Next(left, right + 1);
            if (index != right)
                Swap(array, index, right);

            T pivot = array[right];

            index = left;
            for (int i = left; i < right; i++)
            {
                if (comparer.Compare(array[i], pivot) <= 0)
                {
                    Swap(array, index, i);
                    index++;
                }
            }

            Swap(array, index, right);

            return index;
        }

        private static void Swap<T>(T[] array, int i1, int i2)
        {
            T buf = array[i1];
            array[i1] = array[i2];
            array[i2] = buf;
        }
    }
}