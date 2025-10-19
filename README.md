# 🃏 Terminal Texas Hold’em Poker  
### 🎮 Medium-Level C# Console Project (Rider / .NET 8)

Welcome to **Terminal Texas Hold’em** , I made this Project to warm up my csharp skills after short holiday break so this is a fully playable **poker game in the console**, coded entirely in **C# (.NET 8)**.  
Challenge up to **five AI opponents** and experience authentic poker flow right inside your terminal window.  

> 🧠 This project demonstrates object-oriented structure, AI decision logic, and 7-card hand evaluation — all in a clean console interface.

---

## 🧩 Features

- ♣️ **Texas Hold’em Rules**
  - Blinds: **10 / 20**, starting stack: **500 chips**
  - 1 human vs 1–5 AI bots
  - Full flow: *preflop → flop → turn → river → showdown*
- 🧠 **AI Bot Strategy**
  - Hand strength estimation  
  - Dynamic aggression per street
- 🃏 **Real 7-card evaluator**
  - Detects *High Card* → *Straight Flush*
- 💬 **Interactive console UI**
  - Type actions directly: `bet 50`, `raise 100`, `fold`, etc.
  - Real-time pot, call amount, and stack display
- 🧮 **Mathematical hand ranking**
  - Uses combinations and LINQ to compute best 5-card hand from 7 cards

---

## ⚙️ Project Structure
PokerGame/
│
├── PokerGame.csproj
├── Program.cs # Entry point (asks name & starts game)
│
├── Models/
│ ├── Card.cs # Rank, Suit, and card display (♠ ♥ ♦ ♣)
│ └── Deck.cs # 52-card deck + Fisher–Yates shuffle
│
├── Game/
│ ├── Player.cs 
│ ├── BotStrategy.cs
│ ├── HandEvaluator.cs 
│ ├── HandTypes.cs
│ └── PokerGame.cs
│
└── Utils/
└── Combinatorics.cs 
