// Program.cs  (place this at the project root)
using System;
using Poker_Game_with_csharp.Game;

namespace Poker_Game_with_csharp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Terminal Texas Hold'em";

            Console.WriteLine("\n=== Terminal Texas Hold’em ===");
            Console.WriteLine("Medium-level C# console project (Rider friendly)\n");

            var playerName = Ask("Your name? ", s => !string.IsNullOrWhiteSpace(s));
            var bots = AskInt("How many bots (1-5)? ", 1, 5);

            var game = new PokerGame(playerName, bots);
            game.Run();

            Console.WriteLine("\nThanks for playing! ✌️");
        }

        private static string Ask(string prompt, Func<string, bool> ok)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine() ?? string.Empty;
                if (ok(s)) return s.Trim();
            }
        }

        private static int AskInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out var n) && n >= min && n <= max)
                    return n;
            }
        }
    }
}