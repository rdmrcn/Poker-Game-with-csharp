// Utils/Combinatorics.cs
using System.Collections.Generic;
using System.Linq;

namespace Poker_Game_with_csharp.Utils
{
    public static class Combinatorics
    {
        // Returns all size-k combinations (as lists) from items (0..n-1)
        public static IEnumerable<List<T>> Choose<T>(IReadOnlyList<T> items, int k)
        {
            if (k < 0 || k > items.Count) yield break;
            var idx = Enumerable.Range(0, k).ToArray();
            while (true)
            {
                yield return idx.Select(i => items[i]).ToList();
                int i = k - 1;
                while (i >= 0 && idx[i] == i + items.Count - k) i--;
                if (i < 0) break;
                idx[i]++;
                for (int j = i + 1; j < k; j++) idx[j] = idx[j - 1] + 1;
            }
        }
    }
}