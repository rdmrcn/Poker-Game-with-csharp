// Game/Player.cs
using System.Collections.Generic;
using Poker_Game_with_csharp.Models;

namespace Poker_Game_with_csharp.Game
{
    public class Player
    {
        public string Name { get; }
        public bool IsBot { get; }
        public int Stack { get; set; }
        public bool Folded { get; set; }
        public bool AllIn => Stack == 0;
        public Card? Hole1 { get; set; }
        public Card? Hole2 { get; set; }

        public Player(string name, bool isBot, int stack)
        {
            Name = name;
            IsBot = isBot;
            Stack = stack;
        }

        public IEnumerable<Card> HoleCards()
        {
            if (Hole1.HasValue) yield return Hole1.Value;
            if (Hole2.HasValue) yield return Hole2.Value;
        }

        public void ClearForNextHand()
        {
            Folded = false;
            Hole1 = null;
            Hole2 = null;
        }
    }
}