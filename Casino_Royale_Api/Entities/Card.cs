using System;
using System.Collections.Generic;

namespace Casino_Royale_Api.Models
{
    public class Card
    {
        public string Suit { get; set; }
        private readonly List<string> _suits = new List<string>()
        {
            "Clovers",
            "Diamonds",
            "Hearts",
            "Spades"
        };
        public string Value { get; set; }
        private readonly List<string> _values = new List<string>()
        {
            "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"
        };
        
        public int Weight { get; set; }
        private readonly Random _rnd = new Random();
        

        public Card()
        {
            Suit = GetRandomSuit();
            Value = GetRandomValue();
            Weight = GetWeight();
        }

        private string GetRandomSuit()
        {
            int r = _rnd.Next(_suits.Count);
            return _suits[r];
        }
        private string GetRandomValue()
        {
            int r = _rnd.Next(_values.Count);
            return _values[r];
        }
        
        private int GetWeight()
        {
            int cardWeight;
            if (Value == "J" || Value == "Q" || Value == "K") {
                cardWeight = 10;
            }
            else if (Value == "A") {
                cardWeight = 11;
            }
            else {
                cardWeight = Int32.Parse(Value);
            }
            return cardWeight;
        }
    }
}