using System;
using System.Collections.Generic;
using System.Text;

namespace Zergatul.Algo.Sorting
{
    public static class TimSort
    {
        const int MinMerge = 32;
        const int InitBufferLen = 256;
        const int MinGallop = 7;

        public static void Sort<T>(T[] array, IComparer<T> comparer)
        {
            Sort(array, 0, array.Length, comparer);
        }

        public static void Sort<T>(T[] array, int left, int right, IComparer<T> comparer)
        {
            int len = right - left;
            if (len <= 1)
                return;

            if (len < MinMerge)
            {
                int initRunLen = MakeRun(array, left, right, comparer);
                BinarySort(array, left, right, left + initRunLen, comparer);
                return;
            }

            var sort = new SortStructure<T>(array, comparer);

            int minRun = GetMinRunLength(len);
            while (true)
            {
                int runLen = MakeRun(array, left, right, comparer);
                if (runLen < minRun)
                {
                    int newRunLen = len <= minRun ? len : minRun;
                    BinarySort(array, left, left + newRunLen, left + runLen, comparer);
                    runLen = newRunLen;
                }

                sort.Runs[sort.StackSize++] = new Run(left, runLen);
                sort.MergeCollapse();

                left += runLen;
                len -= runLen;
                if (len == 0)
                    break;
            }

            sort.MergeLastCollapse();
        }

        private static int MakeRun<T>(T[] array, int left, int right, IComparer<T> comparer)
        {
            int runRight = left + 1;
            if (runRight == right)
                return 1;

            if (comparer.Compare(array[runRight++], array[left]) < 0)
            {
                // Descending
                while (runRight < right && comparer.Compare(array[runRight], array[runRight - 1]) < 0)
                    runRight++;
                Array.Reverse(array, left, runRight - left);
            }
            else
            {
                // Ascending
                while (runRight < right && comparer.Compare(array[runRight], array[runRight - 1]) >= 0)
                    runRight++;
            }

            return runRight - left;
        }

        private static void BinarySort<T>(T[] a, int lo, int hi, int start, IComparer<T> c)
        {
            if (start == lo)
                start++;
            for (; start < hi; start++)
            {
                T pivot = a[start];
                // Set left (and right) to the index where a[start] (pivot) belongs
                int left = lo;
                int right = start;

                /*
                 * Invariants:
                 *   pivot >= all in [lo, left).
                 *   pivot <  all in [right, start).
                 */
                while (left < right)
                {
                    int mid = (left + right) >> 1;
                    if (c.Compare(pivot, a[mid]) < 0)
                        right = mid;
                    else
                        left = mid + 1;
                }
                /*
                 * The invariants still hold: pivot >= all in [lo, left) and
                 * pivot < all in [left, start), so pivot belongs at left.  Note
                 * that if there are elements equal to pivot, left points to the
                 * first slot after them -- that's why this sort is stable.
                 * Slide elements over to make room for pivot.
                 */
                int n = start - left;  // The number of elements to move
                Array.Copy(a, left, a, left + 1, n);
                a[left] = pivot;
            }
        }

        private static int GetMinRunLength(int n)
        {
            int r = 0;
            while (n >= MinMerge)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }

        private static int GallopLeft<T>(T key, T[] a, int @base, int len, int hint, IComparer<T> c)
        {
            int lastOfs = 0;
            int ofs = 1;
            if (c.Compare(key, a[@base + hint]) > 0)
            {
                // Gallop right until a[base+hint+lastOfs] < key <= a[base+hint+ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && c.Compare(key, a[@base + hint + ofs]) > 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs * 2) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;
                // Make offsets relative to base
                lastOfs += hint;
                ofs += hint;
            }
            else
            { // key <= a[base + hint]
              // Gallop left until a[base+hint-ofs] < key <= a[base+hint-lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && c.Compare(key, a[@base + hint - ofs]) <= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs * 2) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;
                // Make offsets relative to base
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }

            /*
             * Now a[base+lastOfs] < key <= a[base+ofs], so key belongs somewhere
             * to the right of lastOfs but no farther right than ofs.  Do a binary
             * search, with invariant a[base + lastOfs - 1] < key <= a[base + ofs].
             */
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);
                if (c.Compare(key, a[@base + m]) > 0)
                    lastOfs = m + 1;  // a[base + m] < key
                else
                    ofs = m;          // key <= a[base + m]
            }

