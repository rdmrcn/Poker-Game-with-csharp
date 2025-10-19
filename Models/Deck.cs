// Models/Deck.cs
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Poker_Game_with_csharp.Models
{
    public class Deck
    {
        private readonly List<Card> _cards = new();
        private int _index;

        public Deck() => Reset();

        public void Reset()
        {
            _cards.Clear();
            foreach (Suit s in Enum.GetValues(typeof(Suit)))
                for (int r = 2; r <= 14; r++)
                    _cards.Add(new Card((Rank)r, s));
            Shuffle();
        }

        public void Shuffle()
        {
            _index = 0;
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public Card Draw()
        {
            if (_index >= _cards.Count) throw new InvalidOperationException("Deck is empty");
            return _cards[_index++];
        }
    }
}