// Game/PokerGame.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Poker_Game_with_csharp.Models;

namespace Poker_Game_with_csharp.Game
{
    public class PokerGame
    {
        private readonly List<Player> _players = new();
        private readonly Deck _deck = new();
        private readonly List<Card> _board = new();
        private int _dealerIndex;

        private const int StartingStack = 500;
        private const int SmallBlind = 10;
        private const int BigBlind = 20;

        public PokerGame(string humanName, int botCount)
        {
            _players.Add(new Player(humanName, false, StartingStack));
            for (int i = 1; i <= botCount; i++)
                _players.Add(new Player($"Bot{i}", true, StartingStack));
            _dealerIndex = 0;
        }

        public void Run()
        {
            while (_players.Count(p => p.Stack > 0) >= 2)
            {
                foreach (var p in _players) p.ClearForNextHand();
                _board.Clear();
                _deck.Reset();

                _dealerIndex = NextIndex(_dealerIndex);

                int pot = 0;
                var bet = _players.ToDictionary(p => p, _ => 0);

                var sb = _players[NextIndex(_dealerIndex)];
                var bb = _players[NextIndex(_dealerIndex, 2)];
                pot += PostBlind(sb, SmallBlind, bet);
                pot += PostBlind(bb, BigBlind, bet);

                foreach (var p in _players)
                {
                    if (p.Stack == 0) { p.Folded = true; continue; }
                    p.Hole1 = _deck.Draw();
                    p.Hole2 = _deck.Draw();
                }

                Console.WriteLine("\n———————— NEW HAND ————————");
                Console.WriteLine($"Dealer: {_players[_dealerIndex].Name} | SB: {sb.Name} | BB: {bb.Name}");

                if (!BettingRound(startIndex: NextIndex(_dealerIndex, 3), ref pot, bet, street: 0))
                { EndHand(pot, reveal: false); continue; }

                Burn(); _board.Add(_deck.Draw()); _board.Add(_deck.Draw()); _board.Add(_deck.Draw());
                Console.WriteLine($"Flop: {BoardString()}");
                ResetBets(bet);
                if (!BettingRound(startIndex: NextIndex(_dealerIndex, 1), ref pot, bet, street: 1))
                { EndHand(pot, reveal: false); continue; }

                Burn(); _board.Add(_deck.Draw());
                Console.WriteLine($"Turn: {BoardString()}");
                ResetBets(bet);
                if (!BettingRound(startIndex: NextIndex(_dealerIndex, 1), ref pot, bet, street: 2))
                { EndHand(pot, reveal: false); continue; }

                Burn(); _board.Add(_deck.Draw());
                Console.WriteLine($"River: {BoardString()}");
                ResetBets(bet);
                if (!BettingRound(startIndex: NextIndex(_dealerIndex, 1), ref pot, bet, street: 3))
                { EndHand(pot, reveal: false); continue; }

                EndHand(pot, reveal: true);

                _players.RemoveAll(p => p.Stack == 0);
                if (_players.All(p => p.IsBot)) break;

                if (!AskYesNo("Play another hand? (y/n): ")) break;
            }
        }

        private void Burn() => _deck.Draw();
        private string BoardString() => _board.Count == 0 ? "(preflop)" : string.Join(" ", _board.Select(c => c.ToString()));

        private static int PostBlind(Player p, int amount, Dictionary<Player, int> bet)
        {
            int posted = Math.Min(p.Stack, amount);
            p.Stack -= posted;
            bet[p] += posted;
            return posted;
        }

        private static void ResetBets(Dictionary<Player, int> bet)
        {
            var keys = bet.Keys.ToList();
            foreach (var k in keys) bet[k] = 0;
        }

        private bool BettingRound(int startIndex, ref int potRef, Dictionary<Player, int> bet, int street)
        {
            int raisesLeft = 3;
            int currentBet = bet.Values.Max();
            int minRaise = BigBlind;

            int i = startIndex;

            while (_players.Count(p => !p.Folded) > 1)
            {
                var p = _players[i];
                if (!p.Folded && !p.AllIn)
                {
                    int toCall = currentBet - bet[p];
                    string action = p.IsBot
                        ? BotStrategy.DecideAction(p, _players.Where(pp => !pp.Folded).ToList(), _board, toCall, raisesLeft > 0 ? minRaise : 0, currentBet, potRef, street)
                        : ReadHumanAction(p, toCall, raisesLeft > 0 ? minRaise : 0, currentBet, potRef);

                    if (!ProcessAction(p, action, ref potRef, bet, ref currentBet, ref minRaise, ref raisesLeft))
                        return false;

                    if (AllBetsMatched(bet, currentBet))
                        break;
                }

                if (_players.Count(pp => !pp.Folded) <= 1) break;
                i = NextIndex(i);
            }

            foreach (var kv in bet.ToList()) { potRef += kv.Value; bet[kv.Key] = 0; }
            return true;
        }

