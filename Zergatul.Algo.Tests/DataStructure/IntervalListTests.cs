using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Tests.DataStructure
{
    [TestClass]
    public class IntervalListTests
    {
        private const double Epsilon = 1e-10;

        [TestMethod]
        public void AddOneTest()
        {
            var list = new IntervalList(Epsilon);
            list.Add(0, 100);

            var intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (0, 100));
        }

        [TestMethod]
        public void AddTwoTest()
        {
            var list = new IntervalList(Epsilon);
            list.Add(1, 2);
            list.Add(3, 4);

            var intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 2);
            Assert.IsTrue(intervals[0] == (1, 2));
            Assert.IsTrue(intervals[1] == (3, 4));

            /*****/

            list.Clear();
            list.Add(1, 3);
            list.Add(1, 2);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 3));

            /*****/

            list.Clear();
            list.Add(1, 3);
            list.Add(2, 3);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 3));

            /*****/

            list.Clear();
            list.Add(1, 3);
            list.Add(1, 3);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 3));

            /*****/

            list.Clear();
            list.Add(1, 3);
            list.Add(2, 4);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 4));

            /*****/

            list.Clear();
            list.Add(2, 4);
            list.Add(1, 3);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 4));

            /*****/

            list.Clear();
            list.Add(2, 3);
            list.Add(1, 4);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 4));

            /*****/

            list.Clear();
            list.Add(1, 4);
            list.Add(2, 3);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 4));
        }

        [TestMethod]
        public void AddThreeTest()
        {
            var list = new IntervalList(Epsilon);
            list.Add(1, 2);
            list.Add(3, 4);
            list.Add(5, 6);

            var intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 3);
            Assert.IsTrue(intervals[0] == (1, 2));
            Assert.IsTrue(intervals[1] == (3, 4));
            Assert.IsTrue(intervals[2] == (5, 6));

            /*****/

            list.Clear();
            list.Add(3, 4);
            list.Add(5, 6);
            list.Add(1, 2);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 3);
            Assert.IsTrue(intervals[0] == (1, 2));
            Assert.IsTrue(intervals[1] == (3, 4));
            Assert.IsTrue(intervals[2] == (5, 6));

            /*****/

            list.Clear();
            list.Add(5, 6);
            list.Add(3, 4);
            list.Add(1, 2);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 3);
            Assert.IsTrue(intervals[0] == (1, 2));
            Assert.IsTrue(intervals[1] == (3, 4));
            Assert.IsTrue(intervals[2] == (5, 6));

            /*****/

            list.Clear();
            list.Add(2, 3);
            list.Add(4, 5);
            list.Add(1, 6);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (1, 6));

            /*****/

            list.Clear();
            list.Add(2, 4);
            list.Add(5, 7);
            list.Add(3, 6);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (2, 7));

            /*****/

            list.Clear();
            list.Add(92, 94);
            list.Add(74, 81);
            list.Add(42, 94);

            intervals = list.ToArray();
            Assert.IsTrue(intervals.Length == 1);
            Assert.IsTrue(intervals[0] == (42, 94));
        }

        [TestMethod]
        public void RandomTest()
        {
            var rnd = new Random(0);

            var list = new IntervalList(Epsilon);
            var temp = new List<(int, int)>();
            bool[] array = new bool[100];
            var res = new List<(int, int)>();

            for (int n = 3; n < 10; n++)
            {
                for (int test = 0; test < 100000; test++)
                {
                    Array.Clear(array, 0, array.Length);
                    list.Clear();
                    temp.Clear();
                    res.Clear();

                    for (int i = 0; i < n; i++)
                    {
                        int from = 5 + rnd.Next(0, 90);
                        int to = from + rnd.Next(1, 95 - from + 1);
                        for (int j = from; j < to; j++)
                            array[j] = true;
                        list.Add(from, to);
                        temp.Add((from, to));
                    }

                    // extract intervals
                    int index = 0;
                    while (index < array.Length)
                    {
                        if (array[index])
                        {
                            int from = index;
                            while (array[index])
                                index++;
                            res.Add((from, index));
                        }

                        index++;
                    }

                    var intervals = list.ToArray();
                    Assert.IsTrue(intervals.Length == res.Count);
                    for (int i = 0; i < res.Count; i++)
                        Assert.IsTrue(intervals[i] == res[i]);
                }
            }
        }
    }
}