using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.Sorting
{
    public static class MergeSort
    {
        //public static void Sort<T>(T[] array, int left, int right)
        //{
        //    if (left < right)
        //    {
        //        int middle = (left + right) / 2;

        //        Sort(array, left, middle);
        //        Sort(array, middle + 1, right);

        //        Merge(array, left, middle, right);
        //    }
        //}

        //private static void Merge<T>(T[] array, int left, int middle, int right)
        //{
        //    T[] leftArray = new T[middle - left + 1];
        //    T[] rightArray = new T[right - middle];

        //    Array.Copy(array, left, leftArray, 0, middle - left + 1);
        //    Array.Copy(array, middle + 1, rightArray, 0, right - middle);

        //    int i = 0;
        //    int j = 0;
        //    for (int k = left; k < right + 1; k++)
        //    {
        //        if (i == leftArray.Length)
        //        {
        //            array[k] = rightArray[j];
        //            j++;
        //        }
        //        else if (j == rightArray.Length)
        //        {
        //            array[k] = leftArray[i];
        //            i++;
        //        }
        //        else if (leftArray[i] <= rightArray[j])
        //        {
        //            array[k] = leftArray[i];
        //            i++;
        //        }
        //        else
        //        {
        //            array[k] = rightArray[j];
        //            j++;
        //        }
        //    }
        //}
    }
}