        private bool AllBetsMatched(Dictionary<Player, int> bet, int current)
            => _players.Where(p => !p.Folded && !p.AllIn).All(p => bet[p] == current);

        private string ReadHumanAction(Player p, int toCall, int minRaise, int currentBet, int pot)
        {
            while (true)
            {
                Console.WriteLine($"\nYour cards: {string.Join(" ", p.HoleCards().Select(c => c.ToString()))}");
                Console.WriteLine($"Board: {BoardString()} | Pot: {pot} | To call: {toCall} | Stack: {p.Stack}");
                Console.Write("Action [check/call, bet X, raise X, fold, info, help]: ");
                var line = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

                if (line == "help")
                {
                    Console.WriteLine("check/call: match current bet. bet/raise X: put chips in (X >= min raise). fold: give up. info: hand strength.");
                    continue;
                }
                if (line == "info")
                {
                    var hv = HandEvaluator.Evaluate(p.HoleCards().Concat(_board));
                    Console.WriteLine($"Your best hand: {hv}");
                    continue;
                }
                if (line == "fold") return "fold";
                if (line.StartsWith("check"))
                {
                    if (toCall == 0) return "check";
                    Console.WriteLine("Cannot check — there’s a bet.");
                    continue;
                }
                if (line.StartsWith("call"))
                {
                    if (toCall > 0) return "call";
                    Console.WriteLine("Nothing to call.");
                    continue;
                }
                if (line.StartsWith("bet ") || line.StartsWith("raise "))
                {
                    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2 && int.TryParse(parts[1], out var x))
                    {
                        if (x < minRaise) { Console.WriteLine($"Minimum raise/bet is {minRaise}."); continue; }
                        return (line.StartsWith("bet ") ? "bet " : "raise ") + x;
                    }
                }
                Console.WriteLine("Invalid command.");
            }
        }

        private bool ProcessAction(Player p, string action, ref int potRef, Dictionary<Player, int> bet, ref int currentBet, ref int minRaise, ref int raisesLeft)
        {
            int toCall = currentBet - bet[p];

            switch (action)
            {
                case "fold":
                    p.Folded = true; Console.WriteLine($"{p.Name} folds."); break;

                case "check":
                    Console.WriteLine($"{p.Name} checks."); break;

                case "call":
                    {
                        int pay = Math.Min(p.Stack, toCall);
                        p.Stack -= pay; bet[p] += pay;
                        Console.WriteLine($"{p.Name} calls {pay}.");
                        break;
                    }

                default:
                    if (action.StartsWith("bet ") || action.StartsWith("raise "))
                    {
                        var x = int.Parse(action.Split(' ')[1]);
                        int target = Math.Min(p.Stack + bet[p], currentBet + x); // supports all-in
                        int need = target - bet[p];
                        if (need < 0) need = 0;
                        need = Math.Min(need, p.Stack);

                        p.Stack -= need; bet[p] += need; currentBet = bet[p];
                        minRaise = Math.Max(minRaise, x);
                        raisesLeft = Math.Max(0, raisesLeft - 1);
                        Console.WriteLine($"{p.Name} {(action.StartsWith("bet ") ? "bets" : "raises to")} {currentBet}.");
                    }
                    break;
            }
            return true;
        }

        private void EndHand(int pot, bool reveal)
        {
            var active = _players.Where(p => !p.Folded).ToList();
            if (active.Count == 1)
            {
                active[0].Stack += pot;
                Console.WriteLine($"\n{active[0].Name} wins the pot of {pot} (everyone folded).\n");
                return;
            }

            var ranks = new List<(Player p, HandValue hv)>();
            foreach (var p in active)
                ranks.Add((p, HandEvaluator.Evaluate(p.HoleCards().Concat(_board))));
            ranks.Sort((a, b) => a.hv.CompareTo(b.hv));
            var winner = ranks[^1];

            if (reveal)
            {
                Console.WriteLine("\nShowdown:");
                foreach (var (p, hv) in ranks)
                    Console.WriteLine($"{p.Name}: {string.Join(" ", p.HoleCards().Select(c => c.ToString()))} -> {hv}");
            }

            winner.p.Stack += pot;
            Console.WriteLine($"\n{winner.p.Name} wins {pot} with {winner.hv.Category}!\n");
            Console.WriteLine($"Stacks: {string.Join(" | ", _players.Select(pl => $"{pl.Name}:{pl.Stack}"))}\n");
        }

        private int NextIndex(int i, int offset = 1)
        {
            int n = _players.Count;
            do { i = (i + 1) % n; } while (_players[i].Stack == 0 && _players.Any(p => p.Stack > 0));
            return i;
        }

        private static bool AskYesNo(string prompt)
        {
            Console.Write(prompt);
            var s = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
            return s == "y" || s == "yes";
        }
    }
}
