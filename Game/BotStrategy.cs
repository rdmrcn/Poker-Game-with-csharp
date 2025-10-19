// Game/BotStrategy.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Poker_Game_with_csharp.Models;

namespace Poker_Game_with_csharp.Game
{
    public static class BotStrategy
    {
        private static readonly Random Rng = new();

        // street: 0=preflop, 1=flop, 2=turn, 3=river
        public static string DecideAction(
            Player bot,
            IReadOnlyList<Player> players,
            List<Card> board,
            int toCall,
            int minRaise,
            int currentBet,
            int pot,
            int street)
        {
            double strength = EstimateStrength(bot, board, players.Count);
            double agg = 0.6 + street * 0.1;

            if (strength > 0.80 && bot.Stack > toCall && minRaise > 0 && Rng.NextDouble() < agg)
            {
                int raiseSize = Math.Min(bot.Stack, Math.Max(minRaise, Math.Max(20, pot / 2)));
                return $"raise {raiseSize}";
            }

            if (strength > 0.55)
            {
                if (toCall == 0) return "check";
                if (toCall < Math.Max(20, pot / 5)) return "call";
                return Rng.NextDouble() < 0.30 ? "call" : "fold";
            }

            if (strength > 0.35)
            {
                if (toCall == 0)
                {
                    if (Rng.NextDouble() < 0.25)
                    {
                        int betSize = Math.Min(bot.Stack, Math.Max(20, pot / 4));
                        return $"bet {betSize}";
                    }
                    return "check";
                }

                if (toCall <= Math.Max(10, pot / 10)) return "call";
                return "fold";
            }

            return toCall == 0 ? "check" : "fold";
        }

        private static double EstimateStrength(Player p, List<Card> board, int players)
        {
            int r1 = (int)p.Hole1!.Value.Rank;
            int r2 = (int)p.Hole2!.Value.Rank;
            bool suited = p.Hole1!.Value.Suit == p.Hole2!.Value.Suit;
            int gap = Math.Abs(r1 - r2);
            int high = Math.Max(r1, r2);
            int low = Math.Min(r1, r2);

            double score = high * 0.6 + low * 0.3 - gap * 0.8 + (suited ? 2 : 0);
            if (r1 == r2) score += 6;

            if (board.Count == 0)
            {
                double s = (score - 6) / (26 - 6);
                s = Math.Clamp(s, 0.05, 0.95);
                s *= 1.0 - Math.Min(0.4, (players - 2) * 0.08);
                return s;
            }

            var hv = HandEvaluator.Evaluate(p.HoleCards().Concat(board));
            double baseTier = (int)hv.Category switch
            {
                8 => 0.95, // StraightFlush
                7 => 0.90, // FourKind
                6 => 0.85, // FullHouse
                5 => 0.75, // Flush
                4 => 0.70, // Straight
                3 => 0.55, // Trips
                2 => 0.45, // TwoPair
                1 => 0.35, // OnePair
                _ => 0.25, // HighCard
            };

            baseTier *= 1.0 - Math.Min(0.4, (players - 2) * 0.08);
            return baseTier;
        }
    }
}
