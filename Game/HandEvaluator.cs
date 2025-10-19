// Game/HandEvaluator.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Poker_Game_with_csharp.Models;
using Poker_Game_with_csharp.Utils;

namespace Poker_Game_with_csharp.Game
{
    public static class HandEvaluator
    {
        // Evaluate best 5-card hand out of 5–7 cards
        public static HandValue Evaluate(IEnumerable<Card> cards)
        {
            var list = cards.ToList();
            if (list.Count < 5)
                throw new ArgumentException("Need at least 5 cards to evaluate a poker hand.");

            var best = new HandValue(HandCategory.HighCard, new[] { 0, 0, 0, 0, 0 });

            foreach (var combo in Combinatorics.Choose(list, 5))
            {
                var hv = Evaluate5(combo);
                if (hv.CompareTo(best) > 0) best = hv;
            }

            return best;
        }

        // Evaluate exactly 5 cards
        private static HandValue Evaluate5(IReadOnlyList<Card> c)
        {
            var ranks = c.Select(x => (int)x.Rank).OrderByDescending(x => x).ToArray();
            var suits = c.Select(x => x.Suit).ToArray();

            bool isFlush = suits.Distinct().Count() == 1;
            int straightHigh = StraightHigh(ranks);
            bool isStraight = straightHigh > 0;

            if (isStraight && isFlush)
                return new HandValue(HandCategory.StraightFlush, new[] { straightHigh, 0, 0, 0, 0 });

            var groups = ranks
                .GroupBy(r => r)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .ToList();

            if (groups[0].Count() == 4)
                return new HandValue(HandCategory.FourKind, new[] { groups[0].Key, groups[1].Key, 0, 0, 0 });

            if (groups[0].Count() == 3 && groups[1].Count() == 2)
                return new HandValue(HandCategory.FullHouse, new[] { groups[0].Key, groups[1].Key, 0, 0, 0 });

            if (isFlush)
                return new HandValue(HandCategory.Flush, ranks);

            if (isStraight)
                return new HandValue(HandCategory.Straight, new[] { straightHigh, 0, 0, 0, 0 });

            if (groups[0].Count() == 3)
            {
                var kickers = groups.Skip(1).Select(g => g.Key).OrderByDescending(x => x).Take(2).ToArray();
                return new HandValue(HandCategory.ThreeKind, new[] { groups[0].Key, kickers[0], kickers[1], 0, 0 });
            }

            if (groups[0].Count() == 2 && groups[1].Count() == 2)
            {
                int highPair = Math.Max(groups[0].Key, groups[1].Key);
                int lowPair = Math.Min(groups[0].Key, groups[1].Key);
                int kicker = groups[2].Key;
                return new HandValue(HandCategory.TwoPair, new[] { highPair, lowPair, kicker, 0, 0 });
            }

            if (groups[0].Count() == 2)
            {
                var kickers = groups.Skip(1).Select(g => g.Key).OrderByDescending(x => x).Take(3).ToArray();
                return new HandValue(HandCategory.OnePair, new[] { groups[0].Key, kickers[0], kickers[1], kickers[2], 0 });
            }

            return new HandValue(HandCategory.HighCard, ranks);
        }

        // return high card of a straight, else 0 (A can be low)
        private static int StraightHigh(int[] ranksDesc)
        {
            var set = new HashSet<int>(ranksDesc);
            if (set.Contains(14)) set.Add(1);   // A as 1 for A-2-3-4-5

            int run = 0, last = int.MaxValue, bestHigh = 0;
            foreach (var r in set.OrderByDescending(x => x))
            {
                if (last == int.MaxValue || r == last - 1) run++;
                else run = 1;

                if (run >= 5) bestHigh = Math.Max(bestHigh, r + 4); // r is low of the 5-run
                last = r;
            }

            return bestHigh;
        }
    }
}
