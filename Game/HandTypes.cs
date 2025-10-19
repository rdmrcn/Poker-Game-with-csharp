// Game/HandTypes.cs
using System;

namespace Poker_Game_with_csharp.Game
{
    public enum HandCategory
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        ThreeKind = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourKind = 7,
        StraightFlush = 8
    }

    public readonly struct HandValue : IComparable<HandValue>
    {
        public readonly HandCategory Category;
        public readonly int[] Keys; // tie-breakers high→low (length 5 preferred)

        public HandValue(HandCategory cat, int[] keys)
        {
            Category = cat;
            Keys = keys;
        }

        public int CompareTo(HandValue other)
        {
            int c = Category.CompareTo(other.Category);
            if (c != 0) return c;
            for (int i = 0; i < Math.Min(Keys.Length, other.Keys.Length); i++)
            {
                int d = Keys[i].CompareTo(other.Keys[i]);
                if (d != 0) return d;
            }
            return 0;
        }

        public override string ToString() => $"{Category} ({string.Join(", ", Keys)})";
    }
}