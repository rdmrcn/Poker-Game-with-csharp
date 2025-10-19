//// Project structure:
// PokerGame/
// ├─ PokerGame.csproj
// ├─ Program.cs
// ├─ Models/
// │ ├─ Card.cs
// │ └─ Deck.cs
// ├─ Game/
// │ ├─ Player.cs
// │ ├─ BotStrategy.cs
// │ ├─ HandEvaluator.cs
// │ ├─ HandTypes.cs
// │ └─ PokerGame.cs
// └─ Utils/
// └─ Combinatorics.cs
//
// Notes:
// • Texas Hold’em, 1 human vs 1–5 bots. No‑limit style but with a sensible raise cap per street to keep terminal UX smooth.
// • Blinds: 10/20; starting stack: 500 chips. Dealer/big blind rotates each hand.
// • Actions: fold/check/call/raise (bots decide via a simple strength + position heuristic).
// • Showdown uses a full 7‑card evaluator (best of 21 five‑card combos).
// • Save/exit any time between hands.