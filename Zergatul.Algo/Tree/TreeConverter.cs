using System.Collections.Generic;

namespace Zergatul.Algo.Tree
{
    public static class TreeConverter
    {
        public static int[][] SortAdjList(int root, int[][] adj)
        {
            var list1 = new List<int>();
            var list2 = new List<int>();
            var buffer = new List<int>();
            int n = adj.Length;
            int[][] result = new int[n][];

            list1.Add(root);

            while (list1.Count > 0)
            {
                list2.Clear();
                foreach (int v in list1)
                {
                    buffer.Clear();
                    foreach (int va in adj[v])
                    {
                        if (result[va] == null)
                        {
                            buffer.Add(va);
                            list2.Add(va);
                        }
                    }
                    result[v] = buffer.ToArray();
                }

                (list1, list2) = (list2, list1);
            }

            return result;
        }

        public static int[][] SortAdjList(int root, List<int>[] adj)
        {
            var list1 = new List<int>();
            var list2 = new List<int>();
            var buffer = new List<int>();
            int n = adj.Length;
            int[][] result = new int[n][];

            list1.Add(root);

            while (list1.Count > 0)
            {
                list2.Clear();
                foreach (int v in list1)
                {
                    buffer.Clear();
                    foreach (int va in adj[v])
                    {
                        if (result[va] == null)
                        {
                            buffer.Add(va);
                            list2.Add(va);
                        }
                    }
                    result[v] = buffer.ToArray();
                }

                (list1, list2) = (list2, list1);
            }

            return result;
        }
    }
}