            return ofs;
        }

        private static int GallopRight<T>(T key, T[] a, int @base, int len, int hint, IComparer<T> c)
        {
            int ofs = 1;
            int lastOfs = 0;
            if (c.Compare(key, a[@base + hint]) < 0)
            {
                // Gallop left until a[b+hint - ofs] <= key < a[b+hint - lastOfs]
                int maxOfs = hint + 1;
                while (ofs < maxOfs && c.Compare(key, a[@base + hint - ofs]) < 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs * 2) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;
                // Make offsets relative to b
                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            else
            { // a[b + hint] <= key
              // Gallop right until a[b+hint + lastOfs] <= key < a[b+hint + ofs]
                int maxOfs = len - hint;
                while (ofs < maxOfs && c.Compare(key, a[@base + hint + ofs]) >= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs * 2) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;
                // Make offsets relative to b
                lastOfs += hint;
                ofs += hint;
            }
            /*
             * Now a[b + lastOfs] <= key < a[b + ofs], so key belongs somewhere to
             * the right of lastOfs but no farther right than ofs.  Do a binary
             * search, with invariant a[b + lastOfs - 1] <= key < a[b + ofs].
             */
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);
                if (c.Compare(key, a[@base + m]) < 0)
                    ofs = m;          // key < a[b + m]
                else
                    lastOfs = m + 1;  // a[b + m] <= key
            }

            return ofs;
        }

        private struct SortStructure<T>
        {
            public T[] Array;
            public IComparer<T> Comparer;
            public T[] Buffer;
            public Run[] Runs;
            public int StackSize;
            public int MinGallop;

            public SortStructure(T[] array, IComparer<T> comparer)
            {
                Array = array;
                Comparer = comparer;

                int length = array.Length;
                Buffer = new T[length < 2 * InitBufferLen ? length >> 1 : InitBufferLen];
                int stackLen =
                    length < 120 ? 5 :
                    length < 1542 ? 10 :
                    length < 119151 ? 19 : 40;

                Runs = new Run[stackLen];
                StackSize = 0;
                MinGallop = TimSort.MinGallop;
            }

            public void MergeCollapse()
            {
                while (StackSize > 1)
                {
                    int n = StackSize - 2;
                    if (n > 0 && Runs[n - 1].Len <= Runs[n].Len + Runs[n + 1].Len)
                    {
                        if (Runs[n - 1].Len < Runs[n + 1].Len)
                            n--;
                        MergeAt(n);
                    }
                    else if (Runs[n].Len <= Runs[n + 1].Len)
                    {
                        MergeAt(n);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            public void MergeLastCollapse()
            {
                while (StackSize > 1)
                {
                    int n = StackSize - 2;
                    if (n > 0 && Runs[n - 1].Len < Runs[n + 1].Len)
                        n--;
                    MergeAt(n);
                }
            }

            private void MergeAt(int i)
            {
                var run = Runs[i];
                int base1 = run.Base;
                int len1 = run.Len;

                run = Runs[i + 1];
                int base2 = run.Base;
                int len2 = run.Len;

                Runs[i] = new Run(base1, len1 + len2);
                if (i == StackSize - 3)
                    Runs[i + 1] = Runs[i + 2];
                StackSize--;

                int k = GallopRight(Array[base2], Array, base1, len1, 0, Comparer);
                base1 += k;
                len1 -= k;
                if (len1 == 0)
                    return;

                len2 = GallopLeft(Array[base1 + len1 - 1], Array, base2, len2, len2 - 1, Comparer);
                if (len2 == 0)
                    return;

                if (len1 <= len2)
                    mergeLo(base1, len1, base2, len2);
                else
                    mergeHi(base1, len1, base2, len2);
            }

            private void mergeLo(int base1, int len1, int base2, int len2)
            {
                // Copy first run into temp array
                T[] a = this.Array; // For performance
                EnsureCapacity(len1);
                System.Array.Copy(a, base1, Buffer, 0, len1);
                int cursor1 = 0;       // Indexes into tmp array
                int cursor2 = base2;   // Indexes int a
                int dest = base1;      // Indexes int a
                                       // Move first element of second run and deal with degenerate cases
                a[dest++] = a[cursor2++];
                if (--len2 == 0)
                {
                    System.Array.Copy(Buffer, cursor1, a, dest, len1);
                    return;
                }
                if (len1 == 1)
                {
                    System.Array.Copy(a, cursor2, a, dest, len2);
                    a[dest + len2] = Buffer[cursor1]; // Last elt of run 1 to end of merge
                    return;
                }
                int minGallop = this.MinGallop;
                while (true)
                {
                    int count1 = 0; // Number of times in a row that first run won
                    int count2 = 0; // Number of times in a row that second run won
                    /*
                     * Do the straightforward thing until (if ever) one run starts
                     * winning consistently.
                     */
                    do
                    {
                        if (Comparer.Compare(a[cursor2], Buffer[cursor1]) < 0)
                        {
                            a[dest++] = a[cursor2++];
                            count2++;
                            count1 = 0;
                            if (--len2 == 0)
                                goto outer;
                        }
                        else
                        {
                            a[dest++] = Buffer[cursor1++];
                            count1++;
                            count2 = 0;
                            if (--len1 == 1)
                                goto outer;
                        }
                    } while ((count1 | count2) < minGallop);
                    /*
                     * One run is winning so consistently that galloping may be a
                     * huge win. So try that, and continue galloping until (if ever)
                     * neither run appears to be winning consistently anymore.
                     */
                    do
                    {
                        count1 = GallopRight(a[cursor2], Buffer, cursor1, len1, 0, Comparer);
                        if (count1 != 0)
                        {
                            System.Array.Copy(Buffer, cursor1, a, dest, count1);
                            dest += count1;
                            cursor1 += count1;
                            len1 -= count1;
                            if (len1 <= 1) // len1 == 1 || len1 == 0
                                goto outer;
                        }
                        a[dest++] = a[cursor2++];
                        if (--len2 == 0)
                            goto outer;
                        count2 = GallopLeft(Buffer[cursor1], a, cursor2, len2, 0, Comparer);
                        if (count2 != 0)
                        {
                            System.Array.Copy(a, cursor2, a, dest, count2);
                            dest += count2;
                            cursor2 += count2;
                            len2 -= count2;
                            if (len2 == 0)
                                goto outer;
                        }
                        a[dest++] = Buffer[cursor1++];
                        if (--len1 == 1)
                            goto outer;
                        minGallop--;
                    } while (count1 >= TimSort.MinGallop | count2 >= TimSort.MinGallop);
                    if (minGallop < 0)
                        minGallop = 0;
                    minGallop += 2;  // Penalize for leaving gallop mode
                }  // End of "outer" loop

            outer:
                this.MinGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field
                if (len1 == 1)
                {
                    System.Array.Copy(a, cursor2, a, dest, len2);
                    a[dest + len2] = Buffer[cursor1]; //  Last elt of run 1 to end of merge
                }
                else if (len1 == 0)
                {
                    throw new InvalidOperationException(
                        "Comparison method violates its general contract!");
                }
                else
                {
                    System.Array.Copy(Buffer, cursor1, a, dest, len1);
                }
            }

            private void mergeHi(int base1, int len1, int base2, int len2)
            {
                // Copy second run into temp array
                T[] a = this.Array; // For performance
                EnsureCapacity(len2);
                System.Array.Copy(a, base2, Buffer, 0, len2);
                int cursor1 = base1 + len1 - 1;  // Indexes into a
                int cursor2 = len2 - 1;          // Indexes into tmp array
                int dest = base2 + len2 - 1;     // Indexes into a
                                                 // Move last element of first run and deal with degenerate cases
                a[dest--] = a[cursor1--];
                if (--len1 == 0)
                {
                    System.Array.Copy(Buffer, 0, a, dest - (len2 - 1), len2);
                    return;
                }
                if (len2 == 1)
                {
                    dest -= len1;
                    cursor1 -= len1;
                    System.Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
                    a[dest] = Buffer[cursor2];
                    return;
                }
                int minGallop = this.MinGallop;    //  "    "       "     "      "
                while (true)
                {
                    int count1 = 0; // Number of times in a row that first run won
                    int count2 = 0; // Number of times in a row that second run won
                    /*
                     * Do the straightforward thing until (if ever) one run
                     * appears to win consistently.
                     */
                    do
                    {
                        if (Comparer.Compare(Buffer[cursor2], a[cursor1]) < 0)
                        {
                            a[dest--] = a[cursor1--];
                            count1++;
                            count2 = 0;
                            if (--len1 == 0)
                                goto outer;
                        }
                        else
                        {
                            a[dest--] = Buffer[cursor2--];
                            count2++;
                            count1 = 0;
                            if (--len2 == 1)
                                goto outer;
                        }
                    } while ((count1 | count2) < minGallop);
                    /*
                     * One run is winning so consistently that galloping may be a
                     * huge win. So try that, and continue galloping until (if ever)
                     * neither run appears to be winning consistently anymore.
                     */
                    do
                    {
                        count1 = len1 - GallopRight(Buffer[cursor2], a, base1, len1, len1 - 1, Comparer);
                        if (count1 != 0)
                        {
                            dest -= count1;
                            cursor1 -= count1;
                            len1 -= count1;
                            System.Array.Copy(a, cursor1 + 1, a, dest + 1, count1);
                            if (len1 == 0)
                                goto outer;
                        }
                        a[dest--] = Buffer[cursor2--];
                        if (--len2 == 1)
                            goto outer;
                        count2 = len2 - GallopLeft(a[cursor1], Buffer, 0, len2, len2 - 1, Comparer);
                        if (count2 != 0)
                        {
                            dest -= count2;
                            cursor2 -= count2;
                            len2 -= count2;
                            System.Array.Copy(Buffer, cursor2 + 1, a, dest + 1, count2);
                            if (len2 <= 1)  // len2 == 1 || len2 == 0
                                goto outer;
                        }
                        a[dest--] = a[cursor1--];
                        if (--len1 == 0)
                            goto outer;
                        minGallop--;
                    } while (count1 >= TimSort.MinGallop | count2 >= TimSort.MinGallop);
                    if (minGallop < 0)
                        minGallop = 0;
                    minGallop += 2;  // Penalize for leaving gallop mode
                }  // End of "outer" loop

            outer:
                this.MinGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field
                if (len2 == 1)
                {
                    dest -= len1;
                    cursor1 -= len1;
                    System.Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
                    a[dest] = Buffer[cursor2];  // Move first elt of run2 to front of merge
                }
                else if (len2 == 0)
                {
                    throw new InvalidOperationException(
                        "Comparison method violates its general contract!");
                }
                else
                {
                    System.Array.Copy(Buffer, 0, a, dest - (len2 - 1), len2);
                }
            }

            private void EnsureCapacity(int minCapacity)
            {
                if (Buffer.Length < minCapacity)
                {
                    // Compute smallest power of 2 > minCapacity
                    int newSize = minCapacity;
                    newSize |= newSize >> 1;
                    newSize |= newSize >> 2;
                    newSize |= newSize >> 4;
                    newSize |= newSize >> 8;
                    newSize |= newSize >> 16;
                    newSize++;
                    if (newSize < 0) // Not bloody likely!
                        newSize = minCapacity;
                    else
                        newSize = System.Math.Min(newSize, Array.Length >> 1);

                    Buffer = new T[newSize];
                }
            }
        }

        private struct Run
        {
            public int Base;
            public int Len;

            public Run(int @base, int len)
            {
                Base = @base;
                Len = len;
            }
        }
    }
}